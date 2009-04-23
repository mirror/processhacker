/*
 * Process Hacker - 
 *   static variables and user interface thread management
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.Drawing;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;
using System.Text;

namespace ProcessHacker
{
    public static class Program
    {
        /// <summary>
        /// The main Process Hacker window instance
        /// </summary>
        public static HackerWindow HackerWindow;

        public static ProcessAccess MinProcessQueryRights = ProcessAccess.QueryInformation;
        public static ProcessAccess MinProcessReadMemoryRights = ProcessAccess.VmRead;
        public static ProcessAccess MinProcessWriteMemoryRights = ProcessAccess.VmWrite | ProcessAccess.VmOperation;
        public static ProcessAccess MinProcessGetHandleInformationRights = ProcessAccess.DupHandle;
        public static ThreadAccess MinThreadQueryRights = ThreadAccess.QueryInformation;

        public static int CurrentSessionId;
        public static string CurrentUsername;

        /// <summary>
        /// The Results Window ID Generator
        /// </summary>
        public static IdGenerator ResultsIds = new IdGenerator();

        public static Dictionary<string, Structs.StructDef> Structs = new Dictionary<string, ProcessHacker.Structs.StructDef>();

        public const bool MemoryEditorsThreaded = true;
        public static Dictionary<string, MemoryEditor> MemoryEditors = new Dictionary<string, MemoryEditor>();
        public static Dictionary<string, Thread> MemoryEditorsThreads = new Dictionary<string, Thread>();

        public const bool ResultsWindowsThreaded = true;
        public static Dictionary<string, ResultsWindow> ResultsWindows = new Dictionary<string, ResultsWindow>();
        public static Dictionary<string, Thread> ResultsThreads = new Dictionary<string, Thread>();

        public const bool PEWindowsThreaded = false;
        public static Dictionary<string, PEWindow> PEWindows = new Dictionary<string, PEWindow>();
        public static Dictionary<string, Thread> PEThreads = new Dictionary<string, Thread>();

        public const bool PWindowsThreaded = true;
        public static Dictionary<int, ProcessWindow> PWindows = new Dictionary<int, ProcessWindow>();
        public static Dictionary<int, Thread> PThreads = new Dictionary<int, Thread>();

        public delegate void ResultsWindowInvokeAction(ResultsWindow f);
        public delegate void MemoryEditorInvokeAction(MemoryEditor f);
        public delegate void ThreadWindowInvokeAction(ThreadWindow f);
        public delegate void PEWindowInvokeAction(PEWindow f);
        public delegate void PWindowInvokeAction(ProcessWindow f);
        public delegate void UpdateWindowAction(Form f);

        public static ProcessSystemProvider ProcessProvider;
        public static ServiceProvider ServiceProvider;
        public static NetworkProvider NetworkProvider;
        public static Dictionary<int, object> ProcessesWithThreads = new Dictionary<int, object>();
        public static System.Collections.Specialized.StringCollection ImposterNames = 
            new System.Collections.Specialized.StringCollection();
        public static bool NoKph = false;
        public static bool StartHidden = false;
        public static bool StartVisible = false;
        public static string SelectTab = "Processes";
        public static TokenElevationType ElevationType;
        public static SharedThreadProvider SharedThreadProvider;
        public static SharedThreadProvider SecondarySharedThreadProvider;
        private static object CollectWorkerThreadsLock = new object();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Dictionary<string, string> pArgs = null;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                pArgs = ParseArgs(args);
            }
            catch
            {
                MessageBox.Show(
                    "Usage: processhacker [-m]\n" +
                    "\t-m\tStarts Process Hacker hidden.\n" +
                    "\t-v\tStarts Process Hacker visible.\n" +
                    "\t-nokph\tDisables KProcessHacker. Use this if you are encountering BSODs.\n" + 
                    "\t-a\tAggressive mode.\n" + 
                    "\t-o\tShows Options.\n" +
                    "\t-t n\tShows the specified tab. 0 is Processes, and 1 is Services.",
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pArgs = new Dictionary<string, string>();
            }

            // In case the settings file is corrupt PH won't crash here - it will be dealt with later.
            try
            {
                if (pArgs.ContainsKey("-nokph"))
                    NoKph = true;
                if (Properties.Settings.Default.AllowOnlyOneInstance && 
                    !(pArgs.ContainsKey("-e") || pArgs.ContainsKey("-o") ||
                    pArgs.ContainsKey("-pw") || pArgs.ContainsKey("-pt"))
                    )
                    CheckForPreviousInstance();
            }
            catch
            { }

            // Try to upgrade settings
            try
            {
                if (Properties.Settings.Default.NeedsUpgrade)
                {
                    try
                    {
                        Properties.Settings.Default.Upgrade();
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                        MessageBox.Show("Process Hacker could not upgrade its settings from a previous version.", "Process Hacker",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    Properties.Settings.Default.NeedsUpgrade = false;
                }
            }
            catch
            { }

            if (Environment.Version.Major < 2)
            {
                MessageBox.Show("You must have .NET Framework 2.0 or higher to use Process Hacker.", "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
            }

            if (IntPtr.Size == 8)
            {
                MessageBox.Show("Process Hacker cannot run on 64-bit versions of Windows.", "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
            }

            ThreadPool.SetMaxThreads(10, 20);

            Win32.CreateMutex(0, false, "Global\\ProcessHackerMutex");

            try
            {
                using (var thandle = ProcessHandle.GetCurrent().GetToken())
                {
                    try { thandle.SetPrivilege("SeDebugPrivilege", SePrivilegeAttributes.Enabled); }
                    catch { }
                    try { thandle.SetPrivilege("SeShutdownPrivilege", SePrivilegeAttributes.Enabled); }
                    catch { }

                    if (OSVersion.HasUac)
                    {
                        try { ElevationType = thandle.GetElevationType(); }
                        catch { ElevationType = TokenElevationType.Full; }

                        if (ElevationType == TokenElevationType.Default &&
                            !(new WindowsPrincipal(WindowsIdentity.GetCurrent())).
                                IsInRole(WindowsBuiltInRole.Administrator))
                            ElevationType = TokenElevationType.Limited;
                        else if (ElevationType == TokenElevationType.Default)
                            ElevationType = TokenElevationType.Full;
                    }
                    else
                    {
                        ElevationType = TokenElevationType.Full;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            try
            {
                if (Properties.Settings.Default.EnableKPH && !NoKph)
                    KProcessHacker.Instance = new KProcessHacker("KProcessHacker");
            }
            catch
            { }

            MinProcessQueryRights = OSVersion.MinProcessQueryInfoAccess;
            MinThreadQueryRights = OSVersion.MinThreadQueryInfoAccess;

            if (KProcessHacker.Instance != null)
            {
                MinProcessGetHandleInformationRights = MinProcessQueryRights;
            }

            if (KProcessHacker.Instance != null && OSVersion.HasMmCopyVirtualMemory)
            {
                MinProcessReadMemoryRights = MinProcessQueryRights;
                MinProcessWriteMemoryRights = MinProcessQueryRights;
            }

            try
            {
                CurrentUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            try
            {
                CurrentSessionId = Win32.GetProcessSessionId(Win32.GetCurrentProcessId());
                System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            if (ProcessCommandLine(pArgs))
                return;

#if DEBUG
#else
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#endif

            ProcessProvider = new ProcessSystemProvider();
            ServiceProvider = new ServiceProvider();
            NetworkProvider = new NetworkProvider();
            Windows.GetProcessName = (pid) => 
                ProcessProvider.Dictionary.ContainsKey(pid) ? 
                ProcessProvider.Dictionary[pid].Name :
                null;

            new HackerWindow();
            Application.Run();
        }

        private static bool ProcessCommandLine(Dictionary<string, string> pArgs)
        {
            if (pArgs.ContainsKey("-e"))
            {
                try
                {
                    ExtendedCmd.Run(pArgs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return true;
            }

            if (pArgs.ContainsKey("-pw"))
            {
                int pid = int.Parse(pArgs["-pw"]);

                SharedThreadProvider = new SharedThreadProvider(Properties.Settings.Default.RefreshInterval);
                SecondarySharedThreadProvider = new SharedThreadProvider(Properties.Settings.Default.RefreshInterval);

                ProcessProvider = new ProcessSystemProvider();
                ServiceProvider = new ServiceProvider();
                SharedThreadProvider.Add(ProcessProvider);
                SharedThreadProvider.Add(ServiceProvider);
                ProcessProvider.RunOnce();
                ServiceProvider.RunOnce();
                ProcessProvider.Enabled = true;
                ServiceProvider.Enabled = true;

                Win32.LoadLibrary(Properties.Settings.Default.DbgHelpPath);

                if (!ProcessProvider.Dictionary.ContainsKey(pid))
                {
                    MessageBox.Show("The process (PID " + pid.ToString() + ") does not exist.",
                        "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    return true;
                }

                ProcessWindow pw = new ProcessWindow(ProcessProvider.Dictionary[pid]);

                Application.Run(pw);

                SharedThreadProvider.Dispose();
                ProcessProvider.Dispose();
                ServiceProvider.Dispose();

                Environment.Exit(0);

                return true;
            }

            if (pArgs.ContainsKey("-pt"))
            {
                int pid = int.Parse(pArgs["-pt"]);

                try
                {
                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                        Application.Run(new TokenWindow(phandle));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return true;
            }

            if (pArgs.ContainsKey("-o"))
            {
                OptionsWindow options = new OptionsWindow(true)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                IWin32Window window;

                if (pArgs.ContainsKey("-hwnd"))
                    window = new WindowFromHandle(new IntPtr(int.Parse(pArgs["-hwnd"])));
                else
                    window = new WindowFromHandle(IntPtr.Zero);

                if (pArgs.ContainsKey("-rect"))
                {
                    Rectangle rect = Misc.RectangleFromString(pArgs["-rect"]);

                    options.Location = new Point(rect.X + 20, rect.Y + 20);
                    options.StartPosition = FormStartPosition.Manual;
                }

                options.SelectedTab = options.TabPages["tabAdvanced"];
                options.ShowDialog(window);

                return true;
            }

            if (pArgs.ContainsKey(""))
                if (pArgs[""].Replace("\"", "").Trim().ToLower().EndsWith("taskmgr.exe"))
                    StartVisible = true;

            if (pArgs.ContainsKey("-m"))
                StartHidden = true;
            if (pArgs.ContainsKey("-v"))
                StartVisible = true;

            if (pArgs.ContainsKey("-t"))
            {
                if (pArgs["-t"] == "0")
                    SelectTab = "Processes";
                else if (pArgs["-t"] == "1")
                    SelectTab = "Services";
            }

            return false;
        }

        public static void Unhook()
        {
            PE.PEFile file = new ProcessHacker.PE.PEFile(Environment.SystemDirectory + "\\ntdll.dll");
            System.IO.BinaryReader br = new System.IO.BinaryReader(
                new System.IO.FileStream(Environment.SystemDirectory + "\\ntdll.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read));
            int ntdll = Win32.GetModuleHandle("ntdll.dll");
            MemoryProtection oldProtection;

            oldProtection = ProcessHandle.GetCurrent().ProtectMemory(
                ntdll,
                (int)file.COFFOptionalHeader.SizeOfCode,
                MemoryProtection.ExecuteReadWrite
                );

            for (int i = 0; i < file.ExportData.ExportOrdinalTable.Count; i++)
            {
                ushort ordinal = file.ExportData.ExportOrdinalTable[i];

                if (ordinal >= file.ExportData.ExportAddressTable.Count)
                    continue;

                uint address = file.ExportData.ExportAddressTable[ordinal].ExportRVA;
                int fileAddress = (int)file.RvaToVa(address);

                string name = file.ExportData.ExportNameTable[i];

                if (!name.StartsWith("Nt") || name.StartsWith("Ntdll"))
                    continue;

                byte[] fileData = new byte[5];

                br.BaseStream.Seek(fileAddress, System.IO.SeekOrigin.Begin);

                for (int j = 0; j < 5; j++)
                {
                    System.Runtime.InteropServices.Marshal.WriteByte(new IntPtr(ntdll + address + j), br.ReadByte());
                }
            }

            br.Close();

            ProcessHandle.GetCurrent().ProtectMemory(
                ntdll,
                (int)file.COFFOptionalHeader.SizeOfCode,
                oldProtection
                );
        }

        private static void CheckForPreviousInstance()
        {
            bool found = false;

            Win32.EnumWindows((hWnd, param) =>
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(0x100);
                    int length = Win32.InternalGetWindowText(hWnd, sb, sb.Capacity);

                    if (sb.ToString().Contains("Process Hacker ["))
                    {
                        int result;

                        Win32.SendMessageTimeout(hWnd, (WindowMessage)0x9991, 0, 0,
                            SmtoFlags.Block, 5000, out result);

                        if (result == 0x1119)
                        {
                            Win32.SetForegroundWindow(hWnd);
                            found = true;
                            return false;
                        }
                    }

                    return true;
                }, 0);

            if (found)
                Environment.Exit(0);
        }

        public static void StartProcessHackerAdmin()
        {
            StartProcessHackerAdmin("", null, IntPtr.Zero);
        }

        public static void StartProcessHackerAdmin(string args, MethodInvoker successAction)
        {
            StartProcessHackerAdmin(args, successAction, IntPtr.Zero);
        }

        public static void StartProcessHackerAdmin(string args, MethodInvoker successAction, IntPtr hWnd)
        {
            StartProgramAdmin(ProcessHandle.GetCurrent().GetMainModule().FileName,
                args, successAction, ShowWindowType.Show, hWnd);
        }

        public static WaitResult StartProcessHackerAdminWait(string args, IntPtr hWnd, uint timeout)
        {
            return StartProcessHackerAdminWait(args, null, hWnd, timeout);
        }

        public static WaitResult StartProcessHackerAdminWait(string args, MethodInvoker successAction, IntPtr hWnd, uint timeout)
        {
            var info = new ShellExecuteInfo();

            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpFile = ProcessHandle.GetCurrent().GetMainModule().FileName;
            info.nShow = ShowWindowType.Show;
            info.fMask = 0x40; // SEE_MASK_NOCLOSEPROCESS
            info.lpVerb = "runas";
            info.lpParameters = args;
            info.hWnd = hWnd;

            if (Win32.ShellExecuteEx(ref info))
            {
                if (successAction != null)
                    successAction();

                var result = Win32.WaitForSingleObject(info.hProcess, timeout);

                Win32.CloseHandle(info.hProcess);

                return result;
            }
            else
            {
                // An error occured - the user probably canceled the elevation dialog.
                return WaitResult.Abandoned;
            }
        }

        public static void StartProgramAdmin(string program, string args, 
            MethodInvoker successAction, ShowWindowType showType, IntPtr hWnd)
        {
            var info = new ShellExecuteInfo();

            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpFile = program;
            info.nShow = showType;
            info.lpVerb = "runas";
            info.lpParameters = args;
            info.hWnd = hWnd;

            if (Win32.ShellExecuteEx(ref info))
            {
                if (successAction != null)
                    successAction();
            }
        }

        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string argPending = null;

            foreach (string s in args)
            {
                if (s.StartsWith("-"))
                {
                    if (dict.ContainsKey(s))
                        throw new Exception("Option already specified.");

                    dict.Add(s, "");
                    argPending = s;
                }
                else
                {
                    if (argPending != null)
                    {
                        dict[argPending] = s;
                        argPending = null;
                    }
                    else
                    {
                        if (dict.ContainsKey(""))
                            throw new Exception("Input file already specified.");

                        dict.Add("", s);
                    }
                }
            }

            return dict;
        }

        public static void ApplyFont(Font font)
        {
            HackerWindow.BeginInvoke(new MethodInvoker(() => { HackerWindow.ApplyFont(font); }));

            foreach (var processWindow in PWindows.Values)
            {
                processWindow.BeginInvoke(new MethodInvoker(() => { processWindow.ApplyFont(font); }));
            }
        }

        public static void CollectGarbage()
        {
            /* Garbage collections */
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            /* Compact the native heaps */
            int[] heaps = new int[128];
            int count = Win32.GetProcessHeaps(heaps.Length, heaps);

            if (count <= heaps.Length)
            {
                for (int i = 0; i < count; i++)
                    Win32.HeapCompact(heaps[i], false);
            }

            /* Terminate any unused threadpool threads */
            CollectWorkerThreads();
        }

        public static void CollectWorkerThreads()
        {
            lock (CollectWorkerThreadsLock)
            {
                int workerThreads, completionPortThreads, maxWorkerThreads, maxCompletionPortThreads;

                ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);

                workerThreads = maxWorkerThreads - workerThreads;
                completionPortThreads = maxCompletionPortThreads - completionPortThreads;

                ThreadPool.SetMaxThreads(0, 0);
                ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
            }
        }

        public static string GetDiagnosticInformation()
        {
            StringBuilder info = new StringBuilder();

            info.AppendLine("Process Hacker " + Application.ProductVersion);
            info.AppendLine("CLR Version: " + Environment.Version.ToString());
            info.AppendLine("OS Version: " + Environment.OSVersion.VersionString);
            info.AppendLine("Elevation: " + ElevationType.ToString());

            if (KProcessHacker.Instance == null)
                info.AppendLine("KProcessHacker: not running");
            else
                info.AppendLine("KProcessHacker: " + KProcessHacker.Instance.Features.ToString());

            info.AppendLine();
            info.AppendLine("PRIMARY SHARED THREAD PROVIDER");
            info.AppendLine("Count: " + SharedThreadProvider.Count.ToString());

            if (SharedThreadProvider != null)
            {
                foreach (var provider in SharedThreadProvider.Providers)
                    info.AppendLine(provider.GetType().FullName +
                        " (Enabled: " + provider.Enabled +
                        ", Busy: " + provider.Busy.ToString() +
                        ", CreateThread: " + provider.CreateThread.ToString() +
                        ")");
            }
            else
            {
                info.AppendLine("(null)");
            }

            info.AppendLine();
            info.AppendLine("SECONDARY SHARED THREAD PROVIDER");

            if (SecondarySharedThreadProvider != null)
            {
                info.AppendLine("Count: " + SecondarySharedThreadProvider.Count.ToString());

                foreach (var provider in SecondarySharedThreadProvider.Providers)
                    info.AppendLine(provider.GetType().FullName +
                        " (Enabled: " + provider.Enabled +
                        ", Busy: " + provider.Busy.ToString() +
                        ", CreateThread: " + provider.CreateThread.ToString() +
                        ")");
            }
            else
            {
                info.AppendLine("(null)");
            }

            info.AppendLine();
            info.AppendLine("WINDOWS");
            info.AppendLine("MemoryEditors: " + MemoryEditors.Count.ToString() + ", " + MemoryEditorsThreads.Count.ToString());
            info.AppendLine("PEWindows: " + PEWindows.Count.ToString() + ", " + PEThreads.Count.ToString());
            info.AppendLine("PWindows: " + PWindows.Count.ToString() + ", " + PThreads.Count.ToString());
            info.AppendLine("ResultsWindows: " + ResultsWindows.Count.ToString() + ", " + ResultsThreads.Count.ToString());

            return info.ToString();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(e.ExceptionObject as Exception);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
        }

        private static void UnhandledException(Exception ex)
        {
            Logging.Log(Logging.Importance.Critical, ex.ToString());

            if (false)
            {
                TaskDialog td = new TaskDialog();

                td.WindowTitle = "Process Hacker";
                td.MainInstruction = "Process Hacker has encountered a problem";
                td.Content = "An unhandled exception has occurred in Process Hacker.";

                td.Buttons = new TaskDialogButton[]
                {
                    new TaskDialogButton((int)DialogResult.Yes, "Continue\nIgnore the error and continue. This may cause Process Hacker to crash."),
                    new TaskDialogButton((int)DialogResult.No, "Close\nClose Process Hacker.")
                };
                td.UseCommandLinks = true;

                try
                {
                    if (Program.HackerWindow != null)
                    {
                        td.CustomMainIcon = ProcessHacker.Properties.Resources.Process;
                    }
                }
                catch
                { }

                td.ExpandedInformation = "Please report this problem to " +
                    "<a href=\"report\">http://sourceforge.net/projects/processhacker</a>\r\n\r\n" + ex.ToString();
                td.EnableHyperlinks = true;
                td.ExpandFooterArea = true;
                td.CollapsedControlText = "Show problem details";
                td.ExpandedControlText = "Hide problem details";
                td.Callback = (taskDialog, args, callbackData) =>
                    {
                        if (args.Notification == TaskDialogNotification.HyperlinkClicked)
                        {
                            if (args.Hyperlink == "report")
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start("http://sourceforge.net/tracker2/?group_id=242527");
                                }
                                catch
                                { }
                            }

                            return true;
                        }

                        return false;
                    };

                DialogResult result = (DialogResult)td.Show();

                if (result == DialogResult.No)
                {
                    try
                    {
                        Properties.Settings.Default.Save();
                    }
                    catch
                    { }

                    Win32.ExitProcess(0); 
                }
            }
            else
            {
                ErrorDialog ed = new ErrorDialog(ex);

                ed.ShowDialog();
            }
        }

        /// <summary>
        /// Creates an instance of the memory editor form.
        /// </summary>
        /// <param name="PID">The PID of the process to edit</param>
        /// <param name="address">The address to start editing at</param>
        /// <param name="length">The length to edit</param>
        public static MemoryEditor GetMemoryEditor(int PID, int address, int length)
        {
            return GetMemoryEditor(PID, address, length, new MemoryEditorInvokeAction(delegate {}));
        }

        /// <summary>
        /// Creates an instance of the memory editor form and invokes an action on the memory editor's thread.
        /// </summary>
        /// <param name="PID">The PID of the process to edit</param>
        /// <param name="address">The address to start editing at</param>
        /// <param name="length">The length to edit</param>
        /// <param name="action">The action to be invoked on the memory editor's thread</param>
        /// <returns>Memory editor form</returns>
        public static MemoryEditor GetMemoryEditor(int PID, int address, int length, MemoryEditorInvokeAction action)
        {
            MemoryEditor ed = null;
            string id = PID.ToString() + "-" + address.ToString() + "-" + length.ToString();

            if (MemoryEditors.ContainsKey(id))
            {
                ed = MemoryEditors[id];

                ed.Invoke(action, ed);

                return ed;
            }

            if (MemoryEditorsThreaded)
            {
                Thread t = new Thread(new ThreadStart(delegate
                {
                    ed = new MemoryEditor(PID, address, length);

                    action(ed);

                    try
                    {
                        Application.Run(ed);
                    }
                    catch
                    { }

                    Program.MemoryEditorsThreads.Remove(id);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.MemoryEditorsThreads.Add(id, t);
            }
            else
            {
                ed = new MemoryEditor(PID, address, length);
                action(ed);
                ed.Show();
            }

            return ed;
        }

        /// <summary>
        /// Creates an instance of the results window on a separate thread.
        /// </summary>
        public static ResultsWindow GetResultsWindow(int PID)
        {
            return GetResultsWindow(PID, new ResultsWindowInvokeAction(delegate { }));
        }

        /// <summary>
        /// Creates an instance of the results window on a separate thread and invokes an action on that thread.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        public static ResultsWindow GetResultsWindow(int PID, ResultsWindowInvokeAction action)
        {
            ResultsWindow rw = null;
            string id = "";

            if (ResultsWindowsThreaded)
            {
                Thread t = new Thread(new ThreadStart(delegate
                {
                    rw = new ResultsWindow(PID);

                    id = rw.Id;

                    action(rw);

                    try
                    {
                        Application.Run(rw);
                    }
                    catch
                    { }

                    Program.ResultsThreads.Remove(id);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                while (id == "") Thread.Sleep(1);
                Program.ResultsThreads.Add(id, t);
            }
            else
            {
                rw = new ResultsWindow(PID);
                action(rw);
                rw.Show();
            }

            return rw;
        }

        /// <summary>
        /// Creates an instance of the PE window on a separate thread.
        /// </summary>
        public static PEWindow GetPEWindow(string path)
        {
            return GetPEWindow(path, new PEWindowInvokeAction(delegate { }));
        }

        /// <summary>
        /// Creates an instance of the thread window on a separate thread and invokes an action on that thread.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        public static PEWindow GetPEWindow(string path, PEWindowInvokeAction action)
        {
            PEWindow pw = null;

            if (PEWindows.ContainsKey(path))
            {
                pw = PEWindows[path];

                pw.Invoke(action, pw);

                return pw;
            }

            if (PEWindowsThreaded)
            {
                Thread t = new Thread(new ThreadStart(delegate
                {
                    pw = new PEWindow(path);

                    action(pw);

                    try
                    {
                        Application.Run(pw);
                    }
                    catch
                    { }

                    Program.PEThreads.Remove(path);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PEThreads.Add(path, t);
            }
            else
            {
                pw = new PEWindow(path);
                action(pw);
                pw.Show();
            }

            return pw;
        }

        /// <summary>
        /// Creates an instance of the process window on a separate thread.
        /// </summary>
        public static ProcessWindow GetProcessWindow(ProcessItem process)
        {
            return GetProcessWindow(process, new PWindowInvokeAction(delegate { }));
        }

        /// <summary>
        /// Creates an instance of the process window on a separate thread and invokes an action on that thread.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        public static ProcessWindow GetProcessWindow(ProcessItem process, PWindowInvokeAction action)
        {
            ProcessWindow pw = null;

            if (PWindows.ContainsKey(process.Pid))
            {
                pw = PWindows[process.Pid];

                pw.Invoke(action, pw);

                return pw;
            }

            if (PWindowsThreaded)
            {
                Thread t = new Thread(new ThreadStart(delegate
                {
                    pw = new ProcessWindow(process);

                    action(pw);

                    try
                    {
                        Application.Run(pw);
                    }
                    catch
                    { }

                    Program.PThreads.Remove(process.Pid);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PThreads.Add(process.Pid, t);
            }
            else
            {
                pw = new ProcessWindow(process);
                action(pw);
                pw.Show();
            }

            return pw;
        }

        public static void FocusWindow(Form f)
        {
            if (f.InvokeRequired)
            {
                f.BeginInvoke(new MethodInvoker(delegate { Program.FocusWindow(f); }));

                return;
            }

            f.Visible = true; // just in case it's hidden right now   

            if (f.WindowState == FormWindowState.Minimized)
                f.WindowState = FormWindowState.Normal;

            f.Activate();
        }

        public static void UpdateWindow(Form f)
        {
            if (f.InvokeRequired)
            {
                f.BeginInvoke(new UpdateWindowAction(UpdateWindow), f);

                return;
            }

            MenuItem windowMenuItem = (MenuItem)f.GetType().GetProperty("WindowMenuItem").GetValue(f, null);
            wyDay.Controls.VistaMenu vistaMenu =
                (wyDay.Controls.VistaMenu)f.GetType().GetProperty("VistaMenu").GetValue(f, null);
            MenuItem item;

            lock (windowMenuItem)
            {
                WeakReference<Form> fRef = new WeakReference<Form>(f);

                foreach (MenuItem menuItem in windowMenuItem.MenuItems)
                {
                    vistaMenu.SetImage(menuItem, null);
                    menuItem.Tag = null;
                }

                windowMenuItem.MenuItems.DisposeAndClear();

                item = new MenuItem("&Always On Top");
                item.Tag = fRef;
                item.Click += new EventHandler(windowAlwaysOnTopItemClicked);
                item.Checked = f.TopMost;
                windowMenuItem.MenuItems.Add(item);

                item = new MenuItem("&Close");
                item.Tag = fRef;
                item.Click += new EventHandler(windowCloseItemClicked);
                windowMenuItem.MenuItems.Add(item);
                vistaMenu.SetImage(item, global::ProcessHacker.Properties.Resources.application_delete);
            }
        }

        public static void UpdateWindows()
        {
            Dictionary<string, Form> TextToForm = new Dictionary<string, Form>();
            List<string> Texts = new List<string>();
            List<object> dics = new List<object>();
            List<Form> forms = new List<Form>();

            dics.Add(Program.MemoryEditors);
            dics.Add(Program.ResultsWindows);
            dics.Add(Program.PEWindows);
            dics.Add(Program.PWindows);

            foreach (object dic in dics)
            {
                object valueCollection = dic.GetType().GetProperty("Values").GetValue(dic, null);
                object enumerator = valueCollection.GetType().GetMethod("GetEnumerator").Invoke(valueCollection, null);

                while ((bool)enumerator.GetType().GetMethod("MoveNext").Invoke(enumerator, null))
                {
                    forms.Add((Form)enumerator.GetType().GetProperty("Current").GetValue(enumerator, null));
                }
            }

            if (Program.HackerWindow != null)
                forms.Add(Program.HackerWindow);

            foreach (Form f in forms)
                UpdateWindow(f);
        }

        private static void windowAlwaysOnTopItemClicked(object sender, EventArgs e)
        {
            Form f = ((WeakReference<Form>)((MenuItem)sender).Tag).Target;

            if (f == null)
                return;

            f.Invoke(new MethodInvoker(delegate { f.TopMost = !f.TopMost; }));

            Program.UpdateWindows();
        }

        private static void windowCloseItemClicked(object sender, EventArgs e)
        {
            Form f = ((WeakReference<Form>)((MenuItem)sender).Tag).Target;

            if (f == null)
                return;

            f.Invoke(new MethodInvoker(delegate { f.Close(); }));
        }
    }
}
