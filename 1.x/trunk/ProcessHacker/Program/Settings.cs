using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using ProcessHacker.Common;
using ProcessHacker.Common.Settings;
using System.Windows.Forms;

namespace ProcessHacker
{
    public class Settings : SettingsBase
    {
        // Create instance, before the user creates a new store.
        private static Settings _instance = new Settings();
        private static ISettingsStore _store;

        public static Settings Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        private Settings()
            : base(_store = new VolatileSettingsStore())
        { }

        public Settings(string fileName)
            : base(_store = new XmlFileSettingsStore(fileName))
        {
            this.Invalidate();
        }

        public string SettingsFileName
        {
            get { return (_store as XmlFileSettingsStore).FileName; }
        }

        public override void Invalidate()
        {
            _colorDebuggedProcesses = null;
            _colorDotNetProcesses = null;
            _colorElevatedProcesses = null;
            _colorGuiThreads = null;
            _colorInheritHandles = null;
            _colorJobProcesses = null;
            _colorNew = null;
            _colorOwnProcesses = null;
            _colorPackedProcesses = null;
            _colorPosixProcesses = null;
            _colorProtectedHandles = null;
            _colorRelocatedDlls = null;
            _colorRemoved = null;
            _colorServiceProcesses = null;
            _colorSuspended = null;
            _colorSystemProcesses = null;
            _colorWow64Processes = null;
            _deletedServices = null;
            _elevationLevel = (int)this["ElevationLevel"];
            _floatChildWindows = null;
            _font = null;
            _hideWhenClosed = null;
            _hideWhenMinimized = null;
            _highlightingDuration = (int)this["HighlightingDuration"];
            _iconMenuProcessCount = (int)this["IconMenuProcessCount"];
            _maxSamples = (int)this["MaxSamples"];
            _newProcesses = null;
            _newServices = null;
            _plotterAntialias = (bool)this["PlotterAntialias"];
            _plotterCPUKernelColor = null;
            _plotterCPUUserColor = null;
            _plotterIOROColor = null;
            _plotterIOWColor = null;
            _plotterMemoryPrivateColor = null;
            _plotterMemoryWSColor = null;
            _plotterStep = (int)this["PlotterStep"];
            _refreshInterval = (int)this["RefreshInterval"];
            _showAccountDomains = (bool)this["ShowAccountDomains"];
            _showOneGraphPerCPU = null;
            _startedServices = null;
            _stoppedServices = null;
            _terminatedProcesses = null;
            _unitSpecifier = (int)this["UnitSpecifier"];
            _useColorDebuggedProcesses = null;
            _useColorDotNetProcesses = null;
            _useColorElevatedProcesses = null;
            _useColorGuiThreads = null;
            _useColorInheritHandles = null;
            _useColorJobProcesses = null;
            _useColorOwnProcesses = null;
            _useColorPackedProcesses = null;
            _useColorPosixProcesses = null;
            _useColorProtectedHandles = null;
            _useColorRelocatedDlls = null;
            _useColorServiceProcesses = null;
            _useColorSuspended = null;
            _useColorSystemProcesses = null;
            _useColorWow64Processes = null;
            _verifySignatures = null;
            _warnDangerous = (bool)this["WarnDangerous"];
        }

        [SettingDefault("False")]
        public bool AllowOnlyOneInstance
        {
            get { return (bool)this["AllowOnlyOneInstance"]; }
            set { this["AllowOnlyOneInstance"] = value; }
        }

        [SettingDefault("False")]
        public bool AlwaysOnTop
        {
            get { return (bool)this["AlwaysOnTop"]; }
            set { this["AlwaysOnTop"] = value; }
        }

        [SettingDefault("False")]
        public bool AppUpdateAutomatic
        {
            get { return (bool)this["AppUpdateAutomatic"]; }
            set { this["AppUpdateAutomatic"] = value; }
        }

        [SettingDefault("0")]
        public int AppUpdateLevel
        {
            get { return (int)this["AppUpdateLevel"]; }
            set { this["AppUpdateLevel"] = value; }
        }

        [SettingDefault("http://processhacker.sourceforge.net/AppUpdate.xml")]
        public string AppUpdateUrl
        {
            get { return (string)this["AppUpdateUrl"]; }
            set { this["AppUpdateUrl"] = value; }
        }

        [SettingDefault("")]
        public string CallStackColumns
        {
            get { return (string)this["CallStackColumns"]; }
            set { this["CallStackColumns"] = value; }
        }

        private Color? _colorDebuggedProcesses;
        [SettingDefault("204, 187, 255")]
        public Color ColorDebuggedProcesses
        {
            get { return _colorDebuggedProcesses.HasValue ? _colorDebuggedProcesses.Value : (_colorDebuggedProcesses = (Color)this["ColorDebuggedProcesses"]).Value; }
            set { this["ColorDebuggedProcesses"] = _colorDebuggedProcesses = value; }
        }

