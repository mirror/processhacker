/*
 * Process Hacker - 
 *   thread properties window
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
using System.Reflection;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Symbols;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class ThreadWindow : Form
    {
        private int _pid;
        private int _tid;
        private ProcessHandle _phandle;
        private ThreadHandle _thandle;
        private SymbolProvider _symbols;

        public const string DisplayFormat = "0x{0:x8}";

        public string[] Registers = 
            new string[] { "eax", "ebx", "ecx", "edx", "esi", "edi", "esp", "ebp", "eip", "cs", "ds", "es", "fs", "gs", "ss" };

        public string Id
        {
            get { return _pid + "-" + _tid; }
        }

        public ThreadWindow(int PID, int TID, SymbolProvider symbols)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            listViewCallStack_SelectedIndexChanged(null, null);

            _pid = PID;
            _tid = TID;
            _symbols = symbols;

            this.Text = Program.ProcessProvider.Dictionary[_pid].Name + " (PID " + _pid.ToString() +
                ") - Thread " + _tid.ToString();

            PropertyInfo property = typeof(ListView).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance);

            property.SetValue(listViewCallStack, true, null);

            listViewCallStack.ContextMenu = listViewCallStack.GetCopyMenu();

            try
            {
                using (ThreadHandle thandle = new ThreadHandle(TID, Program.MinThreadQueryRights))
                {
                    try
                    {
                        using (TokenHandle token = thandle.GetToken(TokenAccess.Query))
                        {
                            labelThreadUser.Text = "Username: " + token.GetUser().GetName(true);
                            buttonToken.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        buttonToken.Enabled = false;

                        if (ex.Message.StartsWith("An attempt was made"))
                        {
                            labelThreadUser.Text = "Username: (Not Impersonating)"; 
                        }
                        else
                        {
                            labelThreadUser.Text = "Username: (" + ex.Message + ")";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                labelThreadUser.Text = "Username: (" + ex.Message + ")";
                buttonToken.Enabled = false;
            }

            try
            {
                if (KProcessHacker.Instance != null)
                {
                    _phandle = new ProcessHandle(_pid, Program.MinProcessReadMemoryRights);
                }
                else
                {
                    _phandle = new ProcessHandle(_pid,
                        ProcessAccess.QueryInformation | ProcessAccess.VmRead
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open process:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.Close();

                return;
            }

            try
            {
                if (KProcessHacker.Instance != null)
                {
                    _thandle = new ThreadHandle(_tid,
                        Program.MinThreadQueryRights | ThreadAccess.SuspendResume
                        );
                }
                else
                {
                    _thandle = new ThreadHandle(_tid, ThreadAccess.GetContext | ThreadAccess.SuspendResume);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open thread:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.Close();

                return;
            }
        }

        private void ThreadWindow_Load(object sender, EventArgs e)
        {
            listViewCallStack.SetTheme("explorer");
            listViewCallStack.KeyDown +=
                (sender_, e_) =>
                {
                    if (e_.Control && e_.KeyCode == Keys.A) Misc.SelectAll(listViewCallStack.Items);
                    if (e_.Control && e_.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listViewCallStack, -1);
                };

            this.WalkCallStack();

            this.Size = Properties.Settings.Default.ThreadWindowSize;
            ColumnSettings.LoadSettings(Properties.Settings.Default.CallStackColumns, listViewCallStack);
        }

        private void ThreadWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ThreadWindowSize = this.Size;
            Properties.Settings.Default.CallStackColumns = ColumnSettings.SaveSettings(listViewCallStack);
            _symbols = null;
        }

        private void AddOrModify(ListView lv, ListViewItem item)
        {
            ListViewItem existing = null;
            bool exists = false;

            foreach (ListViewItem it in lv.Items)
            {
                if (it.Text == item.Text)
                {
                    exists = true;
                    existing = it;

                    break;
                }
            }

            if (exists)
            {
                existing.SubItems[1].Text = item.SubItems[1].Text;
            }
            else
            {
                lv.Items.Add(item);  
            }
        }

        private int BytesToInt32(byte[] b)
        {
            return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | (b[3] << 0);
        }

        private unsafe void WalkCallStack()
        {
            var context = new Context();

            context.ContextFlags = ContextFlags.All;

            Win32.SuspendThread(_thandle);

            if (KProcessHacker.Instance != null)
            {
                try
                {
                    KProcessHacker.Instance.KphGetContextThread(_thandle, &context);
                    WalkCallStack(context);
                }
                catch
                { }
            }
            else
            {
                if (Win32.GetThreadContext(_thandle, ref context))
                {
                    WalkCallStack(context);
                }
            }

            Win32.ResumeThread(_thandle);
        }

        private unsafe void WalkCallStack(Context context)
        {
            /*  [ebp+8]... = args   
             *  [ebp+4] = ret addr  
             *  [ebp] = old ebp
             */
            listViewCallStack.BeginUpdate();
            listViewCallStack.Items.Clear();

            var stackFrame = new StackFrame64();

            stackFrame.AddrPC.Mode = AddressMode.AddrModeFlat;
            stackFrame.AddrPC.Offset = context.Eip;
            stackFrame.AddrStack.Mode = AddressMode.AddrModeFlat;
            stackFrame.AddrStack.Offset = context.Esp;
            stackFrame.AddrFrame.Mode = AddressMode.AddrModeFlat;
            stackFrame.AddrFrame.Offset = context.Ebp;
            
            while (true)
            {
                try
                {
                    Win32.ReadProcessMemoryProc64 readMemoryProc = null;

                    if (KProcessHacker.Instance != null)
                    {
                        readMemoryProc = new Win32.ReadProcessMemoryProc64(
                            delegate(int processHandle, ulong baseAddress, byte* buffer, int size, out int bytesRead)
                            {
                                return KProcessHacker.Instance.KphReadVirtualMemorySafe(
                                    ProcessHandle.FromHandle(processHandle), (int)baseAddress, buffer, size, out bytesRead);
                            });
                    }

                    if (!Win32.StackWalk64(MachineType.I386, _phandle, _thandle,
                        ref stackFrame, ref context, readMemoryProc, Win32.SymFunctionTableAccess64, Win32.SymGetModuleBase64, 0))
                        break;

                    if (stackFrame.AddrPC.Offset == 0)
                        break;

                    uint addr = (uint)(stackFrame.AddrPC.Offset & 0xffffffff);

                    ListViewItem newItem = listViewCallStack.Items.Add(new ListViewItem(new string[] {
                        "0x" + addr.ToString("x8"),
                        _symbols.GetSymbolFromAddress(addr)
                    }));

                    newItem.Tag = addr;

                    if (stackFrame.Params.Length > 0)
                        newItem.ToolTipText = "Parameters: ";

                    foreach (long arg in stackFrame.Params)
                        newItem.ToolTipText += "0x" + (arg & 0xffffffff).ToString("x") + ", ";

                    if (newItem.ToolTipText.EndsWith(", "))
                        newItem.ToolTipText = newItem.ToolTipText.Remove(newItem.ToolTipText.Length - 2);
                }
                catch
                {
                    break;
                }
            }

            listViewCallStack.EndUpdate();
        }

        private void buttonWalk_Click(object sender, EventArgs e)
        {
            this.WalkCallStack();
        }

        private void listViewCallStack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCallStack.SelectedItems.Count == 1)
            {
                string fileName;

                _symbols.GetSymbolFromAddress((uint)listViewCallStack.SelectedItems[0].Tag, out fileName);
                fileModule.Text = fileName;
                fileModule.Enabled = true;
            }
            else
            {
                fileModule.Text = "";
                fileModule.Enabled = false;
            }
        }

        private void buttonToken_Click(object sender, EventArgs e)
        {
            try
            {
                using (ThreadHandle thread = new ThreadHandle(_tid, Program.MinThreadQueryRights))
                {
                    TokenWindow tokForm = new TokenWindow(thread);

                    tokForm.TopMost = this.TopMost;
                    tokForm.Text = "Token - " + this.Text;
                    tokForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Cannot access a disposed object"))
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
