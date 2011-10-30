/*
 * Process Hacker - 
 *   main Process Hacker window
 * 
 * Copyright (C) 2008-2009 Dean
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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Common;
using ProcessHacker.Common.Threading;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Debugging;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Threading;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;
using TaskbarLib;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        public delegate void LogUpdatedEventHandler(KeyValuePair<DateTime, string>? value);

        private readonly ThumbButtonManager thumbButtonManager;
        //private JumpListManager jumpListManager; //Reserved for future use

        public delegate void AddMenuItemDelegate(string text, EventHandler onClick);

        // This entire file is a big monolithic mess.

        #region Variables

        // One-instance windows.
        public HelpWindow HelpWindow;
        public HandleFilterWindow HandleFilterWindow;
        public HiddenProcessesWindow HiddenProcessesWindow;
        public LogWindow LogWindow;
        public MiniSysInfo MiniSysInfoWindow; // Not used (yet)

        /// <summary>
        /// The thread for the System Information window. This is to avoid 
        /// freezing the main window every second to update the graphs.
        /// </summary>
        Thread sysInfoThread;
        /// <summary>
        /// The System Information window. No methods should be called on 
        /// it directly because it belongs to another thread.
        /// </summary>
        public SysInfoWindow SysInfoWindow;

        /// <summary>
        /// The UAC shield bitmap. Used for the various menu items which 
        /// require UAC elevation.
        /// </summary>
        Bitmap uacShieldIcon;
        /// <summary>
        /// A black icon which all notification icons are set to initially 
        /// before their first paint.
        /// </summary>
        Icon blackIcon;
        /// <summary>
        /// A dummy UsageIcon to avoid null instance checks in the icon-related 
        /// functions.
        /// </summary>
        UsageIcon dummyIcon;
        /// <summary>
        /// The list of notification icons.
        /// </summary>
        readonly List<UsageIcon> notifyIcons = new List<UsageIcon>();
        /// <summary>
        /// The CPU history icon, with a history of CPU usage.
        /// </summary>
        CpuHistoryIcon cpuHistoryIcon;
        /// <summary>
        /// The CPU usage icon, which indicates the current CPU usage (no history). 
        /// Dedicated to those Process Explorer users who don't like the 
        /// CPU history icon.
        /// </summary>
        CpuUsageIcon cpuUsageIcon;
        /// <summary>
        /// The I/O history icon.
        /// </summary>
        IoHistoryIcon ioHistoryIcon;
        /// <summary>
        /// The commit history icon.
        /// </summary>
        CommitHistoryIcon commitHistoryIcon;
        /// <summary>
        /// The physical memory history icon.
        /// </summary>
        PhysMemHistoryIcon physMemHistoryIcon;

        /// <summary>
        /// A dictionary relating services to processes. Each key is a PID and 
        /// each value is a list of service names hosted in that particular process.
        /// </summary>
        readonly Dictionary<int, List<string>> processServices = new Dictionary<int, List<string>>();

        /// <summary>
        /// The number of selected processes. Not used.
        /// </summary>
        int processSelectedItems;
        /// <summary>
        /// The selected PID.
        /// </summary>
        int processSelectedPid = -1;

        /// <summary>
        /// The PH log, with events such as process creation/termination and various 
        /// service events.
        /// </summary>
        readonly List<KeyValuePair<DateTime, string>> _log = new List<KeyValuePair<DateTime, string>>();

        /// <summary>
        /// windowhandle owned by the currently selected process. 
        /// Only populated when the user right-clicks exactly one process.
        /// </summary>
        WindowHandle windowHandle = WindowHandle.Zero;

        /// <summary>
        /// Synchronizes the enabling of the network provider:
        /// 1. The process provider needs to have run at least once.
        /// 2. The user must have clicked on the Network tab.
        /// </summary>
        ActionSync _enableNetworkProviderSync;
        /// <summary>
        /// Synchronizes a highlighting refresh:
        /// 1. The process provider needs to have run at least once.
        /// 2. The service provider needs to have run at least once.
        /// </summary>
        ActionSync _refreshHighlightingSync;

        #endregion

        #region Properties

        // The following two properties were used by the Window menu system. 
        // Not very useful, but still needed for now.

        public ToolStripMenuItem WindowMenuItem
        {
            get { return windowToolStripMenuItem; }
        }

        // Mostly used by Save.cs.
        public ProcessTree ProcessTree
        {
            get { return treeProcesses; }
        }

        public int SelectedPid
        {
            get { return processSelectedPid; }
        }

        // The two properties below aren't used at all.

        public ListView ServiceList
        {
            get { return listServices.List; }
        }

        public ListView NetworkList
        {
            get { return listNetwork.List; }
        }

        /// <summary>
        /// Provides a list of service names hosted by a process.
        /// </summary>
        public IDictionary<int, List<string>> ProcessServices
        {
            get { return processServices; }
        }

        /// <summary>
        /// The PH log.
        /// </summary>
        public IList<KeyValuePair<DateTime, string>> Log
        {
            get { return _log; }
        }

        #endregion

        #region Events

        public event LogUpdatedEventHandler LogUpdated;

        #endregion

        #region Event Handlers

        #region Lists

        private void listNetwork_DoubleClick(object sender, EventArgs e)
        {
            goToProcessNetworkMenuItem_Click(sender, e);
        }

        private void listNetwork_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                goToProcessNetworkMenuItem_Click(null, null);
            }
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            propertiesServiceMenuItem_Click(null, null);
        }

        private void listServices_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteServiceMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                propertiesServiceMenuItem_Click(null, null);
            }
        }

        #endregion

        #region Main Menu

        private void runMenuItem_Click(object sender, EventArgs e)
        {
            Win32.RunFileDlg(this.Handle, IntPtr.Zero, null, null, null, 0);
        }

        private void runAsMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void runAsAdministratorMenuItem_Click(object sender, EventArgs e)
        {
            using (PromptBox box = new PromptBox
            {
                Text = "Enter the command to start"
            })
            {
                box.TextBox.AutoCompleteSource = AutoCompleteSource.AllSystemSources;
                box.TextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                if (box.ShowDialog() == DialogResult.OK)
                {
                    Program.StartProgramAdmin(box.Value, "", null, ShowWindowType.Show, this.Handle);
                }
            }
        }

        private void runAsServiceMenuItem_Click(object sender, EventArgs e)
        {
            RunWindow run = new RunWindow();
            run.ShowDialog();
        }

        private void showDetailsForAllProcessesMenuItem_Click(object sender, EventArgs e)
        {
            Program.StartProcessHackerAdmin("-v", this.Exit, this.Handle);
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
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
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
                PhUtils.ShowException("Unable to load structs", ex);
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
                }, Utils.SixteenthStackSize);
                sysInfoThread.Start();
            }
            else
            {
                SysInfoWindow.BeginInvoke(new MethodInvoker(() =>
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
            using (AboutWindow about = new AboutWindow())
            {
                about.ShowDialog();
            }
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            OptionsWindow options = new OptionsWindow();

            DialogResult result = options.ShowDialog();

           if (result == DialogResult.OK)
           {
               this.LoadOtherSettings();
           }
        }

        private void freeMemoryMenuItem_Click(object sender, EventArgs e)
        {
            Program.CollectGarbage();
        }

        private void helpMenuItem_Click(object sender, EventArgs e)
        {
            if (HelpWindow == null)
                HelpWindow = new HelpWindow();

            HelpWindow.Show();
            HelpWindow.Activate();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void toolbarMenuItem_Click(object sender, EventArgs e)
        {
            toolbarMenuItem.Checked = !toolbarMenuItem.Checked;
            Settings.Instance.ToolbarVisible = toolStrip.Visible = toolbarMenuItem.Checked;
        }

        private void updateProcessesMenuItem_Click(object sender, EventArgs e)
        {
            updateProcessesMenuItem.Checked = !updateProcessesMenuItem.Checked;
            Program.ProcessProvider.Enabled = updateProcessesMenuItem.Checked;
        }

        private void updateServicesMenuItem_Click(object sender, EventArgs e)
        {
            updateServicesMenuItem.Checked = !updateServicesMenuItem.Checked;
            Program.ServiceProvider.Enabled = updateServicesMenuItem.Checked;
        }

        private void hiddenProcessesMenuItem_Click(object sender, EventArgs e)
        {
            if (HiddenProcessesWindow == null || HiddenProcessesWindow.IsDisposed)
                HiddenProcessesWindow = new HiddenProcessesWindow();

            HiddenProcessesWindow.Show();

            if (HiddenProcessesWindow.WindowState == FormWindowState.Minimized)
                HiddenProcessesWindow.WindowState = FormWindowState.Normal;

            HiddenProcessesWindow.Activate();
        }

        private void verifyFileSignatureMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Executable files (*.exe;*.dll;*.sys;*.scr;*.cpl)|*.exe;*.dll;*.sys;*.scr;*.cpl|All files (*.*)|*.*"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var result = Cryptography.VerifyFile(ofd.FileName);
                        string message = string.Empty;

                        switch (result)
                        {
                            case VerifyResult.Distrust:
                                message = "is not trusted";
                                break;
                            case VerifyResult.Expired:
                                message = "has an expired certificate";
                                break;
                            case VerifyResult.NoSignature:
                                message = "does not have a digital signature";
                                break;
                            case VerifyResult.Revoked:
                                message = "has a revoked certificate";
                                break;
                            case VerifyResult.SecuritySettings:
                                message = "could not be verified due to security settings";
                                break;
                            case VerifyResult.Trusted:
                                message = "is trusted";
                                break;
                            case VerifyResult.Unknown:
                                message = "could not be verified";
                                break;
                            default:
                                message = "could not be verified";
                                break;
                        }

                        PhUtils.ShowInformation("The file \"" + ofd.FileName + "\" " + message + ".");
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to verify the file", ex);
                    }
                }
            }
        }

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Process Hacker Dump Files (*.phi)|*.phi|All Files (*.*)|*.*"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DumpHackerWindow dhw = null;

                    try
                    {
                        dhw = new DumpHackerWindow(ofd.FileName);
                    }
                    catch (ProcessHacker.Native.Mfs.MfsInvalidFileSystemException)
                    {
                        PhUtils.ShowError("Unable to open the dump file: the dump file is invalid.");
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to open the dump file", ex);
                    }

                    if (dhw != null)
                        dhw.Show();
                }
            }
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                Save.SaveToFile(this);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void createServiceMenuItem_Click(object sender, EventArgs e)
        {
            CreateServiceWindow createServiceWindow = new CreateServiceWindow();
            createServiceWindow.ShowDialog();
        }

        private void donateMenuItem_Click(object sender, EventArgs e)
        {
            Program.TryStart("http://sourceforge.net/project/project_donations.php?group_id=242527");
        }

        private void checkForUpdatesMenuItem_Click(object sender, EventArgs e)
        {
            this.UpdateProgram(true);
        }

        #region View

        private void cpuHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Instance.CpuHistoryIconVisible =
                cpuHistoryMenuItem.Checked = !cpuHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void cpuUsageMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Instance.CpuUsageIconVisible =
                cpuUsageMenuItem.Checked = !cpuUsageMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void ioHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Instance.IoHistoryIconVisible =
                ioHistoryMenuItem.Checked = !ioHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void commitHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Instance.CommitHistoryIconVisible =
             commitHistoryMenuItem.Checked = !commitHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        private void physMemHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Instance.PhysMemHistoryIconVisible =
               physMemHistoryMenuItem.Checked = !physMemHistoryMenuItem.Checked;
            this.ApplyIconVisibilities();
        }

        #endregion

        #endregion

        public class ToolStripSearchBox : ToolStripControlHost
        {
            public ToolStripSearchBox()
                : base(new VistaSearchBox())
            {

            }
        }

        #region Network Context Menu

        private void menuNetwork_Popup(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count == 0)
            {
                //contextMenuStripNetwork.DisableAll();
            }
            else if (listNetwork.SelectedItems.Count == 1)
            {
                //contextMenuStripNetwork.EnableAll();
            }
            else
            {
                //contextMenuStripNetwork.EnableAll();
                goToProcessNetworkMenuItem.Enabled = false;
            }

            if (listNetwork.Items.Count > 0)
                selectAllNetworkMenuItem.Enabled = true;
            else
                selectAllNetworkMenuItem.Enabled = false;

            try
            {
                bool hasValid = false;

                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    if (item.SubItems[5].Text == "TCP")
                    {
                        if (item.SubItems[6].Text != "Listening" &&
                            item.SubItems[6].Text != "CloseWait" &&
                            item.SubItems[6].Text != "TimeWait")
                        {
                            hasValid = true;
                            break;
                        }
                    }
                }

                if (!hasValid)
                    closeNetworkMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            try
            {
                bool hasValid = false;

                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    if (item.SubItems[3].Text.Length > 0)
                    {
                        hasValid = true; 
                        break;
                    }
                }

                if (!hasValid)
                {
                    whoisNetworkMenuItem.Enabled = false;
                    tracertNetworkMenuItem.Enabled = false;
                    pingNetworkMenuItem.Enabled = false;
                }
                else
                {
                    whoisNetworkMenuItem.Enabled = true;
                    tracertNetworkMenuItem.Enabled = true;
                    pingNetworkMenuItem.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void goToProcessNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            this.SelectProcess(((NetworkItem)listNetwork.SelectedItems[0].Tag).Connection.Pid);
        }

        private void whoisNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            if (PhUtils.IsInternetConnected())
            {

                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    var remote = ((NetworkItem)item.Tag).Connection.Remote;

                    if (remote != null)
                    {
                        IPInfoWindow iw = new IPInfoWindow(remote.Address, IpAction.Whois);
                        iw.ShowDialog(this);
                    }
                }
            }
            else
                PhUtils.ShowError("An Internet session could not be established. Please verify connectivity.");
        }

        private void tracertNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;

            if (PhUtils.IsInternetConnected())
            {
                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    var remote = ((NetworkItem)item.Tag).Connection.Remote;

                    if (remote != null)
                    {
                        IPInfoWindow iw = new IPInfoWindow(remote.Address, IpAction.Tracert);
                        iw.ShowDialog(this);
                    }
                }
            }
            else
                PhUtils.ShowError("An Internet session could not be established. Please verify connectivity.");
        }

        private void pingNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count != 1)
                return;
            if (PhUtils.IsInternetConnected())
            {
                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    var remote = ((NetworkItem)item.Tag).Connection.Remote;

                    if (remote != null)
                    {
                        IPInfoWindow iw = new IPInfoWindow(remote.Address, IpAction.Ping);
                        iw.ShowDialog(this);
                    }
                }
            }
            else
                PhUtils.ShowError("An Internet session could not be established. Please verify connectivity.");
        }

        private void closeNetworkMenuItem_Click(object sender, EventArgs e)
        {
            if (listNetwork.SelectedItems.Count == 0)
                return;

            bool allGood = true;

            try
            {
                foreach (ListViewItem item in listNetwork.SelectedItems)
                {
                    if (item.SubItems[5].Text != "TCP" ||
                        item.SubItems[6].Text != "Established")
                        continue;

                    try
                    {
                        Program.NetworkProvider.Dictionary[item.Name].Connection.CloseTcpConnection();
                    }
                    catch
                    {
                        allGood = false;

                        if (MessageBox.Show("Could not close the TCP connection. " +
                            "Make sure Process Hacker is running with administrative privileges.", "Process Hacker",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.Cancel)
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            if (allGood)
            {
                foreach (ListViewItem item in listNetwork.SelectedItems)
                    item.Selected = false;
            }          

        }

        private void selectAllNetworkMenuItem_Click(object sender, EventArgs e)
        {
            this.listNetwork.List.Items.SelectAll();
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
            //foreach (MenuItem item in processesMenuItem.MenuItems)
                //vistaMenu.SetImage(item, null);

            processesMenuItem.DropDownItems.Clear();

            // HACK: To be fixed later - we need some sort of locking for the process provider
            try
            {
                foreach (var process in Program.ProcessProvider.Dictionary.Values)
                {
                    if (process.Pid > 0)
                    {
                        processes.Add(process);
                    }
                }

                // Remove zero CPU usage processes and processes running as other users
                for (int i = 0; i < processes.Count && processes.Count > Settings.Instance.IconMenuProcessCount; i++)
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

                // Sort the processes by CPU usage and remove processes with low CPU usage
                processes.Sort((i1, i2) => -i1.CpuUsage.CompareTo(i2.CpuUsage));

                if (processes.Count > Settings.Instance.IconMenuProcessCount)
                {
                    int c = processes.Count;
                    processes.RemoveRange(Settings.Instance.IconMenuProcessCount,
                        processes.Count - Settings.Instance.IconMenuProcessCount);
                }

                // Then sort the processes by name
                processes.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));

                // Add the processes
                foreach (var process in processes)
                {
                    MenuItem processItem = new MenuItem();
                    MenuItem terminateItem = new MenuItem();
                    MenuItem suspendItem = new MenuItem();
                    MenuItem resumeItem = new MenuItem();
                    MenuItem propertiesItem = new MenuItem();

                    processItem.Text = process.Name + " (" + process.Pid.ToString() + ")";
                    processItem.Tag = process;

                    terminateItem.Click += (sender_, e_) =>
                    {
                        ProcessItem item = ((MenuItem)sender_).Parent.Tag as ProcessItem;

                        ProcessActions.Terminate(this, new int[] { item.Pid }, new string[] { item.Name }, true);
                    };
                    terminateItem.Text = "Terminate";

                    suspendItem.Click += (sender_, e_) =>
                    {
                        ProcessItem item = ((MenuItem)sender_).Parent.Tag as ProcessItem;

                        ProcessActions.Suspend(this, new int[] { item.Pid }, new string[] { item.Name }, true);
                    };
                    suspendItem.Text = "Suspend";

                    resumeItem.Click += (sender_, e_) =>
                    {
                        ProcessItem item = ((MenuItem)sender_).Parent.Tag as ProcessItem;

                        ProcessActions.Resume(this, new int[] { item.Pid }, new string[] { item.Name }, true);
                    };
                    resumeItem.Text = "Resume";

                    //propertiesItem.Click += (sender_, e_) =>
                    //{
                    //    try
                    //    {
                    //        ProcessItem item = (ProcessItem)((MenuItem)sender_).Parent.Tag;

                    //        Program.GetProcessWindow(Program.ProcessProvider.Dictionary[item.Pid], f =>
                    //        {
                    //            f.Show();
                    //            f.Activate();
                    //        });
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        PhUtils.ShowException("Unable to inspect the process", ex);
                    //    }
                    //};
                    //propertiesItem.Text = "Properties";

                    //processItem.MenuItems.AddRange(new MenuItem[] { terminateItem, suspendItem, resumeItem, propertiesItem });
                    //processesMenuItem.DropDownItems.Add(processItem);

                   // vistaMenu.SetImage(processItem, (treeProcesses.Tree.Model as ProcessTreeModel).Nodes[process.Pid].Icon);
                }
            }
            catch
            {
                //foreach (MenuItem item in processesMenuItem.MenuItems)
                    //vistaMenu.SetImage(item, null);

                processesMenuItem.DropDownItems.Clear();
            }
        }

        private void showHideMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible)
            {
                Settings.Instance.WindowLocation = this.Location;
                Settings.Instance.WindowSize = this.Size;
            }

            this.Visible = !this.Visible;

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Location = Settings.Instance.WindowLocation;
                this.Size = Settings.Instance.WindowSize;
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
        }

        private void sysInformationIconMenuItem_Click(object sender, EventArgs e)
        {
            sysInfoMenuItem_Click(sender, e);
        }

        private void networkInfomationMenuItem_Click(object sender, EventArgs e)
        {
            new NetInfoWindow().Show();
        }

        private void enableAllNotificationsMenuItem_Click(object sender, EventArgs e)
        {
            newProcessesToolStripMenuItem.Checked = true;
            terminatedProcessesToolStripMenuItem.Checked = true;
            newServicesToolStripMenuItem.Checked = true;
            startedServicesToolStripMenuItem.Checked = true;
            stoppedServicesToolStripMenuItem.Checked = true;
            deletedServicesToolStripMenuItem.Checked = true;
        }

        private void disableAllNotificationsMenuItem_Click(object sender, EventArgs e)
        {
            newProcessesToolStripMenuItem.Checked = false;
            terminatedProcessesToolStripMenuItem.Checked = false;
            newServicesToolStripMenuItem.Checked = false;
            startedServicesToolStripMenuItem.Checked = false;
            stoppedServicesToolStripMenuItem.Checked = false;
            deletedServicesToolStripMenuItem.Checked = false;
        }

        private void exitTrayMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        #endregion

        #region Process Context Menu

        private void menuProcess_Popup(object sender, EventArgs e)
        {
            virtualizationToolStripMenuItem.Checked = false;

            // Menu item fixup...
            if (treeProcesses.SelectedTreeNodes.Count == 0)
            {
                // If nothing is selected, disable everything.
                // The Select All menu item will be enabled later if 
                // we have at least one process in the tree.
                //contextMenuStripProcess.DisableAll();
            }
            else if (treeProcesses.SelectedTreeNodes.Count == 1)
            {
                // All actions should work with one process selected.
                //contextMenuStripProcess.EnableAll();

                // Singular nouns.
                //priorityMenuItem.Text = "&Priority";
                //terminateToolStripMenuItem.Text = "&Terminate Process";
                //suspendToolStripMenuItem.Text = "&Suspend Process";
                //resumeToolStripMenuItem.Text = "&Resume Process";

                // Clear the priority menu items.
                realTimeToolStripMenuItem.Checked = false;
                highToolStripMenuItem.Checked = false;
                aboveNormalToolStripMenuItem.Checked = false;
                normalToolStripMenuItem.Checked = false;
                belowNormalToolStripMenuItem.Checked = false;
                idleToolStripMenuItem.Checked = false;

                // Clear the I/O priority menu items.
                iOPriorityToolStripMenuItem.Enabled = true;
                ioPriority0ThreadMenuItem.Checked = false;
                ioPriority1ThreadMenuItem.Checked = false;
                ioPriority3ThreadMenuItem.Checked = false;
                ioPriority3ThreadMenuItem.Checked = false;

                try
                {
                    using (ProcessHandle phandle = new ProcessHandle(processSelectedPid, Program.MinProcessQueryRights))
                    {
                        try
                        {
                            switch (phandle.PriorityClass)
                            {
                                case ProcessPriorityClass.RealTime:
                                    realTimeToolStripMenuItem.Checked = true;
                                    break;
                                case ProcessPriorityClass.High:
                                    highToolStripMenuItem.Checked = true;
                                    break;
                                case ProcessPriorityClass.AboveNormal:
                                    aboveNormalToolStripMenuItem.Checked = true;
                                    break;
                                case ProcessPriorityClass.Normal:
                                    normalToolStripMenuItem.Checked = true;
                                    break;
                                case ProcessPriorityClass.BelowNormal:
                                    belowNormalToolStripMenuItem.Checked = true;
                                    break;
                                case ProcessPriorityClass.Idle:
                                    idleToolStripMenuItem.Checked = true;
                                    break;
                            }
                        }
                        catch
                        {
                            realTimeToolStripMenuItem.Enabled = false;
                        }

                        try
                        {
                            if (OSVersion.HasIoPriority)
                            {
                                switch (phandle.IoPriority)
                                {
                                    case 0:
                                        ioPriority0ThreadMenuItem.Checked = true;
                                        break;
                                    case 1:
                                        ioPriority1ThreadMenuItem.Checked = true;
                                        break;
                                    case 2:
                                        ioPriority3ThreadMenuItem.Checked = true;
                                        break;
                                    case 3:
                                        ioPriority3ThreadMenuItem.Checked = true;
                                        break;
                                }
                            }
                        }
                        catch
                        {
                            iOPriorityToolStripMenuItem.Enabled = false;
                        }
                    }
                }
                catch
                {
                    priorityToolStripMenuItem.Enabled = false;
                    iOPriorityToolStripMenuItem.Enabled = false;
                }

                // Check if we think the process exists. If we don't, disable all menu items
                // to avoid random exceptions occurring when the user clicks on certain things.
                if (!Program.ProcessProvider.Dictionary.ContainsKey(processSelectedPid))
                {
                    //menuProcess.DisableAll();
                }
                else
                {
                    // Check the virtualization menu item.
                    try
                    {
                        using (ProcessHandle phandle = new ProcessHandle(processSelectedPid, Program.MinProcessQueryRights))
                        using (TokenHandle thandle = phandle.GetToken(TokenAccess.Query))
                        {
                            if (virtualizationToolStripMenuItem.Enabled = thandle.IsVirtualizationAllowed)
                                virtualizationToolStripMenuItem.Checked = thandle.IsVirtualizationEnabled;
                        }
                    }
                    catch
                    {
                        virtualizationToolStripMenuItem.Enabled = false;
                    }

                    // Enable/disable DLL injection based on the process' session ID. This only applies 
                    // on XP and above.
                    try
                    {
                        if (
                            OSVersion.IsBelowOrEqual(WindowsVersion.XP) &&
                            Program.ProcessProvider.Dictionary[processSelectedPid].SessionId != Program.CurrentSessionId
                            )
                            injectDLLToolStripMenuItem.Enabled = false;
                        else
                            injectDLLToolStripMenuItem.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }

                    // Disable Terminate Process Tree if the selected process doesn't 
                    // have any children. Note that this may also happen if the user 
                    // is sorting the list (!).
                    try
                    {
                        if (treeProcesses.SelectedTreeNodes[0].IsLeaf && string.IsNullOrEmpty((treeProcesses.Tree.Model as ProcessTreeModel).GetSortColumn()))
                            terminateProcessTreeToolStripMenuItem.Visible = false;
                        else
                            terminateProcessTreeToolStripMenuItem.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }

                    // Find the process' window (if any).
                    windowHandle = WindowHandle.Zero;
                    WindowHandle.Enumerate(handle =>
                    {
                        // GetWindowLong
                        // Shell_TrayWnd
                        if (handle.IsWindow && handle.IsVisible && handle.IsParent)
                        {
                            int pid;
                            Win32.GetWindowThreadProcessId(handle, out pid);

                            if (pid == processSelectedPid)
                            {
                                windowHandle = handle;
                                return false;
                            }
                        }
                        return true;
                    });

                    // Enable the Window submenu if we found window owned 
                    // by the process. Otherwise, disable the submenu.
                    if (windowHandle.IsInvalid)
                    {
                        windowToolStripMenuItem1.Enabled = false;
                    }
                    else
                    {
                        windowToolStripMenuItem1.Enabled = true;
                        //windowToolStripMenuItem1.EnableAll();

                        switch (windowHandle.Placement.ShowState)
                        {
                            case ShowWindowType.ShowMinimized:
                                minimizeToolStripMenuItem.Enabled = false;
                                break;

                            case ShowWindowType.ShowMaximized:
                                maximizeToolStripMenuItem.Enabled = false;
                                break;

                            case ShowWindowType.ShowNormal:
                                restoreToolStripMenuItem.Enabled = false;
                                break;
                        }
                    }
                }
            }
            else
            {
                // Assume most process actions will not work with more than one process.
                //menuProcess.DisableAll();

                // Use plural nouns.
                terminateProcessTreeToolStripMenuItem.Text = "&Terminate Processes";
                suspendToolStripMenuItem.Text = "&Suspend Processes";
                resumeToolStripMenuItem.Text = "&Resume Processes";

                // Enable a specific set of actions.
                terminateToolStripMenuItem.Enabled = true;
                suspendToolStripMenuItem.Enabled = true;
                resumeToolStripMenuItem.Enabled = true;
                reduceWorkingSetToolStripMenuItem.Enabled = true;
                copyProcessMenuItem.Enabled = true;
            }

            // Special case for invalid PIDs.
            if (processSelectedPid <= 0 && treeProcesses.SelectedNodes.Count == 1)
            {
                //priorityMenuItem.Text = "&Priority";
                //menuProcess.DisableAll();
                propertiesToolStripMenuItem1.Enabled = true;
            }

            // Enable/disable the Select All menu item.
            if (treeProcesses.Model.Nodes.Count == 0)
            {
                selectAllToolStripMenuItem.Enabled = false;
            }
            else
            {
                selectAllToolStripMenuItem.Enabled = true;
            }
        }

        private void terminateMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            if (ProcessActions.Terminate(this, pids, names, true))
            {
                try
                {
                    TreeNodeAdv[] nodes = new TreeNodeAdv[treeProcesses.SelectedTreeNodes.Count];

                    treeProcesses.SelectedTreeNodes.CopyTo(nodes, 0);

                    foreach (TreeNodeAdv node in nodes)
                        node.IsSelected = false;
                }
                catch
                { }
            }
        }

        private void terminateProcessTreeMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            if (ProcessActions.TerminateTree(this, pids, names, true))
            {
                try
                {
                    TreeNodeAdv[] nodes = new TreeNodeAdv[treeProcesses.SelectedTreeNodes.Count];

                    treeProcesses.SelectedTreeNodes.CopyTo(nodes, 0);

                    foreach (TreeNodeAdv node in nodes)
                        node.IsSelected = false;
                }
                catch
                { }
            }
        }

        private void suspendMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            ProcessActions.Suspend(this, pids, names, true);
        }

        private void resumeMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            ProcessActions.Resume(this, pids, names, true);
        }

        private void restartProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (PhUtils.ShowConfirmMessage(
                "restart",
                "the selected process",
                "The process will be restarted with the same command line and " + 
                "working directory, but if it is running under a different user it " + 
                "will be restarted under the current user.",
                true
                ))
            {
                try
                {
                    using (var phandle = new ProcessHandle(processSelectedPid,
                        Program.MinProcessQueryRights | Program.MinProcessReadMemoryRights))
                    {
                        string currentDirectory = phandle.GetPebString(PebOffset.CurrentDirectoryPath);
                        string cmdLine = phandle.GetPebString(PebOffset.CommandLine);

                        try
                        {
                            using (var phandle2 = new ProcessHandle(processSelectedPid, ProcessAccess.Terminate))
                                phandle2.Terminate();
                        }
                        catch (Exception ex)
                        {
                            PhUtils.ShowException("Unable to terminate the process", ex);
                            return;
                        }

                        try
                        {
                            ClientId cid;
                            ThreadHandle thandle;

                            ProcessHandle.CreateWin32(
                                null,
                                cmdLine,
                                false,
                                0,
                                EnvironmentBlock.Zero,
                                currentDirectory,
                                new StartupInfo(),
                                out cid,
                                out thandle
                                ).Dispose();
                            thandle.Dispose();
                        }
                        catch (Exception ex)
                        {
                            PhUtils.ShowException("Unable to start the command '" + cmdLine + "'", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to restart the process", ex);
                }
            }
        }

        private void reduceWorkingSetProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 0)
                return;

            int[] pids = new int[treeProcesses.SelectedNodes.Count];
            string[] names = new string[pids.Length];

            for (int i = 0; i < treeProcesses.SelectedNodes.Count; i++)
            {
                pids[i] = treeProcesses.SelectedNodes[i].Pid;
                names[i] = treeProcesses.SelectedNodes[i].Name;
            }

            ProcessActions.ReduceWorkingSet(this, pids, names, false);
        }

        private void virtualizationProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!PhUtils.ShowConfirmMessage(
                "set",
                "virtualization for the process",
                "Enabling or disabling virtualization for a process may " + 
                "alter its functionality and produce undesirable effects.",
                false
                ))
                return;

            try
            {
                using (ProcessHandle phandle = new ProcessHandle(processSelectedPid, Program.MinProcessQueryRights))
                using (TokenHandle thandle = phandle.GetToken(TokenAccess.GenericWrite))
                {
                    thandle.IsVirtualizationEnabled = !virtualizationToolStripMenuItem.Checked;
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set process virtualization", ex);
            }
        }

        private void propertiesProcessMenuItem_Click(object sender, EventArgs e)
        {
            // user hasn't got any processes selected
            if (processSelectedPid == -1)
                return;

            ProcessActions.ShowProperties(this, processSelectedPid, treeProcesses.SelectedNodes[0].Name);
        }

        private void affinityProcessMenuItem_Click(object sender, EventArgs e)
        {
            using (ProcessAffinity affForm = new ProcessAffinity(processSelectedPid))
            {
                try
                {
                    affForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }
        }

        private void createDumpFileProcessMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Dump Files (*.dmp)|*.dmp|All Files (*.*)|*.*";
            sfd.FileName =
                Program.ProcessProvider.Dictionary[processSelectedPid].Name +
                "_" +
                DateTime.Now.ToString("yyMMdd") +
                ".dmp";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    Exception exception = null;

                    ThreadStart dumpProcess = () =>
                        {
                            try
                            {
                                using (var phandle = new ProcessHandle(processSelectedPid,
                                    ProcessAccess.DupHandle | ProcessAccess.QueryInformation |
                                    ProcessAccess.SuspendResume | ProcessAccess.VmRead))
                                    phandle.WriteDump(sfd.FileName);
                            }
                            catch (Exception ex2)
                            {
                                exception = ex2;
                            }
                        };

                    if (OSVersion.HasTaskDialogs)
                    {
                        // Use a task dialog to display a fancy progress bar.
                        TaskDialog td = new TaskDialog();
                        Thread t = new Thread(dumpProcess, Utils.SixteenthStackSize);

                        td.AllowDialogCancellation = false;
                        td.Buttons = new TaskDialogButton[] { new TaskDialogButton((int)DialogResult.OK, "Close") };
                        td.WindowTitle = "Process Hacker";
                        td.MainInstruction = "Creating the dump file...";
                        td.ShowMarqueeProgressBar = true;
                        td.EnableHyperlinks = true;
                        td.PositionRelativeToWindow = true;
                        td.CallbackTimer = true;
                        td.Callback = (taskDialog, args, userData) =>
                            {
                                if (args.Notification == TaskDialogNotification.Created)
                                {
                                    taskDialog.SetMarqueeProgressBar(true);
                                    taskDialog.SetProgressBarState(ProgressBarState.Normal);
                                    taskDialog.SetProgressBarMarquee(true, 100);
                                    taskDialog.EnableButton((int)DialogResult.OK, false);
                                }
                                else if (args.Notification == TaskDialogNotification.Timer)
                                {
                                    if (!t.IsAlive)
                                    {
                                        taskDialog.EnableButton((int)DialogResult.OK, true);
                                        taskDialog.SetProgressBarMarquee(false, 0);
                                        taskDialog.SetMarqueeProgressBar(false);

                                        if (exception == null)
                                        {
                                            taskDialog.SetMainInstruction("The dump file has been created.");
                                            taskDialog.SetContent(
                                                "The dump file has been saved at: <a href=\"file\">" + sfd.FileName + "</a>.");
                                        }
                                        else
                                        {
                                            taskDialog.UpdateMainIcon(TaskDialogIcon.Warning);
                                            taskDialog.SetMainInstruction("Unable to create the dump file.");
                                            taskDialog.SetContent(
                                                "The dump file could not be created: " + exception.Message
                                                );
                                        }
                                    }
                                }
                                else if (args.Notification == TaskDialogNotification.HyperlinkClicked)
                                {
                                    if (args.Hyperlink == "file")
                                        Utils.ShowFileInExplorer(sfd.FileName);

                                    return true;
                                }

                                return false;
                            };

                        t.Start();
                        td.Show(this);
                    }
                    else
                    {
                        // No task dialogs, do the thing on the GUI thread.
                        dumpProcess();

                        if (exception != null)
                            PhUtils.ShowException("Unable to create the dump file", exception);
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to create the dump file", ex);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void terminatorProcessMenuItem_Click(object sender, EventArgs e)
        {
            using (TerminatorWindow w = new TerminatorWindow(processSelectedPid)
            {
                Text = "Terminator - " + Program.ProcessProvider.Dictionary[this.processSelectedPid].Name +
                       " (PID " + this.processSelectedPid.ToString() + ")"
            })
            {
                w.ShowDialog();
            }
        }

        #region Run As

        private void launchAsUserProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.Instance.RunAsCommand = Program.ProcessProvider.Dictionary[processSelectedPid].FileName;

                using (RunWindow run = new RunWindow())
                {
                    run.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void launchAsThisUserProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (RunWindow run = new RunWindow())
                {
                    run.UsePID(processSelectedPid);
                    run.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        #endregion

        #region Miscellaneous

        private void detachFromDebuggerProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var phandle = new ProcessHandle(processSelectedPid, ProcessAccess.QueryInformation | ProcessAccess.SuspendResume))
                {
                    using (var dhandle = phandle.GetDebugObject())
                        phandle.RemoveDebug(dhandle);
                }
            }
            catch (WindowsException ex)
            {
                if (ex.Status == NtStatus.PortNotSet)
                    PhUtils.ShowInformation("The process is not being debugged.");
                else
                    PhUtils.ShowException("Unable to detach the process", ex);
            }
        }

        private void heapsProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                HeapsWindow heapsWindow;

                using (DebugBuffer buffer = new DebugBuffer())
                {
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        buffer.Query(
                            processSelectedPid,
                            RtlQueryProcessDebugFlags.HeapSummary |
                            RtlQueryProcessDebugFlags.HeapEntries
                            );
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }

                    heapsWindow = new HeapsWindow(processSelectedPid, buffer.GetHeaps());
                }
                heapsWindow.ShowDialog();
            }
            catch (WindowsException ex)
            {
                PhUtils.ShowException("Unable to get heap information", ex);
            }
        }

        private void injectDllProcessMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "DLL Files (*.dll)|*.dll|All Files (*.*)|*.*"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (ProcessHandle phandle = new ProcessHandle(processSelectedPid, ProcessAccess.CreateThread | ProcessAccess.VmOperation | ProcessAccess.VmWrite))
                        {
                            phandle.InjectDll(ofd.FileName, 5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to inject the DLL", ex);
                    }
                }
            }
        }

        #region I/O Priority

        private void ioPriority0ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessIoPriority(0);
        }

        private void ioPriority1ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessIoPriority(1);
        }

        private void ioPriority2ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessIoPriority(2);
        }

        private void ioPriority3ThreadMenuItem_Click(object sender, EventArgs e)
        {
            SetProcessIoPriority(3);
        }

        #endregion

        private void protectionProcessMenuItem_Click(object sender, EventArgs e)
        {
            var protectProcessWindow = new ProtectProcessWindow(processSelectedPid);
            protectProcessWindow.ShowDialog();
        }

        private void setTokenProcessMenuItem_Click(object sender, EventArgs e)
        {
            using (ProcessPickerWindow picker = new ProcessPickerWindow
            {
                Label = "Select the source of the token:"
            })
            {
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //KProcessHacker.Instance.SetProcessToken(picker.SelectedPid, processSelectedPid);
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to set the process token", ex);
                    }
                }
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

        #region Window

        private void bringToFrontProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow)
            {
                WindowPlacement placement = windowHandle.Placement;

                if (placement.ShowState == ShowWindowType.ShowMinimized)
                    windowHandle.Show(ShowWindowType.Restore);
                else
                    windowHandle.SetForeground();
            }
        }

        private void restoreProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow)
            {
                windowHandle.Show(ShowWindowType.Restore);
            }
        }

        private void minimizeProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow)
            {
                windowHandle.Show(ShowWindowType.ShowMinimized);
            }
        }

        private void maximizeProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow)
            {
                windowHandle.Show(ShowWindowType.ShowMaximized);
            }
        }

        private void closeProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (!windowHandle.IsInvalid && windowHandle.IsWindow)
            {
                windowHandle.PostMessage(WindowMessage.Close, 0, 0);
                //windowHandle.Close();
            }
        }

        #endregion

        private void searchProcessMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count != 1)
                return;

            Program.TryStart(Settings.Instance.SearchEngine.Replace("%s", treeProcesses.SelectedNodes[0].Name));
        }

        private void reanalyzeProcessMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Program.ProcessProvider.QueueProcessQuery(processSelectedPid);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void selectAllProcessMenuItem_Click(object sender, EventArgs e)
        {
            treeProcesses.Tree.AllNodes.SelectAll();
            treeProcesses.Tree.Invalidate();
        }

        private void virusTotalMenuItem_Click(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count != 1)
                return;

            if (PhUtils.IsInternetConnected())
            {
                if (string.IsNullOrEmpty(treeProcesses.SelectedNodes[0].FileName))
                {
                    PhUtils.ShowWarning("Unable to upload because the process' file location could not be determined.");
                    return;
                }

                VirusTotalUploaderWindow vt = new VirusTotalUploaderWindow(
                     treeProcesses.SelectedNodes[0].Name,
                     treeProcesses.SelectedNodes[0].FileName
                     );

                int Y = this.Top + (this.Height - vt.Height) / 2;
                int X = this.Left + (this.Width - vt.Width) / 2;

                vt.Location = new Point(X, Y);
                vt.Show();
            }
            else
            {
                PhUtils.ShowError("An Internet session could not be established. Please verify connectivity.");
            }
        }

        private void analyzeWaitChainProcessMenuItem_Click(object sender, EventArgs e)
        {
            WaitChainWindow wcw = new WaitChainWindow(
                treeProcesses.SelectedNodes[0].Name, 
                treeProcesses.SelectedNodes[0].Pid);

            int Y = this.Top + (this.Height - wcw.Height) / 2;
            int X = this.Left + (this.Width - wcw.Width) / 2;

            wcw.Location = new Point(X, Y);
            wcw.Show();
        }

        #endregion

        #region Providers

        private void processP_Updated()
        {
            Program.ProcessProvider.DictionaryAdded += processP_DictionaryAdded;
            Program.ProcessProvider.DictionaryRemoved += processP_DictionaryRemoved;
            Program.ProcessProvider.Updated -= processP_Updated;

            ProcessHandle.Current.PriorityClass = ProcessPriorityClass.High;

            _enableNetworkProviderSync.Increment();
            _refreshHighlightingSync.Increment();

            if (Program.ProcessProvider.RunCount >= 1)
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.treeProcesses.Tree.EndCompleteUpdate();
                    this.treeProcesses.Tree.EndUpdate();

                    if (Settings.Instance.ScrollDownProcessTree)
                    {
                        // HACK
                        try
                        {
                            foreach (var process in this.treeProcesses.Model.Roots)
                            {
                                if (
                                    string.Equals(process.Name, "explorer.exe",
                                                  StringComparison.OrdinalIgnoreCase) &&
                                    process.ProcessItem.Username == Program.CurrentUsername)
                                {
                                    this.treeProcesses.FindTreeNode(process).EnsureVisible2();

                                    break;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    this.treeProcesses.Invalidate();
                    Program.ProcessProvider.Boost();
                    this.Cursor = Cursors.Default;
                }));
        }

        private void processP_InfoUpdater()
        {
            this.BeginInvoke(new MethodInvoker(this.UpdateStatusInfo));
        }

        private void processP_FileProcessingReceived(int stage, int pid)
        {
            // Check if we need to inspect a process at startup.
            if (stage == 0x1 && Program.InspectPid != -1 && pid == Program.InspectPid)
            {
                Program.ProcessProvider.ProcessQueryReceived -= processP_FileProcessingReceived;
                ProcessActions.ShowProperties(this, pid, Program.ProcessProvider.Dictionary[pid].Name);
            }
        }

        public void processP_DictionaryAdded(ProcessItem item)
        {
            ProcessItem parent = null;
            string parentText = string.Empty;

            if (item.HasParent && Program.ProcessProvider.Dictionary.ContainsKey(item.ParentPid))
            {
                try
                {
                    parent = Program.ProcessProvider.Dictionary[item.ParentPid];

                    parentText += " started by " + parent.Name + " (PID " + parent.Pid.ToString() + ")";
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }

            this.QueueMessage("New Process: " + item.Name + " (PID " + item.Pid.ToString() + ")" + parentText);

            if (newProcessesToolStripMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "New Process",
                    "The process " + item.Name + " (" + item.Pid.ToString() +
                    ") was started" + ((parentText != string.Empty) ? " by " +
                    parent.Name + " (" + parent.Pid.ToString() + ")" : string.Empty) + ".", ToolTipIcon.Info);
        }

        public void processP_DictionaryRemoved(ProcessItem item)
        {
            this.QueueMessage("Terminated Process: " + item.Name + " (PID " + item.Pid.ToString() + ")");

            if (processServices.ContainsKey(item.Pid))
                processServices.Remove(item.Pid);

            if (terminatedProcessesToolStripMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "Terminated Process",
                    "The process " + item.Name + " (" + item.Pid.ToString() + ") was terminated.", ToolTipIcon.Info);
        }

        private void serviceP_Updated()
        {
            listServices.BeginInvoke(new MethodInvoker(() => this.listServices.List.EndUpdate()));

            HighlightingContext.StateHighlighting = true;

            Program.ServiceProvider.DictionaryAdded += serviceP_DictionaryAdded;
            Program.ServiceProvider.DictionaryModified += serviceP_DictionaryModified;
            Program.ServiceProvider.DictionaryRemoved += serviceP_DictionaryRemoved;
            Program.ServiceProvider.Updated -= serviceP_Updated;

            _refreshHighlightingSync.Increment();
        }

        public void serviceP_DictionaryAdded(ServiceItem item)
        {
            this.QueueMessage("New Service: " + item.Status.ServiceName +
                " (" + item.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                (!string.IsNullOrEmpty(item.Status.DisplayName) ?
                " (" + item.Status.DisplayName + ")" :
                string.Empty));

            if (newServicesToolStripMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "New Service",
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
            var oldState = oldItem.Status.ServiceStatusProcess.CurrentState;
            var newState = newItem.Status.ServiceStatusProcess.CurrentState;

            if ((oldState == ServiceState.Paused || oldState == ServiceState.Stopped ||
                oldState == ServiceState.StartPending) &&
                newState == ServiceState.Running)
            {
                this.QueueMessage("Service Started: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != string.Empty) ?
                    " (" + newItem.Status.DisplayName + ")" :
                    string.Empty));

                if (startedServicesToolStripMenuItem.Checked)
                    this.GetFirstIcon().ShowBalloonTip(2000, "Service Started",
                        "The service " + newItem.Status.ServiceName + " (" + newItem.Status.DisplayName + ") has been started.",
                        ToolTipIcon.Info);
            }

            if (oldState == ServiceState.Running &&
                newState == ServiceState.Paused)
                this.QueueMessage("Service Paused: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != string.Empty) ?
                    " (" + newItem.Status.DisplayName + ")" :
                    string.Empty));

            if (oldState == ServiceState.Running &&
                newState == ServiceState.Stopped)
            {
                this.QueueMessage("Service Stopped: " + newItem.Status.ServiceName +
                    " (" + newItem.Status.ServiceStatusProcess.ServiceType.ToString() + ")" +
                    ((newItem.Status.DisplayName != string.Empty) ?
                    " (" + newItem.Status.DisplayName + ")" :
                    string.Empty));

                if (stoppedServicesToolStripMenuItem.Checked)
                    this.GetFirstIcon().ShowBalloonTip(2000, "Service Stopped",
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
                ((item.Status.DisplayName != string.Empty) ?
                " (" + item.Status.DisplayName + ")" :
                string.Empty));

            if (deletedServicesToolStripMenuItem.Checked)
                this.GetFirstIcon().ShowBalloonTip(2000, "Service Deleted",
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

        private void goToProcessServiceMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectProcess(
                Program.ServiceProvider.Dictionary[listServices.SelectedItems[0].Name].
                Status.ServiceStatusProcess.ProcessID);
        }

        private void startServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Start(this, listServices.SelectedItems[0].Name, false);
        }

        private void continueServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Continue(this, listServices.SelectedItems[0].Name, false);
        }

        private void pauseServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Pause(this, listServices.SelectedItems[0].Name, false);
        }

        private void stopServiceMenuItem_Click(object sender, EventArgs e)
        {
            ServiceActions.Stop(this, listServices.SelectedItems[0].Name, false);
        }

        private void deleteServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count != 1)
                return;

            ServiceActions.Delete(this, listServices.SelectedItems[0].Name, true);
        }

        private void propertiesServiceMenuItem_Click(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count == 0)
                return;

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
            sw.ShowDialog();
        }

        private void selectAllServiceMenuItem_Click(object sender, EventArgs e)
        {
            this.listServices.Items.SelectAll();
        }

        #endregion

        #region Tab Controls

        private void tabControlBig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabNetwork)
            {
                _enableNetworkProviderSync.IncrementMultiple();
            }
            else
            {
                Program.NetworkProvider.Enabled = false;
            }
        }

        #endregion

        #region Thumbbuttons

        private void sysInfoButton_Clicked(object sender, EventArgs e)
        {
            sysInfoMenuItem_Click(sender, e);
        }

        private void netInfoButton_Click(object sender, EventArgs e)
        {
            networkInfomationMenuItem_Click(sender, e);
        }

        private void appHandleButton_Clicked(object sender, EventArgs e)
        {
            findHandlesMenuItem_Click(sender, e);
        }

        private void appLogButton_Clicked(object sender, EventArgs e)
        {
            logMenuItem_Click(sender, e);
        }

        #endregion

        #region Thumbbutton Managers

        private void thumbButtonManager_TaskbarButtonCreated(object sender, EventArgs e)
        {
            thumbButtonManager.TaskbarButtonCreated -= thumbButtonManager_TaskbarButtonCreated;
            
            //JumpListManager code works but has been commented out and reserved for future use

            //jumpListManager = Windows7Taskbar.CreateJumpListManager();
            //jumpListManager.UserRemovedItems += (o, e_) =>
            //{
            //QueueMessage("User removed " + e_.RemovedItems.Length + " items (cancelling refresh)");
            //e_.Cancel = true;
            //};

            //jumpListManager.ClearAllDestinations();
            //jumpListManager.EnabledAutoDestinationType = ApplicationDestinationType.Recent;
            
            //string shell32DllPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
           
            //jumpListManager.AddUserTask(new ShellLink
            //{
                //Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "eventvwr.msc"),
                //Arguments = "/s",
                //Title = "Event Viewer",
                //IconLocation = shell32DllPath,
                //IconIndex = 14
            //});

            //jumpListManager.AddUserTask(new Separator());

            //jumpListManager.AddUserTask(new ShellLink
            //{
                //Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "msinfo32.exe"),
                //Title = "System Infomation",
                //IconLocation = shell32DllPath,
                //IconIndex = 15
            //});

            //if (jumpListManager.Refresh())
            //{
            //QueueMessage("Maximum slots in JumpList: " + jumpListManager.MaximumSlotsInList);
            //} 
            //jumpListManager.Dispose();

            ThumbButton sysInfoButton = thumbButtonManager.CreateThumbButton(100,
                Icon.FromHandle(ProcessHacker.Properties.Resources.chart_line.GetHicon()),
                "System Infomation");
            sysInfoButton.Click += new EventHandler(sysInfoButton_Clicked);

            //ThumbButton netInfoButton = thumbButtonManager.CreateThumbButton(101,
                //Icon.FromHandle(ProcessHacker.Properties.Resources.ProcessHacker.GetHicon()),
                //"Network Infomation");
            //netInfoButton.Click += new EventHandler(netInfoButton_Click);

            ThumbButton appHandleButton = thumbButtonManager.CreateThumbButton(103,
                Icon.FromHandle(ProcessHacker.Properties.Resources.find.GetHicon()),
                "Find Handles or DLLs");
            appHandleButton.Click += new EventHandler(appHandleButton_Clicked);

            ThumbButton appLogButton = thumbButtonManager.CreateThumbButton(102,
                Icon.FromHandle(ProcessHacker.Properties.Resources.report.GetHicon()),
                "Application Log");
            appLogButton.Click += new EventHandler(appLogButton_Clicked);

            thumbButtonManager.AddThumbButtons(sysInfoButton, appHandleButton, appLogButton);
        }

        #endregion

        #region ToolStrip Items

        private void findHandlesToolStripButton_Click(object sender, EventArgs e)
        {
            findHandlesMenuItem.PerformClick();
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {
            if (Program.ProcessProvider.RunCount > 1)
                Program.ProcessProvider.Boost();

            if (Program.ServiceProvider.RunCount > 1)
                Program.ServiceProvider.Boost();
        }

        private void sysInfoToolStripButton_Click(object sender, EventArgs e)
        {
            sysInfoMenuItem.PerformClick();
        }

        private void optionsToolStripButton_Click(object sender, EventArgs e)
        {
            optionsMenuItem.PerformClick();
        }

        #endregion

        #region Trees

        private void treeProcesses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                terminateMenuItem_Click(null, null);
            }
            else if (e.KeyData == (Keys.Shift | Keys.Delete))
            {
                terminateProcessTreeMenuItem_Click(null, null);
            }
            else if (e.KeyData == Keys.Enter)
            {
                propertiesProcessMenuItem_Click(null, null);
            }
            else if (e.KeyData == (Keys.Control | Keys.M))
            {
                searchProcessMenuItem_Click(null, null);
            }
        }

        private void treeProcesses_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            propertiesProcessMenuItem_Click(null, null);
        }

        private void treeProcesses_SelectionChanged(object sender, EventArgs e)
        {
            processSelectedItems = treeProcesses.SelectedNodes.Count;

            if (processSelectedItems == 1)
            {
                processSelectedPid = treeProcesses.SelectedNodes[0].Pid;
            }
            else
            {
                processSelectedPid = -1;
            }
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
            AddMenuItemDelegate addMenuItem = (text, onClick) =>
            {
                shutdownMenuItem.DropDownItems.Add(text, null, onClick);
                shutdownTrayMenuItem.DropDownItems.Add(text, null, onClick);
                shutDownToolStripMenuItem.DropDownItems.Add(text, null, onClick);
            };

            addMenuItem("Lock", (sender, e) => Win32.LockWorkStation());
            addMenuItem("Logoff", (sender, e) => Win32.ExitWindowsEx(ExitWindowsFlags.Logoff, 0));
            addMenuItem("-", null);
            addMenuItem("Sleep", (sender, e) => Win32.SetSuspendState(false, false, false));
            addMenuItem("Hibernate", (sender, e) => Win32.SetSuspendState(true, false, false));
            addMenuItem("-", null);
            addMenuItem("Restart", (sender, e) =>
            {
                if (PhUtils.ShowConfirmMessage("restart", "the computer", null, false))
                    Win32.ExitWindowsEx(ExitWindowsFlags.Reboot, 0);
            });
            addMenuItem("Shutdown", (sender, e) =>
            {
                if (PhUtils.ShowConfirmMessage("shutdown", "the computer", null, false))
                    Win32.ExitWindowsEx(ExitWindowsFlags.Shutdown, 0);
            });
            addMenuItem("Poweroff", (sender, e) =>
            {
                if (PhUtils.ShowConfirmMessage("poweroff", "the computer", null, false))
                    Win32.ExitWindowsEx(ExitWindowsFlags.Poweroff, 0);
            });
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
            Button button = new Button
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

        private void LoadWindowSettings()
        {
            this.TopMost = Program.HackerWindowTopMost = Settings.Instance.AlwaysOnTop;

            this.Size = Settings.Instance.WindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Settings.Instance.WindowLocation, this.Size), this).Location;

            if (Settings.Instance.WindowState != FormWindowState.Minimized)
                this.WindowState = Settings.Instance.WindowState;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void LoadOtherSettings()
        {
            Utils.UnitSpecifier = Settings.Instance.UnitSpecifier;
            PromptBox.LastValue = Settings.Instance.PromptBoxText;
            toolbarMenuItem.Checked = toolStrip.Visible = Settings.Instance.ToolbarVisible;

            if (Settings.Instance.ToolStripDisplayStyle == 1)
            {
                findHandlesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                sysInfoToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                optionsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                shutDownToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }
            else if (Settings.Instance.ToolStripDisplayStyle == 2)
            {
                refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                optionsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                shutDownToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                findHandlesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                sysInfoToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;   
            }
            else
            {
                refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                optionsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                shutDownToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
                findHandlesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                sysInfoToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;   
            }

            ColumnSettings.LoadSettings(Settings.Instance.ProcessTreeColumns, treeProcesses.Tree);
            ColumnSettings.LoadSettings(Settings.Instance.ServiceListViewColumns, listServices.List);
            ColumnSettings.LoadSettings(Settings.Instance.NetworkListViewColumns, listNetwork.List);

            HighlightingContext.Colors[ListViewItemState.New] = Settings.Instance.ColorNew;
            HighlightingContext.Colors[ListViewItemState.Removed] = Settings.Instance.ColorRemoved;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Settings.Instance.ColorNew;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Settings.Instance.ColorRemoved;

            Program.ImposterNames = new System.Collections.Specialized.StringCollection();

            foreach (string s in Settings.Instance.ImposterNames.Split(','))
                Program.ImposterNames.Add(s.Trim());

            Program.ProcessProvider.HistoryMaxSize = Settings.Instance.MaxSamples;
            ProcessHacker.Components.Plotter.GlobalMoveStep = Settings.Instance.PlotterStep;

            // Set up symbols...

            // If this is the first time Process Hacker is being run, try to 
            // set up symbols automatically to make the user happy :).
            // We need the exception handler because some people have their 
            // ProgramFiles variable set incorrectly.
            try
            {
                if (Settings.Instance.FirstRun)
                {
                    string defaultDbghelp = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                        "\\Debugging Tools for Windows (" +
                        (OSVersion.Architecture == OSArch.I386 ? "x86" : "x64") +
                        ")\\dbghelp.dll";

                    if (System.IO.File.Exists(defaultDbghelp))
                        Settings.Instance.DbgHelpPath = defaultDbghelp;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            // If we couldn't load dbghelp.dll from the user's location, load the default one 
            // in PATH (usually in system32).
            if (Loader.LoadDll(Settings.Instance.DbgHelpPath) == IntPtr.Zero)
                Loader.LoadDll("dbghelp.dll");

            // Load symsrv.dll from the same directory as dbghelp.dll.
            // TODO: improve logic.
            if (Loader.LoadDll(System.IO.Path.GetDirectoryName(Settings.Instance.DbgHelpPath) + "\\symsrv.dll") == IntPtr.Zero)
                Loader.LoadDll("symsrv.dll");

            // Set the first run setting here.
            Settings.Instance.FirstRun = false;
        }

        public void QueueMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => this.QueueMessage(message)));
                return;
            }

            var value = new KeyValuePair<DateTime, string>(DateTime.Now, message);

            _log.Add(value);

            if (this.LogUpdated != null)
                this.LogUpdated(value);
        }

        private void SaveSettings()
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible)
            {
                Settings.Instance.WindowLocation = this.Location;
                Settings.Instance.WindowSize = this.Size;
            }

            Settings.Instance.AlwaysOnTop = this.TopMost;
            Settings.Instance.WindowState = this.WindowState == FormWindowState.Minimized ?
                FormWindowState.Normal : this.WindowState;

            Settings.Instance.PromptBoxText = PromptBox.LastValue;

            Settings.Instance.ProcessTreeColumns = ColumnSettings.SaveSettings(treeProcesses.Tree);
            Settings.Instance.ServiceListViewColumns = ColumnSettings.SaveSettings(listServices.List);
            Settings.Instance.NetworkListViewColumns = ColumnSettings.SaveSettings(listNetwork.List);

            Settings.Instance.NewProcesses = newProcessesToolStripMenuItem.Checked;
            Settings.Instance.TerminatedProcesses = terminatedProcessesToolStripMenuItem.Checked;
            Settings.Instance.NewServices = newServicesToolStripMenuItem.Checked;
            Settings.Instance.StartedServices = startedServicesToolStripMenuItem.Checked;
            Settings.Instance.StoppedServices = stoppedServicesToolStripMenuItem.Checked;
            Settings.Instance.DeletedServices = deletedServicesToolStripMenuItem.Checked;

            try
            {
                Settings.Instance.Save();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to save settings", ex);
            }
        }

        public void SelectAll(TreeViewAdv tree)
        {
            foreach (TreeNodeAdv node in tree.AllNodes)
                node.IsSelected = true;
        }

        private void SelectProcess(int pid)
        {
            DeselectAll(treeProcesses.Tree);

            try
            {
                TreeNodeAdv node = treeProcesses.FindTreeNode(pid);

                node.EnsureVisible();
                node.IsSelected = true;
                treeProcesses.Tree.FullUpdate();
                treeProcesses.Tree.Invalidate();

                tabControl.SelectedTab = tabProcesses;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void UpdateProgram(bool interactive)
        {
            checkForUpdatesMenuItem.Enabled = false;

            NativeThreadPool.QueueWorkItem(o =>
            {
                Updater.Update(this, interactive);
                this.BeginInvoke(new MethodInvoker(() => this.checkForUpdatesMenuItem.Enabled = true));
            }, null);
        }

        private void UpdateSessions()
        {
            TerminalServerHandle currentServer = TerminalServerHandle.GetCurrent();

            usersToolStripMenuItem.DropDownItems.Clear();

            foreach (var session in currentServer.GetSessions())
            {
                string displayName = session.DomainName + "\\" + session.UserName;

                if (displayName == "\\")
                {
                    // Probably the Services or RDP-Tcp session.
                    session.Dispose();
                    continue;
                }

                AddMenuItemDelegate addMenuItem = (text, onClick) =>
                {
                    usersToolStripMenuItem.DropDownItems.Add(text, null, onClick);
                    shutdownTrayMenuItem.DropDownItems.Add(text, null, onClick);
                    shutDownToolStripMenuItem.DropDownItems.Add(text, null, onClick);
                };

                addMenuItem("Disconnect", (sender, e) =>
                {
                    int sessionId = (int)((MenuItem)sender).Tag;

                    SessionActions.Disconnect(this, session.SessionId, false);
                });

                //MenuItem userMenuItem = new MenuItem();
                //userMenuItem.Text = session.SessionId + ": " + displayName;
                addMenuItem("Logoff", (sender, e) =>
                {
                    int sessionId = (int)((MenuItem)sender).Tag;

                    SessionActions.Logoff(this, session.SessionId, true);
                });

                addMenuItem("Send Message...", (sender, e) =>
                {
                    try
                    {
                        MessageBoxWindow mbw = new MessageBoxWindow
                        {
                            MessageBoxTitle = "Message from " + Program.CurrentUsername
                        };

                        mbw.OkButtonClicked += () =>
                        {
                            try
                            {
                                TerminalServerHandle.GetCurrent().GetSession(session.SessionId).SendMessage(
                                    mbw.MessageBoxTitle,
                                    mbw.MessageBoxText,
                                    MessageBoxButtons.OK,
                                    mbw.MessageBoxIcon,
                                    0,
                                    0,
                                    mbw.MessageBoxTimeout,
                                    false
                                    );

                                return true;
                            }
                            catch (Exception ex)
                            {
                                PhUtils.ShowException("Unable to send the message", ex);
                                return false;
                            }
                        };
                        mbw.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to show the message window", ex);
                    }
                });

                addMenuItem("Properties...", (sender, e) =>
                {
                    try
                    {
                        using (var sessionWindow = new SessionInformationWindow(TerminalServerHandle.GetCurrent().GetSession(session.SessionId)))
                            sessionWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to show session properties", ex);
                    }
                });

                session.Dispose();
            }
        }

        private void UpdateStatusInfo()
        {
            if (Program.ProcessProvider.RunCount >= 1)
                statusGeneral.Text = string.Format("{0} processes", Program.ProcessProvider.Dictionary.Count - 2);
            else
                statusGeneral.Text = "Loading...";

            statusCPU.Text = "CPU: " + (Program.ProcessProvider.CurrentCpuUsage * 100).ToString("N2") + "%";
            statusMemory.Text = "Phys. Memory: " +
                ((float)(Program.ProcessProvider.System.NumberOfPhysicalPages - Program.ProcessProvider.Performance.AvailablePages) * 100 /
                Program.ProcessProvider.System.NumberOfPhysicalPages).ToString("N2") + "%";
        }

        #endregion

        #region Helper functions

        private void SetProcessPriority(ProcessPriorityClass priority)
        {
            try
            {
                using (ProcessHandle phandle = new ProcessHandle(processSelectedPid, ProcessAccess.SetInformation))
                    phandle.PriorityClass = priority;
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set process priority", ex);
            }
        }

        private void SetProcessIoPriority(int ioPriority)
        {
            try
            {
                using (ProcessHandle phandle = new ProcessHandle(processSelectedPid, ProcessAccess.SetInformation))
                    phandle.IoPriority = ioPriority;
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to set process I/O priority", ex);
            }
        }

        #endregion

        #region Notification Icons

        public void ExecuteOnIcons(Action<UsageIcon> action)
        {
            notifyIcons.ForEach(action);
        }

        public UsageIcon GetFirstIcon()
        {
            foreach (var icon in notifyIcons)
                if (icon.Visible)
                    return icon;

            return dummyIcon;
        }

        public int GetIconsVisibleCount()
        {
            int count = 0;

            foreach (var icon in notifyIcons)
                if (icon.Visible)
                    count++;

            return count;
        }

        public void ApplyIconVisibilities()
        {
            cpuHistoryIcon.Visible = cpuHistoryIcon.Enabled = Settings.Instance.CpuHistoryIconVisible;
            cpuUsageIcon.Visible = cpuUsageIcon.Enabled = Settings.Instance.CpuUsageIconVisible;
            ioHistoryIcon.Visible = ioHistoryIcon.Enabled = Settings.Instance.IoHistoryIconVisible;
            commitHistoryIcon.Visible = commitHistoryIcon.Enabled = Settings.Instance.CommitHistoryIconVisible;
            physMemHistoryIcon.Visible = physMemHistoryIcon.Enabled = Settings.Instance.PhysMemHistoryIconVisible;

            if (cpuHistoryIcon.Visible)
                UsageIcon.ActiveUsageIcon = cpuHistoryIcon;
            else
                UsageIcon.ActiveUsageIcon = null;
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
                        {
                            this.Location = Settings.Instance.WindowLocation;
                            this.Size = Settings.Instance.WindowSize;
                            this.WindowState = FormWindowState.Normal;
                        }
                        m.Result = new IntPtr(0x1119);

                        return;
                    }
                //break;

                case (int)WindowMessage.SysCommand:
                    {
                        if (m.WParam.ToInt32() == 0xf020) // SC_MINIMIZE
                        {
                            try
                            {
                                if (this.WindowState == FormWindowState.Normal && this.Visible)
                                {
                                    Settings.Instance.WindowLocation = this.Location;
                                    Settings.Instance.WindowSize = this.Size;
                                }

                                if (this.GetIconsVisibleCount() > 0 && Settings.Instance.HideWhenMinimized)
                                {
                                    this.Visible = false;

                                    return;
                                }
                            }
                            catch
                            { }
                        }
                    }
                    break;
                case (int)WindowMessage.Activate:
                case (int)WindowMessage.KillFocus:
                    {
                        if (treeProcesses != null && treeProcesses.Tree != null)
                            treeProcesses.Tree.Invalidate();
                    }
                    break;

                case (int)WindowMessage.WtsSessionChange:
                    {
                        WtsSessionChangeEvent changeEvent = (WtsSessionChangeEvent)m.WParam.ToInt32();

                        if (
                            changeEvent == WtsSessionChangeEvent.SessionLogon ||
                            changeEvent == WtsSessionChangeEvent.SessionLogoff
                            )
                        {
                            try
                            {
                                this.UpdateSessions();
                            }
                            catch (Exception ex)
                            {
                                Logging.Log(ex);
                            }
                        }
                    }
                    break;

                case (int)WindowMessage.SettingChange:
                    {
                        // Refresh icon sizes.
                        this.ExecuteOnIcons(icon => icon.Size = UsageIcon.GetSmallIconSize());
                        // Refresh the tree view visual style.
                        treeProcesses.Tree.RefreshVisualStyles();
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        public void Exit()
        {
            this.Exit(true);
        }

        public void Exit(bool saveSettings)
        {
            //processP.Dispose();
            //serviceP.Dispose();
            //networkP.Dispose();

            this.ExecuteOnIcons(icon => icon.Visible = false);
            this.ExecuteOnIcons(icon => icon.Dispose());

            // Only save settings if requested and no other instance of 
            // PH is running.
            if (saveSettings && !Program.CheckPreviousInstance())
                SaveSettings();

            this.Visible = false;

            if (KProcessHacker2.Instance != null)
                KProcessHacker2.Instance.Close();

            try
            {
                Win32.ExitProcess(0);
            }
            catch
            { }
        }

        private void HackerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (
                e.CloseReason != CloseReason.WindowsShutDown &&
                this.GetIconsVisibleCount() > 0 &&
                Settings.Instance.HideWhenClosed
                )
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

        public void LoadFixOSSpecific()
        {
            //if (KProcessHacker.Instance == null)
                //hiddenProcessesMenuItem.Visible = false;

            //if (KProcessHacker.Instance == null || !OSVersion.HasSetAccessToken)
                setTokenToolStripMenuItem.Visible = false;

            //if (KProcessHacker.Instance == null || !Settings.Instance.EnableExperimentalFeatures)
                protectionToolStripMenuItem.Visible = false;

            if (!OSVersion.HasUac)
                virtualizationToolStripMenuItem.Visible = false;

            if (OSVersion.IsBelow(WindowsVersion.Vista))
                analyzeWaitChainToolStripMenuItem.Visible = false;

            if (OSVersion.IsBelow(WindowsVersion.XP))
                tabControl.TabPages.Remove(tabNetwork);

            if (!OSVersion.HasIoPriority)
                iOPriorityToolStripMenuItem.Visible = false;
        }

        private void LoadFixNProcessHacker()
        {
            bool nphExists, nph32Exists, nph64Exists;
            string startupPath = Application.StartupPath;

            try
            {
                nphExists = System.IO.File.Exists(startupPath + "\\NProcessHacker.dll");
                nph32Exists = System.IO.File.Exists(startupPath + "\\NProcessHacker32.dll");
                nph64Exists = System.IO.File.Exists(startupPath + "\\NProcessHacker64.dll");

                // If we're on 32-bit and NPH32 exists, rename NPH to NPH64 and 
                // NPH32 to NPH.
                if (OSVersion.Architecture == OSArch.I386)
                {
                    if (nph32Exists)
                    {
                        if (nphExists)
                            System.IO.File.Move(startupPath + "\\NProcessHacker.dll", startupPath + "\\NProcessHacker64.dll");

                        System.IO.File.Move(startupPath + "\\NProcessHacker32.dll", startupPath + "\\NProcessHacker.dll");
                    }
                }
                // If we're on 64-bit and NPH64 exists, rename NPH to NPH32 and 
                // NPH64 to NPH.
                else if (OSVersion.Architecture == OSArch.Amd64)
                {
                    if (nph64Exists)
                    {
                        if (nphExists)
                            System.IO.File.Move(startupPath + "\\NProcessHacker.dll", startupPath + "\\NProcessHacker32.dll");

                        System.IO.File.Move(startupPath + "\\NProcessHacker64.dll", startupPath + "\\NProcessHacker.dll");
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        private void LoadUac()
        {
            if (Program.ElevationType == TokenElevationType.Limited)
            {
                this.showDetailsForAllProcessesMenuItem.Image = uacShieldIcon = this.GetUacShieldIcon();
                //vistaMenu.SetImage(showDetailsForAllProcessesMenuItem, uacShieldIcon);
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

        private void LoadNotificationIcons()
        {
            using (Bitmap b = new Bitmap(16, 16))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.FillRectangle(new SolidBrush(Color.Black), 0, 0, b.Width, b.Height);
                    blackIcon = Icon.FromHandle(b.GetHicon());
                }
            }

            dummyIcon = new UsageIcon();
            notifyIcons.Add(cpuHistoryIcon = new CpuHistoryIcon() { Parent = this });
            notifyIcons.Add(cpuUsageIcon = new CpuUsageIcon() { Parent = this });
            notifyIcons.Add(ioHistoryIcon = new IoHistoryIcon() { Parent = this });
            notifyIcons.Add(commitHistoryIcon = new CommitHistoryIcon() { Parent = this });
            notifyIcons.Add(physMemHistoryIcon = new PhysMemHistoryIcon() { Parent = this });

            foreach (var icon in notifyIcons)
                icon.Icon = (Icon)blackIcon.Clone();

            this.ExecuteOnIcons(icon => icon.ContextMenu = contextMenuStripTray);
            this.ExecuteOnIcons(icon => icon.MouseDoubleClick += notifyIcon_MouseDoubleClick);

            this.cpuHistoryMenuItem.Checked = Settings.Instance.CpuHistoryIconVisible;
            this.cpuUsageMenuItem.Checked = Settings.Instance.CpuUsageIconVisible;
            this.ioHistoryMenuItem.Checked = Settings.Instance.IoHistoryIconVisible;
            this.commitHistoryMenuItem.Checked = Settings.Instance.CommitHistoryIconVisible;
            this.physMemHistoryMenuItem.Checked = Settings.Instance.PhysMemHistoryIconVisible;
            this.ApplyIconVisibilities();

            this.newProcessesToolStripMenuItem.Checked = Settings.Instance.NewProcesses;
            this.terminatedProcessesToolStripMenuItem.Checked = Settings.Instance.TerminatedProcesses;
            this.newServicesToolStripMenuItem.Checked = Settings.Instance.NewServices;
            this.startedServicesToolStripMenuItem.Checked = Settings.Instance.StartedServices;
            this.stoppedServicesToolStripMenuItem.Checked = Settings.Instance.StoppedServices;
            this.deletedServicesToolStripMenuItem.Checked = Settings.Instance.DeletedServices;

            this.newProcessesToolStripMenuItem.Click += this.CheckedMenuItem_Click;
            this.terminatedProcessesToolStripMenuItem.Click += this.CheckedMenuItem_Click;
            this.newServicesToolStripMenuItem.Click += this.CheckedMenuItem_Click;
            this.startedServicesToolStripMenuItem.Click += this.CheckedMenuItem_Click;
            this.stoppedServicesToolStripMenuItem.Click += this.CheckedMenuItem_Click;
            this.deletedServicesToolStripMenuItem.Click += this.CheckedMenuItem_Click;
        }

        private void LoadControls()
        {
            networkInfomationMenuItem.Visible = false; // not ready
            analyzeWaitChainToolStripMenuItem.Visible = false; // not ready

            //GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            //GenericViewMenu.AddMenuItems(copyToolStripMenuItem1.MenuItems, listServices.List, null);
            //GenericViewMenu.AddMenuItems(copyNetworkMenuItem.MenuItems, listNetwork.List, null);

            treeProcesses.Provider = Program.ProcessProvider;
            treeProcesses.Tree.BeginUpdate();
            treeProcesses.Tree.BeginCompleteUpdate();
            this.Cursor = Cursors.WaitCursor;
            Program.ProcessProvider.Updated += processP_Updated;
            Program.ProcessProvider.Updated += processP_InfoUpdater;
            if (Program.InspectPid != -1) Program.ProcessProvider.ProcessQueryReceived += processP_FileProcessingReceived;
            Program.ProcessProvider.Enabled = true;
            Program.ProcessProvider.Boost();
            updateProcessesMenuItem.Checked = true;

            HighlightingContext.HighlightingDuration = Settings.Instance.HighlightingDuration;
            HighlightingContext.StateHighlighting = false;

            listServices.List.BeginUpdate();
            listServices.Provider = Program.ServiceProvider;
            Program.ServiceProvider.DictionaryAdded += serviceP_DictionaryAdded_Process;
            Program.ServiceProvider.DictionaryModified += serviceP_DictionaryModified_Process;
            Program.ServiceProvider.DictionaryRemoved += serviceP_DictionaryRemoved_Process;
            Program.ServiceProvider.Updated += serviceP_Updated;
            updateServicesMenuItem.Checked = true;

            if (OSVersion.IsAboveOrEqual(WindowsVersion.XP))
            {
                listNetwork.Provider = Program.NetworkProvider;
            }

            treeProcesses.Tree.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right && e.Location.Y < treeProcesses.Tree.ColumnHeaderHeight)
                {
                    ContextMenu menu = new ContextMenu();

                    menu.MenuItems.Add(new MenuItem("Choose Columns...", (sender_, e_) =>
                    {
                        using (var c = new ChooseColumnsWindow(treeProcesses.Tree))
                        {
                            c.ShowDialog();
                        }

                        copyProcessMenuItem.DropDownItems.Clear();
                        //GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
                        treeProcesses.Tree.InvalidateNodeControlCache();
                        treeProcesses.Tree.Invalidate();
                    }));

                    menu.Show(treeProcesses.Tree, e.Location);
                }
            };

            treeProcesses.Tree.ColumnClicked += (sender, e) => this.DeselectAll(this.treeProcesses.Tree);
            treeProcesses.Tree.ColumnReordered += (sender, e) =>
            {
                copyProcessMenuItem.DropDownItems.Clear();
               // GenericViewMenu.AddMenuItems(copyProcessMenuItem.MenuItems, treeProcesses.Tree);
            };

            tabControlBig_SelectedIndexChanged(null, null);
        }

        private void LoadAddShortcuts()
        {
            treeProcesses.Tree.KeyDown += (sender, e) =>
            {
                if (e.Control && e.KeyCode == Keys.A)
                {
                    treeProcesses.TreeNodes.SelectAll();
                    treeProcesses.Tree.Invalidate();
                }

                if (e.Control && e.KeyCode == Keys.C) GenericViewMenu.TreeViewAdvCopy(treeProcesses.Tree, -1);
            };

            listServices.List.AddShortcuts();
            listNetwork.List.AddShortcuts();
        }

        private void LoadApplyCommandLineArgs()
        {
            tabControl.SelectedTab = tabControl.TabPages["tab" + Program.SelectTab];
        }

        private void LoadStructs()
        {
            WorkQueue.GlobalQueueWorkItemTag(new MethodInvoker(() =>
            {
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
            }), "load-structs");
        }

        private void LoadOther()
        {
            try
            {
                using (TokenHandle thandle = ProcessHandle.Current.GetToken(TokenAccess.Query))
                using (Sid sid = thandle.User)
                    this.Text += " [" + sid.GetFullName(true) + "]" + (KProcessHacker2.Instance != null ? "+" : string.Empty);
            }
            catch
            { }

            // If it's Vista or above and we're elevated.
            if (OSVersion.HasUac && Program.ElevationType == TokenElevationType.Full)
            {
                this.Text += " (Administrator)";
                // We enable the magic window message to Allow only one Application instance to work.
                Win32.ChangeWindowMessageFilter((WindowMessage)0x9991, UipiFilterFlag.Add);
            } 
        }

        public HackerWindow()
        {
            InitializeComponent();

            if (OSVersion.HasExtendedTaskbar)
            {
                // We need to call this here or we don't receive the TaskbarButtonCreated message
                Windows7Taskbar.AllowWindowMessagesThroughUipi();
                Windows7Taskbar.AppId = "ProcessHacker";
                Windows7Taskbar.ProcessAppId = "ProcessHacker";

                thumbButtonManager = new ThumbButtonManager(this);
                thumbButtonManager.TaskbarButtonCreated += this.thumbButtonManager_TaskbarButtonCreated;
            }

            this.AddEscapeToClose();

            // Initialize action syncs
            _enableNetworkProviderSync = new ActionSync(
                () =>
                {
                    Program.NetworkProvider.Enabled = true;
                    Program.NetworkProvider.Boost();
                }, 2);

            _refreshHighlightingSync = new ActionSync(() => this.BeginInvoke(new MethodInvoker(this.treeProcesses.RefreshItems), null), 2);

            Logging.Logged += this.QueueMessage;
            this.LoadWindowSettings();
            this.LoadOtherSettings();
            this.LoadControls();
            this.LoadNotificationIcons();

            if ((!Settings.Instance.StartHidden && !Program.StartHidden) || Program.StartVisible)
            {
                this.Visible = true;
            }

            if (tabControl.SelectedTab == tabProcesses)
                treeProcesses.Tree.Select();

            this.LoadOther();
            this.LoadStructs();

            Program.ServiceProvider.Enabled = true;
            Program.ServiceProvider.Boost();

            ProcessHackerRestartRecovery.ApplicationRestartRecoveryManager.RegisterForRestart();
            //ProcessHackerRestartRecovery.ApplicationRestartRecoveryManager.RegisterForRecovery();

            this.CreateShutdownMenuItems();
            this.LoadFixOSSpecific();
            this.LoadUac();
            this.LoadAddShortcuts();
            this.LoadFixNProcessHacker();

            toolStrip.Items.Add(new ToolStripSeparator());
            var targetButton = new TargetWindowButton();
            targetButton.TargetWindowFound += (pid, tid) => this.SelectProcess(pid);
            toolStrip.Items.Add(targetButton);

            var targetThreadButton = new TargetWindowButton();
            targetThreadButton.TargetWindowFound += (pid, tid) => Program.GetProcessWindow(Program.ProcessProvider.Dictionary[pid], f =>
            {
                Program.FocusWindow(f);
                f.SelectThread(tid);
            });
            targetThreadButton.Image = Properties.Resources.application_go;
            targetThreadButton.Text = "Find window and select thread";
            targetThreadButton.ToolTipText = "Find window and select thread";
            toolStrip.Items.Add(targetThreadButton);

            try { TerminalServerHandle.RegisterNotificationsCurrent(this, true); }
            catch (Exception ex) { Logging.Log(ex); }
            try { this.UpdateSessions(); }
            catch (Exception ex) { Logging.Log(ex); }

            try { Win32.SetProcessShutdownParameters(0x100, 0); }
            catch { }

            if (Settings.Instance.AppUpdateAutomatic)
                this.UpdateProgram(false);

            ToolStripManager.Renderer = new AeroRenderer(ToolbarTheme.Blue);
        }

        private void HackerWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindowMenu(windowToolStripMenuItem, this);
            this.ApplyFont(Settings.Instance.Font);
            this.BeginInvoke(new MethodInvoker(this.LoadApplyCommandLineArgs));
        }

        private void HackerWindow_VisibleChanged(object sender, EventArgs e)
        {
            treeProcesses.Draw = this.Visible;
        }

        private void contextMenuStripService_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (this.listServices.SelectedItems.Count)
            {
                case 0:
                    this.goToProcessServiceMenuItem.Visible = true;
                    this.startToolStripMenuItem.Visible = true;
                    this.continueToolStripMenuItem.Visible = true;
                    this.pauseToolStripMenuItem.Visible = true;
                    this.stopToolStripMenuItem.Visible = true;
                    this.selectAllServiceMenuItem.Enabled = true;
                    break;
                case 1:
                    this.goToProcessServiceMenuItem.Visible = true;
                    this.startToolStripMenuItem.Visible = true;
                    this.continueToolStripMenuItem.Visible = true;
                    this.pauseToolStripMenuItem.Visible = true;
                    this.stopToolStripMenuItem.Visible = true;
                    try
                    {
                        ServiceItem item = Program.ServiceProvider.Dictionary[this.listServices.SelectedItems[0].Name];

                        if (item.Status.ServiceStatusProcess.ProcessID != 0)
                        {
                            this.goToProcessServiceMenuItem.Enabled = true;
                        }
                        else
                        {
                            this.goToProcessServiceMenuItem.Enabled = false;
                        }

                        if ((item.Status.ServiceStatusProcess.ControlsAccepted & ServiceAccept.PauseContinue) == 0)
                        {
                            this.continueToolStripMenuItem.Visible = false;
                            this.pauseToolStripMenuItem.Visible = false;
                        }
                        else
                        {
                            this.continueToolStripMenuItem.Visible = true;
                            this.pauseToolStripMenuItem.Visible = true;
                        }

                        switch (item.Status.ServiceStatusProcess.CurrentState)
                        {
                            case ServiceState.Paused:
                                this.startToolStripMenuItem.Enabled = false;
                                this.pauseToolStripMenuItem.Enabled = false;
                                break;
                            case ServiceState.Running:
                                this.startToolStripMenuItem.Enabled = false;
                                this.continueToolStripMenuItem.Enabled = false;
                                break;
                            case ServiceState.Stopped:
                                this.pauseToolStripMenuItem.Enabled = false;
                                this.stopToolStripMenuItem.Enabled = false;
                                break;
                        }

                        if ((item.Status.ServiceStatusProcess.ControlsAccepted & ServiceAccept.Stop) == 0 &&
                            item.Status.ServiceStatusProcess.CurrentState == ServiceState.Running)
                        {
                            this.stopToolStripMenuItem.Enabled = false;
                        }
                    }
                    catch
                    {
                        //contextMenuStripService.DisableAll();
                        this.copyToolStripMenuItem1.Enabled = true;
                        this.propertiesToolStripMenuItem.Enabled = true;
                    }
                    break;
                default:
                    this.goToProcessServiceMenuItem.Visible = false;
                    this.startToolStripMenuItem.Visible = false;
                    this.continueToolStripMenuItem.Visible = false;
                    this.pauseToolStripMenuItem.Visible = false;
                    this.stopToolStripMenuItem.Visible = false;
                    this.copyToolStripMenuItem1.Enabled = true;
                    this.propertiesToolStripMenuItem.Enabled = true;
                    this.selectAllServiceMenuItem.Enabled = true;
                    break;
            }

            if (listServices.List.Items.Count == 0)
                selectAllServiceMenuItem.Enabled = false;
        }
    }
}
