/*
 * Process Hacker - 
 *   run as window
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
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public partial class RunWindow : Form
    {
        private int _pid = -1;

        public RunWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            textSessionID.Text = Program.CurrentSessionId.ToString();
            comboType.SelectedItem = "Interactive";

            if (Program.ElevationType == TokenElevationType.Limited)
                buttonOK.SetShieldIcon(true);

            List<string> users = new List<string>();

            users.Add("NT AUTHORITY\\SYSTEM");
            users.Add("NT AUTHORITY\\LOCAL SERVICE");
            users.Add("NT AUTHORITY\\NETWORK SERVICE");

            try
            {
                using (var phandle = new LsaPolicyHandle(LsaPolicyAccess.ViewLocalInformation))
                {
                    foreach (var sid in phandle.GetAccounts())
                        if (sid.NameUse == SidNameUse.User)
                            users.Add(sid.GetFullName(true));
                }
            }
            catch
            { }

            users.Sort();

            comboUsername.Items.AddRange(users.ToArray());
        }

        public void UsePID(int PID)
        {
            _pid = PID;

            try
            {
                comboUsername.Text = Program.ProcessProvider.Dictionary[PID].Username;
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
                comboUsername.Text = Settings.Instance.RunAsUsername;
            }

            textCmdLine.Text = Settings.Instance.RunAsCommand;
            textCmdLine.Select();
        }

        private void RunWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Instance.RunAsCommand = textCmdLine.Text;
            Settings.Instance.RunAsUsername = comboUsername.Text;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            try
            {
                ofd.FileName = textCmdLine.Text;
            }
            catch
            { }

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
                Assistant.SetDesktopWinStaAccess();
            }
            catch
            { }

            try
            {
                string binPath;
                string mailslotName;
                bool omitUserAndType = false;

                if (_pid != -1)
                    omitUserAndType = true;

                mailslotName = "ProcessHackerAssistant" + Utils.CreateRandomString(8);
                binPath = "\"" + Application.ExecutablePath + "\" -assistant " +
                    (omitUserAndType ? "" :
                    ("-u \"" + comboUsername.Text + "\" -t " + comboType.SelectedItem.ToString().ToLowerInvariant() + " ")) +
                    (_pid != -1 ? ("-P " + _pid.ToString() + " ") : "") + "-p \"" +
                    textPassword.Text.Replace("\"", "\\\"") + "\" -s " + textSessionID.Text + " -c \"" +
                    textCmdLine.Text.Replace("\"", "\\\"") + "\" -E " + mailslotName;

                if (Program.ElevationType == TokenElevationType.Limited)
                {
                    var result = Program.StartProcessHackerAdminWait(
                        "-e -type processhacker -action runas -obj \"" + binPath.Replace("\"", "\\\"") +
                        "\" -mailslot " + mailslotName +
                        " -hwnd " + this.Handle.ToString(), this.Handle, 5000);

                    if (result == WaitResult.Object0)
                        this.Close();
                }
                else
                {
                    string serviceName = Utils.CreateRandomString(8);

                    using (var manager = new ServiceManagerHandle(ScManagerAccess.CreateService))
                    {
                        using (var service = manager.CreateService(
                            serviceName,
                            serviceName + " (Process Hacker Assistant)",
                            ServiceType.Win32OwnProcess,
                            ServiceStartType.DemandStart,
                            ServiceErrorControl.Ignore,
                            binPath,
                            "",
                            "LocalSystem",
                            null))
                        {
                            // Create a mailslot so we can receive the error code for Assistant.
                            using (var mhandle = MailslotHandle.Create(
                                FileAccess.GenericRead, @"\Device\Mailslot\" + mailslotName, 0, 5000)
                                )
                            {
                                try { service.Start(); }
                                catch { }
                                service.Delete();

                                Win32Error errorCode = (Win32Error)mhandle.Read(4).ToInt32();

                                if (errorCode != Win32Error.Success)
                                    throw new WindowsException(errorCode);
                            }
                        }
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to start the program", ex);
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

                    // hack for XP
                    if (comboUsername.Text.ToUpper() == "NT AUTHORITY\\SYSTEM" && 
                        OSVersion.IsBelowOrEqual(WindowsVersion.XP))
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

            foreach (var session in TerminalServerHandle.GetCurrent().GetSessions())
            {
                MenuItem item = new MenuItem();

                string userName = session.DomainName + "\\" + session.UserName;
                string displayName = session.SessionId.ToString();

                if (!string.IsNullOrEmpty(session.Name))
                    displayName += ": " + session.Name + (userName != "\\" ? (" (" + userName + ")") : "");
                else if (userName != "\\")
                    displayName += ": " + userName;

                item.Text = displayName;
                item.Tag = session.SessionId;
                item.Click += new EventHandler(item_Click);

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
