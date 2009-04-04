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
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Principal;
using Microsoft.Samples;

namespace ProcessHacker
{
    public enum WindowsVersion
    {
        Unknown,
        XP,
        Vista
    }

    public static class Program
    {
        /// <summary>
        /// The main Process Hacker window instance
        /// </summary>
        public static HackerWindow HackerWindow;

        public static WindowsVersion WindowsVersion = WindowsVersion.Unknown;

        public static Win32.PROCESS_RIGHTS MinProcessQueryRights = Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION;
        public static Win32.PROCESS_RIGHTS MinProcessReadMemoryRights = Win32.PROCESS_RIGHTS.PROCESS_VM_READ;
        public static Win32.PROCESS_RIGHTS MinProcessWriteMemoryRights = Win32.PROCESS_RIGHTS.PROCESS_VM_WRITE | Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION;
        public static Win32.THREAD_RIGHTS MinThreadQueryRights = Win32.THREAD_RIGHTS.THREAD_QUERY_INFORMATION;

        public static int CurrentProcess;
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

        public const bool ThreadWindowsThreaded = false;
        public static Dictionary<string, ThreadWindow> ThreadWindows = new Dictionary<string, ThreadWindow>();
        public static Dictionary<string, Thread> ThreadThreads = new Dictionary<string, Thread>();

        public const bool PEWindowsThreaded = false;
        public static Dictionary<string, PEWindow> PEWindows = new Dictionary<string, PEWindow>();
        public static Dictionary<string, Thread> PEThreads = new Dictionary<string, Thread>();

        public const bool PWindowsThreaded = false;
        public static Dictionary<int, ProcessWindow> PWindows = new Dictionary<int, ProcessWindow>();
        public static Dictionary<int, Thread> PThreads = new Dictionary<int, Thread>();

        public delegate void ResultsWindowInvokeAction(ResultsWindow f);
        public delegate void MemoryEditorInvokeAction(MemoryEditor f);
        public delegate void ThreadWindowInvokeAction(ThreadWindow f);
        public delegate void PEWindowInvokeAction(PEWindow f);
        public delegate void PWindowInvokeAction(ProcessWindow f);
        public delegate void UpdateWindowAction(Form f, List<string> Texts, Dictionary<string, Form> TextToForm);

        public static System.Collections.Specialized.StringCollection ImposterNames = 
            new System.Collections.Specialized.StringCollection();
        public static bool StartHidden = false;
        public static bool StartVisible = false;
        public static bool ShowOptions = false;
        public static string SelectTab = "Processes";
        public static Win32.TOKEN_ELEVATION_TYPE ElevationType;
        public static KProcessHacker KPH;
        public static SharedThreadProvider SharedThreadProvider;
        public static SharedThreadProvider SecondarySharedThreadProvider;

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
                    "\t-o\tShows Options.\n" +
                    "\t-t n\tShows the specified tab. 0 is Processes, and 1 is Services.",
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pArgs = new Dictionary<string, string>();
            }

