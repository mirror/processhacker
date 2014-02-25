/*
 * Process Hacker - 
 *   handle properties window
 *
 * Copyright (C) 2009 wj32
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Ui
{
    public partial class HandlePropertiesWindow : Form
    {
        public delegate void HandlePropertiesDelegate(Control objectGroup, string name, string typeName);

        public event HandlePropertiesDelegate HandlePropertiesCallback;

        private string _name, _typeName;
        private NativeHandle _objectHandle;

        public HandlePropertiesWindow(SystemHandleEntry handle)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    e.Handled = true;
                }
            };

            var handleInfo = handle.GetHandleInfo();

            textName.Text = _name = handleInfo.BestName;
            if (textName.Text == "")
                textName.Text = "(unnamed object)";
            textType.Text = _typeName = handleInfo.TypeName;
            textAddress.Text = "0x" + handle.Object.ToString("x");
            textGrantedAccess.Text = "0x" + handle.GrantedAccess.ToString("x");

            if (handle.GrantedAccess != 0)
            {
                try
                {
                    Type accessEnumType = NativeTypeFactory.GetAccessType(handleInfo.TypeName);

                    textGrantedAccess.Text += " (" +
                        NativeTypeFactory.GetAccessString(accessEnumType, handle.GrantedAccess) +
                        ")";
                }
                catch (NotSupportedException)
                { }
            }

            var basicInfo = handle.GetBasicInfo();

            labelReferences.Text = "References: " + (basicInfo.PointerCount - 1).ToString();
            labelHandles.Text = "Handles: " + basicInfo.HandleCount.ToString();
            labelPaged.Text = "Paged: " + basicInfo.PagedPoolUsage.ToString();
            labelNonPaged.Text = "Non-Paged: " + basicInfo.NonPagedPoolUsage.ToString();
        }

        private void HandlePropertiesWindow_Load(object sender, EventArgs e)
        {
            if (HandlePropertiesCallback != null)
            {
                try
                {
                    HandlePropertiesCallback(groupObjectInfo, _name, _typeName);
                }
                catch
                { }

                if (groupObjectInfo.Controls.Count == 0)
                {
                    groupObjectInfo.Visible = false;
                }
                else if (groupObjectInfo.Controls.Count == 1)
                {
                    Control control = groupObjectInfo.Controls[0];

                    // If it's a user control, dock it.
                    if (control is UserControl)
                    {
                        control.Dock = DockStyle.Fill;
                        control.Margin = new Padding(3);
                    }
                    else
                    {
                        control.Location = new System.Drawing.Point(10, 20);
                    }
                }
            }

            if (this.ObjectHandle == null)
                buttonPermissions.Visible = false;
        }

        public NativeHandle ObjectHandle
        {
            get { return _objectHandle; }
            set { _objectHandle = value; }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPermissions_Click(object sender, EventArgs e)
        {
            if (_objectHandle != null)
            {
                try
                {
                    SecurityEditor.EditSecurity(
                        this,
                        SecurityEditor.GetSecurableWrapper(_objectHandle),
                        _name,
                        NativeTypeFactory.GetAccessEntries(NativeTypeFactory.GetObjectType(_typeName))
                        );
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to edit security: " + ex.Message, "Security Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
