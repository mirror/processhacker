/*
 * Process Hacker - 
 *   get-procedure-address tool
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
using System.Windows.Forms;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class GetProcAddressWindow : Form
    {
        private string _fileName;

        public GetProcAddressWindow(string fileName)
        {
            InitializeComponent();

            _fileName = fileName;

            textProcName.Focus();
            textProcName.Select();
        }

        private void buttonLookup_Click(object sender, EventArgs e)
        {
            bool loaded = Win32.GetModuleHandle(_fileName) != 0;
            int module;
            int address = 0;
            int ordinal = 0;

            if (loaded)
                module = Win32.GetModuleHandle(_fileName);
            else
                module = Win32.LoadLibraryEx(_fileName, 0, Win32.DontResolveDllReferences);

            if (module == 0)
            {
                textProcAddress.Text = "Could not load library!";
            }

            if ((textProcName.Text.Length > 0) &&
                (textProcName.Text[0] >= '0' && textProcName.Text[0] <= '9'))
                ordinal = (int)BaseConverter.ToNumberParse(textProcName.Text, false);

            if (ordinal != 0)
            {
                address = Win32.GetProcAddress(module, ordinal);
            }
            else
            {
                address = Win32.GetProcAddress(module, textProcName.Text);
            }

            if (address != 0)
            {
                textProcAddress.Text = String.Format("0x{0:x8}", address);
                textProcAddress.SelectAll();
                textProcAddress.Focus();
            }
            else
            {
                textProcAddress.Text = Win32.GetLastErrorMessage();
            }

            // don't unload libraries we had before
            if (module != 0 && !loaded)
                Win32.FreeLibrary(module);
        }

        private void textProcName_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonLookup;
        }

        private void textProcName_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