        private Color? _colorDotNetProcesses;
        [SettingDefault("222, 255, 0")]
        public Color ColorDotNetProcesses
        {
            get { return _colorDotNetProcesses.HasValue ? _colorDotNetProcesses.Value : (_colorDotNetProcesses = (Color)this["ColorDotNetProcesses"]).Value; }
            set { this["ColorDotNetProcesses"] = _colorDotNetProcesses = value; }
        }

        private Color? _colorElevatedProcesses;
        [SettingDefault("255, 170, 0")]
        public Color ColorElevatedProcesses
        {
            get { return _colorElevatedProcesses.HasValue ? _colorElevatedProcesses.Value : (_colorElevatedProcesses = (Color)this["ColorElevatedProcesses"]).Value; }
            set { this["ColorElevatedProcesses"] = _colorElevatedProcesses = value; }
        }

        private Color? _colorGuiThreads;
        [SettingDefault("255, 255, 128")]
        public Color ColorGuiThreads
        {
            get { return _colorGuiThreads.HasValue ? _colorGuiThreads.Value : (_colorGuiThreads = (Color)this["ColorGuiThreads"]).Value; }
            set { this["ColorGuiThreads"] = _colorGuiThreads = value; }
        }

        private Color? _colorInheritHandles;
        [SettingDefault("128, 255, 255")]
        public Color ColorInheritHandles
        {
            get { return _colorInheritHandles.HasValue ? _colorInheritHandles.Value : (_colorInheritHandles = (Color)this["ColorInheritHandles"]).Value; }
            set { this["ColorInheritHandles"] = _colorInheritHandles = value; }
        }

        private Color? _colorJobProcesses;
        [SettingDefault("Peru")]
        public Color ColorJobProcesses
        {
            get { return _colorJobProcesses.HasValue ? _colorJobProcesses.Value : (_colorJobProcesses = (Color)this["ColorJobProcesses"]).Value; }
            set { this["ColorJobProcesses"] = _colorJobProcesses = value; }
        }

        private Color? _colorNew;
        [SettingDefault("Chartreuse")]
        public Color ColorNew
        {
            get { return _colorNew.HasValue ? _colorNew.Value : (_colorNew = (Color)this["ColorNew"]).Value; }
            set { this["ColorNew"] = _colorNew = value; }
        }

        private Color? _colorOwnProcesses;
        [SettingDefault("255, 255, 170")]
        public Color ColorOwnProcesses
        {
            get { return _colorOwnProcesses.HasValue ? _colorOwnProcesses.Value : (_colorOwnProcesses = (Color)this["ColorOwnProcesses"]).Value; }
            set { this["ColorOwnProcesses"] = _colorOwnProcesses = value; }
        }

        private Color? _colorPackedProcesses;
        [SettingDefault("DeepPink")]
        public Color ColorPackedProcesses
        {
            get { return _colorPackedProcesses.HasValue ? _colorPackedProcesses.Value : (_colorPackedProcesses = (Color)this["ColorPackedProcesses"]).Value; }
            set { this["ColorPackedProcesses"] = _colorPackedProcesses = value; }
        }

        private Color? _colorPosixProcesses;
        [SettingDefault("DarkSlateBlue")]
        public Color ColorPosixProcesses
        {
            get { return _colorPosixProcesses.HasValue ? _colorPosixProcesses.Value : (_colorPosixProcesses = (Color)this["ColorPosixProcesses"]).Value; }
            set { this["ColorPosixProcesses"] = _colorPosixProcesses = value; }
        }

        private Color? _colorProtectedHandles;
        [SettingDefault("Gray")]
        public Color ColorProtectedHandles
        {
            get { return _colorProtectedHandles.HasValue ? _colorProtectedHandles.Value : (_colorProtectedHandles = (Color)this["ColorProtectedHandles"]).Value; }
            set { this["ColorProtectedHandles"] = _colorProtectedHandles = value; }
        }

        private Color? _colorRelocatedDlls;
        [SettingDefault("255, 192, 128")]
        public Color ColorRelocatedDlls
        {
            get { return _colorRelocatedDlls.HasValue ? _colorRelocatedDlls.Value : (_colorRelocatedDlls = (Color)this["ColorRelocatedDlls"]).Value; }
            set { this["ColorRelocatedDlls"] = _colorRelocatedDlls = value; }
        }

        private Color? _colorRemoved;
        [SettingDefault("255, 60, 40")]
        public Color ColorRemoved
        {
            get { return _colorRemoved.HasValue ? _colorRemoved.Value : (_colorRemoved = (Color)this["ColorRemoved"]).Value; }
            set { this["ColorRemoved"] = _colorRemoved = value; }
        }

        private Color? _colorServiceProcesses;
        [SettingDefault("204, 255, 255")]
        public Color ColorServiceProcesses
        {
            get { return _colorServiceProcesses.HasValue ? _colorServiceProcesses.Value : (_colorServiceProcesses = (Color)this["ColorServiceProcesses"]).Value; }
            set { this["ColorServiceProcesses"] = _colorServiceProcesses = value; }
        }

