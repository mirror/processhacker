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
        private static Settings _instance;
        private static XmlFileSettingsStore _store;

        public static Settings Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        public Settings(string fileName)
            : base(new XmlFileSettingsStore(fileName))
        {
            this.Invalidate();
        }

        public string SettingsFileName
        {
            get { return _store.FileName; }
        }

        public override void Invalidate()
        {
            _allowOnlyOneInstance = null;
            _alwaysOnTop = null;
            _appUpdateAutomatic = null;
            _appUpdateLevel = null;
            _appUpdateUrl = null;
            _callStackColumns = null;
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
            _commitHistoryIconVisible = null;
            _cpuHistoryIconVisible = null;
            _cpuUsageIconVisible = null;
            _dbgHelpPath = null;
            _dbgHelpSearchPath = null;
            _dbgHelpUndecorate = null;
            _dbgHelpWarningShown = null;
            _deletedServices = null;
            _elevationLevel = (int)this["ElevationLevel"];
            _enableExperimentalFeatures = null;
            _enableKPH = null;
            _environmentListViewColumns = null;
            _firstRun = null;
            _floatChildWindows = null;
            _font = null;
            _groupListColumns = null;
            _handleFilterWindowListViewColumns = null;
            _handleFilterWindowLocation = null;
            _handleFilterWindowSize = null;
            _handleListViewColumns = null;
            _hiddenProcessesColumns = null;
            _hiddenProcessesWindowLocation = null;
            _hiddenProcessesWindowSize = null;
            _hideHandlesWithNoName = null;
            _hideProcessHackerNetworkConnections = null;
            _hideWhenClosed = null;
            _hideWhenMinimized = null;
            _highlightingDuration = (int)this["HighlightingDuration"];
            _iconMenuProcessCount = (int)this["IconMenuProcessCount"];
            _imposterNames = null;
            _informationBoxSize = null;
            _ioHistoryIconVisible = null;
            _ipInfoPingListViewColumns = null;
            _ipInfoTracertListViewColumns = null;
            _ipInfoWhoIsListViewColumns = null;
            _logWindowAutoScroll = null;
            _logWindowLocation = null;
            _logWindowSize = null;
            _maxSamples = (int)this["MaxSamples"];
            _memoryListViewColumns = null;
            _memoryWindowSize = null;
            _moduleListViewColumns = null;
            _networkListViewColumns = null;
            _newProcesses = null;
            _newServices = null;
            _peCoffHColumns = null;
            _peCoffOHColumns = null;
            _peExportsColumns = null;
            _peImageDataColumns = null;
            _peImportsColumns = null;
            _peSectionsColumns = null;
            _peWindowSize = null;
            _physMemHistoryIconVisible = null;
            _plotterAntialias = (bool)this["PlotterAntialias"];
            _plotterCPUKernelColor = null;
            _plotterCPUUserColor = null;
            _plotterIOROColor = null;
            _plotterIOWColor = null;
            _plotterMemoryPrivateColor = null;
            _plotterMemoryWSColor = null;
            _plotterStep = (int)this["PlotterStep"];
            _privilegeListColumns = null;
            _processTreeColumns = null;
            _processWindowLocation = null;
            _processWindowSelectedTab = null;
            _processWindowSize = null;
            _promptBoxText = null;
            _refreshInterval = (int)this["RefreshInterval"];
            _resultsListViewColumns = null;
            _resultsWindowSize = null;
            _runAsCommand = null;
            _runAsUsername = null;
            _scrollDownProcessTree = null;
            _searchEngine = null;
            _searchType = null;
            _serviceListViewColumns = null;
            _serviceMiniListColumns = null;
            _showAccountDomains = (bool)this["ShowAccountDomains"];
            _showOneGraphPerCPU = null;
            _startedServices = null;
            _startHidden = null;
            _stoppedServices = null;
            _sysInfoWindowLocation = null;
            _sysInfoWindowSize = null;
            _terminatedProcesses = null;
            _threadListViewColumns = null;
            _threadWindowSize = null;
            _tokenWindowSize = null;
            _tokenWindowTab = null;
            _toolbarVisible = null;
            _toolStripDisplayStyle = null;
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
            _windowLocation = null;
            _windowSize = null;
            _windowState = null;
        }

        private bool? _allowOnlyOneInstance;
        [SettingDefault("False")]
        public bool AllowOnlyOneInstance
        {
            get { return _allowOnlyOneInstance.HasValue ? _allowOnlyOneInstance.Value : (bool)this["AllowOnlyOneInstance"]; }
            set { this["AllowOnlyOneInstance"] = _allowOnlyOneInstance = value; }
        }

        private bool? _alwaysOnTop;
        [SettingDefault("False")]
        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop.HasValue ? _alwaysOnTop.Value : (bool)this["AlwaysOnTop"]; }
            set { this["AlwaysOnTop"] = _alwaysOnTop = value; }
        }

        private bool? _appUpdateAutomatic;
        [SettingDefault("True")]
        public bool AppUpdateAutomatic
        {
            get { return _appUpdateAutomatic.HasValue ? _appUpdateAutomatic.Value : (bool)this["AppUpdateAutomatic"]; }
            set { this["AppUpdateAutomatic"] = _appUpdateAutomatic = value; }
        }

        private int? _appUpdateLevel;
        [SettingDefault("1")]
        public int AppUpdateLevel
        {
            get { return _appUpdateLevel.HasValue ? _appUpdateLevel.Value : (int)this["AppUpdateLevel"]; }
            set { this["AppUpdateLevel"] = _appUpdateLevel = value; }
        }

        private string _appUpdateUrl;
        [SettingDefault("http://processhacker.sourceforge.net/AppUpdate.xml")]
        public string AppUpdateUrl
        {
            get { return _appUpdateUrl != null ? _appUpdateUrl : (string)this["AppUpdateUrl"]; }
            set { this["AppUpdateUrl"] = _appUpdateUrl = value; }
        }

        private string _callStackColumns;
        [SettingDefault("")]
        public string CallStackColumns
        {
            get { return _callStackColumns != null ? _callStackColumns : (string)this["CallStackColumns"]; }
            set { this["CallStackColumns"] = _callStackColumns = value; }
        }

        private Color? _colorDebuggedProcesses;
        [SettingDefault("204, 187, 255")]
        public Color ColorDebuggedProcesses
        {
            get { return _colorDebuggedProcesses.HasValue ? _colorDebuggedProcesses.Value : (Color)this["ColorDebuggedProcesses"]; }
            set { this["ColorDebuggedProcesses"] = _colorDebuggedProcesses = value; }
        }

        private Color? _colorDotNetProcesses;
        [SettingDefault("222, 255, 0")]
        public Color ColorDotNetProcesses
        {
            get { return _colorDotNetProcesses.HasValue ? _colorDotNetProcesses.Value : (Color)this["ColorDotNetProcesses"]; }
            set { this["ColorDotNetProcesses"] = _colorDotNetProcesses = value; }
        }

        private Color? _colorElevatedProcesses;
        [SettingDefault("255, 170, 0")]
        public Color ColorElevatedProcesses
        {
            get { return _colorElevatedProcesses.HasValue ? _colorElevatedProcesses.Value : (Color)this["ColorElevatedProcesses"]; }
            set { this["ColorElevatedProcesses"] = _colorElevatedProcesses = value; }
        }

        private Color? _colorGuiThreads;
        [SettingDefault("255, 255, 128")]
        public Color ColorGuiThreads
        {
            get { return _colorGuiThreads.HasValue ? _colorGuiThreads.Value : (Color)this["ColorGuiThreads"]; }
            set { this["ColorGuiThreads"] = _colorGuiThreads = value; }
        }

        private Color? _colorInheritHandles;
        [SettingDefault("128, 255, 255")]
        public Color ColorInheritHandles
        {
            get { return _colorInheritHandles.HasValue ? _colorInheritHandles.Value : (Color)this["ColorInheritHandles"]; }
            set { this["ColorInheritHandles"] = _colorInheritHandles = value; }
        }

        private Color? _colorJobProcesses;
        [SettingDefault("Peru")]
        public Color ColorJobProcesses
        {
            get { return _colorJobProcesses.HasValue ? _colorJobProcesses.Value : (Color)this["ColorJobProcesses"]; }
            set { this["ColorJobProcesses"] = _colorJobProcesses = value; }
        }

        private Color? _colorNew;
        [SettingDefault("Chartreuse")]
        public Color ColorNew
        {
            get { return _colorNew.HasValue ? _colorNew.Value : (Color)this["ColorNew"]; }
            set { this["ColorNew"] = _colorNew = value; }
        }

        private Color? _colorOwnProcesses;
        [SettingDefault("255, 255, 170")]
        public Color ColorOwnProcesses
        {
            get { return _colorOwnProcesses.HasValue ? _colorOwnProcesses.Value : (Color)this["ColorOwnProcesses"]; }
            set { this["ColorOwnProcesses"] = _colorOwnProcesses = value; }
        }

        private Color? _colorPackedProcesses;
        [SettingDefault("DeepPink")]
        public Color ColorPackedProcesses
        {
            get { return _colorPackedProcesses.HasValue ? _colorPackedProcesses.Value : (Color)this["ColorPackedProcesses"]; }
            set { this["ColorPackedProcesses"] = _colorPackedProcesses = value; }
        }

        private Color? _colorPosixProcesses;
        [SettingDefault("DarkSlateBlue")]
        public Color ColorPosixProcesses
        {
            get { return _colorPosixProcesses.HasValue ? _colorPosixProcesses.Value : (Color)this["ColorPosixProcesses"]; }
            set { this["ColorPosixProcesses"] = _colorPosixProcesses = value; }
        }

        private Color? _colorProtectedHandles;
        [SettingDefault("Gray")]
        public Color ColorProtectedHandles
        {
            get { return _colorProtectedHandles.HasValue ? _colorProtectedHandles.Value : (Color)this["ColorProtectedHandles"]; }
            set { this["ColorProtectedHandles"] = _colorProtectedHandles = value; }
        }

        private Color? _colorRelocatedDlls;
        [SettingDefault("255, 192, 128")]
        public Color ColorRelocatedDlls
        {
            get { return _colorRelocatedDlls.HasValue ? _colorRelocatedDlls.Value : (Color)this["ColorRelocatedDlls"]; }
            set { this["ColorRelocatedDlls"] = _colorRelocatedDlls = value; }
        }

        private Color? _colorRemoved;
        [SettingDefault("255, 60, 40")]
        public Color ColorRemoved
        {
            get { return _colorRemoved.HasValue ? _colorRemoved.Value : (Color)this["ColorRemoved"]; }
            set { this["ColorRemoved"] = _colorRemoved = value; }
        }

        private Color? _colorServiceProcesses;
        [SettingDefault("204, 255, 255")]
        public Color ColorServiceProcesses
        {
            get { return _colorServiceProcesses.HasValue ? _colorServiceProcesses.Value : (Color)this["ColorServiceProcesses"]; }
            set { this["ColorServiceProcesses"] = _colorServiceProcesses = value; }
        }

        private Color? _colorSuspended;
        [SettingDefault("Silver")]
        public Color ColorSuspended
        {
            get { return _colorSuspended.HasValue ? _colorSuspended.Value : (Color)this["ColorSuspended"]; }
            set { this["ColorSuspended"] = _colorSuspended = value; }
        }

        private Color? _colorSystemProcesses;
        [SettingDefault("170, 204, 255")]
        public Color ColorSystemProcesses
        {
            get { return _colorSystemProcesses.HasValue ? _colorSystemProcesses.Value : (Color)this["ColorSystemProcesses"]; }
            set { this["ColorSystemProcesses"] = _colorSystemProcesses = value; }
        }

        private Color? _colorWow64Processes;
        [SettingDefault("DeepPink")]
        public Color ColorWow64Processes
        {
            get { return _colorWow64Processes.HasValue ? _colorWow64Processes.Value : (Color)this["ColorWow64Processes"]; }
            set { this["ColorWow64Processes"] = _colorWow64Processes = value; }
        }

        private bool? _commitHistoryIconVisible;
        [SettingDefault("False")]
        public bool CommitHistoryIconVisible
        {
            get { return _commitHistoryIconVisible.HasValue ? _commitHistoryIconVisible.Value : (bool)this["CommitHistoryIconVisible"]; }
            set { this["CommitHistoryIconVisible"] = _commitHistoryIconVisible = value; }
        }

        private bool? _cpuHistoryIconVisible;
        [SettingDefault("True")]
        public bool CpuHistoryIconVisible
        {
            get { return _cpuHistoryIconVisible.HasValue ? _cpuHistoryIconVisible.Value : (bool)this["CpuHistoryIconVisible"]; }
            set { this["CpuHistoryIconVisible"] = _cpuHistoryIconVisible = value; }
        }

        private bool? _cpuUsageIconVisible;
        [SettingDefault("False")]
        public bool CpuUsageIconVisible
        {
            get { return _cpuUsageIconVisible.HasValue ? _cpuUsageIconVisible.Value : (bool)this["CpuUsageIconVisible"]; }
            set { this["CpuUsageIconVisible"] = _cpuUsageIconVisible = value; }
        }

        private string _dbgHelpPath;
        [SettingDefault("dbghelp.dll")]
        public string DbgHelpPath
        {
            get { return _dbgHelpPath != null ? _dbgHelpPath : (string)this["DbgHelpPath"]; }
            set { this["DbgHelpPath"] = _dbgHelpPath = value; }
        }

        private string _dbgHelpSearchPath;
        [SettingDefault("")]
        public string DbgHelpSearchPath
        {
            get { return _dbgHelpSearchPath != null ? _dbgHelpSearchPath : (string)this["DbgHelpSearchPath"]; }
            set { this["DbgHelpSearchPath"] = _dbgHelpSearchPath = value; }
        }

        private bool? _dbgHelpUndecorate;
        [SettingDefault("True")]
        public bool DbgHelpUndecorate
        {
            get { return _dbgHelpUndecorate.HasValue ? _dbgHelpUndecorate.Value : (bool)this["DbgHelpUndecorate"]; }
            set { this["DbgHelpUndecorate"] = _dbgHelpUndecorate = value; }
        }

        private bool? _dbgHelpWarningShown;
        [SettingDefault("False")]
        public bool DbgHelpWarningShown
        {
            get { return _dbgHelpWarningShown.HasValue ? _dbgHelpWarningShown.Value : (bool)this["DbgHelpWarningShown"]; }
            set { this["DbgHelpWarningShown"] = _dbgHelpWarningShown = value; }
        }

        private bool? _deletedServices;
        [SettingDefault("True")]
        public bool DeletedServices
        {
            get { return _deletedServices.HasValue ? _deletedServices.Value : (bool)this["DeletedServices"]; }
            set { this["DeletedServices"] = _deletedServices = value; }
        }

        private int _elevationLevel;
        [SettingDefault("1")]
        public int ElevationLevel
        {
            get { return _elevationLevel; }
            set { this["ElevationLevel"] = _elevationLevel = value; }
        }

        private bool? _enableExperimentalFeatures;
        [SettingDefault("False")]
        public bool EnableExperimentalFeatures
        {
            get { return _enableExperimentalFeatures.HasValue ? _enableExperimentalFeatures.Value : (bool)this["EnableExperimentalFeatures"]; }
            set { this["EnableExperimentalFeatures"] = _enableExperimentalFeatures = value; }
        }

        private bool? _enableKPH;
        [SettingDefault("True")]
        public bool EnableKPH
        {
            get { return _enableKPH.HasValue ? _enableKPH.Value : (bool)this["EnableKPH"]; }
            set { this["EnableKPH"] = _enableKPH = value; }
        }

        private string _environmentListViewColumns;
        [SettingDefault("")]
        public string EnvironmentListViewColumns
        {
            get { return _environmentListViewColumns != null ? _environmentListViewColumns : (string)this["EnvironmentListViewColumns"]; }
            set { this["EnvironmentListViewColumns"] = _environmentListViewColumns = value; }
        }

        private bool? _firstRun;
        [SettingDefault("True")]
        public bool FirstRun
        {
            get { return _firstRun.HasValue ? _firstRun.Value : (bool)this["FirstRun"]; }
            set { this["FirstRun"] = _firstRun = value; }
        }

        private bool? _floatChildWindows;
        [SettingDefault("True")]
        public bool FloatChildWindows
        {
            get { return _floatChildWindows.HasValue ? _floatChildWindows.Value : (bool)this["FloatChildWindows"]; }
            set { this["FloatChildWindows"] = _floatChildWindows = value; }
        }

        private Font _font;
        [SettingDefault("Microsoft Sans Serif, 8.25pt")]
        public Font Font
        {
            get { return _font != null ? _font : (Font)this["Font"]; }
            set { this["Font"] = _font = value; }
        }

        private string _groupListColumns;
        [SettingDefault("")]
        public string GroupListColumns
        {
            get { return _groupListColumns != null ? _groupListColumns : (string)this["GroupListColumns"]; }
            set { this["GroupListColumns"] = _groupListColumns = value; }
        }

        private string _handleFilterWindowListViewColumns;
        [SettingDefault("")]
        public string HandleFilterWindowListViewColumns
        {
            get { return _handleFilterWindowListViewColumns != null ? _handleFilterWindowListViewColumns : (string)this["HandleFilterWindowListViewColumns"]; }
            set { this["HandleFilterWindowListViewColumns"] = _handleFilterWindowListViewColumns = value; }
        }

        private Point? _handleFilterWindowLocation;
        [SettingDefault("200, 200")]
        public Point HandleFilterWindowLocation
        {
            get { return _handleFilterWindowLocation.HasValue ? _handleFilterWindowLocation.Value : (Point)this["HandleFilterWindowLocation"]; }
            set { this["HandleFilterWindowLocation"] = _handleFilterWindowLocation = value; }
        }

        private Size? _handleFilterWindowSize;
        [SettingDefault("554, 463")]
        public Size HandleFilterWindowSize
        {
            get { return _handleFilterWindowSize.HasValue ? _handleFilterWindowSize.Value : (Size)this["HandleFilterWindowSize"]; }
            set { this["HandleFilterWindowSize"] = _handleFilterWindowSize = value; }
        }

        private string _handleListViewColumns;
        [SettingDefault("")]
        public string HandleListViewColumns
        {
            get { return _handleListViewColumns != null ? _handleListViewColumns : (string)this["HandleListViewColumns"]; }
            set { this["HandleListViewColumns"] = _handleListViewColumns = value; }
        }

        private string _hiddenProcessesColumns;
        [SettingDefault("")]
        public string HiddenProcessesColumns
        {
            get { return _hiddenProcessesColumns != null ? _hiddenProcessesColumns : (string)this["HiddenProcessesColumns"]; }
            set { this["HiddenProcessesColumns"] = _hiddenProcessesColumns = value; }
        }

        private Point? _hiddenProcessesWindowLocation;
        [SettingDefault("200, 200")]
        public Point HiddenProcessesWindowLocation
        {
            get { return _hiddenProcessesWindowLocation.HasValue ? _hiddenProcessesWindowLocation.Value : (Point)this["HiddenProcessesWindowLocation"]; }
            set { this["HiddenProcessesWindowLocation"] = _hiddenProcessesWindowLocation = value; }
        }

        private Size? _hiddenProcessesWindowSize;
        [SettingDefault("527, 429")]
        public Size HiddenProcessesWindowSize
        {
            get { return _hiddenProcessesWindowSize.HasValue ? _hiddenProcessesWindowSize.Value : (Size)this["HiddenProcessesWindowSize"]; }
            set { this["HiddenProcessesWindowSize"] = _hiddenProcessesWindowSize = value; }
        }

        private bool? _hideHandlesWithNoName;
        [SettingDefault("True")]
        public bool HideHandlesWithNoName
        {
            get { return _hideHandlesWithNoName.HasValue ? _hideHandlesWithNoName.Value : (bool)this["HideHandlesWithNoName"]; }
            set { this["HideHandlesWithNoName"] = _hideHandlesWithNoName = value; }
        }

        private bool? _hideProcessHackerNetworkConnections;
        [SettingDefault("True")]
        public bool HideProcessHackerNetworkConnections
        {
            get { return _hideProcessHackerNetworkConnections.HasValue ? _hideProcessHackerNetworkConnections.Value : (bool)this["HideProcessHackerNetworkConnections"]; }
            set { this["HideProcessHackerNetworkConnections"] = _hideProcessHackerNetworkConnections = value; }
        }

        private bool? _hideWhenClosed;
        [SettingDefault("False")]
        public bool HideWhenClosed
        {
            get { return _hideWhenClosed.HasValue ? _hideWhenClosed.Value : (bool)this["HideWhenClosed"]; }
            set { this["HideWhenClosed"] = _hideWhenClosed = value; }
        }

        private bool? _hideWhenMinimized;
        [SettingDefault("False")]
        public bool HideWhenMinimized
        {
            get { return _hideWhenMinimized.HasValue ? _hideWhenMinimized.Value : (bool)this["HideWhenMinimized"]; }
            set { this["HideWhenMinimized"] = _hideWhenMinimized = value; }
        }

        private int _highlightingDuration;
        [SettingDefault("1000")]
        public int HighlightingDuration
        {
            get { return _highlightingDuration; }
            set { this["HighlightingDuration"] = _highlightingDuration = value; }
        }

        private int _iconMenuProcessCount;
        [SettingDefault("10")]
        public int IconMenuProcessCount
        {
            get { return _iconMenuProcessCount; }
            set { this["IconMenuProcessCount"] = _iconMenuProcessCount = value; }
        }

        private string _imposterNames;
        [SettingDefault("audiodg.exe, csrss.exe, dwm.exe, explorer.exe, logonui.exe, lsass.exe, lsm.exe, ntkrnlpa.exe, ntoskrnl.exe, procexp.exe, rundll32.exe, services.exe, smss.exe, spoolsv.exe, svchost.exe, taskeng.exe, taskmgr.exe, wininit.exe, winlogon.exe")]
        public string ImposterNames
        {
            get { return _imposterNames != null ? _imposterNames : (string)this["ImposterNames"]; }
            set { this["ImposterNames"] = _imposterNames = value; }
        }

        private Size? _informationBoxSize;
        [SettingDefault("565, 377")]
        public Size InformationBoxSize
        {
            get { return _informationBoxSize.HasValue ? _informationBoxSize.Value : (Size)this["InformationBoxSize"]; }
            set { this["InformationBoxSize"] = _informationBoxSize = value; }
        }

        private bool? _ioHistoryIconVisible;
        [SettingDefault("False")]
        public bool IoHistoryIconVisible
        {
            get { return _ioHistoryIconVisible.HasValue ? _ioHistoryIconVisible.Value : (bool)this["IoHistoryIconVisible"]; }
            set { this["IoHistoryIconVisible"] = _ioHistoryIconVisible = value; }
        }

        private string _ipInfoPingListViewColumns;
        [SettingDefault("0,30|1,60|2,100|3,200")]
        public string IPInfoPingListViewColumns
        {
            get { return _ipInfoPingListViewColumns != null ? _ipInfoPingListViewColumns : (string)this["IPInfoPingListViewColumns"]; }
            set { this["IPInfoPingListViewColumns"] = _ipInfoPingListViewColumns = value; }
        }

        private string _ipInfoTracertListViewColumns;
        [SettingDefault("0,30|1,60|2,100|3,200")]
        public string IPInfoTracertListViewColumns
        {
            get { return _ipInfoTracertListViewColumns != null ? _ipInfoTracertListViewColumns : (string)this["IPInfoTracertListViewColumns"]; }
            set { this["IPInfoTracertListViewColumns"] = _ipInfoTracertListViewColumns = value; }
        }

        private string _ipInfoWhoIsListViewColumns;
        [SettingDefault("0,410")]
        public string IPInfoWhoIsListViewColumns
        {
            get { return _ipInfoWhoIsListViewColumns != null ? _ipInfoWhoIsListViewColumns : (string)this["IPInfoWhoIsListViewColumns"]; }
            set { this["IPInfoWhoIsListViewColumns"] = _ipInfoWhoIsListViewColumns = value; }
        }

        private bool? _logWindowAutoScroll;
        [SettingDefault("False")]
        public bool LogWindowAutoScroll
        {
            get { return _logWindowAutoScroll.HasValue ? _logWindowAutoScroll.Value : (bool)this["LogWindowAutoScroll"]; }
            set { this["LogWindowAutoScroll"] = _logWindowAutoScroll = value; }
        }

        private Point? _logWindowLocation;
        [SettingDefault("300, 300")]
        public Point LogWindowLocation
        {
            get { return _logWindowLocation.HasValue ? _logWindowLocation.Value : (Point)this["LogWindowLocation"]; }
            set { this["LogWindowLocation"] = _logWindowLocation = value; }
        }

        private Size? _logWindowSize;
        [SettingDefault("595, 508")]
        public Size LogWindowSize
        {
            get { return _logWindowSize.HasValue ? _logWindowSize.Value : (Size)this["LogWindowSize"]; }
            set { this["LogWindowSize"] = _logWindowSize = value; }
        }

        private int _maxSamples;
        [SettingDefault("600")]
        public int MaxSamples
        {
            get { return _maxSamples; }
            set { this["MaxSamples"] = _maxSamples = value; }
        }

        private string _memoryListViewColumns;
        [SettingDefault("")]
        public string MemoryListViewColumns
        {
            get { return _memoryListViewColumns != null ? _memoryListViewColumns : (string)this["MemoryListViewColumns"]; }
            set { this["MemoryListViewColumns"] = _memoryListViewColumns = value; }
        }

        private Size? _memoryWindowSize;
        [SettingDefault("791, 503")]
        public Size MemoryWindowSize
        {
            get { return _memoryWindowSize.HasValue ? _memoryWindowSize.Value : (Size)this["MemoryWindowSize"]; }
            set { this["MemoryWindowSize"] = _memoryWindowSize = value; }
        }

        private string _moduleListViewColumns;
        [SettingDefault("")]
        public string ModuleListViewColumns
        {
            get { return _moduleListViewColumns != null ? _moduleListViewColumns : (string)this["ModuleListViewColumns"]; }
            set { this["ModuleListViewColumns"] = _moduleListViewColumns = value; }
        }

        private string _networkListViewColumns;
        [SettingDefault("")]
        public string NetworkListViewColumns
        {
            get { return _networkListViewColumns != null ? _networkListViewColumns : (string)this["NetworkListViewColumns"]; }
            set { this["NetworkListViewColumns"] = _networkListViewColumns = value; }
        }

        private bool? _newProcesses;
        [SettingDefault("False")]
        public bool NewProcesses
        {
            get { return _newProcesses.HasValue ? _newProcesses.Value : (bool)this["NewProcesses"]; }
            set { this["NewProcesses"] = _newProcesses = value; }
        }

        private bool? _newServices;
        [SettingDefault("True")]
        public bool NewServices
        {
            get { return _newServices.HasValue ? _newServices.Value : (bool)this["NewServices"]; }
            set { this["NewServices"] = _newServices = value; }
        }

        private string _peCoffHColumns;
        [SettingDefault("")]
        public string PECOFFHColumns
        {
            get { return _peCoffHColumns != null ? _peCoffHColumns : (string)this["PECOFFHColumns"]; }
            set { this["PECOFFHColumns"] = _peCoffHColumns = value; }
        }

        private string _peCoffOHColumns;
        [SettingDefault("")]
        public string PECOFFOHColumns
        {
            get { return _peCoffOHColumns != null ? _peCoffOHColumns : (string)this["PECOFFOHColumns"]; }
            set { this["PECOFFOHColumns"] = _peCoffOHColumns = value; }
        }

        private string _peExportsColumns;
        [SettingDefault("")]
        public string PEExportsColumns
        {
            get { return _peExportsColumns != null ? _peExportsColumns : (string)this["PEExportsColumns"]; }
            set { this["PEExportsColumns"] = _peExportsColumns = value; }
        }

        private string _peImageDataColumns;
        [SettingDefault("")]
        public string PEImageDataColumns
        {
            get { return _peImageDataColumns != null ? _peImageDataColumns : (string)this["PEImageDataColumns"]; }
            set { this["PEImageDataColumns"] = _peImageDataColumns = value; }
        }

        private string _peImportsColumns;
        [SettingDefault("")]
        public string PEImportsColumns
        {
            get { return _peImportsColumns != null ? _peImportsColumns : (string)this["PEImportsColumns"]; }
            set { this["PEImportsColumns"] = _peImportsColumns = value; }
        }

        private string _peSectionsColumns;
        [SettingDefault("")]
        public string PESectionsColumns
        {
            get { return _peSectionsColumns != null ? _peSectionsColumns : (string)this["PESectionsColumns"]; }
            set { this["PESectionsColumns"] = _peSectionsColumns = value; }
        }

        private Size? _peWindowSize;
        [SettingDefault("439, 413")]
        public Size PEWindowSize
        {
            get { return _peWindowSize.HasValue ? _peWindowSize.Value : (Size)this["PEWindowSize"]; }
            set { this["PEWindowSize"] = _peWindowSize = value; }
        }

        private bool? _physMemHistoryIconVisible;
        [SettingDefault("False")]
        public bool PhysMemHistoryIconVisible
        {
            get { return _physMemHistoryIconVisible.HasValue ? _physMemHistoryIconVisible.Value : (bool)this["PhysMemHistoryIconVisible"]; }
            set { this["PhysMemHistoryIconVisible"] = _physMemHistoryIconVisible = value; }
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
            get { return _plotterCPUKernelColor.HasValue ? _plotterCPUKernelColor.Value : (Color)this["PlotterCPUKernelColor"]; }
            set { this["PlotterCPUKernelColor"] = _plotterCPUKernelColor = value; }
        }

        private Color? _plotterCPUUserColor;
        [SettingDefault("Red")]
        public Color PlotterCPUUserColor
        {
            get { return _plotterCPUUserColor.HasValue ? _plotterCPUUserColor.Value : (Color)this["PlotterCPUUserColor"]; }
            set { this["PlotterCPUUserColor"] = _plotterCPUUserColor = value; }
        }

        private Color? _plotterIOROColor;
        [SettingDefault("Yellow")]
        public Color PlotterIOROColor
        {
            get { return _plotterIOROColor.HasValue ? _plotterIOROColor.Value : (Color)this["PlotterIOROColor"]; }
            set { this["PlotterIOROColor"] = _plotterIOROColor = value; }
        }

        private Color? _plotterIOWColor;
        [SettingDefault("DarkViolet")]
        public Color PlotterIOWColor
        {
            get { return _plotterIOWColor.HasValue ? _plotterIOWColor.Value : (Color)this["PlotterIOWColor"]; }
            set { this["PlotterIOWColor"] = _plotterIOWColor = value; }
        }

        private Color? _plotterMemoryPrivateColor;
        [SettingDefault("Orange")]
        public Color PlotterMemoryPrivateColor
        {
            get { return _plotterMemoryPrivateColor.HasValue ? _plotterMemoryPrivateColor.Value : (Color)this["PlotterMemoryPrivateColor"]; }
            set { this["PlotterMemoryPrivateColor"] = _plotterMemoryPrivateColor = value; }
        }

        private Color? _plotterMemoryWSColor;
        [SettingDefault("Cyan")]
        public Color PlotterMemoryWSColor
        {
            get { return _plotterMemoryWSColor.HasValue ? _plotterMemoryWSColor.Value : (Color)this["PlotterMemoryWSColor"]; }
            set { this["PlotterMemoryWSColor"] = _plotterMemoryWSColor = value; }
        }

        private int _plotterStep;
        [SettingDefault("2")]
        public int PlotterStep
        {
            get { return _plotterStep; }
            set { this["PlotterStep"] = _plotterStep = value; }
        }

        private string _privilegeListColumns;
        [SettingDefault("")]
        public string PrivilegeListColumns
        {
            get { return _privilegeListColumns != null ? _privilegeListColumns : (string)this["PrivilegeListColumns"]; }
            set { this["PrivilegeListColumns"] = _privilegeListColumns = value; }
        }

        private string _processTreeColumns;
        [SettingDefault("")]
        public string ProcessTreeColumns
        {
            get { return _processTreeColumns != null ? _processTreeColumns : (string)this["ProcessTreeColumns"]; }
            set { this["ProcessTreeColumns"] = _processTreeColumns = value; }
        }

        private Point? _processWindowLocation;
        [SettingDefault("200, 200")]
        public Point ProcessWindowLocation
        {
            get { return _processWindowLocation.HasValue ? _processWindowLocation.Value : (Point)this["ProcessWindowLocation"]; }
            set { this["ProcessWindowLocation"] = _processWindowLocation = value; }
        }

        private string _processWindowSelectedTab;
        [SettingDefault("tabGeneral")]
        public string ProcessWindowSelectedTab
        {
            get { return _processWindowSelectedTab != null ? _processWindowSelectedTab : (string)this["ProcessWindowSelectedTab"]; }
            set { this["ProcessWindowSelectedTab"] = _processWindowSelectedTab = value; }
        }

        private Size? _processWindowSize;
        [SettingDefault("505, 512")]
        public Size ProcessWindowSize
        {
            get { return _processWindowSize.HasValue ? _processWindowSize.Value : (Size)this["ProcessWindowSize"]; }
            set { this["ProcessWindowSize"] = _processWindowSize = value; }
        }

        private string _promptBoxText;
        [SettingDefault("")]
        public string PromptBoxText
        {
            get { return _promptBoxText != null ? _promptBoxText : (string)this["PromptBoxText"]; }
            set { this["PromptBoxText"] = _promptBoxText = value; }
        }

        private int _refreshInterval;
        [SettingDefault("1000")]
        public int RefreshInterval
        {
            get { return _refreshInterval; }
            set { this["RefreshInterval"] = _refreshInterval = value; }
        }

        private string _resultsListViewColumns;
        [SettingDefault("")]
        public string ResultsListViewColumns
        {
            get { return _resultsListViewColumns != null ? _resultsListViewColumns : (string)this["ResultsListViewColumns"]; }
            set { this["ResultsListViewColumns"] = _resultsListViewColumns = value; }
        }

        private Size? _resultsWindowSize;
        [SettingDefault("504, 482")]
        public Size ResultsWindowSize
        {
            get { return _resultsWindowSize.HasValue ? _resultsWindowSize.Value : (Size)this["ResultsWindowSize"]; }
            set { this["ResultsWindowSize"] = _resultsWindowSize = value; }
        }

        private string _runAsCommand;
        [SettingDefault("")]
        public string RunAsCommand
        {
            get { return _runAsCommand != null ? _runAsCommand : (string)this["RunAsCommand"]; }
            set { this["RunAsCommand"] = _runAsCommand = value; }
        }

        private string _runAsUsername;
        [SettingDefault("")]
        public string RunAsUsername
        {
            get { return _runAsUsername != null ? _runAsUsername : (string)this["RunAsUsername"]; }
            set { this["RunAsUsername"] = _runAsUsername = value; }
        }

        private bool? _scrollDownProcessTree;
        [SettingDefault("False")]
        public bool ScrollDownProcessTree
        {
            get { return _scrollDownProcessTree.HasValue ? _scrollDownProcessTree.Value : (bool)this["ScrollDownProcessTree"]; }
            set { this["ScrollDownProcessTree"] = _scrollDownProcessTree = value; }
        }

        private string _searchEngine;
        [SettingDefault("http://www.google.com/search?q=%s")]
        public string SearchEngine
        {
            get { return _searchEngine != null ? _searchEngine : (string)this["SearchEngine"]; }
            set { this["SearchEngine"] = _searchEngine = value; }
        }

        private string _searchType;
        [SettingDefault("&String Scan...")]
        public string SearchType
        {
            get { return _searchType != null ? _searchType : (string)this["SearchType"]; }
            set { this["SearchType"] = _searchType = value; }
        }

        private string _serviceListViewColumns;
        [SettingDefault("")]
        public string ServiceListViewColumns
        {
            get { return _serviceListViewColumns != null ? _serviceListViewColumns : (string)this["ServiceListViewColumns"]; }
            set { this["ServiceListViewColumns"] = _serviceListViewColumns = value; }
        }

        private string _serviceMiniListColumns;
        [SettingDefault("")]
        public string ServiceMiniListColumns
        {
            get { return _serviceMiniListColumns != null ? _serviceMiniListColumns : (string)this["ServiceMiniListColumns"]; }
            set { this["ServiceMiniListColumns"] = _serviceMiniListColumns = value; }
        }

        private bool _showAccountDomains;
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
            get { return _showOneGraphPerCPU.HasValue ? _showOneGraphPerCPU.Value : (bool)this["ShowOneGraphPerCPU"]; }
            set { this["ShowOneGraphPerCPU"] = _showOneGraphPerCPU = value; }
        }

        private bool? _startedServices;
        [SettingDefault("False")]
        public bool StartedServices
        {
            get { return _startedServices.HasValue ? _startedServices.Value : (bool)this["StartedServices"]; }
            set { this["StartedServices"] = _startedServices = value; }
        }

        private bool? _startHidden;
        [SettingDefault("False")]
        public bool StartHidden
        {
            get { return _startHidden.HasValue ? _startHidden.Value : (bool)this["StartHidden"]; }
            set { this["StartHidden"] = _startHidden = value; }
        }

        private bool? _stoppedServices;
        [SettingDefault("False")]
        public bool StoppedServices
        {
            get { return _stoppedServices.HasValue ? _stoppedServices.Value : (bool)this["StoppedServices"]; }
            set { this["StoppedServices"] = _stoppedServices = value; }
        }

        private Point? _sysInfoWindowLocation;
        [SettingDefault("100, 100")]
        public Point SysInfoWindowLocation
        {
            get { return _sysInfoWindowLocation.HasValue ? _sysInfoWindowLocation.Value : (Point)this["SysInfoWindowLocation"]; }
            set { this["SysInfoWindowLocation"] = _sysInfoWindowLocation = value; }
        }

        private Size? _sysInfoWindowSize;
        [SettingDefault("851, 577")]
        public Size SysInfoWindowSize
        {
            get { return _sysInfoWindowSize.HasValue ? _sysInfoWindowSize.Value : (Size)this["SysInfoWindowSize"]; }
            set { this["SysInfoWindowSize"] = _sysInfoWindowSize = value; }
        }

        private bool? _terminatedProcesses;
        [SettingDefault("False")]
        public bool TerminatedProcesses
        {
            get { return _terminatedProcesses.HasValue ? _terminatedProcesses.Value : (bool)this["TerminatedProcesses"]; }
            set { this["TerminatedProcesses"] = _terminatedProcesses = value; }
        }

        private string _threadListViewColumns;
        [SettingDefault("")]
        public string ThreadListViewColumns
        {
            get { return _threadListViewColumns != null ? _threadListViewColumns : (string)this["ThreadListViewColumns"]; }
            set { this["ThreadListViewColumns"] = _threadListViewColumns = value; }
        }

        private Size? _threadWindowSize;
        [SettingDefault("415, 503")]
        public Size ThreadWindowSize
        {
            get { return _threadWindowSize.HasValue ? _threadWindowSize.Value : (Size)this["ThreadWindowSize"]; }
            set { this["ThreadWindowSize"] = _threadWindowSize = value; }
        }

        private Size? _tokenWindowSize;
        [SettingDefault("481, 468")]
        public Size TokenWindowSize
        {
            get { return _tokenWindowSize.HasValue ? _tokenWindowSize.Value : (Size)this["TokenWindowSize"]; }
            set { this["TokenWindowSize"] = _tokenWindowSize = value; }
        }

        private string _tokenWindowTab;
        [SettingDefault("tabPrivileges")]
        public string TokenWindowTab
        {
            get { return _tokenWindowTab != null ? _tokenWindowTab : (string)this["TokenWindowTab"]; }
            set { this["TokenWindowTab"] = _tokenWindowTab = value; }
        }

        private bool? _toolbarVisible;
        [SettingDefault("False")]
        public bool ToolbarVisible
        {
            get { return _toolbarVisible.HasValue ? _toolbarVisible.Value : (bool)this["ToolbarVisible"]; }
            set { this["ToolbarVisible"] = _toolbarVisible = value; }
        }

        private int? _toolStripDisplayStyle;
        [SettingDefault("1")]
        public int ToolStripDisplayStyle
        {
            get { return _toolStripDisplayStyle.HasValue ? _toolStripDisplayStyle.Value : (int)this["ToolStripDisplayStyle"]; }
            set { this["ToolStripDisplayStyle"] = _toolStripDisplayStyle = value; }
        }

        private int _unitSpecifier;
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
            get { return _useColorDebuggedProcesses.HasValue ? _useColorDebuggedProcesses.Value : (bool)this["UseColorDebuggedProcesses"]; }
            set { this["UseColorDebuggedProcesses"] = _useColorDebuggedProcesses = value; }
        }

        private bool? _useColorDotNetProcesses;
        [SettingDefault("True")]
        public bool UseColorDotNetProcesses
        {
            get { return _useColorDotNetProcesses.HasValue ? _useColorDotNetProcesses.Value : (bool)this["UseColorDotNetProcesses"]; }
            set { this["UseColorDotNetProcesses"] = _useColorDotNetProcesses = value; }
        }

        private bool? _useColorElevatedProcesses;
        [SettingDefault("True")]
        public bool UseColorElevatedProcesses
        {
            get { return _useColorElevatedProcesses.HasValue ? _useColorElevatedProcesses.Value : (bool)this["UseColorElevatedProcesses"]; }
            set { this["UseColorElevatedProcesses"] = _useColorElevatedProcesses = value; }
        }

        private bool? _useColorGuiThreads;
        [SettingDefault("True")]
        public bool UseColorGuiThreads
        {
            get { return _useColorGuiThreads.HasValue ? _useColorGuiThreads.Value : (bool)this["UseColorGuiThreads"]; }
            set { this["UseColorGuiThreads"] = _useColorGuiThreads = value; }
        }

        private bool? _useColorInheritHandles;
        [SettingDefault("True")]
        public bool UseColorInheritHandles
        {
            get { return _useColorInheritHandles.HasValue ? _useColorInheritHandles.Value : (bool)this["UseColorInheritHandles"]; }
            set { this["UseColorInheritHandles"] = _useColorInheritHandles = value; }
        }

        private bool? _useColorJobProcesses;
        [SettingDefault("True")]
        public bool UseColorJobProcesses
        {
            get { return _useColorJobProcesses.HasValue ? _useColorJobProcesses.Value : (bool)this["UseColorJobProcesses"]; }
            set { this["UseColorJobProcesses"] = _useColorJobProcesses = value; }
        }

        private bool? _useColorOwnProcesses;
        [SettingDefault("True")]
        public bool UseColorOwnProcesses
        {
            get { return _useColorOwnProcesses.HasValue ? _useColorOwnProcesses.Value : (bool)this["UseColorOwnProcesses"]; }
            set { this["UseColorOwnProcesses"] = _useColorOwnProcesses = value; }
        }

        private bool? _useColorPackedProcesses;
        [SettingDefault("True")]
        public bool UseColorPackedProcesses
        {
            get { return _useColorPackedProcesses.HasValue ? _useColorPackedProcesses.Value : (bool)this["UseColorPackedProcesses"]; }
            set { this["UseColorPackedProcesses"] = _useColorPackedProcesses = value; }
        }

        private bool? _useColorPosixProcesses;
        [SettingDefault("True")]
        public bool UseColorPosixProcesses
        {
            get { return _useColorPosixProcesses.HasValue ? _useColorPosixProcesses.Value : (bool)this["UseColorPosixProcesses"]; }
            set { this["UseColorPosixProcesses"] = _useColorPosixProcesses = value; }
        }

        private bool? _useColorProtectedHandles;
        [SettingDefault("True")]
        public bool UseColorProtectedHandles
        {
            get { return _useColorProtectedHandles.HasValue ? _useColorProtectedHandles.Value : (bool)this["UseColorProtectedHandles"]; }
            set { this["UseColorProtectedHandles"] = _useColorProtectedHandles = value; }
        }

        private bool? _useColorRelocatedDlls;
        [SettingDefault("True")]
        public bool UseColorRelocatedDlls
        {
            get { return _useColorRelocatedDlls.HasValue ? _useColorRelocatedDlls.Value : (bool)this["UseColorRelocatedDlls"]; }
            set { this["UseColorRelocatedDlls"] = _useColorRelocatedDlls = value; }
        }

        private bool? _useColorServiceProcesses;
        [SettingDefault("True")]
        public bool UseColorServiceProcesses
        {
            get { return _useColorServiceProcesses.HasValue ? _useColorServiceProcesses.Value : (bool)this["UseColorServiceProcesses"]; }
            set { this["UseColorServiceProcesses"] = _useColorServiceProcesses = value; }
        }

        private bool? _useColorSuspended;
        [SettingDefault("True")]
        public bool UseColorSuspended
        {
            get { return _useColorSuspended.HasValue ? _useColorSuspended.Value : (bool)this["UseColorSuspended"]; }
            set { this["UseColorSuspended"] = _useColorSuspended = value; }
        }

        private bool? _useColorSystemProcesses;
        [SettingDefault("True")]
        public bool UseColorSystemProcesses
        {
            get { return _useColorSystemProcesses.HasValue ? _useColorSystemProcesses.Value : (bool)this["UseColorSystemProcesses"]; }
            set { this["UseColorSystemProcesses"] = _useColorSystemProcesses = value; }
        }

        private bool? _useColorWow64Processes;
        [SettingDefault("True")]
        public bool UseColorWow64Processes
        {
            get { return _useColorWow64Processes.HasValue ? _useColorWow64Processes.Value : (bool)this["UseColorWow64Processes"]; }
            set { this["UseColorWow64Processes"] = _useColorWow64Processes = value; }
        }

        private bool? _verifySignatures;
        [SettingDefault("False")]
        public bool VerifySignatures
        {
            get { return _verifySignatures.HasValue ? _verifySignatures.Value : (bool)this["VerifySignatures"]; }
            set { this["VerifySignatures"] = _verifySignatures = value; }
        }

        private bool _warnDangerous;
        [SettingDefault("True")]
        public bool WarnDangerous
        {
            get { return _warnDangerous; }
            set { this["WarnDangerous"] = _warnDangerous = value; }
        }

        private Point? _windowLocation;
        [SettingDefault("200, 200")]
        public Point WindowLocation
        {
            get { return _windowLocation.HasValue ? _windowLocation.Value : (Point)this["WindowLocation"]; }
            set { this["WindowLocation"] = _windowLocation = value; }
        }

        private Size? _windowSize;
        [SettingDefault("844, 550")]
        public Size WindowSize
        {
            get { return _windowSize.HasValue ? _windowSize.Value : (Size)this["WindowSize"]; }
            set { this["WindowSize"] = _windowSize = value; }
        }

        private FormWindowState? _windowState;
        [SettingDefault("Normal")]
        public FormWindowState WindowState
        {
            get { return _windowState.HasValue ? _windowState.Value : (FormWindowState)this["WindowState"]; }
            set { this["WindowState"] = _windowState = value; }
        }
    }
}
