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
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Symbols;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class ThreadWindow : Form
    {
        private int _pid;
        private int _tid;
        private ProcessHandle _phandle;
        private bool _processHandleOwned = true;
        private ThreadHandle _thandle;
        private SymbolProvider _symbols;

        public const string DisplayFormat = "0x{0:x8}";

        public string[] Registers = 
            new string[] { "eax", "ebx", "ecx", "edx", "esi", "edi", "esp", "ebp", "eip", "cs", "ds", "es", "fs", "gs", "ss" };

        public string Id
        {
            get { return _pid + "-" + _tid; }
        }

        public ThreadWindow(int PID, int TID, SymbolProvider symbols, ProcessHandle processHandle)
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
                            labelThreadUser.Text = "Username: " + token.GetUser().GetFullName(true);
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
                if (processHandle != null)
                {
                    _phandle = processHandle;
                    _processHandleOwned = false;
                }
                else
                {
                    try
                    {
                        _phandle = new ProcessHandle(_pid,
                            ProcessAccess.QueryInformation | ProcessAccess.VmRead
                            );
                    }
                    catch
                    {
                        if (KProcessHacker.Instance != null)
                        {
                            _phandle = new ProcessHandle(_pid, Program.MinProcessReadMemoryRights);
                        }
                        else
                        {
                            throw;
                        }
                    }
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
                try
                {
                    _thandle = new ThreadHandle(_tid, ThreadAccess.GetContext | ThreadAccess.SuspendResume);
                }
                catch
                {
                    if (KProcessHacker.Instance != null)
                    {
                        _thandle = new ThreadHandle(_tid,
                            Program.MinThreadQueryRights | ThreadAccess.SuspendResume
                            );
                    }
                    else
                    {
                        throw;
                    }
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
            listViewCallStack.AddShortcuts();

            this.Size = Properties.Settings.Default.ThreadWindowSize;
            ColumnSettings.LoadSettings(Properties.Settings.Default.CallStackColumns, listViewCallStack);

            this.WalkCallStack();
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

        private void WalkCallStack()
        {
            try
            {
                // Clear the existing frames.
                listViewCallStack.BeginUpdate();
                listViewCallStack.Items.Clear();

                bool suspended;

                try
                {
                    _thandle.Suspend();
                    suspended = true;
                }
                catch
                {
                    suspended = false;
                }

                try
                {
                    // Process the kernel-mode stack (if KPH is present).
                    if (KProcessHacker.Instance != null)
                    {
                        this.WalkKernelStack();
                    }

                    // Process the user-mode stack.
                    _thandle.WalkStack(_phandle, this.WalkStackCallback);
                }
                finally
                {
                    if (suspended)
                        _thandle.Resume();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
            finally
            {
                listViewCallStack.EndUpdate();
            }
        }

        private void WalkKernelStack()
        {
            try
            {
                IntPtr[] frames = _thandle.CaptureKernelStack(1); // skip the KPH frame

                foreach (IntPtr frame in frames)
                {
                    ulong address = frame.ToUInt64();

                    try
                    {
                        ListViewItem newItem = listViewCallStack.Items.Add(new ListViewItem(
                            new string[]
                                {
                                    "0x" + address.ToString("x8"),
                                    _symbols.GetSymbolFromAddress(address)
                                }));

                        newItem.Tag = address;
                    }
                    catch (Exception ex2)
                    {
                        Logging.Log(ex2);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private bool WalkStackCallback(ThreadStackFrame stackFrame)
        {
            ulong address = stackFrame.PcAddress.ToUInt64();

            // HACK for XP where the top user-mode frame for system threads is always 0xffffffff
            if (OSVersion.WindowsVersion == WindowsVersion.XP && address == 0xffffffff)
                return true;

            try
            {
                ListViewItem newItem = listViewCallStack.Items.Add(new ListViewItem(
                    new string[]
                    {
                        "0x" + address.ToString("x8"),
                        _symbols.GetSymbolFromAddress(address)
                    }));

                newItem.Tag = address;

                try
                {
                    if (stackFrame.Params.Length > 0)
                        newItem.ToolTipText = "Parameters: ";

                    foreach (long arg in stackFrame.Params)
                        newItem.ToolTipText += "0x" + (arg & 0xffffffff).ToString("x") + ", ";

                    if (newItem.ToolTipText.EndsWith(", "))
                        newItem.ToolTipText = newItem.ToolTipText.Remove(newItem.ToolTipText.Length - 2);

                    try
                    {
                        string fileAndLine = _symbols.GetLineFromAddress(address);

                        if (fileAndLine != null)
                            newItem.ToolTipText += "\nFile: " + fileAndLine;
                    }
                    catch
                    { }
                }
                catch (Exception ex2)
                {
                    Logging.Log(ex2);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);

                ListViewItem newItem = listViewCallStack.Items.Add(new ListViewItem(new string[] {
                            "0x" + address.ToString("x8"),
                            "???"
                        }));

                newItem.Tag = address;
            }

            return true;
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

                _symbols.GetSymbolFromAddress((ulong)listViewCallStack.SelectedItems[0].Tag, out fileName);
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
                    PhUtils.ShowMessage(ex);
            }
        }
    }
}
