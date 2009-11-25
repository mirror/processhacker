/*
 * Process Hacker - 
 *   service manager handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to the Windows service manager.
    /// </summary>
    public sealed class ServiceManagerHandle : ServiceBaseHandle<ScManagerAccess>
    {
        /// <summary>
        /// Connects to the Windows service manager.
        /// </summary>
        /// <param name="access">The desired access to the service manager.</param>
        public ServiceManagerHandle(ScManagerAccess access)
        {
            this.Handle = Win32.OpenSCManager(null, null, access);

            if (this.Handle == IntPtr.Zero)
            {
                this.MarkAsInvalid();
                Win32.Throw();
            }
        }

        public ServiceHandle CreateService(string name, string displayName,
            ServiceType type, string binaryPath)
        {
            return this.CreateService(name, displayName, type, ServiceStartType.DemandStart,
                ServiceErrorControl.Ignore, binaryPath, null, null, null);
        }

        public ServiceHandle CreateService(string name, string displayName,
            ServiceType type, ServiceStartType startType, string binaryPath)
        {
            return this.CreateService(name, displayName, type, startType,
                ServiceErrorControl.Ignore, binaryPath, null, null, null);
        }

        public ServiceHandle CreateService(string name, string displayName,
            ServiceType type, ServiceStartType startType, ServiceErrorControl errorControl,
            string binaryPath, string group, string accountName, string password)
        {
            IntPtr service;

            if ((service = Win32.CreateService(this, name, displayName, ServiceAccess.All,
                type, startType, errorControl, binaryPath, group,
                IntPtr.Zero, IntPtr.Zero, accountName, password)) == IntPtr.Zero)
                Win32.Throw();

            return new ServiceHandle(service, true);
        }
    }
}
