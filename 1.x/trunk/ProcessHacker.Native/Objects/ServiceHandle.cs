/*
 * Process Hacker - 
 *   service handle
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows service.
    /// </summary>
    public sealed class ServiceHandle : ServiceBaseHandle<ServiceAccess>
    {
        /// <summary>
        /// Creates a service handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <returns>The service handle.</returns>
        public static ServiceHandle FromHandle(IntPtr handle)
        {
            return new ServiceHandle(handle, false);
        }

        public static ServiceHandle OpenWithAnyAccess(string serviceName)
        {
            try
            {
                return new ServiceHandle(serviceName, ServiceAccess.QueryStatus);
            }
            catch
            {
                try
                {
                    return new ServiceHandle(serviceName, (ServiceAccess)StandardRights.Synchronize);
                }
                catch
                {
                    try
                    {
                        return new ServiceHandle(serviceName, (ServiceAccess)StandardRights.ReadControl);
                    }
                    catch
                    {
                        try
                        {
                            return new ServiceHandle(serviceName, (ServiceAccess)StandardRights.WriteDac);
                        }
                        catch
                        {
                            return new ServiceHandle(serviceName, (ServiceAccess)StandardRights.WriteOwner);
                        }
                    }
                }
            }
        }

        internal ServiceHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new service handle.
        /// </summary>
        /// <param name="serviceName">The name of the service to open.</param>
        public ServiceHandle(string serviceName)
            : this(serviceName, ServiceAccess.All)
        { }

        /// <summary>
        /// Creates a new service handle.
        /// </summary>
        /// <param name="serviceName">The name of the service to open.</param>
        /// <param name="access">The desired access to the service.</param>
        public ServiceHandle(string serviceName, ServiceAccess access)
        {
            using (ServiceManagerHandle manager =
                new ServiceManagerHandle(ScManagerAccess.Connect))
            {
                this.Handle = Win32.OpenService(manager, serviceName, access);

                if (this.Handle == IntPtr.Zero)
                {
                    this.MarkAsInvalid();
                    Win32.Throw();
                }
            }
        }

        /// <summary>
        /// Sends a control message to the service.
        /// </summary>
        /// <param name="control">The message.</param>
        public void Control(ServiceControl control)
        {
            ServiceStatus status = new ServiceStatus();

            if (!Win32.ControlService(this, control, out status))
                Win32.Throw();
        }

        /// <summary>
        /// Deletes the service.
        /// </summary>
        public void Delete()
        {
            if (!Win32.DeleteService(this))
                Win32.Throw();
        }

        /// <summary>
        /// Gets the service's configuration.
        /// </summary>
        public QueryServiceConfig GetConfig()
        {
            int requiredSize = 0;

            Win32.QueryServiceConfig(this, IntPtr.Zero, 0, out requiredSize);

            using (MemoryAlloc data = new MemoryAlloc(requiredSize))
            {
                if (!Win32.QueryServiceConfig(this, data, data.Size, out requiredSize))
                    Win32.Throw();

                return data.ReadStruct<QueryServiceConfig>();
            }
        }

        /// <summary>
        /// Gets the service's description.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetDescription()
        {
            int retLen;

            Win32.QueryServiceConfig2(this, ServiceInfoLevel.Description, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!Win32.QueryServiceConfig2(this, ServiceInfoLevel.Description, data, retLen, out retLen))
                    Win32.Throw();

                return data.ReadStruct<ServiceDescription>().Description;
            }
        }

        public override SecurityDescriptor GetSecurity(SecurityInformation securityInformation)
        {
            return this.GetSecurity(SeObjectType.Service, securityInformation);
        }

        /// <summary>
        /// Gets the status of the service.
        /// </summary>
        /// <returns>A SERVICE_STATUS_PROCESS structure.</returns>
        public ServiceStatusProcess GetStatus()
        {
            ServiceStatusProcess status;
            int retLen;

            if (!Win32.QueryServiceStatusEx(this, 0, out status, Marshal.SizeOf(typeof(ServiceStatusProcess)), out retLen))
                Win32.Throw();

            return status;
        }

        public override void SetSecurity(SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
        {
            this.SetSecurity(SeObjectType.Service, securityInformation, securityDescriptor);
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Start()
        {
            if (!Win32.StartService(this, 0, null))
                Win32.Throw();
        }
    }
}