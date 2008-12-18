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
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle to a Windows service.
        /// </summary>
        public class ServiceHandle : Win32Handle
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

            private ServiceHandle(int Handle, bool Owned)
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
                int manager = OpenSCManager(0, 0, SC_MANAGER_RIGHTS.SC_MANAGER_CONNECT);

                if (manager == 0)
                    throw new Exception(GetLastErrorMessage());

                this.Handle = OpenService(manager, ServiceName, access);

                CloseServiceHandle(manager);

                if (this.Handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Sends a control message to the service.
            /// </summary>
            /// <param name="control">The message.</param>
            public void Control(SERVICE_CONTROL control)
            {
                SERVICE_STATUS status = new SERVICE_STATUS();

                if (ControlService(this.Handle, control, ref status) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Starts the service.
            /// </summary>
            public void Start()
            {
                if (StartService(this.Handle, 0, 0) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            /// <summary>
            /// Deletes the service.
            /// </summary>
            public void Delete()
            {
                if (DeleteService(this.Handle) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            protected override void Close()
            {
                CloseServiceHandle(this.Handle);
            }
        }
    }
}
