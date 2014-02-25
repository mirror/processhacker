/*
 * Process Hacker - 
 *   struct viewer window
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
using ProcessHacker.Components;
using ProcessHacker.Structs;

namespace ProcessHacker
{
    public partial class StructWindow : Form
    {
        private int _pid;
        private IntPtr _address;
        private StructDef _struct;

        public StructWindow(int pid, IntPtr address, StructDef struc)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _pid = pid;
            _address = address;
            _struct = struc;
        }

        private void StructWindow_Load(object sender, EventArgs e)
        {
            StructViewer sv = new StructViewer(_pid, _address, _struct);

            if (sv.Error)
                this.Close();

            sv.Dock = DockStyle.Fill;
            panel.Controls.Add(sv);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
