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
    public struct ServiceItem
    {
        public Win32.ENUM_SERVICE_STATUS_PROCESS Status;
        public Win32.QUERY_SERVICE_CONFIG Config;
    }

    public class ServiceProvider : Provider<string, ServiceItem>
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
                    ServiceItem service = Dictionary[s];

                    this.CallDictionaryRemoved(service);
                    Dictionary.Remove(s);
                }
            }

            // check for new services
            foreach (string s in newdictionary.Keys)
            {
                if (!Dictionary.ContainsKey(s))
                {
                    ServiceItem item = new ServiceItem();

                    item.Status = newdictionary[s];

                    try
                    {
                        item.Config = Win32.GetServiceConfig(s);
                    }
                    catch
                    { }

                    this.CallDictionaryAdded(item);
                    Dictionary.Add(s, item);
                }
            }

            // check for modified services
            foreach (ServiceItem service in Dictionary.Values)
            {
                ServiceItem newserviceitem = service;

                newserviceitem.Status = newdictionary[service.Status.ServiceName];

                try
                {
                    newserviceitem.Config = Win32.GetServiceConfig(service.Status.ServiceName);
                }
                catch
                { }

                bool modified = false;

                if (service.Status.DisplayName != newserviceitem.Status.DisplayName)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ControlsAccepted != 
                    newserviceitem.Status.ServiceStatusProcess.ControlsAccepted)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.CurrentState != 
                    newserviceitem.Status.ServiceStatusProcess.CurrentState)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ProcessID != 
                    newserviceitem.Status.ServiceStatusProcess.ProcessID)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ServiceFlags != 
                    newserviceitem.Status.ServiceStatusProcess.ServiceFlags)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ServiceType != 
                    newserviceitem.Status.ServiceStatusProcess.ServiceType)
                    modified = true;

                if (modified)
                {
                    Dictionary[service.Status.ServiceName] = newserviceitem;
                    this.CallDictionaryModified(newserviceitem);
                }
            }
        }
    }
}
