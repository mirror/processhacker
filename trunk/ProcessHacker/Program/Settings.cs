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

        protected override void Invalidate()
        {
            _refreshInterval = (int)this["RefreshInterval"];
        }

        private bool? _alwaysOnTop;
        [SettingDefault("False")]
        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop.HasValue ? _alwaysOnTop.Value : (bool)this["AlwaysOnTop"]; }
            set { this["AlwaysOnTop"] = _alwaysOnTop = value; }
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

        private Color? _colorElevatedProcesses;
        [SettingDefault("255, 170, 0")]
        public Color ColorElevatedProcesses
        {
            get { return _colorElevatedProcesses.HasValue ? _colorElevatedProcesses.Value : (Color)this["ColorElevatedProcesses"]; }
            set { this["ColorElevatedProcesses"] = _colorElevatedProcesses = value; }
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

        private Color? _colorSystemProcesses;
        [SettingDefault("170, 204, 255")]
        public Color ColorSystemProcesses
        {
            get { return _colorSystemProcesses.HasValue ? _colorSystemProcesses.Value : (Color)this["ColorSystemProcesses"]; }
            set { this["ColorSystemProcesses"] = _colorSystemProcesses = value; }
        }

        private bool? _deletedServices;
        [SettingDefault("True")]
        public bool DeletedServices
        {
            get { return _deletedServices.HasValue ? _deletedServices.Value : (bool)this["DeletedServices"]; }
            set { this["DeletedServices"] = _deletedServices = value; }
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

        private bool _showAccountDomains;
        [SettingDefault("False")]
        public bool ShowAccountDomains
        {
            get { return _showAccountDomains; }
            set { this["ShowAccountDomains"] = _showAccountDomains = value; }
        }

        private bool? _startedServices;
        [SettingDefault("False")]
        public bool StartedServices
        {
            get { return _startedServices.HasValue ? _startedServices.Value : (bool)this["StartedServices"]; }
            set { this["StartedServices"] = _startedServices = value; }
        }

        private bool? _stoppedServices;
        [SettingDefault("False")]
        public bool StoppedServices
        {
            get { return _stoppedServices.HasValue ? _stoppedServices.Value : (bool)this["StoppedServices"]; }
            set { this["StoppedServices"] = _stoppedServices = value; }
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


        #region UnSorted - Awaiting Go ahead

        private Color? _ColorSuspended;
        [SettingDefault("Silver")]
        public Color ColorSuspended
        {
            get { return _ColorSuspended.HasValue ? _ColorSuspended.Value : (Color)this["ColorSuspended"]; }
            set { this["ColorSuspended"] = _ColorSuspended = value; }
        }

        private Color? _ColorGuiThreads;
        [SettingDefault("255, 255, 128")]
        public Color ColorGuiThreads
        {
            get { return _ColorGuiThreads.HasValue ? _ColorGuiThreads.Value : (Color)this["ColorGuiThreads"]; }
            set { this["ColorGuiThreads"] = _ColorGuiThreads = value; }
        }

        private Color? _ColorProtectedHandles;
        [SettingDefault("Gray")]
        public Color ColorProtectedHandles
        {
            get { return _ColorProtectedHandles.HasValue ? _ColorProtectedHandles.Value : (Color)this["ColorProtectedHandles"]; }
            set { this["ColorProtectedHandles"] = _ColorProtectedHandles = value; }
        }

        private Color? _ColorInheritHandles;
        [SettingDefault("128, 255, 255")]
        public Color ColorInheritHandles
        {
            get { return _ColorInheritHandles.HasValue ? _ColorInheritHandles.Value : (Color)this["ColorInheritHandles"]; }
            set { this["ColorInheritHandles"] = _ColorInheritHandles = value; }
        }

        private Color? _ColorPackedProcesses;
        [SettingDefault("DeepPink")]
        public Color ColorPackedProcesses
        {
            get { return _ColorPackedProcesses.HasValue ? _ColorPackedProcesses.Value : (Color)this["ColorPackedProcesses"]; }
            set { this["ColorPackedProcesses"] = _ColorPackedProcesses = value; }
        }

        private Color? _ColorJobProcesses;
        [SettingDefault("Peru")]
        public Color ColorJobProcesses
        {
            get { return _ColorJobProcesses.HasValue ? _ColorJobProcesses.Value : (Color)this["ColorJobProcesses"]; }
            set { this["ColorJobProcesses"] = _ColorJobProcesses = value; }
        }

        private Color? _ColorWow64Processes;
        [SettingDefault("DeepPink")]
        public Color ColorWow64Processes
        {
            get { return _ColorWow64Processes.HasValue ? _ColorWow64Processes.Value : (Color)this["ColorWow64Processes"]; }
            set { this["ColorWow64Processes"] = _ColorWow64Processes = value; }
        }

        private Color? _ColorDotNetProcesses;
        [SettingDefault("222, 255, 0")]
        public Color ColorDotNetProcesses
        {
            get { return _ColorDotNetProcesses.HasValue ? _ColorDotNetProcesses.Value : (Color)this["ColorDotNetProcesses"]; }
            set { this["ColorDotNetProcesses"] = _ColorDotNetProcesses = value; }
        }

        private Color? _ColorPosixProcesses;
        [SettingDefault("DarkSlateBlue")]
        public Color ColorPosixProcesses
        {
            get { return _ColorPosixProcesses.HasValue ? _ColorPosixProcesses.Value : (Color)this["ColorPosixProcesses"]; }
            set { this["ColorPosixProcesses"] = _ColorPosixProcesses = value; }
        }

        private Color? _ColorRelocatedDlls;
        [SettingDefault("255, 192, 128")]
        public Color ColorRelocatedDlls
        {
            get { return _ColorRelocatedDlls.HasValue ? _ColorRelocatedDlls.Value : (Color)this["ColorRelocatedDlls"]; }
            set { this["ColorRelocatedDlls"] = _ColorRelocatedDlls = value; }
        }

        private Font _Font;
        [SettingDefault("Microsoft Sans Serif, 8.25pt")]
        public Font Font
        {
            get { return _Font != null ? _Font : (Font)this["Font"]; }
            set { this["Font"] = _Font = value; }
        }

        private string _HiddenProcessesColumns;
        [SettingDefault("")]
        public string HiddenProcessesColumns
        {
            get { return _HiddenProcessesColumns != null ? _HiddenProcessesColumns : (string)this["HiddenProcessesColumns"]; }
            set { this["HiddenProcessesColumns"] = _HiddenProcessesColumns = value; }
        }

        private string _appUpdateUrl;
        [SettingDefault("http://processhacker.sourceforge.net/AppUpdate.xml")]
        public string AppUpdateUrl
        {
            get { return _appUpdateUrl != null ? _appUpdateUrl : (string)this["AppUpdateUrl"]; }
            set { this["AppUpdateUrl"] = _appUpdateUrl = value; }
        }

        private string _IPInfoWhoIsListViewColumns;
        [SettingDefault("0,410")]
        public string IPInfoWhoIsListViewColumns
        {
            get { return _IPInfoWhoIsListViewColumns != null ? _IPInfoWhoIsListViewColumns : (string)this["IPInfoWhoIsListViewColumns"]; }
            set { this["IPInfoWhoIsListViewColumns"] = _IPInfoWhoIsListViewColumns = value; }
        }

        private string _IPInfoTracertListViewColumns;
        [SettingDefault("0,30|1,60|2,100|3,200")]
        public string IPInfoTracertListViewColumns
        {
            get { return _IPInfoTracertListViewColumns != null ? _IPInfoTracertListViewColumns : (string)this["IPInfoTracertListViewColumns"]; }
            set { this["IPInfoTracertListViewColumns"] = _IPInfoTracertListViewColumns = value; }
        }

        private string _IPInfoPingListViewColumns;
        [SettingDefault("0,30|1,60|2,100|3,200")]
        public string IPInfoPingListViewColumns
        {
            get { return _IPInfoPingListViewColumns != null ? _IPInfoPingListViewColumns : (string)this["IPInfoPingListViewColumns"]; }
            set { this["IPInfoPingListViewColumns"] = _IPInfoPingListViewColumns = value; }
        }

        private string _NetworkListViewColumns;
        [SettingDefault("0,137|1,160|2,71|3,195|4,75|5,80|6,70")]
        public string NetworkListViewColumns
        {
            get { return _NetworkListViewColumns != null ? _NetworkListViewColumns : (string)this["NetworkListViewColumns"]; }
            set { this["NetworkListViewColumns"] = _NetworkListViewColumns = value; }
        }

        private string _ImposterNames;
        [SettingDefault("audiodg.exe, csrss.exe, dwm.exe, explorer.exe, logonui.exe, lsass.exe, lsm.exe, ntkrnlpa.exe, ntoskrnl.exe, procexp.exe, rundll32.exe, services.exe, smss.exe, spoolsv.exe, svchost.exe, taskeng.exe, taskmgr.exe, wininit.exe, winlogon.exe")]
        public string ImposterNames
        {
            get { return _ImposterNames != null ? _ImposterNames : (string)this["ImposterNames"]; }
            set { this["ImposterNames"] = _ImposterNames = value; }
        }

        private string _DbgHelpSearchPath;
        [SettingDefault("")]
        public string DbgHelpSearchPath
        {
            get { return _DbgHelpSearchPath != null ? _DbgHelpSearchPath : (string)this["DbgHelpSearchPath"]; }
            set { this["DbgHelpSearchPath"] = _DbgHelpSearchPath = value; }
        }

        private string _DbgHelpPath;
        [SettingDefault("dbghelp.dll")]
        public string DbgHelpPath
        {
            get { return _DbgHelpPath != null ? _DbgHelpPath : (string)this["DbgHelpPath"]; }
            set { this["DbgHelpPath"] = _DbgHelpPath = value; }
        }

        private string _EnvironmentListViewColumns;
        [SettingDefault("")]
        public string EnvironmentListViewColumns
        {
            get { return _EnvironmentListViewColumns != null ? _EnvironmentListViewColumns : (string)this["EnvironmentListViewColumns"]; }
            set { this["EnvironmentListViewColumns"] = _EnvironmentListViewColumns = value; }
        }

        private string _ServiceMiniListColumns;
        [SettingDefault("")]
        public string ServiceMiniListColumns
        {
            get { return _ServiceMiniListColumns != null ? _ServiceMiniListColumns : (string)this["ServiceMiniListColumns"]; }
            set { this["ServiceMiniListColumns"] = _ServiceMiniListColumns = value; }
        }

        private int _ElevationLevel;
        [SettingDefault("1")]
        public int ElevationLevel
        {
            get { return _ElevationLevel; }
            set { this["ElevationLevel"] = _ElevationLevel = value; }
        }

        private int _ToolStripDisplayStyle;
        [SettingDefault("1")]
        public int ToolStripDisplayStyle
        {
            get { return _ToolStripDisplayStyle; }
            set { this["ToolStripDisplayStyle"] = _ToolStripDisplayStyle = value; }
        }

        private int _PlotterStep;
        [SettingDefault("2")]
        public int PlotterStep
        {
            get { return _PlotterStep; }
            set { this["PlotterStep"] = _PlotterStep = value; }
        }

        private int _UnitSpecifier;
        [SettingDefault("6")]
        public int UnitSpecifier
        {
            get { return _UnitSpecifier; }
            set { this["UnitSpecifier"] = _UnitSpecifier = value; }
        }

        private int _MaxSamples;
        [SettingDefault("600")]
        public int MaxSamples
        {
            get { return _MaxSamples; }
            set { this["MaxSamples"] = _MaxSamples = value; }
        }

        private int _AppUpdateLevel;
        [SettingDefault("1")]
        public int AppUpdateLevel
        {
            get { return _AppUpdateLevel; }
            set { this["AppUpdateLevel"] = _AppUpdateLevel = value; }
        }

        private bool? _AppUpdateAutomatic;
        [SettingDefault("True")]
        public bool AppUpdateAutomatic
        {
            get { return _AppUpdateAutomatic.HasValue ? _AppUpdateAutomatic.Value : (bool)this["AppUpdateAutomatic"]; }
            set { this["AppUpdateAutomatic"] = _AppUpdateAutomatic = value; }
        }

        private bool? _UseColorSuspended;
        [SettingDefault("True")]
        public bool UseColorSuspended
        {
            get { return _UseColorSuspended.HasValue ? _UseColorSuspended.Value : (bool)this["UseColorSuspended"]; }
            set { this["UseColorSuspended"] = _UseColorSuspended = value; }
        }

        private bool? _UseColorGuiThreads;
        [SettingDefault("True")]
        public bool UseColorGuiThreads
        {
            get { return _UseColorGuiThreads.HasValue ? _UseColorGuiThreads.Value : (bool)this["UseColorGuiThreads"]; }
            set { this["UseColorGuiThreads"] = _UseColorGuiThreads = value; }
        }

        private bool? _UseColorServiceProcesses;
        [SettingDefault("True")]
        public bool UseColorServiceProcesses
        {
            get { return _UseColorServiceProcesses.HasValue ? _UseColorServiceProcesses.Value : (bool)this["UseColorServiceProcesses"]; }
            set { this["UseColorServiceProcesses"] = _UseColorServiceProcesses = value; }
        }

        private bool? _UseColorSystemProcesses;
        [SettingDefault("True")]
        public bool UseColorSystemProcesses
        {
            get { return _UseColorSystemProcesses.HasValue ? _UseColorSystemProcesses.Value : (bool)this["UseColorSystemProcesses"]; }
            set { this["UseColorSystemProcesses"] = _UseColorSystemProcesses = value; }
        }

        private bool? _UseColorProtectedHandles;
        [SettingDefault("True")]
        public bool UseColorProtectedHandles
        {
            get { return _UseColorProtectedHandles.HasValue ? _UseColorProtectedHandles.Value : (bool)this["UseColorProtectedHandles"]; }
            set { this["UseColorProtectedHandles"] = _UseColorProtectedHandles = value; }
        }

        private bool? _UseColorInheritHandles;
        [SettingDefault("True")]
        public bool UseColorInheritHandles
        {
            get { return _UseColorInheritHandles.HasValue ? _UseColorInheritHandles.Value : (bool)this["UseColorInheritHandles"]; }
            set { this["UseColorInheritHandles"] = _UseColorInheritHandles = value; }
        }

        private bool? _UseColorRelocatedDlls;
        [SettingDefault("True")]
        public bool UseColorRelocatedDlls
        {
            get { return _UseColorRelocatedDlls.HasValue ? _UseColorRelocatedDlls.Value : (bool)this["UseColorRelocatedDlls"]; }
            set { this["UseColorRelocatedDlls"] = _UseColorRelocatedDlls = value; }
        }
                
        private int _IconMenuProcessCount;
        [SettingDefault("10")]
        public int IconMenuProcessCount
        {
            get { return _IconMenuProcessCount; }
            set { this["IconMenuProcessCount"] = _IconMenuProcessCount = value; }
        }

        private bool? _NeedsUpgrade;
        [SettingDefault("True")]
        public bool NeedsUpgrade
        {
            get { return _NeedsUpgrade.HasValue ? _NeedsUpgrade.Value : (bool)this["NeedsUpgrade"]; }
            set { this["NeedsUpgrade"] = _NeedsUpgrade = value; }
        }

        private bool? _UseColorWow64Processes;
        [SettingDefault("True")]
        public bool UseColorWow64Processes
        {
            get { return _UseColorWow64Processes.HasValue ? _UseColorWow64Processes.Value : (bool)this["UseColorWow64Processes"]; }
            set { this["UseColorWow64Processes"] = _UseColorWow64Processes = value; }
        }

        private bool? _UseColorJobProcesses;
        [SettingDefault("True")]
        public bool UseColorJobProcesses
        {
            get { return _UseColorJobProcesses.HasValue ? _UseColorJobProcesses.Value : (bool)this["UseColorJobProcesses"]; }
            set { this["UseColorJobProcesses"] = _UseColorJobProcesses = value; }
        }

        private bool? _CpuHistoryIconVisible;
        [SettingDefault("True")]
        public bool CpuHistoryIconVisible
        {
            get { return _CpuHistoryIconVisible.HasValue ? _CpuHistoryIconVisible.Value : (bool)this["CpuHistoryIconVisible"]; }
            set { this["CpuHistoryIconVisible"] = _CpuHistoryIconVisible = value; }
        }

        private bool? _UseColorDebuggedProcesses;
        [SettingDefault("True")]
        public bool UseColorDebuggedProcesses
        {
            get { return _UseColorDebuggedProcesses.HasValue ? _UseColorDebuggedProcesses.Value : (bool)this["UseColorDebuggedProcesses"]; }
            set { this["UseColorDebuggedProcesses"] = _UseColorDebuggedProcesses = value; }
        }

        private bool? _UseColorElevatedProcesses;
        [SettingDefault("True")]
        public bool UseColorElevatedProcesses
        {
            get { return _UseColorElevatedProcesses.HasValue ? _UseColorElevatedProcesses.Value : (bool)this["UseColorElevatedProcesses"]; }
            set { this["UseColorElevatedProcesses"] = _UseColorElevatedProcesses = value; }
        }

        private bool? _UseColorOwnProcesses;
        [SettingDefault("True")]
        public bool UseColorOwnProcesses
        {
            get { return _UseColorOwnProcesses.HasValue ? _UseColorOwnProcesses.Value : (bool)this["UseColorOwnProcesses"]; }
            set { this["UseColorOwnProcesses"] = _UseColorOwnProcesses = value; }
        }

        private bool? _UseColorPosixProcesses;
        [SettingDefault("True")]
        public bool UseColorPosixProcesses
        {
            get { return _UseColorPosixProcesses.HasValue ? _UseColorPosixProcesses.Value : (bool)this["UseColorPosixProcesses"]; }
            set { this["UseColorPosixProcesses"] = _UseColorPosixProcesses = value; }
        }

        private bool? _FirstRun;
        [SettingDefault("True")]
        public bool FirstRun
        {
            get { return _FirstRun.HasValue ? _FirstRun.Value : (bool)this["FirstRun"]; }
            set { this["FirstRun"] = _FirstRun = value; }
        }

        private bool? _HideHandlesWithNoName;
        [SettingDefault("True")]
        public bool HideHandlesWithNoName
        {
            get { return _HideHandlesWithNoName.HasValue ? _HideHandlesWithNoName.Value : (bool)this["HideHandlesWithNoName"]; }
            set { this["HideHandlesWithNoName"] = _HideHandlesWithNoName = value; }
        }

        private bool? _UseColorPackedProcesses;
        [SettingDefault("True")]
        public bool UseColorPackedProcesses
        {
            get { return _UseColorPackedProcesses.HasValue ? _UseColorPackedProcesses.Value : (bool)this["UseColorPackedProcesses"]; }
            set { this["UseColorPackedProcesses"] = _UseColorPackedProcesses = value; }
        }

        private bool? _FloatChildWindows;
        [SettingDefault("True")]
        public bool FloatChildWindows
        {
            get { return _FloatChildWindows.HasValue ? _FloatChildWindows.Value : (bool)this["FloatChildWindows"]; }
            set { this["FloatChildWindows"] = _FloatChildWindows = value; }
        }

        private bool? _UseColorDotNetProcesses;
        [SettingDefault("True")]
        public bool UseColorDotNetProcesses
        {
            get { return _UseColorDotNetProcesses.HasValue ? _UseColorDotNetProcesses.Value : (bool)this["UseColorDotNetProcesses"]; }
            set { this["UseColorDotNetProcesses"] = _UseColorDotNetProcesses = value; }
        }

        private bool? _HideProcessHackerNetworkConnections;
        [SettingDefault("True")]
        public bool HideProcessHackerNetworkConnections
        {
            get { return _HideProcessHackerNetworkConnections.HasValue ? _HideProcessHackerNetworkConnections.Value : (bool)this["HideProcessHackerNetworkConnections"]; }
            set { this["HideProcessHackerNetworkConnections"] = _HideProcessHackerNetworkConnections = value; }
        }

        private bool? _EnableExperimentalFeatures;
        [SettingDefault("False")]
        public bool EnableExperimentalFeatures
        {
            get { return _EnableExperimentalFeatures.HasValue ? _EnableExperimentalFeatures.Value : (bool)this["EnableExperimentalFeatures"]; }
            set { this["AllowOnlyOneInstance"] = _EnableExperimentalFeatures = value; }
        }

        private bool? _AllowOnlyOneInstance;
        [SettingDefault("False")]
        public bool AllowOnlyOneInstance
        {
            get { return _AllowOnlyOneInstance.HasValue ? _AllowOnlyOneInstance.Value : (bool)this["AllowOnlyOneInstance"]; }
            set { this["AllowOnlyOneInstance"] = _AllowOnlyOneInstance = value; }
        }

        private bool? _StartHidden;
        [SettingDefault("False")]
        public bool StartHidden
        {
            get { return _StartHidden.HasValue ? _StartHidden.Value : (bool)this["StartHidden"]; }
            set { this["StartHidden"] = _StartHidden = value; }
        }

        private bool? _IoHistoryIconVisible;
        [SettingDefault("False")]
        public bool IoHistoryIconVisible
        {
            get { return _IoHistoryIconVisible.HasValue ? _IoHistoryIconVisible.Value : (bool)this["IoHistoryIconVisible"]; }
            set { this["IoHistoryIconVisible"] = _IoHistoryIconVisible = value; }
        }

        private bool? _CommitHistoryIconVisible;
        [SettingDefault("False")]
        public bool CommitHistoryIconVisible
        {
            get { return _CommitHistoryIconVisible.HasValue ? _CommitHistoryIconVisible.Value : (bool)this["CommitHistoryIconVisible"]; }
            set { this["CommitHistoryIconVisible"] = _CommitHistoryIconVisible = value; }
        }

        private bool? _PhysMemHistoryIconVisible;
        [SettingDefault("False")]
        public bool PhysMemHistoryIconVisible
        {
            get { return _PhysMemHistoryIconVisible.HasValue ? _PhysMemHistoryIconVisible.Value : (bool)this["PhysMemHistoryIconVisible"]; }
            set { this["PhysMemHistoryIconVisible"] = _PhysMemHistoryIconVisible = value; }
        }

        private bool? _ToolbarVisible;
        [SettingDefault("False")]
        public bool ToolbarVisible
        {
            get { return _ToolbarVisible.HasValue ? _ToolbarVisible.Value : (bool)this["ToolbarVisible"]; }
            set { this["ToolbarVisible"] = _ToolbarVisible = value; }
        }

        private bool? _ShowOneGraphPerCPU;
        [SettingDefault("False")]
        public bool ShowOneGraphPerCPU
        {
            get { return _ShowOneGraphPerCPU.HasValue ? _ShowOneGraphPerCPU.Value : (bool)this["ShowOneGraphPerCPU"]; }
            set { this["ShowOneGraphPerCPU"] = _ShowOneGraphPerCPU = value; }
        }

        private bool? _ScrollDownProcessTree;
        [SettingDefault("False")]
        public bool ScrollDownProcessTree
        {
            get { return _ScrollDownProcessTree.HasValue ? _ScrollDownProcessTree.Value : (bool)this["ScrollDownProcessTree"]; }
            set { this["ScrollDownProcessTree"] = _ScrollDownProcessTree = value; }
        }

        private bool? _CpuUsageIconVisible;
        [SettingDefault("False")]
        public bool CpuUsageIconVisible
        {
            get { return _CpuUsageIconVisible.HasValue ? _CpuUsageIconVisible.Value : (bool)this["CpuUsageIconVisible"]; }
            set { this["CpuUsageIconVisible"] = _CpuUsageIconVisible = value; }
        }

        private bool? _VerifySignatures;
        [SettingDefault("False")]
        public bool VerifySignatures
        {
            get { return _VerifySignatures.HasValue ? _VerifySignatures.Value : (bool)this["VerifySignatures"]; }
            set { this["VerifySignatures"] = _VerifySignatures = value; }
        }

        private bool? _HideWhenClosed;
        [SettingDefault("False")]
        public bool HideWhenClosed
        {
            get { return _HideWhenClosed.HasValue ? _HideWhenClosed.Value : (bool)this["HideWhenClosed"]; }
            set { this["HideWhenClosed"] = _HideWhenClosed = value; }
        }

        private bool? _EnableKPH;
        [SettingDefault("True")]
        public bool EnableKPH
        {
            get { return _EnableKPH.HasValue ? _EnableKPH.Value : (bool)this["EnableKPH"]; }
            set { this["EnableKPH"] = _EnableKPH = value; }
        }

        private bool? _DbgHelpUndecorate;
        [SettingDefault("True")]
        public bool DbgHelpUndecorate
        {
            get { return _DbgHelpUndecorate.HasValue ? _DbgHelpUndecorate.Value : (bool)this["DbgHelpUndecorate"]; }
            set { this["DbgHelpUndecorate"] = _DbgHelpUndecorate = value; }
        }

        private bool? _DbgHelpWarningShown;
        [SettingDefault("False")]
        public bool DbgHelpWarningShown
        {
            get { return _DbgHelpWarningShown.HasValue ? _DbgHelpWarningShown.Value : (bool)this["DbgHelpWarningShown"]; }
            set { this["DbgHelpWarningShown"] = _DbgHelpWarningShown = value; }
        }

        private bool? _LogWindowAutoScroll;
        [SettingDefault("False")]
        public bool LogWindowAutoScroll
        {
            get { return _LogWindowAutoScroll.HasValue ? _LogWindowAutoScroll.Value : (bool)this["LogWindowAutoScroll"]; }
            set { this["LogWindowAutoScroll"] = _LogWindowAutoScroll = value; }
        }

        private Size? _InformationBoxSize;
        [SettingDefault("565, 377")]
        public Size InformationBoxSize
        {
            get { return _InformationBoxSize.HasValue ? _InformationBoxSize.Value : (Size)this["InformationBoxSize"]; }
            set { this["InformationBoxSize"] = _InformationBoxSize = value; }
        }

        private Size? _SysInfoWindowSize;
        [SettingDefault("851, 577")]
        public Size SysInfoWindowSize
        {
            get { return _SysInfoWindowSize.HasValue ? _SysInfoWindowSize.Value : (Size)this["SysInfoWindowSize"]; }
            set { this["SysInfoWindowSize"] = _SysInfoWindowSize = value; }
        }

        private Size? _LogWindowSize;
        [SettingDefault("595, 508")]
        public Size LogWindowSize
        {
            get { return _LogWindowSize.HasValue ? _LogWindowSize.Value : (Size)this["LogWindowSize"]; }
            set { this["LogWindowSize"] = _LogWindowSize = value; }
        }

        private Size? _HiddenProcessesWindowSize;
        [SettingDefault("527, 429")]
        public Size HiddenProcessesWindowSize
        {
            get { return _HiddenProcessesWindowSize.HasValue ? _HiddenProcessesWindowSize.Value : (Size)this["HiddenProcessesWindowSize"]; }
            set { this["HiddenProcessesWindowSize"] = _HiddenProcessesWindowSize = value; }
        }

        private Point? _SysInfoWindowLocation;
        [SettingDefault("100, 100")]
        public Point SysInfoWindowLocation
        {
            get { return _SysInfoWindowLocation.HasValue ? _SysInfoWindowLocation.Value : (Point)this["SysInfoWindowLocation"]; }
            set { this["SysInfoWindowLocation"] = _SysInfoWindowLocation = value; }
        }

        private Point? _HiddenProcessesWindowLocation;
        [SettingDefault("200, 200")]
        public Point HiddenProcessesWindowLocation
        {
            get { return _HiddenProcessesWindowLocation.HasValue ? _HiddenProcessesWindowLocation.Value : (Point)this["HiddenProcessesWindowLocation"]; }
            set { this["HiddenProcessesWindowLocation"] = _HiddenProcessesWindowLocation = value; }
        }

        private Point? _LogWindowLocation;
        [SettingDefault("300, 300")]
        public Point LogWindowLocation
        {
            get { return _LogWindowLocation.HasValue ? _LogWindowLocation.Value : (Point)this["LogWindowLocation"]; }
            set { this["LogWindowLocation"] = _LogWindowLocation = value; }
        }

        private Point? _HandleFilterWindowLocation;
        [SettingDefault("200, 200")]
        public Point HandleFilterWindowLocation
        {
            get { return _HandleFilterWindowLocation.HasValue ? _HandleFilterWindowLocation.Value : (Point)this["HandleFilterWindowLocation"]; }
            set { this["HandleFilterWindowLocation"] = _HandleFilterWindowLocation = value; }
        }

        private Point? _ProcessWindowLocation;
        [SettingDefault("200, 200")]
        public Point ProcessWindowLocation
        {
            get { return _ProcessWindowLocation.HasValue ? _ProcessWindowLocation.Value : (Point)this["ProcessWindowLocation"]; }
            set { this["ProcessWindowLocation"] = _ProcessWindowLocation = value; }
        }

        #endregion

    }
}
