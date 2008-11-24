/*
 * Process Hacker
 * 
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ProcessHacker
{
    public class ServiceProvider : Provider<string, Win32.ENUM_SERVICE_STATUS_PROCESS>
    {
        public ServiceProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        private void UpdateOnce()
        {
            Dictionary<string, Win32.ENUM_SERVICE_STATUS_PROCESS> newdictionary
                = Win32.EnumServices();

            // check for removed services
            foreach (string s in Dictionary.Keys)
            {
                if (!newdictionary.ContainsKey(s))
                {
                    Win32.ENUM_SERVICE_STATUS_PROCESS service = Dictionary[s];

                    this.CallDictionaryRemoved(service);
                    Dictionary.Remove(s);
                }
            }

            // check for new services
            foreach (string s in newdictionary.Keys)
            {
                if (!Dictionary.ContainsKey(s))
                {
                    this.CallDictionaryAdded(newdictionary[s]);
                    Dictionary.Add(s, newdictionary[s]);
                }
            }

            // check for modified services
            foreach (Win32.ENUM_SERVICE_STATUS_PROCESS service in Dictionary.Values)
            {
                Win32.ENUM_SERVICE_STATUS_PROCESS newservice = newdictionary[service.ServiceName];
                bool modified = false;

                if (service.DisplayName != newservice.DisplayName)
                    modified = true;
                else if (service.ServiceStatusProcess.ControlsAccepted != newservice.ServiceStatusProcess.ControlsAccepted)
                    modified = true;
                else if (service.ServiceStatusProcess.CurrentState != newservice.ServiceStatusProcess.CurrentState)
                    modified = true;
                else if (service.ServiceStatusProcess.ProcessID != newservice.ServiceStatusProcess.ProcessID)
                    modified = true;
                else if (service.ServiceStatusProcess.ServiceFlags != newservice.ServiceStatusProcess.ServiceFlags)
                    modified = true;
                else if (service.ServiceStatusProcess.ServiceType != newservice.ServiceStatusProcess.ServiceType)
                    modified = true;

                if (modified)
                {
                    Dictionary[service.ServiceName] = newservice;
                    this.CallDictionaryModified(newservice);
                }
            }
        }
    }
}
