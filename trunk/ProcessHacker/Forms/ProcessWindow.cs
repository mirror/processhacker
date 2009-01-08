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

        private DeltaManager<string, long> _processStats =
            new DeltaManager<string, long>(new Int64Subtractor());

        public ProcessWindow(ProcessItem process)
        {
            InitializeComponent();

            _processItem = process;
            _pid = process.PID;

            this.Text = process.Name + " (PID " + _pid.ToString() + ")";

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

        private void ProcessWindow_Load(object sender, EventArgs e)
        {
            // load settings
            this.Size = Properties.Settings.Default.ProcessWindowSize;
            buttonSearch.Text = Properties.Settings.Default.SearchType;
            checkHideHandlesNoName.Checked = Properties.Settings.Default.HideHandlesNoName;

            if (tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab];

            // update the Window menu
            Program.UpdateWindows();

            this.InitializeSubControls();
            this.ClearStatistics();

            // intialize everything
            try
            {
                _process = Process.GetProcessById(_pid);

                this.InitializeProviders();
                this.UpdateProcessProperties();
            }
            catch
            {
                // this "process" is probably DPCs or Interrupts, so we won't try to load any more information
                buttonEditDEP.Enabled = false;
                buttonInspectParent.Enabled = false;
                buttonInspectPEB.Enabled = false;
                buttonOpenCurDir.Enabled = false;
                buttonOpenFileNameFolder.Enabled = false;
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

            this.UpdateDeltas();

            // add our handler to the process provider
            Program.HackerWindow.ProcessProvider.Updated += 
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);

            // disable providers which aren't in use
            tabControl_SelectedIndexChanged(null, null);
        }

        private void UpdateProcessProperties()
        {
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
            else
            {
                textParent.Text = "No parent";
                buttonInspectParent.Enabled = false;
            }

            this.UpdateDEPStatus();
        }

        private void InitializeSubControls()
        {
            _tokenProps = new TokenProperties(_processItem.ProcessQueryLimitedHandle);
            _tokenProps.Dock = DockStyle.Fill;
            tabToken.Controls.Add(_tokenProps);

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
            _handleP.Interval = Properties.Settings.Default.RefreshInterval;
            _handleP.Updated += new Provider<short, HandleItem>.ProviderUpdateOnce(_handleP_Updated);
            _handleP.RunOnceAsync();
            listHandles.Provider = _handleP;
            _handleP.Enabled = true;
        }

        private void ProcessWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // don't try to save settings if we're inspecting DPCs or Interrupts
            if (_pid >= 0)
            {
                _threadP.Kill();
                _moduleP.Kill();
                _memoryP.Kill();
                _handleP.Kill();

                listThreads.SaveSettings();
                listModules.SaveSettings();
                listMemory.SaveSettings();
                listHandles.SaveSettings();
                _tokenProps.SaveSettings();
            }

            Program.HackerWindow.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);

            Properties.Settings.Default.ProcessWindowSelectedTab = tabControl.SelectedTab.Name;
            Properties.Settings.Default.SearchType = buttonSearch.Text;
            Properties.Settings.Default.ProcessWindowSize = this.Size;
        }

        public void UpdateDEPStatus()
        {
            labelDEP.Enabled = true;
            textDEP.Enabled = true;
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
            }
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
            _memoryP.IgnoreFreeRegions = checkHideFreeRegions.Checked;
        }

        private void checkHideHandlesNoName_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HideHandlesNoName = checkHideHandlesNoName.Checked;

            if (_handleP != null)
            {
                _handleP.Kill();
                listHandles.BeginUpdate();
                listHandles.Highlight = false;
                _handleP = new HandleProvider(_pid);
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

        #endregion      

        #region Tab Controls

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_threadP != null)
                _threadP.Enabled = tabControl.SelectedTab == tabThreads;
            if (_moduleP != null)
                _moduleP.Enabled = tabControl.SelectedTab == tabModules;
            if (_memoryP != null)
                _memoryP.Enabled = tabControl.SelectedTab == tabMemory;
            if (_handleP != null)
                _handleP.Enabled = tabControl.SelectedTab == tabHandles;
        }

        #endregion
    }
}
