/* 
 * 
 * Process Hacker - 
 *   process picker window
 * 
 * Copyright (C) 2009 wj32
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ProcessPickerWindow : Form
    {
        public ProcessPickerWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
        }

        public int SelectedPid { get; private set; }

        public string Label
        {
            get { return labelLabel.Text; }
            set { labelLabel.Text = value; }
        }

        private void ProcessPickerWindow_Load(object sender, EventArgs e)
        {
            treeProcesses.Tree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Single;
            treeProcesses.Provider = Program.ProcessProvider;
        }

        private void ProcessPickerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            treeProcesses.Provider = null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SelectedPid = treeProcesses.SelectedNodes[0].PID;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void treeProcesses_SelectionChanged(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 1)
                buttonOK.Enabled = true;
            else
                buttonOK.Enabled = false;
        }

        private void treeProcesses_DoubleClick(object sender, EventArgs e)
        {
            if (treeProcesses.SelectedNodes.Count == 1)
                buttonOK_Click(sender, e);
        }
    }
}
