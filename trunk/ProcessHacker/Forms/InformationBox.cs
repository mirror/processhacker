/*
 * Process Hacker - 
 *   simple-to-use text display box
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
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class InformationBox : Form
    {
        public InformationBox(string values)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            if (!Program.BadConfig)
                this.Size = Settings.Instance.InformationBoxSize;

            textValues.Text = values;
            textValues.Select(0, 0);
        }

        private void InformationBox_Load(object sender, EventArgs e)
        {
            // doesn't work???
            textValues.Select();
            textValues.ScrollToCaret();
        }

        private void InformationBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Program.BadConfig)
                Settings.Instance.InformationBoxSize = this.Size;
        }

        public TextBox TextBox { get { return textValues; } }

        public string DefaultFileName { get; set; }

        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public bool ShowSaveButton
        {
            get { return buttonSave.Visible; }
            set { buttonSave.Visible = value; }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = DefaultFileName;
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
                System.IO.File.WriteAllText(sfd.FileName, textValues.Text);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (textValues.Text.Length == 0)
                return;

            if (textValues.SelectionLength == 0)
            {
                Clipboard.SetText(textValues.Text);
                textValues.Select();
                textValues.SelectAll();
            }
            else
            {
                Clipboard.SetText(textValues.SelectedText);
            }
        }

        private void InformationBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textValues.SelectAll();    
                e.Handled = true;
            }
        }
    }
}
