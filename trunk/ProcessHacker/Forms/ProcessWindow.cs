/*
 * Process Hacker - 
 *   process properties window
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
        private Bitmap _processImage;

        private ThreadProvider _threadP;
        private ModuleProvider _moduleP;
        private MemoryProvider _memoryP;
        private HandleProvider _handleP;

        private TokenProperties _tokenProps;
        private ServiceProperties _serviceProps;

        private DeltaManager<string, long> _processStats =
            new DeltaManager<string, long>(new Int64Subtractor());

        private string _realCurrentDirectory;

        public ProcessWindow(ProcessItem process)
        {
            InitializeComponent();

            fileCurrentDirectory.TextBoxLeave += new EventHandler(fileCurrentDirectory_TextBoxLeave);

            _processItem = process;
            _pid = process.PID;

            this.Text = process.Name + " (PID " + _pid.ToString() + ")";

            // DPCs or Interrupts
            if (_pid < 0)
            {
                this.Text = process.Name;
                textFileDescription.Text = process.Name;
                textFileCompany.Text = "";
            }
            else
            {
                timerUpdate.Enabled = true;
            }

            if (process.Icon != null)
                this.Icon = process.Icon;
            else
                this.Icon = Program.HackerWindow.Icon;

            Program.PWindows.Add(_pid, this);

            try
            {
                ProcessItem item = Program.HackerWindow.ProcessProvider.Dictionary[_pid];

                _processStats.Add("System Other",
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.IdleTime + 
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.DpcTime + 
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.InterruptTime);
                _processStats.Add("System Kernel",
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.KernelTime);
                _processStats.Add("System User",
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.UserTime);
                _processStats.Add("Process Kernel", item.Process.KernelTime);
                _processStats.Add("Process User", item.Process.UserTime);
                _processStats.Add("IO Read+Other",
                    (long)item.Process.IoCounters.ReadTransferCount +
                    (long)item.Process.IoCounters.OtherTransferCount);
                _processStats.Add("IO Write",
                    (long)item.Process.IoCounters.WriteTransferCount);
            }
            catch
            { }
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        public ListView ThreadListView
        {
            get { return listThreads.List; }
        }

        public ListView ModuleListView
        {
            get { return listModules.List; }
        }

        public ListView MemoryListView
        {
            get { return listMemory.List; }
        }

        public ListView HandleListView
        {
            get { return listHandles.List; }
        }

        public ListView ServiceListView
        {
            get { return _serviceProps.List; }
        }

        private void ProcessWindow_Load(object sender, EventArgs e)
        {
            // load settings
            this.Size = Properties.Settings.Default.ProcessWindowSize;
            buttonSearch.Text = Properties.Settings.Default.SearchType;
            checkHideHandlesNoName.Checked = Properties.Settings.Default.HideHandlesWithNoName;

            if (tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab];

            // update the Window menu
            Program.UpdateWindows();
            this.ApplyFont(Properties.Settings.Default.Font);

            this.ClearStatistics();

            try
            {
                _process = Process.GetProcessById(_pid);
                this.UpdateProcessProperties();
            }
            catch
            {
                // this "process" is probably DPCs or Interrupts, so we won't try to load any more information
                buttonEditDEP.Enabled = false;
                buttonEditProtected.Enabled = false;
                buttonInspectParent.Enabled = false;
                buttonInspectPEB.Enabled = false;

                if (fileCurrentDirectory.Text != "")
                    fileCurrentDirectory.Enabled = false;

                if (_pid != 4)
                    fileImage.Enabled = false;

                buttonSearch.Enabled = false;
                buttonTerminate.Enabled = false;

                // remove tab controls not relevant to DPCs/Interrupts
                tabControl.TabPages.Remove(tabHandles);
                tabControl.TabPages.Remove(tabMemory);
                tabControl.TabPages.Remove(tabModules);
                tabControl.TabPages.Remove(tabServices);
                tabControl.TabPages.Remove(tabThreads);
                tabControl.TabPages.Remove(tabToken);
            }

            if (_pid == 0)
                textFileDescription.Text = "System Idle Process";

            this.UpdateDeltas();

            // add our handler to the process provider
            Program.HackerWindow.ProcessProvider.Updated += 
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);

            // HACK: Delay loading
            Timer t = new Timer();

            t.Tick += (sender_, e_) => { t.Enabled = false; this.LoadStage2(); };
            t.Interval = 1;
            t.Enabled = true;
        }

        private void LoadStage2()
        {
            this.SuspendLayout();
                
            this.InitializeSubControls();

            try
            {
                this.InitializeProviders();
            }
            catch
            { }

            // disable providers which aren't in use
            tabControl_SelectedIndexChanged(null, null);

            this.ResumeLayout();
        }

        private void ProcessWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pid >= 0)
            {
                listThreads.SaveSettings();
                listModules.SaveSettings();
                listMemory.SaveSettings();
                listHandles.SaveSettings();
            }

            if (_tokenProps != null)
            {
                _tokenProps.SaveSettings();
                (_tokenProps.Object as Win32.ProcessHandle).Dispose();
            }

            timerUpdate.Enabled = false;

            if (_processImage != null)
            {
                pictureIcon.Image = null;
                _processImage.Dispose();
            }

            Program.HackerWindow.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);

            Properties.Settings.Default.ProcessWindowSelectedTab = tabControl.SelectedTab.Name;
            Properties.Settings.Default.SearchType = buttonSearch.Text;
            Properties.Settings.Default.ProcessWindowSize = this.Size;
        }

        private void ProcessWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_process != null)
                _process.Close();

            if (_threadP != null)
            {
                _threadP.Dispose();
                _threadP = null;
            }

            if (_moduleP != null)
            {
                _moduleP.Dispose();
                _moduleP = null;
            }

            if (_memoryP != null)
            {
                _memoryP.Dispose();
                _memoryP = null;
            }

            if (_handleP != null)
            {
                _handleP.Dispose();
                _handleP = null;
            }

            // A temporary fix for any handle leaks
            System.GC.Collect();
        }

        public void ApplyFont(Font f)
        {
            listThreads.List.Font = f;
            listModules.List.Font = f;
            listMemory.List.Font = f;
            listHandles.List.Font = f;

            if (_serviceProps != null)
                _serviceProps.List.Font = f;
        }

        private void UpdateProcessProperties()
        {
            try
            {
                string fileName;

                if (_pid == 4)
                    fileName = Misc.GetKernelFileName();
                else
                    fileName = _processItem.FileName;

                FileVersionInfo info = FileVersionInfo.GetVersionInfo(fileName);

                textFileDescription.Text = info.FileDescription;
                textFileCompany.Text = info.CompanyName;
                textFileVersion.Text = info.FileVersion;
                fileImage.Text = info.FileName;

                try
                {
                    using (Icon icon = Win32.GetFileIcon(fileName, true))
                    {
                        pictureIcon.Image = _processImage = icon.ToBitmap();
                    }
                }
                catch 
                {
                    pictureIcon.Image = _processImage = ProcessHacker.Properties.Resources.Process.ToBitmap();
                }

                if (Properties.Settings.Default.VerifySignatures)
                {
                    var verifyResult = _processItem.VerifyResult;

                    if (verifyResult == Win32.VerifyResult.Trusted)
                        textFileCompany.Text += " (verified)";
                    else if (verifyResult == Win32.VerifyResult.TrustedInstaller)
                        textFileCompany.Text += " (verified, Windows component)";
                    else if (verifyResult == Win32.VerifyResult.NoSignature)
                        textFileCompany.Text += " (not verified, no signature)";
                    else if (verifyResult == Win32.VerifyResult.Distrust)
                        textFileCompany.Text += " (not verified, distrusted)";
                    else if (verifyResult == Win32.VerifyResult.Expired)
                        textFileCompany.Text += " (not verified, expired)";
                    else if (verifyResult == Win32.VerifyResult.Revoked)
                        textFileCompany.Text += " (not verified, revoked)";
                    else if (verifyResult == Win32.VerifyResult.SecuritySettings)
                        textFileCompany.Text += " (not verified, security settings)";
                    else
                        textFileCompany.Text += " (not verified)";
                }
            }
            catch
            {
                fileImage.Text = _processItem.FileName;
                textFileDescription.Text = "";
                textFileCompany.Text = "";
            }

            if (_processItem.CmdLine != null)
                textCmdLine.Text = _processItem.CmdLine.Replace("\0", "");

            try
            {
                DateTime startTime = DateTime.FromFileTime(_processItem.Process.CreateTime);

                textStartTime.Text = Misc.GetNiceRelativeDateTime(startTime) +
                    " (" + startTime.ToString() + ")";
            }
            catch (Exception ex)
            {
                textStartTime.Text = "(" + ex.Message + ")";
            }

            try
            {
                using (Win32.ProcessHandle phandle
                    = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                {
                    fileCurrentDirectory.Text =
                        phandle.GetPebString(Win32.ProcessHandle.PebOffset.CurrentDirectoryPath);
                }

                fileCurrentDirectory.Enabled = true;
            }
            catch (Exception ex)
            {
                fileCurrentDirectory.Text = "(" + ex.Message + ")";
                fileCurrentDirectory.Enabled = false;
            }

            _realCurrentDirectory = fileCurrentDirectory.Text;

            try
            {
                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights))
                {
                    textPEBAddress.Text = "0x" + phandle.GetBasicInformation().PebBaseAddress.ToString("x8");
                }
            }
            catch (Exception ex)
            {
                textPEBAddress.Text = "(" + ex.Message + ")";
            }

            if (_processItem.HasParent)
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
            else if (_processItem.ParentPID == -1)
            {
                // this process doesn't actually have a parent
                textParent.Text = "No Parent Process";
                buttonInspectParent.Enabled = false;
            }
            else
            {
                // This process had a parent and it's dead, but 
                // another running process has the same PID as 
                // its parent. We checked their creation times 
                // back in ProcessSystemProvider.cs.
                textParent.Text = "Non-existent Process (" + _processItem.ParentPID.ToString() + ")";
                buttonInspectParent.Enabled = false;
            }

            this.UpdateProtected();
            this.UpdateDEPStatus();
        }

        private void InitializeSubControls()
        {
            try
            {
                _tokenProps = new TokenProperties(new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights));
                _tokenProps.Dock = DockStyle.Fill;
                tabToken.Controls.Add(_tokenProps);
            }
            catch
            { }

            _serviceProps = new ServiceProperties(
                Program.HackerWindow.ProcessServices.ContainsKey(_pid) ?
                Program.HackerWindow.ProcessServices[_pid].ToArray() :
                new string[0]);
            _serviceProps.Dock = DockStyle.Fill;
            _serviceProps.PID = _pid;
            tabServices.Controls.Add(_serviceProps);
        }

        private void InitializeProviders()
        {
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
            _memoryP.IgnoreFreeRegions = true;
            _memoryP.Interval = Properties.Settings.Default.RefreshInterval;
            _memoryP.Updated += new Provider<int, MemoryItem>.ProviderUpdateOnce(_memoryP_Updated);
            _memoryP.RunOnceAsync();
            listMemory.Provider = _memoryP;
            _memoryP.Enabled = true;

            listHandles.BeginUpdate();
            listHandles.Highlight = false;
            _handleP = new HandleProvider(_pid);
            _handleP.HideHandlesWithNoName = Properties.Settings.Default.HideHandlesWithNoName;
            _handleP.Interval = Properties.Settings.Default.RefreshInterval;
            _handleP.Updated += new Provider<short, HandleItem>.ProviderUpdateOnce(_handleP_Updated);
            _handleP.RunOnceAsync();
            listHandles.Provider = _handleP;
            _handleP.Enabled = true;

            Win32.SetWindowTheme(listThreads.List.Handle, "explorer", null);
            Win32.SetWindowTheme(listModules.List.Handle, "explorer", null);
            Win32.SetWindowTheme(listMemory.List.Handle, "explorer", null);
            Win32.SetWindowTheme(listHandles.List.Handle, "explorer", null);

            this.InitializeShortcuts();
        }

        private void InitializeShortcuts()
        {
            listThreads.List.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listThreads.List.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listThreads.List, -1);
                };
            listModules.List.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listModules.List.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listModules.List, -1);
                };
            listMemory.List.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listMemory.List.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listMemory.List, -1);
                };
            listHandles.List.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listHandles.List.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listHandles.List, -1);
                };
        }

        public void UpdateProtected()
        {
            labelProtected.Enabled = true;
            textProtected.Enabled = true;
            buttonEditProtected.Enabled = true;

            if (Program.KPH != null && Program.WindowsVersion == "Vista")
            {
                try
                {
                    textProtected.Text = Program.KPH.GetProcessProtected(_pid) ? "Protected" : "Not Protected";
                }
                catch (Exception ex)
                {
                    textProtected.Text = "(" + ex.Message + ")";
                    buttonEditProtected.Enabled = false;
                }
            }
            else
            {
                labelProtected.Enabled = false;
                textProtected.Enabled = false;
                buttonEditProtected.Enabled = false;
            }
        }

        public void UpdateDEPStatus()
        {
            labelDEP.Enabled = true;
            textDEP.Enabled = true;
            try
            {
                using (var phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION))
                {
                    var depStatus = phandle.GetDepStatus();
                    string str;

                    if ((depStatus & Win32.ProcessHandle.DepStatus.Enabled) != 0)
                    {
                        str = "Enabled";
                    }
                    else
                    {
                        str = "Disabled";
                    }

                    if ((depStatus & Win32.ProcessHandle.DepStatus.Permanent) != 0)
                    {
                        buttonEditDEP.Enabled = false;
                        str += ", Permanent";
                    }

                    if ((depStatus & Win32.ProcessHandle.DepStatus.AtlThunkEmulationDisabled) != 0)
                        str += ", DEP-ATL thunk emulation disabled";

                    textDEP.Text = str;
                }
            }
            catch (EntryPointNotFoundException)
            {
                labelDEP.Enabled = false;
                textDEP.Enabled = false;
                textDEP.Text = "";
                //textDEP.Text = "(This feature is not supported on your version of Windows)";
                buttonEditDEP.Enabled = false;
            }
            catch (Exception ex)
            {
                textDEP.Text = "(" + ex.Message + ")";
                buttonEditDEP.Enabled = false;
            }

            if (_processItem.SessionId != Program.CurrentSessionId)
                buttonEditDEP.Enabled = false;
        }

        private void PerformSearch(string text)
        {
            Point location = this.Location;
            System.Drawing.Size size = this.Size;

            ResultsWindow rw = Program.GetResultsWindow(_pid, 
                new Program.ResultsWindowInvokeAction(delegate(ResultsWindow f)
            {
                if (text == "&New Results Window...")
                {
                    f.Show();
                }
                else if (text == "&Literal...")
                {
                    if (f.EditSearch(SearchType.Literal, location, size) == DialogResult.OK)
                    {
                        f.Show();
                        f.StartSearch();
                    }
                    else
                    {
                        f.Close();
                    }
                }
                else if (text == "&Regex...")
                {
                    if (f.EditSearch(SearchType.Regex, location, size) == DialogResult.OK)
                    {
                        f.Show();
                        f.StartSearch();
                    }
                    else
                    {
                        f.Close();
                    }
                }
                else if (text == "&String Scan...")
                {
                    f.SearchOptions.Type = SearchType.String;
                    f.Show();
                    f.StartSearch();
                }
                else if (text == "&Heap Scan...")
                {
                    f.SearchOptions.Type = SearchType.Heap;
                    f.Show();
                    f.StartSearch();
                }
                else if (text == "S&truct...")
                {
                    if (f.EditSearch(SearchType.Struct, location, size) == DialogResult.OK)
                    {
                        f.Show();
                        f.StartSearch();
                    }
                    else
                    {
                        f.Close();
                    }
                }
            }));

            buttonSearch.Text = text;
        }

        private void ClearStatistics()
        {
            labelCPUPriority.Text = "";
            labelCPUKernelTime.Text = "";
            labelCPUUserTime.Text = "";
            labelCPUTotalTime.Text = "";

            labelMemoryPB.Text = "";
            labelMemoryWS.Text = "";
            labelMemoryPWS.Text = "";
            labelMemoryVS.Text = "";
            labelMemoryPVS.Text = "";
            labelMemoryPU.Text = "";
            labelMemoryPPU.Text = "";
            labelMemoryPF.Text = "";

            labelIOReads.Text = "";
            labelIOReadBytes.Text = "";
            labelIOWrites.Text = "";
            labelIOWriteBytes.Text = "";
            labelIOOther.Text = "";
            labelIOOtherBytes.Text = "";

            labelOtherHandles.Text = "";
            labelOtherGDIHandles.Text = "";
            labelOtherUSERHandles.Text = "";
        }

        private void UpdateDeltas()
        {
            ProcessItem item = Program.HackerWindow.ProcessProvider.Dictionary[_pid];

            // update deltas         
            _processStats.Update("System Other",
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.IdleTime +
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.DpcTime +
                    Program.HackerWindow.ProcessProvider.ProcessorPerf.InterruptTime);
            _processStats.Update("System Kernel",
                Program.HackerWindow.ProcessProvider.ProcessorPerf.KernelTime);
            _processStats.Update("System User",
                Program.HackerWindow.ProcessProvider.ProcessorPerf.UserTime);
            _processStats.Update("Process Kernel", item.Process.KernelTime);
            _processStats.Update("Process User", item.Process.UserTime);
            _processStats.Update("IO Read+Other",
                (long)item.Process.IoCounters.ReadTransferCount +
                (long)item.Process.IoCounters.OtherTransferCount);
            _processStats.Update("IO Write",
                (long)item.Process.IoCounters.WriteTransferCount);
        }

        private void UpdatePerformance()
        {
            ProcessItem item = Program.HackerWindow.ProcessProvider.Dictionary[_pid];

            plotterCPUUsage.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            plotterCPUUsage.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
            plotterMemory.LineColor1 = Properties.Settings.Default.PlotterMemoryPrivateColor;
            plotterMemory.LineColor2 = Properties.Settings.Default.PlotterMemoryWSColor;
            plotterIO.LineColor1 = Properties.Settings.Default.PlotterIOROColor;
            plotterIO.LineColor2 = Properties.Settings.Default.PlotterIOWColor;

            // update graphs
            long sysTotal = _processStats.GetDelta("System Kernel") + _processStats.GetDelta("System User")
                + _processStats.GetDelta("System Other");
            float procKernel = (float)_processStats.GetDelta("Process Kernel") / sysTotal;
            float procUser = (float)_processStats.GetDelta("Process User") / sysTotal;

            plotterCPUUsage.Add(procKernel, procUser);
            plotterCPUUsage.Text = ((procKernel + procUser) * 100).ToString("F2") +
                "% (K: " + (procKernel * 100).ToString("F2") +
                "%, U: " + (procUser * 100).ToString("F2") + "%)";

            plotterMemory.Add(item.Process.VirtualMemoryCounters.PrivateBytes,
                item.Process.VirtualMemoryCounters.WorkingSetSize);
            plotterMemory.Text = "Pvt: " + Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PrivateBytes) + 
                ", WS: " + Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.WorkingSetSize);

            plotterIO.Add(_processStats.GetDelta("IO Read+Other"), _processStats.GetDelta("IO Write"));
            plotterIO.Text = "R+O: " + Misc.GetNiceSizeName(_processStats.GetDelta("IO Read+Other")) +
                ", W: " + Misc.GetNiceSizeName(_processStats.GetDelta("IO Write"));

            // update statistics
            if (tabControl.SelectedTab == tabStatistics)
            {
                labelCPUPriority.Text = item.Process.BasePriority.ToString();
                labelCPUKernelTime.Text = Misc.GetNiceTimeSpan(new TimeSpan(item.Process.KernelTime));
                labelCPUUserTime.Text = Misc.GetNiceTimeSpan(new TimeSpan(item.Process.UserTime));
                labelCPUTotalTime.Text = Misc.GetNiceTimeSpan(new TimeSpan(item.Process.KernelTime + item.Process.UserTime));

                labelMemoryPB.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PrivateBytes);
                labelMemoryWS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.WorkingSetSize);
                labelMemoryPWS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PeakWorkingSetSize);
                labelMemoryVS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.VirtualSize);
                labelMemoryPVS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PeakVirtualSize);
                labelMemoryPU.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PagefileUsage);
                labelMemoryPPU.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PeakPagefileUsage);
                labelMemoryPF.Text = item.Process.VirtualMemoryCounters.PageFaultCount.ToString("N0");

                labelIOReads.Text = item.Process.IoCounters.ReadOperationCount.ToString("N0");
                labelIOReadBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.ReadTransferCount);
                labelIOWrites.Text = item.Process.IoCounters.WriteOperationCount.ToString("N0");
                labelIOWriteBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.WriteTransferCount);
                labelIOOther.Text = item.Process.IoCounters.OtherOperationCount.ToString("N0");
                labelIOOtherBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.OtherTransferCount);

                labelOtherHandles.Text = item.Process.HandleCount.ToString("N0");

                try
                {
                    using (var phandle = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights))
                    {
                        labelOtherGDIHandles.Text = phandle.GetGuiResources(false).ToString("N0");
                        labelOtherUSERHandles.Text = phandle.GetGuiResources(true).ToString("N0");
                    }
                }
                catch
                { }
            }
        }

        private void fileCurrentDirectory_TextBoxLeave(object sender, EventArgs e)
        {
            if (fileCurrentDirectory.Text != _realCurrentDirectory)
                fileCurrentDirectory.Text = _realCurrentDirectory;
        }

        #region Buttons

        private void buttonPEBStrings_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            try
            {
                using (Win32.ProcessHandle ph
                    = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                {
                    list.Add(new KeyValuePair<string, string>("Command Line",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.CommandLine)));
                    list.Add(new KeyValuePair<string, string>("Current Directory Path",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.CurrentDirectoryPath)));
                    list.Add(new KeyValuePair<string, string>("Desktop Name",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.DesktopName)));
                    list.Add(new KeyValuePair<string, string>("Path",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.DllPath)));
                    list.Add(new KeyValuePair<string, string>("Image File Name",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.ImagePathName)));
                    list.Add(new KeyValuePair<string, string>("Runtime Data",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.RuntimeData)));
                    list.Add(new KeyValuePair<string, string>("Shell Info",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.ShellInfo)));
                    list.Add(new KeyValuePair<string, string>("Window Title",
                        ph.GetPebString(Win32.ProcessHandle.PebOffset.WindowTitle)));
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

        private void buttonEditProtected_Click(object sender, EventArgs e)
        {
            try
            {
                ComboBoxPickerWindow picker = new ComboBoxPickerWindow(new string[] { "Protect", "Unprotect" });

                picker.Message = "Select an action below:";
                picker.SelectedItem = (textProtected.Text == "Protected") ? "Protect" : "Unprotect";

                if (picker.ShowDialog() == DialogResult.OK)
                {
                    Program.KPH.SetProcessProtected(_pid, picker.SelectedItem == "Protect");
                    this.UpdateProtected();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonInspectPEB_Click(object sender, EventArgs e)
        {
            try
            { 
                if (!Program.Structs.ContainsKey("PEB"))
                    throw new Exception("The struct 'PEB' has not been loaded. Make sure structs.txt was loaded successfully.");

                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights))
                {
                    int baseAddress = phandle.GetBasicInformation().PebBaseAddress;

                    Program.HackerWindow.BeginInvoke(new MethodInvoker(delegate
                    {
                        StructWindow sw = new StructWindow(_pid, baseAddress, Program.Structs["PEB"]);

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
            catch (Exception ex)
            {
                MessageBox.Show("Could not inspect the PEB:\n\n" + ex.Message,
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            PerformSearch(buttonSearch.Text);
        }

        #endregion

        #region Check Boxes

        private void checkHideFreeRegions_CheckedChanged(object sender, EventArgs e)
        {
            checkHideFreeRegions.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            _memoryP.IgnoreFreeRegions = checkHideFreeRegions.Checked;  
            _memoryP.Updated += new MemoryProvider.ProviderUpdateOnce(_memoryP_Updated);
        }

        private void checkHideHandlesNoName_CheckedChanged(object sender, EventArgs e)
        {
            if (_handleP != null)
            {
                checkHideHandlesNoName.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                _handleP.Dispose();
                listHandles.BeginUpdate();
                listHandles.Highlight = false;
                _handleP = new HandleProvider(_pid);
                _handleP.HideHandlesWithNoName = checkHideHandlesNoName.Checked;
                _handleP.Interval = Properties.Settings.Default.RefreshInterval;
                _handleP.Updated += new Provider<short, HandleItem>.ProviderUpdateOnce(_handleP_Updated);
                _handleP.RunOnceAsync();
                listHandles.Provider = _handleP;
                _handleP.Enabled = true;
            }
        }

        #endregion       

        #region Menu Items 

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
                    path = _processItem.FileName;
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

        #endregion

        #region Providers

        private void ProcessProvider_Updated()
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        this.UpdateDeltas();
                        this.UpdatePerformance();
                    }
                    catch
                    { }
                }));
            }
            catch
            { }
        }

        private void _memoryP_Updated()
        {
            if (_memoryP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listMemory.EndUpdate();
                    listMemory.Invalidate();
                    listMemory.Highlight = true;
                    checkHideFreeRegions.Enabled = true;
                    this.Cursor = Cursors.Default;
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
                    listHandles.Invalidate();
                    listHandles.Highlight = true;
                    checkHideHandlesNoName.Enabled = true;
                    this.Cursor = Cursors.Default;
                }));
                _handleP.Updated -= new HandleProvider.ProviderUpdateOnce(_handleP_Updated);
            }
        }

        private void _moduleP_Updated()
        {
            if (_moduleP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listModules.EndUpdate();
                    listModules.Invalidate();
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
                    listThreads.Invalidate();
                    listThreads.Highlight = true;
                }));
                _threadP.Updated -= new Provider<int, ThreadItem>.ProviderUpdateOnce(_threadP_Updated);
            }
        }

        #endregion

        #region Search Menu Items

        private void newWindowSearchMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch(newWindowSearchMenuItem.Text);
        }

        private void literalSearchMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch(literalSearchMenuItem.Text);
        }

        private void regexSearchMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch(regexSearchMenuItem.Text);
        }

        private void stringScanMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch(stringScanMenuItem.Text);
        }

        private void heapScanMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch(heapScanMenuItem.Text);
        }

        private void structSearchMenuItem_Click(object sender, EventArgs e)
        {
            PerformSearch(structSearchMenuItem.Text);
        }

        #endregion      

        #region Tab Controls

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_threadP != null)
                if (_threadP.Enabled = tabControl.SelectedTab == tabThreads)
                    _threadP.RunOnceAsync();
            if (_moduleP != null)
                if (_moduleP.Enabled = tabControl.SelectedTab == tabModules)
                    _moduleP.RunOnceAsync();
            if (_memoryP != null)
                if (_memoryP.Enabled = tabControl.SelectedTab == tabMemory)
                    _memoryP.RunOnceAsync();
            if (_handleP != null)
                if (_handleP.Enabled = tabControl.SelectedTab == tabHandles)
                    _handleP.RunOnceAsync();
        }

        #endregion

        #region Timers

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_process.HasExited)
                {
                    timerUpdate.Enabled = false;

                    try
                    {
                        using (var phandle = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights))
                        {
                            this.Text += " (exited with code " + phandle.GetExitCode() + ")";
                        }
                    }
                    catch
                    { }

                    return;
                }
            }
            catch
            { }

            try
            {
                using (var phandle
                    = new Win32.ProcessHandle(_pid, Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                {
                    _realCurrentDirectory  =
                        phandle.GetPebString(Win32.ProcessHandle.PebOffset.CurrentDirectoryPath);

                    // we don't want to set the text if the user is selecting something in the textbox!
                    if (!fileCurrentDirectory.TextBoxFocused)
                        fileCurrentDirectory.Text = _realCurrentDirectory;
                }
            }
            catch
            { }
        }

        #endregion
    }
}