            // In case the settings file is corrupt PH won't crash here - it will be dealt with later.
            try
            {
                if (Properties.Settings.Default.AllowOnlyOneInstance && !pArgs.ContainsKey("-e"))
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
                    catch
                    {
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

            //Asm.LockedBus = 1;
            //Asm.Lowercase = true;
            //Asm.ExtraSpace = true;

            try
            {
                KPH = new KProcessHacker("KProcessHacker");
            }
            catch
            { }

            Win32.CreateMutex(0, false, "Global\\ProcessHackerMutex");

            if (Environment.OSVersion.Version.Major <= 5)
                WindowsVersion = WindowsVersion.XP;
            else if (Environment.OSVersion.Version.Major >= 6)
                WindowsVersion = WindowsVersion.Vista;

            if (WindowsVersion == WindowsVersion.Vista)
            {
                MinProcessQueryRights = Win32.PROCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION;
                MinThreadQueryRights = Win32.THREAD_RIGHTS.THREAD_QUERY_LIMITED_INFORMATION;
            }

            if (KPH != null && WindowsVersion == WindowsVersion.Vista)
            {
                MinProcessReadMemoryRights = MinProcessQueryRights;
                MinProcessWriteMemoryRights = MinProcessQueryRights;
            }

            try
            {
                using (var thandle = new Win32.ProcessHandle(System.Diagnostics.Process.GetCurrentProcess().Id,
                        MinProcessQueryRights).GetToken())
                {
                    try { thandle.SetPrivilege("SeDebugPrivilege", Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED); }
                    catch { }
                    try { thandle.SetPrivilege("SeShutdownPrivilege", Win32.SE_PRIVILEGE_ATTRIBUTES.SE_PRIVILEGE_ENABLED); }
                    catch { }

                    if (Program.WindowsVersion == WindowsVersion.Vista)
                    {
                        try { ElevationType = thandle.GetElevationType(); }
                        catch { ElevationType = Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull; }

                        if (ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault &&
                            !(new WindowsPrincipal(WindowsIdentity.GetCurrent())).
                                IsInRole(WindowsBuiltInRole.Administrator))
                            ElevationType = Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited;
                        else if (ElevationType == Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault)
                            ElevationType = Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
                    }
                    else
                    {
                        ElevationType = Win32.TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
                    }
                }
            }
            catch
            { }

            CurrentUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            CurrentProcess = Win32.OpenProcess(
                Win32.PROCESS_RIGHTS.PROCESS_ALL_ACCESS, 0, 
                System.Diagnostics.Process.GetCurrentProcess().Id);

            if (CurrentProcess == 0)
                CurrentProcess = 
                    System.Diagnostics.Process.GetCurrentProcess().Handle.ToInt32();

            try
            {
                CurrentSessionId = Win32.GetProcessSessionId(System.Diagnostics.Process.GetCurrentProcess().Id);
                System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }
            catch
            { }

            {
                if (pArgs.ContainsKey("-m"))
                    StartHidden = true;
                if (pArgs.ContainsKey("-v"))
                    StartVisible = true;
                if (pArgs.ContainsKey("-o"))
                    ShowOptions = true;

                if (pArgs.ContainsKey(""))
                    if (pArgs[""].Replace("\"", "").Trim().ToLower().EndsWith("taskmgr.exe"))
                        StartVisible = true;

                if (pArgs.ContainsKey("-t"))
                {
                    if (pArgs["-t"] == "0")
                        SelectTab = "Processes";
                    else if (pArgs["-t"] == "1")
                        SelectTab = "Services";
                }

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

                    return;
                }
            }

#if DEBUG
#else
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#endif

            new HackerWindow();
            Application.Run();
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

                        Win32.SendMessageTimeout(hWnd, (Win32.WindowMessage)0x9991, 0, 0,
                            Win32.SmtoFlags.Block, 5000, out result);

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
            StartProgramAdmin(Win32.ProcessHandle.FromHandle(Program.CurrentProcess).GetMainModule().FileName,
                args, successAction, Win32.ShowWindowType.Show, hWnd);
        }

