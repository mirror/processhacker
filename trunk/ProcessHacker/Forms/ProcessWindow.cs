/*
 * Process Hacker
 * 
 * Copyright (C) 2008 Dean
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
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ProcessWindow : Form
    {
        private ProcessItem _processItem;
        private int _pid;
        private Process _process;

        private ThreadProvider _threadP;

        private TokenProperties _tokenProps;

        public ProcessWindow(ProcessItem process)
        {
            InitializeComponent();

            _processItem = process;
            _pid = process.PID;
            _process = Process.GetProcessById(_pid);

            this.Text = process.Name + " (PID " + _pid.ToString() + ")";

            if (process.Icon != null)
                this.Icon = process.Icon;
            else
                this.Icon = Program.HackerWindow.Icon;

            Program.PWindows.Add(_pid, this);
        }

        private void ProcessWindow_Load(object sender, EventArgs e)
        {
            this.Size = Properties.Settings.Default.ProcessWindowSize;

            if (tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab] != null)
                tabControl.SelectedTab = tabControl.TabPages[Properties.Settings.Default.ProcessWindowSelectedTab];

            Program.UpdateWindows();

            _tokenProps = new TokenProperties(_processItem.ProcessQueryLimitedHandle);
            _tokenProps.Dock = DockStyle.Fill;
            tabToken.Controls.Add(_tokenProps);

            _threadP = new ThreadProvider(_pid);
            _threadP.Interval = Properties.Settings.Default.RefreshInterval;
            _threadP.RunOnceAsync();
            listThreads.Provider = _threadP;
            _threadP.Enabled = true;
        }

        private void ProcessWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _threadP.Kill();
            listThreads.SaveSettings();
            _tokenProps.SaveSettings();

            Properties.Settings.Default.ProcessWindowSelectedTab = tabControl.SelectedTab.Name;
            Properties.Settings.Default.ProcessWindowSize = this.Size;
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        private void inspectImageFileMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path;

                if (_pid == 4)
                {
                    path = Misc.GetKernelFileName();
                }
                else
                {
                    path = Misc.GetRealPath(_process.MainModule.FileName);
                }

                PEWindow pw = Program.GetPEWindow(path,
                    new Program.PEWindowInvokeAction(delegate(PEWindow f)
                    {
                        try
                        {
                            f.Show();
                            f.Activate();
                        }
                        catch
                        { }
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inspecting:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
