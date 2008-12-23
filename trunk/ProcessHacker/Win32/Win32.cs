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
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessHacker
{
    /// <summary>
    /// Provides interfacing to the Win32 and Native APIs.
    /// </summary>
    public partial class Win32
    {
        /// <summary>
        /// Contains code which uses pointers.
        /// </summary>
        public unsafe class Unsafe
        {
            /// <summary>
            /// Converts a multi-string into a managed string array. A multi-string 
            /// consists of an array of null-terminated strings plus an extra null to 
            /// terminate the array.
            /// </summary>
            /// <param name="ptr">The pointer to the array.</param>
            /// <returns>A string array.</returns>
            public static string[] GetMultiString(IntPtr ptr)
            {
                List<string> list = new List<string>();
                char* chptr = (char*)ptr.ToPointer();
                StringBuilder currentString = new StringBuilder();

                while (true)
                {
                    while (*chptr != 0)
                    {
                        currentString.Append(*chptr);  
                        chptr++;
                    }

                    string str = currentString.ToString();

                    if (str == "")
                    {
                        break;
                    }
                    else
                    {
                        list.Add(str);
                        currentString = new StringBuilder();
                    }
                }

                return list.ToArray();
            }
        }

        public delegate int EnumWindowsProc(int hwnd, int param);
        public delegate int SymEnumSymbolsProc(SYMBOL_INFO pSymInfo, int SymbolSize, int UserContext);
        public delegate int FunctionTableAccessProc64(int ProcessHandle, int AddrBase);
        public delegate int GetModuleBaseProc64(int ProcessHandle, int Address);

        public static Dictionary<byte, string> ObjectTypes = new Dictionary<byte, string>();

        #region Consts

        public const int ANYSIZE_ARRAY = 1;
        public const int DONT_RESOLVE_DLL_REFERENCES = 0x1;
        public const int ERROR_NO_MORE_ITEMS = 259;
        public const int MAXIMUM_SUPPORTED_EXTENSION = 512;
        public const int SEE_MASK_INVOKEIDLIST = 0xc;
        public const uint SERVICE_NO_CHANGE = 0xffffffff;
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;
        public const int SID_SIZE = 0x1000;
        public const int SIZE_OF_80387_REGISTERS = 72;
        public const uint STATUS_INFO_LENGTH_MISMATCH = 0xc0000004;
        public const int SW_SHOW = 5;
        public const int SYMBOL_NAME_MAXSIZE = 255;
        public const int WAIT_ABANDONED = 0x80;
        public const int WAIT_OBJECT_0 = 0x0;
        public const int WAIT_TIMEOUT = 0x102;

        #endregion    

        #region Errors

        /// <summary>
        /// Gets the error message associated with the specified error code.
        /// </summary>
        /// <param name="ErrorCode">The error code.</param>
        /// <returns>An error message.</returns>
        public static string GetErrorMessage(int ErrorCode)
        {
            try
            {
                throw new System.ComponentModel.Win32Exception(ErrorCode);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets the error message associated with the last error that occured.
        /// </summary>
        /// <returns>An error message.</returns>
        public static string GetLastErrorMessage()
        {
            return GetErrorMessage(Marshal.GetLastWin32Error());
        }

        #endregion

        #region Handles

        public struct ObjectInformation
        {
            public OBJECT_BASIC_INFORMATION Basic;
            public OBJECT_NAME_INFORMATION Name;
            public string OrigName;
            public string BestName;
            public string TypeName;
        }

        /// <summary>
        /// Enumerates the handles opened by every running process.
        /// </summary>
        /// <returns>An array containing information about the handles.</returns>
        public static SYSTEM_HANDLE_INFORMATION[] EnumHandles()
        {
            int retLength = 0;
            int handleCount = 0;
            SYSTEM_HANDLE_INFORMATION[] returnHandles;

            using (MemoryAlloc data = new MemoryAlloc(0x1000))
            {
                while (ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, data.Memory,
                    data.Size, ref retLength) == STATUS_INFO_LENGTH_MISMATCH)
                    data.Resize(data.Size * 2);

                handleCount = data.ReadInt32(0);
                returnHandles = new SYSTEM_HANDLE_INFORMATION[handleCount];

                for (int i = 0; i < handleCount; i++)
                {
                    returnHandles[i] = data.ReadStruct<SYSTEM_HANDLE_INFORMATION>(4, i);
                }

                return returnHandles;
            }
        }

        public static ObjectInformation GetHandleInfo(SYSTEM_HANDLE_INFORMATION handle)
        {
            using (ProcessHandle process = new ProcessHandle(handle.ProcessId, PROCESS_RIGHTS.PROCESS_DUP_HANDLE))
            {
                return GetHandleInfo(process, handle);
            }
        }

        public static ObjectInformation GetHandleInfo(ProcessHandle process, SYSTEM_HANDLE_INFORMATION handle)
        {
            int object_handle;
            int retLength = 0;

            if (ZwDuplicateObject(process.Handle, handle.Handle,
                Program.CurrentProcess, out object_handle, 0, 0, 0) != 0)
                throw new Exception("Could not duplicate object!");

            try
            {
                ObjectInformation info = new ObjectInformation();

                ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectBasicInformation,
                    IntPtr.Zero, 0, ref retLength);

                if (retLength > 0)
                {
                    using (MemoryAlloc obiMem = new MemoryAlloc(retLength))
                    {
                        ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectBasicInformation,
                            obiMem.Memory, obiMem.Size, ref retLength);

                        OBJECT_BASIC_INFORMATION obi = obiMem.ReadStruct<OBJECT_BASIC_INFORMATION>();
                        info.Basic = obi;
                    }
                }

                if (ObjectTypes.ContainsKey(handle.ObjectTypeNumber))
                {
                    info.TypeName = ObjectTypes[handle.ObjectTypeNumber];
                }
                else
                {
                    ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation,
                        IntPtr.Zero, 0, ref retLength);

                    if (retLength > 0)
                    {
                        using (MemoryAlloc otiMem = new MemoryAlloc(retLength))
                        {
                            if (ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation,
                                otiMem.Memory, otiMem.Size, ref retLength) != 0)
                                throw new Exception("ZwQueryObject failed");

                            OBJECT_TYPE_INFORMATION oti = otiMem.ReadStruct<OBJECT_TYPE_INFORMATION>();

                            info.TypeName = ReadUnicodeString(oti.Name);
                            ObjectTypes.Add(handle.ObjectTypeNumber, info.TypeName);
                        }
                    }
                }

                if (info.TypeName == "File")
                    if ((int)handle.GrantedAccess == 0x0012019f)
                        throw new Exception("0x0012019f access is banned");

                ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation,
                    IntPtr.Zero, 0, ref retLength);
                
                if (retLength > 0)
                {
                    using (MemoryAlloc oniMem = new MemoryAlloc(retLength))
                    {
                        if (ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation,
                            oniMem.Memory, oniMem.Size, ref retLength) != 0)
                            throw new Exception("ZwQueryObject failed");

                        OBJECT_NAME_INFORMATION oni = oniMem.ReadStruct<OBJECT_NAME_INFORMATION>();

                        info.OrigName = ReadUnicodeString(oni.Name);
                        info.Name = oni;
                    }
                }

                try
                {
                    switch (info.TypeName)
                    {
                        case "Key":
                            string hklmString = "\\registry\\machine";
                            string hkcrString = "\\registry\\machine\\software\\classes";
                            string hkcuString = "\\registry\\user\\" +
                                System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString().ToLower();
                            string hkcucrString = "\\registry\\user\\" +
                                System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString().ToLower() + "_classes";
                            string hkuString = "\\registry\\user";

                            if (info.OrigName.ToLower().StartsWith(hklmString))
                                info.BestName = "HKLM" + info.OrigName.Substring(hklmString.Length);
                            else if (info.OrigName.ToLower().StartsWith(hkcucrString))
                                info.BestName = "HKCU\\Software\\Classes" + info.OrigName.Substring(hkcucrString.Length);
                            else if (info.OrigName.ToLower().StartsWith(hkcuString))
                                info.BestName = "HKCU" + info.OrigName.Substring(hkcuString.Length);
                            else if (info.OrigName.ToLower().StartsWith(hkcrString))
                                info.BestName = "HKCR" + info.OrigName.Substring(hkcrString.Length);
                            else if (info.OrigName.ToLower().StartsWith(hkuString))
                                info.BestName = "HKU" + info.OrigName.Substring(hkuString.Length);
                            else
                                info.BestName = info.OrigName;

                            break;

                        case "Process":
                            {
                                int process_handle = 0;
                                int processId = 0;

                                if (ZwDuplicateObject(process.Handle, handle.Handle,
                                    Program.CurrentProcess, out process_handle,
                                    (STANDARD_RIGHTS)Program.MinProcessQueryRights, 0, 0) != 0)
                                    throw new Exception("Could not duplicate process handle!");

                                try
                                {
                                    if ((processId = GetProcessId(process_handle)) == 0)
                                        throw new Exception(GetLastErrorMessage());

                                    if (Program.HackerWindow.ProcessProvider.Dictionary.ContainsKey(processId))
                                        info.BestName = Program.HackerWindow.ProcessProvider.Dictionary[processId].Name +
                                            " (" + processId.ToString() + ")";
                                    else
                                        info.BestName = "Non-existent process (" + processId.ToString() + ")";
                                }
                                finally
                                {
                                    CloseHandle(process_handle);
                                }
                            }

                            break;

                        case "Thread":
                            {
                                int thread_handle = 0;
                                int processId = 0;
                                int threadId = 0;

                                if (ZwDuplicateObject(process.Handle, handle.Handle,
                                    Program.CurrentProcess, out thread_handle,
                                    (STANDARD_RIGHTS)Program.MinThreadQueryRights, 0, 0) != 0)
                                    throw new Exception("Could not duplicate thread handle!");

                                try
                                {
                                    if ((threadId = GetThreadId(thread_handle)) == 0)
                                        throw new Exception(GetLastErrorMessage());

                                    if ((processId = GetProcessIdOfThread(thread_handle)) == 0)
                                        throw new Exception(GetLastErrorMessage());

                                    if (Program.HackerWindow.ProcessProvider.Dictionary.ContainsKey(processId))
                                        info.BestName = Program.HackerWindow.ProcessProvider.Dictionary[processId].Name +
                                            " (" + processId.ToString() + "): " + threadId.ToString();
                                    else
                                        info.BestName = "Non-existent process (" + processId.ToString() + "): " +
                                            threadId.ToString();
                                }
                                finally
                                {
                                    CloseHandle(thread_handle);
                                }
                            }

                            break;

                        case "Token":
                            {
                                int token_handle = 0;

                                if (ZwDuplicateObject(process.Handle, handle.Handle,
                                    Program.CurrentProcess, out token_handle,
                                    (STANDARD_RIGHTS)TOKEN_RIGHTS.TOKEN_QUERY, 0, 0) != 0)
                                    throw new Exception("Could not duplicate token handle!");

                                try
                                {
                                    info.BestName = TokenHandle.FromHandle(token_handle).GetUsername(true);
                                }
                                finally
                                {
                                    CloseHandle(token_handle);
                                }
                            }

                            break;

                        default:
                            if (info.OrigName != null &&
                                info.OrigName != "")
                            {
                                info.BestName = info.OrigName;
                            }
                            else
                            {
                                info.BestName = null;
                            }

                            break;
                    }
                }
                catch
                {
                    if (info.OrigName != null &&
                                info.OrigName != "")
                    {
                        info.BestName = info.OrigName;
                    }
                    else
                    {
                        info.BestName = null;
                    }
                }

                return info;
            }
            finally
            {
                CloseHandle(object_handle);
            }

            throw new Exception("Failed");
        }

        #endregion

        #region Misc.

        public static string ReadUnicodeString(UNICODE_STRING str)
        {
            if (str.Length == 0)
                return null;

            byte[] buf = new byte[str.Length];
            int bytesRead = 0;

            ReadProcessMemory(GetCurrentProcess(), str.Buffer, buf, str.Length, out bytesRead);

            return UnicodeEncoding.Unicode.GetString(buf);
        }

        #endregion

        #region Processes

        public struct SystemProcess
        {
            public string Name;
            public SYSTEM_PROCESS_INFORMATION Process;
            public SYSTEM_THREAD_INFORMATION[] Threads;
        }

        public static SystemProcess[] EnumProcesses()
        {
            int retLength = 0;
            List<SystemProcess> returnProcesses;

            using (MemoryAlloc data = new MemoryAlloc(0x1000))
            {
                while (ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemProcessesAndThreadsInformation, data.Memory,
                    data.Size, ref retLength) == STATUS_INFO_LENGTH_MISMATCH)
                    data.Resize(data.Size * 2);

                returnProcesses = new List<SystemProcess>();

                int i = 0;
                SystemProcess currentProcess = new SystemProcess();

                while (true)
                {
                    currentProcess.Process = data.ReadStruct<SYSTEM_PROCESS_INFORMATION>(i, 0);
                    currentProcess.Threads = new SYSTEM_THREAD_INFORMATION[currentProcess.Process.NumberOfThreads];
                    currentProcess.Name = ReadUnicodeString(currentProcess.Process.ImageName);

                    for (int j = 0; j < currentProcess.Process.NumberOfThreads; j++)
                    {
                        currentProcess.Threads[j] = data.ReadStruct<SYSTEM_THREAD_INFORMATION>(i +
                            Marshal.SizeOf(typeof(SYSTEM_PROCESS_INFORMATION)), j);
                    }

                    returnProcesses.Add(currentProcess);

                    if (currentProcess.Process.NextEntryOffset == 0)
                        break;

                    i += currentProcess.Process.NextEntryOffset;
                }

                return returnProcesses.ToArray();
            }
        }

        public static string GetNameFromPID(int pid)
        {
            PROCESSENTRY32 proc = new PROCESSENTRY32();
            int snapshot = 0;

            snapshot = CreateToolhelp32Snapshot(SnapshotFlags.Process, pid);

            if (snapshot == 0)
                return "(error)";

            proc.dwSize = Marshal.SizeOf(typeof(PROCESSENTRY32));

            Process32First(snapshot, ref proc);

            do
            {
                if (proc.th32ProcessID == pid)
                    return proc.szExeFile;
            } while (Process32Next(snapshot, ref proc) != 0);

            return "(unknown)";
        }

        public static Icon GetProcessIcon(Process p)
        {
            Win32.SHFILEINFO shinfo = new Win32.SHFILEINFO();

            try
            {
                if (Win32.SHGetFileInfo(Misc.GetRealPath(p.MainModule.FileName), 0, ref shinfo,
                      (uint)Marshal.SizeOf(shinfo),
                       Win32.SHGFI_ICON |
                       Win32.SHGFI_SMALLICON) == 0)
                {
                    return null;
                }
                else
                {
                    return Icon.FromHandle(shinfo.hIcon);
                }
            }
            catch
            {
                return null;
            }
        }

        public static int GetProcessSessionId(int ProcessId)
        {
            int sessionId = -1;

            try
            {
                if (!ProcessIdToSessionId(ProcessId, out sessionId))
                    throw new Exception(GetLastErrorMessage());
            }
            catch
            {
                using (ProcessHandle phandle = new ProcessHandle(ProcessId, PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION))
                {
                    return phandle.GetToken(TOKEN_RIGHTS.TOKEN_QUERY).GetSessionId();
                }
            }

            return sessionId;
        }

        #endregion

        #region Security

        public static string GetAccountName(int SID, bool IncludeDomain)
        {
            StringBuilder name = new StringBuilder(255);
            StringBuilder domain = new StringBuilder(255);
            int namelen = 255;
            int domainlen = 255;
            SID_NAME_USE use = SID_NAME_USE.SidTypeUser;

            try
            {
                if (!LookupAccountSid(null, SID, name, out namelen, domain, out domainlen, out use))
                {
                    name.EnsureCapacity(namelen);
                    domain.EnsureCapacity(domainlen);

                    if (!LookupAccountSid(null, SID, name, out namelen, domain, out domainlen, out use))
                    {
                        if (name.ToString() == "" && domain.ToString() == "")
                            throw new Exception("Could not lookup account SID: " + Win32.GetLastErrorMessage());
                    }
                }
            }
            catch
            {
                return new System.Security.Principal.SecurityIdentifier(
                                new IntPtr(SID)).ToString();
            }

            if (IncludeDomain)
            {
                return ((domain.ToString() != "") ? domain.ToString() + "\\" : "") + name.ToString();
            }
            else
            {
                return name.ToString();
            }
        }

        public static SID_NAME_USE GetAccountType(int SID)
        {
            StringBuilder name = new StringBuilder(255);
            StringBuilder domain = new StringBuilder(255);
            int namelen = 255;
            int domainlen = 255;
            SID_NAME_USE use = SID_NAME_USE.SidTypeUser;

            if (!LookupAccountSid(null, SID, name, out namelen, domain, out domainlen, out use))
            {
                name.EnsureCapacity(namelen);
                domain.EnsureCapacity(domainlen);

                if (!LookupAccountSid(null, SID, name, out namelen, domain, out domainlen, out use))
                {
                    if (name.ToString() == "" && domain.ToString() == "")
                        throw new Exception("Could not lookup account SID: " + Win32.GetLastErrorMessage());
                }
            }

            return use;
        }

        public static string GetPrivilegeDisplayName(string PrivilegeName)
        {
            StringBuilder sb = null;
            int size = 0;
            int languageId = 0;

            LookupPrivilegeDisplayName(0, PrivilegeName, sb, out size, out languageId);
            sb = new StringBuilder(size);
            LookupPrivilegeDisplayName(0, PrivilegeName, sb, out size, out languageId);

            return sb.ToString();
        }

        public static string GetPrivilegeName(LUID Luid)
        {
            StringBuilder sb = null;
            int size = 0;

            LookupPrivilegeName(0, ref Luid, sb, out size);
            sb = new StringBuilder(size);
            LookupPrivilegeName(0, ref Luid, sb, out size);

            return sb.ToString();
        }

        public static TOKEN_GROUPS ReadTokenGroups(TokenHandle TokenHandle, bool IncludeDomains)
        {
            int retLen = 0;

            GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenGroups, data,
                    data.Size, out retLen))
                    throw new Exception(GetLastErrorMessage());

                uint number = data.ReadUInt32(0);
                TOKEN_GROUPS groups = new TOKEN_GROUPS();

                groups.GroupCount = number;
                groups.Groups = new SID_AND_ATTRIBUTES[number];
                groups.Names = new string[number];

                for (int i = 0; i < number; i++)
                {
                    groups.Groups[i] = data.ReadStruct<SID_AND_ATTRIBUTES>(4, i);

                    try
                    {
                        groups.Names[i] = GetAccountName(groups.Groups[i].SID, IncludeDomains);
                    }
                    catch
                    { }
                }

                return groups;
            }
        }

        public static TOKEN_PRIVILEGES ReadTokenPrivileges(TokenHandle TokenHandle)
        {
            int retLen = 0;

            GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenPrivileges, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if (!GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenPrivileges, data.Memory,
                    data.Size, out retLen))
                    throw new Exception(GetLastErrorMessage());

                uint number = data.ReadUInt32(0);
                TOKEN_PRIVILEGES privileges = new TOKEN_PRIVILEGES();

                privileges.PrivilegeCount = number;
                privileges.Privileges = new LUID_AND_ATTRIBUTES[number];

                for (int i = 0; i < number; i++)
                {
                    privileges.Privileges[i] = data.ReadStruct<LUID_AND_ATTRIBUTES>(4, i);
                }

                return privileges;
            }
        }

        public static void WriteTokenPrivilege(string PrivilegeName, SE_PRIVILEGE_ATTRIBUTES Attributes)
        {
            WriteTokenPrivilege(
                ProcessHandle.FromHandle(Program.CurrentProcess).GetToken(), PrivilegeName, Attributes);
        }

        public static void WriteTokenPrivilege(TokenHandle TokenHandle, string PrivilegeName, SE_PRIVILEGE_ATTRIBUTES Attributes)
        {
            TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();

            tkp.Privileges = new LUID_AND_ATTRIBUTES[1];

            if (!LookupPrivilegeValue(null, PrivilegeName, ref tkp.Privileges[0].Luid))
                throw new Exception("Invalid privilege name '" + PrivilegeName + "'.");

            tkp.PrivilegeCount = 1;
            tkp.Privileges[0].Attributes = Attributes;

            AdjustTokenPrivileges(TokenHandle.Handle, 0, ref tkp, 0, 0, 0);

            if (Marshal.GetLastWin32Error() != 0)
                throw new Exception(GetLastErrorMessage());
        }

        #endregion

        #region Services

        public static Dictionary<string, ENUM_SERVICE_STATUS_PROCESS> EnumServices()
        {
            using (ServiceManagerHandle manager =
                new ServiceManagerHandle(SC_MANAGER_RIGHTS.SC_MANAGER_ENUMERATE_SERVICE))
            {
                int requiredSize;
                int servicesReturned;
                int resume;

                // get required size
                EnumServicesStatusEx(manager, 0, SERVICE_QUERY_TYPE.Win32 | SERVICE_QUERY_TYPE.Driver,
                    SERVICE_QUERY_STATE.All, IntPtr.Zero, 0, out requiredSize, out servicesReturned,
                    out resume, 0);

                using (MemoryAlloc data = new MemoryAlloc(requiredSize))
                {
                    Dictionary<string, ENUM_SERVICE_STATUS_PROCESS> dictionary =
                        new Dictionary<string, ENUM_SERVICE_STATUS_PROCESS>();

                    if (!EnumServicesStatusEx(manager, 0, SERVICE_QUERY_TYPE.Win32 | SERVICE_QUERY_TYPE.Driver,
                        SERVICE_QUERY_STATE.All, data,
                        data.Size, out requiredSize, out servicesReturned,
                        out resume, 0))
                    {
                        throw new Exception(GetLastErrorMessage());
                    }

                    for (int i = 0; i < servicesReturned; i++)
                    {
                        ENUM_SERVICE_STATUS_PROCESS service = data.ReadStruct<ENUM_SERVICE_STATUS_PROCESS>(i);

                        dictionary.Add(service.ServiceName, service);
                    }

                    return dictionary;
                }
            }
        }

        public static QUERY_SERVICE_CONFIG GetServiceConfig(string ServiceName)
        {
            using (ServiceHandle service = new ServiceHandle(ServiceName, SERVICE_RIGHTS.SERVICE_QUERY_CONFIG))
            {
                int requiredSize = 0;

                QueryServiceConfig(service, IntPtr.Zero, 0, ref requiredSize);

                using (MemoryAlloc data = new MemoryAlloc(requiredSize))
                {
                    if (!QueryServiceConfig(service, data, data.Size, ref requiredSize))
                        throw new Exception("Could not get service configuration: " + GetLastErrorMessage());

                    return data.ReadStruct<QUERY_SERVICE_CONFIG>();
                }
            }
        }

        #endregion

        #region Statistics

        public static IO_COUNTERS GetProcessIoCounters(ProcessHandle process)
        {
            IO_COUNTERS counters = new IO_COUNTERS();

            if (!GetProcessIoCounters(process.Handle, out counters))
                throw new Exception(GetLastErrorMessage());

            return counters;
        }

        public static ulong[] GetProcessTimes(ProcessHandle process)
        {
            ulong[] times = new ulong[4];

            if (!GetProcessTimes(process.Handle, out times[0], out times[1], out times[2], out times[3]))
                throw new Exception(GetLastErrorMessage());

            return times;
        }

        public static ulong[] GetSystemTimes()
        {
            ulong[] times = new ulong[3];

            if (!GetSystemTimes(out times[0], out times[1], out times[2]))
                throw new Exception(GetLastErrorMessage());

            return times; 
        }

        #endregion

        #region Terminal Server

        public static WTS_SESSION_INFO[] TSEnumSessions()
        {
            IntPtr sessions;
            int count;
            WTS_SESSION_INFO[] returnSessions;

            WTSEnumerateSessions(0, 0, 1, out sessions, out count);
            returnSessions = new WTS_SESSION_INFO[count];

            WtsMemoryAlloc data = WtsMemoryAlloc.FromPointer(sessions);

            for (int i = 0; i < count; i++)
            {
                returnSessions[i] = data.ReadStruct<WTS_SESSION_INFO>(i);
            }

            data.Dispose();

            return returnSessions;
        }

        public struct WtsEnumProcessesData
        {
            public WTS_PROCESS_INFO[] Processes;
            public WtsMemoryAlloc Memory;
        }

        public static WtsEnumProcessesData TSEnumProcesses()
        {
            IntPtr processes;
            int count;
            WTS_PROCESS_INFO[] returnProcesses;

            WTSEnumerateProcesses(0, 0, 1, out processes, out count);
            returnProcesses = new WTS_PROCESS_INFO[count];

            WtsMemoryAlloc data = WtsMemoryAlloc.FromPointer(processes);

            for (int i = 0; i < count; i++)
            {
                returnProcesses[i] = data.ReadStruct<WTS_PROCESS_INFO>(i);
            }

            return new WtsEnumProcessesData() { Processes = returnProcesses, Memory = data };
        }

        #endregion
    }
}
