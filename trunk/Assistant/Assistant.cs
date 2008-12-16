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
using System.Collections.Generic; 
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Assistant
{
    static class Program
    {
        const int SecurityDescriptorMinLength = 20;
        const int SecuritDescriptorRevision = 1;

        #region Enums & Structs

        enum LogonType : int
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkCleartext = 8,
            NewCredentials = 9
        }

        enum LogonProvider : int
        {
            Default = 0,
            WinNT35 = 1,
            WinNT40 = 2,
            WinNT50 = 3
        }

        [Flags]
        enum CreationFlags : uint
        {
            DebugProcess = 0x1,
            DebugOnlyThisProcess = 0x2,
            CreateSuspended = 0x4,
            DetachedProcess = 0x8,
            CreateNewConsole = 0x10,
            NormalPriorityClass = 0x20,
            IdlePriorityClass = 0x40,
            HighPriorityClass = 0x80,
            RealtimePriorityClass = 0x100,
            CreateNewProcessGroup = 0x200,
            CreateUnicodeEnvironment = 0x400,
            CreateSeparateWowVdm = 0x800,
            CreateSharedWowVdm = 0x1000,
            CreateForceDos = 0x2000,
            BelowNormalPriorityClass = 0x4000,
            AboveNormalPriorityClass = 0x8000,
            StackSizeParamIsAReservation = 0x10000,
            InheritCallerPriority = 0x20000,
            CreateProtectedProcess = 0x40000,
            ExtendedStartupInfoPresent = 0x80000,
            ProcessModeBackgroundBegin = 0x100000,
            ProcessModeBackgroundEnd = 0x200000,
            CreateBreakawayFromJob = 0x1000000,
            CreatePreserveCodeAuthzLevel = 0x2000000,
            CreateDefaultErrorMode = 0x4000000,
            CreateNoWindow = 0x8000000,
            ProfileUser = 0x10000000,
            ProfileKernel = 0x20000000,
            ProfileServer = 0x40000000,
            CreateIgnoreSystemDefault = 0x80000000
        }

        [Flags]
        enum StartupFlags : uint
        {
            UseShowWindow = 0x1,
            UseSize = 0x2,
            UsePosition = 0x4,
            UseCountChars = 0x8,
            UseFillAttribute = 0x10,
            RunFullScreen = 0x20,
            ForceOnFeedback = 0x40,
            ForceOffFeedback = 0x80,
            UseStdHandles = 0x100,   
            UseHotkey = 0x200
        }

        enum ShowWindowType : short
        {
            Hide = 0,
            ShowNormal = 1,
            Normal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNa = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11,
            Max = 11
        }

        [Flags]
        enum Access : uint
        {
            DesktopReadObjects = 0x1,
            DesktopWriteObjects = 0x80,
            ReadControl = 0x20000,
            WriteDac = 0x40000
        }

        [Flags]
        enum SIRequested : uint
        {
            OwnerSecurityInformation = 0x1,
            GroupSecurityInformation = 0x2,
            DaclSecurityInformation = 0x4,
            SaclSecurityInformation = 0x8,
            LabelSecurityInformation = 0x10
        }

        [Flags]
        enum AllocFlags : uint
        {
            LHnd = 0x42,
            LMemFixed = 0x0,
            LMemMoveable = 0x2,
            LMemZeroInit = 0x40,
            LPtr = 0x40,
            NonZeroLHnd = LMemMoveable,
            NonZeroLPtr = LMemFixed
        }

        enum TokenInformationClass
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass  // MaxTokenInfoClass should always be the last enum
        }

        enum ImpersonationLevel
        {
            SecurityAnonymous = 0,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        enum TokenType
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        [Flags]
        public enum STANDARD_RIGHTS : uint
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000f0000,

            STANDARD_RIGHTS_READ = READ_CONTROL,
            STANDARD_RIGHTS_WRITE = READ_CONTROL,
            STANDARD_RIGHTS_EXECUTE = READ_CONTROL,

            STANDARD_RIGHTS_ALL = 0x001f0000,

            SPECIFIC_RIGHTS_ALL = 0x0000ffff,
            ACCESS_SYSTEM_SECURITY = 0x01000000,
            MAXIMUM_ALLOWED = 0x02000000,
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000
        }

        [Flags]
        public enum TokenRights : uint
        {
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED |
                TOKEN_ASSIGN_PRIMARY |
                TOKEN_DUPLICATE |
                TOKEN_IMPERSONATE |
                TOKEN_QUERY |
                TOKEN_QUERY_SOURCE |
                TOKEN_ADJUST_PRIVILEGES |
                TOKEN_ADJUST_GROUPS |
                TOKEN_ADJUST_DEFAULT |
                TOKEN_ADJUST_SESSIONID,
            TOKEN_READ = STANDARD_RIGHTS.STANDARD_RIGHTS_READ | TOKEN_QUERY,
            TOKEN_WRITE = STANDARD_RIGHTS.STANDARD_RIGHTS_WRITE |
                TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT,
            TOKEN_EXECUTE = STANDARD_RIGHTS.STANDARD_RIGHTS_EXECUTE
        }

        [StructLayout(LayoutKind.Sequential)]
        struct StartupInfo
        {
            public int Size;
            public int Reserved;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Desktop;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Title;

            public int X;
            public int Y;
            public int XSize;
            public int YSize;
            public int XCountChars;
            public int YCountChars;
            public int FillAttribute;
            public StartupFlags Flags;

            public ShowWindowType ShowWindow;
            public short Reserved2;
            public int Reserved3;

            public int StdInputHandle;
            public int StdOutputHandle;
            public int StdErrorHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ProcessInformation
        {
            public int ProcessHandle;
            public int ThreadHandle;
            public int ProcessId;
            public int ThreadId;
        }

        #endregion

        #region Imports

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CloseHandle(int Handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(int ProcessHandle, TokenRights DesiredAccess,
            out int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool DuplicateToken(
            int TokenHandle,
            int Level,
            out int DuplicatedToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool DuplicateTokenEx(
            int TokenHandle,
            TokenRights DesiredAccess,
            int TokenAttributes,
            ImpersonationLevel ImpersonationLevel,
            TokenType TokenType,
            out int DuplicatedToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool SetTokenInformation(
            int TokenHandle,
            TokenInformationClass TokenInformationClass,
            ref int SessionId,
            int InformationLength);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool LogonUser(
            [MarshalAs(UnmanagedType.LPWStr)] string Username,
            [MarshalAs(UnmanagedType.LPWStr)] string Domain,
            [MarshalAs(UnmanagedType.LPWStr)] string Password,
            LogonType LogonType, LogonProvider LogonProvider,
            out int TokenHandle
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CreateProcessAsUser(
            int TokenHandle,
            [MarshalAs(UnmanagedType.LPWStr)] string ApplicationName,
            [MarshalAs(UnmanagedType.LPWStr)] string CommandLine,
            int ProcessAttributes,
            int ThreadAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool InheritHandles,
            CreationFlags CreationFlags,
            int Environment,
            [MarshalAs(UnmanagedType.LPWStr)] string CurrentDirectory,
            [MarshalAs(UnmanagedType.Struct)] ref StartupInfo StartupInfo,
            [MarshalAs(UnmanagedType.Struct)] ref ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CreateProcess(
            [MarshalAs(UnmanagedType.LPWStr)] string ApplicationName,
            [MarshalAs(UnmanagedType.LPWStr)] string CommandLine,
            int ProcessAttributes,
            int ThreadAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool InheritHandles,
            CreationFlags CreationFlags,
            int Environment,
            [MarshalAs(UnmanagedType.LPWStr)] string CurrentDirectory,
            [MarshalAs(UnmanagedType.Struct)] ref StartupInfo StartupInfo,
            [MarshalAs(UnmanagedType.Struct)] ref ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LocalAlloc(AllocFlags Flags, int Bytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LocalFree(IntPtr Memory);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetThreadDesktop(int ThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int OpenDesktop(
            [MarshalAs(UnmanagedType.LPWStr)] string Desktop,
            int Flags,
            [MarshalAs(UnmanagedType.Bool)] bool Inherit,
            Access DesiredAccess
            );

        [DllImport("user32.dll")]
        static extern bool CloseDesktop(int Handle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int OpenWindowStation(
            [MarshalAs(UnmanagedType.LPWStr)] string WinSta,
            [MarshalAs(UnmanagedType.Bool)] bool Inherit,
            Access DesiredAccess
            );

        [DllImport("user32.dll")]
        static extern bool CloseWindowStation(int Handle);

        [DllImport("user32.dll")]
        static extern bool SetUserObjectSecurity(
            int Handle, 
            ref SIRequested SIRequested,
            IntPtr SID
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool InitializeSecurityDescriptor(IntPtr SecurityDescriptor, int Revision);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool SetSecurityDescriptorDacl(IntPtr SecurityDescriptor, 
            [MarshalAs(UnmanagedType.Bool)] bool DaclPresent, 
            int Dacl,      
            [MarshalAs(UnmanagedType.Bool)] bool DaclDefaulted
            );

        #endregion

        static void ThrowLastWin32Error()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        static string GetLastErrorMessage()
        {
            try
            {
                ThrowLastWin32Error();
            }
            catch (Win32Exception ex)
            {
                return ex.Message;
            }

            throw new Exception("wtf");
        }

        static void SetAllAccess(int ObjectHandle)
        {
            IntPtr sd = LocalAlloc(AllocFlags.LPtr, SecurityDescriptorMinLength);

            try
            {
                if (!InitializeSecurityDescriptor(sd, SecuritDescriptorRevision))
                    ThrowLastWin32Error();

                if (!SetSecurityDescriptorDacl(sd, true, 0, false))
                    ThrowLastWin32Error();

                SIRequested si = SIRequested.DaclSecurityInformation;

                if (!SetUserObjectSecurity(ObjectHandle, ref si, sd))
                    ThrowLastWin32Error();
            }
            finally
            {
                LocalFree(sd);
            }
        }

        static void SetDesktopWinStaAccess()
        {
            int WinStaHandle, DesktopHandle;

            if ((WinStaHandle = OpenWindowStation("WinSta0", false, Access.WriteDac)) == 0)
                ThrowLastWin32Error();

            try
            {
                SetAllAccess(WinStaHandle);
            }
            finally
            {
                CloseWindowStation(WinStaHandle);
            }

            if ((DesktopHandle = OpenDesktop("Default", 0, false, 
                Access.WriteDac | Access.DesktopReadObjects | Access.DesktopWriteObjects)) == 0)
                ThrowLastWin32Error();

            try
            {
                SetAllAccess(DesktopHandle);
            }
            finally
            {
                CloseDesktop(DesktopHandle);
            }
        }

        static Dictionary<string, string> ParseArgs(string[] args)
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

        static void PrintUsage()
        {
            Console.Write("Process Hacker Assistant\nCopyright (c) 2008 wj32. Licensed under the GNU GPL v3.\n\nUsage:\n" +
                "\tassistant [-w] [-k] [-P pid] [-u username] [-p password] [-t logontype] [-s sessionid] [-d dir] " + 
                "[-c cmdline] [-f filename]\n\n" +
                "-w\t\tSpecifies that the permissions of WinSta0 and WinSta0\\Default should be " +
                "modified with all access. You should use this option as a normal user (\"assistant -w\") before attempting to " +
                "use this program as a Windows service.\n" + 
                "-k\t\tDebugging purposes: specifies that this program should sleep after completion.\n" +
                "-P pid\t\t\"Steals\" the token of the specified process to start the specified program. You must not use " + 
                "the -u and -p options with this option.\n" + 
                "-u username\tSpecifies the user under which the program should be run. The username can be specified " + 
                "as username, domain\\username, or username@domain. On Windows XP, specifying NT AUTHORITY\\SYSTEM does " +
                "not work. Instead, omit the -u option and specify \"-t newcredentials\".\n" +
                "-p password\tSpecifies the password for the user.\n" +
                "-t logontype\tSpecifies the logon type. For logons to normal users, specify \"interactive\". For logons " + 
                "to NT AUTHORITY\\SYSTEM, LOCAL SERVICE or NETWORK SERVICE, specify \"service\".\n" + 
                "-s sessionid\tSpecifies the session ID under which the program should be run.\n" +
                "-d dir\t\tSpecifies the current directory for the program.\n" +
                "-c cmdline\tSpecifies the command line for the program. You must not use the -f option if you use this.\n" +
                "-f filename\tSpecifies the full path to the program.\n\n" + 
                "This application is not useful by itself; even Administrators do not normally have " + 
                "SeAssignPrimaryTokenPrivilege and SeTcbPrivilege, both of which are required for the useful " + 
                "functioning of this program. You must create a Windows service for this program:\n" + 
                "\tsc.exe create PHAssistant binPath= \"\\\"[path to this program]\\\" -u \\\"SYSTEM@NT AUTHORITY\\\" " + 
                "-t service -s [your session Id, normally 0 on XP and 1 on Vista] -c calc.exe\"\n" + 
                "then start it:\n\tsc.exe start PHAssistant\n" + 
                "and finally delete it:\n\tsc.exe delete PHAssistant\n");
        }

        static void Exit(int exitCode)
        {
            if (args.ContainsKey("-k"))
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            Environment.Exit(exitCode);
        }

        static void Exit()
        {
            Exit(0);
        }

        static Dictionary<string, string> args;

        static void Main()
        {
            try
            {
                args = ParseArgs(Environment.GetCommandLineArgs());

                bool bad = false;

                if (!args.ContainsKey("-w"))
                {
                    if (!args.ContainsKey("-c") && !args.ContainsKey("-f"))
                        bad = true;

                    if (args.ContainsKey("-c") && args.ContainsKey("-f"))
                        bad = true;

                    if (!args.ContainsKey("-u") && !args.ContainsKey("-P"))
                        bad = true;

                    if (args.ContainsKey("-u") && args.ContainsKey("-P"))
                        bad = true;
                }

                if (args.ContainsKey("-v") || args.ContainsKey("-h"))
                    bad = true;

                if (bad)
                {
                    PrintUsage();
                    Exit();
                }
            }
            catch
            {
                PrintUsage();
                Exit();
            }

            if (args.ContainsKey("-w"))
            {
                try
                {
                    SetDesktopWinStaAccess();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: Could not set desktop and window station access: " + ex.Message);
                }
            }

            int token = 0;

            if (args.ContainsKey("-u"))
            {
                string user = args["-u"];
                string domain = null;
                string username = "";

                if (user.Contains("\\"))
                {
                    domain = user.Split('\\')[0];
                    username = user.Split('\\')[1];
                }
                else if (user.Contains("@"))
                {
                    username = user.Split('@')[0];
                    domain = user.Split('@')[1];
                }
                else
                {
                    username = user;
                }

                LogonType type = LogonType.Interactive;

                if (args.ContainsKey("-t"))
                {
                    try
                    {
                        type = (LogonType)Enum.Parse(typeof(LogonType), args["-t"], true);
                    }
                    catch
                    {
                        Console.WriteLine("Error: Invalid logon type.");
                        Exit(-1);
                    }
                }
                
                if (!LogonUser(username, domain, args.ContainsKey("-p") ? args["-p"] : "", type,
                    LogonProvider.Default, out token))
                {
                    Console.WriteLine("Error: Could not logon as user: " + GetLastErrorMessage());
                    Exit(Marshal.GetLastWin32Error());
                }
            }
            else
            {
                int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

                try
                {
                    if (args.ContainsKey("-P"))
                        pid = int.Parse(args["-P"]);
                }
                catch
                {
                    Console.WriteLine("Error: Invalid PID.");
                }

                int handle = 0;

                try
                {
                    handle = System.Diagnostics.Process.GetProcessById(pid).Handle.ToInt32();
                }
                catch
                {
                    Console.WriteLine("Error: Could not open process.");
                }

                if (!OpenProcessToken(handle, TokenRights.TOKEN_ALL_ACCESS, out token))
                {
                    Console.WriteLine("Error: Could not open process token: " + GetLastErrorMessage());
                    Exit(Marshal.GetLastWin32Error());
                }

                if (Environment.OSVersion.Version.Major != 5)
                {
                    int dupToken;

                    if (!DuplicateTokenEx(token, TokenRights.TOKEN_ALL_ACCESS, 0, ImpersonationLevel.SecurityImpersonation,
                        TokenType.TokenPrimary, out dupToken))
                    {
                        Console.WriteLine("Error: Could not duplicate own token: " + GetLastErrorMessage());
                        Exit(Marshal.GetLastWin32Error());
                    }

                    CloseHandle(token);
                    token = dupToken;
                }
            }

            if (args.ContainsKey("-s"))
            {
                int sessionId = int.Parse(args["-s"]);

                if (!SetTokenInformation(token, TokenInformationClass.TokenSessionId, ref sessionId, 4))
                {
                    Console.WriteLine("Error: Could not set token session Id: " + GetLastErrorMessage());
                }
            }

            if (args.ContainsKey("-c") || args.ContainsKey("-f"))
            {
                StartupInfo info = new StartupInfo();
                ProcessInformation pinfo = new ProcessInformation();

                info.Size = Marshal.SizeOf(info);
                info.Desktop = "WinSta0\\Default";

                if (!CreateProcessAsUser(token,
                    args.ContainsKey("-f") ? args["-f"] : null,
                    args.ContainsKey("-c") ? args["-c"] : null,
                    0, 0, false, 0, 0,
                    args.ContainsKey("-d") ? args["-d"] : null,
                    ref info, ref pinfo))
                {
                    Console.WriteLine("Error: Could not create process: " + GetLastErrorMessage());
                    Exit(Marshal.GetLastWin32Error());
                }

                CloseHandle(token);
            }

            Exit();
        }
    }
}
