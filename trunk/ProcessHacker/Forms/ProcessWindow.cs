/*
 * Process Hacker
 * 
 * Copyright (C) 2008 Dean
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ProcessWindow : Form
    {
        private ProcessItem _processItem;
        private int _pid;
        private Process _process;

        private ThreadProvider _threadP;
        private ModuleProvider _moduleP;
        private MemoryProvider _memoryP;
        private HandleProvider _handleP;

        private TokenProperties _tokenProps;
        private ServiceProperties _serviceProps;

        public ProcessWindow(ProcessItem process)
        {
            InitializeComponent();

            _processItem = process;
            _pid = process.PID;
            _process = Process.GetProcessById(_pid);

            this.Text = process.Name + " (PID " + _pid.ToString() + ")";

            if (process.Icon != null)
                this.Icon = process.Icon;
            else
                this.Icon = Program.HackerWindow.Icon;

            Program.PWindows.Add(_pid, this);
        }

        private void ProcessWindow_Load(object sender, EventArgs e)
        {
            this.Size = Properties.Settings.Default.ProcessWindowSize;

            if (tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab];

            Program.UpdateWindows();

            _tokenProps = new TokenProperties(_processItem.ProcessQueryLimitedHandle);
            _tokenProps.Dock = DockStyle.Fill;
            tabToken.Controls.Add(_tokenProps);

            listThreads.BeginUpdate();
            listThreads.Highlight = false;
            _threadP = new ThreadProvider(_pid);
            _threadP.Interval = Properties.Settings.Default.RefreshInterval;
            _threadP.Updated += new Provider<int, ThreadItem>.ProviderUpdateOnce(_threadP_Updated);
            _threadP.RunOnceAsync();
            listThreads.Provider = _threadP;
            _threadP.Enabled = true;

            listModules.BeginUpdate();
            listModules.Highlight = false;
            _moduleP = new ModuleProvider(_pid);
            _moduleP.Interval = Properties.Settings.Default.RefreshInterval;
            _moduleP.Updated += new Provider<int, ModuleItem>.ProviderUpdateOnce(_moduleP_Updated);
            _moduleP.RunOnceAsync();
            listModules.Provider = _moduleP;
            _moduleP.Enabled = true;

            listMemory.BeginUpdate();
            listMemory.Highlight = false;
            _memoryP = new MemoryProvider(_pid);
            _memoryP.Interval = Properties.Settings.Default.RefreshInterval;
            _memoryP.Updated += new Provider<int, MemoryItem>.ProviderUpdateOnce(_memoryP_Updated);
            _memoryP.RunOnceAsync();
            listMemory.Provider = _memoryP;
            _memoryP.Enabled = true;

            listHandles.BeginUpdate();
            listHandles.Highlight = false;
            _handleP = new HandleProvider(_pid);
            _handleP.Interval = Properties.Settings.Default.RefreshInterval;
            _handleP.Updated += new Provider<short, HandleItem>.ProviderUpdateOnce(_handleP_Updated);
            _handleP.RunOnceAsync();
            listHandles.Provider = _handleP;
            _handleP.Enabled = true;

            _serviceProps = new ServiceProperties(
                Program.HackerWindow.ProcessServices.ContainsKey(_pid) ?
                Program.HackerWindow.ProcessServices[_pid].ToArray() : 
                new string[0]);
            _serviceProps.Dock = DockStyle.Fill;
            _serviceProps.PID = _pid;
            tabServices.Controls.Add(_serviceProps);

            try { pictureIcon.Image = Win32.GetProcessIcon(_process, true).ToBitmap(); }
            catch { pictureIcon.Image = global::ProcessHacker.Properties.Resources.Process.ToBitmap(); }
            try
            {
                string fileName;

                if (_pid == 4)
                    fileName = Misc.GetKernelFileName();
                else
                    fileName = Misc.GetRealPath(_process.MainModule.FileName);

                FileVersionInfo info = FileVersionInfo.GetVersionInfo(fileName);

                textFileDescription.Text = info.FileDescription;
                textFileCompany.Text = info.CompanyName;
                textFileVersion.Text = info.FileVersion;
                textFileName.Text = info.FileName;
            }
            catch (Exception ex)
            {
                textFileDescription.Text = "(" + ex.Message + ")";
                textFileCompany.Text = "";
            }

            textCmdLine.Text = _processItem.CmdLine;

            try
            {
                using (Win32.ProcessHandle phandle
                    = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                {
                    textCurrentDirectory.Text =
                        phandle.GetPEBString(Win32.ProcessHandle.PEBOffset.CurrentDirectoryPath);
                }
            }
            catch (Exception ex)
            {
                textCurrentDirectory.Text = "(" + ex.Message + ")";
            }

            if (_processItem.ParentPID != -1)
            {
                if (Program.HackerWindow.ProcessProvider.Dictionary.ContainsKey(_processItem.ParentPID))
                {
                    textParent.Text =
                        Program.HackerWindow.ProcessProvider.Dictionary[_processItem.ParentPID].Name +
                        " (" + _processItem.ParentPID.ToString() + ")";
                }
                else
                {
                    textParent.Text = "Non-existent Process (" + _processItem.ParentPID.ToString() + ")";
                    buttonInspectParent.Enabled = false;
                }
            }

            this.UpdateDEPStatus();

            tabControl_TabIndexChanged(null, null);
        }

        public void UpdateDEPStatus()
        {
            try
            {
                using (Win32.ProcessHandle phandle
                    = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION))
                {
                    var depStatus = phandle.GetDEPStatus();
                    string str;

                    if ((depStatus & Win32.ProcessHandle.DEPStatus.Enabled) != 0)
                    {
                        str = "Enabled";
                    }
                    else
                    {
                        str = "Disabled";
                    }

                    if ((depStatus & Win32.ProcessHandle.DEPStatus.Permanent) != 0)
                    {
                        buttonEditDEP.Enabled = false;
                        str += ", Permanent";
                    }

                    if ((depStatus & Win32.ProcessHandle.DEPStatus.ATLThunkEmulationDisabled) != 0)
                        str += ", DEP-ATL thunk emulation disabled";

                    textDEP.Text = str;
                }
            }
            catch (Exception ex)
            {
                textDEP.Text = "(" + ex.Message + ")";
            }

            if (_processItem.SessionId != Program.CurrentSessionId)
                buttonEditDEP.Enabled = false;
        }

        private void _memoryP_Updated()
        {
            if (_memoryP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listMemory.EndUpdate();
                    listMemory.Highlight = true;
                }));
                _memoryP.Updated -= new Provider<int, MemoryItem>.ProviderUpdateOnce(_memoryP_Updated);
            }
        }

        private void _handleP_Updated()
        {
            if (_handleP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listHandles.EndUpdate();
                    listHandles.Highlight = true;
                }));
                _handleP.Updated -= new Provider<short, HandleItem>.ProviderUpdateOnce(_handleP_Updated);
            }
        }

        private void _moduleP_Updated()
        {
            if (_moduleP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listModules.EndUpdate();
                    listModules.Highlight = true;
                }));
                _moduleP.Updated -= new Provider<int, ModuleItem>.ProviderUpdateOnce(_moduleP_Updated);
            }
        }

        private void _threadP_Updated()
        {
            if (_threadP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listThreads.EndUpdate();
                    listThreads.Highlight = true;
                }));
                _threadP.Updated -= new Provider<int, ThreadItem>.ProviderUpdateOnce(_threadP_Updated);
            }
        }

        private void ProcessWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _threadP.Kill();
            listThreads.SaveSettings();
            _tokenProps.SaveSettings();

            Properties.Settings.Default.ProcessWindowSelectedTab = tabControl.SelectedTab.Name;
            Properties.Settings.Default.ProcessWindowSize = this.Size;
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        private void inspectImageFileMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path;

                if (_pid == 4)
                {
                    path = Misc.GetKernelFileName();
                }
                else
                {
                    path = Misc.GetRealPath(_process.MainModule.FileName);
                }

                PEWindow pw = Program.GetPEWindow(path,
                    new Program.PEWindowInvokeAction(delegate(PEWindow f)
                    {
                        try
                        {
                            f.Show();
                            f.Activate();
                        }
                        catch
                        { }
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inspecting:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            _threadP.Enabled = false;
            _moduleP.Enabled = false;
            _memoryP.Enabled = false;
            _handleP.Enabled = false;

            if (tabControl.SelectedTab == tabThreads)
                _threadP.Enabled = true;
            else if (tabControl.SelectedTab == tabModules)
                _moduleP.Enabled = true;
            else if (tabControl.SelectedTab == tabMemory)
                _memoryP.Enabled = true;
            else if (tabControl.SelectedTab == tabHandles)
                _handleP.Enabled = true;
        }

        private void buttonPEBStrings_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            try
            {
                using (Win32.ProcessHandle ph
                    = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                {
                    list.Add(new KeyValuePair<string, string>("Command Line",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.CommandLine)));
                    list.Add(new KeyValuePair<string, string>("Current Directory Path",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.CurrentDirectoryPath)));
                    list.Add(new KeyValuePair<string, string>("Desktop Name",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.DesktopName)));
                    list.Add(new KeyValuePair<string, string>("Path",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.DllPath)));
                    list.Add(new KeyValuePair<string, string>("Image File Name",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.ImagePathName)));
                    list.Add(new KeyValuePair<string, string>("Runtime Data",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.RuntimeData)));
                    list.Add(new KeyValuePair<string, string>("Shell Info",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.ShellInfo)));
                    list.Add(new KeyValuePair<string, string>("Window Title",
                        ph.GetPEBString(Win32.ProcessHandle.PEBOffset.WindowTitle)));
                }

                ListWindow window = new ListWindow(list);

                window.TopMost = this.TopMost;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonOpenFileNameFolder_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "/select," + textFileName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonOpenCurDir_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "/select," + textCurrentDirectory.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonTerminate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to terminate this process?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                try
                {
                    using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                        phandle.Terminate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonEditDEP_Click(object sender, EventArgs e)
        {
            EditDEPWindow w = new EditDEPWindow(_pid);

            w.TopMost = this.TopMost;
            w.ShowDialog();

            this.UpdateDEPStatus();
        }

        private void buttonInspectParent_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessWindow pForm = Program.GetProcessWindow(
                    Program.HackerWindow.ProcessProvider.Dictionary[_processItem.ParentPID],
                    new Program.PWindowInvokeAction(delegate(ProcessWindow f)
                    {
                        f.Show();
                        f.Activate();
                    }));
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("The process could not be found.", "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not inspect the process:\n\n" + ex.Message,
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
