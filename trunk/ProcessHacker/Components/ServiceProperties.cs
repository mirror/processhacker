/*
 * Process Hacker - 
 *   embeddable service properties control
 * 
 * Copyright (C) 2008 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ServiceProperties : UserControl
    {
        ServiceProvider _provider;

        public event EventHandler NeedsClose;

        public ServiceProperties(string service)
            : this(new string[] { service })
        { }

        public ServiceProperties(string[] services)
        {
            InitializeComponent();

            PID = -1;

            _provider = Program.HackerWindow.ServiceProvider;

            if (services.Length == 1)
            {
                this.Text = "Service - " + services[0];
            }
            else
            {
                this.Text = "Services";
            }

            foreach (string s in services)
            {
                listServices.Items.Add(new ListViewItem(new string[] { s, _provider.Dictionary[s].Status.DisplayName,
                    _provider.Dictionary[s].Status.ServiceStatusProcess.CurrentState.ToString() })).Name = s;
            }

            _provider.DictionaryModified += new ServiceProvider.ProviderDictionaryModified(_provider_DictionaryModified);
            _provider.DictionaryRemoved += new ServiceProvider.ProviderDictionaryRemoved(_provider_DictionaryRemoved);

            this.FillComboBox(comboErrorControl, typeof(Win32.SERVICE_ERROR_CONTROL));
            this.FillComboBox(comboStartType, typeof(Win32.SERVICE_START_TYPE));
            this.FillComboBox(comboType, typeof(Win32.SERVICE_TYPE));
            comboType.Items.Add("Win32OwnProcess, InteractiveProcess");

            listServices.Visible = true;
            if (listServices.Items.Count > 0)
                listServices.Items[0].Selected = true;

            this.UpdateInformation();
        }

        public int PID { get; set; }

        public ListView List
        {
            get { return listServices; }
        }

        private void Close()
        {
            if (this.NeedsClose != null)
                this.NeedsClose(this, new EventArgs());
        }

        public void Deinit()
        {
            _provider.DictionaryModified -= new ServiceProvider.ProviderDictionaryModified(_provider_DictionaryModified);
            _provider.DictionaryRemoved -= new ServiceProvider.ProviderDictionaryRemoved(_provider_DictionaryRemoved);
        }

        private void FillComboBox(ComboBox box, Type t)
        {
            foreach (string s in Enum.GetNames(t))
                box.Items.Add(s);
        }

        private void _provider_DictionaryRemoved(ServiceItem item)
        {
            // remove the item from the list if it's there
            if (listServices.Items.ContainsKey(item.Status.ServiceName))
                listServices.Items[item.Status.ServiceName].Remove();
        }

        private void _provider_DictionaryModified(ServiceItem oldItem, ServiceItem newItem)
        {
            // update the state of the service
            if (listServices.Items.ContainsKey(newItem.Status.ServiceName))
                listServices.Items[newItem.Status.ServiceName].SubItems[2].Text =
                    newItem.Status.ServiceStatusProcess.CurrentState.ToString();

            // update the start and stop buttons if we have a service selected
            if (listServices.SelectedItems.Count == 1)
            {
                if (listServices.SelectedItems[0].Name == newItem.Status.ServiceName)
                {
                    buttonStart.Enabled = false;
                    buttonStop.Enabled = false;

                    if (newItem.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Running)
                        buttonStop.Enabled = true;
                    else if (newItem.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Stopped)
                        buttonStart.Enabled = true;
                }
            }

            // if the service was just started in this process, add it to the list
            if (newItem.Status.ServiceStatusProcess.ProcessID == this.PID && oldItem.Status.ServiceStatusProcess.ProcessID == 0)
            {
                if (!listServices.Items.ContainsKey(newItem.Status.ServiceName))
                {
                    listServices.Items.Add(new ListViewItem(new string[] { 
                    newItem.Status.ServiceName, 
                    newItem.Status.DisplayName,
                    newItem.Status.ServiceStatusProcess.CurrentState.ToString() 
                })).Name = newItem.Status.ServiceName;
                }
            }
        }

        private void listServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateInformation();
        }

        private void UpdateInformation()
        {
            if (listServices.SelectedItems.Count == 0)
            {
                buttonApply.Enabled = false;
                buttonStart.Enabled = false;
                buttonStop.Enabled = false;
                comboType.Enabled = false;
                comboStartType.Enabled = false;
                comboErrorControl.Enabled = false;
                this.ClearControls();
            }
            else
            {
                try
                {
                    buttonApply.Enabled = true;
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = true;
                    comboType.Enabled = true;
                    comboStartType.Enabled = true;
                    comboErrorControl.Enabled = true;

                    ServiceItem item = _provider.Dictionary[listServices.SelectedItems[0].Name];

                    buttonStart.Enabled = true;
                    buttonStop.Enabled = true;

                    if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Running)
                        buttonStart.Enabled = false;
                    else if (item.Status.ServiceStatusProcess.CurrentState == Win32.SERVICE_STATE.Stopped)
                        buttonStop.Enabled = false;

                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & Win32.SERVICE_ACCEPT.Stop) == 0)
                        buttonStop.Enabled = false;

                    labelServiceName.Text = item.Status.ServiceName;
                    labelServiceDisplayName.Text = item.Status.DisplayName;
                    comboType.SelectedItem = item.Config.ServiceType.ToString();

                    if (item.Config.ServiceType == (Win32.SERVICE_TYPE.Win32OwnProcess | Win32.SERVICE_TYPE.InteractiveProcess))
                        comboType.SelectedItem = "Win32OwnProcess, InteractiveProcess";

                    comboStartType.SelectedItem = item.Config.StartType.ToString();
                    comboErrorControl.SelectedItem = item.Config.ErrorControl.ToString();
                    textServiceBinaryPath.Text = item.Config.BinaryPathName;
                    textUserAccount.Text = item.Config.ServiceStartName;
                    textLoadOrderGroup.Text = item.Config.LoadOrderGroup;
                }
                catch (Exception ex)
                {
                    labelServiceName.Text = ex.Message;
                    this.ClearControls();
                }
            }
        }

        private void ClearControls()
        {
            labelServiceDisplayName.Text = "N/A";
            comboType.Text = "";
            comboStartType.Text = "";
            comboErrorControl.Text = "";
            textServiceBinaryPath.Text = "";
            textUserAccount.Text = "";
            textLoadOrderGroup.Text = "";
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = listServices.SelectedItems[0].Name;

                using (Win32.ServiceHandle service = new Win32.ServiceHandle(serviceName,
                    Win32.SERVICE_RIGHTS.SERVICE_CHANGE_CONFIG))
                {
                    Win32.SERVICE_TYPE type;

                    if (comboType.SelectedItem.ToString() == "Win32OwnProcess, InteractiveProcess")
                        type = Win32.SERVICE_TYPE.Win32OwnProcess | Win32.SERVICE_TYPE.InteractiveProcess;
                    else
                        type = (Win32.SERVICE_TYPE)Enum.Parse(typeof(Win32.SERVICE_TYPE), comboType.SelectedItem.ToString());

                    if (!Win32.ChangeServiceConfig(service.Handle,
                        type,
                        (Win32.SERVICE_START_TYPE)
                        Enum.Parse(typeof(Win32.SERVICE_START_TYPE), comboStartType.SelectedItem.ToString()),
                        (Win32.SERVICE_ERROR_CONTROL)Enum.Parse(typeof(Win32.SERVICE_ERROR_CONTROL),
                        comboErrorControl.SelectedItem.ToString()),
                        textServiceBinaryPath.Text, textLoadOrderGroup.Text,
                        0, 0, textUserAccount.Text, null, null))
                        Win32.ThrowLastWin32Error();
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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                using (Win32.ServiceHandle service = new Win32.ServiceHandle(
                    listServices.SelectedItems[0].Name, Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS))
                    service.Control(Win32.SERVICE_CONTROL.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error stopping service:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
