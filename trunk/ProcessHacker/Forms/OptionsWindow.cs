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
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;

namespace ProcessHacker
{
    public partial class OptionsWindow : Form
    {
        public OptionsWindow()
        {
            InitializeComponent();

            textUpdateInterval.Value = Properties.Settings.Default.RefreshInterval;
            textSearchEngine.Text = Properties.Settings.Default.SearchEngine;
            checkWarnDangerous.Checked = Properties.Settings.Default.WarnDangerous;
            checkShowProcessDomains.Checked = Properties.Settings.Default.ShowAccountDomains;
            checkHideHandlesNoName.Checked = Properties.Settings.Default.HideHandlesNoName;
            checkShowTrayIcon.Checked = Properties.Settings.Default.ShowIcon;
            checkHideWhenMinimized.Checked = Properties.Settings.Default.HideWhenMinimized;

            textHighlightingDuration.Value = Properties.Settings.Default.HighlightingDuration;
            colorNewProcesses.Color = Properties.Settings.Default.ColorNewProcesses;
            colorRemovedProcesses.Color = Properties.Settings.Default.ColorRemovedProcesses;
            colorOwnProcesses.Color = Properties.Settings.Default.ColorOwnProcesses;
            colorSystemProcesses.Color = Properties.Settings.Default.ColorSystemProcesses;
            colorServiceProcesses.Color = Properties.Settings.Default.ColorServiceProcesses;
            colorBeingDebugged.Color = Properties.Settings.Default.ColorBeingDebugged;
            colorElevatedProcesses.Color = Properties.Settings.Default.ColorElevatedProcesses;
        }

        private void textUpdateInterval_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.RefreshInterval = Int32.Parse(textUpdateInterval.Value.ToString());
            }
            catch
            {
                MessageBox.Show("The entered value is not valid.", "Process Hacker", MessageBoxButtons.OK,
                 MessageBoxIcon.Error);

                textUpdateInterval.Focus();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SearchEngine = textSearchEngine.Text;
            Properties.Settings.Default.WarnDangerous = checkWarnDangerous.Checked;
            Properties.Settings.Default.ShowAccountDomains = checkShowProcessDomains.Checked;
            Properties.Settings.Default.HideHandlesNoName = checkHideHandlesNoName.Checked;
            Properties.Settings.Default.ShowIcon = checkShowTrayIcon.Checked;
            Properties.Settings.Default.HideWhenMinimized = checkHideWhenMinimized.Checked;

            Program.HackerWindow.NotifyIcon.Visible = Properties.Settings.Default.ShowIcon;
            Program.HackerWindow.ProcessProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.HackerWindow.ServiceProvider.Interval = Properties.Settings.Default.RefreshInterval;

            Properties.Settings.Default.HighlightingDuration = (int)textHighlightingDuration.Value;
            Properties.Settings.Default.ColorNewProcesses = colorNewProcesses.Color;
            Properties.Settings.Default.ColorRemovedProcesses = colorRemovedProcesses.Color;
            Properties.Settings.Default.ColorOwnProcesses = colorOwnProcesses.Color;
            Properties.Settings.Default.ColorSystemProcesses = colorSystemProcesses.Color;
            Properties.Settings.Default.ColorServiceProcesses = colorServiceProcesses.Color;
            Properties.Settings.Default.ColorBeingDebugged = colorBeingDebugged.Color;
            Properties.Settings.Default.ColorElevatedProcesses = colorElevatedProcesses.Color;

            HighlightedListViewItem.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            HighlightedListViewItem.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNewProcesses;
            HighlightedListViewItem.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Properties.Settings.Default.ColorNewProcesses;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Properties.Settings.Default.ColorRemovedProcesses;

            Properties.Settings.Default.Save();
            Program.HackerWindow.ProcessList.RefreshItems();
            this.Close();
        }
    }
}
