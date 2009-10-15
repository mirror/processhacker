using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ProcessHacker
{
    public static class Settings
    {
        private static System.Xml.XmlDocument basedoc;
        private static System.Xml.XmlDocument rootdoc;

        public static void Refresh()
        {
            RefreshInterval = Properties.Settings.Default.RefreshInterval;
            ShowAccountDomains = Properties.Settings.Default.ShowAccountDomains;

            LoadConfig();
        }

        public static void LoadConfig()
        {
            basedoc = new System.Xml.XmlDocument();
            rootdoc = new System.Xml.XmlDocument();
            
            basedoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
            rootdoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            //we can use this code to setup the location of our own config file location here, or keep the mfcrs in sync :P
            //Create appdomainsetup information for the new appdomain.
            //AppDomainSetup domaininfo = new AppDomainSetup();
            //domaininfo.ApplicationBase = System.Environment.CurrentDirectory;
            //domaininfo.ConfigurationFile = System.Environment.CurrentDirectory + "\\ProcessHacker.exe.config";
            //domaininfo.ApplicationName = "PHAlpha";
            //domaininfo.LicenseFile = System.Environment.CurrentDirectory + "\\license.txt";

            //Create evidence for new appdomain.
            //System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;
            ////Add the zone and url information to restrict permissions assigned to the appdomain.
            //adevidence.AddHost(new System.Security.Policy.Url("http://www.example.com"));
            //adevidence.AddHost(new System.Security.Policy.Zone(System.Security.SecurityZone.Internet));

            //Create the application domain.
            //AppDomain newDomain = AppDomain.CreateDomain("MyDom", adevidence, domaininfo);

            //Write out the application domain information.
            //ProcessHacker.Program.HackerWindow.QueueMessage("Host domain: " + AppDomain.CurrentDomain.FriendlyName);
            //ProcessHacker.Program.HackerWindow.QueueMessage("child domain: " + newDomain.FriendlyName);
            //ProcessHacker.Program.HackerWindow.QueueMessage("Application base is: " + newDomain.SetupInformation.ApplicationBase);
            //ProcessHacker.Program.HackerWindow.QueueMessage("Configuration file is: " + newDomain.SetupInformation.ConfigurationFile);
            //ProcessHacker.Program.HackerWindow.QueueMessage("Application name is: " + newDomain.SetupInformation.ApplicationName);
            //ProcessHacker.Program.HackerWindow.QueueMessage("License file is: " + newDomain.SetupInformation.LicenseFile);
            //ProcessHacker.Program.HackerWindow.QueueMessage("OS Version: " + ProcessHacker.Native.OSVersion.WindowsVersion.ToString());

            //System.Collections.IEnumerator newevidenceenum = newDomain.Evidence.GetEnumerator();
            //while (newevidenceenum.MoveNext())
            //    ProcessHacker.Program.HackerWindow.QueueMessage(newevidenceenum.Current.ToString());

            //AppDomain.Unload(newDomain);
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

        #region Settings Methods

        //This code will add a listviewitem
        //to a listview for each database entry 
        //in the appSettings section of an App.config file.
        private static void loadFromConfig()
        {
            //this.lst.Items.Clear();
            basedoc = new XmlDocument();
            basedoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");

            XmlNode appSettingsNode = basedoc.SelectSingleNode("configuration/appSettings");
            foreach (XmlNode node in appSettingsNode.ChildNodes)
            {
                System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();
                string connStr = node.Attributes["value"].Value.ToString();
                string keyName = node.Attributes["key"].Value.ToString();
                lvi.Text = keyName;
                lvi.SubItems.Add(connStr);
                //this.lst.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Adds a key and value to the App.config
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strValue"></param>
        public static void AddKey(string strKey, string strValue)
        {
            XmlNode appSettingsNode = rootdoc.SelectSingleNode("configuration/appSettings");
            try
            {
                if (KeyExists(strKey))
                     ProcessHacker.Common.Logging.Log(new ArgumentException("Key name: <" + strKey + "> already exists in the configuration."));
                XmlNode newChild = appSettingsNode.FirstChild.Clone();
                newChild.Attributes["key"].Value = strKey;
                newChild.Attributes["value"].Value = strValue;
                appSettingsNode.AppendChild(newChild);

                //We have to save the configuration in two places, 
                //because while we have a root App.config,
                //we also have an ApplicationName.exe.config.
                basedoc.Save(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
                rootdoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Updates a key within the App.config
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="newValue"></param>
        public static void UpdateKey(string strKey, string newValue)
        {
            if (!KeyExists(strKey))
               ProcessHacker.Common.Logging.Log(new ArgumentNullException("Key", "<" + strKey + "> does not exist in the configuration. Update failed."));
            System.Xml.XmlNode appSettingsNode = rootdoc.SelectSingleNode("configuration/appSettings");
            
            // Attempt to locate the requested setting.
            foreach (System.Xml.XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    childNode.Attributes["value"].Value = newValue;
            }

            //We have to save the configuration in two places, 
            //because while we have a root App.config,
            //we also have an ApplicationName.exe.config.
            basedoc.Save(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
            rootdoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        /// <summary>
        /// Determines if a key exists within the App.config
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static bool KeyExists(string strKey)
        {
            XmlNode appSettingsNode = rootdoc.SelectSingleNode("configuration/appSettings");

            // Attempt to locate the requested setting.
            foreach (System.Xml.XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
