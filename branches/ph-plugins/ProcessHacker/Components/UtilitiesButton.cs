/*
 * Process Hacker - 
 *   button for inserting various data
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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using ProcessHacker.Common;

namespace ProcessHacker.Components
{
    public partial class UtilitiesButton : UserControl
    {
        private HexBox _hexbox;

        public UtilitiesButton()
        {
            InitializeComponent();

            this.Size = new Size(24, 24);
        }

        [Category("General"), Description("The HexBox which is modified by this control")]
        public HexBox HexBox
        {
            get { return _hexbox; }
            set { _hexbox = value; }
        }

        public override ContextMenu ContextMenu
        {
            get { return menuUtilities; }
        }

        private void InsertNumber(HexBox _hexbox, int byteCount, bool BigEndian)
        {
            PromptBox prompt = new PromptBox();

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                byte[] bytes = new byte[byteCount];
                long number = (long)BaseConverter.ToNumberParse(prompt.Value);

                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[BigEndian ? (bytes.Length - i - 1) : i] = (byte)((number >> (i * 8)) & 0xff);
                }

                _hexbox.ByteProvider.DeleteBytes(_hexbox.SelectionStart, _hexbox.SelectionLength);
                _hexbox.ByteProvider.InsertBytes(_hexbox.SelectionStart, bytes);
                _hexbox.Select(_hexbox.SelectionStart, bytes.Length);
            }
        }

        private void InsertUsingEncoding(Encoding encoding, HexBox _hexbox, bool Multiline)
        {
            PromptBox prompt = new PromptBox(Multiline);

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                byte[] bytes = new byte[prompt.Value.Length * encoding.GetByteCount("A")];

                encoding.GetBytes(prompt.Value, 0, prompt.Value.Length, bytes, 0);

                _hexbox.ByteProvider.DeleteBytes(_hexbox.SelectionStart, _hexbox.SelectionLength);
                _hexbox.ByteProvider.InsertBytes(_hexbox.SelectionStart, bytes);
                _hexbox.Select(_hexbox.SelectionStart, bytes.Length);
            }
        }

        private void buttonUtilities_Click(object sender, System.EventArgs e)
        {
            menuUtilities.Show(buttonUtilities, new Point(
             buttonUtilities.Size.Width,
             0));
        }

        #region Insert Number

        private void bitMenuItem_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 1, false);
        }

        private void bitLittleEndianMenuItem_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 2, false);
        }

        private void bitBigEndianMenuItem_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 2, true);
        }

        private void bitLittleEndianMenuItem1_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 4, false);
        }

        private void bitBigEndianMenuItem1_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 4, true);
        }

        private void bitLittleEndianMenuItem2_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 8, false);
        }

        private void bitBigEndianMenuItem2_Click(object sender, EventArgs e)
        {
            InsertNumber(_hexbox, 8, true);
        }

        #endregion

        #region Insert String

        private void aSCIIMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.ASCII, _hexbox, false);
        }

        private void uTF8MenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.UTF8, _hexbox, false);
        }

        private void uTF16MenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.Unicode, _hexbox, false);
        }

        private void uTF16BigEndianMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.BigEndianUnicode, _hexbox, false);
        }

        private void uTF32MenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.UTF32, _hexbox, false);
        }

        private void aSCIIMultilineMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.ASCII, _hexbox, true);
        }

        private void uTF8MultilineMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.UTF8, _hexbox, true);
        }

        private void uTF16MultilineMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.Unicode, _hexbox, true);
        }

        private void uTF16BigEndianMultilineMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.BigEndianUnicode, _hexbox, true);
        }

        private void uTF32MultilineMenuItem_Click(object sender, EventArgs e)
        {
            InsertUsingEncoding(UnicodeEncoding.UTF32, _hexbox, true);
        }

        #endregion
    }
}
