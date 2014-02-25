/*
 * Process Hacker - 
 *   memory editor window
 * 
 * Copyright (C) 2008 wj32
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
using System.Collections.Generic;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker
{
    public partial class MemoryEditor : Form
    {
        public static MemoryEditor ReadWriteMemory(int pid, IntPtr address, int size, bool RO)
        {
            return ReadWriteMemory(pid, address, size, RO, 
                new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f) { }));
        }

        public static MemoryEditor ReadWriteMemory(int pid, IntPtr address, int size, bool RO, 
            Program.MemoryEditorInvokeAction action)
        {
            try
            {
                MemoryEditor ed = null;

                ed = Program.GetMemoryEditor(pid, address, size,
                    new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f)
                    {
                        if (!f.IsDisposed)
                        {
                            f.ReadOnly = RO;
                            f.Show();
                            action(f);
                            f.Activate();
                        }
                    }));

                return ed;
            }
            catch
            {
                return null;
            }
        }

        private int _pid;
        private long _length;
        private IntPtr _address;
        private byte[] _data;

        public string Id
        {
            get { return _pid.ToString() + "-" + _address.ToString() + "-" + _length.ToString(); }
        }

        public MemoryEditor(int PID, IntPtr Address, long Length)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _pid = PID;
            _address = Address;
            _length = Length;

            Program.MemoryEditors.Add(this.Id, this);

            if (Program.ProcessProvider.Dictionary.ContainsKey(_pid))
            {
                this.Text = Program.ProcessProvider.Dictionary[_pid].Name + " (PID " + _pid.ToString() +
                    "), " + Utils.FormatAddress(_address) + "-" +
                    Utils.FormatAddress(_address.Increment(_length)) + " - Memory Editor";
            }
            else
            {
                this.Text = "PID " + _pid.ToString() + " - Memory Editor";
            }

            try
            {
                ReadMemory();
            }
            catch
            {
                this.Visible = false;
                MessageBox.Show("Could not read process memory:\n\n" + Win32.GetLastErrorMessage(), 
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            hexBoxMemory.Select();
        }

        private void MemoryEditor_Load(object sender, EventArgs e)
        {
            Program.UpdateWindowMenu(windowMenuItem, this);

            this.Size = Settings.Instance.MemoryWindowSize;
            this.SetPhParent(false);
        }

        private void MemoryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;

            if (this.WindowState == FormWindowState.Normal)
                Settings.Instance.MemoryWindowSize = this.Size;
        }

        public bool ReadOnly
        {
            get { return hexBoxMemory.ReadOnly; }
            set
            {
                hexBoxMemory.ReadOnly = value;

                try
                {
                    if (!value)
                    {
                        writeMenuItem.Enabled = true;
                        utilitiesButtonMemory.Enabled = true;
                    }
                    else
                    {
                        writeMenuItem.Enabled = false;
                        utilitiesButtonMemory.Enabled = false;
                    }
                }
                catch
                { }
            }
        }

        public void Select(long start, long length)
        {
            hexBoxMemory.Select(start, length);
        }

        private void ReadMemory()
        {
            using (var phandle = new ProcessHandle(_pid, Program.MinProcessReadMemoryRights))
            {
                _data = new byte[_length];

                if (phandle.ReadMemory(_address, _data, (int)_length) == 0)
                    throw new Exception("Unknown error.");

                hexBoxMemory.ByteProvider = new Be.Windows.Forms.DynamicByteProvider(_data);
            }
        }

        private void WriteMemory()
        {
            using (var phandle = new ProcessHandle(_pid, Program.MinProcessWriteMemoryRights))
            {
                for (long i = 0; i < hexBoxMemory.ByteProvider.Length; i++)
                {
                    _data[i] = hexBoxMemory.ByteProvider.ReadByte(i);
                }

                if (phandle.WriteMemory(_address, _data) == 0)
                    throw new Exception("Unknown error.");
            }
        }

        private void buttonValues_Click(object sender, EventArgs e)
        {
            string values = "";
            InformationBox valuesForm;
            long addr = hexBoxMemory.SelectionStart;
            long space = hexBoxMemory.ByteProvider.Length - hexBoxMemory.SelectionStart;

            if (space >= 1)
                values += "\r\n\r\n8-bit Integer: " + hexBoxMemory.ByteProvider.ReadByte(addr).ToString();

            if (space >= 2)
            {
                ushort value = 0;

                value = (ushort)(hexBoxMemory.ByteProvider.ReadByte(addr + 1) << 8 | hexBoxMemory.ByteProvider.ReadByte(addr));
                values += "\r\n\r\n16-bit Integer, little-endian, unsigned: " + value.ToString();
                values += "\r\n16-bit Integer, little-endian, signed: " + ((short)value).ToString();

                value = (ushort)(hexBoxMemory.ByteProvider.ReadByte(addr) << 8 | hexBoxMemory.ByteProvider.ReadByte(addr + 1));
                values += "\r\n16-bit Integer, big-endian, unsigned: " + value.ToString();
                values += "\r\n16-bit Integer, big-endian, signed: " + ((short)value).ToString();
            }

            if (space >= 4)
            {
                uint value = 0;

                value = ((uint)hexBoxMemory.ByteProvider.ReadByte(addr + 3) << 24) +
                    ((uint)hexBoxMemory.ByteProvider.ReadByte(addr + 2) << 16) +
                    ((uint)hexBoxMemory.ByteProvider.ReadByte(addr + 1) << 8) +
                    ((uint)hexBoxMemory.ByteProvider.ReadByte(addr));
                values += "\r\n\r\n32-bit Integer, little-endian, unsigned: " + value.ToString();
                values += "\r\n32-bit Integer, little-endian, signed: " + ((int)value).ToString();

                value = ((uint)hexBoxMemory.ByteProvider.ReadByte(addr) << 24) +
                     ((uint)hexBoxMemory.ByteProvider.ReadByte(addr + 1) << 16) +
                     ((uint)hexBoxMemory.ByteProvider.ReadByte(addr + 2) << 8) +
                     ((uint)hexBoxMemory.ByteProvider.ReadByte(addr + 3));
                values += "\r\n32-bit Integer, big-endian, unsigned: " + value.ToString();
                values += "\r\n32-bit Integer, big-endian, signed: " + ((int)value).ToString();
            }

            if (space >= 8)
            {
                ulong value = 0;

                value = ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 7) << 56) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 6) << 48) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 5) << 40) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 4) << 32) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 3) << 24) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 2) << 16) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 1) << 8) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr));
                values += "\r\n\r\n64-bit Integer, little-endian, unsigned: " + value.ToString();
                values += "\r\n64-bit Integer, little-endian, signed: " + ((long)value).ToString();

                value = ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr) << 56) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 1) << 48) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 2) << 40) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 3) << 32) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 4) << 24) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 5) << 16) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 6) << 8) |
                    ((ulong)hexBoxMemory.ByteProvider.ReadByte(addr + 7));
                values += "\r\n64-bit Integer, big-endian, unsigned: " + value.ToString();
                values += "\r\n64-bit Integer, big-endian, signed: " + ((long)value).ToString();
            }

            valuesForm = new InformationBox(values.Trim());
            valuesForm.ShowDialog();
        }

        private void UpdateHexBoxSelectionInfo()
        {
            labelHexSelection.Text =
                string.Format("Selection: 0x{0:x}, length 0x{1:x}",
                hexBoxMemory.SelectionStart, hexBoxMemory.SelectionLength);
        }

        private void hexBoxMemory_SelectionLengthChanged(object sender, EventArgs e)
        {
            UpdateHexBoxSelectionInfo();
        }

        private void hexBoxMemory_SelectionStartChanged(object sender, EventArgs e)
        {
            UpdateHexBoxSelectionInfo();
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            try
            {
                _data = null;
                ReadMemory();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to read process memory", ex);
            }
        }

        private void writeMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WriteMemory();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to write to process memory", ex);
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            try
            {
                using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                {
                    string fileName = phandle.GetImageFileName();

                    sfd.FileName = fileName.Substring(fileName.LastIndexOf('\\') + 1) + "-" + Utils.FormatAddress(_address) + ".bin";
                }
            }
            catch
            {
                sfd.FileName = "memory.bin";
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                for (long i = 0; i < hexBoxMemory.ByteProvider.Length; i++)
                {
                    _data[i] = hexBoxMemory.ByteProvider.ReadByte(i);
                }

                System.IO.File.WriteAllBytes(sfd.FileName, _data);
            }
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textGoTo_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonGoToMemory;
        }

        private void textGoTo_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }

        private void textSearchMemory_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonNextFind;
        }

        private void textSearchMemory_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }

        private void textSearchMemory_TextChanged(object sender, EventArgs e)
        {
            try
            {
                byte[] data = new byte[textSearchMemory.Text.Length];

                for (int i = 0; i < textSearchMemory.Text.Length; i++)
                    data[i] = (byte)textSearchMemory.Text[i];

                hexBoxMemory.Find(data, hexBoxMemory.SelectionStart + hexBoxMemory.SelectionLength);
            }
            catch { }
        }

        private void buttonNextFind_Click(object sender, EventArgs e)
        {
            hexBoxMemory.Select(hexBoxMemory.SelectionStart + hexBoxMemory.SelectionLength, 0);
            textSearchMemory_TextChanged(null, null);
        }

        private void buttonTopFind_Click(object sender, EventArgs e)
        {
            hexBoxMemory.Select(0, 1);
        }

        private void buttonGoToMemory_Click(object sender, EventArgs e)
        {
            try
            {
                int location = (int)BaseConverter.ToNumberParse(textGoTo.Text);

                if (location < hexBoxMemory.ByteProvider.Length)
                    hexBoxMemory.Select(location, 1);
            }
            catch { }
        }

        private void buttonStruct_Click(object sender, EventArgs e)
        {
            int selectionStart = (int)hexBoxMemory.SelectionStart;

            List<string> structNames = new List<string>(Program.Structs.Keys);

            structNames.Sort();

            ListPickerWindow lpw = new ListPickerWindow(structNames.ToArray());

            lpw.Text = "Select a Struct";

            if (lpw.ShowDialog() == DialogResult.OK)
            {
                if (Program.Structs.ContainsKey(lpw.SelectedItem))
                {
                    // stupid TreeViewAdv only works on the one thread
                    Program.HackerWindow.BeginInvoke(new MethodInvoker(delegate
                        {
                            StructWindow sw = new StructWindow(_pid, (_address.Increment(selectionStart)),
                                Program.Structs[lpw.SelectedItem]);

                            try
                            {
                                sw.Show();
                                sw.Activate();
                            }
                            catch
                            { }
                        }));
                }
            }
        }
    }
}