        private Color? _colorSuspended;
        [SettingDefault("Silver")]
        public Color ColorSuspended
        {
            get { return _colorSuspended.HasValue ? _colorSuspended.Value : (_colorSuspended = (Color)this["ColorSuspended"]).Value; }
            set { this["ColorSuspended"] = _colorSuspended = value; }
        }

        private Color? _colorSystemProcesses;
        [SettingDefault("170, 204, 255")]
        public Color ColorSystemProcesses
        {
            get { return _colorSystemProcesses.HasValue ? _colorSystemProcesses.Value : (_colorSystemProcesses = (Color)this["ColorSystemProcesses"]).Value; }
            set { this["ColorSystemProcesses"] = _colorSystemProcesses = value; }
        }

        private Color? _colorWow64Processes;
        [SettingDefault("RosyBrown")]
        public Color ColorWow64Processes
        {
            get { return _colorWow64Processes.HasValue ? _colorWow64Processes.Value : (_colorWow64Processes = (Color)this["ColorWow64Processes"]).Value; }
            set { this["ColorWow64Processes"] = _colorWow64Processes = value; }
        }

        [SettingDefault("False")]
        public bool CommitHistoryIconVisible
        {
            get { return (bool)this["CommitHistoryIconVisible"]; }
            set { this["CommitHistoryIconVisible"] = value; }
        }

        [SettingDefault("True")]
        public bool CpuHistoryIconVisible
        {
            get { return (bool)this["CpuHistoryIconVisible"]; }
            set { this["CpuHistoryIconVisible"] = value; }
        }

        [SettingDefault("False")]
        public bool CpuUsageIconVisible
        {
            get { return (bool)this["CpuUsageIconVisible"]; }
            set { this["CpuUsageIconVisible"] = value; }
        }

        [SettingDefault("dbghelp.dll")]
        public string DbgHelpPath
        {
            get { return (string)this["DbgHelpPath"]; }
            set { this["DbgHelpPath"] = value; }
        }

        [SettingDefault("")]
        public string DbgHelpSearchPath
        {
            get { return (string)this["DbgHelpSearchPath"]; }
            set { this["DbgHelpSearchPath"] = value; }
        }

        [SettingDefault("True")]
        public bool DbgHelpUndecorate
        {
            get { return (bool)this["DbgHelpUndecorate"]; }
            set { this["DbgHelpUndecorate"] = value; }
        }

        [SettingDefault("False")]
        public bool DbgHelpWarningShown
        {
            get { return (bool)this["DbgHelpWarningShown"]; }
            set { this["DbgHelpWarningShown"] = value; }
        }

        private bool? _deletedServices;
        [SettingDefault("True")]
        public bool DeletedServices
        {
            get { return _deletedServices.HasValue ? _deletedServices.Value : (_deletedServices = (bool)this["DeletedServices"]).Value; }
            set { this["DeletedServices"] = _deletedServices = value; }
        }

        private int _elevationLevel = 1;
        [SettingDefault("1")]
        public int ElevationLevel
        {
            get { return _elevationLevel; }
            set { this["ElevationLevel"] = _elevationLevel = value; }
        }

        [SettingDefault("False")]
        public bool EnableExperimentalFeatures
        {
            get { return (bool)this["EnableExperimentalFeatures"]; }
            set { this["EnableExperimentalFeatures"] = value; }
        }

        [SettingDefault("True")]
        public bool EnableKPH
        {
            get { return (bool)this["EnableKPH"]; }
            set { this["EnableKPH"] = value; }
        }

        [SettingDefault("")]
        public string EnvironmentListViewColumns
        {
            get { return (string)this["EnvironmentListViewColumns"]; }
            set { this["EnvironmentListViewColumns"] = value; }
        }

        [SettingDefault("True")]
        public bool FirstRun
        {
            get { return (bool)this["FirstRun"]; }
            set { this["FirstRun"] = value; }
        }

        private bool? _floatChildWindows;
        [SettingDefault("True")]
        public bool FloatChildWindows
        {
            get { return _floatChildWindows.HasValue ? _floatChildWindows.Value : (_floatChildWindows = (bool)this["FloatChildWindows"]).Value; }
            set { this["FloatChildWindows"] = _floatChildWindows = value; }
        }

        private Font _font;
        [SettingDefault("Microsoft Sans Serif, 8.25pt")]
        public Font Font
        {
            get { return _font != null ? _font : (_font = (Font)this["Font"]); }
            set { this["Font"] = _font = value; }
        }

        [SettingDefault("")]
        public string GroupListColumns
        {
            get { return (string)this["GroupListColumns"]; }
            set { this["GroupListColumns"] = value; }
        }

        [SettingDefault("")]
        public string HandleFilterWindowListViewColumns
        {
            get { return (string)this["HandleFilterWindowListViewColumns"]; }
            set { this["HandleFilterWindowListViewColumns"] = value; }
        }

        [SettingDefault("200, 200")]
        public Point HandleFilterWindowLocation
        {
            get { return (Point)this["HandleFilterWindowLocation"]; }
            set { this["HandleFilterWindowLocation"] = value; }
        }

