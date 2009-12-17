using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
        private string _phVersion;
        private string _osVersion;
        private string _userName;
        private Dictionary<int, ProcessItem> _processes = new Dictionary<int, ProcessItem>();
        private Dictionary<string, ServiceItem> _services = new Dictionary<string, ServiceItem>();
        private Dictionary<int, List<string>> _processServices = new Dictionary<int, List<string>>();

        public DumpHackerWindow(string fileName)
        {
            InitializeComponent();

            _mfs = new MemoryFileSystem(fileName, MfsOpenMode.Open, true);

            ColumnSettings.LoadSettings(Settings.Instance.ProcessTreeColumns, treeProcesses.Tree);
        }

        private void DumpHackerWindow_Load(object sender, EventArgs e)
        {
            treeProcesses.DumpMode = true;
            treeProcesses.DumpProcesses = _processes;
            treeProcesses.DumpProcessServices = _processServices;
            treeProcesses.DumpServices = _services;

            this.LoadSystemInformation();
            this.LoadProcesses();
            this.LoadServices();

            treeProcesses.UpdateItems();
            listServices.UpdateItems();
        }

        private void DumpHackerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mfs.Dispose();
        }

        private bool ParseBool(string str)
        {
            return str != "0";
        }

        private int ParseInt32(string str)
        {
            return int.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private long ParseInt64(string str)
        {
            return long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private DateTime ParseDateTime(string str)
        {
            return DateTime.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
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
            _userName = dict["UserName"];

            treeProcesses.DumpUserName = _userName;

            this.Text = "Process Hacker " + _phVersion + " [" + _userName + "] (" + _osVersion + ")";
        }

        private void LoadProcesses()
        {
            MemoryObject processesMo;

            processesMo = _mfs.RootObject.GetChild("Processes");

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

            processesMo.Dispose();
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
            pitem.Pid = ParseInt32(generalDict["ProcessId"]);
            pitem.Name = generalDict["Name"];
            pitem.CreateTime = ParseDateTime(generalDict["StartTime"]);
            pitem.HasParent = ParseBool(generalDict["HasParent"]);
            pitem.ParentPid = ParseInt32(generalDict["ParentPid"]);
            pitem.SessionId = ParseInt32(generalDict["SessionId"]);

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
                pitem.IsPosix = ParseBool(generalDict["IsPosix"]);
            if (generalDict.ContainsKey("IsWow64"))
                pitem.IsWow64 = ParseBool(generalDict["IsWow64"]);
            if (generalDict.ContainsKey("IsBeingDebugged"))
                pitem.IsBeingDebugged = ParseBool(generalDict["IsBeingDebugged"]);
            if (generalDict.ContainsKey("UserName"))
                pitem.Username = generalDict["UserName"];
            if (generalDict.ContainsKey("ElevationType"))
                pitem.ElevationType = (TokenElevationType)ParseInt32(generalDict["ElevationType"]);

            if (generalDict.ContainsKey("CpuUsage"))
                pitem.CpuUsage = float.Parse(generalDict["CpuUsage"]);
            if (generalDict.ContainsKey("JobName"))
                pitem.JobName = generalDict["JobName"];
            if (generalDict.ContainsKey("IsInJob"))
                pitem.IsInJob = ParseBool(generalDict["IsInJob"]);
            if (generalDict.ContainsKey("IsInSignificantJob"))
                pitem.IsInSignificantJob = ParseBool(generalDict["IsInSignificantJob"]);
            if (generalDict.ContainsKey("Integrity"))
                pitem.Integrity = generalDict["Integrity"];
            if (generalDict.ContainsKey("IntegrityLevel"))
                pitem.IntegrityLevel = ParseInt32(generalDict["IntegrityLevel"]);
            if (generalDict.ContainsKey("IsDotNet"))
                pitem.IsDotNet = ParseBool(generalDict["IsDotNet"]);
            if (generalDict.ContainsKey("IsPacked"))
                pitem.IsPacked = ParseBool(generalDict["IsPacked"]);
            if (generalDict.ContainsKey("VerifyResult"))
                pitem.VerifyResult = (VerifyResult)ParseInt32(generalDict["VerifyResult"]);
            if (generalDict.ContainsKey("VerifySignerName"))
                pitem.VerifySignerName = generalDict["VerifySignerName"];
            if (generalDict.ContainsKey("ImportFunctions"))
                pitem.ImportFunctions = ParseInt32(generalDict["ImportFunctions"]);
            if (generalDict.ContainsKey("ImportModules"))
                pitem.ImportModules = ParseInt32(generalDict["ImportModules"]);

            if (names.Contains("SmallIcon"))
            {
                using (var smallIcon = mo.GetChild("SmallIcon"))
                    pitem.Icon = Dump.GetIcon(smallIcon);
            }

            using (var vmCounters = mo.GetChild("VmCounters"))
                pitem.Process.VirtualMemoryCounters = Dump.GetStruct<VmCountersEx64>(vmCounters).ToVmCountersEx();
            using (var ioCounters = mo.GetChild("IoCounters"))
                pitem.Process.IoCounters = Dump.GetStruct<IoCounters>(ioCounters);

            _processes.Add(pitem.Pid, pitem);
            treeProcesses.AddItem(pitem);
        }

        private void LoadServices()
        {
            MemoryObject servicesMo;

            servicesMo = _mfs.RootObject.GetChild("Services");

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

            servicesMo.Dispose();
        }

        private void LoadService(MemoryObject mo)
        {
            ServiceItem item = new ServiceItem();
            var dict = Dump.GetDictionary(mo);

            item.Status.ServiceName = dict["Name"];
            item.Status.DisplayName = dict["DisplayName"];
            item.Status.ServiceStatusProcess.ControlsAccepted = (ServiceAccept)ParseInt32(dict["ControlsAccepted"]);
            item.Status.ServiceStatusProcess.CurrentState = (ServiceState)ParseInt32(dict["State"]);
            item.Status.ServiceStatusProcess.ProcessID = ParseInt32(dict["ProcessId"]);
            item.Status.ServiceStatusProcess.ServiceFlags = (ServiceFlags)ParseInt32(dict["Flags"]);
            item.Status.ServiceStatusProcess.ServiceType = (ServiceType)ParseInt32(dict["Type"]);

            if (dict.ContainsKey("BinaryPath"))
            {
                item.Config.BinaryPathName = dict["BinaryPath"];
                item.Config.DisplayName = item.Status.DisplayName;
                item.Config.ErrorControl = (ServiceErrorControl)ParseInt32(dict["ErrorControl"]);
                item.Config.LoadOrderGroup = dict["Group"];
                item.Config.ServiceStartName = dict["UserName"];
                item.Config.ServiceType = item.Status.ServiceStatusProcess.ServiceType;
                item.Config.StartType = (ServiceStartType)ParseInt32(dict["StartType"]);
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
    }
}
