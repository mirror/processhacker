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

        private string _processTreeColumns;
        [SettingDefault("")]
        public string ProcessTreeColumns
        {
            get { return _processTreeColumns != null ? _processTreeColumns : (string)this["ProcessTreeColumns"]; }
            set { this["ProcessTreeColumns"] = _processTreeColumns = value; }
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

        private string _threadListViewColumns;
        [SettingDefault("")]
        public string ThreadListViewColumns
        {
            get { return _threadListViewColumns != null ? _threadListViewColumns : (string)this["ThreadListViewColumns"]; }
            set { this["ThreadListViewColumns"] = _threadListViewColumns = value; }
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