        [SettingDefault("554, 463")]
        public Size HandleFilterWindowSize
        {
            get { return (Size)this["HandleFilterWindowSize"]; }
            set { this["HandleFilterWindowSize"] = value; }
        }

        [SettingDefault("")]
        public string HandleListViewColumns
        {
            get { return (string)this["HandleListViewColumns"]; }
            set { this["HandleListViewColumns"] = value; }
        }

        [SettingDefault("")]
        public string HiddenProcessesColumns
        {
            get { return (string)this["HiddenProcessesColumns"]; }
            set { this["HiddenProcessesColumns"] = value; }
        }

        [SettingDefault("200, 200")]
        public Point HiddenProcessesWindowLocation
        {
            get { return (Point)this["HiddenProcessesWindowLocation"]; }
            set { this["HiddenProcessesWindowLocation"] = value; }
        }

        [SettingDefault("527, 429")]
        public Size HiddenProcessesWindowSize
        {
            get { return (Size)this["HiddenProcessesWindowSize"]; }
            set { this["HiddenProcessesWindowSize"] = value; }
        }

        [SettingDefault("True")]
        public bool HideHandlesWithNoName
        {
            get { return (bool)this["HideHandlesWithNoName"]; }
            set { this["HideHandlesWithNoName"] = value; }
        }

        [SettingDefault("True")]
        public bool HideProcessHackerNetworkConnections
        {
            get { return (bool)this["HideProcessHackerNetworkConnections"]; }
            set { this["HideProcessHackerNetworkConnections"] = value; }
        }

        private bool? _hideWhenClosed;
        [SettingDefault("False")]
        public bool HideWhenClosed
        {
            get { return _hideWhenClosed.HasValue ? _hideWhenClosed.Value : (_hideWhenClosed = (bool)this["HideWhenClosed"]).Value; }
            set { this["HideWhenClosed"] = _hideWhenClosed = value; }
        }

        private bool? _hideWhenMinimized;
        [SettingDefault("False")]
        public bool HideWhenMinimized
        {
            get { return _hideWhenMinimized.HasValue ? _hideWhenMinimized.Value : (_hideWhenMinimized = (bool)this["HideWhenMinimized"]).Value; }
            set { this["HideWhenMinimized"] = _hideWhenMinimized = value; }
        }

        private int _highlightingDuration = 1000;
        [SettingDefault("1000")]
        public int HighlightingDuration
        {
            get { return _highlightingDuration; }
            set { this["HighlightingDuration"] = _highlightingDuration = value; }
        }

        private int _iconMenuProcessCount = 10;
        [SettingDefault("10")]
        public int IconMenuProcessCount
        {
            get { return _iconMenuProcessCount; }
            set { this["IconMenuProcessCount"] = _iconMenuProcessCount = value; }
        }

        [SettingDefault("audiodg.exe, csrss.exe, dwm.exe, explorer.exe, logonui.exe, lsass.exe, lsm.exe, ntkrnlpa.exe, ntoskrnl.exe, procexp.exe, rundll32.exe, services.exe, smss.exe, spoolsv.exe, svchost.exe, taskeng.exe, taskmgr.exe, wininit.exe, winlogon.exe")]
        public string ImposterNames
        {
            get { return (string)this["ImposterNames"]; }
            set { this["ImposterNames"] = value; }
        }

        [SettingDefault("565, 377")]
        public Size InformationBoxSize
        {
            get { return (Size)this["InformationBoxSize"]; }
            set { this["InformationBoxSize"] = value; }
        }

        [SettingDefault("False")]
        public bool IoHistoryIconVisible
        {
            get { return (bool)this["IoHistoryIconVisible"]; }
            set { this["IoHistoryIconVisible"] = value; }
        }

        [SettingDefault("0,410")]
        public string IPInfoPingListViewColumns
        {
            get { return (string)this["IPInfoPingListViewColumns"]; }
            set { this["IPInfoPingListViewColumns"] = value; }
        }

        [SettingDefault("0,30|1,60|2,100|3,200")]
        public string IPInfoTracertListViewColumns
        {
            get { return (string)this["IPInfoTracertListViewColumns"]; }
            set { this["IPInfoTracertListViewColumns"] = value; }
        }

        [SettingDefault("0,410")]
        public string IPInfoWhoIsListViewColumns
        {
            get { return (string)this["IPInfoWhoIsListViewColumns"]; }
            set { this["IPInfoWhoIsListViewColumns"] = value; }
        }

        [SettingDefault("False")]
        public bool LogWindowAutoScroll
        {
            get { return (bool)this["LogWindowAutoScroll"]; }
            set { this["LogWindowAutoScroll"] = value; }
        }

        [SettingDefault("300, 300")]
        public Point LogWindowLocation
        {
            get { return (Point)this["LogWindowLocation"]; }
            set { this["LogWindowLocation"] = value; }
        }

        [SettingDefault("595, 508")]
        public Size LogWindowSize
        {
            get { return (Size)this["LogWindowSize"]; }
            set { this["LogWindowSize"] = value; }
        }

