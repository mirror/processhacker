/*
 * Process Hacker - 
 *   main Process Hacker window
 * 
 * Copyright (C) 2008 Dean
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        delegate void QueueUpdatedCallback();
        delegate void AddIconCallback(Icon icon);
        delegate void AddListViewItemCallback(ListView lv, string[] text);

        #region Variables

        public int RefreshInterval = 1000;

        public HelpWindow HelpForm = new HelpWindow();
        public SysInfoWindow SysInfoWindow = null;
        public HandleFilterWindow HandleFilterForm = new HandleFilterWindow();

        ProcessSystemProvider processP = new ProcessSystemProvider();
        ServiceProvider serviceP = new ServiceProvider();
        NetworkProvider networkP = new NetworkProvider();

        UsageIcon cpuUsageIcon = new UsageIcon(16, 16);

        Dictionary<int, List<string>> processServices = new Dictionary<int, List<string>>();

        int processSelectedItems;
        int processSelectedPID;
        Process processSelected;

        List<Control> listControls = new List<Control>();

        Queue<KeyValuePair<string, Icon>> statusMessages = new Queue<KeyValuePair<string, Icon>>();
        List<string> log = new List<string>();

        #endregion

        #region Properties

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public NotifyIcon NotifyIcon
        {
            get { return notifyIcon; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        public ProcessSystemProvider ProcessProvider
        {
            get { return processP; }
        }

        public ServiceProvider ServiceProvider
        {
            get { return serviceP; }
        }

        public NetworkProvider NetworkProvider
        {
            get { return networkP; }
        }

        public ProcessTree ProcessList
        {
            get { return treeProcesses; }
        }

        public Dictionary<int, List<string>> ProcessServices
        {
            get { return processServices; }
        }

        #endregion

        #region Events

        #region Buttons

        //private void buttonSearch_Click(object sender, EventArgs e)
        //{
        //    PerformSearch(buttonSearch.Text);
        //}

        #endregion

        #region Lists

        private void listProcesses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                terminateMenuItem_Click(null, null);
            }
        }

        private void listProcesses_SelectionChanged(object sender, EventArgs e)
        {
            processSelectedItems = treeProcesses.SelectedNodes.Count;

            if (processSelectedItems == 1)
            {
                processSelectedPID = treeProcesses.SelectedNodes[0].PID;

                try
                {
                    try
                    {
                        if (processSelected != null)
                            processSelected.Close();
                    }
                    catch
                    { }

                    processSelected = Process.GetProcessById(processSelectedPID);
                }
                catch
                {
                    processSelected = null;
                }
            }
            else
            {
                processSelectedPID = -1;

                try
                {
                    if (processSelected != null)
                        processSelected.Close();
                }
                catch
                { }

                processSelected = null;
            }
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            propertiesServiceMenuItem_Click(null, null);
        }

        #endregion

        #region Main Menu

        private void selectAllHackerMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control c in listControls)
            {
                if (c.Focused)
                {
                    if (c is ListView)
                    {
                        Misc.SelectAll((c as ListView).Items);
                    }
                    else if (c is TreeViewAdv)
                    {
                        Misc.SelectAll((c as TreeViewAdv).AllNodes);
                    }
                }
            }
        }

        private void runAsMenuItem_Click(object sender, EventArgs e)
        {
             RunWindow run = new RunWindow();

            run.TopMost = this.TopMost;
            run.ShowDialog();
        }

        private void findHandlesMenuItem_Click(object sender, EventArgs e)
        {
            HandleFilterForm.Show();
            HandleFilterForm.Activate();
        }

        private void inspectPEFileMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PEWindow pw = Program.GetPEWindow(ofd.FileName, new Program.PEWindowInvokeAction(delegate(PEWindow f)
                {
                    try
                    {
                        f.Show();
                    }
                    catch
                    { }
                }));
            }
        }

        private void reloadStructsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Program.Structs.Clear();
                Structs.StructParser parser = new ProcessHacker.Structs.StructParser(Program.Structs);

                parser.Parse(Application.StartupPath + "\\structs.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void logMenuItem_Click(object sender, EventArgs e)
        {
            string str = "";

            foreach (string item in log)
                str += item + "\r\n";

            InformationBox box = new InformationBox(str);

            box.TopMost = this.TopMost;
            box.ShowDialog();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();

            about.TopMost = this.TopMost;
            about.ShowDialog();
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            OptionsWindow options = new OptionsWindow();

            options.TopMost = this.TopMost;
            options.ShowDialog();

            RefreshInterval = Properties.Settings.Default.RefreshInterval;
            timerFire.Interval = RefreshInterval;
        }

        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm.Show();
            HelpForm.Activate();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }     

        private void sysInfoMenuItem_Click(object sender, EventArgs e)
        {
            if (SysInfoWindow == null)
            {
                SysInfoWindow = new SysInfoWindow();
            }

            SysInfoWindow.Show();
            SysInfoWindow.Activate();
            SysInfoWindow.Start();
        }

        #endregion

        #region Notification Icon & Menu

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            showHideMenuItem_Click(null, null);
        }

        private void showHideMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Process Context Menu

        private void menuProcess_Popup(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedTreeNodes.Count == 0)
            {
                Misc.DisableAllMenuItems(menuProcess);
            }
            else if (treeProcesses.SelectedTreeNodes.Count == 1)
            {
                Misc.EnableAllMenuItems(menuProcess);

                priorityMenuItem.Text = "&Priority";
                terminateMenuItem.Text = "&Terminate Process";
                suspendMenuItem.Text = "&Suspend Process";
                resumeMenuItem.Text = "&Resume Process";

                realTimeMenuItem.Checked = false;
                highMenuItem.Checked = false;
                aboveNormalMenuItem.Checked = false;
                normalMenuItem.Checked = false;
                belowNormalMenuItem.Checked = false;
                idleMenuItem.Checked = false;

                try
                {
                    switch (Process.GetProcessById(processSelectedPID).PriorityClass)
                    {
                        case ProcessPriorityClass.RealTime:
                            realTimeMenuItem.Checked = true;
                            break;

                        case ProcessPriorityClass.High:
                            highMenuItem.Checked = true;
                            break;

                        case ProcessPriorityClass.AboveNormal:
                            aboveNormalMenuItem.Checked = true;
                            break;

                        case ProcessPriorityClass.Normal:
                            normalMenuItem.Checked = true;
                            break;

                        case ProcessPriorityClass.BelowNormal:
                            belowNormalMenuItem.Checked = true;
                            break;

                        case ProcessPriorityClass.Idle:
                            idleMenuItem.Checked = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    priorityMenuItem.Text = "(" + ex.Message + ")";
                    priorityMenuItem.Enabled = false;
                }
            }
            else
            {
                Misc.DisableAllMenuItems(menuProcess);

                terminateMenuItem.Text = "&Terminate Processes";
                suspendMenuItem.Text = "&Suspend Processes";
                resumeMenuItem.Text = "&Resume Processes";

                terminateMenuItem.Enabled = true;
                suspendMenuItem.Enabled = true;
                resumeMenuItem.Enabled = true;
                copyProcessMenuItem.Enabled = true;
            }

            if (processSelectedPID < 0 && treeProcesses.SelectedNodes.Count == 1)
            {
                // probably DPCs or Interrupts
                priorityMenuItem.Text = "&Priority";
                Misc.DisableAllMenuItems(menuProcess);
                propertiesProcessMenuItem.Enabled = true;
            }

            if (treeProcesses.Model.Nodes.Count == 0)
            {
                selectAllProcessMenuItem.Enabled = false;
            }
            else
            {
                selectAllProcessMenuItem.Enabled = true;
            }
        }

        private void terminateMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to terminate the selected process(es)?", 
                "Process Hacker", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                foreach (ProcessNode node in treeProcesses.SelectedNodes)
                {
                    try
                    {
                        using (Win32.ProcessHandle handle = new Win32.ProcessHandle(node.PID,
                            Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                            handle.Terminate();
                    }
                    catch (Exception ex)
                    {
                        DialogResult result = MessageBox.Show("Could not terminate process \"" + node.Name +
                            "\" with PID " + node.PID.ToString() + ":\n\n" +
                                ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                        if (result == DialogResult.Cancel)
                            return;
                    }
                }
            }
        }

        private void suspendMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ProcessNode node in treeProcesses.SelectedNodes)
            {
                if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(processSelectedPID))
                {
                    DialogResult result = MessageBox.Show("The process with PID " + processSelectedPID + " is a system process. Are you" +
                        " sure you want to suspend it?", "Process Hacker", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.No)
                        continue;
                    else if (result == DialogResult.Cancel)
                        return;
                }

                try
                {
                    using (Win32.ProcessHandle handle = new Win32.ProcessHandle(node.PID,
                        Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        handle.Suspend();
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not suspend process \"" + node.Name +
                        "\" with PID " + node.PID.ToString() + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void resumeMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ProcessNode node in treeProcesses.SelectedNodes)
            {
                if (Properties.Settings.Default.WarnDangerous && IsDangerousPID(processSelectedPID))
                {
                    DialogResult result = MessageBox.Show("The process with PID " + processSelectedPID + " is a system process. Are you" +
                        " sure you want to resume it?", "Process Hacker", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                    if (result == DialogResult.No)
                        continue;
                    else if (result == DialogResult.Cancel)
                        return;
                }

                try
                {
                    using (Win32.ProcessHandle handle = new Win32.ProcessHandle(node.PID,
                        Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        handle.Resume();
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Could not resume process \"" + node.Name +
                        "\" with PID " + node.PID.ToString() + ":\n\n" +
                            ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void inspectProcessMenuItem_Click(object sender, EventArgs e)
        {
            // user hasn't got any processes selected
            if (processSelectedPID == -1)
                return;

            try
            {
                ProcessWindow pForm = Program.GetProcessWindow(processP.Dictionary[processSelectedPID],
                    new Program.PWindowInvokeAction(delegate(ProcessWindow f)
                {
                    f.Show();
                    f.Activate();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not inspect the process:\n\n" + ex.Message,
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void affinityProcessMenuItem_Click(object sender, EventArgs e)
        {
            ProcessAffinity affForm = new ProcessAffinity(processSelectedPID);

            try
            {
                affForm.TopMost = this.TopMost;
                affForm.ShowDialog();
            }
            catch
            { }
        }

        private void servicesProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceWindow sw = new ServiceWindow(processServices[processSelectedPID].ToArray());

                sw.TopMost = this.TopMost;
                sw.ShowDialog();
            }
            catch
            { }
        }

        private void tokenMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ProcessHandle process = new Win32.ProcessHandle(processSelectedPID,
                        Program.MinProcessQueryRights))
                {
                    TokenWindow tokForm = new TokenWindow(process);

                    tokForm.TopMost = this.TopMost;
                    tokForm.Text = "Token - " + processP.Dictionary[processSelectedPID].Name +
                        " (PID " + processSelectedPID.ToString() + ")";
                    tokForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void terminatorProcessMenuItem_Click(object sender, EventArgs e)
        {
            TerminatorWindow w = new TerminatorWindow(processSelectedPID);

            w.Text = "Terminator - " + processP.Dictionary[processSelectedPID].Name + 
                " (PID " + processSelectedPID.ToString() + ")";

            w.ShowDialog();
        }

        #region Run As

        private void launchAsUserProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.RunAsCommand = processP.Dictionary[processSelectedPID].FileName;

                RunWindow run = new RunWindow();

                run.TopMost = this.TopMost;
                run.ShowDialog();
            }
            catch
            { }
        }

        private void launchAsThisUserProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RunWindow run = new RunWindow();

                run.TopMost = this.TopMost;
                run.UsePID(processSelectedPID);
                run.ShowDialog();
            }
            catch
            { }
        }

        #endregion

        #region Injector

        private void startProcessProcessMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox box = new PromptBox();

            box.TextBox.AutoCompleteSource = AutoCompleteSource.FileSystem;
            box.TextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            if (box.ShowDialog() == DialogResult.OK)
            {
                ProcessStartInfo info = new ProcessStartInfo();

                info.FileName = Application.StartupPath + "\\Injector.exe";
                info.Arguments = "createprocessc " + processSelectedPID.ToString() + " \"" + 
                    box.Value.Replace("\"", "\\\"") + "\"";
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                Process p = Process.Start(info);

                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    InformationBox infoBox = new InformationBox(p.StandardOutput.ReadToEnd() + "\r\nReturn code: " + p.ExitCode +
                        " (" + Win32.GetErrorMessage(p.ExitCode) + ")");

                    infoBox.ShowDialog();
                }
            }
        }

        private void getCommandLineProcessMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = Application.StartupPath + "\\Injector.exe";
            info.Arguments = "cmdline " + processSelectedPID.ToString();
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process p = Process.Start(info);

            p.WaitForExit();

            InformationBox infoBox = new InformationBox(p.StandardOutput.ReadToEnd() + (p.ExitCode != 0 ? "\r\nReturn code: " + p.ExitCode + 
                " (" + Win32.GetErrorMessage(p.ExitCode) + ")" : ""));

            infoBox.ShowDialog();
        }

        private void exitProcessProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to terminate the selected process?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;

            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = Application.StartupPath + "\\Injector.exe";
            info.Arguments = "exitprocess " + processSelectedPID.ToString();
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            Process p = Process.Start(info);

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                InformationBox infoBox = new InformationBox(p.StandardOutput.ReadToEnd() + "\r\nReturn code: " + p.ExitCode +
                    " (" + Win32.GetErrorMessage(p.ExitCode) + ")");

                infoBox.ShowDialog();
            }
        }

        #endregion

        #region Priority

        private void realTimeMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.RealTime);
        }

        private void highMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.High);
        }

        private void aboveNormalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.AboveNormal);
        }

        private void normalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.Normal);
        }

        private void belowNormalMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.BelowNormal);
        }

        private void idleMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessPriority(ProcessPriorityClass.Idle);
        }

        #endregion

        private void searchProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.SearchEngine.Replace("%s",
                    treeProcesses.SelectedNodes[0].Name));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void selectAllProcessMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(treeProcesses.Tree.AllNodes);
        } 

        #endregion

        #region Providers

        private void processP_Updated()
        {
            processP.DictionaryAdded += new ProcessSystemProvider.ProviderDictionaryAdded(processP_DictionaryAdded);
            processP.DictionaryRemoved += new ProcessSystemProvider.ProviderDictionaryRemoved(processP_DictionaryRemoved);
            processP.Updated -= new ProcessSystemProvider.ProviderUpdateOnce(processP_Updated);

            if (processP.RunCount >= 1)
                this.Invoke(new MethodInvoker(UpdateCommon));
        }

        private void processP_IconUpdater()
        {
            cpuUsageIcon.Update(processP.CurrentCPUUsage);
            notifyIcon.Icon = cpuUsageIcon.GetIcon();
        }

        public void processP_DictionaryAdded(ProcessItem item)
        {
            ProcessItem parent = new ProcessItem();
            string parentText = "";

            if (item.HasParent)
            {
                try
                {
                    parent = processP.Dictionary[item.ParentPID];

                    parentText += " started by " + parent.Name + " (PID " + parent.PID.ToString() + ")";
                }
                catch
                { }
            }

            this.QueueMessage("New Process: " + item.Name + " (PID " + item.PID.ToString() + ")" + parentText, item.Icon);

            if (NPMenuItem.Checked)
                notifyIcon.ShowBalloonTip(2000, "New Process",
                    "The process " + item.Name + " (" + item.PID.ToString() + 
                    ") was started" + ((parentText != "") ? " by " + 
                    parent.Name + " (PID " + parent.PID.ToString() + ")" : "") + ".", ToolTipIcon.Info);
        }

        public void processP_DictionaryRemoved(ProcessItem item)
        {
            this.QueueMessage("Terminated Process: " + item.Name + " (PID " + item.PID.ToString() + ")", item.Icon);

            if (processServices.ContainsKey(item.PID))
                processServices.Remove(item.PID);

            if (TPMenuItem.Checked)
                notifyIcon.ShowBalloonTip(2000, "Terminated Process",
                    "The process " + item.Name + " (" + item.PID.ToString() + ") was terminated.", ToolTipIcon.Info);
        }

        private void serviceP_Updated()
        {
            listServices.List.EndUpdate();
            HighlightedListViewItem.StateHighlighting = true;

            serviceP.DictionaryAdded += new ServiceProvider.ProviderDictionaryAdded(serviceP_DictionaryAdded);
            serviceP.DictionaryModified += new ServiceProvider.ProviderDictionaryModified(serviceP_DictionaryModified);
            serviceP.DictionaryRemoved += new ServiceProvider.ProviderDictionaryRemoved(serviceP_DictionaryRemoved);
            serviceP.Updated -= new ServiceProvider.ProviderUpdateOnce(serviceP_Updated);

            if (processP.RunCount >= 1)
                this.Invoke(new MethodInvoker(UpdateCommon));
        }

        public void serviceP_DictionaryAdded(ServiceItem item)
        {
            this.QueueMessage("New Service: " + item.Status.ServiceName +
                " (" + item.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                ((item.Status.DisplayName != "") ?
                " (" + item.Status.DisplayName + ")" :
                ""), null);

            if (NSMenuItem.Checked)
                notifyIcon.ShowBalloonTip(2000, "New Service",
                    "The service " + item.Status.ServiceName + " (" + item.Status.DisplayName + ") has been created.",
                    ToolTipIcon.Info);
        }

        public void serviceP_DictionaryAdded_Process(ServiceItem item)
        {
            if (item.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(item.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(item.Status.ServiceStatusProcess.ProcessID, new List<string>());

                processServices[item.Status.ServiceStatusProcess.ProcessID].Add(item.Status.ServiceName);
            }
        }

        public void serviceP_DictionaryModified(ServiceItem oldItem, ServiceItem newItem)
        {
            Win32.SERVICE_STATE oldState = oldItem.Status.ServiceStatusProcess.CurrentState;
            Win32.SERVICE_STATE newState = newItem.Status.ServiceStatusProcess.CurrentState;

            if ((oldState == Win32.SERVICE_STATE.Paused || oldState == Win32.SERVICE_STATE.Stopped ||
                oldState == Win32.SERVICE_STATE.StartPending) &&
                newState == Win32.SERVICE_STATE.Running)
            {
                this.QueueMessage("Service Started: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != "") ?
                    " (" + newItem.Status.DisplayName + ")" :
                    ""), null);

                if (startedSMenuItem.Checked)
                    notifyIcon.ShowBalloonTip(2000, "Service Started",
                        "The service " + newItem.Status.ServiceName + " (" + newItem.Status.DisplayName + ") has been started.",
                        ToolTipIcon.Info);
            }

            if (oldState == Win32.SERVICE_STATE.Running &&
                newState == Win32.SERVICE_STATE.Paused)
                this.QueueMessage("Service Paused: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != "") ?
                    " (" + newItem.Status.DisplayName + ")" :
                    ""), null);

            if (oldState == Win32.SERVICE_STATE.Running &&
                newState == Win32.SERVICE_STATE.Stopped)
            {
                this.QueueMessage("Service Stopped: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != "") ?
                    " (" + newItem.Status.DisplayName + ")" :
                    ""), null);

                if (stoppedSMenuItem.Checked)
                    notifyIcon.ShowBalloonTip(2000, "Service Stopped",
                        "The service " + newItem.Status.ServiceName + " (" + newItem.Status.DisplayName + ") has been stopped.",
                        ToolTipIcon.Info);
            }
        }

        public void serviceP_DictionaryModified_Process(ServiceItem oldItem, ServiceItem newItem)
        {
            ServiceItem sitem = (ServiceItem)newItem;

            if (sitem.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!processServices.ContainsKey(sitem.Status.ServiceStatusProcess.ProcessID))
                    processServices.Add(sitem.Status.ServiceStatusProcess.ProcessID, new List<string>());

                if (!processServices[sitem.Status.ServiceStatusProcess.ProcessID].Contains(
                    sitem.Status.ServiceName))
                    processServices[sitem.Status.ServiceStatusProcess.ProcessID].Add(sitem.Status.ServiceName);

                processServices[sitem.Status.ServiceStatusProcess.ProcessID].Sort();
            }
            else
            {
                int oldId = ((ServiceItem)oldItem).Status.ServiceStatusProcess.ProcessID;

                if (processServices.ContainsKey(oldId))
                {
                    if (processServices[oldId].Contains(
                        sitem.Status.ServiceName))
                        processServices[oldId].Remove(sitem.Status.ServiceName);
                }
            }
        }

        public void serviceP_DictionaryRemoved(ServiceItem item)
        {
            this.QueueMessage("Deleted Service: " + item.Status.ServiceName +
                " (" + item.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                ((item.Status.DisplayName != "") ?
                " (" + item.Status.DisplayName + ")" :
                ""), null);

            if (DSMenuItem.Checked)
                notifyIcon.ShowBalloonTip(2000, "Service Deleted",
                    "The service " + item.Status.ServiceName + " (" + item.Status.DisplayName + ") has been deleted.",
                    ToolTipIcon.Info);
        }

        public void serviceP_DictionaryRemoved_Process(ServiceItem item)
        {
            if (item.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (processServices.ContainsKey(item.Status.ServiceStatusProcess.ProcessID))
                {
                    if (processServices[item.Status.ServiceStatusProcess.ProcessID].Contains(
                        item.Status.ServiceName))
                        processServices[item.Status.ServiceStatusProcess.ProcessID].Remove(item.Status.ServiceName);
                }
            }
        }

        #endregion

        #region Service Context Menu

        private void menuService_Popup(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count == 0)
            {
                Misc.DisableAllMenuItems(menuService);
                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                selectAllServiceMenuItem.Enabled = true;
            }
            else if (listServices.SelectedItems.Count == 1)
            {
                Misc.EnableAllMenuItems(menuService);

                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                try
                {
                    ServiceItem item = serviceP.Dictionary[listServices.SelectedItems[0].Name];

                    if (item.Status.ServiceStatusProcess.ProcessID != 0)
                    {
                        goToProcessServiceMenuItem.Enabled = true;
                    }
                    else
                    {
                        goToProcessServiceMenuItem.Enabled = false;
                    }
                          
                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & Win32.SERVICE_ACCEPT.PauseContinue)
                        == 0)
                    {
                        continueServiceMenuItem.Visible = false;
                        pauseServiceMenuItem.Visible = false;
                    }
                    else
                    {
                        continueServiceMenuItem.Visible = true;
                        pauseServiceMenuItem.Visible = true;
                    }

                    if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Paused)
                    {                                        
                        startServiceMenuItem.Enabled = false;
                        pauseServiceMenuItem.Enabled = false;
                    }
                    else if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Running)
                    {
                        startServiceMenuItem.Enabled = false;
                        continueServiceMenuItem.Enabled = false;
                    }
                    else if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Stopped)
                    {
                        pauseServiceMenuItem.Enabled = false;
                        stopServiceMenuItem.Enabled = false;
                    }

                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & Win32.SERVICE_ACCEPT.Stop) == 0 &&
                        item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Running)
                    {
                        stopServiceMenuItem.Enabled = false;
                    }
                }
                catch
                {
                    Misc.DisableAllMenuItems(menuService);
                    copyServiceMenuItem.Enabled = true;
                    propertiesServiceMenuItem.Enabled = true;
                }
            }
            else
            {
                Misc.DisableAllMenuItems(menuService);

                goToProcessServiceMenuItem.Visible = false;
                startServiceMenuItem.Visible = false;
                continueServiceMenuItem.Visible = false;
                pauseServiceMenuItem.Visible = false;
                stopServiceMenuItem.Visible = false;

                copyServiceMenuItem.Enabled = true;
                propertiesServiceMenuItem.Enabled = true;
                selectAllServiceMenuItem.Enabled = true;
            }

            if (listServices.List.Items.Count == 0)
                selectAllServiceMenuItem.Enabled = false;
        }

        private void goToProcessServiceMenuItem_Click(object sender, EventArgs e)
        {
            DeselectAll(treeProcesses.Tree);

            try
            {
                TreeNodeAdv node = treeProcesses.FindTreeNode(serviceP.Dictionary[
                    listServices.SelectedItems[0].Name].Status.ServiceStatusProcess.ProcessID);
                
                node.IsSelected = true;
                node.EnsureVisible();

                tabControlBig.SelectedTab = tabProcesses;
            }
            catch
            { }
        }

        private void startServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void continueServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Continue);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error continuing service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pauseServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Pause);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pausing service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopServiceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the service '" + 
                listServices.SelectedItems[0].Name + "'?", 
                "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                        listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                        service.Delete();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting service:\n\n" + ex.Message, "Process Hacker",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void propertiesServiceMenuItem_Click(object sender, EventArgs e)
        {
            List<string> selected = new List<string>();
            ServiceWindow sw;

            foreach (ListViewItem item in listServices.SelectedItems)
                selected.Add(item.Name);

            if (selected.Count == 1)
            {
                sw = new ServiceWindow(selected[0]);
            }
            else
            {
                sw = new ServiceWindow(selected.ToArray());
            }

            sw.TopMost = this.TopMost;
            sw.ShowDialog();
        }

        private void selectAllServiceMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listServices.Items);
        }

        #endregion

        #region Tab Controls

        private void tabControlBig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlBig.SelectedTab == tabNetwork)
            {
                networkP.Enabled = true;
            }
            else
            {
                networkP.Enabled = false;
            }
        }

        #endregion

        #region Timers

        private void timerFire_Tick(object sender, EventArgs e)
        {
            UpdateStatusInfo();
             
            notifyIcon.Text = "Process Hacker\n" + 
                "CPU Usage: " + (processP.CurrentCPUUsage * 100).ToString("F2") + "%";

            try
            {
                notifyIcon.Text += "\n" + processP.Dictionary[processP.PIDWithMostCPUUsage].Name +
                    ": " + processP.Dictionary[processP.PIDWithMostCPUUsage].CPUUsage.ToString("F2") + "%";
            }
            catch
            { }
        }

        private void timerMessages_Tick(object sender, EventArgs e)
        {
            if (statusMessages.Count != 0)
            {
                KeyValuePair<string, Icon> v = statusMessages.Dequeue();
                statusText.Text = v.Key;

                if (v.Value != null)
                    statusIcon.Icon = v.Value;
                else
                    statusIcon.Icon = null;
            }
            else
            {
                statusText.Text = "";
                statusIcon.Icon = null;
            }
        }

        #endregion

        #region Trees

        private void treeProcesses_DoubleClick(object sender, EventArgs e)
        {
            inspectProcessMenuItem_Click(null, null);
        }

        #endregion

        #endregion

        #region Form-related Helper functions

        private void AddIcon(Icon icon)
        {
            imageList.Images.Add(icon);
        }

        private void AddListViewItem(ListView lv, string[] text)
        {
            ListViewItem item = new ListViewItem();

            item.Text = text[0];

            for (int i = 1; i < text.Length; i++)
            {
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, text[i]));
            }
        }

        public void DeselectAll(ListView list)
        {
            foreach (ListViewItem item in list.SelectedItems)
                item.Selected = false;
        }

        public void DeselectAll(TreeViewAdv tree)
        {
            foreach (TreeNodeAdv node in tree.AllNodes)
                node.IsSelected = false;
        }

        private void LoadSettings()
        {
            RefreshInterval = Properties.Settings.Default.RefreshInterval;
            this.Location = Properties.Settings.Default.WindowLocation;
            this.Size = Properties.Settings.Default.WindowSize;
            this.WindowState = Properties.Settings.Default.WindowState;
            PromptBox.LastValue = Properties.Settings.Default.PromptBoxText;

            ColumnSettings.LoadSettings(Properties.Settings.Default.ProcessTreeColumns, treeProcesses.Tree);
            ColumnSettings.LoadSettings(Properties.Settings.Default.ServiceListViewColumns, listServices.List);
            ColumnSettings.LoadSettings(Properties.Settings.Default.NetworkListViewColumns, listNetwork.List);
        }

        public void QueueMessage(string message)
        {
            this.QueueMessage(message, null);
        }

        public void QueueMessage(string message, Icon icon)
        {
            log.Add(DateTime.Now.ToString() + ": " + message);
            statusMessages.Enqueue(new KeyValuePair<string,Icon>(message, icon));
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.RefreshInterval = RefreshInterval;

            if (this.WindowState == FormWindowState.Normal && this.Visible)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
            }

            Properties.Settings.Default.WindowState = this.WindowState == FormWindowState.Minimized ?
                FormWindowState.Normal : this.WindowState;

            Properties.Settings.Default.PromptBoxText = PromptBox.LastValue;

            Properties.Settings.Default.ProcessTreeColumns = ColumnSettings.SaveSettings(treeProcesses.Tree);
            Properties.Settings.Default.ServiceListViewColumns = ColumnSettings.SaveSettings(listServices.List);
            Properties.Settings.Default.NetworkListViewColumns = ColumnSettings.SaveSettings(listNetwork.List);

            Properties.Settings.Default.NewProcesses = NPMenuItem.Checked;
            Properties.Settings.Default.TerminatedProcesses = TPMenuItem.Checked;
            Properties.Settings.Default.NewServices = NSMenuItem.Checked;
            Properties.Settings.Default.StartedServices = startedSMenuItem.Checked;
            Properties.Settings.Default.StoppedServices = stoppedSMenuItem.Checked;
            Properties.Settings.Default.DeletedServices = DSMenuItem.Checked;
            
            try
            {
                Properties.Settings.Default.Save();
            }
            catch
            { }
        }

        public void SelectAll(TreeViewAdv tree)
        {
            foreach (TreeNodeAdv node in tree.AllNodes)
                node.IsSelected = true;
        }

        private void UpdateStatusInfo()
        {
            statusGeneral.Text = string.Format("{0} processes", processP.Dictionary.Count);
            statusCPU.Text = "CPU: " + (processP.CurrentCPUUsage * 100).ToString("N2") + "%";
            processP.UpdatePerformance();
            statusMemory.Text = "Phys. Memory: " +
                ((float)(processP.System.NumberOfPhysicalPages - processP.Performance.AvailablePages) * 100 /
                processP.System.NumberOfPhysicalPages).ToString("N2") + "%";
        }

        #endregion

        #region Helper functions

        private bool IsDangerousPID(int pid)
        {
            if (pid == 4)
                return true;

            try
            {
                Process p = Process.GetProcessById(pid);

                foreach (string s in Misc.DangerousNames)
                {
                    if ((Environment.SystemDirectory + "\\" + s).ToLower() == Misc.GetRealPath(p.MainModule.FileName).ToLower())
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        private void SetProcessPriority(ProcessPriorityClass priority)
        {
            try
            {
                Process.GetProcessById(processSelectedPID).PriorityClass = priority;
            }
            catch (Exception ex)
            {
                MessageBox.Show("The priority could not be set:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void formViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.AlwaysOnTop = this.TopMost;

            processP.Kill();
            serviceP.Kill();
            networkP.Kill();

            notifyIcon.Visible = false;

            SaveSettings();

            if (Program.KPH != null)
                Program.KPH.Close();
        }

        public HackerWindow()
        {
            InitializeComponent();

            if (!System.IO.File.Exists(Application.StartupPath + "\\Assistant.exe"))
            {
                runAsMenuItem.Enabled = false;
                runAsProcessMenuItem.Visible = false;
            }

            this.TopMost = Properties.Settings.Default.AlwaysOnTop;
            HighlightedListViewItem.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNewProcesses;
            HighlightedListViewItem.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Properties.Settings.Default.ColorNewProcesses;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;

            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            notifyIcon.ContextMenu = menuIcon;
            notifyIcon.Visible = Properties.Settings.Default.ShowIcon;
            NPMenuItem.Checked = Properties.Settings.Default.NewProcesses;
            TPMenuItem.Checked = Properties.Settings.Default.TerminatedProcesses;
            NSMenuItem.Checked = Properties.Settings.Default.NewServices;
            startedSMenuItem.Checked = Properties.Settings.Default.StartedServices;
            stoppedSMenuItem.Checked = Properties.Settings.Default.StoppedServices;
            DSMenuItem.Checked = Properties.Settings.Default.DeletedServices;

            NPMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            TPMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            NSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            startedSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            stoppedSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);
            DSMenuItem.Click += new EventHandler(CheckedMenuItem_Click);

            try
            {
                if (System.IO.File.Exists(Application.StartupPath + "\\structs.txt"))
                {
                    Structs.StructParser parser = new ProcessHacker.Structs.StructParser(Program.Structs);

                    parser.Parse(Application.StartupPath + "\\structs.txt");
                }
            }
            catch (Exception ex)
            {
                QueueMessage("Error loading structure definitions: " + ex.Message);
            }

            try
            {
                this.Text +=
                    " [" + Win32.ProcessHandle.FromHandle(Program.CurrentProcess).
                    GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY).GetUser().GetName(true) + "]";
            }
            catch
            { }
        }

        private void CheckedMenuItem_Click(object sender, EventArgs e)
        {
            ((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
        }

        private void UpdateCommon()
        {
            timerMessages.Enabled = true;
            treeProcesses.RefreshItems();
        }

        private void HackerWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindows();

            timerFire.Interval = RefreshInterval;
            timerFire.Enabled = true;
            timerFire_Tick(null, null);

            listControls.Add(treeProcesses.Tree);
            listControls.Add(listServices);

            GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            GenericViewMenu.AddMenuItems(copyServiceMenuItem.MenuItems, listServices.List, null);

            treeProcesses.ContextMenu = menuProcess;
            listServices.ContextMenu = menuService;
            listNetwork.ContextMenu = GenericViewMenu.GetMenu(listNetwork.List);

            processP.Interval = RefreshInterval;
            treeProcesses.Provider = processP;
            processP.RunOnceAsync();
            processP.Updated += new ProcessSystemProvider.ProviderUpdateOnce(processP_Updated);
            processP.Updated += new ProcessSystemProvider.ProviderUpdateOnce(processP_IconUpdater);
            processP.Enabled = true;

            cpuUsageIcon.BackColor = Color.Black;
            cpuUsageIcon.Color = Color.Red;

            HighlightedListViewItem.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            HighlightedListViewItem.StateHighlighting = false;
            listServices.List.BeginUpdate();
            serviceP.Interval = RefreshInterval;
            listServices.Provider = serviceP;
            serviceP.RunOnceAsync();
            serviceP.DictionaryAdded += new ServiceProvider.ProviderDictionaryAdded(serviceP_DictionaryAdded_Process);
            serviceP.DictionaryModified += new ServiceProvider.ProviderDictionaryModified(serviceP_DictionaryModified_Process);
            serviceP.DictionaryRemoved += new ServiceProvider.ProviderDictionaryRemoved(serviceP_DictionaryRemoved_Process);
            serviceP.Updated += new ServiceProvider.ProviderUpdateOnce(serviceP_Updated);
            serviceP.Enabled = true;

            networkP.Interval = RefreshInterval;
            listNetwork.Provider = networkP;
            networkP.RunOnceAsync();
            networkP.Enabled = true;

            statusText.Text = "Waiting...";

            LoadSettings();

            // load symbols on a separate thread
            Thread t = new Thread(new ThreadStart(delegate
            {
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                        {
                            statusIcon.Icon = null;
                            statusText.Text = "Loading symbols for " + module.ModuleName + "...";
                        }));

                    try
                    {
                        if (!module.FileName.ToLower().EndsWith(".exe"))
                            SymbolProvider.BaseInstance.LoadSymbolsFromLibrary(module.FileName, module.BaseAddress.ToInt32());
                    }
                    catch (Exception ex)
                    {
                        QueueMessage("Could not load symbols for " + module.ModuleName + ": " + ex.Message, null);
                    }
                }

                this.BeginInvoke(new MethodInvoker(delegate
                {
                    statusIcon.Icon = null;
                    statusText.Text = "";
                }));
            }));

            t.Priority = ThreadPriority.Lowest;
            t.Start();

            tabControlBig_SelectedIndexChanged(null, null);
        }

        private void HackerWindow_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (this.NotifyIcon.Visible && Properties.Settings.Default.HideWhenMinimized)
                {
                    this.Hide();
                }
            } 
        }
    }
}
