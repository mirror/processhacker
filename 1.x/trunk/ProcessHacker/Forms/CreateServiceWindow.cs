using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public partial class CreateServiceWindow : Form
    {
        public CreateServiceWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            Utils.Fill(comboErrorControl, typeof(ServiceErrorControl));
            Utils.Fill(comboStartType, typeof(ServiceStartType));
            Utils.Fill(comboType, typeof(ServiceType));
            comboType.Items.Add("Win32OwnProcess, InteractiveProcess");
            comboErrorControl.SelectedItem = "Ignore";
            comboStartType.SelectedItem = "DemandStart";
            comboType.SelectedItem = "Win32OwnProcess";
        }

        private void CreateServiceWindow_Load(object sender, EventArgs e)
        {
            textName.Select();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            ofd.FileName = textBinaryPath.Text;

            if (ofd.ShowDialog() == DialogResult.OK)
                textBinaryPath.Text = ofd.FileName;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                using (var scmhandle = new ServiceManagerHandle(ScManagerAccess.CreateService))
                {
                    ServiceType serviceType;

                    if (comboType.SelectedItem.ToString() == "Win32OwnProcess, InteractiveProcess")
                        serviceType = ServiceType.Win32OwnProcess |
                            ServiceType.InteractiveProcess;
                    else
                        serviceType = (ServiceType)Enum.Parse(typeof(ServiceType), comboType.SelectedItem.ToString());

                    var startType = (ServiceStartType)
                        Enum.Parse(typeof(ServiceStartType), comboStartType.SelectedItem.ToString());
                    var errorControl = (ServiceErrorControl)
                        Enum.Parse(typeof(ServiceErrorControl), comboErrorControl.SelectedItem.ToString());

                    scmhandle.CreateService(
                        textName.Text,
                        textDisplayName.Text,
                        serviceType,
                        startType,
                        errorControl,
                        textBinaryPath.Text,
                        null,
                        null,
                        null
                        ).Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to create the service", ex);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
