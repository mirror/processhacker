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

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows service.
        /// </summary>
        public class ServiceHandle : ServiceBaseHandle
        {
            /// <summary>
            /// Creates a service handle using an existing handle. 
            /// The handle will not be closed automatically.
            /// </summary>
            /// <param name="Handle">The handle value.</param>
            /// <returns>The service handle.</returns>
            public static ServiceHandle FromHandle(int Handle)
            {
                return new ServiceHandle(Handle, false);
            }

            internal ServiceHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            /// <summary>
            /// Creates a new service handle.
            /// </summary>
            /// <param name="ServiceName">The name of the service to open.</param>
            public ServiceHandle(string ServiceName)
                : this(ServiceName, SERVICE_RIGHTS.SERVICE_ALL_ACCESS)
            { }

            /// <summary>
            /// Creates a new service handle.
            /// </summary>
            /// <param name="ServiceName">The name of the service to open.</param>
            /// <param name="access">The desired access to the service.</param>
            public ServiceHandle(string ServiceName, SERVICE_RIGHTS access)
            {
                using (ServiceManagerHandle manager =
                    new ServiceManagerHandle(SC_MANAGER_RIGHTS.SC_MANAGER_CONNECT))
                {
                    this.Handle = OpenService(manager, ServiceName, access);

                    if (this.Handle == 0)
                        ThrowLastWin32Error();
                }
            }

            /// <summary>
            /// Sends a control message to the service.
            /// </summary>
            /// <param name="control">The message.</param>
            public void Control(SERVICE_CONTROL control)
            {
                SERVICE_STATUS status = new SERVICE_STATUS();

                if (!ControlService(this.Handle, control, ref status))
                    ThrowLastWin32Error();
            }        

            /// <summary>
            /// Deletes the service.
            /// </summary>
            public void Delete()
            {
                if (!DeleteService(this.Handle))
                    ThrowLastWin32Error();
            }

            /// <summary>
            /// Gets the service's description.
            /// </summary>
            /// <returns>A string.</returns>
            public string GetDescription()
            {
                int retLen;

                QueryServiceConfig2(this, SERVICE_INFO_LEVEL.Description, IntPtr.Zero, 0, out retLen);

                using (MemoryAlloc data = new MemoryAlloc(retLen))
                {
                    if (!QueryServiceConfig2(this, SERVICE_INFO_LEVEL.Description, data, retLen, out retLen))
                        ThrowLastWin32Error();

                    return data.ReadStruct<SERVICE_DESCRIPTION>().Description;
                }
            }

            /// <summary>
            /// Starts the service.
            /// </summary>
            public void Start()
            {
                if (!StartService(this.Handle, 0, 0))
                    ThrowLastWin32Error();
            }
        }
    }
}
