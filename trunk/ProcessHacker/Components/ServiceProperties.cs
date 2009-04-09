/*
 * Process Hacker - 
 *   embeddable service properties control
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.ServiceProcess;
using System.Windows.Forms;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class ServiceProperties : UserControl
    {
        private Win32.QUERY_SERVICE_CONFIG _oldConfig;
        private ServiceProvider _provider;

        public event EventHandler NeedsClose;

        public ServiceProperties(string service)
            : this(new string[] { service })
        { }

        public ServiceProperties(string[] services)
        {
            InitializeComponent();

            listServices.ListViewItemSorter = new SortedListComparer(listServices);
            listServices.SetTheme("explorer");
            ColumnSettings.LoadSettings(Properties.Settings.Default.ServiceMiniListColumns, listServices);

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
            comboType.Items.Add("Win32ShareProcess, InteractiveProcess");

            listServices.Visible = true;
            if (listServices.Items.Count > 0)
                listServices.Items[0].Selected = true;

            this.UpdateInformation();

            if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                buttonApply.SetShieldIcon(true);
        }

        public int PID { get; set; }

        public ListView List
        {
            get { return listServices; }
        }

        public string ApplyButtonText
        {
            get { return buttonApply.Text; }
            set { buttonApply.Text = value; }
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ServiceMiniListColumns = ColumnSettings.SaveSettings(listServices);
        }

        private void Close()
        {
            this.SaveSettings();

            if (this.NeedsClose != null)
                this.NeedsClose(this, new EventArgs());
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
            checkChangePassword.Checked = false;

            if (listServices.SelectedItems.Count == 0)
            {
                buttonApply.Enabled = false;
                buttonStart.Enabled = false;
                buttonStop.Enabled = false;
                buttonDependents.Enabled = false;
                buttonDependencies.Enabled = false;
                comboType.Enabled = false;
                comboStartType.Enabled = false;
                comboErrorControl.Enabled = false;         
                _oldConfig = new Win32.QUERY_SERVICE_CONFIG();
                this.ClearControls();
            }
            else
            {
                try
                {
                    buttonApply.Enabled = true;
                    buttonStart.Enabled = true;
                    buttonStop.Enabled = true;
                    buttonDependents.Enabled = true;
                    buttonDependencies.Enabled = true;
                    comboType.Enabled = true;
                    comboStartType.Enabled = true;
                    comboErrorControl.Enabled = true;

                    try
                    {
                        using (var shandle = new Win32.ServiceHandle(listServices.SelectedItems[0].Name,
                            Win32.SERVICE_RIGHTS.SERVICE_QUERY_CONFIG))
                            _provider.UpdateServiceConfig(listServices.SelectedItems[0].Name,
                                Win32.GetServiceConfig(listServices.SelectedItems[0].Name));
                    }
                    catch
                    { }

                    ServiceItem item = _provider.Dictionary[listServices.SelectedItems[0].Name];

                    _oldConfig = item.Config;
                    _oldConfig.BinaryPathName = Misc.GetRealPath(_oldConfig.BinaryPathName);

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
                    else if (item.Config.ServiceType == (Win32.SERVICE_TYPE.Win32ShareProcess | Win32.SERVICE_TYPE.InteractiveProcess))
                        comboType.SelectedItem = "Win32ShareProcess, InteractiveProcess";

                    comboStartType.SelectedItem = item.Config.StartType.ToString();
                    comboErrorControl.SelectedItem = item.Config.ErrorControl.ToString();
                    textServiceBinaryPath.Text = Misc.GetRealPath(item.Config.BinaryPathName);
                    textUserAccount.Text = item.Config.ServiceStartName;
                    textLoadOrderGroup.Text = item.Config.LoadOrderGroup;

                    try
                    {
                        using (Win32.ServiceHandle shandle
                            = new Win32.ServiceHandle(item.Status.ServiceName,  Win32.SERVICE_RIGHTS.SERVICE_QUERY_CONFIG))
                            textDescription.Text = shandle.GetDescription();
                    }
                    catch
                    {
                        textDescription.Text = "";
                    }

                    textServiceDll.Text = "";

                    if (item.Config.ServiceType == Win32.SERVICE_TYPE.Win32ShareProcess)
                    {
                        try
                        {
                            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                                "SYSTEM\\CurrentControlSet\\Services\\" + item.Status.ServiceName + "\\Parameters"))
                                textServiceDll.Text = Environment.ExpandEnvironmentVariables((string)key.GetValue("ServiceDll"));
                        }
                        catch
                        { }
                    }

                    try
                    {
                        using (ServiceController controller = new ServiceController(
                            listServices.SelectedItems[0].Name))
                        {
                            if (controller.DependentServices.Length == 0)
                                buttonDependents.Enabled = false;
                            if (controller.ServicesDependedOn.Length == 0)
                                buttonDependencies.Enabled = false;
                        }
                    }
                    catch
                    {
                        buttonDependents.Enabled = false;
                        buttonDependencies.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    labelServiceName.Text = ex.Message;
                    _oldConfig = new Win32.QUERY_SERVICE_CONFIG();
                    this.ClearControls();
                }
            }
        }

        private void ClearControls()
        {
            labelServiceName.Text = "";
            labelServiceDisplayName.Text = "";
            comboType.Text = "";
            comboStartType.Text = "";
            comboErrorControl.Text = "";
            textServiceBinaryPath.Text = "";
            textUserAccount.Text = "";
            textPassword.Text = "password";
            textLoadOrderGroup.Text = "";
            textDescription.Text = "";
            textServiceDll.Text = "";
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = listServices.SelectedItems[0].Name;

                Win32.SERVICE_TYPE type;

                if (comboType.SelectedItem.ToString() == "Win32OwnProcess, InteractiveProcess")
                    type = Win32.SERVICE_TYPE.Win32OwnProcess | Win32.SERVICE_TYPE.InteractiveProcess;
                else if (comboType.SelectedItem.ToString() == "Win32ShareProcess, InteractiveProcess")
                    type = Win32.SERVICE_TYPE.Win32ShareProcess | Win32.SERVICE_TYPE.InteractiveProcess;
                else
                    type = (Win32.SERVICE_TYPE)Enum.Parse(typeof(Win32.SERVICE_TYPE), comboType.SelectedItem.ToString());

                string binaryPath = textServiceBinaryPath.Text;
                string loadOrderGroup = textLoadOrderGroup.Text;
                string userAccount = textUserAccount.Text;
                string password = textPassword.Text;
                var startType = (Win32.SERVICE_START_TYPE)
                    Enum.Parse(typeof(Win32.SERVICE_START_TYPE), comboStartType.SelectedItem.ToString());
                var errorControl = (Win32.SERVICE_ERROR_CONTROL)
                    Enum.Parse(typeof(Win32.SERVICE_ERROR_CONTROL), comboErrorControl.SelectedItem.ToString());

                // Only change the items which the user modified.
                if (binaryPath == _oldConfig.BinaryPathName)
                    binaryPath = null;
                if (loadOrderGroup == _oldConfig.LoadOrderGroup)
                    loadOrderGroup = null;
                if (userAccount == _oldConfig.ServiceStartName)
                    userAccount = null;
                if (!checkChangePassword.Checked)
                    password = null;

                if (type == Win32.SERVICE_TYPE.KernelDriver || type == Win32.SERVICE_TYPE.FileSystemDriver)
                    userAccount = null;

                if (Program.ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull)
                {
                    using (Win32.ServiceHandle service = new Win32.ServiceHandle(serviceName,
                        Win32.SERVICE_RIGHTS.SERVICE_CHANGE_CONFIG))
                    {
                        if (!Win32.ChangeServiceConfig(service.Handle,
                            type, startType, errorControl,
                            binaryPath, loadOrderGroup, 0, 0, userAccount, password, null))
                            Win32.ThrowLastWin32Error();
                    }
                }
                else
                {
                    string args = "-e -type service -action config -obj \"" + serviceName + "\" -hwnd " +
                        this.Handle.ToString();

                    args += " -servicetype \"" + comboType.SelectedItem.ToString() + "\"";
                    args += " -servicestarttype \"" + comboStartType.SelectedItem.ToString() + "\"";
                    args += " -serviceerrorcontrol \"" + comboErrorControl.SelectedItem.ToString() + "\"";

                    if (binaryPath != null)
                        args += " -servicebinarypath \"" + binaryPath.Replace("\"", "\\\"") + "\"";
                    if (loadOrderGroup != null)
                        args += " -serviceloadordergroup \"" + loadOrderGroup.Replace("\"", "\\\"") + "\"";
                    if (userAccount != null)
                        args += " -serviceuseraccount \"" + userAccount.Replace("\"", "\\\"") + "\"";
                    if (password != null)
                        args += " -servicepassword \"" + password.Replace("\"", "\\\"") + "\"";

                    var result = Program.StartProcessHackerAdminWait(args, this.Handle, 2000);

                    if (result == Win32.WaitResult.Timeout || result == Win32.WaitResult.Abandoned)
                        return;
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
            ServiceActions.Start(this, listServices.SelectedItems[0].Name, false);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            ServiceActions.Stop(this, listServices.SelectedItems[0].Name, false);
        }

        private void buttonDependents_Click(object sender, EventArgs e)
        {
            try
            {
                using (ServiceController controller = new ServiceController(
                    listServices.SelectedItems[0].Name))
                {
                    List<string> dependents = new List<string>();

                    foreach (var service in controller.DependentServices)
                        dependents.Add(service.ServiceName);

                    ServiceWindow sw = new ServiceWindow(dependents.ToArray());

                    sw.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDependencies_Click(object sender, EventArgs e)
        {
            try
            {
                using (ServiceController controller = new ServiceController(
                    listServices.SelectedItems[0].Name))
                {
                    List<string> dependencies = new List<string>();

                    foreach (var service in controller.ServicesDependedOn)
                        dependencies.Add(service.ServiceName);

                    ServiceWindow sw = new ServiceWindow(dependencies.ToArray());

                    sw.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textPassword_TextChanged(object sender, EventArgs e)
        {
            checkChangePassword.Checked = true;
        }
    }
}
