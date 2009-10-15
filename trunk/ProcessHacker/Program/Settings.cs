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
