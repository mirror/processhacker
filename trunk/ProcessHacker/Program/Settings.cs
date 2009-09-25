using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public static class Settings
    {
        public static void Refresh()
        {
            RefreshInterval = Properties.Settings.Default.RefreshInterval;
            ShowAccountDomains = Properties.Settings.Default.ShowAccountDomains;

            #region "Proxy Settings"
            UseProxy = Properties.Settings.Default.ProxyUse;
            BypassProxyOnLocal = Properties.Settings.Default.ProxyBypassOnLocal;
            ProxyAddress = Properties.Settings.Default.ProxyAddress;
            ProxyUsername = Properties.Settings.Default.ProxyUsername;
            ProxyPassword = Properties.Settings.Default.ProxyPassword;
            #endregion
        }

        private static int _refreshInterval;
        public static int RefreshInterval
        {
            get { return _refreshInterval; }
            set
            {
                Properties.Settings.Default.RefreshInterval = _refreshInterval = value;
            }
        }

        private static bool _showAccountDomains;
        public static bool ShowAccountDomains
        {
            get { return _showAccountDomains; }
            set
            {
                Properties.Settings.Default.ShowAccountDomains = _showAccountDomains = value;
            }
        }

        #region "Proxy Settings"

        private static bool _UseProxy;
        public static bool UseProxy
        {
            get { return _UseProxy; }
            set
            {
                Properties.Settings.Default.ProxyUse = _UseProxy = value;
            }
        }

        private static bool _BypassProxyOnLocal;
        public static bool BypassProxyOnLocal
        {
            get { return _BypassProxyOnLocal; }
            set
            {
                Properties.Settings.Default.ProxyBypassOnLocal = _BypassProxyOnLocal = value;
            }
        }

        private static string _ProxyAddress;
        public static string ProxyAddress
        {
            get { return _ProxyAddress; }
            set
            {
                Properties.Settings.Default.ProxyAddress = _ProxyAddress = value;
            }
        }

        private static string _ProxyUsername;
        public static string ProxyUsername
        {
            get { return _ProxyUsername; }
            set
            {
                Properties.Settings.Default.ProxyUsername = _ProxyUsername = value;
            }
        }

        private static string _ProxyPassword;
        public static string ProxyPassword
        {
            get { return _ProxyPassword; }
            set
            {
                Properties.Settings.Default.ProxyPassword = _ProxyPassword = value;
            }
        }

        #endregion
    }
}
