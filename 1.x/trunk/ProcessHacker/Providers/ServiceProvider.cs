/*
 * Process Hacker - 
 *   service provider
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
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public class ServiceItem : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int RunId;
        public EnumServiceStatusProcess Status;
        public QueryServiceConfig Config;
        public string ServiceDll;
    }

    public class ServiceProvider : Provider<string, ServiceItem>
    {
        public ServiceProvider()
            : base(StringComparer.InvariantCultureIgnoreCase) // Windows is case-insensitive with services
        {
            this.Name = this.GetType().Name;
        }

        public void UpdateServiceConfig(string name, QueryServiceConfig config)
        {
            ServiceItem item = Dictionary[name];

            Dictionary[name] = new ServiceItem()
            {
                Config = config,
                Status = item.Status
            };

            this.OnDictionaryModified(item, Dictionary[name]);
        }

        protected override void Update()
        {
            var newdictionary = Windows.GetServices();

            List<string> toRemove = null;

            // check for removed services
            foreach (string s in Dictionary.Keys)
            {
                if (!newdictionary.ContainsKey(s))
                {
                    ServiceItem service = Dictionary[s];

                    this.OnDictionaryRemoved(service);

                    if (toRemove == null)
                        toRemove = new List<string>();

                    toRemove.Add(s);
                }
            }

            if (toRemove != null)
            {
                foreach (var serviceName in toRemove)
                    Dictionary.Remove(serviceName);
            }

            // check for new services
            foreach (string s in newdictionary.Keys)
            {
                if (!Dictionary.ContainsKey(s))
                {
                    ServiceItem item = new ServiceItem();

                    item.RunId = this.RunCount;
                    item.Status = newdictionary[s];

                    try
                    {
                        using (var shandle = new ServiceHandle(s, ServiceAccess.QueryConfig))
                            item.Config = shandle.GetConfig();
                    }
                    catch
                    { }

                    this.OnDictionaryAdded(item);
                    Dictionary.Add(s, item);
                }
            }

            var toModify = new Dictionary<string, ServiceItem>();

            // check for modified services
            foreach (ServiceItem service in Dictionary.Values)
            {
                var newStatus = newdictionary[service.Status.ServiceName];

                bool modified = false;

                if (service.Status.DisplayName != newStatus.DisplayName)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ControlsAccepted !=
                    newStatus.ServiceStatusProcess.ControlsAccepted)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.CurrentState !=
                    newStatus.ServiceStatusProcess.CurrentState)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ProcessID !=
                    newStatus.ServiceStatusProcess.ProcessID)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ServiceFlags !=
                    newStatus.ServiceStatusProcess.ServiceFlags)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ServiceType !=
                    newStatus.ServiceStatusProcess.ServiceType)
                    modified = true;

                if (modified)
                {
                    var newServiceItem = new ServiceItem()
                    {
                        RunId = service.RunId,
                        Status = newStatus,
                        Config = service.Config
                    };

                    this.OnDictionaryModified(service, newServiceItem);
                    toModify.Add(service.Status.ServiceName, newServiceItem);
                }         
            }

            foreach (string serviceName in toModify.Keys)
                Dictionary[serviceName] = toModify[serviceName];
        }
    }
}
