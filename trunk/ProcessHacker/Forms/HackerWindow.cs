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
        public delegate void LogUpdatedEventHandler(KeyValuePair<DateTime, string>? value);

        private delegate void AddMenuItemDelegate(string text, EventHandler onClick);

        #region Variables

        public HelpWindow HelpForm;
        public HandleFilterWindow HandleFilterWindow;
        public CSRProcessesWindow CSRProcessesWindow;
        public LogWindow LogWindow;
        public MiniSysInfo MiniSysInfoWindow;

        Thread sysInfoThread;
        public SysInfoWindow SysInfoWindow;

        ProcessSystemProvider processP = new ProcessSystemProvider();
        ServiceProvider serviceP = new ServiceProvider();
        NetworkProvider networkP = new NetworkProvider();

        Bitmap uacShieldIcon;
        UsageIcon cpuUsageIcon;

        Dictionary<int, List<string>> processServices = new Dictionary<int, List<string>>();

        int processSelectedItems;
        int processSelectedPID;

        List<Control> listControls = new List<Control>();

        Queue<KeyValuePair<string, Icon>> statusMessages = new Queue<KeyValuePair<string, Icon>>();
        List<KeyValuePair<DateTime, string>> _log = new List<KeyValuePair<DateTime, string>>();

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

        public ProcessTree ProcessTree
        {
            get { return treeProcesses; }
        }

        public ServiceProvider ServiceProvider
        {
            get { return serviceP; }
        }

        public ListView ServiceList
        {
            get { return listServices.List; }
        }

        public NetworkProvider NetworkProvider
        {
            get { return networkP; }
        }

        public ListView NetworkList
        {
            get { return listNetwork.List; }
        }

        public IDictionary<int, List<string>> ProcessServices
        {
            get { return processServices; }
        }

        public IList<KeyValuePair<DateTime, string>> Log
        {
            get { return _log; }
        }

        #endregion

        #region Events

        public event LogUpdatedEventHandler LogUpdated;

        #endregion

        #region Event Handlers

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
            }
            else
            {
                processSelectedPID = -1;
            }
        }

        private void listNetwork_DoubleClick(object sender, EventArgs e)
        {
            goToProcessNetworkMenuItem_Click(sender, e);
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            propertiesServiceMenuItem_Click(null, null);
        }

        #endregion

        #region Main Menu

        private void runMenuItem_Click(object sender, EventArgs e)
        {
            Win32.SHRunDialog(this.Handle, 0, 0, null, null, 0);
        }

        private void runAsMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void runAsAdministratorMenuItem_Click(object sender, EventArgs e)
        {
            PromptBox box = new PromptBox();

            box.Text = "Enter the command to start";
            box.TextBox.AutoCompleteSource = AutoCompleteSource.AllSystemSources;
            box.TextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            if (box.ShowDialog() == DialogResult.OK)
            {
                Program.StartProgramAdmin(box.Value, "", null, Win32.ShowWindowType.Show, this.Handle);
            }
        }

        private void runAsServiceMenuItem_Click(object sender, EventArgs e)
        {
            RunWindow run = new RunWindow();

            run.TopMost = this.TopMost;
            run.ShowDialog();
        }

        private void showDetailsForAllProcessesMenuItem_Click(object sender, EventArgs e)
        {
            Program.StartProcessHackerAdmin("-v", () =>
                {
                    this.SaveSettings();
                    notifyIcon.Visible = false;
                    Process.GetCurrentProcess().Kill();
                }, this.Handle);
        }

        private void findHandlesMenuItem_Click(object sender, EventArgs e)
        {
            if (HandleFilterWindow == null)
                HandleFilterWindow = new HandleFilterWindow();

            HandleFilterWindow.Show();
            HandleFilterWindow.Activate();
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

        private void sysInfoMenuItem_Click(object sender, EventArgs e)
        {
            if (sysInfoThread == null || !sysInfoThread.IsAlive)
            {
                sysInfoThread = new Thread(() =>
                {
                    SysInfoWindow = new SysInfoWindow();

                    Application.Run(SysInfoWindow);
                });
                sysInfoThread.Start();
            }
            else
            {
                SysInfoWindow.BeginInvoke(new MethodInvoker(delegate
                {
                    SysInfoWindow.Show();
                    SysInfoWindow.Activate();
                }));
            }
        }

        private void logMenuItem_Click(object sender, EventArgs e)
        {
            if (LogWindow == null || LogWindow.IsDisposed)
            {
                LogWindow = new LogWindow();
            }

            LogWindow.Show();

            if (LogWindow.WindowState == FormWindowState.Minimized)
                LogWindow.WindowState = FormWindowState.Normal;

            LogWindow.Activate();
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
        }

        private void freeMemoryMenuItem_Click(object sender, EventArgs e)
        {
            Program.CollectGarbage();
        }  

        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            if (HelpForm == null)
                HelpForm = new HelpWindow();

            HelpForm.Show();
            HelpForm.Activate();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void toolbarMenuItem_Click(object sender, EventArgs e)
        {
            toolbarMenuItem.Checked = !toolbarMenuItem.Checked;
            toolStrip.Visible = toolbarMenuItem.Checked;
        }

        private void updateNowMenuItem_Click(object sender, EventArgs e)
        {
            if (processP.RunCount > 1)
                processP.RunOnce();

            if (serviceP.RunCount > 1)
                serviceP.RunOnce();
        }

        private void updateProcessesMenuItem_Click(object sender, EventArgs e)
        {
            updateProcessesMenuItem.Checked = !updateProcessesMenuItem.Checked;
            processP.Enabled = updateProcessesMenuItem.Checked;
        }

        private void updateServicesMenuItem_Click(object sender, EventArgs e)
        {
            updateServicesMenuItem.Checked = !updateServicesMenuItem.Checked;
            serviceP.Enabled = updateServicesMenuItem.Checked;
        }   

        private void csrProcessesMenuItem_Click(object sender, EventArgs e)
        {
            if (CSRProcessesWindow == null || CSRProcessesWindow.IsDisposed)
                CSRProcessesWindow = new CSRProcessesWindow();

            CSRProcessesWindow.Show();

            if (CSRProcessesWindow.WindowState == FormWindowState.Minimized)
                CSRProcessesWindow.WindowState = FormWindowState.Normal;

            CSRProcessesWindow.Activate();
        }

        private void verifyFileSignatureMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Filter = "Executable files (*.exe;*.dll;*.sys;*.scr;*.cpl)|*.exe;*.dll;*.sys;*.scr;*.cpl|All files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var result = Win32.VerifyFile(ofd.FileName);
                    string message = "";

                    switch (result)
                    {
                        case Win32.VerifyResult.Distrust:
                            message = "is not trusted";
                            break;
                        case Win32.VerifyResult.Expired:
                            message = "has an expired certificate";
                            break;
                        case Win32.VerifyResult.NoSignature:
                            message = "does not have a digital signature";
                            break;
                        case Win32.VerifyResult.Revoked:
                            message = "has a revoked certificate";
                            break;
                        case Win32.VerifyResult.SecuritySettings:
                            message = "could not be verified due to security settings";
                            break;
                        case Win32.VerifyResult.Trusted:
                            message = "is trusted";
                            break;
                        case Win32.VerifyResult.Unknown:
                            message = "could not be verified";
                            break;
                        default:
                            message = "could not be verified";
                            break;
                    }

                    MessageBox.Show("The file \"" + ofd.FileName + "\" " + message +
                        ".", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            Save.SaveToFile();
        }

        #endregion

        #region Network Context Menu

        private void menuNetwork_Popup(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count == 0)
            {
                menuNetwork.DisableAll();
            }
            else if (listNetwork.SelectedItems.Count == 1)
            {
                menuNetwork.EnableAll();
            }
            else
            {
                menuNetwork.EnableAll();
                goToProcessNetworkMenuItem.Enabled = false;
            }

            if (listNetwork.Items.Count > 0)
                selectAllNetworkMenuItem.Enabled = true;
            else
                selectAllNetworkMenuItem.Enabled = false;
        }

        private void goToProcessNetworkMenuItem_Click(object sender, EventArgs e)
        {
            DeselectAll(treeProcesses.Tree);

            try
            {
                TreeNodeAdv node = treeProcesses.FindTreeNode((int)listNetwork.SelectedItems[0].Tag);

                node.EnsureVisible();
                node.IsSelected = true;
                treeProcesses.Tree.FullUpdate();

                tabControl.SelectedTab = tabProcesses;
            }
            catch
            { }
        }

        private void selectAllNetworkMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(listNetwork.List.Items);
        }

        #endregion

        #region Notification Icon & Menu

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            showHideMenuItem_Click(null, null);
        }

        private void menuIcon_Popup(object sender, EventArgs e)
        {
            List<ProcessItem> processes = new List<ProcessItem>();

            // Clear the images so we don't get GDI+ handle leaks
            foreach (MenuItem item in processesMenuItem.MenuItems)
                vistaMenu.SetImage(item, null);

            processesMenuItem.MenuItems.DisposeAndClear();

            // HACK: To be fixed later - we need some sort of locking for the process provider
            try
            {
                foreach (var process in processP.Dictionary.Values)
                {
                    if (process.PID > 0)
                    {
                        processes.Add(process);
                    }
                }

                for (int i = 0; i < processes.Count && processes.Count > Properties.Settings.Default.IconMenuProcessCount; i++)
                {
                    if (processes[i].CpuUsage == 0)
                    {
                        processes.RemoveAt(i);
                        i--;
                    }
                    else if (processes[i].Username != Program.CurrentUsername)
                    {
                        processes.RemoveAt(i);
                        i--;
                    }
                }

                processes.Sort((i1, i2) => -i1.CpuUsage.CompareTo(i2.CpuUsage));

                if (processes.Count > Properties.Settings.Default.IconMenuProcessCount)
                {
                    int c = processes.Count;
                    processes.RemoveRange(Properties.Settings.Default.IconMenuProcessCount,
                        processes.Count - Properties.Settings.Default.IconMenuProcessCount);
                }

                foreach (var process in processes)
                {
                    MenuItem processItem = new MenuItem();
                    MenuItem terminateItem = new MenuItem();
                    MenuItem suspendItem = new MenuItem();
                    MenuItem resumeItem = new MenuItem();
                    MenuItem propertiesItem = new MenuItem();

                    processItem.Text = process.Name + " (" + process.PID.ToString() + ")";
                    processItem.Tag = process.PID;

                    terminateItem.Click += new EventHandler((sender_, e_) =>
                    {
                        if (MessageBox.Show("Are you sure you want to terminate the process?", "Process Hacker",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            try
                            {
                                (new Win32.ProcessHandle((int)((MenuItem)sender_).Parent.Tag,
                                    Win32.PROCESS_RIGHTS.PROCESS_TERMINATE)).Terminate();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    });
                    terminateItem.Text = "Terminate";

                    suspendItem.Click += new EventHandler((sender_, e_) =>
                    {
                        try
                        {
                            (new Win32.ProcessHandle((int)((MenuItem)sender_).Parent.Tag,
                                Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME)).Suspend();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                    suspendItem.Text = "Suspend";

                    resumeItem.Click += new EventHandler((sender_, e_) =>
                    {
                        try
                        {
                            (new Win32.ProcessHandle((int)((MenuItem)sender_).Parent.Tag,
                                Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME)).Resume();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                    resumeItem.Text = "Resume";

                    propertiesItem.Click += new EventHandler((sender_, e_) =>
                    {
                        try
                        {
                            ProcessWindow pForm = Program.GetProcessWindow(processP.Dictionary[(int)((MenuItem)sender_).Parent.Tag],
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
                    });
                    propertiesItem.Text = "Properties...";

                    processItem.MenuItems.AddRange(new MenuItem[] { terminateItem, suspendItem, resumeItem, propertiesItem });
                    processesMenuItem.MenuItems.Add(processItem);

                    vistaMenu.SetImage(processItem, (treeProcesses.Tree.Model as ProcessTreeModel).Nodes[process.PID].Icon);
                }
            }
            catch
            {
                foreach (MenuItem item in processesMenuItem.MenuItems)
                    vistaMenu.SetImage(item, null);

                processesMenuItem.MenuItems.DisposeAndClear();
            }
        }

        private void showHideMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;

            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            this.Activate();
        }

        private void sysInformationIconMenuItem_Click(object sender, EventArgs e)
        {
            sysInfoMenuItem_Click(sender, e);
        }

        private void enableAllNotificationsMenuItem_Click(object sender, EventArgs e)
        {
            NPMenuItem.Checked = true;
            TPMenuItem.Checked = true;
            NSMenuItem.Checked = true;
            startedSMenuItem.Checked = true;
            stoppedSMenuItem.Checked = true;
            DSMenuItem.Checked = true;
        }

        private void disableAllNotificationsMenuItem_Click(object sender, EventArgs e)
        {
            NPMenuItem.Checked = false;
            TPMenuItem.Checked = false;
            NSMenuItem.Checked = false;
            startedSMenuItem.Checked = false;
            stoppedSMenuItem.Checked = false;
            DSMenuItem.Checked = false;
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        #endregion

        #region Process Context Menu

        private void menuProcess_Popup(object sender, EventArgs e)
        {
            virtualizationProcessMenuItem.Checked = false;

            if (treeProcesses.SelectedTreeNodes.Count == 0)
            {
                menuProcess.DisableAll();
            }
            else if (treeProcesses.SelectedTreeNodes.Count == 1)
            {
                menuProcess.EnableAll();

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
                    using (var phandle = new Win32.ProcessHandle(processSelectedPID, Program.MinProcessQueryRights))
                    {
                        switch (phandle.GetPriorityClass())
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
                }
                catch (Exception ex)
                {
                    priorityMenuItem.Text = "(" + ex.Message + ")";
                    priorityMenuItem.Enabled = false;
                }

                try
                {
                    using (var phandle = new Win32.ProcessHandle(processSelectedPID, Program.MinProcessQueryRights))
                    {
                        using (var thandle = phandle.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY))
                        {
                            if (virtualizationProcessMenuItem.Enabled = thandle.IsVirtualizationAllowed())
                                virtualizationProcessMenuItem.Checked = thandle.IsVirtualizationEnabled();
                        }
                    }
                }
                catch
                {
                    virtualizationProcessMenuItem.Enabled = false;
                }
            }
            else
            {
                menuProcess.DisableAll();

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
                menuProcess.DisableAll();
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
            string name = "the selected process(es)";

            if (treeProcesses.SelectedTreeNodes.Count == 0)
                return;
            else if (treeProcesses.SelectedTreeNodes.Count == 1)
                name = treeProcesses.SelectedNodes[0].Name;
            else
                name = "the selected processes";

            if (MessageBox.Show("Are you sure you want to terminate " + name + "?", 
                "Process Hacker", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited && 
                    Program.KPH == null)
                {
                    try
                    {
                        foreach (ProcessNode node in treeProcesses.SelectedNodes)
                        {
                            using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(node.PID,
                                Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                            { }
                        }
                    }
                    catch
                    {
                        string objects = "";

                        foreach (ProcessNode node_ in treeProcesses.SelectedNodes)
                            objects += node_.PID + ",";

                        Program.StartProcessHackerAdmin("-e -type process -action terminate -obj \"" +
                            objects + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);

                        return;
                    }
                }

                foreach (ProcessNode node in treeProcesses.SelectedNodes)
                {
                    try
                    {
                        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(node.PID,
                            Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                            phandle.Terminate();
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
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited &&
                Program.KPH == null)
            {
                try
                {
                    foreach (ProcessNode node in treeProcesses.SelectedNodes)
                    {
                        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(node.PID,
                            Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (ProcessNode node_ in treeProcesses.SelectedNodes)
                        objects += node_.PID + ",";

                    Program.StartProcessHackerAdmin("-e -type process -action suspend -obj \"" +
                        objects + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);

                    return;
                }
            }

            foreach (ProcessNode node in treeProcesses.SelectedNodes)
            {
                if (Properties.Settings.Default.WarnDangerous && Misc.IsDangerousPid(node.PID))
                {
                    DialogResult result = MessageBox.Show(node.Name + " (PID " + node.PID.ToString() + ") is a system process. Are you" +
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
                    DialogResult result = MessageBox.Show("Could not suspend process " + node.Name +
                        " (PID " + node.PID.ToString() + "):\n\n" +
                        ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void resumeMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited &&
                Program.KPH == null)
            {
                try
                {
                    foreach (ProcessNode node in treeProcesses.SelectedNodes)
                    {
                        using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(node.PID,
                            Win32.PROCESS_RIGHTS.PROCESS_SUSPEND_RESUME))
                        { }
                    }
                }
                catch
                {
                    string objects = "";

                    foreach (ProcessNode node_ in treeProcesses.SelectedNodes)
                        objects += node_.PID + ",";

                    Program.StartProcessHackerAdmin("-e -type process -action resume -obj \"" +
                        objects + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);

                    return;
                }
            }

            foreach (ProcessNode node in treeProcesses.SelectedNodes)
            {
                if (Properties.Settings.Default.WarnDangerous && Misc.IsDangerousPid(node.PID))
                {
                    DialogResult result = MessageBox.Show(node.Name + " (PID " + node.PID.ToString() + ") is a system process. Are you" +
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
                    DialogResult result = MessageBox.Show("Could not resume process " + node.Name +
                        " (PID " + node.PID.ToString() + "):\n\n" +
                        ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                        return;
                }
            }
        }

        private void restartProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart the process?", "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    using (var phandle = new Win32.ProcessHandle(processSelectedPID,
                        Program.MinProcessQueryRights | Win32.PROCESS_RIGHTS.PROCESS_VM_READ))
                    {
                        string currentDirectory = phandle.GetPebString(Win32.ProcessHandle.PebOffset.CurrentDirectoryPath);
                        string cmdLine = phandle.GetPebString(Win32.ProcessHandle.PebOffset.CommandLine);

                        try
                        {
                            using (var phandle2 = new Win32.ProcessHandle(processSelectedPID, Win32.PROCESS_RIGHTS.PROCESS_TERMINATE))
                                phandle2.Terminate();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not terminate the process: " + ex.Message, "Process Hacker",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        try
                        {
                            Win32.STARTUPINFO startupInfo = new Win32.STARTUPINFO();
                            Win32.PROCESS_INFORMATION procInfo = new Win32.PROCESS_INFORMATION();

                            startupInfo.Size = Marshal.SizeOf(startupInfo);

                            if (!Win32.CreateProcess(null, cmdLine, 0, 0, false, 0, 0, currentDirectory,
                                ref startupInfo, ref procInfo))
                                Win32.ThrowLastWin32Error();

                            Win32.CloseHandle(procInfo.hProcess);
                            Win32.CloseHandle(procInfo.hThread);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Could not start the command '" + cmdLine + "': " + ex.Message, "Process Hacker",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not restart the process: " + ex.Message, "Process Hacker",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void reduceWorkingSetProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var phandle = new Win32.ProcessHandle(processSelectedPID,
                    Win32.PROCESS_RIGHTS.PROCESS_ALL_ACCESS))
                    phandle.EmptyWorkingSet();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void virtualizationProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to set virtualization for this process?",
                "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;

            try
            {
                using (var phandle = new Win32.ProcessHandle(processSelectedPID, Program.MinProcessQueryRights))
                {
                    using (var thandle = phandle.GetToken(Win32.TOKEN_RIGHTS.TOKEN_WRITE))
                    {
                        thandle.SetVirtualizationEnabled(!virtualizationProcessMenuItem.Checked);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Program.FocusWindow(f);
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

        private void setTokenProcessMenuItem_Click(object sender, EventArgs e)
        {
            ProcessPickerWindow picker = new ProcessPickerWindow();

            picker.Label = "Select the source of the token:";

            if (picker.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Program.KPH.SetProcessToken(picker.SelectedPid, processSelectedPID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void reanalyzeProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                processP.QueueFileProcessing(processSelectedPID);
            }
            catch
            { }
        }   

        private void selectAllProcessMenuItem_Click(object sender, EventArgs e)
        {
            Misc.SelectAll(treeProcesses.Tree.AllNodes);
        } 

        #endregion

        #region Providers

        private void processP_Updated()
        {
            networkP.RunOnceAsync();

            processP.DictionaryAdded += new ProcessSystemProvider.ProviderDictionaryAdded(processP_DictionaryAdded);
            processP.DictionaryRemoved += new ProcessSystemProvider.ProviderDictionaryRemoved(processP_DictionaryRemoved);
            processP.Updated -= new ProcessSystemProvider.ProviderUpdateOnce(processP_Updated);

            try { Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; }
            catch { }

            if (processP.RunCount >= 1)
                this.Invoke(new MethodInvoker(delegate
                {
                    treeProcesses.Tree.EndUpdate();
                    this.Cursor = Cursors.Default;
                    this.UpdateCommon();
                }));
        }

        private void processP_InfoUpdater()
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                cpuUsageIcon.Update(processP.CurrentCpuKernelUsage, processP.CurrentCpuUserUsage);

                if (NotifyIcon.Icon != null)
                    Win32.DestroyIcon(notifyIcon.Icon.Handle);

                notifyIcon.Icon = cpuUsageIcon.GetIcon();
                
                UpdateStatusInfo();

                notifyIcon.Text = "Process Hacker\n" +
                    "CPU Usage: " + (processP.CurrentCpuUsage * 100).ToString("F2") + "%";

                if (processP.Dictionary.ContainsKey(processP.PIDWithMostCpuUsage))
                    if (processP.Dictionary[processP.PIDWithMostCpuUsage].Name != null)
                        if (notifyIcon.Text.Length +
                            processP.Dictionary[processP.PIDWithMostCpuUsage].Name.Length + 7 < 62)
                            notifyIcon.Text += "\n" + processP.Dictionary[processP.PIDWithMostCpuUsage].Name +
                                ": " + processP.Dictionary[processP.PIDWithMostCpuUsage].CpuUsage.ToString("F2") + "%";
            }));
        }

        public void processP_DictionaryAdded(ProcessItem item)
        {
            ProcessItem parent = null;
            string parentText = "";

            if (item.HasParent)
            {
                try
                {
                    parent = processP.Dictionary[item.ParentPid];

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
            this.QueueMessage("Terminated Process: " + item.Name + " (PID " + item.PID.ToString() + ")", null);

            if (processServices.ContainsKey(item.PID))
                processServices.Remove(item.PID);

            if (TPMenuItem.Checked)
                notifyIcon.ShowBalloonTip(2000, "Terminated Process",
                    "The process " + item.Name + " (" + item.PID.ToString() + ") was terminated.", ToolTipIcon.Info);
        }

        private void serviceP_Updated()
        {
            listServices.BeginInvoke(new MethodInvoker(delegate
            {
                listServices.List.EndUpdate();
            }));

            HighlightingContext.StateHighlighting = true;

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
                menuService.DisableAll();
                goToProcessServiceMenuItem.Visible = true;
                startServiceMenuItem.Visible = true;
                continueServiceMenuItem.Visible = true;
                pauseServiceMenuItem.Visible = true;
                stopServiceMenuItem.Visible = true;

                selectAllServiceMenuItem.Enabled = true;
            }
            else if (listServices.SelectedItems.Count == 1)
            {
                menuService.EnableAll();

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
                    menuService.DisableAll();
                    copyServiceMenuItem.Enabled = true;
                    propertiesServiceMenuItem.Enabled = true;
                }
            }
            else
            {
                menuService.DisableAll();

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

                node.EnsureVisible();
                node.IsSelected = true;
                treeProcesses.Tree.FullUpdate();

                tabControl.SelectedTab = tabProcesses;
            }
            catch
            { }
        }

        private void startServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
            {
                Program.StartProcessHackerAdmin("-e -type service -action start -obj \"" +
                    listServices.SelectedItems[0].Name + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);
            }
            else
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
        }

        private void continueServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
            {
                Program.StartProcessHackerAdmin("-e -type service -action continue -obj \"" +
                    listServices.SelectedItems[0].Name + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);
            }
            else
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
        }

        private void pauseServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
            {
                Program.StartProcessHackerAdmin("-e -type service -action pause -obj \"" +
                    listServices.SelectedItems[0].Name + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);
            }
            else
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
        }

        private void stopServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
            {
                Program.StartProcessHackerAdmin("-e -type service -action stop -obj \"" +
                    listServices.SelectedItems[0].Name + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);
            }
            else
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
        }

        private void deleteServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the service '" + 
                listServices.SelectedItems[0].Name + "'?", 
                "Process Hacker",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                {
                    Program.StartProcessHackerAdmin("-e -type service -action delete -obj \"" +
                        listServices.SelectedItems[0].Name + "\" -hwnd " + this.Handle.ToString(), null, this.Handle);
                }
                else
                {
                    try
                    {
                        using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                            listServices.SelectedItems[0].Name, (Win32.SERVICE_RIGHTS)Win32.STANDARD_RIGHTS.DELETE))
                            service.Delete();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting service:\n\n" + ex.Message, "Process Hacker",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
            if (tabControl.SelectedTab == tabNetwork)
            {
                if (processP.RunCount > 0)
                {
                    networkP.Enabled = true;
                    networkP.RunOnceAsync();
                }
            }
            else
            {
                networkP.Enabled = false;
            }
        }

        #endregion

        #region Timers

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

        #region ToolStrip Items

        private void findHandlesToolStripButton_Click(object sender, EventArgs e)
        {
            findHandlesMenuItem_Click(sender, e);
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            updateNowMenuItem_Click(sender, e);
        }

        private void sysInfoToolStripButton_Click(object sender, EventArgs e)
        {
            sysInfoMenuItem_Click(sender, e);
        }

        private void optionsToolStripButton_Click(object sender, EventArgs e)
        {
            optionsMenuItem_Click(sender, e);
        }

        #endregion

        #region Trees

        private void treeProcesses_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            inspectProcessMenuItem_Click(null, null);
        }

        #endregion

        #endregion

        #region Form-related Helper functions

        public void ApplyFont(Font f)
        {
            treeProcesses.Tree.Font = f;

            if (f.Height > 16)
                treeProcesses.Tree.RowHeight = f.Height;
            else
                treeProcesses.Tree.RowHeight = 16;

            listServices.List.Font = f;
            listNetwork.List.Font = f;
        }

        public void ClearLog()
        {
            _log.Clear();

            if (this.LogUpdated != null)
                this.LogUpdated(null);
        }

        private void CreateShutdownMenuItems()
        {
            AddMenuItemDelegate addMenuItem = (string text, EventHandler onClick) =>
            {
                shutdownMenuItem.MenuItems.Add(new MenuItem(text, onClick));
                shutdownTrayMenuItem.MenuItems.Add(new MenuItem(text, onClick));
                shutDownToolStripMenuItem.DropDownItems.Add(text, null, onClick);
            };

            addMenuItem("Lock", (sender, e) => { Win32.LockWorkStation(); });
            addMenuItem("Logoff", (sender, e) => { Win32.ExitWindowsEx(Win32.ExitWindowsFlags.Logoff, 0); });
            addMenuItem("-", null);
            addMenuItem("Sleep", (sender, e) => { Win32.SetSuspendState(false, false, false); });
            addMenuItem("Hibernate", (sender, e) => { Win32.SetSuspendState(true, false, false); });
            addMenuItem("-", null);
            addMenuItem("Restart", (sender, e) =>
            {
                if (MessageBox.Show("Are you sure you want to restart your computer?", "Process Hacker",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, 
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    Win32.ExitWindowsEx(Win32.ExitWindowsFlags.Reboot, 0);
            });
            addMenuItem("Shutdown", (sender, e) =>
            {
                if (MessageBox.Show("Are you sure you want to shutdown your computer?", "Process Hacker",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    Win32.ExitWindowsEx(Win32.ExitWindowsFlags.Shutdown, 0);
            });
            addMenuItem("Poweroff", (sender, e) =>
            {
                if (MessageBox.Show("Are you sure you want to poweroff your computer?", "Process Hacker",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    Win32.ExitWindowsEx(Win32.ExitWindowsFlags.Poweroff, 0);
            });
        }

        private void DeleteSettings()
        {
            //Type localFileSettingsProviderType = typeof(System.Configuration.LocalFileSettingsProvider);
            //PropertyInfo storeType = 
            //    localFileSettingsProviderType.GetProperty("Store", BindingFlags.NonPublic);
            //object store = storeType.GetValue(Properties.Settings.Default.Properties["AlwaysOnTop"].Provider, null);
            //MethodInfo getUserConfig =
            //    store.GetType().GetMethod("GetUserConfig", BindingFlags.NonPublic);

            //object localConfig = getUserConfig.Invoke(store, new object[] { false });
            //object roamingConfig = getUserConfig.Invoke(store, new object[] { true });
            //PropertyInfo filePath = localConfig.GetType().GetProperty("FilePath", BindingFlags.NonPublic);

            //try { System.IO.File.Delete(filePath.GetValue(localConfig, null) as string); }
            //catch { }
            //try { System.IO.File.Delete(filePath.GetValue(roamingConfig, null) as string); }
            //catch { }
            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\wj32"))
                System.IO.Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    + "\\wj32", true);
            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\wj32"))
                System.IO.Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\wj32", true); 
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

        // Technique from http://www.vb-helper.com/howto_2008_uac_shield.html
        private Bitmap GetUacShieldIcon()
        {
            const int width = 50;
            const int height = 50;
            const int margin = 4;
            Bitmap shieldImage;
            Button button = new Button()
            {
                Text = " ",
                Size = new Size(width, height),
                FlatStyle = FlatStyle.System
            };

            button.SetShieldIcon(true);

            Bitmap buttonImage = new Bitmap(width, height);

            button.Refresh();
            button.DrawToBitmap(buttonImage, new Rectangle(0, 0, width, height));

            int minX = width;
            int maxX = 0;
            int minY = width;
            int maxY = 0;

            for (int y = margin; y < height - margin; y++)
            {
                var targetColor = buttonImage.GetPixel(margin, y);

                for (int x = margin; x < width - margin; x++)
                {
                    if (buttonImage.GetPixel(x, y).Equals(targetColor))
                    {
                        buttonImage.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        if (minY > y) minY = y;
                        if (minX > x) minX = x;
                        if (maxY < y) maxY = y;
                        if (maxX < x) maxX = x;
                    }
                }
            }

            int shieldWidth = maxX - minX + 1;
            int shieldHeight = maxY - minY + 1;

            shieldImage = new Bitmap(shieldWidth, shieldHeight);

            using (Graphics g = Graphics.FromImage(shieldImage))
                g.DrawImage(buttonImage, 0, 0, new Rectangle(minX, minY, shieldWidth, shieldHeight), GraphicsUnit.Pixel);

            buttonImage.Dispose();

            return shieldImage;
        }

        private void LoadSettings()
        {
            this.TopMost = Properties.Settings.Default.AlwaysOnTop;                
            this.Location = Properties.Settings.Default.WindowLocation;
            this.Size = Properties.Settings.Default.WindowSize;

            if (Properties.Settings.Default.WindowState != FormWindowState.Minimized)
                this.WindowState = Properties.Settings.Default.WindowState;
            else
                this.WindowState = FormWindowState.Normal;

            PromptBox.LastValue = Properties.Settings.Default.PromptBoxText;
            toolbarMenuItem.Checked = toolStrip.Visible = Properties.Settings.Default.ToolbarVisible;

            ColumnSettings.LoadSettings(Properties.Settings.Default.ProcessTreeColumns, treeProcesses.Tree);
            ColumnSettings.LoadSettings(Properties.Settings.Default.ServiceListViewColumns, listServices.List);
            ColumnSettings.LoadSettings(Properties.Settings.Default.NetworkListViewColumns, listNetwork.List);

            HighlightingContext.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNewProcesses;
            HighlightingContext.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Properties.Settings.Default.ColorNewProcesses;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;

            Program.ImposterNames = new System.Collections.Specialized.StringCollection();

            foreach (string s in Properties.Settings.Default.ImposterNames.Split(','))
                Program.ImposterNames.Add(s.Trim());

            HistoryManager.GlobalMaxCount = Properties.Settings.Default.MaxSamples;
            ProcessHacker.Components.Plotter.GlobalMoveStep = Properties.Settings.Default.PlotterStep;
        }

        public void QueueMessage(string message)
        {
            this.QueueMessage(message, null);
        }

        public void QueueMessage(string message, Icon icon)
        {
            var value = new KeyValuePair<DateTime, string>(DateTime.Now, message);

            _log.Add(value);
            statusMessages.Enqueue(new KeyValuePair<string, Icon>(message, icon));

            if (this.LogUpdated != null)
                this.LogUpdated(value);
        }

        private void SaveSettings()
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible)
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
            }

            Properties.Settings.Default.AlwaysOnTop = this.TopMost;
            Properties.Settings.Default.WindowState = this.WindowState == FormWindowState.Minimized ?
                FormWindowState.Normal : this.WindowState;
            Properties.Settings.Default.ToolbarVisible = toolStrip.Visible;

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
            if (processP.RunCount >= 1)
                statusGeneral.Text = string.Format("{0} processes", processP.Dictionary.Count - 2);
            else
                statusGeneral.Text = "Loading...";

            statusCPU.Text = "CPU: " + (processP.CurrentCpuUsage * 100).ToString("N2") + "%";
            statusMemory.Text = "Phys. Memory: " +
                ((float)(processP.System.NumberOfPhysicalPages - processP.Performance.AvailablePages) * 100 /
                processP.System.NumberOfPhysicalPages).ToString("N2") + "%";
        }

        #endregion

        #region Helper functions

        private void SetProcessPriority(ProcessPriorityClass priority)
        {
            try
            {
                using (var phandle = new Win32.ProcessHandle(processSelectedPID, Win32.PROCESS_RIGHTS.PROCESS_SET_INFORMATION))
                    phandle.SetPriorityClass(priority);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The priority could not be set:\n\n" + ex.Message, 
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // Magic number - PH uses this to detect previous instances.
                case 0x9991:
                    {
                        this.Visible = true;

                        if (this.WindowState == FormWindowState.Minimized)
                            this.WindowState = FormWindowState.Normal;

                        m.Result = new IntPtr(0x1119);

                        return;
                    }
                    //break;

                case (int)Win32.WindowMessage.SysCommand:
                    {
                        if (m.WParam.ToInt32() == 0xf020) // SC_MINIMIZE
                        {
                            if (this.NotifyIcon.Visible && Properties.Settings.Default.HideWhenMinimized)
                            {
                                this.Visible = false;

                                return;
                            }
                        }
                    }
                    break;

                case (int)Win32.WindowMessage.Activate:
                case (int)Win32.WindowMessage.KillFocus:
                    {
                        treeProcesses.Tree.Invalidate();
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void Exit()
        {
            processP.Dispose();
            serviceP.Dispose();
            networkP.Dispose();

            notifyIcon.Visible = false;

            SaveSettings();

            if (Program.KPH != null)
                Program.KPH.Close();

            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch
            { }
        }

        private void HackerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                this.Exit();
                return;
            }

            if (Properties.Settings.Default.HideWhenClosed)
            {
                e.Cancel = true;
                showHideMenuItem_Click(sender, null);
                return;
            }

            this.Exit();
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

        private void LoadVerifySettings()
        {
            // Try to get a setting. If the file is corrupt, we can reset the settings.
            try
            {
                var a = Properties.Settings.Default.AlwaysOnTop;
            }
            catch
            {
                if (MessageBox.Show("Process Hacker cannot start because your configuration file is corrupt. " +
                    "Do you want Process Hacker to reset your settings?", "Process Hacker", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    try
                    {
                        this.DeleteSettings();
                        MessageBox.Show("Process Hacker has reset your settings and will now restart.", "Process Hacker",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process.Start(Application.ExecutablePath);
                    }
                    catch
                    {
                        MessageBox.Show("Process Hacker could not reset your settings. Please delete the folder " +
                            "'wj32' in your Application Data/Local Application Data directories.",
                            "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private void LoadFixMenuItems()
        {
            if (!System.IO.File.Exists(Application.StartupPath + "\\Assistant.exe"))
            {
                runAsServiceMenuItem.Enabled = false;
                runAsProcessMenuItem.Visible = false;
            }

            if (Program.KPH == null)
                csrProcessesMenuItem.Visible = false;

            if (Program.KPH == null || Program.WindowsVersion != "XP")
                setTokenProcessMenuItem.Visible = false;
        }

        private void LoadUac()
        {
            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
            {
                uacShieldIcon = this.GetUacShieldIcon();

                vistaMenu.SetImage(showDetailsForAllProcessesMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(startServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(continueServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(pauseServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(stopServiceMenuItem, uacShieldIcon);
                //vistaMenu.SetImage(deleteServiceMenuItem, uacShieldIcon);
                //runAsServiceMenuItem.Visible = false;
                //runAsProcessMenuItem.Visible = false;
            }
            else
            {
                runAsAdministratorMenuItem.Visible = false;
                showDetailsForAllProcessesMenuItem.Visible = false;
            }
        }

        private void LoadNotificationIcon()
        {
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
        }

        private void LoadControls()
        {
            cpuUsageIcon = new UsageIcon(16, 16);

            listControls.Add(treeProcesses.Tree);
            listControls.Add(listServices);

            GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            GenericViewMenu.AddMenuItems(copyServiceMenuItem.MenuItems, listServices.List, null);
            GenericViewMenu.AddMenuItems(copyNetworkMenuItem.MenuItems, listNetwork.List, null);

            treeProcesses.ContextMenu = menuProcess;
            listServices.ContextMenu = menuService;
            listNetwork.ContextMenu = menuNetwork;

            Program.SecondarySharedThreadProvider = new SharedThreadProvider(Properties.Settings.Default.RefreshInterval);
            Program.SharedThreadProvider = new SharedThreadProvider(Properties.Settings.Default.RefreshInterval);
            Program.SharedThreadProvider.Add(processP);
            Program.SharedThreadProvider.Add(serviceP);
            Program.SharedThreadProvider.Add(networkP);

            processP.Interval = Properties.Settings.Default.RefreshInterval;
            treeProcesses.Provider = processP;
            treeProcesses.Tree.BeginUpdate();
            this.Cursor = Cursors.WaitCursor;
            processP.RunOnceAsync();
            processP.Updated += new ProcessSystemProvider.ProviderUpdateOnce(processP_Updated);
            processP.Updated += new ProcessSystemProvider.ProviderUpdateOnce(processP_InfoUpdater);
            processP.Enabled = true;
            updateProcessesMenuItem.Checked = true;

            cpuUsageIcon.BackColor = Color.Black;
            cpuUsageIcon.Color = Color.FromArgb(0, 255, 0);

            HighlightingContext.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            HighlightingContext.StateHighlighting = false;
            listServices.List.BeginUpdate();
            serviceP.Interval = Properties.Settings.Default.RefreshInterval;
            listServices.Provider = serviceP;
            serviceP.RunOnceAsync();
            serviceP.DictionaryAdded += new ServiceProvider.ProviderDictionaryAdded(serviceP_DictionaryAdded_Process);
            serviceP.DictionaryModified += new ServiceProvider.ProviderDictionaryModified(serviceP_DictionaryModified_Process);
            serviceP.DictionaryRemoved += new ServiceProvider.ProviderDictionaryRemoved(serviceP_DictionaryRemoved_Process);
            serviceP.Updated += new ServiceProvider.ProviderUpdateOnce(serviceP_Updated);
            serviceP.Enabled = true;
            updateServicesMenuItem.Checked = true;

            networkP.Interval = Properties.Settings.Default.RefreshInterval;
            listNetwork.Provider = networkP;
            networkP.Enabled = true;

            treeProcesses.Tree.MouseDown += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Right && e.Location.Y < treeProcesses.Tree.ColumnHeaderHeight)
                    {
                        ContextMenu menu = new ContextMenu();

                        menu.MenuItems.Add(new MenuItem("Choose Columns...", (sender_, e_) =>
                            {
                                (new ChooseColumnsWindow(treeProcesses.Tree)
                                {
                                    TopMost = this.TopMost
                                }).ShowDialog();

                                copyProcessMenuItem.MenuItems.DisposeAndClear();
                                GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
                                treeProcesses.Tree.Invalidate();
                            }));

                        menu.Show(treeProcesses.Tree, e.Location);
                    }
                };
            treeProcesses.Tree.ColumnClicked += (sender, e) => { DeselectAll(treeProcesses.Tree); };
            treeProcesses.Tree.ColumnReordered += (sender, e) =>
            {
                copyProcessMenuItem.MenuItems.DisposeAndClear();
                GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            };

            tabControlBig_SelectedIndexChanged(null, null);
        }

        private void LoadAddShortcuts()
        {
            treeProcesses.Tree.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(treeProcesses.TreeNodes);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.TreeViewAdvCopy(treeProcesses.Tree, -1);
                };
            listServices.List.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listServices.List.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listServices.List, -1);
                };
            listNetwork.List.KeyDown +=
                (sender, e) =>
                {
                    if (e.Control && e.KeyCode == Keys.A) Misc.SelectAll(listNetwork.List.Items);
                    if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.ListViewCopy(listNetwork.List, -1);
                };
        }

        private void LoadSymbols()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
            {
                try
                {
                    string[] modules =
                    {
                        "advapi32.dll", "comctl32.dll", "crypt32.dll", "dnsapi.dll",
                        "gdi32.dll", "imagehlp.dll", "kernel32.dll", 
                        "ntdll.dll", "ole32.dll", "psapi.dll", "rpcrt4.dll", "shell32.dll",
                        "user32.dll", "winsta.dll", "wintrust.dll", "wtsapi32.dll" };

                    foreach (string module in modules)
                    {
                        try
                        {
                            SymbolProvider.BaseInstance.LoadSymbolsFromLibrary(Environment.SystemDirectory + "\\" + module,
                                (uint)Win32.GetModuleHandle(module));
                        }
                        catch
                        { }
                    }
                }
                catch
                { }
            }));
        }

        private void LoadApplyCommandLineArgs()
        {
            tabControl.SelectedTab = tabControl.TabPages["tab" + Program.SelectTab];

            if (Program.ShowOptions)
            {
                OptionsWindow options = new OptionsWindow();

                options.TopMost = this.TopMost;
                options.ShowDialog();

                processP.Interval = Properties.Settings.Default.RefreshInterval;
                serviceP.Interval = Properties.Settings.Default.RefreshInterval;
                networkP.Interval = Properties.Settings.Default.RefreshInterval;
            }
        }

        public HackerWindow()
        {
            Program.HackerWindow = this;

            this.LoadVerifySettings();

            InitializeComponent();

            // Force the handle to be created
            { var handle = this.Handle; }

            this.SuspendLayout();
            this.CreateShutdownMenuItems();
            this.LoadFixMenuItems();
            this.LoadNotificationIcon();
            this.LoadUac();
            this.LoadSettings();
            this.LoadControls();
            this.LoadAddShortcuts();
            this.LoadSymbols();
            this.ResumeLayout();

            if ((!Properties.Settings.Default.StartHidden && !Program.StartHidden) ||
                Program.StartVisible || Program.ShowOptions)
            {
                this.Visible = true;
            }

            if (tabControl.SelectedTab == tabProcesses)
                treeProcesses.Tree.Select();

            statusText.Text = "Waiting...";

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
                    GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY).GetUser().GetName(true) +
                    (Program.KPH != null ? "+" : "") + "]";
            }
            catch
            { }

            // If it's Vista and we're elevated, we should allow the magic window message to allow 
            // Allow only one instance to work.
            if (Program.WindowsVersion == "Vista" &&
                Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull)
            {
                Win32.ChangeWindowMessageFilter((Win32.WindowMessage)0x9991, Win32.UipiFilterFlag.Add);
            }
        }

        private void HackerWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindows();
            this.ApplyFont(Properties.Settings.Default.Font);
            this.BeginInvoke(new MethodInvoker(this.LoadApplyCommandLineArgs));
        }

        private void HackerWindow_SizeChanged(object sender, EventArgs e)
        {
            tabControl.Invalidate(false);
        }

        private void HackerWindow_VisibleChanged(object sender, EventArgs e)
        {
            treeProcesses.Draw = this.Visible;
        }
    }
}
