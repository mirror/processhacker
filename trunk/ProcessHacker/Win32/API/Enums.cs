/*
 * Process Hacker - 
 *   windows API enums
 * 
 * Copyright (C) 2009 Uday Shanbhag
 * Copyright (C) 2009 Dean
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

        [Flags]
        public enum CreationFlags : uint
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
        public enum ExitWindowsFlags : int
        {
            Logoff = 0x0,
            Poweroff = 0x8,
            Reboot = 0x2,
            RestartApps = 0x40,
            Shutdown = 0x1,
            Force = 0x4,
            ForceIfHung = 0x10
        }

        public enum FILE_CREATION_DISPOSITION : uint
        {
            CreateNew = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting
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
        public enum FILE_RIGHTS : uint
        {
            FILE_READ_DATA = 0x0001,
            FILE_LIST_DIRECTORY = 0x0001,
            FILE_WRITE_DATA = 0x0002,
            FILE_ADD_FILE = 0x0002,
            FILE_APPEND_DATA = 0x0004,
            FILE_ADD_SUBDIRECTORY = 0x0004,
            FILE_CREATE_PIPE_INSTANCE = 0x0004,
            FILE_READ_EA = 0x0008,
            FILE_WRITE_EA = 0x0010,
            FILE_EXECUTE = 0x0020,
            FILE_TRAVERSE = 0x0020,
            FILE_DELETE_CHILD = 0x0040,
            FILE_READ_ATTRIBUTES = 0x0080,
            FILE_WRITE_ATTRIBUTES = 0x0100,
            FILE_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED | STANDARD_RIGHTS.SYNCHRONIZE | 0x1ff,
            FILE_GENERIC_READ = STANDARD_RIGHTS.STANDARD_RIGHTS_READ | FILE_READ_DATA |
                FILE_READ_ATTRIBUTES | FILE_READ_EA | STANDARD_RIGHTS.SYNCHRONIZE,
            FILE_GENERIC_WRITE = STANDARD_RIGHTS.STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA |
                FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA |
                STANDARD_RIGHTS.SYNCHRONIZE,
            FILE_GENERIC_EXECUTE = STANDARD_RIGHTS.STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES |
                FILE_EXECUTE | STANDARD_RIGHTS.SYNCHRONIZE
        }

        [Flags]
        public enum FILE_SHARE_MODE : uint
        {
            Exclusive = 0,
            Read = 1,
            Write = 2,
            Delete = 4
        }

        [Flags]
        public enum HANDLE_FLAGS : byte
        {
            Inherit = 0x1,
            ProtectFromClose = 0x2
        }

        [Flags]
        public enum HEAPENTRY32FLAGS : int
        {
            LF32_FIXED = 0x00000001,
            LF32_FREE = 0x00000002,
            LF32_MOVEABLE = 0x00000004
        }

        [Flags]
        public enum JOB_OBJECT_INFORMATION_CLASS : int
        {
            JobObjectBasicAccountingInformation = 1,
            JobObjectBasicLimitInformation = 2,
            JobObjectBasicProcessIdList = 3,
            JobObjectBasicUIRestrictions = 4,
            JobObjectSecurityLimitInformation = 5,
            JobObjectBasicAndIoAccountingInformation = 8,
            JobObjectExtendedLimitInformation = 9,
            JobObjectGroupInformation = 11
        }

        [Flags]
        public enum JOB_OBJECT_LIMIT_FLAGS : uint
        {
            JOB_OBJECT_LIMIT_WORKINGSET = 0x1,
            JOB_OBJECT_LIMIT_PROCESS_TIME = 0x2,
            JOB_OBJECT_LIMIT_JOB_TIME = 0x4,
            JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 0x8,
            JOB_OBJECT_LIMIT_AFFINITY = 0x10,
            JOB_OBJECT_LIMIT_PRIORITY_CLASS = 0x20, 
            JOB_OBJECT_LIMIT_PRESERVE_JOB_TIME = 0x40,
            JOB_OBJECT_LIMIT_SCHEDULING_CLASS = 0x80,
            JOB_OBJECT_LIMIT_PROCESS_MEMORY = 0x100, 
            JOB_OBJECT_LIMIT_JOB_MEMORY = 0x200,
            JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION = 0x400,
            JOB_OBJECT_LIMIT_BREAKAWAY_OK = 0x800,
            JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK = 0x1000,
            JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000,
        }

        [Flags]
        public enum JOB_OBJECT_RIGHTS : uint
        {
            JOB_OBJECT_ASSIGN_PROCESS = 0x0001,
            JOB_OBJECT_SET_ATTRIBUTES = 0x0002,
            JOB_OBJECT_QUERY = 0x0004,
            JOB_OBJECT_TERMINATE = 0x0008,
            JOB_OBJECT_SET_SECURITY_ATTRIBUTES = 0x0010,
            JOB_OBJECT_ALL_ACCESS = STANDARD_RIGHTS.STANDARD_RIGHTS_REQUIRED | STANDARD_RIGHTS.SYNCHRONIZE | 0x1f
        }

        [Flags]
        public enum JOB_OBJECT_BASIC_UI_RESTRICTIONS : uint
        {
            JOB_OBJECT_UILIMIT_HANDLES = 0x1,
            JOB_OBJECT_UILIMIT_READCLIPBOARD = 0x2,
            JOB_OBJECT_UILIMIT_WRITECLIPBOARD = 0x4, 
            JOB_OBJECT_UILIMIT_SYSTEMPARAMETERS = 0x8,
            JOB_OBJECT_UILIMIT_DISPLAYSETTINGS = 0x10,
            JOB_OBJECT_UILIMIT_GLOBALATOMS = 0x20,
            JOB_OBJECT_UILIMIT_DESKTOP = 0x40,
            JOB_OBJECT_UILIMIT_EXITWINDOWS = 0x80
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
            Closed = 1,
            Listening,
            SynSent,
            SynReceived,
            Established,
            FinWait1,
            FinWait2,
            CloseWait,
            Closing,
            LastAck,
            TimeWait,
            DeleteTcb
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

        public enum PeekMessageFlags : int
        {
            NoRemove = 0,
            Remove = 1,
            NoYield = 2,
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
            ProcessCycleTime,
            ProcessPagePriority,
            ProcessInstrumentationCallback,
            ProcessThreadStackAllocation,
            ProcessWorkingSetWatchEx,
            ProcessImageFileNameWin32,
            ProcessImageFileMapping,
            ProcessAffinityUpdateMode,
            ProcessMemoryAllocationMode,
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

        public enum SERVICE_INFO_LEVEL : int
        {
            Description = 1,
            FailureActions = 2,
            DelayedAutoStartInfo = 3,
            FailureActionsFlag = 4,
            SidInfo = 5,
            RequiredPrivilegesInfo = 6,
            PreShutdownInfo = 7,
            TriggerInfo = 8,
            PreferredNode = 9
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

        public enum ShowWindowType : short
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
        public enum SmtoFlags : int
        {
            Normal = 0x0,
            Block = 0x1,
            AbortIfHung = 0x2,
            NoTimeoutIfNotHung = 0x8,
            ErrorOnExit = 0x20
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
        public enum StartupFlags : uint
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
            SystemFileCacheInformation,
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

        public enum TCP_TABLE_CLASS : int
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
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
            ThreadHideFromDebugger,
            ThreadBreakOnTermination,
            ThreadSwitchLegacyState,
            ThreadIsTerminated,
            ThreadLastSystemCall,
            ThreadIoPriority,
            ThreadCycleTime,
            ThreadPagePriority,
            ThreadActualBasePriority,
            ThreadTebInformation,
            ThreadCSwitchMon,
            MaxThreadInfoClass
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

        public enum UipiFilterFlag : int
        {
            Add = 1,
            Remove = 2
        }

        public enum UDP_TABLE_CLASS : int
        {
            UDP_TABLE_BASIC,
            UDP_TABLE_OWNER_PID,
            UDP_TABLE_OWNER_MODULE
        }

        public enum WaitResult : int
        {
            Object0 = 0x0,
            Abandoned = 0x80,
            Timeout = 0x102
        }

        public enum WindowMessage : uint
        {
            Null = 0x00,
            Create = 0x01,
            Destroy = 0x02,
            Move = 0x03,
            Size = 0x05,
            Activate = 0x06,
            SetFocus = 0x07,
            KillFocus = 0x08,
            Enable = 0x0a,
            SetRedraw = 0x0b,
            SetText = 0x0c,
            GetText = 0x0d,
            GetTextLength = 0x0e,
            Paint = 0x0f,
            Close = 0x10,
            QueryEndSession = 0x11,
            Quit = 0x12,
            QueryOpen = 0x13,
            EraseBkgnd = 0x14,
            SysColorChange = 0x15,
            EndSession = 0x16,
            SystemError = 0x17,
            ShowWindow = 0x18,
            CtlColor = 0x19,
            WinIniChange = 0x1a,
            SettingChange = 0x1a,
            DevModeChange = 0x1b,
            ActivateApp = 0x1c,
            FontChange = 0x1d,
            TimeChange = 0x1e,
            CancelMode = 0x1f,
            SetCursor = 0x20,
            MouseActivate = 0x21,
            ChildActivate = 0x22,
            QueueSync = 0x23,
            GetMinMaxInfo = 0x24,
            PaintIcon = 0x26,
            IconEraseBkgnd = 0x27,
            NextDlgCtl = 0x28,
            SpoolerStatus = 0x2a,
            DrawIcon = 0x2b,
            MeasureItem = 0x2c,
            DeleteItem = 0x2d,
            VKeyToItem = 0x2e,
            CharToItem = 0x2f,

            SetFont = 0x30,
            GetFont = 0x31,
            SetHotkey = 0x32,
            GetHotkey = 0x33,
            QueryDragIcon = 0x37,
            CompareItem = 0x39,
            Compacting = 0x41,
            WindowPosChanging = 0x46,
            WindowPosChanged = 0x47,
            Power = 0x48,
            CopyData = 0x4a,
            CancelJournal = 0x4b,
            Notify = 0x4e,
            InputLangChangeRequest = 0x50,
            InputLangChange = 0x51,
            TCard = 0x52,
            Help = 0x53,
            UserChanged = 0x54,
            NotifyFormat = 0x55,
            ContextMenu = 0x7b,
            StyleChanging = 0x7c,
            StyleChanged = 0x7d,
            DisplayChange = 0x7e,
            GetIcon = 0x7f,
            SetIcon = 0x80,

            NcCreate = 0x81,
            NcDestroy = 0x82,
            NcCalcSize = 0x83,
            NcHitTest = 0x84,
            NcPaint = 0x85,
            NcActivate = 0x86,
            GetDlgCode = 0x87,
            NcMouseMove = 0xa0,
            NcLButtonDown = 0xa1,
            NcLButtonUp = 0xa2,
            NcLButtonDblClk = 0xa3,
            NcRButtonDown = 0xa4,
            NcRButtonUp = 0xa5,
            NcRButtonDblClk = 0xa6,
            NcMButtonDown = 0xa7,
            NcMButtonUp = 0xa8,
            NcMButtonDblClk = 0xa9,

            KeyDown = 0x100,
            KeyUp = 0x101,
            Char = 0x102,
            DeadChar = 0x103,
            SysKeyDown = 0x104,
            SysKeyUp = 0x105,
            SysChar = 0x106,
            SysDeadChar = 0x107,

            ImeStartComposition = 0x10d,
            ImeEndComposition = 0x10e,
            ImeComposition = 0x10f,
            ImeKeyLast = 0x10f,

            InitDialog = 0x110,
            Command = 0x111,
            SysCommand = 0x112,
            Timer = 0x113,
            HScroll = 0x114,
            VScroll = 0x115,
            InitMenu = 0x116,
            InitMenuPopup = 0x117,
            MenuSelect = 0x11f,
            MenuChar = 0x120,
            EnterIdle = 0x121,

            CtlColorMsgBox = 0x132,
            CtlColorEdit = 0x133,
            CtlColorListBox = 0x134,
            CtlColorBtn = 0x135,
            CtlColorDlg = 0x136,
            CtlColorScrollbar = 0x137,
            CtlColorStatic = 0x138,

            MouseMove = 0x200,
            LButtonDown = 0x201,
            LButtonUp = 0x202,
            LButtonDblClk = 0x203,
            RButtonDown = 0x204,
            RButtonUp = 0x205,
            RButtonDblClk = 0x206,
            MButtonDown = 0x207,
            MButtonUp = 0x208,
            MButtonDblClk = 0x209,
            MouseWheel = 0x20a,

            ParentNotify = 0x210,
            EnterMenuLoop = 0x211,
            ExitMenuLoop = 0x212,
            NextMenu = 0x213,
            Sizing = 0x214,
            CaptureChanged = 0x215,
            Moving = 0x216,
            PowerBroadcast = 0x218,
            DeviceChange = 0x219,

            MdiCreate = 0x220,
            MdiDestroy = 0x221,
            MdiActivate = 0x222,
            MdiRestore = 0x223,
            MdiNext = 0x224,
            MdiMaximize = 0x225,
            MdiTile = 0x226,
            MdiCascade = 0x227,
            MdiIconArrange = 0x228,
            MdiGetActive = 0x229,
            MdiSetMenu = 0x230,
            EnterSizeMove = 0x231,
            ExitSizeMove = 0x232,
            DropFiles = 0x233,
            MdiRefreshMenu = 0x234,

            ImeSetContext = 0x281,
            ImeNotify = 0x282,
            ImeControl = 0x283,
            ImeCompositionFull = 0x284,
            ImeSelect = 0x285,
            ImeChar = 0x286,
            ImeKeyDown = 0x290,
            ImeKeyUp = 0x291,

            MouseHover = 0x2a1,
            NcMouseLeave = 0x2a2,
            MouseLeave = 0x2a3,

            Cut = 0x300,
            Copy = 0x301,
            Paste = 0x302,
            Clear = 0x303,
            Undo = 0x304,

            RenderFormat = 0x305,
            RenderAllFormats = 0x306,
            DestroyClipboard = 0x307,
            DrawClipboard = 0x308,
            PaintClipboard = 0x309,
            VScrollClipboard = 0x30a,
            SizeClipboard = 0x30b,
            AskCbFormatName = 0x30c,
            ChangeCbChain = 0x30d,
            HScrollClipboard = 0x30e,
            QueryNewPalette = 0x30f,
            PaletteIsChanging = 0x310,
            PaletteChanged = 0x311,

            Hotkey = 0x312,
            Print = 0x317,
            PrintClient = 0x318,

            HandheldFirst = 0x358,
            HandheldLast = 0x35f,
            PenWinFirst = 0x380,
            PenWinLast = 0x38f,
            CoalesceFirst = 0x390,
            CoalesceLast = 0x39f,
            DdeInitiate = 0x3e0,
            DdeTerminate = 0x3e1,
            DdeAdvise = 0x3e2,
            DdeUnadvise = 0x3e3,
            DdeAck = 0x3e4,
            DdeData = 0x3e5,
            DdeRequest = 0x3e6,
            DdePoke = 0x3e7,
            DdeExecute = 0x3e8,
            
            User = 0x400,

            BcmSetShield = 0x160c,

            App = 0x8000
        }

        public enum WindowStyles : uint
        {
            Overlapped = 0x00000000,
            Popup = 0x80000000,
            Child = 0x40000000,
            Minimize = 0x20000000,
            Visible = 0x10000000,
            Disabled = 0x08000000,
            ClipSiblings = 0x04000000,
            ClipChildren = 0x02000000,
            Maximize = 0x01000000,
            Caption = 0x00C00000, /* WindowStyles.Border | WindowStyles.DialogFrame */
            Border = 0x00800000,
            DialogFrame = 0x00400000,
            VerticalScroll = 0x00200000,
            HorizontalScroll = 0x00100000,
            SystemMenu = 0x00080000,
            ThickFrame = 0x00040000,
            Group = 0x00020000,
            TabStop = 0x00010000,
            MinimizeBox = 0x00020000,
            MaximizeBox = 0x00010000
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
