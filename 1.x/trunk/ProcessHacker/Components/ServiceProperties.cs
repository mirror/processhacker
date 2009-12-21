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
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;

namespace ProcessHacker.Components
{
    public partial class ServiceProperties : UserControl
    {
        private QueryServiceConfig _oldConfig;
        private ServiceProvider _provider;

        public event EventHandler NeedsClose;

        public ServiceProperties(string service)
            : this(new string[] { service })
        { }

        public ServiceProperties(string[] services)
        {
            InitializeComponent();

            listServices.ListViewItemSorter = new SortedListViewComparer(listServices);
            listServices.SetTheme("explorer");
            ColumnSettings.LoadSettings(Settings.Instance.ServiceMiniListColumns, listServices);

            PID = -1;

            _provider = Program.ServiceProvider;

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

            Utils.Fill(comboErrorControl, typeof(ServiceErrorControl));
            Utils.Fill(comboStartType, typeof(ServiceStartType));
            Utils.Fill(comboType, typeof(ProcessHacker.Native.Api.ServiceType));
            comboType.Items.Add("Win32OwnProcess, InteractiveProcess");
            comboType.Items.Add("Win32ShareProcess, InteractiveProcess");

            listServices.Visible = true;
            if (listServices.Items.Count > 0)
                listServices.Items[0].Selected = true;

            this.UpdateInformation();

            if (Program.ElevationType == TokenElevationType.Limited)
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

        public Button ApplyButton
        {
            get { return buttonApply; }
        }

        public void SaveSettings()
        {
            Settings.Instance.ServiceMiniListColumns = ColumnSettings.SaveSettings(listServices);
        }

        private void Close()
        {
            this.SaveSettings();

            if (this.NeedsClose != null)
                this.NeedsClose(this, new EventArgs());
        }

        private void _provider_DictionaryRemoved(ServiceItem item)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                {
                    // remove the item from the list if it's there
                    if (listServices.Items.ContainsKey(item.Status.ServiceName))
                        listServices.Items[item.Status.ServiceName].Remove();
                }));
        }

        private void _provider_DictionaryModified(ServiceItem oldItem, ServiceItem newItem)
        {
            if (!this.IsHandleCreated)
                return;

            this.BeginInvoke(new MethodInvoker(() =>
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

                            if (newItem.Status.ServiceStatusProcess.CurrentState == ServiceState.Running)
                                buttonStop.Enabled = true;
                            else if (newItem.Status.ServiceStatusProcess.CurrentState == ServiceState.Stopped)
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
                }));
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
                buttonPermissions.Enabled = false;
                comboType.Enabled = false;
                comboStartType.Enabled = false;
                comboErrorControl.Enabled = false;         
                _oldConfig = new QueryServiceConfig();
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
                    buttonPermissions.Enabled = true;
                    comboType.Enabled = true;
                    comboStartType.Enabled = true;
                    comboErrorControl.Enabled = true;

                    try
                    {
                        using (var shandle = 
                            new ServiceHandle(listServices.SelectedItems[0].Name, ServiceAccess.QueryConfig))
                            _provider.UpdateServiceConfig(listServices.SelectedItems[0].Name, shandle.GetConfig());
                    }
                    catch
                    { }

                    ServiceItem item = _provider.Dictionary[listServices.SelectedItems[0].Name];

                    _oldConfig = item.Config;
                    _oldConfig.BinaryPathName = FileUtils.GetFileName(_oldConfig.BinaryPathName);

                    buttonStart.Enabled = true;
                    buttonStop.Enabled = true;

                    if (item.Status.ServiceStatusProcess.CurrentState == ServiceState.Running)
                        buttonStart.Enabled = false;
                    else if (item.Status.ServiceStatusProcess.CurrentState == ServiceState.Stopped)
                        buttonStop.Enabled = false;

                    if ((item.Status.ServiceStatusProcess.ControlsAccepted & ServiceAccept.Stop) == 0)
                        buttonStop.Enabled = false;

                    labelServiceName.Text = item.Status.ServiceName;
                    labelServiceDisplayName.Text = item.Status.DisplayName;
                    comboType.SelectedItem = item.Config.ServiceType.ToString();

                    if (item.Config.ServiceType ==
                        (ProcessHacker.Native.Api.ServiceType.Win32OwnProcess |
                        ProcessHacker.Native.Api.ServiceType.InteractiveProcess))
                        comboType.SelectedItem = "Win32OwnProcess, InteractiveProcess";
                    else if (item.Config.ServiceType ==
                        (ProcessHacker.Native.Api.ServiceType.Win32ShareProcess |
                        ProcessHacker.Native.Api.ServiceType.InteractiveProcess))
                        comboType.SelectedItem = "Win32ShareProcess, InteractiveProcess";

                    comboStartType.SelectedItem = item.Config.StartType.ToString();
                    comboErrorControl.SelectedItem = item.Config.ErrorControl.ToString();
                    textServiceBinaryPath.Text = FileUtils.GetFileName(item.Config.BinaryPathName);
                    textUserAccount.Text = item.Config.ServiceStartName;
                    textLoadOrderGroup.Text = item.Config.LoadOrderGroup;

                    try
                    {
                        using (var shandle
                            = new ServiceHandle(item.Status.ServiceName, ServiceAccess.QueryConfig))
                            textDescription.Text = shandle.GetDescription();
                    }
                    catch
                    {
                        textDescription.Text = "";
                    }

                    textServiceDll.Text = "";

                    if (item.Config.ServiceType == ProcessHacker.Native.Api.ServiceType.Win32ShareProcess)
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
                    _oldConfig = new QueryServiceConfig();
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

                ProcessHacker.Native.Api.ServiceType type;

                if (comboType.SelectedItem.ToString() == "Win32OwnProcess, InteractiveProcess")
                    type = ProcessHacker.Native.Api.ServiceType.Win32OwnProcess |
                        ProcessHacker.Native.Api.ServiceType.InteractiveProcess;
                else if (comboType.SelectedItem.ToString() == "Win32ShareProcess, InteractiveProcess")
                    type = ProcessHacker.Native.Api.ServiceType.Win32ShareProcess |
                        ProcessHacker.Native.Api.ServiceType.InteractiveProcess;
                else
                    type = (ProcessHacker.Native.Api.ServiceType)
                        Enum.Parse(typeof(ProcessHacker.Native.Api.ServiceType), 
                        comboType.SelectedItem.ToString());

                string binaryPath = textServiceBinaryPath.Text;
                string loadOrderGroup = textLoadOrderGroup.Text;
                string userAccount = textUserAccount.Text;
                string password = textPassword.Text;
                var startType = (ServiceStartType)
                    Enum.Parse(typeof(ServiceStartType), comboStartType.SelectedItem.ToString());
                var errorControl = (ServiceErrorControl)
                    Enum.Parse(typeof(ServiceErrorControl), comboErrorControl.SelectedItem.ToString());

                // Only change the items which the user modified.
                if (binaryPath == _oldConfig.BinaryPathName)
                    binaryPath = null;
                if (loadOrderGroup == _oldConfig.LoadOrderGroup)
                    loadOrderGroup = null;
                if (userAccount == _oldConfig.ServiceStartName)
                    userAccount = null;
                if (!checkChangePassword.Checked)
                    password = null;

                if (type == ProcessHacker.Native.Api.ServiceType.KernelDriver ||
                    type == ProcessHacker.Native.Api.ServiceType.FileSystemDriver)
                    userAccount = null;

                if (Program.ElevationType == TokenElevationType.Full)
                {
                    using (var shandle = new ServiceHandle(serviceName, ServiceAccess.ChangeConfig))
                    {
                        if (!Win32.ChangeServiceConfig(shandle.Handle,
                            type, startType, errorControl,
                            binaryPath, loadOrderGroup, IntPtr.Zero, null, userAccount, password, null))
                            Win32.Throw();
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

                    if (result == WaitResult.Timeout || result == WaitResult.Abandoned)
                        return;
                }

                using (var shandle = new ServiceHandle(serviceName, ServiceAccess.QueryConfig))
                    _provider.UpdateServiceConfig(serviceName, shandle.GetConfig());

                if (listServices.Items.Count == 1)
                    this.Close();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to change service configuration", ex);
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
                PhUtils.ShowException("Unable to show dependents for the service", ex);
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
                PhUtils.ShowException("Unable to show dependencies of the service", ex);
            }
        }

        private void textPassword_TextChanged(object sender, EventArgs e)
        {
            checkChangePassword.Checked = true;
        }

        private void buttonPermissions_Click(object sender, EventArgs e)
        {
            try
            {
                SecurityEditor.EditSecurity(
                    this,
                    SecurityEditor.GetSecurableWrapper(
                        (access) => new ServiceHandle(listServices.SelectedItems[0].Name, (ServiceAccess)access)
                        ),
                    listServices.SelectedItems[0].Name,
                    NativeTypeFactory.GetAccessEntries(NativeTypeFactory.ObjectType.Service)
                    );
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to edit security", ex);
            }
        }
    }
}
