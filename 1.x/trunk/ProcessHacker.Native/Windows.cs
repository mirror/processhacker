/*
 * Process Hacker - 
 *   system-related functions
 *
 * Copyright (C) 2009 Flavio Erlich
 * Copyright (C) 2009 wj32
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

// 'member' is obsolete: 'text'
#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using ProcessHacker.Common;
using ProcessHacker.Common.Threading;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Provides methods for manipulating the operating system.
    /// </summary>
    public static class Windows
    {
        public delegate bool EnumKernelModulesDelegate(KernelModule kernelModule);

        public delegate string GetProcessNameCallback(int pid);

        public static GetProcessNameCallback GetProcessName;

        /// <summary>
        /// A cache for type names; QuerySystemInformation with ALL_TYPES_INFORMATION fails for some 
        /// reason. The dictionary relates object type numbers to their names.
        /// </summary>
        internal static Dictionary<byte, string> ObjectTypes = new Dictionary<byte, string>();
        internal static FastResourceLock ObjectTypesLock = new FastResourceLock();

        [ThreadStatic]
        private static MemoryAlloc _handlesBuffer;
        [ThreadStatic]
        private static MemoryAlloc _kernelModulesBuffer;
        [ThreadStatic]
        private static MemoryAlloc _processesBuffer;
        [ThreadStatic]
        private static MemoryAlloc _servicesBuffer;

        private static int _numberOfProcessors = 0;
        private static int _pageSize = 0;
        private static IntPtr _kernelBase = IntPtr.Zero;
        private static string _kernelFileName = null;

        /// <summary>
        /// Gets the number of active processors.
        /// </summary>
        public static int NumberOfProcessors
        {
            get
            {
                if (_numberOfProcessors == 0)
                    _numberOfProcessors = GetBasicInformation().NumberOfProcessors;

                return _numberOfProcessors;
            }
        }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public static int PageSize
        {
            get
            {
                if (_pageSize == 0)
                    _pageSize = GetBasicInformation().PageSize;

                return _pageSize;
            }
        }

        /// <summary>
        /// Gets the base address of the kernel.
        /// </summary>
        public static IntPtr KernelBase
        {
            get
            {
                if (_kernelBase == IntPtr.Zero)
                    _kernelBase = GetKernelBase();

                return _kernelBase;
            }
        }

        /// <summary>
        /// Gets the file name of the kernel.
        /// </summary>
        public static string KernelFileName
        {
            get
            {
                if (_kernelFileName == null)
                    _kernelFileName = GetKernelFileName();

                return _kernelFileName;
            }
        }

        /// <summary>
        /// Gets the number of pages needed to store the 
        /// specified number of bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes.</param>
        /// <returns>The number of pages needed.</returns>
        public static int BytesToPages(int bytes)
        {
            return Utils.DivideUp(bytes, PageSize);
        }

        /// <summary>
        /// Enumerates the modules loaded by the kernel.
        /// </summary>
        /// <param name="enumCallback">A callback for the enumeration.</param>
        public static void EnumKernelModules(EnumKernelModulesDelegate enumCallback)
        {
            NtStatus status;
            int retLength;

            if (_kernelModulesBuffer == null)
                _kernelModulesBuffer = new MemoryAlloc(0x1000);

            status = Win32.NtQuerySystemInformation(
                SystemInformationClass.SystemModuleInformation,
                _kernelModulesBuffer,
                _kernelModulesBuffer.Size,
                out retLength
                );

            if (status == NtStatus.InfoLengthMismatch)
            {
                _kernelModulesBuffer.ResizeNew(retLength);

                status = Win32.NtQuerySystemInformation(
                    SystemInformationClass.SystemModuleInformation,
                    _kernelModulesBuffer,
                    _kernelModulesBuffer.Size,
                    out retLength
                    );
            }

            if (status >= NtStatus.Error)
                Win32.Throw(status);

            RtlProcessModules modules = _kernelModulesBuffer.ReadStruct<RtlProcessModules>();

            for (int i = 0; i < modules.NumberOfModules; i++)
            {
                var module = _kernelModulesBuffer.ReadStruct<RtlProcessModuleInformation>(RtlProcessModules.ModulesOffset, i);
                var moduleInfo = new Debugging.ModuleInformation(module);

                if (!enumCallback(new KernelModule(
                    moduleInfo.BaseAddress,
                    moduleInfo.Size,
                    moduleInfo.Flags,
                    moduleInfo.BaseName,
                    FileUtils.GetFileName(moduleInfo.FileName)
                    )))
                    break;
            }
        }

        /// <summary>
        /// Gets basic information about the system.
        /// </summary>
        /// <returns>A structure containing basic information.</returns>
        public static SystemBasicInformation GetBasicInformation()
        {
            NtStatus status;
            SystemBasicInformation sbi;
            int retLength;

            if ((status = Win32.NtQuerySystemInformation(
                SystemInformationClass.SystemBasicInformation,
                out sbi,
                Marshal.SizeOf(typeof(SystemBasicInformation)),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return sbi;
        }

        /// <summary>
        /// Enumerates the handles opened by every running process.
        /// </summary>
        /// <returns>An array containing information about the handles.</returns>
        public static SystemHandleEntry[] GetHandles()
        {
            int retLength = 0;
            int handleCount = 0;
            SystemHandleEntry[] returnHandles;

            if (_handlesBuffer == null)
                _handlesBuffer = new MemoryAlloc(0x1000);

            MemoryAlloc data = _handlesBuffer;

            NtStatus status;

            // This is needed because NtQuerySystemInformation with SystemHandleInformation doesn't 
            // actually give a real return length when called with an insufficient buffer. This code 
            // tries repeatedly to call the function, doubling the buffer size each time it fails.
            while ((status = Win32.NtQuerySystemInformation(
                SystemInformationClass.SystemHandleInformation,
                data,
                data.Size,
                out retLength)
                ) == NtStatus.InfoLengthMismatch)
            {
                data.ResizeNew(data.Size * 2);

                // Fail if we've resized it to over 16MB - protect from infinite resizing
                if (data.Size > 16 * 1024 * 1024)
                    throw new OutOfMemoryException();
            }

            if (status >= NtStatus.Error)
                Win32.Throw(status);

            // The structure of the buffer is the handle count plus an array of SYSTEM_HANDLE_INFORMATION 
            // structures.
            handleCount = data.ReadStruct<SystemHandleInformation>().NumberOfHandles;
            returnHandles = new SystemHandleEntry[handleCount];

            // Unsafe code for speed.
            unsafe
            {
                SystemHandleEntry* handlesPtr = (SystemHandleEntry*)((byte*)data.Memory + SystemHandleInformation.HandlesOffset);

                for (int i = 0; i < handleCount; i++)
                {
                    //returnHandles[i] = data.ReadStruct<SystemHandleEntry>(SystemHandleInformation.HandlesOffset, i);
                    returnHandles[i] = handlesPtr[i];
                }
            }

            return returnHandles;
        }

        /// <summary>
        /// Gets the base address of the currently running kernel.
        /// </summary>
        /// <returns>The kernel's base address.</returns>
        private static IntPtr GetKernelBase()
        {
            IntPtr kernelBase = IntPtr.Zero;

            Windows.EnumKernelModules((module) =>
            {
                kernelBase = module.BaseAddress;
                return false;
            });

            return kernelBase;
        }

        /// <summary>
        /// Gets the file name of the currently running kernel.
        /// </summary>
        /// <returns>The kernel file name.</returns>
        private static string GetKernelFileName()
        {
            string kernelFileName = null;

            EnumKernelModules((module) =>
            {
                kernelFileName = module.FileName;
                return false;
            });

            return kernelFileName;
        }

        /// <summary>
        /// Gets the modules loaded by the kernel.
        /// </summary>
        /// <returns>A collection of module information structures.</returns>
        public static KernelModule[] GetKernelModules()
        {
            List<KernelModule> kernelModules = new List<KernelModule>();

            EnumKernelModules((kernelModule) =>
            {
                kernelModules.Add(kernelModule);
                return true;
            });

            return kernelModules.ToArray();
        }

        public static SystemLogonSession GetLogonSession(Luid logonId)
        {
            NtStatus status;
            IntPtr logonSessionData;

            if ((status = Win32.LsaGetLogonSessionData(
                ref logonId,
                out logonSessionData
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var logonSessionDataAlloc = new LsaMemoryAlloc(logonSessionData, true))
            {
                var info = logonSessionDataAlloc.ReadStruct<SecurityLogonSessionData>();

                return new SystemLogonSession(
                    info.AuthenticationPackage.Read(),
                    info.DnsDomainName.Read(),
                    info.LogonDomain.Read(),
                    info.LogonId,
                    info.LogonServer.Read(),
                    DateTime.FromFileTime(info.LogonTime),
                    info.LogonType,
                    info.Session,
                    new Sid(info.Sid),
                    info.Upn.Read(),
                    info.UserName.Read()
                    );
            }
        }

        public static Luid[] GetLogonSessions()
        {
            NtStatus status;
            int logonSessionCount;
            IntPtr logonSessionList;

            if ((status = Win32.LsaEnumerateLogonSessions(
                out logonSessionCount,
                out logonSessionList
                )) >= NtStatus.Error)
                Win32.Throw(status);

            Luid[] logonSessions = new Luid[logonSessionCount];

            using (var logonSessionListAlloc = new LsaMemoryAlloc(logonSessionList, true))
            {
                for (int i = 0; i < logonSessionCount; i++)
                    logonSessions[i] = logonSessionListAlloc.ReadStruct<Luid>(i);

                return logonSessions;
            }
        }

        /// <summary>
        /// Gets the network connections currently active.
        /// </summary>
        /// <returns>A dictionary of network connections.</returns>
        public static Dictionary<int, List<NetworkConnection>> GetNetworkConnections()
        {
            var retDict = new Dictionary<int, List<NetworkConnection>>();
            int length;

            // TCP IPv4

            length = 0;
            Win32.GetExtendedTcpTable(IntPtr.Zero, ref length, false, AiFamily.INet, TcpTableClass.OwnerPidAll, 0);

            using (var mem = new MemoryAlloc(length))
            {
                if (Win32.GetExtendedTcpTable(mem, ref length, false, AiFamily.INet, TcpTableClass.OwnerPidAll, 0) != 0)
                    Win32.Throw();

                int count = mem.ReadInt32(0);

                for (int i = 0; i < count; i++)
                {
                    var struc = mem.ReadStruct<MibTcpRowOwnerPid>(sizeof(int), i);

                    if (!retDict.ContainsKey(struc.OwningProcessId))
                        retDict.Add(struc.OwningProcessId, new List<NetworkConnection>());

                    retDict[struc.OwningProcessId].Add(new NetworkConnection()
                    {
                        Protocol = NetworkProtocol.Tcp,
                        Local = new IPEndPoint(struc.LocalAddress, ((ushort)struc.LocalPort).Reverse()),
                        Remote = new IPEndPoint(struc.RemoteAddress, ((ushort)struc.RemotePort).Reverse()),
                        State = struc.State,
                        Pid = struc.OwningProcessId
                    });
                }
            }

            // UDP IPv4

            length = 0;
            Win32.GetExtendedUdpTable(IntPtr.Zero, ref length, false, AiFamily.INet, UdpTableClass.OwnerPid, 0);

            using (var mem = new MemoryAlloc(length))
            {
                if (Win32.GetExtendedUdpTable(mem, ref length, false, AiFamily.INet, UdpTableClass.OwnerPid, 0) != 0)
                    Win32.Throw();

                int count = mem.ReadInt32(0);

                for (int i = 0; i < count; i++)
                {
                    var struc = mem.ReadStruct<MibUdpRowOwnerPid>(sizeof(int), i);

                    if (!retDict.ContainsKey(struc.OwningProcessId))
                        retDict.Add(struc.OwningProcessId, new List<NetworkConnection>());

                    retDict[struc.OwningProcessId].Add(
                        new NetworkConnection()
                        {
                            Protocol = NetworkProtocol.Udp,
                            Local = new IPEndPoint(struc.LocalAddress, ((ushort)struc.LocalPort).Reverse()),
                            Pid = struc.OwningProcessId
                        });
                }
            }

            // TCP IPv6

            length = 0;
            Win32.GetExtendedTcpTable(IntPtr.Zero, ref length, false, AiFamily.INet6, TcpTableClass.OwnerPidAll, 0);

            using (var mem = new MemoryAlloc(length))
            {
                if (Win32.GetExtendedTcpTable(mem, ref length, false, AiFamily.INet6, TcpTableClass.OwnerPidAll, 0) == 0)
                {
                    int count = mem.ReadInt32(0);

                    for (int i = 0; i < count; i++)
                    {
                        var struc = mem.ReadStruct<MibTcp6RowOwnerPid>(sizeof(int), i);

                        if (!retDict.ContainsKey(struc.OwningProcessId))
                            retDict.Add(struc.OwningProcessId, new List<NetworkConnection>());

                        retDict[struc.OwningProcessId].Add(new NetworkConnection()
                        {
                            Protocol = NetworkProtocol.Tcp6,
                            Local = new IPEndPoint(new IPAddress(struc.LocalAddress, struc.LocalScopeId), ((ushort)struc.LocalPort).Reverse()),
                            Remote = new IPEndPoint(new IPAddress(struc.RemoteAddress, struc.RemoteScopeId), ((ushort)struc.RemotePort).Reverse()),
                            State = struc.State,
                            Pid = struc.OwningProcessId
                        });
                    }
                }
            }

            // UDP IPv6

            length = 0;
            Win32.GetExtendedUdpTable(IntPtr.Zero, ref length, false, AiFamily.INet6, UdpTableClass.OwnerPid, 0);

            using (var mem = new MemoryAlloc(length))
            {
                if (Win32.GetExtendedUdpTable(mem, ref length, false, AiFamily.INet6, UdpTableClass.OwnerPid, 0) == 0)
                {
                    int count = mem.ReadInt32(0);

                    for (int i = 0; i < count; i++)
                    {
                        var struc = mem.ReadStruct<MibUdp6RowOwnerPid>(sizeof(int), i);

                        if (!retDict.ContainsKey(struc.OwningProcessId))
                            retDict.Add(struc.OwningProcessId, new List<NetworkConnection>());

                        retDict[struc.OwningProcessId].Add(
                            new NetworkConnection()
                            {
                                Protocol = NetworkProtocol.Udp6,
                                Local = new IPEndPoint(new IPAddress(struc.LocalAddress, struc.LocalScopeId), ((ushort)struc.LocalPort).Reverse()),
                                Pid = struc.OwningProcessId
                            });
                    }
                }
            }

            return retDict;
        }

        /// <summary>
        /// Gets the page files currently active.
        /// </summary>
        /// <returns>A collection of page file information structures.</returns>
        public static SystemPagefile[] GetPagefiles()
        {
            int retLength;
            List<SystemPagefile> pagefiles = new List<SystemPagefile>();

            using (MemoryAlloc data = new MemoryAlloc(0x200))
            {
                NtStatus status;

                while ((status = Win32.NtQuerySystemInformation(
                    SystemInformationClass.SystemPageFileInformation,
                    data,
                    data.Size,
                    out retLength)
                    ) == NtStatus.InfoLengthMismatch)
                {
                    data.ResizeNew(data.Size * 2);

                    // Fail if we've resized it to over 16MB - protect from infinite resizing
                    if (data.Size > 16 * 1024 * 1024)
                        throw new OutOfMemoryException();
                }

                if (status >= NtStatus.Error)
                    Win32.Throw(status);

                pagefiles = new List<SystemPagefile>(2);

                int i = 0;
                SystemPagefileInformation currentPagefile;

                do
                {
                    currentPagefile = data.ReadStruct<SystemPagefileInformation>(i, 0);

                    pagefiles.Add(new SystemPagefile(
                        currentPagefile.TotalSize,
                        currentPagefile.TotalInUse,
                        currentPagefile.PeakUsage,
                        FileUtils.GetFileName(currentPagefile.PageFileName.Read())
                        ));

                    i += currentPagefile.NextEntryOffset;
                } while (currentPagefile.NextEntryOffset != 0);

                return pagefiles.ToArray();
            }
        }

        /// <summary>
        /// Gets a dictionary containing the currently running processes.
        /// </summary>
        /// <returns>A dictionary, indexed by process ID.</returns>
        public static Dictionary<int, SystemProcess> GetProcesses()
        {
            return GetProcesses(false);
        }

        /// <summary>
        /// Gets a dictionary containing the currently running processes.
        /// </summary>
        /// <param name="getThreads">Whether to get thread information.</param>
        /// <returns>A dictionary, indexed by process ID.</returns>
        public static Dictionary<int, SystemProcess> GetProcesses(bool getThreads)
        {
            int retLength;
            Dictionary<int, SystemProcess> returnProcesses;

            if (_processesBuffer == null)
                _processesBuffer = new MemoryAlloc(0x10000);

            MemoryAlloc data = _processesBuffer;

            NtStatus status;
            int attempts = 0;

            while (true)
            {
                attempts++;

                if ((status = Win32.NtQuerySystemInformation(
                    SystemInformationClass.SystemProcessInformation,
                    data,
                    data.Size,
                    out retLength
                    )) >= NtStatus.Error)
                {
                    if (attempts > 3)
                        Win32.Throw(status);

                    data.ResizeNew(retLength);
                }
                else
                {
                    break;
                }
            }

            returnProcesses = new Dictionary<int, SystemProcess>(32); // 32 processes on a computer?

            int i = 0;
            SystemProcess currentProcess = new SystemProcess();

            do
            {
                //currentProcess.Process = data.ReadStruct<SystemProcessInformation>(i, 0);
                unsafe
                {
                    currentProcess.Process = *(SystemProcessInformation*)((byte*)data.Memory + i);
                }

                currentProcess.Name = currentProcess.Process.ImageName.Read();

                if (getThreads &&
                    currentProcess.Process.ProcessId != 0)
                {
                    currentProcess.Threads = new Dictionary<int, SystemThreadInformation>();

                    for (int j = 0; j < currentProcess.Process.NumberOfThreads; j++)
                    {
                        var thread = data.ReadStruct<SystemThreadInformation>(i +
                            Marshal.SizeOf(typeof(SystemProcessInformation)), j);

                        currentProcess.Threads.Add(thread.ClientId.ThreadId, thread);
                    }
                }

                returnProcesses.Add(currentProcess.Process.ProcessId, currentProcess);

                i += currentProcess.Process.NextEntryOffset;
            } while (currentProcess.Process.NextEntryOffset != 0);

            return returnProcesses;
        }

        /// <summary>
        /// Gets a dictionary containing the threads owned by the specified process.
        /// </summary>
        /// <param name="pid">A process ID.</param>
        /// <returns>A dictionary, indexed by thread ID.</returns>
        public static Dictionary<int, SystemThreadInformation> GetProcessThreads(int pid)
        {
            int retLength;

            if (_processesBuffer == null)
                _processesBuffer = new MemoryAlloc(0x10000);

            MemoryAlloc data = _processesBuffer;

            NtStatus status;
            int attempts = 0;

            while (true)
            {
                attempts++;

                if ((status = Win32.NtQuerySystemInformation(SystemInformationClass.SystemProcessInformation, data.Memory,
                    data.Size, out retLength)) >= NtStatus.Error)
                {
                    if (attempts > 3)
                        Win32.Throw(status);

                    data.ResizeNew(retLength);
                }
                else
                {
                    break;
                }
            }

            int i = 0;
            SystemProcessInformation process;

            do
            {
                unsafe
                {
                    //process = data.ReadStruct<SystemProcessInformation>(i, 0);
                    process = *(SystemProcessInformation*)((byte*)data.Memory + i);
                }

                if (process.ProcessId == pid)
                {
                    var threads = new Dictionary<int, SystemThreadInformation>();

                    for (int j = 0; j < process.NumberOfThreads; j++)
                    {
                        var thread = data.ReadStruct<SystemThreadInformation>(i +
                            Marshal.SizeOf(typeof(SystemProcessInformation)), j);

                        if (pid != 0)
                        {
                            threads.Add(thread.ClientId.ThreadId, thread);
                        }
                        else
                        {
                            // Fix System Idle Process threads.
                            // There is one thread per CPU, but they 
                            // all have a TID of 0. Assign unique TIDs.
                            threads.Add(j, thread);
                        }
                    }

                    return threads;
                }

                i += process.NextEntryOffset;

            } while (process.NextEntryOffset != 0);

            return null;
        }

        /// <summary>
        /// Gets a dictionary containing the services on the system.
        /// </summary>
        /// <returns>A dictionary, indexed by service name.</returns>
        public static Dictionary<string, EnumServiceStatusProcess> GetServices()
        {
            using (ServiceManagerHandle manager =
                new ServiceManagerHandle(ScManagerAccess.EnumerateService))
            {
                int requiredSize;
                int servicesReturned;
                int resume = 0;

                if (_servicesBuffer == null)
                    _servicesBuffer = new MemoryAlloc(0x10000);

                MemoryAlloc data = _servicesBuffer;

                if (!Win32.EnumServicesStatusEx(manager, IntPtr.Zero, ServiceQueryType.Win32 | ServiceQueryType.Driver,
                    ServiceQueryState.All, data,
                    data.Size, out requiredSize, out servicesReturned,
                    ref resume, null))
                {
                    // resize buffer
                    data.ResizeNew(requiredSize);

                    if (!Win32.EnumServicesStatusEx(manager, IntPtr.Zero, ServiceQueryType.Win32 | ServiceQueryType.Driver,
                        ServiceQueryState.All, data,
                        data.Size, out requiredSize, out servicesReturned,
                        ref resume, null))
                        Win32.Throw();
                }

                var dictionary = new Dictionary<string, EnumServiceStatusProcess>(servicesReturned);

                for (int i = 0; i < servicesReturned; i++)
                {
                    var service = data.ReadStruct<EnumServiceStatusProcess>(i);

                    dictionary.Add(service.ServiceName, service);
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Gets the 64-bit tick count.
        /// </summary>
        /// <returns>A 64-bit tick count.</returns>
        public static long GetTickCount()
        {
            // Read the tick count multiplier.
            int tickCountMultiplier = Marshal.ReadInt32(Win32.UserSharedData.Increment(
                KUserSharedData.TickCountMultiplierOffset));

            // Read the tick count.
            var tickCount = QueryKSystemTime(Win32.UserSharedData.Increment(
                KUserSharedData.TickCountOffset));

            return (((long)tickCount.LowPart * tickCountMultiplier) >> (int)24) +
                (((long)tickCount.HighPart * tickCountMultiplier) << (int)8);
        }

        /// <summary>
        /// Gets information about the system time.
        /// </summary>
        /// <returns>A time of day structure.</returns>
        public static SystemTimeOfDayInformation GetTimeOfDay()
        {
            NtStatus status;
            SystemTimeOfDayInformation timeOfDay;
            int retLength;

            status = Win32.NtQuerySystemInformation(
                SystemInformationClass.SystemTimeOfDayInformation,
                out timeOfDay,
                Marshal.SizeOf(typeof(SystemTimeOfDayInformation)),
                out retLength
                );

            if (status >= NtStatus.Error)
                Win32.Throw(status);

            return timeOfDay;
        }

        /// <summary>
        /// Gets the uptime of the system.
        /// </summary>
        /// <returns>A time span describing the time elapsed since the system was booted.</returns>
        public static TimeSpan GetUptime()
        {
            var timeOfDay = GetTimeOfDay();

            return new TimeSpan(timeOfDay.CurrentTime - timeOfDay.BootTime);
        }

        /// <summary>
        /// Loads a driver.
        /// </summary>
        /// <param name="serviceName">The service name of the driver.</param>
        public static void LoadDriver(string serviceName)
        {
            var str = new UnicodeString(
                "\\REGISTRY\\MACHINE\\SYSTEM\\CurrentControlSet\\Services\\" + serviceName);

            try
            {
                NtStatus status;

                if ((status = Win32.NtLoadDriver(ref str)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                str.Dispose();
            }
        }

        /// <summary>
        /// Reads a KSYSTEM_TIME value atomically.
        /// </summary>
        /// <param name="time">A pointer to a KSYSTEM_TIME value.</param>
        /// <returns>A 64-bit time value.</returns>
        private static LargeInteger QueryKSystemTime(IntPtr time)
        {
            unsafe
            {
                return QueryKSystemTime((KSystemTime*)time);
            }
        }

        /// <summary>
        /// Reads a KSYSTEM_TIME value atomically.
        /// </summary>
        /// <param name="time">A pointer to a KSYSTEM_TIME value.</param>
        /// <returns>A 64-bit time value.</returns>
        private unsafe static LargeInteger QueryKSystemTime(KSystemTime* time)
        {
            LargeInteger localTime = new LargeInteger();

            // If we're on 32-bit, we need to use a special 
            // method to read the time atomically. On 64-bit, 
            // we can simply read the time.

            if (OSVersion.Architecture == OSArch.I386)
            {
                localTime.QuadPart = 0;

                while (true)
                {
                    localTime.HighPart = time->High1Time;
                    localTime.LowPart = time->LowPart;

                    // Check if someone started changing the time 
                    // while we were reading the two values.
                    if (localTime.HighPart == time->High2Time)
                        break;

                    System.Threading.Thread.SpinWait(1);
                }
            }
            else
            {
                localTime.QuadPart = time->QuadPart;
            }

            return localTime;
        }

        /// <summary>
        /// Unloads a driver.
        /// </summary>
        /// <param name="serviceName">The service name of the driver.</param>
        public static void UnloadDriver(string serviceName)
        {
            var str = new UnicodeString(
                "\\REGISTRY\\MACHINE\\SYSTEM\\CurrentControlSet\\Services\\" + serviceName);

            try
            {
                NtStatus status;

                if ((status = Win32.NtUnloadDriver(ref str)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                str.Dispose();
            }
        }
    }

    public enum NetworkProtocol
    {
        Tcp,
        Udp,
        Tcp6,
        Udp6
    }

    public struct ObjectInformation
    {
        public string OrigName;
        public string BestName;
        public string TypeName;
    }

    public struct NetworkConnection
    {
        public int Pid;
        public NetworkProtocol Protocol;
        public IPEndPoint Local;
        public IPEndPoint Remote;
        public MibTcpState State;
        public object Tag;

        public void CloseTcpConnection()
        {
            MibTcpRow row = new MibTcpRow()
            {
                State = MibTcpState.DeleteTcb,
                LocalAddress = (uint)this.Local.Address.Address,
                LocalPort = ((ushort)this.Local.Port).Reverse(),
                RemoteAddress = this.Remote != null ? (uint)this.Remote.Address.Address : 0,
                RemotePort = this.Remote != null ? ((ushort)this.Remote.Port).Reverse() : 0
            };
            int result = Win32.SetTcpEntry(ref row);

            if (result != 0)
                Win32.Throw(result);
        }
    }

    public struct SystemProcess
    {
        public string Name;
        public SystemProcessInformation Process;
        public Dictionary<int, SystemThreadInformation> Threads;
    }

    public class KernelModule : ILoadedModule
    {
        public KernelModule(
            IntPtr baseAddress,
            int size,
            LdrpDataTableEntryFlags flags,
            string baseName,
            string fileName
            )
        {
            this.BaseAddress = baseAddress;
            this.Size = size;
            this.Flags = flags;
            this.BaseName = baseName;
            this.FileName = fileName;
        }

        /// <summary>
        /// The base address of the module.
        /// </summary>
        public IntPtr BaseAddress { get; private set; }
        /// <summary>
        /// The size of the module.
        /// </summary>
        public int Size { get; private set; }
        /// <summary>
        /// The flags set by the loader for this module.
        /// </summary>
        public LdrpDataTableEntryFlags Flags { get; private set; }
        /// <summary>
        /// The base name of the module (e.g. module.dll).
        /// </summary>
        public string BaseName { get; private set; }
        /// <summary>
        /// The file name of the module (e.g. C:\Windows\system32\module.dll).
        /// </summary>
        public string FileName { get; private set; }
    }

    public class SystemLogonSession
    {
        public SystemLogonSession(
            string authenticationPackage,
            string dnsDomainName,
            string logonDomain,
            Luid logonId,
            string logonServer,
            DateTime logonTime,
            LogonType logonType,
            int session,
            Sid sid,
            string upn,
            string userName
            )
        {
            this.AuthenticationPackage = authenticationPackage;
            this.DnsDomainName = dnsDomainName;
            this.LogonDomain = logonDomain;
            this.LogonId = logonId;
            this.LogonServer = logonServer;
            this.LogonTime = logonTime;
            this.LogonType = logonType;
            this.Session = session;
            this.Sid = sid;
            this.Upn = upn;
            this.UserName = userName;
        }

        public string AuthenticationPackage { get; private set; }
        public string DnsDomainName { get; private set; }
        public string LogonDomain { get; private set; }
        public Luid LogonId { get; private set; }
        public string LogonServer { get; private set; }
        public DateTime LogonTime { get; private set; }
        public LogonType LogonType { get; private set; }
        public int Session { get; private set; }
        public Sid Sid { get; private set; }
        public string Upn { get; private set; }
        public string UserName { get; private set; }
    }

    public class SystemPagefile
    {
        public SystemPagefile(int totalSize, int totalInUse, int peakUsage, string fileName)
        {
            this.TotalSize = totalSize;
            this.TotalInUse = totalInUse;
            this.PeakUsage = peakUsage;
            this.FileName = fileName;
        }

        public int TotalSize { get; private set; }
        public int TotalInUse { get; private set; }
        public int PeakUsage { get; private set; }
        public string FileName { get; private set; }
    }
}
