using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Common;
using System.Xml;

namespace ProcessHacker
{
    public static class Settings
    {
        private static XmlDocument basedoc;
        
        //temporary, will be added as a setting
        private static string settingsPath = 
            System.Windows.Forms.Application.StartupPath + "\\Settings.xml";

        #region Settings

        public static string AppUpdateUrl
        {
            get { return Settings.GetStringKey("AppUpdateUrl"); }
            set { Settings.UpdateKey("AppUpdateUrl", value); }
        }

        public static string DbgHelpPath
        {
            get { return Settings.GetStringKey("DbgHelpPath"); }
            set { Settings.UpdateKey("DbgHelpPath", value); }
        }

        public static int RefreshInterval
        {
            get { return Settings.GetIntKey("RefreshInterval"); }
            set { Settings.UpdateKey("RefreshInterval", value.ToString()); }
        }

        public static bool ShowAccountDomains
        {
            get { return Settings.GetBoolKey("ShowAccountDomains"); }
            set { Settings.UpdateKey("ShowAccountDomains", value.ToString()); }
        }

        #endregion

        #region Settings Management

        public static void AddKey(string id, string strValue)
        {
            if (basedoc == null)
                loadBaseSettings();

            loadBaseSettings();

            XmlNode appSettingsNode = basedoc.SelectSingleNode("//setting[@id='" + id + "']");
            
            try
            {
                if (!KeyExists(id))
                {
                    Logging.Log(new ArgumentException(
                        "Key name: <" + id + "> already exists in the configuration."));
                    return;
                }

                System.Xml.XmlNode newChild = appSettingsNode.FirstChild.Clone();
                newChild.Attributes["id"].Value = id;
                newChild.Attributes["value"].Value = strValue;
                
                appSettingsNode.AppendChild(newChild);

                System.Xml.XmlNode newNode = basedoc.SelectSingleNode("//setting[@id='" + id + "']");
                //get the node where you want to insert the data
                System.Xml.XmlNode childNode = basedoc.CreateNode(System.Xml.XmlNodeType.Element, "your node name where you want to insert data", "");

                //In the below step "name" is your node name and "sree" is your data to insert
                System.Xml.XmlAttribute newAttribute = basedoc.CreateAttribute("name", "sree", "");
                childNode.Attributes.Append(newAttribute);
                newNode.AppendChild(childNode);

                basedoc.Save(settingsPath);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        public static void UpdateKey(string id, string newValue)
        {
            if (basedoc == null)
                loadBaseSettings();

            XmlNode appSettingsNode = basedoc.SelectSingleNode("//setting[@id='" + id + "']");

            // Attempt to locate the requested setting.
            foreach (XmlNode childNode in appSettingsNode)
            {
                childNode.InnerText = newValue;
            }

            basedoc.Save(settingsPath);
        }

        public static void DeleteKey(string id)
        {
            XmlNodeList newXMLNodes = basedoc.SelectNodes("//setting[@id='" + id + "']");
            foreach (XmlNode newXMLNode in newXMLNodes)
            {
                newXMLNode.ParentNode.RemoveChild(newXMLNode);
            }

            Settings.SaveSettings();
            Settings.ReloadSettings();
        }

        #endregion

        #region Settings Functions

        public static void ReloadSettings()
        {
            RefreshInterval = Settings.GetIntKey("RefreshInterval");         
            ShowAccountDomains = Settings.GetBoolKey("ShowAccountDomains");
            AppUpdateUrl = Settings.GetStringKey("AppUpdateUrl");

            //System.Windows.Forms.MessageBox.Show(Settings.GetStringKey("AppUpdateUrl"));

            //Settings.UpdateKey("AppUpdateUrl", "http://address.com");

            //System.Windows.Forms.MessageBox.Show(Settings.GetStringKey("AppUpdateUrl"));
        }

        public static void SaveSettings()
        {
            if (basedoc == null)
                Settings.loadBaseSettings();

            basedoc.Save(settingsPath);
        }

        #endregion

        #region Settings Helpers

        public static string GetStringKey(string id)
        {
            if (basedoc == null)
                loadBaseSettings();

            if (!KeyExists(id))
            {
                Logging.Log(new ArgumentNullException
                    ("Key", "<" + id + "> does not exist in the configuration. Update failed."));
                return string.Empty;
            }

            System.Xml.XmlNodeList nodes = basedoc.SelectNodes("//setting[@id='" + id + "']");
            foreach (System.Xml.XmlNode node in nodes)
            {
                return node["value"].InnerText;
            }
            return null;
        }

        public static bool GetBoolKey(string id)
        {
            if (basedoc == null)
                loadBaseSettings();

            if (!KeyExists(id))
            {
                Logging.Log(new ArgumentNullException
                    ("Key", "<" + id + "> does not exist in the configuration. Update failed."));
                return false;
            }

            System.Xml.XmlNodeList nodes = basedoc.SelectNodes("//setting[@id='" + id + "']");
            foreach (System.Xml.XmlNode node in nodes)
            {
                return (node["value"].InnerText.Length != 0);
            }

            return false;
        }

        public static int GetIntKey(string id)
        {
            if (basedoc == null)
                loadBaseSettings();

            if (!KeyExists(id))
            {
                Logging.Log(new ArgumentNullException
                    ("Key", "<" + id + "> does not exist in the configuration. Update failed."));
                return 0;
            }

            System.Xml.XmlNodeList nodes = basedoc.SelectNodes("//setting[@id='" + id + "']");
            foreach (System.Xml.XmlNode node in nodes)
            {
                return Convert.ToInt32(node["value"].InnerText);
            }

            return 0;
        }

        /// <summary>
        /// Determines if a key exists within the App.config
        /// </summary>
        public static bool KeyExists(string id)
        {
            if (basedoc == null)
                loadBaseSettings();

            System.Xml.XmlNode appSettingsNode =  basedoc.SelectSingleNode("//setting[@id='" + id + "']");

            if (appSettingsNode != null)
            {
                return true;
            }
            else
            {
                UpdateKey(id, "");
                return true;
            }
        }

        #endregion

        #region Settings Base

        public static bool loadBaseSettings()
        {
            basedoc = new System.Xml.XmlDocument();

            try
            {
                if (System.IO.File.Exists(settingsPath))
                {
                    basedoc.Load(settingsPath);
                    return true;
                }
                else
                {
                    Settings.resetBaseSettings(); //reset config, reload base    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        public static bool resetBaseSettings()
        {
            try
            {
                basedoc = new System.Xml.XmlDocument();

                basedoc.LoadXml(ProcessHacker.Properties.Resources.Settings);

                //if (true)? portable mode?? commandline switch??    
                Settings.SaveSettings();

                Settings.loadBaseSettings(); // switch settings to newly created config file   
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        #endregion

    }
}