        private int _maxSamples = 512;
        [SettingDefault("512")]
        public int MaxSamples
        {
            get { return _maxSamples; }
            set { this["MaxSamples"] = _maxSamples = value; }
        }

        [SettingDefault("")]
        public string MemoryListViewColumns
        {
            get { return (string)this["MemoryListViewColumns"]; }
            set { this["MemoryListViewColumns"] = value; }
        }

        [SettingDefault("791, 503")]
        public Size MemoryWindowSize
        {
            get { return (Size)this["MemoryWindowSize"]; }
            set { this["MemoryWindowSize"] = value; }
        }

        [SettingDefault("")]
        public string ModuleListViewColumns
        {
            get { return (string)this["ModuleListViewColumns"]; }
            set { this["ModuleListViewColumns"] = value; }
        }

        [SettingDefault("")]
        public string NetworkListViewColumns
        {
            get { return (string)this["NetworkListViewColumns"]; }
            set { this["NetworkListViewColumns"] = value; }
        }

        private bool? _newProcesses;
        [SettingDefault("False")]
        public bool NewProcesses
        {
            get { return _newProcesses.HasValue ? _newProcesses.Value : (_newProcesses = (bool)this["NewProcesses"]).Value; }
            set { this["NewProcesses"] = _newProcesses = value; }
        }

        private bool? _newServices;
        [SettingDefault("True")]
        public bool NewServices
        {
            get { return _newServices.HasValue ? _newServices.Value : (_newServices = (bool)this["NewServices"]).Value; }
            set { this["NewServices"] = _newServices = value; }
        }

        [SettingDefault("")]
        public string PECOFFHColumns
        {
            get { return (string)this["PECOFFHColumns"]; }
            set { this["PECOFFHColumns"] = value; }
        }

        [SettingDefault("")]
        public string PECOFFOHColumns
        {
            get { return (string)this["PECOFFOHColumns"]; }
            set { this["PECOFFOHColumns"] = value; }
        }

        [SettingDefault("")]
        public string PEExportsColumns
        {
            get { return (string)this["PEExportsColumns"]; }
            set { this["PEExportsColumns"] = value; }
        }

        [SettingDefault("")]
        public string PEImageDataColumns
        {
            get { return (string)this["PEImageDataColumns"]; }
            set { this["PEImageDataColumns"] = value; }
        }

        [SettingDefault("")]
        public string PEImportsColumns
        {
            get { return (string)this["PEImportsColumns"]; }
            set { this["PEImportsColumns"] = value; }
        }

        [SettingDefault("")]
        public string PESectionsColumns
        {
            get { return (string)this["PESectionsColumns"]; }
            set { this["PESectionsColumns"] = value; }
        }

        [SettingDefault("439, 413")]
        public Size PEWindowSize
        {
            get { return (Size)this["PEWindowSize"]; }
            set { this["PEWindowSize"] = value; }
        }

        [SettingDefault("False")]
        public bool PhysMemHistoryIconVisible
        {
            get { return (bool)this["PhysMemHistoryIconVisible"]; }
            set { this["PhysMemHistoryIconVisible"] = value; }
        }

        private bool _plotterAntialias;
        [SettingDefault("False")]
        public bool PlotterAntialias
        {
            get { return _plotterAntialias; }
            set { this["PlotterAntialias"] = _plotterAntialias = value; }
        }

        private Color? _plotterCPUKernelColor;
        [SettingDefault("Lime")]
        public Color PlotterCPUKernelColor
        {
            get { return _plotterCPUKernelColor.HasValue ? _plotterCPUKernelColor.Value : (_plotterCPUKernelColor = (Color)this["PlotterCPUKernelColor"]).Value; }
            set { this["PlotterCPUKernelColor"] = _plotterCPUKernelColor = value; }
        }

        private Color? _plotterCPUUserColor;
        [SettingDefault("Red")]
        public Color PlotterCPUUserColor
        {
            get { return _plotterCPUUserColor.HasValue ? _plotterCPUUserColor.Value : (_plotterCPUUserColor = (Color)this["PlotterCPUUserColor"]).Value; }
            set { this["PlotterCPUUserColor"] = _plotterCPUUserColor = value; }
        }

        private Color? _plotterIOROColor;
        [SettingDefault("Yellow")]
        public Color PlotterIOROColor
        {
            get { return _plotterIOROColor.HasValue ? _plotterIOROColor.Value : (_plotterIOROColor = (Color)this["PlotterIOROColor"]).Value; }
            set { this["PlotterIOROColor"] = _plotterIOROColor = value; }
        }

        private Color? _plotterIOWColor;
        [SettingDefault("DarkViolet")]
        public Color PlotterIOWColor
        {
            get { return _plotterIOWColor.HasValue ? _plotterIOWColor.Value : (_plotterIOWColor = (Color)this["PlotterIOWColor"]).Value; }
            set { this["PlotterIOWColor"] = _plotterIOWColor = value; }
        }

