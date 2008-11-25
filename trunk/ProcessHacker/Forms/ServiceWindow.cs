using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ServiceWindow : Form
    {
        ServiceProvider _provider;

        public ServiceWindow(string service)
            : this(true, new string[] { service })
        { }

        public ServiceWindow(string[] services)
            : this(false, services)
        { }

        private ServiceWindow(bool hideList, string[] services)
        {
            InitializeComponent();

            _provider = Program.HackerWindow.ServiceProvider;

            if (hideList)
            {
                listServices.Visible = false;
                this.Height -= listServices.Height - 5;
                panelService.Dock = DockStyle.Fill;
            }
            else
            {
                buttonCancel.Visible = false;
            }

            foreach (string s in services)
            {
                listServices.Items.Add(new ListViewItem(new string[] { s, _provider.Dictionary[s].Status.DisplayName,
                    _provider.Dictionary[s].Status.ServiceStatusProcess.CurrentState.ToString() })).Name = s;
            }

            _provider.DictionaryModified += new ProviderDictionaryModified(_provider_DictionaryModified);

            this.FillComboBox(comboErrorControl, typeof(Win32.SERVICE_ERROR_CONTROL));
            this.FillComboBox(comboStartType, typeof(Win32.SERVICE_START_TYPE));
            this.FillComboBox(comboType, typeof(Win32.SERVICE_TYPE));
        }

        private void FillComboBox(ComboBox box, Type t)
        {                    
            foreach (string s in Enum.GetNames(t))
                box.Items.Add(s);    
        }

        private void ServiceWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _provider.DictionaryModified -= new ProviderDictionaryModified(_provider_DictionaryModified);
        }

        private void _provider_DictionaryModified(object oldItem, object newItem)
        {
            ServiceItem sitem = (ServiceItem)newItem;

            if (listServices.Items.ContainsKey(sitem.Status.ServiceName))
                listServices.Items[sitem.Status.ServiceName].SubItems[2].Text = 
                    sitem.Status.ServiceStatusProcess.CurrentState.ToString();
        }

        private void listServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateInformation();  
        }

        private void UpdateInformation()
        {
            try
            {
                if (listServices.SelectedItems.Count == 0)
                {
                    buttonCancel.Enabled = false;
                    buttonApply.Enabled = false;

                    throw new Exception("N/A");
                }
                else
                {
                    buttonCancel.Enabled = true;
                    buttonApply.Enabled = true;
                }

                ServiceItem item = _provider.Dictionary[listServices.SelectedItems[0].Name];

                labelServiceName.Text = item.Status.ServiceName;
                labelServiceDisplayName.Text = item.Status.DisplayName;
                comboType.SelectedItem = item.Config.ServiceType.ToString();
                comboStartType.SelectedItem = item.Config.StartType.ToString();
                comboErrorControl.SelectedItem = item.Config.ErrorControl.ToString();
                textServiceBinaryPath.Text = item.Config.BinaryPathName;
                textUserAccount.Text = item.Config.ServiceStartName;
                textLoadOrderGroup.Text = item.Config.LoadOrderGroup;
            }
            catch (Exception ex)
            {
                labelServiceName.Text = ex.Message;
                labelServiceDisplayName.Text = "N/A";
                comboType.SelectedItem = "";
                comboStartType.SelectedItem = "";
                comboErrorControl.SelectedItem = "";
                textServiceBinaryPath.Text = "";
                textUserAccount.Text = "";
                textLoadOrderGroup.Text = "";
            }
        }

        private void ServiceWindow_Load(object sender, EventArgs e)
        {
            listServices.Visible = true;
            if (listServices.Items.Count > 0)
                listServices.Items[0].Selected = true;
            if (listServices.Items.Count == 1)
                listServices.Visible = false;

            this.UpdateInformation();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = listServices.SelectedItems[0].Name;

                using (Win32.ServiceHandle service = new Win32.ServiceHandle(serviceName,
                    Win32.SERVICE_RIGHTS.SERVICE_CHANGE_CONFIG))
                {
                    if (Win32.ChangeServiceConfig(service.Handle,
                        (Win32.SERVICE_TYPE)Enum.Parse(typeof(Win32.SERVICE_TYPE), comboType.SelectedItem.ToString()),
                        (Win32.SERVICE_START_TYPE)
                        Enum.Parse(typeof(Win32.SERVICE_START_TYPE), comboStartType.SelectedItem.ToString()),
                        (Win32.SERVICE_ERROR_CONTROL)Enum.Parse(typeof(Win32.SERVICE_ERROR_CONTROL),
                        comboErrorControl.SelectedItem.ToString()),
                        textServiceBinaryPath.Text, textLoadOrderGroup.Text,
                        0, 0, textUserAccount.Text, 0, 0) == 0)
                    {
                        throw new Exception(Win32.GetLastErrorMessage());
                    }
                }
                
                _provider.UpdateServiceConfig(serviceName, Win32.GetServiceConfig(serviceName));

                if (listServices.Items.Count == 1)
                    this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not change service configuration:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
