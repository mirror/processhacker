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

// you won't get some of this stuff from anywhere else... :)

namespace ProcessHacker
{
    public class Win32
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

        public class ThreadHandle : IDisposable
        {
            private int _handle;

            public ThreadHandle(int TID, THREAD_RIGHTS access)
            {
                _handle = OpenThread(access, 0, TID);

                if (_handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public int Handle
            {
                get { return _handle; }
            }

            public int Wait(int Timeout)
            {
                return WaitForSingleObject(_handle, Timeout);
            }

            public void Suspend()
            {
                if (SuspendThread(_handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Resume()
            {
                if (ResumeThread(_handle) == -1)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Terminate()
            {
                this.Terminate(0);
            }

            public void Terminate(int ExitCode)
            {
                if (TerminateThread(_handle, ExitCode) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            #region IDisposable Members

            public void Dispose()
            {
                CloseHandle(_handle);
            }

            #endregion
        }

        public class ProcessHandle : IDisposable
        {
            private int _handle;

            public ProcessHandle(int PID, PROCESS_RIGHTS access)
            {
                _handle = OpenProcess(access, 0, PID);

                if (_handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public int Handle
            {
                get { return _handle; }
            }

            public int Wait(int Timeout)
            {
                return WaitForSingleObject(_handle, Timeout);
            }

            public void Terminate()
            {
                this.Terminate(0);
            }

            public void Terminate(int ExitCode)
            {
                if (TerminateProcess(_handle, ExitCode) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            #region IDisposable Members

            public void Dispose()
            {
                CloseHandle(_handle);
            }

            #endregion
        }

        public class ServiceHandle : IDisposable
        {
            private int _handle;

            public ServiceHandle(string ServiceName, SERVICE_RIGHTS access)
            {
                int manager = OpenSCManager(0, 0, SC_MANAGER_RIGHTS.SC_MANAGER_CONNECT);

                if (manager == 0)
                    throw new Exception(GetLastErrorMessage());

                _handle = OpenService(manager, ServiceName, access);

                CloseHandle(manager);

                if (_handle == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Control(SERVICE_CONTROL control)
            {
                SERVICE_STATUS status = new SERVICE_STATUS();

                if (ControlService(_handle, control, ref status) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Start()
            {
                if (StartService(_handle, 0, 0) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public void Delete()
            {
                if (DeleteService(_handle) == 0)
                    throw new Exception(GetLastErrorMessage());
            }

            public int Handle
            {
                get { return _handle; }
            }

            #region IDisposable Members

            public void Dispose()
            {
                CloseHandle(_handle);    
            }

            #endregion
        }

        public delegate int EnumWindowsProc(int hwnd, int param);
        public delegate int SymEnumSymbolsProc(SYMBOL_INFO pSymInfo, int SymbolSize, int UserContext);
        public delegate int FunctionTableAccessProc64(int ProcessHandle, int AddrBase);
        public delegate int GetModuleBaseProc64(int ProcessHandle, int Address);

        public static Dictionary<byte, string> ObjectTypes = new Dictionary<byte, string>();

        #region Imported Consts

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

        #region Imported Enums   

        public enum ADDRESS_MODE : int
        {
            AddrMode1616,
            AddrMode1632,
            AddrModeReal,
            AddrModeFlat
        }

        [Flags]
        public enum CONTEXT_FLAGS : int
        {
            CONTEXT_i386 = 0x00010000,
            CONTEXT_i486 = 0x00010000,
            CONTEXT_CONTROL = CONTEXT_i386 | 0x00000001,
            CONTEXT_INTEGER = CONTEXT_i386 | 0x00000002,
            CONTEXT_SEGMENTS = CONTEXT_i386 | 0x00000004,
            CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x00000008,
            CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x00000010,
            CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x00000020,
            CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
            CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS |
                          CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS |
                          CONTEXT_EXTENDED_REGISTERS
        }
  
        public enum DEPFLAGS : int
        {
            PROCESS_DEP_DISABLE = 0x00000000,
            PROCESS_DEP_ENABLE = 0x00000001,
            PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION = 0x00000002
        }

        [Flags]
        public enum FILE_OBJECT_FLAGS : int
        {
            FO_FILE_OPEN = 0x00000001,
            FO_SYNCHRONOUS_IO = 0x00000002,
            FO_ALERTABLE_IO = 0x00000004,
            FO_NO_INTERMEDIATE_BUFFERING = 0x00000008,
            FO_WRITE_THROUGH = 0x00000010,
            FO_SEQUENTIAL_ONLY = 0x00000020,
            FO_CACHE_SUPPORTED = 0x00000040,
            FO_NAMED_PIPE = 0x00000080,
            FO_STREAM_FILE = 0x00000100,
            FO_MAILSLOT = 0x00000200,
            FO_GENERATE_AUDIT_ON_CLOSE = 0x00000400,
            FO_QUEUE_IRP_TO_THREAD = FO_GENERATE_AUDIT_ON_CLOSE,
            FO_DIRECT_DEVICE_OPEN = 0x00000800,
            FO_FILE_MODIFIED = 0x00001000,
            FO_FILE_SIZE_CHANGED = 0x00002000,
            FO_CLEANUP_COMPLETE = 0x00004000,
            FO_TEMPORARY_FILE = 0x00008000,
            FO_DELETE_ON_CLOSE = 0x00010000,
            FO_OPENED_CASE_SENSITIVE = 0x00020000,
            FO_HANDLE_CREATED = 0x00040000,
            FO_FILE_FAST_IO_READ = 0x00080000,
            FO_RANDOM_ACCESS = 0x00100000,
            FO_FILE_OPEN_CANCELLED = 0x00200000,
            FO_VOLUME_OPEN = 0x00400000,
            FO_REMOTE_ORIGIN = 0x01000000,
            FO_SKIP_COMPLETION_PORT = 0x02000000,
            FO_SKIP_SET_EVENT = 0x04000000,
            FO_SKIP_SET_FAST_IO = 0x08000000
        }

        [Flags]
        public enum HEAPENTRY32FLAGS : int
        {
            LF32_FIXED = 0x00000001,
            LF32_FREE = 0x00000002,
            LF32_MOVEABLE = 0x00000004
        }

        public enum MachineType : int
        {
            IMAGE_FILE_MACHINE_i386 = 0x014c,
            IMAGE_FILE_MACHINE_IA64 = 0x0200,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664
        }

        public enum MEMORY_PROTECTION : int
        {
            PAGE_ACCESS_DENIED = 0x0,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08
        }

        [Flags]
        public enum MEMORY_STATE : int
        {
            MEM_COMMIT = 0x1000,
            MEM_RESERVE = 0x2000,  
            MEM_FREE = 0x10000,
            MEM_RESET = 0x80000,
            MEM_TOP_DOWN = 0x100000,
            MEM_PHYSICAL = 0x400000,
            MEM_LARGE_PAGES = 0x20000000
        }

        public enum MEMORY_TYPE : int
        {
            MEM_IMAGE = 0x1000000,
            MEM_MAPPED = 0x40000,
            MEM_PRIVATE = 0x20000
        }

        public enum OBJECT_INFORMATION_CLASS : int
        {
            ObjectBasicInformation,
            ObjectNameInformation,
            ObjectTypeInformation,
            ObjectAllTypesInformation, 
            ObjectHandleInformation
        }

        [Flags]
        public enum PIPE_STATE : int
        {
            PIPE_NOWAIT = 0x1,
            PIPE_READMODE_MESSAGE = 0x2
        }

        [Flags]
        public enum POLICY_RIGHTS : uint
        {
            POLICY_VIEW_LOCAL_INFORMATION = 0x00000001,
            POLICY_VIEW_AUDIT_INFORMATION = 0x00000002,
            POLICY_GET_PRIVATE_INFORMATION = 0x00000004,
            POLICY_TRUST_ADMIN = 0x00000008,
            POLICY_CREATE_ACCOUNT = 0x00000010,
            POLICY_CREATE_SECRET = 0x00000020,
            POLICY_CREATE_PRIVILEGE = 0x00000040,
            POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x00000080,
            POLICY_SET_AUDIT_REQUIREMENTS = 0x00000100,
            POLICY_AUDIT_LOG_ADMIN = 0x00000200,
            POLICY_SERVER_ADMIN = 0x00000400,
            POLICY_LOOKUP_NAMES = 0x00000800,
            POLICY_NOTIFICATION = 0x00001000
        }

        public enum POOL_TYPE : uint
        {
            NonPagedPool,
            PagedPool,
            NonPagedPoolMustSucceed,
            DontUseThisType,
            NonPagedPoolCacheAligned,
            PagedPoolCacheAligned,
            NonPagedPoolCacheAlignedMustS
        }

        [Flags]
        public enum PROCESS_RIGHTS : uint
        {
            PROCESS_TERMINATE = 0x0001,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_SET_SESSIONID = 0x0004,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED | 
                STANDARD_RIGHTS.SYNCHRONIZE | 0xffff
        }

        public enum PROCESSINFOCLASS : int
        {
            ProcessBasicInformation = 0,
            ProcessDebugPort = 7,
            ProcessWow64Information = 26,
            ProcessImageFileName = 27
        }

        public enum SC_ACTION_TYPE : int
        {
            None = 0,
            Reboot = 2,
            Restart = 1,
            RunCommand = 3
        }

        public enum SC_MANAGER_RIGHTS : uint
        {
            SC_MANAGER_CONNECT = 0x0001,
            SC_MANAGER_CREATE_SERVICE = 0x0002,
            SC_MANAGER_ENUMERATE_SERVICE = 0x0004,
            SC_MANAGER_LOCK = 0x0008,
            SC_MANAGER_QUERY_LOCK_STATUS = 0x0010,
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x0020,
            SC_MANAGER_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED |
                SC_MANAGER_CONNECT | SC_MANAGER_CREATE_SERVICE | SC_MANAGER_ENUMERATE_SERVICE |
                SC_MANAGER_LOCK | SC_MANAGER_QUERY_LOCK_STATUS | SC_MANAGER_MODIFY_BOOT_CONFIG
        }

        [Flags]
        public enum SE_PRIVILEGE_ATTRIBUTES : uint
        {
            SE_PRIVILEGE_DISABLED = 0x00000000,
            SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001,
            SE_PRIVILEGE_ENABLED = 0x00000002,
            SE_PRIVILEGE_REMOVED = 0x00000004,
            SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000
        }

        public enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        public enum SERVICE_ACCEPT : int
        {
            NetBindChange = 0x10,
            ParamChange = 0x8,
            PauseContinue = 0x2,
            PreShutdown = 0x100,
            Shutdown = 0x4,
            Stop = 0x1,
            HardwareProfileChange = 0x20,
            PowerEvent = 0x40,
            SessionChange = 0x80
        }

        public enum SERVICE_CONTROL : int
        {
            Continue = 0x3,
            Interrogate = 0x4,
            NetBindAdd = 0x7,
            NetBindDisable = 0xa,
            NetBindEnable = 0x9,
            NetBindRemove = 0x8,
            ParamChange = 0x6,
            Pause = 0x2,
            Stop = 0x1
        }

        public enum SERVICE_ERROR_CONTROL : int
        {
            Critical = 0x3,
            Ignore = 0x0,
            Normal = 0x1,
            Severe = 0x2
        }

        public enum SERVICE_FLAGS : int
        {
            None = 0,
            RunsInSystemProcess = 0x1
        }

        public enum SERVICE_QUERY_STATE : int
        {
            Active = 1,
            Inactive = 2,
            All = 3
        }

        [Flags]
        public enum SERVICE_QUERY_TYPE : int
        {
            Driver = 0xb,
            Win32 = 0x30
        }

        public enum SERVICE_RIGHTS : uint
        {
            SERVICE_QUERY_CONFIG = 0x0001,
            SERVICE_CHANGE_CONFIG = 0x0002,
            SERVICE_QUERY_STATUS = 0x0004,
            SERVICE_ENUMERATE_DEPENDENTS = 0x0008,
            SERVICE_START = 0x0010,
            SERVICE_STOP = 0x0020,
            SERVICE_PAUSE_CONTINUE = 0x0040,
            SERVICE_INTERROGATE = 0x0080,
            SERVICE_USER_DEFINED_CONTROL = 0x0100,
            SERVICE_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED | 
                SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS |
                SERVICE_ENUMERATE_DEPENDENTS | SERVICE_START | SERVICE_STOP |
                SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL
        }

        public enum SERVICE_START_TYPE : int
        {
            AutoStart = 0x2,
            BootStart = 0x0,
            DemandStart = 0x3,
            Disabled = 0x4,
            SystemStart = 0x1
        }

        public enum SERVICE_STATE : int
        {
            ContinuePending = 0x5,
            PausePending = 0x6,
            Paused = 0x7,
            Running = 0x4,
            StartPending = 0x2,
            StopPending = 0x3,
            Stopped = 0x1
        }

        [Flags]
        public enum SERVICE_TYPE : int
        {
            FileSystemDriver = 0x2,
            KernelDriver = 0x1,
            Win32OwnProcess = 0x10,
            Win32ShareProcess = 0x20,
            InteractiveProcess = 0x100
        }

        public enum SID_ATTRIBUTES : uint
        {
            SE_GROUP_MANDATORY = 0x00000001,
            SE_GROUP_ENABLED_BY_DEFAULT = 0x00000002,
            SE_GROUP_ENABLED = 0x00000004,
            SE_GROUP_OWNER = 0x00000008,
            SE_GROUP_USE_FOR_DENY_ONLY = 0x00000010,
            SE_GROUP_INTEGRITY = 0x00000020,
            SE_GROUP_INTEGRITY_ENABLED = 0x00000040,
            SE_GROUP_LOGON_ID = 0xc0000000,
            SE_GROUP_RESOURCE = 0x20000000
        }

        public enum SID_NAME_USE : int
        {
              SidTypeUser = 1,
              SidTypeGroup,
              SidTypeDomain,
              SidTypeAlias,
              SidTypeWellKnownGroup,
              SidTypeDeletedAccount,
              SidTypeInvalid,
              SidTypeUnknown,
              SidTypeComputer,
              SidTypeLabel
        }

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001f
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
        public enum SYMBOL_FLAGS : int
        {
            SYMFLAG_CLR_TOKEN = 0x00040000,
            SYMFLAG_CONSTANT = 0x00000100,
            SYMFLAG_EXPORT = 0x00000200,
            SYMFLAG_FORWARDER = 0x00000400,
            SYMFLAG_FRAMEREL = 0x00000020,
            SYMFLAG_FUNCTION = 0x00000800,
            SYMFLAG_ILREL = 0x00010000,
            SYMFLAG_LOCAL = 0x00000080,
            SYMFLAG_METADATA = 0x00020000,
            SYMFLAG_PARAMETER = 0x00000040,
            SYMFLAG_REGISTER = 0x00000008,
            SYMFLAG_REGREL = 0x00000010,
            SYMFLAG_SLOT = 0x00008000,
            SYMFLAG_THUNK = 0x00002000,
            SYMFLAG_TLSREL = 0x00004000,
            SYMFLAG_VALUEPRESENT = 0x00000001,
            SYMFLAG_VIRTUAL = 0x00001000
        }

        [Flags]
        public enum SYSTEM_HANDLE_FLAGS : byte
        {
            PROTECT_FROM_CLOSE = 0x1,
            INHERIT = 0x2
        }

        public enum SYSTEM_INFORMATION_CLASS : int
        {
            SystemBasicInformation,              
            SystemProcessorInformation,          
            SystemPerformanceInformation,        
            SystemTimeOfDayInformation,          
            SystemNotImplemented1,               
            SystemProcessesAndThreadsInformation,
            SystemCallCounts,                    
            SystemConfigurationInformation,      
            SystemProcessorTimes,                
            SystemGlobalFlag,                    
            SystemNotImplemented2,               
            SystemModuleInformation,             
            SystemLockInformation,               
            SystemNotImplemented3,               
            SystemNotImplemented4,               
            SystemNotImplemented5,               
            SystemHandleInformation,             
            SystemObjectInformation,             
            SystemPagefileInformation,           
            SystemInstructionEmulationCounts,    
            SystemInvalidInfoClass1,             
            SystemCacheInformation,              
            SystemPoolTagInformation,            
            SystemProcessorStatistics,           
            SystemDpcInformation,                
            SystemNotImplemented6,               
            SystemLoadImage,                     
            SystemUnloadImage,                   
            SystemTimeAdjustment,                
            SystemNotImplemented7,               
            SystemNotImplemented8,               
            SystemNotImplemented9,               
            SystemCrashDumpInformation,          
            SystemExceptionInformation,          
            SystemCrashDumpStateInformation,     
            SystemKernelDebuggerInformation,     
            SystemContextSwitchInformation,      
            SystemRegistryQuotaInformation,      
            SystemLoadAndCallImage,              
            SystemPrioritySeparation,            
            SystemNotImplemented10,              
            SystemNotImplemented11,              
            SystemInvalidInfoClass2,             
            SystemInvalidInfoClass3,             
            SystemTimeZoneInformation,           
            SystemLookasideInformation,          
            SystemSetTimeSlipEvent,              
            SystemCreateSession,                 
            SystemDeleteSession,                 
            SystemInvalidInfoClass4,             
            SystemRangeStartInformation,         
            SystemVerifierInformation,           
            SystemAddVerifier,                   
            SystemSessionProcessesInformation    
        }

        [Flags]
        public enum THREAD_RIGHTS : uint
        {
            THREAD_TERMINATE = 0x0001,
            THREAD_SUSPEND_RESUME = 0x0002,
            THREAD_GET_CONTEXT = 0x0008,
            THREAD_SET_CONTEXT = 0x0010,
            THREAD_QUERY_INFORMATION = 0x0040,
            THREAD_SET_INFORMATION = 0x0020,
            THREAD_SET_THREAD_TOKEN = 0x0080,
            THREAD_IMPERSONATE = 0x0100,
            THREAD_DIRECT_IMPERSONATION = 0x0200,
            THREAD_SET_LIMITED_INFORMATION = 0x0400,
            THREAD_QUERY_LIMITED_INFORMATION = 0x0800,
            THREAD_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED |
                STANDARD_RIGHTS.SYNCHRONIZE | 0xffff
        }

        public enum TOKEN_INFORMATION_CLASS
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

        [Flags]
        public enum TOKEN_RIGHTS : uint
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

        public enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        public enum WTS_CONNECTSTATE_CLASS : int
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        public enum WTS_INFO_CLASS : int
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames
        }

        #endregion

        #region Imported Functions

        #region Terminal Server

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ProcessIdToSessionId(int ProcessId, ref int SessionId);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            [MarshalAs(UnmanagedType.LPTStr)] ref string Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            ref int Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            ref ushort Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSQuerySessionInformation(int ServerHandle, int SessionID,
            WTS_INFO_CLASS InfoClass,
            ref WTS_CLIENT_DISPLAY[] Buffer,
            ref int BytesReturned);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSLogoffSession(int ServerHandle, int SessionID, int Wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSDisconnectSession(int ServerHandle, int SessionID, int Wait);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSTerminateProcess(int ServerHandle, int ProcessID, int ExitCode);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateSessions(int ServerHandle, int Reserved,
            int Version, ref WTS_SESSION_INFO[] SessionInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateSessions(int ServerHandle, int Reserved,
            int Version, ref int SessionInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateProcesses(int ServerHandle, int Reserved,
            int Version, ref WTS_PROCESS_INFO[] ProcessInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int WTSEnumerateProcesses(int ServerHandle, int Reserved,
            int Version, ref int ProcessInfo, ref int Count);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(int Memory);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(WTS_PROCESS_INFO[] Memory);
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSFreeMemory(WTS_SESSION_INFO[] Memory);

        #endregion

        #region NT-specific functions

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQuerySymbolicLinkObject(int LinkHandle, ref UNICODE_STRING LinkTarget,
            ref int ReturnedLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            IntPtr ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            int ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            ref UNICODE_STRING ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int ZwQueryInformationProcess(int ProcessHandle, PROCESSINFOCLASS ProcessInformationClass,
            ref PROCESS_BASIC_INFORMATION ProcessInformation, int ProcessInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, int TargetHandle, STANDARD_RIGHTS DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwDuplicateObject(int SourceProcessHandle, int SourceHandle,
            int TargetProcessHandle, ref int TargetHandle, STANDARD_RIGHTS DesiredAccess, int Attributes, int Options);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            int SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            SYSTEM_HANDLE_INFORMATION[] SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQuerySystemInformation(SYSTEM_INFORMATION_CLASS SystemInformationClass,
            uint[] SystemInformation, int SystemInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            ref OBJECT_BASIC_INFORMATION ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            ref OBJECT_TYPE_INFORMATION ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            ref OBJECT_NAME_INFORMATION ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            IntPtr ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern uint ZwQueryObject(int Handle, OBJECT_INFORMATION_CLASS ObjectInformationClass,
            int ObjectInformation, int ObjectInformationLength, ref int ReturnLength);

        #endregion

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int StartService(int Service, int NumServiceArgs, int Args);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int ChangeServiceConfig(int Service,
            SERVICE_TYPE ServiceType, SERVICE_START_TYPE StartType,
            SERVICE_ERROR_CONTROL ErrorControl,
            [MarshalAs(UnmanagedType.LPTStr)] string BinaryPath,
            [MarshalAs(UnmanagedType.LPTStr)] string LoadOrderGroup,
            int TagID, int Dependencies,
            [MarshalAs(UnmanagedType.LPTStr)] string StartName,
            int Password, int DisplayName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int ControlService(int Service,
            SERVICE_CONTROL Control, ref SERVICE_STATUS ServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CreateService(int SCManager, 
            [MarshalAs(UnmanagedType.LPTStr)] string ServiceName,
            [MarshalAs(UnmanagedType.LPTStr)] string DisplayName,
            SERVICE_RIGHTS DesiredAccess, SERVICE_TYPE ServiceType,
            SERVICE_START_TYPE StartType, SERVICE_ERROR_CONTROL ErrorControl,
            [MarshalAs(UnmanagedType.LPTStr)] string BinaryPathName,
            [MarshalAs(UnmanagedType.LPTStr)] string LoadOrderGroup,
            int TagID, int Dependencies,
            [MarshalAs(UnmanagedType.LPTStr)] string ServiceStartName,
            [MarshalAs(UnmanagedType.LPTStr)] string Password);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DeleteService(int Service);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceConfig(int Service,
            int ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceConfig(int Service,
            IntPtr ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryServiceConfig(int Service,
            [MarshalAs(UnmanagedType.Struct)] ref QUERY_SERVICE_CONFIG ServiceConfig,
            int BufSize, ref int BytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int OpenService(int SCManager,
            [MarshalAs(UnmanagedType.LPTStr)] string ServiceName, SERVICE_RIGHTS DesiredAccess);

        /// <summary>
        /// Enumerates services in the specified service control manager database. 
        /// The name and status of each service are provided, along with additional 
        /// data based on the specified information level.
        /// </summary>
        /// <param name="SCManager">A handle to the service control manager database.</param>
        /// <param name="InfoLevel">Set this to 0.</param>
        /// <param name="ServiceType">The type of services to be enumerated.</param>
        /// <param name="ServiceState">The state of the services to be enumerated.</param>
        /// <param name="Services">A pointer to the buffer that receives the status information.</param>
        /// <param name="BufSize">The size of the buffer pointed to by the Services parameter, in bytes.</param>
        /// <param name="BytesNeeded">A pointer to a variable that receives the number of bytes needed to 
        /// return the remaining service entries, if the buffer is too small.</param>
        /// <param name="ServicesReturned">A pointer to a variable that receives the number of service 
        /// entries returned.</param>
        /// <param name="ResumeHandle">A pointer to a variable that, on input, specifies the 
        /// starting point of enumeration. You must set this value to zero the first time the 
        /// EnumServicesStatusEx function is called.</param>
        /// <param name="GroupName">Must be 0 for this definition.</param>
        /// <returns>A non-zero value for success, zero for failure.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int EnumServicesStatusEx(int SCManager, int InfoLevel,
            SERVICE_QUERY_TYPE ServiceType, SERVICE_QUERY_STATE ServiceState,
            ref int Services, int BufSize, ref int BytesNeeded, ref int ServicesReturned,
            ref int ResumeHandle, int GroupName);

        /// <summary>
        /// Enumerates services in the specified service control manager database. 
        /// The name and status of each service are provided, along with additional 
        /// data based on the specified information level.
        /// </summary>
        /// <param name="SCManager">A handle to the service control manager database.</param>
        /// <param name="InfoLevel">Set this to 0.</param>
        /// <param name="ServiceType">The type of services to be enumerated.</param>
        /// <param name="ServiceState">The state of the services to be enumerated.</param>
        /// <param name="Services">A pointer to the buffer that receives the status information.</param>
        /// <param name="BufSize">The size of the buffer pointed to by the Services parameter, in bytes.</param>
        /// <param name="BytesNeeded">A pointer to a variable that receives the number of bytes needed to 
        /// return the remaining service entries, if the buffer is too small.</param>
        /// <param name="ServicesReturned">A pointer to a variable that receives the number of service 
        /// entries returned.</param>
        /// <param name="ResumeHandle">A pointer to a variable that, on input, specifies the 
        /// starting point of enumeration. You must set this value to zero the first time the 
        /// EnumServicesStatusEx function is called.</param>
        /// <param name="GroupName">Must be 0 for this definition.</param>
        /// <returns>A non-zero value for success, zero for failure.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int EnumServicesStatusEx(int SCManager, int InfoLevel,
            SERVICE_QUERY_TYPE ServiceType, SERVICE_QUERY_STATE ServiceState,
            IntPtr Services, int BufSize, ref int BytesNeeded, ref int ServicesReturned,
            ref int ResumeHandle, int GroupName);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenSCManager(int MachineName, int DatabaseName,
            SC_MANAGER_RIGHTS DesiredAccess); 
     
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaAddAccountRights(int PolicyHandle, int AccountSid,
            LSA_UNICODE_STRING[] UserRights, uint CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaOpenPolicy(int SystemName, LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            POLICY_RIGHTS DesiredAccess, ref int PolicyHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LsaClose(int Handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int OpenProcessToken(int ProcessHandle, TOKEN_RIGHTS DesiredAccess,
            ref int TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ConvertSidToStringSid(
            int pSID,
            [In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid
        );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ConvertStringSidToSid(
            [In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid,
            ref IntPtr pSID
        );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int CreateProcessWithTokenW(int Token, int LogonFlags,
            [MarshalAs(UnmanagedType.LPWStr)] string ApplicationName,
            [MarshalAs(UnmanagedType.LPWStr)] string CommandLine, int CreationFlags,
            int Environment, int CurrentDirectory, STARTUPINFO StartupInfo,
            PROCESS_INFORMATION ProcessInfo);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DuplicateTokenEx(int ExistingToken, TOKEN_RIGHTS DesiredAccess,
            int TokenAttributes, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType,
            ref int NewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref int TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_GROUPS TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_USER TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetTokenInformation(int TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass, ref TOKEN_PRIVILEGES TokenInformation,
            int TokenInformationLength, ref int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupAccountSid(string SystemName,
            int SID, [Out] System.Text.StringBuilder Name, ref int NameSize,
            [Out] System.Text.StringBuilder ReferencedDomainName, ref int ReferencedDomainNameSize,
            ref SID_NAME_USE Use);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupAccountSid(int SystemName,
            int SID, [Out] System.Text.StringBuilder Name, ref int NameSize,
            [Out] StringBuilder ReferencedDomainName, ref int ReferencedDomainNameSize,
            ref SID_NAME_USE Use);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeDisplayName(int SystemName, string Name,
            [Out] StringBuilder DisplayName, ref int DisplayNameSize, ref int LanguageId);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeName(int SystemName, ref LUID Luid,
            [Out] StringBuilder Name, ref int RequiredSize);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int LookupPrivilegeValue(string SystemName, string PrivilegeName,
            [MarshalAs(UnmanagedType.Struct)] ref LUID Luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int AdjustTokenPrivileges(int TokenHandle, int DisableAllPrivileges,
            [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength,
            int PreviousState, int ReturnLength);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymCleanup(int ProcessHandle);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymEnumSymbols(int ProcessHandle, int BaseOfDll, int Mask,
            [MarshalAs(UnmanagedType.FunctionPtr)] SymEnumSymbolsProc EnumSymbolsCallback, int UserContext);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymEnumSymbols(int ProcessHandle, int BaseOfDll, string Mask,
            [MarshalAs(UnmanagedType.FunctionPtr)] SymEnumSymbolsProc EnumSymbolsCallback, int UserContext);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFromAddr(int ProcessHandle, long Address, ref long Displacement, ref SYMBOL_INFO Symbol);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFromIndex(int ProcessHandle, int BaseOfDll, int Index, ref SYMBOL_INFO Symbol);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymFunctionTableAccess64(int ProcessHandle, int AddrBase);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymGetModuleBase64(int ProcessHandle, int dwAddr);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int SymInitialize(int ProcessHandle, int UserSearchPath, int InvadeProcess);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int StackWalk64(MachineType MachineType, int ProcessHandle, int ThreadHandle,
            [MarshalAs(UnmanagedType.Struct)] ref STACKFRAME64 StackFrame,
            [MarshalAs(UnmanagedType.Struct)] ref CONTEXT ContextRecord, int ReadMemoryRoutine,
            [MarshalAs(UnmanagedType.FunctionPtr)] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [MarshalAs(UnmanagedType.FunctionPtr)] GetModuleBaseProc64 GetModuleBaseRoutine,
            int TranslateAddress);

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern int StackWalk64(MachineType MachineType, int ProcessHandle, int ThreadHandle,
            [MarshalAs(UnmanagedType.Struct)] ref STACKFRAME64 StackFrame,
            [MarshalAs(UnmanagedType.Struct)] ref CONTEXT ContextRecord, int ReadMemoryRoutine,
            int FunctionTableAccessRoutine,
            int GetModuleBaseRoutine,
            int TranslateAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIoCounters(int ProcessHandle, ref IO_COUNTERS IoCounters);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetProcessAffinityMask(int ProcessHandle, uint ProcessAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessAffinityMask(int ProcessHandle, ref uint ProcessAffinityMask,
            ref uint SystemAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CheckRemoteDebuggerPresent(int ProcessHandle, ref int DebuggerPresent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetNamedPipeHandleState(int NamedPipeHandle, ref PIPE_STATE State,
            int CurInstances, int MaxCollectionCount, int CollectDataTimeout, int UserName, int MaxUserNameSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateThread(int ThreadAttributes, int StackSize,
            [MarshalAs(UnmanagedType.FunctionPtr)] System.Threading.ThreadStart StartAddress,
            int Parameter, int CreationFlags, ref int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(int Object, int Timeout);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIdOfThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(int ProcessHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessDEPPolicy(int ProcessHandle, ref DEPFLAGS Flags, ref int Permanent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(int Handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int TerminateProcess(int ProcessHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenProcess(PROCESS_RIGHTS DesiredAccess, int InheritHandle, int ProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int OpenThread(THREAD_RIGHTS DesiredAccess, int InheritHandle, int ThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int TerminateThread(int ThreadHandle, int ExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(int ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadContext(int ThreadHandle, ref CONTEXT Context);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateRemoteThread(int ProcessHandle, int ThreadAttributes,
            int StackSize, int StartAddress, int Parameter, int CreationFlags, ref int ThreadId); 

        [DllImport("shell32.dll")]
        public static extern int ShellExecuteEx(
            [MarshalAs(UnmanagedType.Struct)] ref SHELLEXECUTEINFO s);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(int HookId, int HookFunction, int Module, int ThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int LoadLibrary(string FileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int LoadLibraryEx(string FileName, int File, int Flags);

        [DllImport("kernel32.dll")]
        public static extern int FreeLibrary(int Handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetModuleHandle(string ModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetProcAddress(int Module, string ProcName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetProcAddress(int Module, int ProcOrdinal);

        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(int Process, int Address,
            [MarshalAs(UnmanagedType.Struct)] ref MEMORY_BASIC_INFORMATION Buffer, int Size);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualProtect(int Address, int Size, int NewProtect, ref int OldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualProtectEx(int Process, int Address, int Size, int NewProtect, ref int OldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualAllocEx(int Process, int Address, int Size, MEMORY_STATE Type, MEMORY_PROTECTION Protect);

        [DllImport("kernel32.dll")]
        public static extern int DebugActiveProcess(int PID);

        [DllImport("kernel32.dll")]
        public static extern int DebugActiveProcessStop(int PID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ReadProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, ref int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ReadProcessMemory(int Process, int BaseAddress, int Buffer, int Size, ref int BytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProcessMemory(int Process, int BaseAddress, byte[] Buffer, int Size, ref int BytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Process32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Process32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Thread32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref THREADENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Thread32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref THREADENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Module32First(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref MODULEENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Module32Next(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref MODULEENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32ListFirst(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HEAPLIST32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32ListNext(int hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HEAPLIST32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32First([MarshalAs(UnmanagedType.Struct)] ref HEAPENTRY32 lppe,
            int ProcessID, int HeapID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32Next([MarshalAs(UnmanagedType.Struct)] ref HEAPENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern bool GetThreadTimes(int hThread, out long lpCreationTime,
           out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DuplicateHandle(int hSourceProcessHandle,
           int hSourceHandle, int hTargetProcessHandle, ref int lpTargetHandle,
           uint dwDesiredAccess, int bInheritHandle, uint dwOptions);

        [DllImport("kernel32.dll")]
        public static extern int SearchPath(
            int Zero0, [MarshalAs(UnmanagedType.LPTStr)] string FileName,
            int Zero1, int BufferLength,
            [Out] System.Text.StringBuilder Buffer, int Zero2);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern int EnumDeviceDrivers(int[] ImageBases, int Size, ref int Needed);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceDriverBaseName(int ImageBase,
            [Out] System.Text.StringBuilder FileName, int Size);

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetDeviceDriverFileName(int ImageBase, 
            [Out] System.Text.StringBuilder FileName, int Size);

        [DllImport("user32.dll")]
        public static extern int EnumWindows([MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProc Callback, int param);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hWnd); 

        [DllImport("shell32.dll")]
        public extern static int ExtractIconEx(string libName, int iconIndex,
        IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        [DllImport("shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);

        #endregion

        #region Imported Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct ADDRESS64
        {
            public long Offset;
            public short Segment;
            public ADDRESS_MODE Mode;
        }

        // NOTE: This x86 CONTEXT ONLY!!!
        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public CONTEXT_FLAGS ContextFlags;

            public int Dr0;
            public int Dr1;
            public int Dr2;
            public int Dr3;
            public int Dr4;
            public int Dr5;
            public int Dr6;
            public int Dr7;

            [MarshalAs(UnmanagedType.Struct)]
            public FLOATING_SAVE_AREA FloatSave;
                     
            public int SegGs;
            public int SegFs;
            public int SegEs;
            public int SegDs;

            public int Edi;
            public int Esi;
            public int Ebx;
            public int Edx;
            public int Ecx;
            public int Eax;

            public int Ebp;
            public int Eip;
            public int SegCs;
            public int EFlags;
            public int Esp;
            public int SegSs;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_SUPPORTED_EXTENSION)]
            public byte[] ExtendedRegisters;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENUM_SERVICE_STATUS
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string ServiceName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.Struct)]
            public SERVICE_STATUS ServiceStatus;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ENUM_SERVICE_STATUS_PROCESS
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string ServiceName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.Struct)]
            public SERVICE_STATUS_PROCESS ServiceStatusProcess;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FLOATING_SAVE_AREA
        {
            public int ControlWord;
            public int StatusWord;
            public int TagWord;
            public int ErrorOffset;
            public int ErrorSelector;
            public int DataOffset;
            public int DataSelector;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SIZE_OF_80387_REGISTERS)]
            public byte[] RegisterArea;

            public int Cr0NpxState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FPO_DATA
        {
            public int ulOffStart;
            public int cbProcSize;
            public int cdwLocals;
            public short cdwParams;

            public long Part1;
            public long Part2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GENERIC_MAPPING
        {
            public STANDARD_RIGHTS GenericRead;
            public STANDARD_RIGHTS GenericWrite;
            public STANDARD_RIGHTS GenericExecute;
            public STANDARD_RIGHTS GenericAll;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HEAPENTRY32
        {
            public int dwSize;
            public int hHandle;
            public int dwAddress;
            public int dwBlockSize;
            public HEAPENTRY32FLAGS dwFlags;
            public int dwLockCount;
            public int dwResvd;
            public int th32ProcessID;
            public int th32HeapID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HEAPLIST32
        {
            public int dwSize;
            public int th32ProcessID;
            public int th32HeapID;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KDHELP64
        {
            public long Thread;
            public int ThCallbackStack;
            public int ThCallbackBSTore;
            public int NextCallback;
            public int FramePointer;
            public long KiCallUserMode;
            public long KeUserCallbackDispatcher;
            public long SystemRangeStart;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public long[] Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LSA_OBJECT_ATTRIBUTES
        {
            uint Length;
            int RootDirectory;
            int ObjectName;
            uint Attributes;
            int SecurityDescriptor;
            int SecurityQualityOfService;
        }   
  
        [StructLayout(LayoutKind.Sequential)]
        public struct LSA_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public int LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public SE_PRIVILEGE_ATTRIBUTES Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public MEMORY_PROTECTION AllocationProtect;
            public int RegionSize;
            public MEMORY_STATE State;
            public MEMORY_PROTECTION Protect;
            public MEMORY_TYPE Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEENTRY32
        {
            public int dwSize;
            public int th32ModuleID;
            public int th32ProcessID;
            public int GlblcntUsage;
            public int ProccntUsage;
            public int modBaseAddr;
            public int modBaseSize;
            public int hModule;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szModule;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_BASIC_INFORMATION
        {
            public uint Attributes;
            public STANDARD_RIGHTS GrantedAccess;
            public uint HandleCount;
            public uint PointerCount;
            public uint PagedPoolUsage;
            public uint NonPagedPoolUsage;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public uint[] Reserved;

            public uint NameInformationLength;
            public uint TypeInformationLength;
            public uint SecurityDescriptorLength;
            public ulong CreateTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_NAME_INFORMATION
        {
            public UNICODE_STRING Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_TYPE_INFORMATION
        {
            public UNICODE_STRING Name;
            public uint ObjectCount;
            public uint HandleCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] Reserved1;

            public uint PeakObjectCount;
            public uint PeakHandleCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] Reserved2;

            public STANDARD_RIGHTS InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public STANDARD_RIGHTS ValidAccess;
            public byte SecurityRequired;
            public byte MaintainHandleCount;
            public ushort MaintainTypeList;

            public POOL_TYPE PoolType;
            public uint PagedPoolUsage;
            public uint NonPagedPoolUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_BASIC_INFORMATION
        {
            public int Reserved1;
            public int PebBaseAddress;
            public int Reserved2_1;
            public int Reserved2_2;
            public int UniqueProcessId;
            public int Reserved3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public int dwSize;
            public int cntUsage;
            public int th32ProcessID;
            public int th32DefaultHeapID;
            public int th32ModuleID;
            public int cntThreads;
            public int th32ParentProcessID;
            public int pcPriClassBase;
            public int dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            int hProcess;
            int hThread;
            int dwProcessId;
            int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QUERY_SERVICE_CONFIG
        {
            public SERVICE_TYPE ServiceType;
            public SERVICE_START_TYPE StartType;
            public SERVICE_ERROR_CONTROL ErrorControl;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string BinaryPathName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string LoadOrderGroup;

            public int TagID;
            public int Dependencies; // pointer to a string array

            [MarshalAs(UnmanagedType.LPTStr)]
            public string ServiceStartName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string DisplayName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SC_ACTION
        {
            public SC_ACTION_TYPE Type;
            public int Delay;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_STATUS
        {
            public SERVICE_TYPE ServiceType;
            public SERVICE_STATE CurrentState;
            public SERVICE_ACCEPT ControlsAccepted;
            public int Win32ExitCode;
            public int ServiceSpecificExitCode;
            public int CheckPoint;
            public int WaitHint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_STATUS_PROCESS
        {
            public SERVICE_TYPE ServiceType;
            public SERVICE_STATE CurrentState;
            public SERVICE_ACCEPT ControlsAccepted;
            public int Win32ExitCode;
            public int ServiceSpecificExitCode;
            public int CheckPoint;
            public int WaitHint;
            public int ProcessID;
            public SERVICE_FLAGS ServiceFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public int fMask;
            public int hWnd;
            public string lpVerb;
            public string lpFile;
            public string lpParameters;
            public string lpDirectory;
            public int nShow;
            public int hInstApp;

            public int lpIDList;
            public string lpClass;
            public int hkeyClass;
            public int dwHotKey;
            public int hIcon;
            public int hProcess;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SID_SIZE)]
            public byte[] SIDContents;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES
        {
            public int SID; // ptr to a SID object
            public SID_ATTRIBUTES Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STACKFRAME64
        {
            public ADDRESS64 AddrPC;
            public ADDRESS64 AddrReturn;
            public ADDRESS64 AddrFrame;
            public ADDRESS64 AddrStack;
            public ADDRESS64 AddrBStore;

            public int FuncTableEntry;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public long[] Params;

            public int Far;
            public int Virtual;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public long[] Reserved;

            KDHELP64 KdHelp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            int cb;
            [MarshalAs(UnmanagedType.LPTStr)] string lpReserved;
            [MarshalAs(UnmanagedType.LPTStr)] string lpDesktop;
            [MarshalAs(UnmanagedType.LPTStr)] string lpTitle;
            int dwX;
            int dwY;
            int dwXSize;
            int dwYSize;
            int dwXCountChars;
            int dwYCountChars;
            int dwFillAttribute;
            int dwFlags;
            short wShowWindow;
            short cbReserved2;
            byte lpReserved2;
            int hStdInput;
            int hStdOutput;
            int hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYMBOL_INFO
        {
            public int SizeOfStruct;
            public int TypeIndex;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public long[] Reserved;

            public int Index;
            public int Size;
            public long ModBase;
            public SYMBOL_FLAGS Flags;
            public long Value;
            public long Address;
            public int Register;
            public int Scope;
            public int Tag;
            public int NameLen;
            public int MaxNameLen;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SYMBOL_NAME_MAXSIZE)]
            public string Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_HANDLE_INFORMATION
        {
            public int ProcessId;
            public byte ObjectTypeNumber;
            public SYSTEM_HANDLE_FLAGS Flags;
            public short Handle;
            public int Object;
            public STANDARD_RIGHTS GrantedAccess;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_OBJECT_TYPE_INFORMATION
        {
            public uint NextEntryOffset;
            public uint ObjectCount;
            public uint HandleCount;
            public uint TypeNumber;
            public uint InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public STANDARD_RIGHTS ValidAccessMask;
            public POOL_TYPE PoolType;
            public byte Unknown;

            [MarshalAs(UnmanagedType.Struct)]
            public UNICODE_STRING Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_PROCESS_INFORMATION
        {
            public uint NextEntryOffset;
            public uint NumberOfThreads;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] Reserved1;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] Reserved2;

            public int UniqueProcessId;

            public int Reserved3;
            public uint HandleCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Reserved4;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public int[] Reserved5;

            public uint PeakPagefileUsage;
            public uint PrivatePageCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public long[] Reserved6;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct THREADENTRY32
        {
            public int dwSize;
            public int cntUsage;
            public int th32ThreadID;
            public int th32OwnerProcessID;
            public int tpBasePri;
            public int tpDeltaPri;
            public int dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_GROUPS
        {
            public uint GroupCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public SID_AND_ATTRIBUTES[] Groups;
        }  

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ANYSIZE_ARRAY)]
            public LUID_AND_ATTRIBUTES[] Privileges;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public LUID_AND_ATTRIBUTES[] Privileges2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;

            // space for random crap
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SID_SIZE)]
            public byte[] SIDContents;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;

            public int Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_CLIENT_DISPLAY
        {
            public int HorizontalResolution;
            public int VerticalResolution;
            public int ColorDepth;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_PROCESS_INFO
        {
            public int SessionID;
            public int ProcessID;
            
            [MarshalAs(UnmanagedType.LPTStr)]
            public string ProcessName;

            public int SID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public int SessionID;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string WinStationName;

            WTS_CONNECTSTATE_CLASS State;
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
                returnSessions[i] = (WTS_SESSION_INFO)Marshal.PtrToStructure(
                    new IntPtr(sessions + Marshal.SizeOf(typeof(WTS_SESSION_INFO)) * i), typeof(WTS_SESSION_INFO));
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
                returnProcesses[i].Info = (WTS_PROCESS_INFO)Marshal.PtrToStructure(
                    new IntPtr(processes + Marshal.SizeOf(typeof(WTS_PROCESS_INFO)) * i), typeof(WTS_PROCESS_INFO));

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

        #region NT-specific

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
                returnHandles[i] = (SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(
                    new IntPtr(4 + data.ToInt32() + i * Marshal.SizeOf(typeof(SYSTEM_HANDLE_INFORMATION))),
                    typeof(SYSTEM_HANDLE_INFORMATION));
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
                Program.CurrentProcess, ref object_handle, 0, 0,
                0x4 // DUPLICATE_SAME_ATTRIBUTES
                ) != 0)
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
                    OBJECT_BASIC_INFORMATION obi =
                        (OBJECT_BASIC_INFORMATION)Marshal.PtrToStructure(obiMem, typeof(OBJECT_BASIC_INFORMATION));
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
                            OBJECT_TYPE_INFORMATION oti =
                                (OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(otiMem, typeof(OBJECT_TYPE_INFORMATION));
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
                        OBJECT_NAME_INFORMATION oni =
                            (OBJECT_NAME_INFORMATION)Marshal.PtrToStructure(oniMem, typeof(OBJECT_NAME_INFORMATION));

                        try
                        {
                            info.OrigName = GetSymbolicLinkTarget(object_handle);
                        }
                        catch
                        {
                            info.OrigName = ReadUnicodeString(oni.Name);
                        }

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
                                    (STANDARD_RIGHTS)PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0,
                                    0x4 // DUPLICATE_SAME_ATTRIBUTES
                                    ) != 0)
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
                                    (STANDARD_RIGHTS)THREAD_RIGHTS.THREAD_QUERY_INFORMATION, 0,
                                    0x4 // DUPLICATE_SAME_ATTRIBUTES
                                    ) != 0)
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
                                    (STANDARD_RIGHTS)TOKEN_RIGHTS.TOKEN_QUERY, 0,
                                    0x4 // DUPLICATE_SAME_ATTRIBUTES
                                    ) != 0)
                                    throw new Exception("Could not duplicate token handle!");

                                try
                                {
                                    info.BestName = GetTokenUsername(token_handle, true);
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

        public static string GetSymbolicLinkTarget(int Object)
        {
            UNICODE_STRING str = new UNICODE_STRING();
            int neededLength = 0;

            str.MaximumLength = 0;
            str.Length = 0;
            str.Buffer = 0;

            if (ZwQuerySymbolicLinkObject(Object, ref str, ref neededLength) != 0)
                throw new Exception(GetLastErrorMessage());

            str.MaximumLength = (ushort)neededLength;
            str.Length = 0;
            str.Buffer = Marshal.AllocHGlobal(neededLength).ToInt32();

            try
            {
                if (ZwQuerySymbolicLinkObject(Object, ref str, ref neededLength) != 0)
                    throw new Exception(GetLastErrorMessage());


                return ReadUnicodeString(str);
            }
            finally
            {
                Marshal.FreeHGlobal(new IntPtr(str.Buffer));
            }
        }

        #endregion

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
                    ENUM_SERVICE_STATUS_PROCESS service = (ENUM_SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(
                        new IntPtr(data.ToInt32() + Marshal.SizeOf(typeof(ENUM_SERVICE_STATUS_PROCESS)) * i),
                        typeof(ENUM_SERVICE_STATUS_PROCESS));

                    dictionary.Add(service.ServiceName, service);
                }
            }
            finally
            {
                CloseHandle(manager);
                Marshal.FreeHGlobal(data);
            }

            return dictionary;
        }

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

        public static string GetLastErrorMessage()
        {
            return GetErrorMessage(Marshal.GetLastWin32Error());
        }

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

        public static IO_COUNTERS GetProcessIoCounters(ProcessHandle process)
        {
            IO_COUNTERS counters = new IO_COUNTERS();

            if (GetProcessIoCounters(process.Handle, ref counters) == 0)
                throw new Exception(GetLastErrorMessage());

            return counters;
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
                        ref token) == 0)
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

        public static int GetProcessSID(int ProcessHandle)
        {
            int token = 0;
            TOKEN_USER user = new TOKEN_USER();
            int retlen = 0;

            if (OpenProcessToken(ProcessHandle, TOKEN_RIGHTS.TOKEN_QUERY, ref token) == 0)
                return 0;

            if (GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, ref user,
                Marshal.SizeOf(user), ref retlen) == 0)
            {
                CloseHandle(token);
                return 0;
            }

            CloseHandle(token);

            return user.User.SID;
        }

        public static string GetProcessUsername(int ProcessHandle, bool IncludeDomain)
        {
            int token = 0;
            TOKEN_USER user = new TOKEN_USER();
            int retlen = 0;

            if (OpenProcessToken(ProcessHandle, TOKEN_RIGHTS.TOKEN_QUERY, ref token) == 0)
                throw new Exception("Could not open process handle with TOKEN_QUERY: " + Win32.GetLastErrorMessage());

            try
            {
                if (GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, ref user,
                    Marshal.SizeOf(user), ref retlen) == 0)
                {
                    throw new Exception("Could not get token information: " + Win32.GetLastErrorMessage());
                }
            }
            finally
            {
                CloseHandle(token);
            }

            return GetAccountName(user.User.SID, IncludeDomain); 
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
                CloseHandle(manager);

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

                config = (QUERY_SERVICE_CONFIG)Marshal.PtrToStructure(data, typeof(QUERY_SERVICE_CONFIG));
            }
            finally
            {
                CloseHandle(handle);
                CloseHandle(manager);
                Marshal.FreeHGlobal(data);
            }

            return config;
        }

        public static string GetTokenUsername(int TokenHandle, bool IncludeDomain)
        {           
            TOKEN_USER user = new TOKEN_USER();     
            int retlen = 0;

            if (GetTokenInformation(TokenHandle, TOKEN_INFORMATION_CLASS.TokenUser, ref user,
                Marshal.SizeOf(user), ref retlen) == 0)
            {
                throw new Exception("Could not get token information: " + Win32.GetLastErrorMessage());
            }

            return GetAccountName(user.User.SID, IncludeDomain);
        }

        public static bool IsBeingDebugged(int ProcessHandle)
        {
            int debugged = 0;

            if (Win32.CheckRemoteDebuggerPresent(ProcessHandle, ref debugged) == 0)
                throw new Exception(GetLastErrorMessage());

            return debugged == 1;
        }

        public static int OpenLocalPolicy()
        {
            LSA_OBJECT_ATTRIBUTES attributes = new LSA_OBJECT_ATTRIBUTES();
            int handle = 0;

            if (LsaOpenPolicy(0, attributes, POLICY_RIGHTS.POLICY_CREATE_PRIVILEGE, ref handle) == 0)
                return 0;

            return handle;
        }

        public static TOKEN_GROUPS ReadTokenGroups(int ProcessHandle)
        {
            int token = 0;
            int retlen = 0;
            TOKEN_GROUPS tkg = new TOKEN_GROUPS();

            if (OpenProcessToken(ProcessHandle, TOKEN_RIGHTS.TOKEN_QUERY, ref token) == 0)
                return new TOKEN_GROUPS() { GroupCount = 0 };

            if (GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenGroups, ref tkg,
                Marshal.SizeOf(tkg), ref retlen) == 0)
            {
                CloseHandle(token);
                return new TOKEN_GROUPS() { GroupCount = 0 };
            }

            CloseHandle(token);

            return tkg;
        }

        public static TOKEN_PRIVILEGES ReadTokenPrivileges(int ProcessHandle)
        {
            int token = 0;
            int retlen = 0;
            TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();

            if (OpenProcessToken(ProcessHandle, TOKEN_RIGHTS.TOKEN_QUERY, ref token) == 0)
                return new TOKEN_PRIVILEGES() { PrivilegeCount = 0 };
                                              
            if (GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenPrivileges, ref tkp,
                Marshal.SizeOf(tkp), ref retlen) == 0)
            {
                CloseHandle(token);
                return new TOKEN_PRIVILEGES() { PrivilegeCount = 0 };
            }

            CloseHandle(token);

            return tkp;
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

        public static int WriteTokenPrivilege(string PrivilegeName, SE_PRIVILEGE_ATTRIBUTES Attributes)
        {
            return WriteTokenPrivilege(Program.CurrentProcess, PrivilegeName, Attributes);
        }

        public static int WriteTokenPrivilege(int ProcessHandle, string PrivilegeName, SE_PRIVILEGE_ATTRIBUTES Attributes)
        {
            int token = 0;
            TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();

            tkp.Privileges = new LUID_AND_ATTRIBUTES[1];

            if (OpenProcessToken(ProcessHandle,
                TOKEN_RIGHTS.TOKEN_ADJUST_PRIVILEGES | TOKEN_RIGHTS.TOKEN_QUERY,
                ref token) == 0)
                return 0;

            if (LookupPrivilegeValue(null, PrivilegeName, ref tkp.Privileges[0].Luid) == 0)
                return 0;

            tkp.PrivilegeCount = 1;
            tkp.Privileges[0].Attributes = Attributes;  

            AdjustTokenPrivileges(token, 0, ref tkp, 0, 0, 0);

            if (Marshal.GetLastWin32Error() != 0)
                return 0;

            return 1;
        }
    }
}