        private Color? _plotterMemoryPrivateColor;
        [SettingDefault("Orange")]
        public Color PlotterMemoryPrivateColor
        {
            get { return _plotterMemoryPrivateColor.HasValue ? _plotterMemoryPrivateColor.Value : (_plotterMemoryPrivateColor = (Color)this["PlotterMemoryPrivateColor"]).Value; }
            set { this["PlotterMemoryPrivateColor"] = _plotterMemoryPrivateColor = value; }
        }

        private Color? _plotterMemoryWSColor;
        [SettingDefault("Cyan")]
        public Color PlotterMemoryWSColor
        {
            get { return _plotterMemoryWSColor.HasValue ? _plotterMemoryWSColor.Value : (_plotterMemoryWSColor = (Color)this["PlotterMemoryWSColor"]).Value; }
            set { this["PlotterMemoryWSColor"] = _plotterMemoryWSColor = value; }
        }

        private int _plotterStep = 2;
        [SettingDefault("2")]
        public int PlotterStep
        {
            get { return _plotterStep; }
            set { this["PlotterStep"] = _plotterStep = value; }
        }

        [SettingDefault("")]
        public string PrivilegeListColumns
        {
            get { return (string)this["PrivilegeListColumns"]; }
            set { this["PrivilegeListColumns"] = value; }
        }

        [SettingDefault("")]
        public string ProcessTreeColumns
        {
            get { return (string)this["ProcessTreeColumns"]; }
            set { this["ProcessTreeColumns"] = value; }
        }

        [SettingDefault("1")]
        public int ProcessTreeStyle
        {
            get { return (int)this["ProcessTreeStyle"]; }
            set { this["ProcessTreeStyle"] = value; }
        }

        [SettingDefault("200, 200")]
        public Point ProcessWindowLocation
        {
            get { return (Point)this["ProcessWindowLocation"]; }
            set { this["ProcessWindowLocation"] = value; }
        }

        [SettingDefault("tabGeneral")]
        public string ProcessWindowSelectedTab
        {
            get { return (string)this["ProcessWindowSelectedTab"]; }
            set { this["ProcessWindowSelectedTab"] = value; }
        }

        [SettingDefault("505, 512")]
        public Size ProcessWindowSize
        {
            get { return (Size)this["ProcessWindowSize"]; }
            set { this["ProcessWindowSize"] = value; }
        }

        [SettingDefault("")]
        public string PromptBoxText
        {
            get { return (string)this["PromptBoxText"]; }
            set { this["PromptBoxText"] = value; }
        }

        private int _refreshInterval = 1000;
        [SettingDefault("1000")]
        public int RefreshInterval
        {
            get { return _refreshInterval; }
            set { this["RefreshInterval"] = _refreshInterval = value; }
        }

        [SettingDefault("")]
        public string ResultsListViewColumns
        {
            get { return (string)this["ResultsListViewColumns"]; }
            set { this["ResultsListViewColumns"] = value; }
        }

        [SettingDefault("504, 482")]
        public Size ResultsWindowSize
        {
            get { return (Size)this["ResultsWindowSize"]; }
            set { this["ResultsWindowSize"] = value; }
        }

        [SettingDefault("")]
        public string RunAsCommand
        {
            get { return (string)this["RunAsCommand"]; }
            set { this["RunAsCommand"] = value; }
        }

        [SettingDefault("")]
        public string RunAsUsername
        {
            get { return (string)this["RunAsUsername"]; }
            set { this["RunAsUsername"] = value; }
        }

        [SettingDefault("False")]
        public bool ScrollDownProcessTree
        {
            get { return (bool)this["ScrollDownProcessTree"]; }
            set { this["ScrollDownProcessTree"] = value; }
        }

        [SettingDefault("http://www.google.com/search?q=%s")]
        public string SearchEngine
        {
            get { return (string)this["SearchEngine"]; }
            set { this["SearchEngine"] = value; }
        }

        [SettingDefault("&String Scan...")]
        public string SearchType
        {
            get { return (string)this["SearchType"]; }
            set { this["SearchType"] = value; }
        }

        [SettingDefault("")]
        public string ServiceListViewColumns
        {
            get { return (string)this["ServiceListViewColumns"]; }
            set { this["ServiceListViewColumns"] = value; }
        }

        [SettingDefault("")]
        public string ServiceMiniListColumns
        {
            get { return (string)this["ServiceMiniListColumns"]; }
            set { this["ServiceMiniListColumns"] = value; }
        }

        private bool _showAccountDomains = false;
        [SettingDefault("False")]
        public bool ShowAccountDomains
        {
            get { return _showAccountDomains; }
            set { this["ShowAccountDomains"] = _showAccountDomains = value; }
        }

        private bool? _showOneGraphPerCPU;
        [SettingDefault("False")]
        public bool ShowOneGraphPerCPU
        {
            get { return _showOneGraphPerCPU.HasValue ? _showOneGraphPerCPU.Value : (_showOneGraphPerCPU = (bool)this["ShowOneGraphPerCPU"]).Value; }
            set { this["ShowOneGraphPerCPU"] = _showOneGraphPerCPU = value; }
        }