        public static Win32.WaitResult StartProcessHackerAdminWait(string args, IntPtr hWnd, uint timeout)
        {
            Win32.SHELLEXECUTEINFO info = new Win32.SHELLEXECUTEINFO();

            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.SHELLEXECUTEINFO));
            info.lpFile = Win32.ProcessHandle.FromHandle(Program.CurrentProcess).GetMainModule().FileName;
            info.nShow = Win32.ShowWindowType.Show;
            info.fMask = 0x40; // SEE_MASK_NOCLOSEPROCESS
            info.lpVerb = "runas";
            info.lpParameters = args;
            info.hWnd = hWnd;

            if (Win32.ShellExecuteEx(ref info))
            {
                var result = Win32.WaitForSingleObject(info.hProcess, timeout);

                Win32.CloseHandle(info.hProcess);

                return result;
            }
            else
            {
                // An error occured - the user probably canceled the elevation dialog.
                return Win32.WaitResult.Abandoned;
            }
        }

        public static void StartProgramAdmin(string program, string args, 
            MethodInvoker successAction, Win32.ShowWindowType showType, IntPtr hWnd)
        {
            Win32.SHELLEXECUTEINFO info = new Win32.SHELLEXECUTEINFO();

            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.SHELLEXECUTEINFO));
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
            int workerThreads, completionPortThreads, maxWorkerThreads, maxCompletionPortThreads;

            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);

            workerThreads = maxWorkerThreads - workerThreads;
            completionPortThreads = maxCompletionPortThreads - completionPortThreads;

            ThreadPool.SetMaxThreads(0, 0);
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
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

                    System.Diagnostics.Process.GetCurrentProcess().Kill(); 
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
        /// Creates an instance of the thread window on a separate thread.
        /// </summary>
        public static ThreadWindow GetThreadWindow(int PID, int TID, SymbolProvider symbols)
        {
            return GetThreadWindow(PID, TID, symbols, new ThreadWindowInvokeAction(delegate { }));
        }

        /// <summary>
        /// Creates an instance of the thread window on a separate thread and invokes an action on that thread.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        public static ThreadWindow GetThreadWindow(int PID, int TID, SymbolProvider symbols, ThreadWindowInvokeAction action)
        {
            ThreadWindow tw = null;
            string id = PID + "-" + TID;

            if (ThreadWindows.ContainsKey(id))
            {
                tw = ThreadWindows[id];

                tw.Invoke(action, tw);

                return tw;
            }

            if (ThreadWindowsThreaded)
            {
                Thread t = new Thread(new ThreadStart(delegate
                {
                    tw = new ThreadWindow(PID, TID, symbols);

                    id = tw.Id;

                    action(tw);

                    try
                    {
                        Application.Run(tw);
                    }
                    catch
                    { }

                    Program.ThreadThreads.Remove(id);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                while (id == "") Thread.Sleep(1);
                Program.ThreadThreads.Add(id, t);
            }
            else
            {
                tw = new ThreadWindow(PID, TID, symbols);
                action(tw);
                tw.Show();
            }

            return tw;
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

            if (PWindows.ContainsKey(process.PID))
            {
                pw = PWindows[process.PID];

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

                    Program.PThreads.Remove(process.PID);
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Program.PThreads.Add(process.PID, t);
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

        public static void UpdateWindow(Form f, List<string> Texts, Dictionary<string, Form> TextToForm)
        {
            try
            {
                if (f.InvokeRequired)
                {
                    f.BeginInvoke(new UpdateWindowAction(UpdateWindow), f, Texts, TextToForm);

                    return;
                }

                MenuItem windowMenuItem = (MenuItem)f.GetType().GetProperty("WindowMenuItem").GetValue(f, null);
                wyDay.Controls.VistaMenu vistaMenu =
                    (wyDay.Controls.VistaMenu)f.GetType().GetProperty("VistaMenu").GetValue(f, null);
                MenuItem item;

                lock (windowMenuItem)
                {
                    foreach (MenuItem menuItem in windowMenuItem.MenuItems)
                    {
                        vistaMenu.SetImage(menuItem, null);
                        menuItem.Tag = null;
                    }

                    windowMenuItem.MenuItems.DisposeAndClear();

                    foreach (string s in Texts)
                    {
                        Bitmap image = new Bitmap(16, 16);

                        item = new MenuItem(s);
                        item.Tag = TextToForm[s];
                        item.Click += new EventHandler(windowItemClicked);

                        if (item.Tag == f)
                            item.DefaultItem = true;

                        windowMenuItem.MenuItems.Add(item);

                        // don't add icon on XP - doesn't work for some reason
                        if (Program.WindowsVersion == WindowsVersion.Vista)
                        {
                            using (Graphics g = Graphics.FromImage(image))
                            {
                                g.DrawIcon(TextToForm[s].Icon, new Rectangle(0, 0, 16, 16));

                                vistaMenu.SetImage(item, image);
                            }
                        }

                        image.Dispose();
                    }

                    windowMenuItem.MenuItems.Add(new MenuItem("-"));

                    item = new MenuItem("&Always On Top");
                    item.Tag = f;
                    item.Click += new EventHandler(windowAlwaysOnTopItemClicked);
                    item.Checked = f.TopMost;
                    windowMenuItem.MenuItems.Add(item);

                    item = new MenuItem("&Close");
                    item.Tag = f;
                    item.Click += new EventHandler(windowCloseItemClicked);
                    windowMenuItem.MenuItems.Add(item);
                    vistaMenu.SetImage(item, global::ProcessHacker.Properties.Resources.application_delete);
                }
            }
            catch
            { }
        }

        public static void UpdateWindows()
        {
            Dictionary<string, Form> TextToForm = new Dictionary<string, Form>();
            List<string> Texts = new List<string>();
            List<object> dics = new List<object>();
            List<Form> forms = new List<Form>();

            dics.Add(Program.MemoryEditors);
            dics.Add(Program.ResultsWindows);
            dics.Add(Program.ThreadWindows);
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

            TextToForm.Add("Process Hacker", HackerWindow);
            Texts.Add("Process Hacker");

            foreach (Form f in forms)
            {
                TextToForm.Add(f.Text, f);
                Texts.Add(f.Text);
            }

            Texts.Sort();

            UpdateWindow(HackerWindow, Texts, TextToForm);

            foreach (Form f in forms)
            {
                UpdateWindow(f, Texts, TextToForm);
            }
        }

        private static void windowItemClicked(object sender, EventArgs e)
        {
            Form f = (Form)((MenuItem)sender).Tag;

            Program.FocusWindow(f);
        }

        private static void windowAlwaysOnTopItemClicked(object sender, EventArgs e)
        {
            Form f = (Form)((MenuItem)sender).Tag;

            f.Invoke(new MethodInvoker(delegate { f.TopMost = !f.TopMost; }));

            Program.UpdateWindows();
        }

        private static void windowCloseItemClicked(object sender, EventArgs e)
        {
            Form f = (Form)((MenuItem)sender).Tag;

            f.Invoke(new MethodInvoker(delegate { f.Close(); }));
        }
    }
}
