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
        /// The main Process Hacker window instance.
        /// </summary>
        public static HackerWindow HackerWindow;
        public static IntPtr HackerWindowHandle;
        public static bool HackerWindowTopMost;

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

        public static bool BadConfig = false;
        public static TokenElevationType ElevationType;
        public static ProcessHacker.Native.Threading.Mutant GlobalMutex;
        public static string GlobalMutexName = @"\BaseNamedObjects\ProcessHackerMutex";
        public static System.Collections.Specialized.StringCollection ImposterNames =
            new System.Collections.Specialized.StringCollection();
        public static int InspectPid = -1;
        public static bool NoKph = false;
        public static string SelectTab = "Processes";
        public static bool StartHidden = false;
        public static bool StartVisible = false;
        public static ProviderThread PrimaryProviderThread;
        public static ProviderThread SecondaryProviderThread;
        public static ProcessHacker.Native.Threading.Waiter SharedWaiter;

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

            if (Environment.Version.Major < 2)
            {
                PhUtils.ShowError("You must have .NET Framework 2.0 or higher to use Process Hacker.");
                Environment.Exit(1);
            }

            // Check OS support.
            if (OSVersion.IsBelow(WindowsVersion.TwoThousand) || OSVersion.IsAbove(WindowsVersion.Seven))
            {
                PhUtils.ShowWarning("Your operating system is not supported by Process Hacker.");
            }
