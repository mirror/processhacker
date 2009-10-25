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

    }
}
