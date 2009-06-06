/*
 * Process Hacker - 
 *   file name textbox with extra actions
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
using ProcessHacker.Common;
using ProcessHacker.Native;

namespace ProcessHacker.Components
{
    public partial class FileNameBox : UserControl
    {
        public FileNameBox()
        {
            InitializeComponent();
        }

        public event EventHandler TextBoxLeave;

        public bool TextBoxFocused
        {
            get { return textFileName.Focused; }
        }

        public bool ReadOnly
        {
            get { return textFileName.ReadOnly; }
            set { textFileName.ReadOnly = value; }
        }

        public override string Text
        {
            get
            {
                return textFileName.Text;
            }
            set
            {
                textFileName.Text = value;
            }
        }

        private void buttonProperties_Click(object sender, EventArgs e)
        {
            try
            {
                FileUtils.ShowProperties(textFileName.Text);
            }
            catch (Exception ex)
            {
                PhUtils.ShowMessage(ex);
            }
        }

        private void buttonExplore_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.ShowFileInExplorer(textFileName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textFileName_Leave(object sender, EventArgs e)
        {
            if (TextBoxLeave != null)
                TextBoxLeave(sender, e);
        }
    }
}
