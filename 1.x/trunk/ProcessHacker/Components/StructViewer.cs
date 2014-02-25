/*
 * Process Hacker - 
 *   struct viewer control
 * 
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Structs;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class StructViewer : UserControl
    {
        private StructModel _model = new StructModel();
        int _pid;
        IntPtr _address;
        StructDef _struct;

        public StructViewer(int pid, IntPtr address, StructDef struc)
        {
            InitializeComponent();

            _pid = pid;
            _address = address;
            _struct = struc;
            treeStruct.Model = _model;
            treeStruct.ContextMenu = menuStruct;

            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, treeStruct);

            try
            {
                FieldValue[] values;

                _struct.Offset = address;
                _struct.IOProvider = new ProcessMemoryIO(pid);
                _struct.Structs = Program.Structs;
                values = _struct.Read();

                _model.Nodes.Add(new StructNode(new FieldValue() 
                { Name = "Struct", FieldType = FieldType.StringUTF16, Value = "" }));

                foreach (FieldValue val in values)
                    this.AddNode(_model.Nodes[0], val);

                treeStruct.Root.Children[0].IsExpanded = true;
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to view the struct", ex);
                this.Error = true;
            }
        }

        private void AddNode(Node node, FieldValue value)
        {
            StructNode newNode = new StructNode(value);

            if (value.Value is FieldValue[])
            {
                foreach (FieldValue val in (FieldValue[])value.Value)
                    AddNode(newNode, val);
            }

            node.Nodes.Add(newNode);
        }

        public bool Error { get; private set; }

        private void menuStruct_Popup(object sender, EventArgs e)
        {
            if (treeStruct.SelectedNodes.Count == 0)
            {
                copyMenuItem.Enabled = false;
            }
            else
            {
                copyMenuItem.Enabled = true;
            }

            decMenuItem.Checked = false;
            hexMenuItem.Checked = false;

            if (_model.IntegerDisplayBase == IntegerDisplayBase.Decimal)
                decMenuItem.Checked = true;
            else if (_model.IntegerDisplayBase == IntegerDisplayBase.Hexadecimal)
                hexMenuItem.Checked = true;
        }

        private void decMenuItem_Click(object sender, EventArgs e)
        {
            _model.IntegerDisplayBase = IntegerDisplayBase.Decimal;
            _model.OnStructureChanged(new TreePathEventArgs(new TreePath()));
        }

        private void hexMenuItem_Click(object sender, EventArgs e)
        {
            _model.IntegerDisplayBase = IntegerDisplayBase.Hexadecimal;
            _model.OnStructureChanged(new TreePathEventArgs(new TreePath()));
        }
    }

    public enum IntegerDisplayBase
    {
        Decimal,
        Hexadecimal
    }

    public class StructModel : TreeModel
    {
        public IntegerDisplayBase IntegerDisplayBase { get; set; }
    }

    public class StructNode : Node
    {
        private FieldValue _value;

        public StructNode(FieldValue value)
        {
            _value = value;
        }

        public string Name
        {
            get { return _value.Name; }
        }

        public string Value
        {
            get
            {
                FieldType type = _value.FieldType & (~FieldType.Array) & (~FieldType.Pointer);

                if ((_value.FieldType & FieldType.Array) != 0)
                {
                    int memberCount = ((FieldValue[])_value.Value).Length;

                    if (_value.StructName != null)
                        return _value.StructName + "[" + memberCount.ToString() +  "]";
                    else
                        return type.ToString() + "[" + memberCount.ToString() + "]";
                }

                if (_value.StructName != null)
                    return _value.StructName;

                if (_value.Value == null)
                    return "null";

                IntegerDisplayBase b = (this.FindModel() as StructModel).IntegerDisplayBase;
                string formatStr = "{0:d}";

                switch (b)
                {
                    case IntegerDisplayBase.Decimal:
                        formatStr = "{0:d}";
                        break;
                    case IntegerDisplayBase.Hexadecimal:
                        formatStr = "0x{0:x}";
                        break;
                }

                switch (type)
                {
                    case FieldType.Bool32:
                    case FieldType.Bool8:
                        return ((bool)_value.Value) ? "True" : "False";
                    case FieldType.CharASCII:
                    case FieldType.CharUTF16:
                        return ((char)_value.Value).ToString();
                    case FieldType.Double:
                    case FieldType.Single:
                        return ((double)_value.Value).ToString();
                    case FieldType.Int16:
                    case FieldType.Int32:
                    case FieldType.Int64:
                    case FieldType.Int8:
                    case FieldType.UInt16:
                    case FieldType.UInt32:
                    case FieldType.UInt64:
                    case FieldType.UInt8:
                        return string.Format(formatStr, long.Parse(_value.Value.ToString()));
                    case FieldType.PVoid:
                        return Utils.FormatAddress(long.Parse(_value.Value.ToString()).ToIntPtr());
                    case FieldType.StringASCII:
                    case FieldType.StringUTF16:
                        return _value.Value.ToString();
                    default:
                        return _value.Value.ToString();
                }
            }
        }
    }
}
