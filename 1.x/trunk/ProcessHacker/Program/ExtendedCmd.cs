/*
 * Process Hacker - 
 *   extended command line options
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
using System.Collections.Generic;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;
using ProcessHacker.UI.Actions;

namespace ProcessHacker
{
    public static class ExtendedCmd
    {
        public static void Run(IDictionary<string, string> args)
        {
            try
            {
                ThemingScope.Activate();
            }
            catch
            { }

            if (!args.ContainsKey("-type"))
                throw new Exception("-type switch required.");

            string type = args["-type"].ToLowerInvariant();

            if (!args.ContainsKey("-obj"))
                throw new Exception("-obj switch required.");

            string obj = args["-obj"];

            if (!args.ContainsKey("-action"))
                throw new Exception("-action switch required.");

            string action = args["-action"].ToLowerInvariant();

            WindowFromHandle window = new WindowFromHandle(IntPtr.Zero);

            if (args.ContainsKey("-hwnd"))
                window = new WindowFromHandle(new IntPtr(int.Parse(args["-hwnd"])));

            try
            {
                switch (type)
                {
                    case "processhacker":
                        {
                            switch (action)
                            {
                                case "runas":
                                    {
                                        using (var manager = new ServiceManagerHandle(ScManagerAccess.CreateService))
                                        {
                                            Random r = new Random((int)(DateTime.Now.ToFileTime() & 0xffffffff));
                                            string serviceName = "";

                                            for (int i = 0; i < 8; i++)
                                                serviceName += (char)('A' + r.Next(25));

                                            using (var service = manager.CreateService(
                                                serviceName,
                                                serviceName + " (Process Hacker Assistant)",
                                                ServiceType.Win32OwnProcess,
                                                ServiceStartType.DemandStart,
                                                ServiceErrorControl.Ignore,
                                                obj,
                                                "",
                                                "LocalSystem",
                                                null))
                                            { 
                                                // Create a mailslot so we can receive the error code for Assistant.
                                                using (var mhandle = MailslotHandle.Create(
                                                    FileAccess.GenericRead, @"\Device\Mailslot\" + args["-mailslot"], 0, 5000)
                                                    )
                                                {
                                                    try { service.Start(); }
                                                    catch { }
                                                    service.Delete();

                                                    Win32Error errorCode = (Win32Error)mhandle.Read(4).ToInt32();

                                                    if (errorCode != Win32Error.Success)
                                                        throw new WindowsException(errorCode);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    throw new Exception("Unknown action '" + action + "'");
                            }
                        }
                        break;

                    case "process":
                        {
                            var processes = Windows.GetProcesses();
                            string[] pidStrings = obj.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            int[] pids = new int[pidStrings.Length];
                            string[] names = new string[pidStrings.Length];

                            for (int i = 0; i < pidStrings.Length; i++)
                            {
                                pids[i] = int.Parse(pidStrings[i]);
                                names[i] = processes[pids[i]].Name;
                            }

                            switch (action)
                            {
                                case "terminate":
                                    ProcessActions.Terminate(window, pids, names, true);
                                    break;
                                case "suspend":
                                    ProcessActions.Suspend(window, pids, names, true);
                                    break;
                                case "resume":
                                    ProcessActions.Resume(window, pids, names, true);
                                    break;
                                case "reduceworkingset":
                                    ProcessActions.ReduceWorkingSet(window, pids, names, false);
                                    break;
                                default:
                                    throw new Exception("Unknown action '" + action + "'");
                            }
                        }
                        break;

                    case "thread":
                        {
                            foreach (string tid in obj.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                switch (action)
                                {
                                    case "terminate":
                                        {
                                            try
                                            {
                                                using (var thandle =
                                                    new ThreadHandle(int.Parse(tid), ThreadAccess.Terminate))
                                                    thandle.Terminate();
                                            }
                                            catch (Exception ex)
                                            {
                                                DialogResult result = MessageBox.Show(window,
                                                    "Could not terminate thread with ID " + tid + ":\n\n" +
                                                    ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                                                if (result == DialogResult.Cancel)
                                                    return;
                                            }
                                        }
                                        break;
                                    case "suspend":
                                        {
                                            try
                                            {
                                                using (var thandle =
                                                    new ThreadHandle(int.Parse(tid), ThreadAccess.SuspendResume))
                                                    thandle.Suspend();
                                            }
                                            catch (Exception ex)
                                            {
                                                DialogResult result = MessageBox.Show(window,
                                                    "Could not suspend thread with ID " + tid + ":\n\n" +
                                                    ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                                                if (result == DialogResult.Cancel)
                                                    return;
                                            }
                                        }
                                        break;
                                    case "resume":
                                        {
                                            try
                                            {
                                                using (var thandle =
                                                    new ThreadHandle(int.Parse(tid), ThreadAccess.SuspendResume))
                                                    thandle.Resume();
                                            }
                                            catch (Exception ex)
                                            {
                                                DialogResult result = MessageBox.Show(window,
                                                    "Could not resume thread with ID " + tid + ":\n\n" +
                                                    ex.Message, "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                                                if (result == DialogResult.Cancel)
                                                    return;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown action '" + action + "'");
                                }
                            }
                        }
                        break;

                    case "service":
                        {
                            switch (action)
                            {
                                case "start":
                                    {
                                        ServiceActions.Start(window, obj, false);
                                    }
                                    break;
                                case "continue":
                                    {
                                        ServiceActions.Continue(window, obj, false);
                                    }
                                    break;
                                case "pause":
                                    {
                                        ServiceActions.Pause(window, obj, false);
                                    }
                                    break;
                                case "stop":
                                    {
                                        ServiceActions.Stop(window, obj, false);
                                    }
                                    break;
                                case "delete":
                                    {
                                        ServiceActions.Delete(window, obj, true);
                                    }
                                    break;
                                case "config":
                                    {
                                        using (ServiceHandle service = new ServiceHandle(obj, ServiceAccess.ChangeConfig))
                                        {
                                            ServiceType serviceType;

                                            if (args["-servicetype"] == "Win32OwnProcess, InteractiveProcess")
                                                serviceType = ServiceType.Win32OwnProcess | ServiceType.InteractiveProcess;
                                            else if (args["-servicetype"] == "Win32ShareProcess, InteractiveProcess")
                                                serviceType = ServiceType.Win32ShareProcess | ServiceType.InteractiveProcess;
                                            else
                                                serviceType = (ServiceType)Enum.Parse(typeof(ServiceType), args["-servicetype"]);

                                            var startType = (ServiceStartType)
                                                Enum.Parse(typeof(ServiceStartType), args["-servicestarttype"]);
                                            var errorControl = (ServiceErrorControl)
                                                Enum.Parse(typeof(ServiceErrorControl), args["-serviceerrorcontrol"]);

                                            string binaryPath = null;
                                            string loadOrderGroup = null;
                                            string userAccount = null;
                                            string password = null;

                                            if (args.ContainsKey("-servicebinarypath"))
                                                binaryPath = args["-servicebinarypath"];
                                            if (args.ContainsKey("-serviceloadordergroup"))
                                                loadOrderGroup = args["-serviceloadordergroup"];
                                            if (args.ContainsKey("-serviceuseraccount"))
                                                userAccount = args["-serviceuseraccount"];
                                            if (args.ContainsKey("-servicepassword"))
                                                password = args["-servicepassword"];

                                            if (!Win32.ChangeServiceConfig(service,
                                                serviceType, startType, errorControl,
                                                binaryPath, loadOrderGroup, IntPtr.Zero, null, userAccount, password, null))
                                                Win32.Throw();
                                        }
                                    }
                                    break;
                                default:
                                    throw new Exception("Unknown action '" + action + "'");
                            }
                        }
                        break;

                    case "session":
                        {
                            int sessionId = int.Parse(obj);

                            switch (action)
                            {
                                case "disconnect":
                                    {
                                        SessionActions.Disconnect(window, sessionId, false);
                                    }
                                    break;
                                case "logoff":
                                    {
                                        SessionActions.Logoff(window, sessionId, false);
                                    }
                                    break;
                                default:
                                    throw new Exception("Unknown action '" + action + "'");
                            }
                        }
                        break;

                    default:
                        throw new Exception("Unknown object type '" + type + "'");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(window, ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