#if !DEBUG
            // Setup exception handling at first opportunity.
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);
#endif
            try
            {
                pArgs = ParseArgs(args);
            }
            catch
            {
                ShowCommandLineUsage();
                pArgs = new Dictionary<string, string>();
            }

            if (pArgs.ContainsKey("-h") || pArgs.ContainsKey("-help") || pArgs.ContainsKey("-?"))
            {
                ShowCommandLineUsage();
                return;
            }

            if (pArgs.ContainsKey("-elevate"))
            {
                // Propagate arguments.
                pArgs.Remove("-elevate");
                StartProcessHackerAdmin(Utils.JoinCommandLine(pArgs), null);
                return;
            }

            LoadSettings(!pArgs.ContainsKey("-nosettings"), pArgs.ContainsKey("-settings") ? pArgs["-settings"] : null);

            try
            {
                if (pArgs.ContainsKey("-nokph"))
                    NoKph = true;
                if (Settings.Instance.AllowOnlyOneInstance && 
                    !(pArgs.ContainsKey("-e") || pArgs.ContainsKey("-o") ||
                    pArgs.ContainsKey("-pw") || pArgs.ContainsKey("-pt"))
                    )
                    ActivatePreviousInstance();
            }
            catch
            { }

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(2, 2);
            WorkQueue.GlobalWorkQueue.MaxWorkerThreads = 2;

            // Create or open the Process Hacker mutex, used only by the installer.
            try
            {
                GlobalMutex = new ProcessHacker.Native.Threading.Mutant(GlobalMutexName);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            try
            {
                using (var thandle = ProcessHandle.Current.GetToken())
                {
                    thandle.TrySetPrivilege("SeDebugPrivilege", SePrivilegeAttributes.Enabled);
                    thandle.TrySetPrivilege("SeIncreaseBasePriorityPrivilege", SePrivilegeAttributes.Enabled);
                    thandle.TrySetPrivilege("SeLoadDriverPrivilege", SePrivilegeAttributes.Enabled);
                    thandle.TrySetPrivilege("SeRestorePrivilege", SePrivilegeAttributes.Enabled);
                    thandle.TrySetPrivilege("SeShutdownPrivilege", SePrivilegeAttributes.Enabled);
                    thandle.TrySetPrivilege("SeTakeOwnershipPrivilege", SePrivilegeAttributes.Enabled);

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
                if (
                    // Only load KPH if we're on 32-bit and it's enabled.
                    OSVersion.Architecture == OSArch.I386 &&
                    Settings.Instance.EnableKPH &&
                    !NoKph &&
                    // Don't load KPH if we're going to install/uninstall it.
                    !pArgs.ContainsKey("-installkph") && !pArgs.ContainsKey("-uninstallkph")
                    )
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

            Win32.FileIconInit(true);
            LoadProviders();
            Windows.GetProcessName = (pid) => 
                ProcessProvider.Dictionary.ContainsKey(pid) ? 
                ProcessProvider.Dictionary[pid].Name :
                null;

            // Create the shared waiter.
            SharedWaiter = new ProcessHacker.Native.Threading.Waiter();

            HackerWindow = new HackerWindow();
            Application.Run();
        }

        private static void ShowCommandLineUsage()
        {
            PhUtils.ShowInformation(
                "Option: \tUsage:\n" +
                "-a\tAggressive mode.\n" +
                "-elevate\tStarts Process Hacker elevated.\n" +
                "-h\tDisplays command line usage information.\n" +
                "-installkph\tInstalls the KProcessHacker service.\n" +
                "-ip pid\tDisplays the main window, then properties for the specified process.\n" +
                "-m\tStarts Process Hacker hidden.\n" +
                "-nokph\tDisables KProcessHacker. Use this if you encounter BSODs.\n" +
                "-nosettings\tUses defaults for all settings and does not attempt to load or save any settings.\n" +
                "-o\tShows Options.\n" +
                "-pw pid\tDisplays properties for the specified process.\n" +
                "-pt pid\tDisplays properties for the specified process' token.\n" +
                "-settings filename\tUses the specified file name as the settings file.\n" +
                "-t n\tShows the specified tab. 0 is Processes, 1 is Services and 2 is Network.\n" +
                "-uninstallkph\tUninstalls the KProcessHacker service.\n" +
                "-v\tStarts Process Hacker visible.\n" +
                ""
                );
        }

        private static void LoadProviders()
        {
            ProcessProvider = new ProcessSystemProvider();
            ServiceProvider = new ServiceProvider();
            NetworkProvider = new NetworkProvider();
            Program.PrimaryProviderThread =
                new ProviderThread(Settings.Instance.RefreshInterval);
            Program.PrimaryProviderThread.Add(ProcessProvider);
            Program.PrimaryProviderThread.Add(ServiceProvider);
            Program.PrimaryProviderThread.Add(NetworkProvider);
            Program.SecondaryProviderThread =
                new ProviderThread(Settings.Instance.RefreshInterval);
        }

        private static void LoadSettings(bool useSettings, string settingsFileName)
        {
            if (!useSettings)
            {
                Settings.Instance = new Settings(null);
                return;
            }

            if (settingsFileName == null)
            {
                bool success = true;
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                try
                {
                    if (!System.IO.Directory.Exists(appData + @"\Process Hacker"))
                    {
                        System.IO.Directory.CreateDirectory(appData + @"\Process Hacker");
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to create the settings directory", ex);
                    success = false;
                }

                if (success)
                {
                    settingsFileName = appData + @"\Process Hacker\settings.xml";
                }
                else
                {
                    settingsFileName = null;
                }
            }

            // Make sure we have an absolute path so we don't run into problems 
            // when saving.
            if (settingsFileName != null)
            {
                try
                {
                    settingsFileName = System.IO.Path.GetFullPath(settingsFileName);
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to parse the settings file name", ex);
                    Environment.Exit(1);
                }
            }

            try
            {
                Settings.Instance = new Settings(settingsFileName);
            }
            catch
            {
                // Settings file is probably corrupt. Ask the user.

                try { ThemingScope.Activate(); }
                catch { }

                DialogResult result;

                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();

                    td.MainIcon = TaskDialogIcon.Warning;
                    td.MainInstruction = "The settings file is corrupt";
                    td.WindowTitle = "Process Hacker";
                    td.Content = "The settings file used by Process Hacker is corrupt. You can either " +
                        "delete the settings file or start Process Hacker with default settings.";
                    td.UseCommandLinks = true;

                    td.Buttons = new TaskDialogButton[]
                    {
                        new TaskDialogButton((int)DialogResult.Yes, "Delete the settings file\n" + settingsFileName),
                        new TaskDialogButton((int)DialogResult.No, "Start with default settings\n" + 
                            "Any settings you change will not be saved."),
                    };
                    td.CommonButtons = TaskDialogCommonButtons.Cancel;

                    result = (DialogResult)td.Show();
                }
                else
                {
                    result = MessageBox.Show("The settings file used by Process Hacker is corrupt. It will be deleted " + 
                        "and all settings will be reset.", "Process Hacker", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.OK)
                        result = DialogResult.Yes;
                    else
                        result = DialogResult.Cancel;
                }

                if (result == DialogResult.Yes)
                {
                    try { System.IO.File.Delete(settingsFileName); }
                    catch (Exception ex)
                    {
                        PhUtils.ShowException("Unable to delete the settings file", ex);
                        Environment.Exit(1);
                    }

                    Settings.Instance = new Settings(settingsFileName);
                }
                else if (result == DialogResult.No)
                {
                    Settings.Instance = new Settings(null);
                }
                else if (result == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static bool ProcessCommandLine(Dictionary<string, string> pArgs)
        {
            if (pArgs.ContainsKey("-assistant"))
            {
                Assistant.Main(pArgs);

                return true;
            }

            if (pArgs.ContainsKey("-e"))
            {
                try
                {
                    ExtendedCmd.Run(pArgs);
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to complete the operation", ex);
                }

                return true;
            }

            if (pArgs.ContainsKey("-installkph"))
            {
                try
                {
                    using (var scm = new ServiceManagerHandle(ScManagerAccess.CreateService))
                    {
                        using (var shandle = scm.CreateService(
                            "KProcessHacker",
                            "KProcessHacker",
                            ServiceType.KernelDriver,
                            ServiceStartType.SystemStart,
                            ServiceErrorControl.Ignore,
                            Application.StartupPath + "\\kprocesshacker.sys",
                            null,
                            null,
                            null
                            ))
                        {
                            shandle.Start();
                        }
                    }
                }
                catch (WindowsException ex)
                {
                    // Need to pass status back.
                    Environment.Exit((int)ex.ErrorCode);
                }

                return true;
            }

            if (pArgs.ContainsKey("-uninstallkph"))
            {
                try
                {
                    using (var shandle = new ServiceHandle("KProcessHacker", ServiceAccess.Stop | (ServiceAccess)StandardRights.Delete))
                    {
                        try { shandle.Control(ServiceControl.Stop); }
                        catch { }

                        shandle.Delete();
                    }
                }
                catch (WindowsException ex)
                {
                    // Need to pass status back.
                    Environment.Exit((int)ex.ErrorCode);
                }

                return true;
            }

            if (pArgs.ContainsKey("-ip"))
                InspectPid = int.Parse(pArgs["-ip"]);

            if (pArgs.ContainsKey("-pw"))
            {
                int pid = int.Parse(pArgs["-pw"]);

                PrimaryProviderThread = new ProviderThread(Settings.Instance.RefreshInterval);
                SecondaryProviderThread = new ProviderThread(Settings.Instance.RefreshInterval);

                ProcessProvider = new ProcessSystemProvider();
                ServiceProvider = new ServiceProvider();
                PrimaryProviderThread.Add(ProcessProvider);
                PrimaryProviderThread.Add(ServiceProvider);
                ProcessProvider.Boost();
                ServiceProvider.Boost();
                ProcessProvider.Enabled = true;
                ServiceProvider.Enabled = true;

                Win32.LoadLibrary(Settings.Instance.DbgHelpPath);

                if (!ProcessProvider.Dictionary.ContainsKey(pid))
                {
                    PhUtils.ShowError("The process (PID " + pid.ToString() + ") does not exist.");
                    Environment.Exit(0);
                    return true;
                }

                ProcessWindow pw = new ProcessWindow(ProcessProvider.Dictionary[pid]);

                Application.Run(pw);

                PrimaryProviderThread.Dispose();
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
                    PhUtils.ShowException("Unable to show token properties", ex);
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
                    Rectangle rect = Utils.GetRectangle(pArgs["-rect"]);

                    options.Location = new Point(rect.X + 20, rect.Y + 20);
                    options.StartPosition = FormStartPosition.Manual;
                }

                options.SelectedTab = options.TabPages["tabAdvanced"];
                options.ShowDialog(window);

                return true;
            }

            if (pArgs.ContainsKey(""))
                if (pArgs[""].Replace("\"", "").Trim().EndsWith("taskmgr.exe", StringComparison.OrdinalIgnoreCase))
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
                else if (pArgs["-t"] == "2")
                    SelectTab = "Network";
            }

            return false;
        }

        public static void Unhook()
        {
            ProcessHacker.Native.Image.MappedImage file =
                new ProcessHacker.Native.Image.MappedImage(Environment.SystemDirectory + "\\ntdll.dll");
            IntPtr ntdll = Loader.GetDllHandle("ntdll.dll");
            MemoryProtection oldProtection;

            oldProtection = ProcessHandle.Current.ProtectMemory(
                ntdll,
                (int)file.Size,
                MemoryProtection.ExecuteReadWrite
                );

            for (int i = 0; i < file.Exports.Count; i++)
            {
                var entry = file.Exports.GetEntry(i);

                if (!entry.Name.StartsWith("Nt") || entry.Name.StartsWith("Ntdll"))
                    continue;

                byte[] fileData = new byte[5];

                unsafe
                {
                    IntPtr function = file.Exports.GetFunction(entry.Ordinal).Function;

                    Win32.RtlMoveMemory(
                        function.Decrement(new IntPtr(file.Memory)).Increment(ntdll),
                        function,
                        (5).ToIntPtr()
                        );
                }
            }

            ProcessHandle.Current.ProtectMemory(
                ntdll,
                (int)file.Size,
                oldProtection
                );

            file.Dispose();
        }

        public static void NopNtYieldExecution()
        {
            IntPtr ntYieldExecution = Loader.GetProcedure("ntdll.dll", "NtYieldExecution");

            if (ntYieldExecution != IntPtr.Zero)
            {
                ProcessHandle.Current.ProtectMemory(
                    ntYieldExecution,
                    12,
                    MemoryProtection.ExecuteReadWrite
                    );
                Win32.RtlFillMemory(ntYieldExecution, (12).ToIntPtr(), 0x90);
                ProcessHandle.Current.ProtectMemory(
                    ntYieldExecution,
                    12,
                    MemoryProtection.ExecuteRead
                    );
            }
        }

        public static bool CheckPreviousInstance()
        {
            // Close the handle to the mutex. If the object still exists, 
            // it means there is another handle to the mutex, and most likely 
            // there is another instance of PH running.

            if (GlobalMutex == null)
                return false;

            GlobalMutex.Dispose();
            GlobalMutex = null;

            try
            {
                return NativeUtils.ObjectExists(GlobalMutexName);
            }
            finally
            {
                try { GlobalMutex = new ProcessHacker.Native.Threading.Mutant(GlobalMutexName); }
                catch { }
            }
        }

        private static void ActivatePreviousInstance()
        {
            bool found = false;

            WindowHandle.Enumerate((window) =>
                {
                    if (window.GetText().Contains("Process Hacker ["))
                    {
                        int result;

                        window.SendMessageTimeout((WindowMessage)0x9991, 0, 0, SmtoFlags.Block, 5000, out result);

                        if (result == 0x1119)
                        {
                            window.SetForeground();
                            found = true;
                            return false;
                        }
                    }

                    return true;
                });

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
            StartProgramAdmin(ProcessHandle.Current.GetMainModule().FileName,
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
            info.lpFile = ProcessHandle.Current.GetMainModule().FileName;
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
                if (command.StartsWith("http://"))
                {
                    if (ex is System.ComponentModel.Win32Exception)
                    {
                        // Ignore file not found errors when opening web pages.
                        if ((ex as System.ComponentModel.Win32Exception).NativeErrorCode == 2)
                            return;
                    }
                }

                PhUtils.ShowException("Unable to start the process", ex);
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
                        // On Windows 7 if PH replaces Task Manager, PH will be 
                        // started with a command line of:
                        // ProcessHacker.exe "C:\...\taskmgr.exe" /4
                        // The following two lines are commented out due to this.

                        //if (dict.ContainsKey(""))
                        //    throw new Exception("Input file already specified.");

                        if (!dict.ContainsKey(""))
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
            // Garbage collections
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            // Compact the native heaps
            CompactNativeHeaps();
            // Terminate any unused threadpool threads
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

        public static void CompactNativeHeaps()
        {
            foreach (var heap in Heap.GetHeaps())
                heap.Compact(0);
        }

        public static string GetDiagnosticInformation()
        {
            StringBuilder info = new StringBuilder();
            AppDomain app = System.AppDomain.CurrentDomain;

            info.AppendLine("Process Hacker " + Application.ProductVersion);
            info.AppendLine("Process Hacker Build Time: " + Utils.GetAssemblyBuildDate(System.Reflection.Assembly.GetExecutingAssembly(), false).ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo));
            info.AppendLine("CLR Version: " + Environment.Version.ToString());
            info.AppendLine("OS Version: " + OSVersion.VersionString + " (" + OSVersion.BitsString + ")");
            info.AppendLine("Elevation: " + ElevationType.ToString());
            info.AppendLine("Working set: " + Utils.FormatSize(Environment.WorkingSet));

            if (Settings.Instance != null)
                info.AppendLine("Settings file: " + (Settings.Instance.SettingsFileName != null ? Settings.Instance.SettingsFileName : "(volatile)"));
            else
                info.AppendLine("Settings file: (not initialized)");

            if (KProcessHacker.Instance != null)
                info.AppendLine("KProcessHacker: " + KProcessHacker.Instance.Features.ToString());
            else
                info.AppendLine("KProcessHacker: not running");

            info.AppendLine();
            info.AppendLine("OBJECTS");

            int objectsCreatedCount = BaseObject.CreatedCount;
            int objectsFreedCount = BaseObject.FreedCount;

            info.AppendLine("Live: " + (objectsCreatedCount - objectsFreedCount).ToString());
            info.AppendLine("Created: " + objectsCreatedCount.ToString());
            info.AppendLine("Freed: " + objectsFreedCount.ToString());
            info.AppendLine("Disposed: " + BaseObject.DisposedCount.ToString());
            info.AppendLine("Finalized: " + BaseObject.FinalizedCount.ToString());
            info.AppendLine("Referenced: " + BaseObject.ReferencedCount.ToString());
            info.AppendLine("Dereferenced: " + BaseObject.DereferencedCount.ToString());

            info.AppendLine();
            info.AppendLine("PRIVATE HEAP");

            int heapAllocatedCount = MemoryAlloc.AllocatedCount;
            int heapFreedCount = MemoryAlloc.FreedCount;
            int heapReallocatedCount = MemoryAlloc.ReallocatedCount;

            info.AppendLine("Address: 0x" + MemoryAlloc.PrivateHeap.Address.ToString("x"));
            info.AppendLine("Live: " + (heapAllocatedCount - heapFreedCount).ToString());
            info.AppendLine("Allocated: " + heapAllocatedCount.ToString());
            info.AppendLine("Freed: " + heapFreedCount.ToString());
            info.AppendLine("Reallocated: " + heapReallocatedCount.ToString());

            info.AppendLine();
            info.AppendLine("MISCELLANEOUS COUNTERS");
            info.AppendLine("LSA lookup policy handle misses: " + LsaPolicyHandle.LookupPolicyHandleMisses.ToString());

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
            
            if (PrimaryProviderThread != null)
            {
                info.AppendLine("Count: " + PrimaryProviderThread.Count.ToString());

                foreach (IProvider provider in PrimaryProviderThread)
                    info.AppendLine(provider.GetType().FullName +
                        " (Enabled: " + provider.Enabled +
                        ", Busy: " + provider.Busy.ToString() +
                        ")");
            }
            else
            {
                info.AppendLine("(null)");
            }

            info.AppendLine();
            info.AppendLine("SECONDARY SHARED THREAD PROVIDER");

            if (SecondaryProviderThread != null)
            {
                info.AppendLine("Count: " + SecondaryProviderThread.Count.ToString());

                foreach (IProvider provider in SecondaryProviderThread)
                    info.AppendLine(provider.GetType().FullName +
                        " (Enabled: " + provider.Enabled +
                        ", Busy: " + provider.Busy.ToString() +
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

            //info.AppendLine();
            //info.AppendLine("LOADED MODULES");
            //info.AppendLine();

            //foreach (ProcessModule module in ProcessHandle.Current.GetModules())
            //{
            //    info.AppendLine("Module: " + module.BaseName);
            //    info.AppendLine("Location: " + module.FileName);

            //    DateTime fileCreatedInfo = System.IO.File.GetCreationTime(module.FileName);
            //    info.AppendLine(
            //        "Created: " + fileCreatedInfo.ToLongDateString().ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo) + " " +
            //        fileCreatedInfo.ToLongTimeString().ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo)
            //        );

            //    DateTime fileModifiedInfo = System.IO.File.GetLastWriteTime(module.FileName);
            //    info.AppendLine(
            //        "Modified: " + fileModifiedInfo.ToLongDateString().ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo) + " " +
            //        fileModifiedInfo.ToLongTimeString().ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo)
            //        );

            //    info.AppendLine("Version: " + System.Diagnostics.FileVersionInfo.GetVersionInfo(module.FileName).FileVersion);
            //    info.AppendLine();
            //}

            return info.ToString();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UnhandledException(e.Exception, false);
        }

        private static void UnhandledException(Exception ex, bool terminating)
        {
            Logging.Log(Logging.Importance.Critical, ex.ToString());

            ErrorDialog ed = new ErrorDialog(ex, terminating);

            ed.ShowDialog();
        }

        /// <summary>
        /// Creates an instance of the memory editor form.
        /// </summary>
        /// <param name="PID">The PID of the process to edit</param>
        /// <param name="address">The address to start editing at</param>
        /// <param name="length">The length to edit</param>
        public static MemoryEditor GetMemoryEditor(int PID, IntPtr address, long length)
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
        public static MemoryEditor GetMemoryEditor(int PID, IntPtr address, long length, MemoryEditorInvokeAction action)
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
                        action(ed);
                    if (!ed.IsDisposed)
                        Application.Run(ed);

                    Program.MemoryEditorsThreads.Remove(id);
                }), Utils.SixteenthStackSize);

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.MemoryEditorsThreads.Add(id, t);
            }
            else
            {
                ed = new MemoryEditor(PID, address, length);
                if (!ed.IsDisposed)
                    action(ed);
                if (!ed.IsDisposed)
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

                    if (!rw.IsDisposed)
                        action(rw); 
                    if (!rw.IsDisposed)
                        Application.Run(rw);

                    Program.ResultsThreads.Remove(id);
                }), Utils.SixteenthStackSize);

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                while (id == "") Thread.Sleep(1);
                Program.ResultsThreads.Add(id, t);
            }
            else
            {
                rw = new ResultsWindow(PID);
                if (!rw.IsDisposed)
                    action(rw);
                if (!rw.IsDisposed)
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

                    if (!pw.IsDisposed)
                        action(pw);
                    if (!pw.IsDisposed)
                        Application.Run(pw);

                    Program.PEThreads.Remove(path);
                }), Utils.SixteenthStackSize);

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PEThreads.Add(path, t);
            }
            else
            {
                pw = new PEWindow(path);
                if (!pw.IsDisposed)
                    action(pw);
                if (!pw.IsDisposed)
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

                    if (!pw.IsDisposed)
                        action(pw);
                    if (!pw.IsDisposed)
                        Application.Run(pw);

                    Program.PThreads.Remove(process.Pid);
                }), Utils.SixteenthStackSize);

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PThreads.Add(process.Pid, t);
            }
            else
            {
                pw = new ProcessWindow(process);
                if (!pw.IsDisposed)
                    action(pw);
                if (!pw.IsDisposed)
                    pw.Show();
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

        public static void UpdateWindowMenu(Menu windowMenuItem, Form f)
        {
            WeakReference<Form> fRef = new WeakReference<Form>(f);

            windowMenuItem.MenuItems.DisposeAndClear();

            MenuItem item;

            item = new MenuItem("&Always On Top");
            item.Tag = fRef;
            item.Click += new EventHandler(windowAlwaysOnTopItemClicked);
            item.Checked = f.TopMost;
            windowMenuItem.MenuItems.Add(item);

            item = new MenuItem("&Close");
            item.Tag = fRef;
            item.Click += new EventHandler(windowCloseItemClicked);
            windowMenuItem.MenuItems.Add(item);
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

        public static void SetTopMost(this Form f)
        {
            if (HackerWindowTopMost)
                f.TopMost = true;
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
            if (Settings.Instance.FloatChildWindows)
            {
                if (hideInTaskbar)
                    f.ShowInTaskbar = false;

                IntPtr oldParent = Win32.SetWindowLongPtr(f.Handle, GetWindowLongOffset.HwndParent, Program.HackerWindowHandle);

                //f.FormClosing += (sender, e) => Win32.SetWindowLongPtr(f.Handle, GetWindowLongOffset.HwndParent, oldParent);
            }
        }

        private static void windowAlwaysOnTopItemClicked(object sender, EventArgs e)
        {
            Form f = ((WeakReference<Form>)((MenuItem)sender).Tag).Target;

            if (f == null)
                return;

            f.Invoke(new MethodInvoker(delegate
                {
                    f.TopMost = !f.TopMost;

                    if (f == HackerWindow)
                        HackerWindowTopMost = f.TopMost;
                }));

            UpdateWindowMenu(((MenuItem)sender).Parent, f);
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
