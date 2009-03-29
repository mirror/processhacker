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
using System.Text;
using System.Windows.Forms;
using ProcessHacker.UI;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ProcessHacker
{
    public static class ExtendedCmd
    {
        private static class ThemingScope
        {
            [DllImport("kernel32.dll")]
            private static extern bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);

            public static void Activate()
            {
                IntPtr zero = IntPtr.Zero;
                Assembly windowsForms = Assembly.GetAssembly(typeof(Control));

                // HACK
                IntPtr hActCtx = (IntPtr)windowsForms.GetType("System.Windows.Forms.UnsafeNativeMethods", true).
                    GetNestedType("ThemingScope", BindingFlags.NonPublic | BindingFlags.Static).
                    GetField("hActCtx", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                if (OSFeature.Feature.IsPresent(OSFeature.Themes))
                    ActivateActCtx(hActCtx, out zero);
            }
        }

        public class WindowFromHWnd : IWin32Window
        {
            private IntPtr _handle;

            public WindowFromHWnd(IntPtr handle)
            {
                _handle = handle;
            }

            public IntPtr Handle
            {
                get { return _handle; }
            }
        }

        public static void Run(IDictionary<string, string> args)
        {
            ThemingScope.Activate();

            if (!args.ContainsKey("-type"))
                throw new Exception("-type switch required.");

            string type = args["-type"].ToLower();

            if (!args.ContainsKey("-obj"))
                throw new Exception("-obj switch required.");

            string obj = args["-obj"];

            if (!args.ContainsKey("-action"))
                throw new Exception("-action switch required.");

            string action = args["-action"].ToLower();

            WindowFromHWnd window = new WindowFromHWnd(IntPtr.Zero);

            if (args.ContainsKey("-hwnd"))
                window = new WindowFromHWnd(new IntPtr(int.Parse(args["-hwnd"])));

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
                                        using (var manager = new Win32.ServiceManagerHandle(Win32.SC_MANAGER_RIGHTS.SC_MANAGER_CREATE_SERVICE))
                                        {
                                            Random r = new Random((int)(DateTime.Now.ToFileTime() & 0xffffffff));
                                            string serviceName = "";

                                            for (int i = 0; i < 8; i++)
                                                serviceName += (char)('A' + r.Next(25));

                                            using (var service = manager.CreateService(
                                                serviceName,
                                                serviceName + " (Process Hacker Assistant)",
                                                Win32.SERVICE_TYPE.Win32OwnProcess,
                                                Win32.SERVICE_START_TYPE.DemandStart,
                                                Win32.SERVICE_ERROR_CONTROL.Ignore,
                                                obj,
                                                "",
                                                "LocalSystem",
                                                null))
                                            {
                                                try { service.Start(); }
                                                catch { }
                                                service.Delete();
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
                            var processes = Win32.EnumProcesses();
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
                                                    new Win32.ThreadHandle(int.Parse(tid), Win32.THREAD_RIGHTS.THREAD_TERMINATE))
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
                                                    new Win32.ThreadHandle(int.Parse(tid), Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
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
                                                    new Win32.ThreadHandle(int.Parse(tid), Win32.THREAD_RIGHTS.THREAD_SUSPEND_RESUME))
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
                                        using (var shandle = new Win32.ServiceHandle(obj, Win32.SERVICE_RIGHTS.SERVICE_START))
                                            shandle.Start();
                                    }
                                    break;
                                case "continue":
                                    {
                                        using (var shandle = new Win32.ServiceHandle(obj, Win32.SERVICE_RIGHTS.SERVICE_PAUSE_CONTINUE))
                                            shandle.Control(Win32.SERVICE_CONTROL.Continue);
                                    }
                                    break;
                                case "pause":
                                    {
                                        using (var shandle = new Win32.ServiceHandle(obj, Win32.SERVICE_RIGHTS.SERVICE_PAUSE_CONTINUE))
                                            shandle.Control(Win32.SERVICE_CONTROL.Pause);
                                    }
                                    break;
                                case "stop":
                                    {
                                        using (var shandle = new Win32.ServiceHandle(obj, Win32.SERVICE_RIGHTS.SERVICE_STOP))
                                            shandle.Control(Win32.SERVICE_CONTROL.Stop);
                                    }
                                    break;
                                case "delete":
                                    {
                                        using (var shandle = new Win32.ServiceHandle(obj, (Win32.SERVICE_RIGHTS)Win32.STANDARD_RIGHTS.DELETE))
                                            shandle.Delete();
                                    }
                                    break;
                                case "config":
                                    {
                                        using (Win32.ServiceHandle service = new Win32.ServiceHandle(obj,
                                            Win32.SERVICE_RIGHTS.SERVICE_CHANGE_CONFIG))
                                        {
                                            Win32.SERVICE_TYPE serviceType;

                                            if (args["-servicetype"] == "Win32OwnProcess, InteractiveProcess")
                                                serviceType = Win32.SERVICE_TYPE.Win32OwnProcess | Win32.SERVICE_TYPE.InteractiveProcess;
                                            else if (args["-servicetype"] == "Win32ShareProcess, InteractiveProcess")
                                                serviceType = Win32.SERVICE_TYPE.Win32ShareProcess | Win32.SERVICE_TYPE.InteractiveProcess;
                                            else
                                                serviceType = (Win32.SERVICE_TYPE)Enum.Parse(typeof(Win32.SERVICE_TYPE), args["-servicetype"]);

                                            var startType = (Win32.SERVICE_START_TYPE)
                                                Enum.Parse(typeof(Win32.SERVICE_START_TYPE), args["-servicestarttype"]);
                                            var errorControl = (Win32.SERVICE_ERROR_CONTROL)
                                                Enum.Parse(typeof(Win32.SERVICE_ERROR_CONTROL), args["-serviceerrorcontrol"]);

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

                                            if (!Win32.ChangeServiceConfig(service.Handle,
                                                serviceType, startType, errorControl,
                                                binaryPath, loadOrderGroup, 0, 0, userAccount, password, null))
                                                Win32.ThrowLastWin32Error();
                                        }
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
