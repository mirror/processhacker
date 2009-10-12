using System;
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Ui
{
    public partial class ChooseProcessDialog : Form
    {
        private int _selectedPid;

        public ChooseProcessDialog()
        {
            InitializeComponent();
        }

        private void ChooseProcessDialog_Load(object sender, EventArgs e)
        {
            this.RefreshProcesses();
        }

        public int SelectedPid
        {
            get { return _selectedPid; }
        }

        private void RefreshProcesses()
        {
            var processes = Windows.GetProcesses();

            listProcesses.BeginUpdate();
            listProcesses.Items.Clear();

            var generic_process = imageList.Images["generic_process"];
            imageList.Images.Clear();
            imageList.Images.Add("generic_process", generic_process);

            foreach (var process in processes.Values)
            {
                string userName = "";
                string fileName = null;

                try
                {
                    using (var phandle = new ProcessHandle(process.Process.ProcessId, OSVersion.MinProcessQueryInfoAccess))
                    {
                        using (var thandle = phandle.GetToken(TokenAccess.Query))
                        using (var sid = thandle.GetUser())
                            userName = sid.GetFullName(true);

                        fileName = FileUtils.GetFileName(phandle.GetImageFileName());
                    }
                }
                catch
                { }

                ListViewItem item = new ListViewItem(
                    new string[]
                    {
                        process.Process.ProcessId == 0 ? "System Idle Process" : process.Name,
                        process.Process.ProcessId.ToString(),
                        userName
                    });

                if (!string.IsNullOrEmpty(fileName))
                {
                    Icon fileIcon = FileUtils.GetFileIcon(fileName);

                    if (fileIcon != null)
                    {
                        imageList.Images.Add(process.Process.ProcessId.ToString(), fileIcon);
                        item.ImageKey = process.Process.ProcessId.ToString();
                    }
                }

                if (string.IsNullOrEmpty(item.ImageKey))
                    item.ImageKey = "generic_process";

                listProcesses.Items.Add(item);
            }

            listProcesses.EndUpdate();
        }

        private void ChooseProcess()
        {
            if (listProcesses.SelectedItems.Count != 1)
                return;

            _selectedPid = int.Parse(listProcesses.SelectedItems[0].SubItems[1].Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshProcesses();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.ChooseProcess();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listProcesses_DoubleClick(object sender, EventArgs e)
        {
            this.ChooseProcess();
        }

        private void listProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = listProcesses.SelectedItems.Count == 1;
        }
    }
}
