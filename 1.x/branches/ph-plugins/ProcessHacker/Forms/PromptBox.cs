/*
 * Process Hacker - 
 *   easy-to-use prompt box
 * 
 * Copyright (C) 2008 wj32
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class PromptBox : Form
    {
        private string _value;
        public static string LastValue;

        public string Value
        {
            get { return _value; }
        }

        public PromptBox() : this("", false) { }

        public PromptBox(string value) : this(value, false) { }

        public PromptBox(bool multiline) : this("", multiline) { }

        public PromptBox(string value, bool multiline)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            if (value == "")
            {
                textValue.Text = LastValue;
            }
            else
            {
                textValue.Text = value;
            }

            if (multiline)
            {
                textValue.Multiline = true;
                textValue.ScrollBars = ScrollBars.Vertical;
                this.Size = new Size(this.Size.Width, this.Size.Height + 100);
                this.AcceptButton = null;
            }
            else
            {
                this.AcceptButton = buttonOK;
            }
        }

        public TextBox TextBox
        {
            get { return textValue; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _value = textValue.Text;
            LastValue = _value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
