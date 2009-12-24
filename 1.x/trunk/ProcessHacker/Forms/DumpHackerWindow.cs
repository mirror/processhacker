/*
 * Process Hacker - 
 *   dump viewer
 * 
 * Copyright (C) 2009 wj32
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
using ProcessHacker.Native.Mfs;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class DumpHackerWindow : Form
    {
        private MemoryFileSystem _mfs;
        private MemoryObject _processesMo;
        private MemoryObject _servicesMo;
        private string _phVersion;
        private string _osVersion;
        private OSArch _architecture;
        private string _userName;
        private Dictionary<int, ProcessItem> _processes = new Dictionary<int, ProcessItem>();
        private Dictionary<string, ServiceItem> _services = new Dictionary<string, ServiceItem>();
        private Dictionary<int, List<string>> _processServices = new Dictionary<int, List<string>>();

        public DumpHackerWindow(string fileName)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            _mfs = new MemoryFileSystem(fileName, MfsOpenMode.Open, true);

            ColumnSettings.LoadSettings(Settings.Instance.ProcessTreeColumns, treeProcesses.Tree);
            ColumnSettings.LoadSettings(Settings.Instance.ServiceListViewColumns, listServices.List);

            listServices.DoubleClick += new EventHandler(listServices_DoubleClick);
        }

        private void DumpHackerWindow_Load(object sender, EventArgs e)
        {
            treeProcesses.DumpMode = true;
            treeProcesses.DumpProcesses = _processes;
            treeProcesses.DumpProcessServices = _processServices;
            treeProcesses.DumpServices = _services;

            GenericViewMenu.AddMenuItems(copyMenuItem.MenuItems, treeProcesses.Tree);
            treeProcesses.Tree.ContextMenu = menuProcess;

            GenericViewMenu.AddMenuItems(copyServiceMenuItem.MenuItems, listServices.List, null);
            listServices.List.ContextMenu = menuService;

            this.LoadSystemInformation();
            this.LoadProcesses();
            this.LoadServices();

            treeProcesses.UpdateItems();
            listServices.UpdateItems();
        }

        private void DumpHackerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_processesMo != null)
                _processesMo.Dispose();
            if (_servicesMo != null)
                _servicesMo.Dispose();
            _mfs.Dispose();

            foreach (var item in _processes.Values)
            {
                if (item.Icon != null)
                    Win32.DestroyIcon(item.Icon.Handle);
            }
        }

        public OSArch Architecture
        {
            get { return _architecture; }
        }

        public Dictionary<int, ProcessItem> Processes
        {
            get { return _processes; }
        }

        private void LoadSystemInformation()
        {
            MemoryObject sysInfoMo;

            sysInfoMo = _mfs.RootObject.GetChild("SystemInformation");

            if (sysInfoMo == null)
            {
                PhUtils.ShowWarning("The dump file does not contain system information. This most likely " +
                    "means the file is corrupt.");
                return;
            }

            var dict = Dump.GetDictionary(sysInfoMo);

            sysInfoMo.Dispose();

            _phVersion = dict["ProcessHackerVersion"];
            _osVersion = dict["OSVersion"];
            _architecture = (OSArch)Dump.ParseInt32(dict["Architecture"]);
            _userName = dict["UserName"];

            treeProcesses.DumpUserName = _userName;

            this.Text = "Process Hacker " + _phVersion +
                " [" + _userName + "] (" + _osVersion + ", " +
                (_architecture == OSArch.I386 ? "32-bit" : "64-bit") +
                ")";
        }

        private void LoadProcesses()
        {
            MemoryObject processesMo;

            processesMo = _mfs.RootObject.GetChild("Processes");
            _processesMo = processesMo;

            if (processesMo == null)
            {
                PhUtils.ShowWarning("The dump file does not contain process information. This most likely " +
                    "means the file is corrupt.");
                return;
            }

            processesMo.EnumChildren((childMo) =>
                {
                    using (childMo)
                        this.LoadProcess(childMo);

                    return true;
                });
        }

        private void LoadProcess(MemoryObject mo)
        {
            var names = mo.GetChildNames();
            ProcessItem pitem;

            if (!names.Contains("General"))
                return;

            IDictionary<string, string> generalDict;

            using (var general = mo.GetChild("General"))
                generalDict = Dump.GetDictionary(general);

            pitem = new ProcessItem();
            pitem.Pid = Dump.ParseInt32(generalDict["ProcessId"]);
            pitem.Name = generalDict["Name"];
            pitem.ParentPid = Dump.ParseInt32(generalDict["ParentPid"]);

            if (generalDict.ContainsKey("HasParent"))
                pitem.HasParent = Dump.ParseBool(generalDict["HasParent"]);
            if (generalDict.ContainsKey("StartTime"))
                pitem.CreateTime = Dump.ParseDateTime(generalDict["StartTime"]);
            if (generalDict.ContainsKey("SessionId"))
                pitem.SessionId = Dump.ParseInt32(generalDict["SessionId"]);

            if (generalDict.ContainsKey("FileName"))
                pitem.FileName = generalDict["FileName"];

            if (generalDict.ContainsKey("FileDescription"))
            {
                pitem.VersionInfo = new ImageVersionInfo();
                pitem.VersionInfo.FileDescription = generalDict["FileDescription"];
                pitem.VersionInfo.CompanyName = generalDict["FileCompanyName"];
                pitem.VersionInfo.FileVersion = generalDict["FileVersion"];
                pitem.VersionInfo.FileName = pitem.FileName;
            }

            if (generalDict.ContainsKey("CommandLine"))
                pitem.CmdLine = generalDict["CommandLine"];
            if (generalDict.ContainsKey("IsPosix"))
                pitem.IsPosix = Dump.ParseBool(generalDict["IsPosix"]);
            if (generalDict.ContainsKey("IsWow64"))
                pitem.IsWow64 = Dump.ParseBool(generalDict["IsWow64"]);
            if (generalDict.ContainsKey("IsBeingDebugged"))
                pitem.IsBeingDebugged = Dump.ParseBool(generalDict["IsBeingDebugged"]);
            if (generalDict.ContainsKey("UserName"))
                pitem.Username = generalDict["UserName"];
            if (generalDict.ContainsKey("ElevationType"))
                pitem.ElevationType = (TokenElevationType)Dump.ParseInt32(generalDict["ElevationType"]);

            if (generalDict.ContainsKey("CpuUsage"))
                pitem.CpuUsage = float.Parse(generalDict["CpuUsage"]);
            if (generalDict.ContainsKey("JobName"))
                pitem.JobName = generalDict["JobName"];
            if (generalDict.ContainsKey("IsInJob"))
                pitem.IsInJob = Dump.ParseBool(generalDict["IsInJob"]);
            if (generalDict.ContainsKey("IsInSignificantJob"))
                pitem.IsInSignificantJob = Dump.ParseBool(generalDict["IsInSignificantJob"]);
            if (generalDict.ContainsKey("Integrity"))
                pitem.Integrity = generalDict["Integrity"];
            if (generalDict.ContainsKey("IntegrityLevel"))
                pitem.IntegrityLevel = Dump.ParseInt32(generalDict["IntegrityLevel"]);
            if (generalDict.ContainsKey("IsDotNet"))
                pitem.IsDotNet = Dump.ParseBool(generalDict["IsDotNet"]);
            if (generalDict.ContainsKey("IsPacked"))
                pitem.IsPacked = Dump.ParseBool(generalDict["IsPacked"]);
            if (generalDict.ContainsKey("VerifyResult"))
                pitem.VerifyResult = (VerifyResult)Dump.ParseInt32(generalDict["VerifyResult"]);
            if (generalDict.ContainsKey("VerifySignerName"))
                pitem.VerifySignerName = generalDict["VerifySignerName"];
            if (generalDict.ContainsKey("ImportFunctions"))
                pitem.ImportFunctions = Dump.ParseInt32(generalDict["ImportFunctions"]);
            if (generalDict.ContainsKey("ImportModules"))
                pitem.ImportModules = Dump.ParseInt32(generalDict["ImportModules"]);

            if (names.Contains("SmallIcon"))
            {
                using (var smallIcon = mo.GetChild("SmallIcon"))
                    pitem.Icon = Dump.GetIcon(smallIcon);
            }

            if (names.Contains("VmCounters"))
            {
                using (var vmCounters = mo.GetChild("VmCounters"))
                    pitem.Process.VirtualMemoryCounters = Dump.GetStruct<VmCountersEx64>(vmCounters).ToVmCountersEx();
            }

            if (names.Contains("IoCounters"))
            {
                using (var ioCounters = mo.GetChild("IoCounters"))
                    pitem.Process.IoCounters = Dump.GetStruct<IoCounters>(ioCounters);
            }

            _processes.Add(pitem.Pid, pitem);
            treeProcesses.AddItem(pitem);
        }

        private void LoadServices()
        {
            MemoryObject servicesMo;

            servicesMo = _mfs.RootObject.GetChild("Services");
            _servicesMo = servicesMo;

            if (servicesMo == null)
            {
                PhUtils.ShowWarning("The dump file does not contain service information. This most likely " +
                    "means the file is corrupt.");
                return;
            }

            servicesMo.EnumChildren((childMo) =>
            {
                using (childMo)
                    this.LoadService(childMo);

                return true;
            });
        }

        private void LoadService(MemoryObject mo)
        {
            ServiceItem item = new ServiceItem();
            var dict = Dump.GetDictionary(mo);

            item.Status.ServiceName = dict["Name"];
            item.Status.DisplayName = dict["DisplayName"];
            item.Status.ServiceStatusProcess.ControlsAccepted = (ServiceAccept)Dump.ParseInt32(dict["ControlsAccepted"]);
            item.Status.ServiceStatusProcess.CurrentState = (ServiceState)Dump.ParseInt32(dict["State"]);
            item.Status.ServiceStatusProcess.ProcessID = Dump.ParseInt32(dict["ProcessId"]);
            item.Status.ServiceStatusProcess.ServiceFlags = (ServiceFlags)Dump.ParseInt32(dict["Flags"]);
            item.Status.ServiceStatusProcess.ServiceType = (ServiceType)Dump.ParseInt32(dict["Type"]);

            if (dict.ContainsKey("BinaryPath"))
            {
                item.Config.BinaryPathName = dict["BinaryPath"];
                item.Config.DisplayName = item.Status.DisplayName;
                item.Config.ErrorControl = (ServiceErrorControl)Dump.ParseInt32(dict["ErrorControl"]);
                item.Config.LoadOrderGroup = dict["Group"];
                item.Config.ServiceStartName = dict["UserName"];
                item.Config.ServiceType = item.Status.ServiceStatusProcess.ServiceType;
                item.Config.StartType = (ServiceStartType)Dump.ParseInt32(dict["StartType"]);

                if (dict.ContainsKey("ServiceDll"))
                    item.ServiceDll = dict["ServiceDll"];
            }

            _services.Add(item.Status.ServiceName, item);
            listServices.AddItem(item);

            if (item.Status.ServiceStatusProcess.ProcessID != 0)
            {
                if (!_processServices.ContainsKey(item.Status.ServiceStatusProcess.ProcessID))
                    _processServices.Add(item.Status.ServiceStatusProcess.ProcessID, new List<string>());

                _processServices[item.Status.ServiceStatusProcess.ProcessID].Add(item.Status.ServiceName);
            }
        }

        public void ShowProperties(ProcessItem item)
        {
            DumpProcessWindow dpw = new DumpProcessWindow(
                this,
                item,
                _processesMo.GetChild(item.Pid.ToString("x"))
                );

            dpw.Show();
        }

        public void ShowProperties(IWin32Window owner, ServiceItem item)
        {
            DumpServiceWindow dsw = new DumpServiceWindow(item, _servicesMo.GetChild(item.Status.ServiceName));

            dsw.ShowDialog(owner);
        }

        private void SelectProcess(int pid)
        {
            foreach (var node in treeProcesses.Tree.AllNodes)
                node.IsSelected = false;

            try
            {
                var node = treeProcesses.FindTreeNode(pid);

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

        private void treeProcesses_NodeMouseDoubleClick(object sender, Aga.Controls.Tree.TreeNodeAdvMouseEventArgs e)
        {
            var pNode = treeProcesses.FindNode(e.Node);

            this.ShowProperties(pNode.ProcessItem);
        }

        private void menuProcess_Popup(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedTreeNodes.Count == 0)
            {
                menuProcess.DisableAll();
            }
            else if (treeProcesses.SelectedTreeNodes.Count == 1)
            {
                menuProcess.EnableAll();
            }
            else
            {
                menuProcess.EnableAll();
                propertiesMenuItem.Enabled = false;
            }
        }

        private void propertiesMenuItem_Click(object sender, EventArgs e)
        {
            var pNode = treeProcesses.SelectedNodes[0];

            this.ShowProperties(pNode.ProcessItem);
        }

        private void listServices_DoubleClick(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count != 1)
                return;

            propertiesServiceMenuItem_Click(sender, e);
        }

        private void menuService_Popup(object sender, EventArgs e)
        {
            if (listServices.SelectedItems.Count == 0)
            {
                menuService.DisableAll();
            }
            else if (listServices.SelectedItems.Count == 1)
            {
                menuService.EnableAll();

                if (_services[listServices.SelectedItems[0].Text].Status.ServiceStatusProcess.ProcessID == 0)
                    goToProcessServiceMenuItem.Enabled = false;
            }
            else
            {
                menuService.EnableAll();
                goToProcessServiceMenuItem.Enabled = false;
                propertiesServiceMenuItem.Enabled = false;
            }
        }

        private void goToProcessServiceMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectProcess(_services[listServices.SelectedItems[0].Text].Status.ServiceStatusProcess.ProcessID);
        }

        private void propertiesServiceMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowProperties(this, _services[listServices.SelectedItems[0].Text]);
        }
    }
}
