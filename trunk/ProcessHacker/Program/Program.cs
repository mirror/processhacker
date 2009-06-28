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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public static class Program
    {
        /// <summary>
        /// The main Process Hacker window instance
        /// </summary>
        public static HackerWindow HackerWindow;
        public static IntPtr HackerWindowHandle;

        public static ProcessAccess MinProcessQueryRights = ProcessAccess.QueryInformation;
        public static ProcessAccess MinProcessReadMemoryRights = ProcessAccess.VmRead;
        public static ProcessAccess MinProcessWriteMemoryRights = ProcessAccess.VmWrite | ProcessAccess.VmOperation;
        public static ProcessAccess MinProcessGetHandleInformationRights = ProcessAccess.DupHandle;
        public static ThreadAccess MinThreadQueryRights = ThreadAccess.QueryInformation;

        public static int CurrentProcessId;
        public static int CurrentSessionId;
        public static string CurrentUsername;

        /// <summary>
        /// The Results Window ID Generator
        /// </summary>
        public static IdGenerator ResultsIds = new IdGenerator() { Sort = true };

        public static Dictionary<string, Structs.StructDef> Structs = new Dictionary<string, ProcessHacker.Structs.StructDef>();

        public static bool MemoryEditorsThreaded = true;
        public static Dictionary<string, MemoryEditor> MemoryEditors = new Dictionary<string, MemoryEditor>();
        public static Dictionary<string, Thread> MemoryEditorsThreads = new Dictionary<string, Thread>();

        public static bool ResultsWindowsThreaded = true;
        public static Dictionary<string, ResultsWindow> ResultsWindows = new Dictionary<string, ResultsWindow>();
        public static Dictionary<string, Thread> ResultsThreads = new Dictionary<string, Thread>();

        public static bool PEWindowsThreaded = false;
        public static Dictionary<string, PEWindow> PEWindows = new Dictionary<string, PEWindow>();
        public static Dictionary<string, Thread> PEThreads = new Dictionary<string, Thread>();

        public static bool PWindowsThreaded = true;
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

            VerifySettings();

            if (Environment.Version.Major < 2)
            {
                MessageBox.Show("You must have .NET Framework 2.0 or higher to use Process Hacker.", "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Application.Exit();
            }

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(2, 2);
            WorkQueue.GlobalWorkQueue.MaxWorkerThreads = 3;

            // Create or open the Process Hacker mutex, used only by the installer.
            Win32.CreateMutex(IntPtr.Zero, false, "Global\\ProcessHackerMutex");

            try
            {
                using (var thandle = ProcessHandle.GetCurrent().GetToken())
                {
                    try { thandle.SetPrivilege("SeDebugPrivilege", SePrivilegeAttributes.Enabled); }
                    catch { }
                    try { thandle.SetPrivilege("SeLoadDriverPrivilege", SePrivilegeAttributes.Enabled); }
                    catch { }
                    try { thandle.SetPrivilege("SeSecurityPrivilege", SePrivilegeAttributes.Enabled); }
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
                // Only load KPH if we're on 32-bit and it's enabled.
                if (IntPtr.Size == 4 && Properties.Settings.Default.EnableKPH && !NoKph)
                    KProcessHacker.Instance = new KProcessHacker("KProcessHacker");
            }
            catch
            { }

            MinProcessQueryRights = OSVersion.MinProcessQueryInfoAccess;
            MinThreadQueryRights = OSVersion.MinThreadQueryInfoAccess;

            if (KProcessHacker.Instance != null)
            {
                MinProcessGetHandleInformationRights = MinProcessQueryRights;
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
                CurrentProcessId = Win32.GetCurrentProcessId();
                CurrentSessionId = Win32.GetProcessSessionId(Win32.GetCurrentProcessId());
                System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            if (ProcessCommandLine(pArgs))
                return;

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Win32.FileIconInit(true);
            LoadProviders();
            Windows.GetProcessName = (pid) => 
                ProcessProvider.Dictionary.ContainsKey(pid) ? 
                ProcessProvider.Dictionary[pid].Name :
                null;

            new HackerWindow();
            Application.Run();
        }

        private static void LoadProviders()
        {
            ProcessProvider = new ProcessSystemProvider();
            ServiceProvider = new ServiceProvider();
            NetworkProvider = new NetworkProvider();
            Program.SharedThreadProvider = 
                new SharedThreadProvider(Properties.Settings.Default.RefreshInterval);
            Program.SharedThreadProvider.Add(ProcessProvider);
            Program.SharedThreadProvider.Add(ServiceProvider);
            Program.SharedThreadProvider.Add(NetworkProvider);
            Program.SecondarySharedThreadProvider = 
                new SharedThreadProvider(Properties.Settings.Default.RefreshInterval);
        }

        private static void DeleteSettings()
        {
            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\wj32"))
                System.IO.Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    + "\\wj32", true);
            if (System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\wj32"))
                System.IO.Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + "\\wj32", true);
        }

        private static void VerifySettings()
        {
            // Try to get a setting. If the file is corrupt, we can reset the settings.
            try
            {
                var a = Properties.Settings.Default.AlwaysOnTop;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);

                try { ThemingScope.Activate(); }
                catch { }

                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();

                    td.WindowTitle = "Process Hacker";
                    td.MainInstruction = "Process Hacker could not initialize the configuration manager";
                    td.Content = "The Process Hacker configuration file is corrupt or the configuration manager " +
                        "could not be initialized. Do you want Process Hacker to reset your settings?";
                    td.MainIcon = TaskDialogIcon.Warning;
                    td.CommonButtons = TaskDialogCommonButtons.Cancel;
                    td.Buttons = new TaskDialogButton[]
                    {
                        new TaskDialogButton((int)DialogResult.Yes, "Yes, reset the settings and restart Process Hacker"),
                        new TaskDialogButton((int)DialogResult.No, "No, attempt to start Process Hacker anyway"),
                        new TaskDialogButton((int)DialogResult.Retry, "Show me the error message")
                    };
                    td.UseCommandLinks = true;
                    td.Callback = (taskDialog, args, userData) =>
                    {
                        if (args.Notification == TaskDialogNotification.ButtonClicked)
                        {
                            if (args.ButtonId == (int)DialogResult.Yes)
                            {
                                taskDialog.SetMarqueeProgressBar(true);
                                taskDialog.SetProgressBarMarquee(true, 1000);

                                try
                                {
                                    DeleteSettings();
                                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                                }
                                catch (Exception ex2)
                                {
                                    taskDialog.SetProgressBarMarquee(false, 1000);
                                    MessageBox.Show("The settings could not be reset:\r\n\r\n" + ex2.ToString(),
                                        "Process Hacker",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return true;
                                }

                                return false;
                            }
                            else if (args.ButtonId == (int)DialogResult.Retry)
                            {
                                InformationBox box = new InformationBox(ex.ToString());

                                box.ShowDialog();

                                return true;
                            }
                        }

                        return false;
                    };

                    int result = td.Show();

                    if (result == (int)DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    if (MessageBox.Show("Process Hacker cannot start because your configuration file is corrupt. " +
                        "Do you want Process Hacker to reset your settings?", "Process Hacker", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        try
                        {
                            DeleteSettings();
                            MessageBox.Show("Process Hacker has reset your settings and will now restart.", "Process Hacker",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            System.Diagnostics.Process.Start(Application.ExecutablePath);
                        }
                        catch (Exception ex2)
                        {
                            Logging.Log(ex2);

                            MessageBox.Show("Process Hacker could not reset your settings. Please delete the folder " +
                                "'wj32' in your Application Data/Local Application Data directories.",
                                "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }

                Win32.ExitProcess(0);
            }
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
                    PhUtils.ShowMessage(ex);
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
                    PhUtils.ShowMessage(ex);
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
                    Rectangle rect = Utils.RectangleFromString(pArgs["-rect"]);

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

            if (pArgs.ContainsKey("-a"))
            {
                try { Unhook(); }
                catch { }
                try { NProcessHacker.KphHookInit(); }
                catch { }
            }

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
                new System.IO.FileStream(Environment.SystemDirectory + "\\ntdll.dll", 
                    System.IO.FileMode.Open, System.IO.FileAccess.Read));
            IntPtr ntdll = Win32.GetModuleHandle("ntdll.dll");
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
                    System.Runtime.InteropServices.Marshal.WriteByte(ntdll.Increment((int)address + j), br.ReadByte());
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

        public static void TryStart(string command)
        {
            try
            {
                System.Diagnostics.Process.Start(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

            /* Compact the native heaps */
            IntPtr[] heaps = new IntPtr[128];
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
            info.AppendLine("Working set: " + Utils.GetNiceSizeName(Environment.WorkingSet));

            if (KProcessHacker.Instance == null)
                info.AppendLine("KProcessHacker: not running");
            else
                info.AppendLine("KProcessHacker: " + KProcessHacker.Instance.Features.ToString());

            info.AppendLine();
            info.AppendLine("OBJECTS");
            info.AppendLine("Created: " + BaseObject.CreatedCount.ToString());
            info.AppendLine("Freed: " + BaseObject.FreedCount.ToString());
            info.AppendLine("Disposed: " + BaseObject.DisposedCount.ToString());
            info.AppendLine("Finalized: " + BaseObject.FinalizedCount.ToString());
            info.AppendLine("Referenced: " + BaseObject.ReferencedCount.ToString());
            info.AppendLine("Dereferenced: " + BaseObject.DereferencedCount.ToString());

            info.AppendLine();
            info.AppendLine("PROCESS HACKER THREAD POOL");
            info.AppendLine("Worker thread maximum: " + WorkQueue.GlobalWorkQueue.MaxWorkerThreads.ToString());
            info.AppendLine("Worker thread minimum: " + WorkQueue.GlobalWorkQueue.MinWorkerThreads.ToString());
            info.AppendLine("Busy worker threads: " + WorkQueue.GlobalWorkQueue.BusyCount.ToString());
            info.AppendLine("Total worker threads: " + WorkQueue.GlobalWorkQueue.WorkerCount.ToString());
            info.AppendLine("Queued work items: " + WorkQueue.GlobalWorkQueue.QueuedCount.ToString());

            foreach (WorkQueue.WorkItem workItem in WorkQueue.GlobalWorkQueue.GetQueuedWorkItems())
                if (workItem.Tag != null)
                    info.AppendLine("[" + workItem.Tag + "]: " + workItem.Work.Method.Name);
                else
                    info.AppendLine(workItem.Work.Method.Name);

            info.AppendLine();
            info.AppendLine("CLR THREAD POOL");
            int maxWt, maxIoc, minWt, minIoc, wt, ioc;
            ThreadPool.GetAvailableThreads(out wt, out ioc);
            ThreadPool.GetMinThreads(out minWt, out minIoc);
            ThreadPool.GetMaxThreads(out maxWt, out maxIoc);
            info.AppendLine("Worker threads: " + (maxWt - wt).ToString() + " current, " +
                maxWt.ToString() + " max, " + minWt.ToString() + " min");
            info.AppendLine("I/O completion threads: " + (maxIoc - ioc).ToString() + " current, " +
                maxIoc.ToString() + " max, " + minIoc.ToString() + " min");

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

            ErrorDialog ed = new ErrorDialog(ex);

            ed.ShowDialog();
        }

        /// <summary>
        /// Creates an instance of the memory editor form.
        /// </summary>
        /// <param name="PID">The PID of the process to edit</param>
        /// <param name="address">The address to start editing at</param>
        /// <param name="length">The length to edit</param>
        public static MemoryEditor GetMemoryEditor(int PID, IntPtr address, int length)
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
        public static MemoryEditor GetMemoryEditor(int PID, IntPtr address, int length, MemoryEditorInvokeAction action)
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

                    if (!ed.IsDisposed)
                    {
                        action(ed);
                        Application.Run(ed);
                    }

                    Program.MemoryEditorsThreads.Remove(id);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.MemoryEditorsThreads.Add(id, t);
            }
            else
            {
                ed = new MemoryEditor(PID, address, length);
                if (!ed.IsDisposed)
                {
                    action(ed);
                    ed.Show();
                }
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

                    if (!rw.IsDisposed)
                    {
                        action(rw);
                        Application.Run(rw);
                    }

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
                if (!rw.IsDisposed)
                {
                    action(rw);
                    rw.Show();
                }
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

                    if (!pw.IsDisposed)
                    {
                        action(pw);
                        Application.Run(pw);
                    }

                    Program.PEThreads.Remove(path);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PEThreads.Add(path, t);
            }
            else
            {
                pw = new PEWindow(path);
                if (!pw.IsDisposed)
                {
                    action(pw);
                    pw.Show();
                }
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

                    if (!pw.IsDisposed)
                    {
                        action(pw);
                        Application.Run(pw);
                    }

                    Program.PThreads.Remove(process.Pid);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PThreads.Add(process.Pid, t);
            }
            else
            {
                pw = new ProcessWindow(process);
                if (!pw.IsDisposed)
                {
                    action(pw);
                    pw.Show();
                }
            }

            return pw;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        [System.Diagnostics.Conditional("NOT_DEFINED")]
        public static void Void()
        {
            // Do nothing
            int a = 0;
            int b = a * (a + 0);

            for (a = 0; a < b; a++)
                a += a * (a + b);
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

        public static void AddEscapeToClose(this Form f)
        {
            f.KeyPreview = true;
            f.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    f.Close();
                    e.Handled = true;
                }
            };
        }

        /// <summary>
        /// Floats the window on top of the main Process Hacker window.
        /// </summary>
        /// <param name="f">The form to float.</param>
        /// <remarks>
        /// Always call this method before calling InitializeComponent in order for the 
        /// parent to be restored properly.
        /// </remarks>
        public static void SetPhParent(this Form f)
        {
            f.SetPhParent(true);
        }

        public static void SetPhParent(this Form f, bool hideInTaskbar)
        {
            if (Properties.Settings.Default.FloatChildWindows)
            {
                if (hideInTaskbar)
                    f.ShowInTaskbar = false;

                IntPtr oldParent = Win32.SetWindowLongPtr(f.Handle, GetWindowLongOffset.HwndParent, Program.HackerWindowHandle);

                f.FormClosing += (sender, e) => Win32.SetWindowLongPtr(f.Handle, GetWindowLongOffset.HwndParent, oldParent);
            }
        }

        private static void windowAlwaysOnTopItemClicked(object sender, EventArgs e)
        {
            Form f = ((WeakReference<Form>)((MenuItem)sender).Tag).Target;

            if (f == null)
                return;

            f.Invoke(new MethodInvoker(delegate { f.TopMost = !f.TopMost; }));

            Program.UpdateWindow(f);
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
