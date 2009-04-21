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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Symbols;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;

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
        private JobProperties _jobProps;
        private ServiceProperties _serviceProps;

        private string _realCurrentDirectory;

        public ProcessWindow(ProcessItem process)
        {
            InitializeComponent();

            _processItem = process;
            _pid = process.Pid;

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

            plotterCPUUsage.Data1 = _processItem.FloatHistoryManager[ProcessStats.CpuKernel];
            plotterCPUUsage.Data2 = _processItem.FloatHistoryManager[ProcessStats.CpuUser];
            plotterCPUUsage.GetToolTip = i =>
                ((plotterCPUUsage.Data1[i] + plotterCPUUsage.Data2[i]) * 100).ToString("N2") +
                "% (K: " + (plotterCPUUsage.Data1[i] * 100).ToString("N2") +
                "%, U: " + (plotterCPUUsage.Data2[i] * 100).ToString("N2") + "%)" + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterMemory.LongData1 = _processItem.LongHistoryManager[ProcessStats.PrivateMemory];
            plotterMemory.LongData2 = _processItem.LongHistoryManager[ProcessStats.WorkingSet];
            plotterMemory.GetToolTip = i =>
                "Pvt. Memory: " + Misc.GetNiceSizeName(plotterMemory.LongData1[i]) + "\n" +
                "Working Set: " + Misc.GetNiceSizeName(plotterMemory.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterIO.LongData1 = _processItem.LongHistoryManager[ProcessStats.IoReadOther];
            plotterIO.LongData2 = _processItem.LongHistoryManager[ProcessStats.IoWrite];
            plotterIO.GetToolTip = i =>
                "R+O: " + Misc.GetNiceSizeName(plotterIO.LongData1[i]) + "\n" +
                "W: " + Misc.GetNiceSizeName(plotterIO.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
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
                tabControl.TabPages.Remove(tabJob);
                tabControl.TabPages.Remove(tabEnvironment);
            }

            if (_pid == 0)
                textFileDescription.Text = "System Idle Process";

            // add our handler to the process provider
            Program.ProcessProvider.Updated += 
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);

            // HACK: Delay loading
            System.Threading.Timer t = null;

            t = new System.Threading.Timer(
                new System.Threading.TimerCallback(o =>
                {
                    t.Dispose();
                    this.BeginInvoke(new MethodInvoker(this.LoadStage2));
                }),
                null, 100, System.Threading.Timeout.Infinite);

            SymbolProvider.ShowWarning(this, false);
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

            this.UpdateEnvironmentVariables();

            // disable providers which aren't in use
            tabControl_SelectedIndexChanged(null, null);

            this.ResumeLayout();
        }

        private void ProcessWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;

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
                (_tokenProps.Object as ProcessHandle).Dispose();
            }

            if (_jobProps != null)
            {
                _jobProps.SaveSettings();
                _jobProps.JobObject.Dispose();
            }

            if (_serviceProps != null)
            {
                _serviceProps.SaveSettings();
            }

            timerUpdate.Enabled = false;

            if (_processImage != null)
            {
                pictureIcon.Image = null;
                _processImage.Dispose();
            }

            Program.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);

            Properties.Settings.Default.EnvironmentListViewColumns = ColumnSettings.SaveSettings(listEnvironment);
            Properties.Settings.Default.ProcessWindowSelectedTab = tabControl.SelectedTab.Name;
            Properties.Settings.Default.SearchType = buttonSearch.Text;

            if (this.WindowState == FormWindowState.Normal)
                Properties.Settings.Default.ProcessWindowSize = this.Size;
        }

        private void ProcessWindow_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate(true);
        }

        public void ApplyFont(Font f)
        {
            listThreads.List.Font = f;
            listModules.List.Font = f;
            listMemory.List.Font = f;
            listHandles.List.Font = f;
            listEnvironment.Font = f;

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
                    Icon icon = FileUtils.GetFileIcon(fileName, true);
                    
                    pictureIcon.Image = _processImage = icon.ToBitmap();

                    Win32.DestroyIcon(icon.Handle);
                }
                catch 
                {
                    pictureIcon.Image = _processImage = ProcessHacker.Properties.Resources.Process.ToBitmap();
                }

                var verifyResult = _processItem.VerifyResult;

                if (verifyResult == VerifyResult.Unknown)
                    textFileCompany.Text += "";
                else if (verifyResult == VerifyResult.Trusted)
                    textFileCompany.Text += " (verified)";
                else if (verifyResult == VerifyResult.TrustedInstaller)
                    textFileCompany.Text += " (verified, Windows component)";
                else if (verifyResult == VerifyResult.NoSignature)
                    textFileCompany.Text += " (not verified, no signature)";
                else if (verifyResult == VerifyResult.Distrust)
                    textFileCompany.Text += " (not verified, distrusted)";
                else if (verifyResult == VerifyResult.Expired)
                    textFileCompany.Text += " (not verified, expired)";
                else if (verifyResult == VerifyResult.Revoked)
                    textFileCompany.Text += " (not verified, revoked)";
                else if (verifyResult == VerifyResult.SecuritySettings)
                    textFileCompany.Text += " (not verified, security settings)";
                else
                    textFileCompany.Text += " (not verified)";
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
                using (ProcessHandle phandle
                    = new ProcessHandle(_pid, Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                {
                    fileCurrentDirectory.Text =
                        phandle.GetPebString(PebOffset.CurrentDirectoryPath);
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
                using (ProcessHandle phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
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
                if (Program.ProcessProvider.Dictionary.ContainsKey(_processItem.ParentPid))
                {
                    textParent.Text =
                        Program.ProcessProvider.Dictionary[_processItem.ParentPid].Name +
                        " (" + _processItem.ParentPid.ToString() + ")";
                }
                else
                {
                    textParent.Text = "Non-existent Process (" + _processItem.ParentPid.ToString() + ")";
                    buttonInspectParent.Enabled = false;
                }
            }
            else if (_processItem.ParentPid == -1)
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
                textParent.Text = "Non-existent Process (" + _processItem.ParentPid.ToString() + ")";
                buttonInspectParent.Enabled = false;
            }

            this.UpdateProtected();
            this.UpdateDEPStatus();
        }

        private void InitializeSubControls()
        {
            try
            {
                _tokenProps = new TokenProperties(new ProcessHandle(_pid, Program.MinProcessQueryRights));
                _tokenProps.Dock = DockStyle.Fill;
                tabToken.Controls.Add(_tokenProps);
            }
            catch
            { }

            try
            {
                using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                {
                    var jhandle = phandle.GetJob(JobObjectAccess.Query);

                    _jobProps = new JobProperties(jhandle);
                    _jobProps.Dock = DockStyle.Fill;
                    tabJob.Controls.Add(_jobProps);
                }
            }
            catch
            {
                tabControl.TabPages.Remove(tabJob);
            }

            if (Program.HackerWindow != null)
            {
                _serviceProps = new ServiceProperties(
                    Program.HackerWindow.ProcessServices.ContainsKey(_pid) ?
                    Program.HackerWindow.ProcessServices[_pid].ToArray() :
                    new string[0]);
                _serviceProps.Dock = DockStyle.Fill;
                _serviceProps.PID = _pid;
                tabServices.Controls.Add(_serviceProps);
            }
            else
            {
                tabControl.TabPages.Remove(tabServices);
            }

            listEnvironment.ListViewItemSorter = new SortedListComparer(listEnvironment);
            listEnvironment.SetDoubleBuffered(true);
            listEnvironment.SetTheme("explorer");
            listEnvironment.ContextMenu = listEnvironment.GetCopyMenu();
            ColumnSettings.LoadSettings(Properties.Settings.Default.EnvironmentListViewColumns, listEnvironment);
        }

        private void InitializeProviders()
        {
            listThreads.BeginUpdate();
            listThreads.Highlight = false;
            _threadP = new ThreadProvider(_pid);
            Program.SecondarySharedThreadProvider.Add(_threadP);
            _threadP.Interval = Properties.Settings.Default.RefreshInterval;
            _threadP.Updated += new ThreadProvider.ProviderUpdateOnce(_threadP_Updated);
            listThreads.Provider = _threadP;
            //_threadP.RunOnceAsync();

            listModules.BeginUpdate();
            listModules.Highlight = false;
            _moduleP = new ModuleProvider(_pid);
            Program.SecondarySharedThreadProvider.Add(_moduleP);
            _moduleP.Interval = Properties.Settings.Default.RefreshInterval;
            _moduleP.Updated += new ModuleProvider.ProviderUpdateOnce(_moduleP_Updated);
            listModules.Provider = _moduleP;
            _moduleP.RunOnceAsync();

            listMemory.BeginUpdate();
            listMemory.Highlight = false;
            _memoryP = new MemoryProvider(_pid);
            Program.SecondarySharedThreadProvider.Add(_memoryP);
            _memoryP.IgnoreFreeRegions = true;
            _memoryP.Interval = Properties.Settings.Default.RefreshInterval;
            _memoryP.Updated += new MemoryProvider.ProviderUpdateOnce(_memoryP_Updated);
            listMemory.Provider = _memoryP;
            //_memoryP.RunOnceAsync();

            listHandles.BeginUpdate();
            listHandles.Highlight = false;
            _handleP = new HandleProvider(_pid);
            Program.SecondarySharedThreadProvider.Add(_handleP);
            _handleP.HideHandlesWithNoName = Properties.Settings.Default.HideHandlesWithNoName;
            _handleP.Interval = Properties.Settings.Default.RefreshInterval;
            _handleP.Updated += new HandleProvider.ProviderUpdateOnce(_handleP_Updated);
            listHandles.Provider = _handleP;
            //_handleP.RunOnceAsync();

            listThreads.List.SetTheme("explorer");
            listModules.List.SetTheme("explorer");
            listMemory.List.SetTheme("explorer");
            listHandles.List.SetTheme("explorer");

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
            listEnvironment.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listEnvironment.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listEnvironment, -1);
                };
        }

        private void UpdateEnvironmentVariables()
        {
            listEnvironment.Items.Clear();

            listEnvironment.BeginUpdate();
            try
            {
                using (ProcessHandle phandle = new ProcessHandle(_pid,
                    ProcessAccess.QueryInformation | Program.MinProcessReadMemoryRights))
                {
                    foreach (var pair in phandle.GetEnvironmentVariables())
                    {
                        if (pair.Key != "")
                            listEnvironment.Items.Add(new ListViewItem(new string[] { pair.Key, pair.Value }));
                    }
                }
            }
            catch
            { }
            listEnvironment.EndUpdate();
        }

        public void UpdateProtected()
        {
            labelProtected.Enabled = true;
            textProtected.Enabled = true;
            buttonEditProtected.Enabled = true;

            if (KProcessHacker.Instance != null && OSVersion.HasProtectedProcesses)
            {
                try
                {
                    textProtected.Text = KProcessHacker.Instance.GetProcessProtected(_pid) ? "Protected" : "Not Protected";
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
                using (var phandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation))
                {
                    var depStatus = phandle.GetDepStatus();
                    string str;

                    if ((depStatus & DepStatus.Enabled) != 0)
                    {
                        str = "Enabled";
                    }
                    else
                    {
                        str = "Disabled";
                    }

                    if ((depStatus & DepStatus.Permanent) != 0)
                    {
                        buttonEditDEP.Enabled = false;
                        str += ", Permanent";
                    }

                    if ((depStatus & DepStatus.AtlThunkEmulationDisabled) != 0)
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

        private void UpdatePerformance()
        {
            ProcessSystemProvider sysProvider = Program.ProcessProvider;

            if (!sysProvider.Dictionary.ContainsKey(_pid))
                return;

            ProcessItem item = sysProvider.Dictionary[_pid];

            plotterCPUUsage.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            plotterCPUUsage.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
            plotterMemory.LineColor1 = Properties.Settings.Default.PlotterMemoryPrivateColor;
            plotterMemory.LineColor2 = Properties.Settings.Default.PlotterMemoryWSColor;
            plotterIO.LineColor1 = Properties.Settings.Default.PlotterIOROColor;
            plotterIO.LineColor2 = Properties.Settings.Default.PlotterIOWColor;

            // update graphs
            long sysTotal = sysProvider.LongDeltas[SystemStats.CpuKernel] + sysProvider.LongDeltas[SystemStats.CpuUser]
                + sysProvider.LongDeltas[SystemStats.CpuOther];
            float procKernel = (float)item.DeltaManager[ProcessStats.CpuKernel] / sysTotal;
            float procUser = (float)item.DeltaManager[ProcessStats.CpuUser] / sysTotal;  
            long ioRO = item.DeltaManager[ProcessStats.IoRead] + item.DeltaManager[ProcessStats.IoOther];
            long ioW = item.DeltaManager[ProcessStats.IoWrite];

            plotterCPUUsage.Text = ((procKernel + procUser) * 100).ToString("F2") +
                "% (K: " + (procKernel * 100).ToString("F2") +
                "%, U: " + (procUser * 100).ToString("F2") + "%)";

            plotterMemory.Text = "Pvt: " + Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PrivateBytes) + 
                ", WS: " + Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.WorkingSetSize);

            plotterIO.Text = "R+O: " + Misc.GetNiceSizeName(ioRO) + ", W: " + Misc.GetNiceSizeName(ioW);

            plotterCPUUsage.MoveGrid();
            plotterCPUUsage.Draw();
            plotterMemory.MoveGrid();
            plotterMemory.Draw();
            plotterIO.MoveGrid();
            plotterIO.Draw();
        }

        private void UpdateStatistics()
        {
            if (!Program.ProcessProvider.Dictionary.ContainsKey(_pid))
                return;

            ProcessItem item = Program.ProcessProvider.Dictionary[_pid];

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
            labelMemoryPF.Text = ((ulong)item.Process.VirtualMemoryCounters.PageFaultCount).ToString("N0");

            labelIOReads.Text = ((ulong)item.Process.IoCounters.ReadOperationCount).ToString("N0");
            labelIOReadBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.ReadTransferCount);
            labelIOWrites.Text = ((ulong)item.Process.IoCounters.WriteOperationCount).ToString("N0");
            labelIOWriteBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.WriteTransferCount);
            labelIOOther.Text = ((ulong)item.Process.IoCounters.OtherOperationCount).ToString("N0");
            labelIOOtherBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.OtherTransferCount);

            labelOtherHandles.Text = ((ulong)item.Process.HandleCount).ToString("N0");

            try
            {
                using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                {
                    labelOtherGDIHandles.Text = phandle.GetGuiResources(false).ToString("N0");
                    labelOtherUSERHandles.Text = phandle.GetGuiResources(true).ToString("N0");
                }
            }
            catch
            { }
        }

        private void fileCurrentDirectory_TextBoxLeave(object sender, EventArgs e)
        {
            if (fileCurrentDirectory.Text != _realCurrentDirectory)
                fileCurrentDirectory.Text = _realCurrentDirectory;
        }

        #region Buttons

        private void buttonTerminate_Click(object sender, EventArgs e)
        {
            ProcessActions.Terminate(this, new int[] { _processItem.Pid }, new string[] { _processItem.Name }, true);
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
                    KProcessHacker.Instance.SetProcessProtected(_pid, picker.SelectedItem == "Protect");
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

                using (ProcessHandle phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
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
                    Program.ProcessProvider.Dictionary[_processItem.ParentPid],
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
                    if (tabControl.SelectedTab == tabStatistics)
                    {
                        this.UpdateStatistics();
                    }
                    else if (tabControl.SelectedTab == tabPerformance)
                    {
                        this.UpdatePerformance();
                    }
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
                    listMemory.Refresh();
                    listMemory.Highlight = true;
                    checkHideFreeRegions.Enabled = true;
                    this.Cursor = Cursors.Default;
                }));
                _memoryP.Updated -= new MemoryProvider.ProviderUpdateOnce(_memoryP_Updated);
            }
        }

        private void _handleP_Updated()
        {
            if (_handleP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listHandles.EndUpdate();
                    listHandles.Refresh();
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
                    listModules.Refresh();
                    listModules.Highlight = true;
                }));
                _moduleP.Updated -= new ModuleProvider.ProviderUpdateOnce(_moduleP_Updated);
            }
        }

        private void _threadP_Updated()
        {
            if (_threadP.RunCount > 1)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    listThreads.EndUpdate();
                    listThreads.Refresh();
                    listThreads.Highlight = true;
                }));
                _threadP.Updated -= new ThreadProvider.ProviderUpdateOnce(_threadP_Updated);
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

            if (tabControl.SelectedTab == tabStatistics)
            {
                this.UpdateStatistics();
            }

            if (tabControl.SelectedTab == tabPerformance)
            {
                this.UpdatePerformance();
            }

            if (_jobProps != null)
            {
                if (tabControl.SelectedTab == tabJob)
                {
                    _jobProps.UpdateEnabled = true;
                }
                else
                {
                    _jobProps.UpdateEnabled = false;
                }
            }
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
                        using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
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
                    = new ProcessHandle(_pid, Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                {
                    _realCurrentDirectory  =
                        phandle.GetPebString(PebOffset.CurrentDirectoryPath);

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
