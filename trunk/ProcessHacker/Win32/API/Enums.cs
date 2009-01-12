/*
 * Process Hacker
 * 
 * Copyright (C) 2009 Dean
 * Copyright (C) 2008-2009 wj32
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

        public enum DEP_SYSTEM_POLICY_TYPE : int
        {
            AlwaysOff = 0,
            AlwaysOn,
            OptIn,
            OptOut
        }

        public enum EVENT_INFORMATION_CLASS : int
        {
            EventBasicInformation
        }

        public enum EVENT_TYPE : int
        {
            NotificationEvent,
            SynchronizationEvent
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

        public enum KWAIT_REASON : int
        {
            Executive = 0,
            FreePage = 1,
            PageIn = 2,
            PoolAllocation = 3,
            DelayExecution = 4,
            Suspended = 5,
            UserRequest = 6,
            WrExecutive = 7,
            WrFreePage = 8,
            WrPageIn = 9,
            WrPoolAllocation = 10,
            WrDelayExecution = 11,
            WrSuspended = 12,
            WrUserRequest = 13,
            WrEventPair = 14,
            WrQueue = 15,
            WrLpcReceive = 16,
            WrLpcReply = 17,
            WrVirtualMemory = 18,
            WrPageOut = 19,
            WrRendezvous = 20,
            Spare2 = 21,
            Spare3 = 22,
            Spare4 = 23,
            Spare5 = 24,
            WrCalloutStack = 25,
            WrKernel = 26,
            WrResource = 27,
            WrPushLock = 28,
            WrMutex = 29,
            WrQuantumEnd = 30,
            WrDispatchInt = 31,
            WrPreempted = 32,
            WrYieldExecution = 33,
            WrFastMutex = 34,
            WrGuardedMutex = 35,
            WrRundown = 36,
            MaximumWaitReason = 37
        }

        public enum MachineType : int
        {
            IMAGE_FILE_MACHINE_i386 = 0x014c,
            IMAGE_FILE_MACHINE_IA64 = 0x0200,
            IMAGE_FILE_MACHINE_AMD64 = 0x8664
        }

        [Flags]
        public enum MEM_EXECUTE_OPTIONS : int
        {
            EnableDEP = 0x1,
            DisableDEP = 0x2,
            DisableATLThunkEmulation = 0x4,
            Permanent = 0x8
        }

        [Flags]
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

            /// <summary>
            /// Decommits memory, putting them into the reserved state.
            /// </summary>
            MEM_DECOMMIT = 0x4000,

            /// <summary>
            /// Frees memory, putting them into the freed state.
            /// </summary>
            MEM_RELEASE = 0x8000,
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

        public enum MIB_TCP_STATE : int
        {
            MIB_TCP_STATE_CLOSED = 1,
            MIB_TCP_STATE_LISTEN,
            MIB_TCP_STATE_SYN_SENT,
            MIB_TCP_STATE_SYN_RCVD,
            MIB_TCP_STATE_ESTAB,
            MIB_TCP_STATE_FIN_WAIT1,
            MIB_TCP_STATE_FIN_WAIT2,
            MIB_TCP_STATE_CLOSE_WAIT,
            MIB_TCP_STATE_CLOSING,
            MIB_TCP_STATE_LAST_ACK,
            MIB_TCP_STATE_TIME_WAIT,
            MIB_TCP_STATE_DELETE_TCB
        }

        public enum MUTANT_INFORMATION_CLASS : int
        {
            MutantBasicInformation
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

        public enum PROCESS_INFORMATION_CLASS : int
        {
            ProcessBasicInformation,
            ProcessQuotaLimits,
            ProcessIoCounters,
            ProcessVmCounters,
            ProcessTimes,
            ProcessBasePriority,
            ProcessRaisePriority,
            ProcessDebugPort,
            ProcessExceptionPort,
            ProcessAccessToken,
            ProcessLdtInformation,
            ProcessLdtSize,
            ProcessDefaultHardErrorMode,
            ProcessIoPortHandlers,
            ProcessPooledUsageAndLimits,
            ProcessWorkingSetWatch,
            ProcessUserModeIOPL,
            ProcessEnableAlignmentFaultFixup,
            ProcessPriorityClass,
            ProcessWx86Information,
            ProcessHandleCount,
            ProcessAffinityMask,
            ProcessPriorityBoost,
            ProcessDeviceMap,
            ProcessSessionInformation,
            ProcessForegroundInformation,
            ProcessWow64Information,
            ProcessImageFileName,
            ProcessLUIDDeviceMapsEnabled,
            ProcessBreakOnTermination,
            ProcessDebugObjectHandle,
            ProcessDebugFlags,
            ProcessHandleTracing,
            ProcessIoPriority,
            ProcessExecuteFlags,
            ProcessResourceManagement,
            ProcessCookie,
            ProcessImageInformation, 
            MaxProcessInfoClass
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

        [Flags]
        public enum SECTION_ATTRIBUTES : int
        {
            SEC_FILE = 0x800000,
            SEC_IMAGE = 0x1000000,
            SEC_RESERVE = 0x4000000
        }

        [Flags]
        public enum SECTION_INFORMATION_CLASS : int
        {
            SectionBasicInformation,
            SectionImageInformation
        }

        [Flags]
        public enum SECTION_RIGHTS : uint
        {
            SECTION_QUERY = 0x0001,
            SECTION_MAP_WRITE = 0x0002,
            SECTION_MAP_READ = 0x0004,
            SECTION_MAP_EXECUTE = 0x0008,
            SECTION_EXTEND_SIZE = 0x0010,
            SECTION_MAP_EXECUTE_EXPLICIT = 0x0020,

            SECTION_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED | SECTION_QUERY | 
                SECTION_MAP_WRITE | SECTION_MAP_READ | SECTION_MAP_EXECUTE |
                SECTION_EXTEND_SIZE
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
        public enum SYNC_RIGHTS : int
        {
            EVENT_ALL_ACCESS = 0x1f0003,
            EVENT_MODIFY_STATE = 0x0002,
            EVENT_QUERY_STATE = 0x0001,
            MUTEX_ALL_ACCESS = 0x1f0001,
            MUTEX_MODIFY_STATE = 0x0001,
            SEMAPHORE_ALL_ACCESS = 0x1f0003,
            SEMAPHORE_MODIFY_STATE = 0x0002,
            TIMER_ALL_ACCESS = 0x1f0003,
            TIMER_MODIFY_STATE = 0x0002,
            TIMER_QUERY_STATE = 0x0001
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

        public enum THREAD_INFORMATION_CLASS
        {
            ThreadBasicInformation,
            ThreadTimes,
            ThreadPriority,
            ThreadBasePriority,
            ThreadAffinityMask,
            ThreadImpersonationToken,
            ThreadDescriptorTableEntry,
            ThreadEnableAlignmentFaultFixup,
            ThreadEventPair,
            ThreadQuerySetWin32StartAddress,
            ThreadZeroTlsCell,
            ThreadPerformanceCount,
            ThreadAmILastThread,
            ThreadIdealProcessor,
            ThreadPriorityBoost,
            ThreadSetTlsArrayAddress,
            ThreadIsIoPending,
            ThreadHideFromDebugger
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

        public enum TOKEN_ELEVATION_TYPE : int
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
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
    }
}
