/*
 * Process Hacker - 
 *   service properties window
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
using ProcessHacker.Components;

namespace ProcessHacker
{
    public partial class ServiceWindow : Form
    {
        private ServiceProperties _serviceProps;

        public ServiceWindow(string service)
            : this(new string[] { service })
        { }

        public ServiceWindow(string[] services)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _serviceProps = new ServiceProperties(services);
            _serviceProps.Dock = DockStyle.Fill;
            _serviceProps.NeedsClose += new EventHandler(_serviceProps_NeedsClose);
            this.Controls.Add(_serviceProps);
            this.Text = _serviceProps.Text;
            this.AcceptButton = _serviceProps.ApplyButton;

            if (services.Length == 1)
                _serviceProps.ApplyButtonText = "&OK";
        }

        private void _serviceProps_NeedsClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ServiceWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serviceProps.SaveSettings();
            _serviceProps.Dispose();
        }
    }
}
