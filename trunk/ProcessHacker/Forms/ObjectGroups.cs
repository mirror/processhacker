using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ProcessHacker
{
    public partial class ObjectGroups : Form
    {
        private Win32.IWithToken _object;

        public ObjectGroups(Win32.IWithToken obj)
        {
            InitializeComponent();

            Misc.SetDoubleBuffered(listGroups, typeof(ListView), true);

            listGroups.ContextMenu = ListViewMenu.GetMenu(listGroups);

            _object = obj;

            try
            {
                Win32.TOKEN_GROUPS groups = Win32.ReadTokenGroups(_object.GetToken(Win32.TOKEN_RIGHTS.TOKEN_QUERY), 
                    Properties.Settings.Default.ShowAccountDomains);

                for (int i = 0; i < groups.GroupCount; i++)
                {
                    string name = groups.Names[i];

                    if (name == "")
                        continue;

                    ListViewItem item = listGroups.Items.Add(name.ToLower(), name, 0);

                    item.BackColor = GetAttributeColor(groups.Groups[i].Attributes);
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item,
                        GetAttributeString(groups.Groups[i].Attributes)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                  "Process Hacker", MessageBoxButtons.OK,
                  MessageBoxIcon.Error);

                this.Close();
            }
        }

        private void ProcessGroups_Load(object sender, EventArgs e)
        {
            this.Size = Properties.Settings.Default.GroupWindowSize;
            ColumnSettings.LoadSettings(Properties.Settings.Default.GroupListColumns, listGroups);
        }

        private void ProcessGroups_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.GroupWindowSize = this.Size;
            Properties.Settings.Default.GroupListColumns = ColumnSettings.SaveSettings(listGroups);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetAttributeString(Win32.SID_ATTRIBUTES Attributes)
        {
            string text = "";

            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY) != 0)
            {
                if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY_ENABLED) != 0)
                    return "Integrity";
                else
                    return "Integrity (Disabled)";
            }
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_LOGON_ID) != 0)
                text = "Logon ID";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_MANDATORY) != 0)
                text = "Mandatory";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_OWNER) != 0)
                text = "Owner";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_RESOURCE) != 0)
                text = "Resource";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_USE_FOR_DENY_ONLY) != 0)
                text = "Use for Deny Only";

            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED_BY_DEFAULT) != 0)
                return text + " (Default Enabled)";
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED) != 0)
                return text;
            else
                return text + " (Disabled)";
        }

        private Color GetAttributeColor(Win32.SID_ATTRIBUTES Attributes)
        {
            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY) != 0)
            {
                if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_INTEGRITY_ENABLED) == 0)
                    return Color.FromArgb(0xe0e0e0);
                else
                    return Color.White;
            }

            if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED_BY_DEFAULT) != 0)
                return Color.FromArgb(0xe0f0e0);
            else if ((Attributes & Win32.SID_ATTRIBUTES.SE_GROUP_ENABLED) != 0) 
                return Color.White;
            else
                return Color.FromArgb(0xf0e0e0);
        }
    }
}
