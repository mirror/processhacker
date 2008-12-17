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
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace ProcessHacker
{
    public partial class Win32
    {
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

        public class Win32Handle : IDisposable
        {
            private bool _owned = true;
            private bool _closed = false;
            private int _handle;

            public Win32Handle()
            { }

            public Win32Handle(int Handle)
            {
                _handle = Handle;
            }

            public Win32Handle(int Handle, bool Owned)
            {
                _handle = Handle;
                _owned = Owned;
            }

            public int Handle
            {
                get { return _handle; }
                protected set { _handle = value; }
            }

            protected virtual void Close()
            {
                CloseHandle(_handle);
            }

            ~Win32Handle()
            {
                this.Dispose();
            }

            public void Dispose()
            {
                if (!_closed && _owned)
                {
                    _closed = true;
                    Close();
                }
            }
        }

        public interface IWithToken
        {
            TokenHandle GetToken();
            TokenHandle GetToken(TOKEN_RIGHTS access);
        }

        public class ProcessHandle : Win32Handle, IWithToken
        {
            public static ProcessHandle FromHandle(int Handle)
            {
                return new ProcessHandle(Handle, false);
            }

            private ProcessHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            public ProcessHandle(int PID)
                : this(PID, PROCESS_RIGHTS.PROCESS_ALL_ACCESS)
            { }

            public ProcessHandle(int PID, PROCESS_RIGHTS access)
            {
                this.Handle = OpenProcess(access, 0, PID);

                if (this.Handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public int Wait(int Timeout)
            {
                return WaitForSingleObject(this.Handle, Timeout);
            }

            public void Terminate()
            {
                this.Terminate(0);
            }

            public void Terminate(int ExitCode)
            {
                if (TerminateProcess(this.Handle, ExitCode) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public TokenHandle GetToken()
            {
                return GetToken(TOKEN_RIGHTS.TOKEN_ALL_ACCESS);
            }

            public TokenHandle GetToken(TOKEN_RIGHTS access)
            {
                return new TokenHandle(this, access);
            }
        }

        public class ServiceHandle : Win32Handle
        {          
            public static ServiceHandle FromHandle(int Handle)
            {
                return new ServiceHandle(Handle, false);
            }

            private ServiceHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

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

            public void Control(SERVICE_CONTROL control)
            {
                SERVICE_STATUS status = new SERVICE_STATUS();

                if (ControlService(this.Handle, control, ref status) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Start()
            {
                if (StartService(this.Handle, 0, 0) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

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

        public class ThreadHandle : Win32Handle, IWithToken
        {        
            public static ThreadHandle FromHandle(int Handle)
            {
                return new ThreadHandle(Handle, false);
            }

            private ThreadHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            public ThreadHandle(int TID, THREAD_RIGHTS access)
            {
                this.Handle = OpenThread(access, 0, TID);

                if (this.Handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public int Wait(int Timeout)
            {
                return WaitForSingleObject(this.Handle, Timeout);
            }

            public void Suspend()
            {
                if (SuspendThread(this.Handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Resume()
            {
                if (ResumeThread(this.Handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Terminate()
            {
                this.Terminate(0);
            }

            public void Terminate(int ExitCode)
            {
                if (TerminateThread(this.Handle, ExitCode) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public TokenHandle GetToken()
            {
                return GetToken(TOKEN_RIGHTS.TOKEN_ALL_ACCESS);
            }

            public TokenHandle GetToken(TOKEN_RIGHTS access)
            {
                return new TokenHandle(this, access);
            }
        }

        public class TokenHandle : Win32Handle
        {         
            public static TokenHandle FromHandle(int Handle)
            {
                return new TokenHandle(Handle, false);
            }

            private TokenHandle(int Handle, bool Owned)
                : base(Handle, Owned)
            { }

            public TokenHandle(ProcessHandle handle, TOKEN_RIGHTS access)
            {
                int h;

                if (OpenProcessToken(handle.Handle, access, out h) == 0)
                    throw new Exception(GetLastErrorMessage());

                this.Handle = h;
            }

            public TokenHandle(ThreadHandle handle, TOKEN_RIGHTS access)
            {
                int h;

                if (OpenThreadToken(handle.Handle, access, false, out h) == 0)
                    throw new Exception(GetLastErrorMessage());

                this.Handle = h;
            }

            public string GetUsername(bool IncludeDomain)
            {
                int retLen = 0;

                GetTokenInformation(this.Handle, TOKEN_INFORMATION_CLASS.TokenUser, 0, 0, ref retLen);

                IntPtr data = Marshal.AllocHGlobal(retLen);

                try
                {
                    if (GetTokenInformation(this.Handle, TOKEN_INFORMATION_CLASS.TokenUser, data,
                        retLen, ref retLen) == 0)
                    {
                        throw new Exception(Win32.GetLastErrorMessage());
                    }

                    TOKEN_USER user = PtrToStructure<TOKEN_USER>(data);

                    return GetAccountName(user.User.SID, IncludeDomain);
                }
                finally
                {
                    Marshal.FreeHGlobal(data);
                }
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

        public static SYSTEM_HANDLE_INFORMATION[] EnumHandles()
        {
            int length = 0x1000;
            int retLength = 0;
            int handles = 0;
            IntPtr data = Marshal.AllocHGlobal(length);
            SYSTEM_HANDLE_INFORMATION[] returnHandles;

            while (ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, data.ToInt32(),
                length, ref retLength) == STATUS_INFO_LENGTH_MISMATCH)
            {
                length *= 2;
                Marshal.FreeHGlobal(data);
                data = Marshal.AllocHGlobal(length);
            }

            handles = Marshal.ReadInt32(data);
            returnHandles = new SYSTEM_HANDLE_INFORMATION[handles];

            for (int i = 0; i < handles; i++) 
            {
                returnHandles[i] = PtrToStructure<SYSTEM_HANDLE_INFORMATION>(
                    new IntPtr(4 + data.ToInt32() + i * Marshal.SizeOf(typeof(SYSTEM_HANDLE_INFORMATION))));
            }

            Marshal.FreeHGlobal(data);

            return returnHandles;
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
            int object_handle = 0;
            int retLength = 0;

            if (ZwDuplicateObject(process.Handle, handle.Handle,
                Program.CurrentProcess, ref object_handle, 0, 0, 0) != 0)
                throw new Exception("Could not duplicate object!");

            try
            {
                ObjectInformation info = new ObjectInformation();

                ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectBasicInformation,
                    0, 0, ref retLength);

                if (retLength > 0)
                {
                    IntPtr obiMem = Marshal.AllocHGlobal(retLength);
                    ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectBasicInformation,
                        obiMem, retLength, ref retLength);
                    OBJECT_BASIC_INFORMATION obi = PtrToStructure<OBJECT_BASIC_INFORMATION>(obiMem);
                    Marshal.FreeHGlobal(obiMem);
                    info.Basic = obi;
                }

                if (ObjectTypes.ContainsKey(handle.ObjectTypeNumber))
                {
                    info.TypeName = ObjectTypes[handle.ObjectTypeNumber];
                }
                else
                {
                    ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation,
                        0, 0, ref retLength);

                    if (retLength > 0)
                    {
                        IntPtr otiMem = Marshal.AllocHGlobal(retLength);

                        try
                        {
                            if (ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation,
                                otiMem, retLength, ref retLength) != 0)
                                throw new Exception("ZwQueryObject failed");
                            OBJECT_TYPE_INFORMATION oti = PtrToStructure<OBJECT_TYPE_INFORMATION>(otiMem);
                            info.TypeName = ReadUnicodeString(oti.Name);
                            ObjectTypes.Add(handle.ObjectTypeNumber, info.TypeName);
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(otiMem);
                        }
                    }
                }

                if (info.TypeName == "File")
                    if ((int)handle.GrantedAccess == 0x0012019f)
                        throw new Exception("0x0012019f access is banned");

                ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation,
                  0, 0, ref retLength);
                
                if (retLength > 0)
                {
                    IntPtr oniMem = Marshal.AllocHGlobal(retLength);

                    try
                    {
                        if (ZwQueryObject(object_handle, OBJECT_INFORMATION_CLASS.ObjectNameInformation,
                            oniMem, retLength, ref retLength) != 0)
                            throw new Exception("ZwQueryObject failed");
                        OBJECT_NAME_INFORMATION oni = PtrToStructure<OBJECT_NAME_INFORMATION>(oniMem);

                        info.OrigName = ReadUnicodeString(oni.Name);
                        info.Name = oni;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(oniMem);
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
                                    Program.CurrentProcess, ref process_handle,
                                    (STANDARD_RIGHTS)PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0, 0) != 0)
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
                                    Program.CurrentProcess, ref thread_handle,
                                    (STANDARD_RIGHTS)THREAD_RIGHTS.THREAD_QUERY_INFORMATION, 0, 0) != 0)
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
                                    Program.CurrentProcess, ref token_handle,
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

        public static T PtrToStructure<T>(IntPtr data)
        {
            return (T)Marshal.PtrToStructure(data, typeof(T));
        }

        public static string ReadUnicodeString(UNICODE_STRING str)
        {
            if (str.Length == 0)
                return null;

            byte[] buf = new byte[str.Length];
            int bytesRead = 0;

            ReadProcessMemory(GetCurrentProcess(), str.Buffer, buf, str.Length, ref bytesRead);

            return UnicodeEncoding.Unicode.GetString(buf);
        }

        #endregion

        #region Processes

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

        public static string GetProcessCmdLine(ProcessHandle process)
        {
            return GetProcessPEBString(process, 66);
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

        public static string GetProcessImageFileName(ProcessHandle process)
        {
            return GetProcessPEBString(process, 58);
        }

        public static int GetProcessParent(int pid)
        {
            PROCESSENTRY32 proc = new PROCESSENTRY32();
            int snapshot = 0;

            snapshot = CreateToolhelp32Snapshot(SnapshotFlags.Process, pid);

            if (snapshot == 0)
                return -1;

            proc.dwSize = Marshal.SizeOf(typeof(PROCESSENTRY32));

            Process32First(snapshot, ref proc);

            do
            {
                if (proc.th32ProcessID == pid)
                    return proc.th32ParentProcessID;
            } while (Process32Next(snapshot, ref proc) != 0);

            return -1;
        }

        public static string GetProcessPEBString(ProcessHandle process, int offset)
        {
            PROCESS_BASIC_INFORMATION basicInfo = new PROCESS_BASIC_INFORMATION();
            int retLen = 0;
            int pebBaseAddress = 0x7ffd7000;

            if (ZwQueryInformationProcess(process.Handle, PROCESSINFOCLASS.ProcessBasicInformation,
                ref basicInfo, Marshal.SizeOf(basicInfo), ref retLen) != 0)
                pebBaseAddress = basicInfo.PebBaseAddress;

            byte[] data2 = new byte[4];

            // read address of parameter information block
            if (ReadProcessMemory(process.Handle, basicInfo.PebBaseAddress + 16, data2, 4, ref retLen) == 0)
                throw new Exception(GetLastErrorMessage());

            int paramInfoAddrI = Misc.BytesToInt(data2, Misc.Endianness.Little);

            // read length of string
            if (ReadProcessMemory(process.Handle, paramInfoAddrI + offset, data2, 2, ref retLen) == 0)
                throw new Exception(GetLastErrorMessage());

            ushort strLength = Misc.BytesToUShort(data2, Misc.Endianness.Little);
            byte[] stringData = new byte[strLength];

            // read address of string
            if (ReadProcessMemory(process.Handle, paramInfoAddrI + offset + 2, data2, 4, ref retLen) == 0)
                throw new Exception(GetLastErrorMessage());

            int strAddr = Misc.BytesToInt(data2, Misc.Endianness.Little);

            // read string
            if (ReadProcessMemory(process.Handle, strAddr, stringData, strLength, ref retLen) == 0)
                throw new Exception(GetLastErrorMessage());

            // return decoded unicode string
            return UnicodeEncoding.Unicode.GetString(stringData).TrimEnd('\0');
        }

        public static int GetProcessSessionId(int ProcessId)
        {
            int sessionId = -1;

            try
            {
                if (ProcessIdToSessionId(ProcessId, ref sessionId) == 0)
                    throw new Exception(GetLastErrorMessage());
            }
            catch
            {
                int handle = 0;
                int token = 0;
                int retLen = 0;

                try
                {
                    if ((handle = OpenProcess(PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0, ProcessId)) == 0)
                        throw new Exception(GetLastErrorMessage());

                    if (OpenProcessToken(handle, TOKEN_RIGHTS.TOKEN_QUERY,
                        out token) == 0)
                        throw new Exception(GetLastErrorMessage());

                    if (GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenSessionId,
                        ref sessionId, 4, ref retLen) == 0)
                    {
                        throw new Exception(GetLastErrorMessage());
                    }
                }
                finally
                {
                    CloseHandle(token);
                    CloseHandle(handle);
                }

                return sessionId;
            }

            return sessionId;
        }

        public static string GetProcessUsername(int handle, bool IncludeDomain)
        {
            using (TokenHandle token = new TokenHandle(ProcessHandle.FromHandle(handle), TOKEN_RIGHTS.TOKEN_QUERY))
                return token.GetUsername(IncludeDomain);
        }

        public static bool IsBeingDebugged(int ProcessHandle)
        {
            int debugged = 0;

            if (Win32.CheckRemoteDebuggerPresent(ProcessHandle, ref debugged) == 0)
                throw new Exception(GetLastErrorMessage());

            return debugged == 1;
        }

        #endregion

        #region Security

        public static string GetAccountName(WTS_PROCESS_INFO info, bool IncludeDomain)
        {
            return GetAccountName(info.SID, IncludeDomain);
        }

        public static string GetAccountName(int SID, bool IncludeDomain)
        {
            StringBuilder name = new StringBuilder(255);
            StringBuilder domain = new StringBuilder(255);
            int namelen = 255;
            int domainlen = 255;
            SID_NAME_USE use = SID_NAME_USE.SidTypeUser;

            if (LookupAccountSid(0, SID, name, ref namelen, domain, ref domainlen, ref use) == 0)
            {
                name.EnsureCapacity(namelen);
                domain.EnsureCapacity(domainlen);

                if (LookupAccountSid(0, SID, name, ref namelen, domain, ref domainlen, ref use) == 0)
                {
                    if (name.ToString() == "" && domain.ToString() == "")
                        throw new Exception("Could not lookup account SID: " + Win32.GetLastErrorMessage());
                }
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

            if (LookupAccountSid(0, SID, name, ref namelen, domain, ref domainlen, ref use) == 0)
            {
                name.EnsureCapacity(namelen);
                domain.EnsureCapacity(domainlen);

                if (LookupAccountSid(0, SID, name, ref namelen, domain, ref domainlen, ref use) == 0)
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

            LookupPrivilegeDisplayName(0, PrivilegeName, sb, ref size, ref languageId);
            sb = new StringBuilder(size);
            LookupPrivilegeDisplayName(0, PrivilegeName, sb, ref size, ref languageId);

            return sb.ToString();
        }

        public static string GetPrivilegeName(LUID Luid)
        {
            StringBuilder sb = null;
            int size = 0;
                         
            LookupPrivilegeName(0, ref Luid, sb, ref size);
            sb = new StringBuilder(size);
            LookupPrivilegeName(0, ref Luid, sb, ref size);

            return sb.ToString();
        }

        public static int OpenLocalPolicy(POLICY_RIGHTS DesiredAccess)
        {
            LSA_OBJECT_ATTRIBUTES attributes = new LSA_OBJECT_ATTRIBUTES();
            int handle = 0;

            if (LsaOpenPolicy(0, ref attributes, DesiredAccess, ref handle) != 0)
                return 0;

            return handle;
        }

        public static TOKEN_GROUPS ReadTokenGroups(TokenHandle TokenHandle, bool IncludeDomains)
        {
            int retLen = 0;

            GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenGroups, 0, 0, ref retLen);

            IntPtr data = Marshal.AllocHGlobal(retLen);

            if (GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenGroups, data,
                retLen, ref retLen) == 0)
                throw new Exception(GetLastErrorMessage());

            uint number = (uint)Marshal.ReadInt32(data);
            TOKEN_GROUPS groups = new TOKEN_GROUPS();

            groups.GroupCount = number;
            groups.Groups = new SID_AND_ATTRIBUTES[number];
            groups.Names = new string[number];

            for (int i = 0; i < number; i++)
            {
                groups.Groups[i] = PtrToStructure<SID_AND_ATTRIBUTES>(
                    new IntPtr(data.ToInt32() + 4 + i * Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES))));

                try
                {
                    groups.Names[i] = GetAccountName(groups.Groups[i].SID, IncludeDomains);
                }
                catch
                { }
            }

            return groups;
        }

        public static TOKEN_PRIVILEGES ReadTokenPrivileges(TokenHandle TokenHandle)
        {
            int retLen = 0;

            GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenPrivileges, 0, 0, ref retLen);

            IntPtr data = Marshal.AllocHGlobal(retLen);

            if (GetTokenInformation(TokenHandle.Handle, TOKEN_INFORMATION_CLASS.TokenPrivileges, data,
                retLen, ref retLen) == 0)
                throw new Exception(GetLastErrorMessage());

            uint number = (uint)Marshal.ReadInt32(data);
            TOKEN_PRIVILEGES privileges = new TOKEN_PRIVILEGES();

            privileges.PrivilegeCount = number;
            privileges.Privileges = new LUID_AND_ATTRIBUTES[number];

            for (int i = 0; i < number; i++)
            {
                privileges.Privileges[i] = PtrToStructure<LUID_AND_ATTRIBUTES>(
                    new IntPtr(data.ToInt32() + 4 + i * Marshal.SizeOf(typeof(LUID_AND_ATTRIBUTES))));
            }

            return privileges;
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

            if (LookupPrivilegeValue(null, PrivilegeName, ref tkp.Privileges[0].Luid) == 0)
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
            int manager = OpenSCManager(0, 0, SC_MANAGER_RIGHTS.SC_MANAGER_ENUMERATE_SERVICE);

            if (manager == 0)
                throw new Exception("Could not open service control manager: "
                    + GetLastErrorMessage() + ".");

            int requiredSize = 0;
            int servicesReturned = 0;
            int resume = 0;

            // get required size
            EnumServicesStatusEx(manager, 0, SERVICE_QUERY_TYPE.Win32 | SERVICE_QUERY_TYPE.Driver,
                SERVICE_QUERY_STATE.All, ref servicesReturned // hack
                , 0, ref requiredSize, ref servicesReturned,
                ref resume, 0);

            IntPtr data = Marshal.AllocHGlobal(requiredSize);
            Dictionary<string, ENUM_SERVICE_STATUS_PROCESS> dictionary =
                new Dictionary<string, ENUM_SERVICE_STATUS_PROCESS>();

            try
            {
                if (EnumServicesStatusEx(manager, 0, SERVICE_QUERY_TYPE.Win32 | SERVICE_QUERY_TYPE.Driver,
                    SERVICE_QUERY_STATE.All, data, 
                    requiredSize, ref requiredSize, ref servicesReturned,
                    ref resume, 0) == 0)
                {
                    throw new Exception(GetLastErrorMessage());
                }

                for (int i = 0; i < servicesReturned; i++)
                {
                    ENUM_SERVICE_STATUS_PROCESS service = PtrToStructure<ENUM_SERVICE_STATUS_PROCESS>(
                        new IntPtr(data.ToInt32() + Marshal.SizeOf(typeof(ENUM_SERVICE_STATUS_PROCESS)) * i));

                    dictionary.Add(service.ServiceName, service);
                }
            }
            finally
            {
                CloseServiceHandle(manager);
                Marshal.FreeHGlobal(data);
            }

            return dictionary;
        }

        public static QUERY_SERVICE_CONFIG GetServiceConfig(string ServiceName)
        {            
            int manager = OpenSCManager(0, 0, SC_MANAGER_RIGHTS.SC_MANAGER_CONNECT);

            if (manager == 0)
                throw new Exception("Could not open service control manager: "
                    + GetLastErrorMessage() + ".");

            int handle = OpenService(manager, ServiceName, SERVICE_RIGHTS.SERVICE_QUERY_CONFIG);

            if (handle == 0)
            {
                CloseServiceHandle(manager);

                throw new Exception("Could not open service handle: "
                    + GetLastErrorMessage() + ".");
            }
                                      
            int requiredSize = 0;

            QueryServiceConfig(handle, 0, 0, ref requiredSize);

            IntPtr data = Marshal.AllocHGlobal(requiredSize);
            QUERY_SERVICE_CONFIG config;

            try
            {
                if (QueryServiceConfig(handle, data, requiredSize, ref requiredSize) == 0)
                {
                    throw new Exception("Could not get service configuration: " + GetLastErrorMessage());
                }

                config = PtrToStructure<QUERY_SERVICE_CONFIG>(data);
            }
            finally
            {
                CloseServiceHandle(handle);
                CloseServiceHandle(manager);
                Marshal.FreeHGlobal(data);
            }

            return config;
        }

        #endregion

        #region Statistics

        public static IO_COUNTERS GetProcessIoCounters(ProcessHandle process)
        {
            IO_COUNTERS counters = new IO_COUNTERS();

            if (GetProcessIoCounters(process.Handle, ref counters) == 0)
                throw new Exception(GetLastErrorMessage());

            return counters;
        }

        public static ulong[] GetProcessTimes(ProcessHandle process)
        {
            ulong[] times = new ulong[4];

            if (GetProcessTimes(process.Handle, ref times[0], ref times[1], ref times[2], ref times[3]) == 0)
                throw new Exception(GetLastErrorMessage());

            return times;
        }

        public static ulong[] GetSystemTimes()
        {
            ulong[] times = new ulong[3];

            if (GetSystemTimes(ref times[0], ref times[1], ref times[2]) == 0)
                throw new Exception(GetLastErrorMessage());

            return times; 
        }

        #endregion

        #region Terminal Server

        public struct WtsProcess
        {
            public WTS_PROCESS_INFO Info;
            public string Username;
            public string UsernameWithDomain;
        }

        public static WTS_SESSION_INFO[] TSEnumSessions()
        {
            int sessions = 0;
            int count = 0;
            WTS_SESSION_INFO[] returnSessions;

            WTSEnumerateSessions(0, 0, 1, ref sessions, ref count);
            returnSessions = new WTS_SESSION_INFO[count];

            for (int i = 0; i < count; i++)
            {
                returnSessions[i] = PtrToStructure<WTS_SESSION_INFO>(
                    new IntPtr(sessions + Marshal.SizeOf(typeof(WTS_SESSION_INFO)) * i));
            }

            WTSFreeMemory(sessions);

            return returnSessions;
        }

        public static WtsProcess[] TSEnumProcesses()
        {
            int processes = 0;
            int count = 0;
            WtsProcess[] returnProcesses;

            WTSEnumerateProcesses(0, 0, 1, ref processes, ref count);
            returnProcesses = new WtsProcess[count];

            for (int i = 0; i < count; i++)
            {
                returnProcesses[i].Info = PtrToStructure<WTS_PROCESS_INFO>(
                    new IntPtr(processes + Marshal.SizeOf(typeof(WTS_PROCESS_INFO)) * i));

                try
                {
                    if (returnProcesses[i].Info.SID == 0)
                        throw new Exception("Null SID pointer");

                    returnProcesses[i].Username = GetAccountName(returnProcesses[i].Info.SID, false);
                }
                catch
                { }

                try
                {
                    if (returnProcesses[i].Info.SID == 0)
                        throw new Exception("Null SID pointer");

                    returnProcesses[i].UsernameWithDomain = GetAccountName(returnProcesses[i].Info.SID, true);
                }
                catch
                { }
            }

            WTSFreeMemory(processes);

            return returnProcesses;
        }

        public static string TSGetProcessUsername(int PID, bool IncludeDomain)
        {
            WtsProcess[] processes = TSEnumProcesses();

            foreach (WtsProcess process in processes)
            {
                if (process.Info.ProcessID == PID)
                {
                    if (IncludeDomain)
                        return process.UsernameWithDomain;
                    else
                        return process.Username;
                }
            }

            throw new Exception("Process does not exist.");
        }

        #endregion
    }
}
