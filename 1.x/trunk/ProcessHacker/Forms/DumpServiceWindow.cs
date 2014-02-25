/*
 * Process Hacker - 
 *   dump viewer (service)
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
using System.Windows.Forms;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Mfs;

namespace ProcessHacker
{
    public partial class DumpServiceWindow : Form
    {
        private MemoryObject _serviceMo;

        public DumpServiceWindow(ServiceItem item, MemoryObject serviceMo)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            _serviceMo = serviceMo;

            this.Text = "Service - " + item.Status.ServiceName;

            labelServiceName.Text = item.Status.ServiceName;
            labelServiceDisplayName.Text = item.Status.DisplayName;
            textServiceType.Text = item.Config.ServiceType.ToString();

            if (item.Config.ServiceType == (ServiceType.Win32OwnProcess | ServiceType.InteractiveProcess))
                textServiceType.Text = "Win32OwnProcess, InteractiveProcess";
            else if (item.Config.ServiceType == (ServiceType.Win32ShareProcess | ServiceType.InteractiveProcess))
                textServiceType.Text = "Win32ShareProcess, InteractiveProcess";

            textStartType.Text = item.Config.StartType.ToString();
            textErrorControl.Text = item.Config.ErrorControl.ToString();
            textLoadOrderGroup.Text = item.Config.LoadOrderGroup;
            textServiceBinaryPath.Text = item.Config.BinaryPathName;
            textUserAccount.Text = item.Config.ServiceStartName;
            textServiceDll.Text = item.ServiceDll;

            try
            {
                var dict = Dump.GetDictionary(_serviceMo);

                if (dict.ContainsKey("Description"))
                    textDescription.Text = dict["Description"];
            }
            catch
            { }
        }

        private void DumpServiceWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serviceMo.Dispose();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