        private bool? _startedServices;
        [SettingDefault("False")]
        public bool StartedServices
        {
            get { return _startedServices.HasValue ? _startedServices.Value : (_startedServices = (bool)this["StartedServices"]).Value; }
            set { this["StartedServices"] = _startedServices = value; }
        }

        [SettingDefault("False")]
        public bool StartHidden
        {
            get { return (bool)this["StartHidden"]; }
            set { this["StartHidden"] = value; }
        }

        private bool? _stoppedServices;
        [SettingDefault("False")]
        public bool StoppedServices
        {
            get { return _stoppedServices.HasValue ? _stoppedServices.Value : (_stoppedServices = (bool)this["StoppedServices"]).Value; }
            set { this["StoppedServices"] = _stoppedServices = value; }
        }

        [SettingDefault("100, 100")]
        public Point SysInfoWindowLocation
        {
            get { return (Point)this["SysInfoWindowLocation"]; }
            set { this["SysInfoWindowLocation"] = value; }
        }

        [SettingDefault("851, 577")]
        public Size SysInfoWindowSize
        {
            get { return (Size)this["SysInfoWindowSize"]; }
            set { this["SysInfoWindowSize"] = value; }
        }

        private bool? _terminatedProcesses;
        [SettingDefault("False")]
        public bool TerminatedProcesses
        {
            get { return _terminatedProcesses.HasValue ? _terminatedProcesses.Value : (_terminatedProcesses = (bool)this["TerminatedProcesses"]).Value; }
            set { this["TerminatedProcesses"] = _terminatedProcesses = value; }
        }

        [SettingDefault("")]
        public string ThreadListViewColumns
        {
            get { return (string)this["ThreadListViewColumns"]; }
            set { this["ThreadListViewColumns"] = value; }
        }

        [SettingDefault("415, 503")]
        public Size ThreadWindowSize
        {
            get { return (Size)this["ThreadWindowSize"]; }
            set { this["ThreadWindowSize"] = value; }
        }

        [SettingDefault("481, 468")]
        public Size TokenWindowSize
        {
            get { return (Size)this["TokenWindowSize"]; }
            set { this["TokenWindowSize"] = value; }
        }

        [SettingDefault("tabPrivileges")]
        public string TokenWindowTab
        {
            get { return (string)this["TokenWindowTab"]; }
            set { this["TokenWindowTab"] = value; }
        }

        [SettingDefault("False")]
        public bool ToolbarVisible
        {
            get { return (bool)this["ToolbarVisible"]; }
            set { this["ToolbarVisible"] = value; }
        }

        [SettingDefault("1")]
        public int ToolStripDisplayStyle
        {
            get { return (int)this["ToolStripDisplayStyle"]; }
            set { this["ToolStripDisplayStyle"] = value; }
        }

        private int _unitSpecifier = 6;
        [SettingDefault("6")]
        public int UnitSpecifier
        {
            get { return _unitSpecifier; }
            set { this["UnitSpecifier"] = _unitSpecifier = value; }
        }

        private bool? _useColorDebuggedProcesses;
        [SettingDefault("True")]
        public bool UseColorDebuggedProcesses
        {
            get { return _useColorDebuggedProcesses.HasValue ? _useColorDebuggedProcesses.Value : (_useColorDebuggedProcesses = (bool)this["UseColorDebuggedProcesses"]).Value; }
            set { this["UseColorDebuggedProcesses"] = _useColorDebuggedProcesses = value; }
        }

        private bool? _useColorDotNetProcesses;
        [SettingDefault("True")]
        public bool UseColorDotNetProcesses
        {
            get { return _useColorDotNetProcesses.HasValue ? _useColorDotNetProcesses.Value : (_useColorDotNetProcesses = (bool)this["UseColorDotNetProcesses"]).Value; }
            set { this["UseColorDotNetProcesses"] = _useColorDotNetProcesses = value; }
        }

        private bool? _useColorElevatedProcesses;
        [SettingDefault("True")]
        public bool UseColorElevatedProcesses
        {
            get { return _useColorElevatedProcesses.HasValue ? _useColorElevatedProcesses.Value : (_useColorElevatedProcesses = (bool)this["UseColorElevatedProcesses"]).Value; }
            set { this["UseColorElevatedProcesses"] = _useColorElevatedProcesses = value; }
        }

        private bool? _useColorGuiThreads;
        [SettingDefault("True")]
        public bool UseColorGuiThreads
        {
            get { return _useColorGuiThreads.HasValue ? _useColorGuiThreads.Value : (_useColorGuiThreads = (bool)this["UseColorGuiThreads"]).Value; }
            set { this["UseColorGuiThreads"] = _useColorGuiThreads = value; }
        }

        private bool? _useColorInheritHandles;
        [SettingDefault("True")]
        public bool UseColorInheritHandles
        {
            get { return _useColorInheritHandles.HasValue ? _useColorInheritHandles.Value : (_useColorInheritHandles = (bool)this["UseColorInheritHandles"]).Value; }
            set { this["UseColorInheritHandles"] = _useColorInheritHandles = value; }
        }

