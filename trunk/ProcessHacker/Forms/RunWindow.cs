using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class RunWindow : Form
    {
        public RunWindow()
        {
            InitializeComponent();

            textSessionID.Text = Program.CurrentSessionId.ToString();
        }

        private void RunWindow_Load(object sender, EventArgs e)
        {
            textCmdLine.Text = PromptBox.LastValue;
            textCmdLine.Focus();
            textCmdLine.Select();
        }

        private void RunWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            PromptBox.LastValue = textCmdLine.Text;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.FileName = textCmdLine.Text;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
                textCmdLine.Text = ofd.FileName;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("\"" + Application.StartupPath + "\\Assistant.exe\" -w");
            }
            catch
            { }

            int manager = 0;
            int service = 0;

            try
            {
                if ((manager = Win32.OpenSCManager(0, 0, Win32.SC_MANAGER_RIGHTS.SC_MANAGER_CREATE_SERVICE)) == 0)
                    throw new Exception("Could not open the service manager: " + Win32.GetLastErrorMessage());

                Random r = new Random((int)(DateTime.Now.ToFileTime() & 0xffffffff));
                string serviceName = "";

                for (int i = 0; i < 8; i++)
                    serviceName += (char)('A' + r.Next(25));

                string username = comboUsername.Text;

                if (username.Contains("\\"))
                    username = username.Split('\\')[1] + "@" + username.Split('\\')[0];

                if ((service = Win32.CreateService(manager, serviceName, "Process Hacker Assistant", 
                    Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS,
                    Win32.SERVICE_TYPE.Win32OwnProcess, Win32.SERVICE_START_TYPE.DemandStart, Win32.SERVICE_ERROR_CONTROL.Ignore,
                    "\"" + Application.StartupPath + "\\Assistant.exe\" -u \"" + username + "\" -p \"" +
                    Misc.EscapeString(textPassword.Text) + "\" -t " +
                    (isServiceUser() ? "service" : "interactive") + " -s " + textSessionID.Text + " -c \"" +
                    Misc.EscapeString(textCmdLine.Text) + "\"", "", 0, 0, "LocalSystem", "")) == 0)
                    throw new Exception("Could not create service: " + Win32.GetLastErrorMessage());

                Win32.StartService(service, 0, 0);
                Win32.DeleteService(service);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Win32.CloseHandle(manager);
                Win32.CloseHandle(service);
            }
        }

        private bool isServiceUser()
        {
            if (comboUsername.Text == "NT AUTHORITY\\SYSTEM" || comboUsername.Text == "NT AUTHORITY\\LOCAL SERVICE" ||
                comboUsername.Text == "NT AUTHORITY\\NETWORK SERVICE")
                return true;
            else
                return false;
        }

        private void comboUsername_TextChanged(object sender, EventArgs e)
        {
            if (isServiceUser())
                textPassword.Enabled = false;
            else
                textPassword.Enabled = true;
        }
    }
}
