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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class RunWindow : Form
    {
        private int _pid = -1;

        public RunWindow()
        {
            InitializeComponent();
            
            textSessionID.Text = Program.CurrentSessionId.ToString();
            comboType.SelectedItem = "Interactive";
        }

        public void UsePID(int PID)
        {
            _pid = PID;

            try
            {
                comboUsername.Text = Program.HackerWindow.ProcessProvider.Dictionary[PID].UsernameWithDomain;
            }
            catch
            {
                _pid = -1;
                return;
            }

            comboUsername.Enabled = false;
            comboType.Enabled = false;
            textPassword.Enabled = false;
        }

        private void RunWindow_Load(object sender, EventArgs e)
        {
            if (_pid == -1)
            {
                comboUsername.Text = Properties.Settings.Default.RunAsUsername;
            }

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

                bool omitUserAndType = false;

                if (_pid != -1)
                    omitUserAndType = true;

                if ((service = Win32.CreateService(manager, serviceName, serviceName + " (Process Hacker Assistant)", 
                    Win32.SERVICE_RIGHTS.SERVICE_ALL_ACCESS,
                    Win32.SERVICE_TYPE.Win32OwnProcess, Win32.SERVICE_START_TYPE.DemandStart, Win32.SERVICE_ERROR_CONTROL.Ignore,
                    "\"" + Application.StartupPath + "\\Assistant.exe\" " + 
                    (omitUserAndType ? "" : 
                    ("-u \"" + comboUsername.Text + "\" -t " + comboType.SelectedItem.ToString().ToLower() + " ")) + 
                    (_pid != -1 ? ("-P " + _pid.ToString() + " ") : "") + "-p \"" +
                    Misc.EscapeString(textPassword.Text) + "\" -s " + textSessionID.Text + " -c \"" +
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
            if (comboUsername.Text.ToUpper() == "NT AUTHORITY\\SYSTEM" || 
                comboUsername.Text.ToUpper() == "NT AUTHORITY\\LOCAL SERVICE" ||
                comboUsername.Text.ToUpper() == "NT AUTHORITY\\NETWORK SERVICE")
                return true;
            else
                return false;
        }

        private void comboUsername_TextChanged(object sender, EventArgs e)
        {
            if (_pid == -1)
            {
                if (isServiceUser())
                {
                    textPassword.Enabled = false;
                    comboType.SelectedItem = "Service";

                    if (comboUsername.Text.ToUpper() == "NT AUTHORITY\\SYSTEM" && Program.WindowsVersion == "XP")
                        comboType.SelectedItem = "NewCredentials";
                }
                else
                {
                    textPassword.Enabled = true;
                    comboType.SelectedItem = "Interactive";
                }
            }
        }

        private void buttonSessions_Click(object sender, EventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            foreach (Win32.WTS_SESSION_INFO session in Win32.TSEnumSessions())
            {
                MenuItem item = new MenuItem();
                string user = null;
                string domain = null;
                int retLen = 0;
                                                                                                                                   
                Win32.WTSQuerySessionInformation(0, session.SessionID, Win32.WTS_INFO_CLASS.WTSUserName, ref user, ref retLen);
                Win32.WTSQuerySessionInformation(0, session.SessionID, Win32.WTS_INFO_CLASS.WTSDomainName, ref domain, ref retLen);

                string username = domain + "\\" + user;

                item.Text = session.SessionID.ToString() + ": " + session.WinStationName + 
                    (username != "\\" ? (" (" + username + ")") : "");
                item.Tag = session.SessionID;
                item.Click += new EventHandler(item_Click);

                Win32.WTSFreeMemory(user);
                Win32.WTSFreeMemory(domain);

                menu.MenuItems.Add(item);
            }

            menu.Show(buttonSessions, new Point(buttonSessions.Width, 0));
        }

        private void item_Click(object sender, EventArgs e)
        {
            textSessionID.Text = ((MenuItem)sender).Tag.ToString();
        }
    }
}