        private bool? _useColorJobProcesses;
        [SettingDefault("True")]
        public bool UseColorJobProcesses
        {
            get { return _useColorJobProcesses.HasValue ? _useColorJobProcesses.Value : (_useColorJobProcesses = (bool)this["UseColorJobProcesses"]).Value; }
            set { this["UseColorJobProcesses"] = _useColorJobProcesses = value; }
        }

        private bool? _useColorOwnProcesses;
        [SettingDefault("True")]
        public bool UseColorOwnProcesses
        {
            get { return _useColorOwnProcesses.HasValue ? _useColorOwnProcesses.Value : (_useColorOwnProcesses = (bool)this["UseColorOwnProcesses"]).Value; }
            set { this["UseColorOwnProcesses"] = _useColorOwnProcesses = value; }
        }

        private bool? _useColorPackedProcesses;
        [SettingDefault("True")]
        public bool UseColorPackedProcesses
        {
            get { return _useColorPackedProcesses.HasValue ? _useColorPackedProcesses.Value : (_useColorPackedProcesses = (bool)this["UseColorPackedProcesses"]).Value; }
            set { this["UseColorPackedProcesses"] = _useColorPackedProcesses = value; }
        }

        private bool? _useColorPosixProcesses;
        [SettingDefault("True")]
        public bool UseColorPosixProcesses
        {
            get { return _useColorPosixProcesses.HasValue ? _useColorPosixProcesses.Value : (_useColorPosixProcesses = (bool)this["UseColorPosixProcesses"]).Value; }
            set { this["UseColorPosixProcesses"] = _useColorPosixProcesses = value; }
        }

        private bool? _useColorProtectedHandles;
        [SettingDefault("True")]
        public bool UseColorProtectedHandles
        {
            get { return _useColorProtectedHandles.HasValue ? _useColorProtectedHandles.Value : (_useColorProtectedHandles = (bool)this["UseColorProtectedHandles"]).Value; }
            set { this["UseColorProtectedHandles"] = _useColorProtectedHandles = value; }
        }

        private bool? _useColorRelocatedDlls;
        [SettingDefault("True")]
        public bool UseColorRelocatedDlls
        {
            get { return _useColorRelocatedDlls.HasValue ? _useColorRelocatedDlls.Value : (_useColorRelocatedDlls = (bool)this["UseColorRelocatedDlls"]).Value; }
            set { this["UseColorRelocatedDlls"] = _useColorRelocatedDlls = value; }
        }

        private bool? _useColorServiceProcesses;
        [SettingDefault("True")]
        public bool UseColorServiceProcesses
        {
            get { return _useColorServiceProcesses.HasValue ? _useColorServiceProcesses.Value : (_useColorServiceProcesses = (bool)this["UseColorServiceProcesses"]).Value; }
            set { this["UseColorServiceProcesses"] = _useColorServiceProcesses = value; }
        }

        private bool? _useColorSuspended;
        [SettingDefault("True")]
        public bool UseColorSuspended
        {
            get { return _useColorSuspended.HasValue ? _useColorSuspended.Value : (_useColorSuspended = (bool)this["UseColorSuspended"]).Value; }
            set { this["UseColorSuspended"] = _useColorSuspended = value; }
        }

        private bool? _useColorSystemProcesses;
        [SettingDefault("True")]
        public bool UseColorSystemProcesses
        {
            get { return _useColorSystemProcesses.HasValue ? _useColorSystemProcesses.Value : (_useColorSystemProcesses = (bool)this["UseColorSystemProcesses"]).Value; }
            set { this["UseColorSystemProcesses"] = _useColorSystemProcesses = value; }
        }

        private bool? _useColorWow64Processes;
        [SettingDefault("True")]
        public bool UseColorWow64Processes
        {
            get { return _useColorWow64Processes.HasValue ? _useColorWow64Processes.Value : (_useColorWow64Processes = (bool)this["UseColorWow64Processes"]).Value; }
            set { this["UseColorWow64Processes"] = _useColorWow64Processes = value; }
        }

        private bool? _verifySignatures;
        [SettingDefault("False")]
        public bool VerifySignatures
        {
            get { return _verifySignatures.HasValue ? _verifySignatures.Value : (_verifySignatures = (bool)this["VerifySignatures"]).Value; }
            set { this["VerifySignatures"] = _verifySignatures = value; }
        }

        private bool _warnDangerous = true;
        [SettingDefault("True")]
        public bool WarnDangerous
        {
            get { return _warnDangerous; }
            set { this["WarnDangerous"] = _warnDangerous = value; }
        }

        [SettingDefault("200, 200")]
        public Point WindowLocation
        {
            get { return (Point)this["WindowLocation"]; }
            set { this["WindowLocation"] = value; }
        }

        [SettingDefault("844, 550")]
        public Size WindowSize
        {
            get { return (Size)this["WindowSize"]; }
            set { this["WindowSize"] = value; }
        }

        [SettingDefault("Normal")]
        public FormWindowState WindowState
        {
            get { return (FormWindowState)this["WindowState"]; }
            set { this["WindowState"] = value; }
        }
    }
}
