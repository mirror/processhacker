/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
            comboUsername.Text = Properties.Settings.Default.RunAsUsername;
            textCmdLine.Text = Properties.Settings.Default.RunAsCommand;
            textCmdLine.Focus();
            textCmdLine.Select();
        }

        private void RunWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.RunAsCommand = textCmdLine.Text;
            Properties.Settings.Default.RunAsUsername = comboUsername.Text;
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
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            try
            {
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();

                info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                info.FileName = Application.StartupPath + "\\Assistant.exe";
                info.Arguments = "-w";
                
                System.Diagnostics.Process.Start(info);
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

                if ((service = Win32.CreateService(manager, serviceName, serviceName + " (Process Hacker Assistant)", 
                    Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS,
                    Win32.SERVICE_TYPE.Win32OwnProcess, Win32.SERVICE_START_TYPE.DemandStart, Win32.SERVICE_ERROR_CONTROL.Ignore,
                    "\"" + Application.StartupPath + "\\Assistant.exe\" -u \"" + comboUsername.Text + "\" -p \"" +
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

            this.Cursor = Cursors.Default;
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
