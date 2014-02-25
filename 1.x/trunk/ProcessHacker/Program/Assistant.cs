/*
 * Process Hacker Assistant
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
using System.Runtime.InteropServices;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker
{
    public static class Assistant
    {
        private static Dictionary<string, string> args;

        public static void SetDesktopWinStaAccess()
        {
            using (var wsHandle = new WindowStationHandle("WinSta0", (WindowStationAccess)StandardRights.WriteDac))
                wsHandle.SetSecurity(SecurityInformation.Dacl, new SecurityDescriptor());

            using (var dhandle = new DesktopHandle("Default", false,
                (DesktopAccess)StandardRights.WriteDac | DesktopAccess.ReadObjects | DesktopAccess.WriteObjects))
                dhandle.SetSecurity(SecurityInformation.Dacl, new SecurityDescriptor());
        }

        private static void PrintUsage()
        {
            Console.Write("Process Hacker Assistant\nCopyright (c) 2008 wj32. Licensed under the GNU GPL v3.\n\nUsage:\n" +
                "\tassistant [-w] [-k] [-P pid] [-u username] [-p password] [-t logontype] [-s sessionid] [-d dir] " + 
                "[-c cmdline] [-f filename] [-E name]\n\n" +
                "-w\t\tSpecifies that the permissions of WinSta0 and WinSta0\\Default should be " +
                "modified with all access. You should use this option as a normal user (\"assistant -w\") before attempting to " +
                "use this program as a Windows service.\n" + 
                "-k\t\tDebugging purposes: specifies that this program should sleep after completion.\n" +
                "-P pid\t\t\"Steals\" the token of the specified process to start the specified program. You must not use " + 
                "the -u and -p options with this option.\n" + 
                "-u username\tSpecifies the user under which the program should be run. The username can be specified " + 
                "as username, domain\\username, or username@domain. On Windows XP, specifying NT AUTHORITY\\SYSTEM does " +
                "not work by itself. You must specify \"-t newcredentials\" as well.\n" +
                "-p password\tSpecifies the password for the user.\n" +
                "-t logontype\tSpecifies the logon type. For logons to normal users, specify \"interactive\". For logons " + 
                "to NT AUTHORITY\\SYSTEM, LOCAL SERVICE or NETWORK SERVICE, specify \"service\" (see above for using SYSTEM on " +
                "Windows XP).\n" + 
                "-s sessionid\tSpecifies the session ID under which the program should be run.\n" +
                "-d dir\t\tSpecifies the current directory for the program.\n" +
                "-c cmdline\tSpecifies the command line for the program. You must not use the -f option if you use this.\n" +
                "-f filename\tSpecifies the full path to the program.\n" +
                "-E name\tSpecifies the partial name of the mailslot to write a 4-byte error code to.\n" +
                "\n" +
                "This application is not useful by itself; even Administrators do not normally have " + 
                "SeAssignPrimaryTokenPrivilege and SeTcbPrivilege, both of which are required for the useful " + 
                "functioning of this program. You must create a Windows service for this program:\n" + 
                "\tsc.exe create PHAssistant binPath= \"\\\"[path to this program]\\\" -u \\\"SYSTEM@NT AUTHORITY\\\" " + 
                "-t service -s [your session Id, normally 0 on XP and 1 on Vista] -c calc.exe\"\n" + 
                "then start it:\n\tsc.exe start PHAssistant\n" + 
                "and finally delete it:\n\tsc.exe delete PHAssistant\n");
        }

        private static void Exit(int exitCode)
        {
            if (args.ContainsKey("-k"))
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            if (args.ContainsKey("-E"))
            {
                string mailslotName = args["-E"];

                using (var fhandle = new FileHandle(
                    @"\Device\Mailslot\" + mailslotName,
                    FileShareMode.ReadWrite,
                    FileAccess.GenericWrite
                    ))
                    fhandle.Write(exitCode.GetBytes());
            }

            Environment.Exit(exitCode);
        }

        private static void Exit()
        {
            Exit(0);
        }

        private static bool EnablePrivilege(string name)
        {
            try
            {
                Privilege.Enable(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Main(Dictionary<string, string> pArgs)
        {
            args = pArgs;

            EnablePrivilege("SeAssignPrimaryTokenPrivilege");
            EnablePrivilege("SeBackupPrivilege");
            EnablePrivilege("SeRestorePrivilege");

            try
            {
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

            TokenHandle token = null;
            string domain = null;
            string username = "";

            if (args.ContainsKey("-u"))
            {
                string user = args["-u"];

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

                try
                {
                    token = TokenHandle.Logon(
                        username,
                        domain,
                        args.ContainsKey("-p") ? args["-p"] : "",
                        type,
                        LogonProvider.Default
                        );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not logon as user: " + ex.Message);
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

                try
                {
                    using (var phandle = new ProcessHandle(pid, OSVersion.MinProcessQueryInfoAccess))
                    {
                        try
                        {
                            token = phandle.GetToken(TokenAccess.All);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: Could not open process token: " + ex.Message);
                            Exit(Marshal.GetLastWin32Error());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not open process: " + ex.Message);
                    Exit(Marshal.GetLastWin32Error());
                }

                // Need to duplicate the token if we're going to set the session ID.
                if (args.ContainsKey("-s"))
                {
                    try
                    {
                        TokenHandle dupToken;

                        dupToken = token.Duplicate(
                            TokenAccess.All,
                            SecurityImpersonationLevel.SecurityImpersonation,
                            TokenType.Primary
                            );
                        token.Dispose();
                        token = dupToken;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: Could not duplicate own token: " + ex.Message);
                        Exit(Marshal.GetLastWin32Error());
                    }
                }
            }

            if (args.ContainsKey("-s"))
            {
                int sessionId = int.Parse(args["-s"]);

                try
                {
                    token.SetSessionId(sessionId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not set token session ID: " + ex.Message);
                }
            }

            if (args.ContainsKey("-c") || args.ContainsKey("-f"))
            {
                if (!args.ContainsKey("-e"))
                {
                    EnvironmentBlock environment;
                    StartupInfo startupInfo = new StartupInfo();
                    ProcessHandle processHandle;
                    ThreadHandle threadHandle;
                    ClientId clientId;

                    environment = new EnvironmentBlock(token);
                    startupInfo.Desktop = "WinSta0\\Default";

                    try
                    {
                        processHandle = ProcessHandle.CreateWin32(
                            token,
                            args.ContainsKey("-f") ? args["-f"] : null,
                            args.ContainsKey("-c") ? args["-c"] : null,
                            false,
                            ProcessCreationFlags.CreateUnicodeEnvironment,
                            environment,
                            args.ContainsKey("-d") ? args["-d"] : null,
                            startupInfo,
                            out clientId,
                            out threadHandle
                            );
                        processHandle.Dispose();
                        threadHandle.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: Could not create process: " + ex.Message);
                        Exit(Marshal.GetLastWin32Error());
                    }
                    finally
                    {
                        environment.Destroy();
                    }
                }
            }

            token.Dispose();

            Exit();
        }
    }
}
