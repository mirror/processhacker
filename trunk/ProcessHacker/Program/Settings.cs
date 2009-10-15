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
            LoadBaseSettings();

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

                Settings.SaveBaseSettings();
                Settings.LoadBaseSettings();
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        public static void UpdateKey(string id, string Value)
        {
            if (basedoc == null)
                Settings.LoadBaseSettings();

            XmlNode appSettingsNode = basedoc.SelectSingleNode("//setting[@id='" + id + "']");
            System.Xml.XmlNode currNode;
            System.Xml.XmlDocumentFragment docFrag = basedoc.CreateDocumentFragment();
            docFrag.InnerXml =
                "<setting id=" + id + ">" +
                "<value>" + Value + "</value>" +
                "<defvalue>" + Value + "</defvalue>" + 
                "</item>";

            //insert the node into the document 
            currNode = basedoc.DocumentElement;
            currNode.InsertBefore(docFrag, currNode.FirstChild);

            Settings.SaveBaseSettings();
            Settings.LoadBaseSettings();
        }

        public static void DeleteKey(string id)
        {
            XmlNodeList newXMLNodes = basedoc.SelectNodes("//setting[@id='" + id + "']");
            foreach (XmlNode newXMLNode in newXMLNodes)
            {
                newXMLNode.ParentNode.RemoveChild(newXMLNode);
            }

            Settings.SaveBaseSettings();
            Settings.LoadBaseSettings();
        }

        #endregion

        #region Settings Helpers

        public static string GetStringKey(string id)
        {
            if (basedoc == null)
                Settings.LoadBaseSettings();

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
                Settings.LoadBaseSettings();

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
                Settings.LoadBaseSettings();

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

        public static bool KeyExists(string id)
        {
            if (basedoc == null)
                Settings.LoadBaseSettings();

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

        public static bool LoadBaseSettings()
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
                    Settings.ResetBaseSettings(); //reset config, reload base    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        public static bool ResetBaseSettings()
        {
            try
            {
                basedoc = new System.Xml.XmlDocument();
                basedoc.LoadXml(ProcessHacker.Properties.Resources.Settings);

                //if (true)? portable mode?? commandline switch??    
                Settings.SaveBaseSettings();

                basedoc = null; // making sure new config file is used

                Settings.LoadBaseSettings(); // switch settings to newly created config file   
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        public static void SaveBaseSettings()
        {
            if (basedoc == null)
                Settings.LoadBaseSettings();

            basedoc.Save(settingsPath);
        }

        #endregion

    }
}
