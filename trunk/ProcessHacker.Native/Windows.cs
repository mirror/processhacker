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

#pragma warning disable 0618

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    public static class Windows
    {
        public delegate bool EnumKernelModulesDelegate(KernelModule kernelModule);

        public delegate string GetProcessNameCallback(int pid);

        public static GetProcessNameCallback GetProcessName;

        public static string[] KernelNames = { "ntoskrnl.exe", "ntkrnlpa.exe", "ntkrnlmp.exe", "ntkrpamp.exe" };

        /// <summary>
        /// A cache for type names; QuerySystemInformation with ALL_TYPES_INFORMATION fails for some 
        /// reason. The dictionary relates object type numbers to their names.
        /// </summary>
        internal static Dictionary<byte, string> ObjectTypes = new Dictionary<byte, string>();

        [ThreadStatic]
        private static MemoryAlloc _handlesBuffer;
        [ThreadStatic]
        private static MemoryAlloc _processesBuffer;
        [ThreadStatic]
        private static MemoryAlloc _servicesBuffer;

        private static int _numberOfProcessors = 0;
        private static int _pageSize = 0;
        private static IntPtr _kernelBase = IntPtr.Zero;
        private static string _kernelFileName = null;

        public static int NumberOfProcessors
        {
            get
            {
                if (_numberOfProcessors == 0)
                    _numberOfProcessors = GetBasicInformation().NumberOfProcessors;

                return _numberOfProcessors;
            }
        }

        public static int PageSize
        {
            get
            {
                if (_pageSize == 0)
                    _pageSize = GetBasicInformation().PageSize;

                return _pageSize;
            }
        }

        public static IntPtr KernelBase
        {
            get
            {
                if (_kernelBase == IntPtr.Zero)
                    _kernelBase = GetKernelBase();

                return _kernelBase;
            }
        }

        public static string KernelFileName
        {
            get
            {
                if (_kernelFileName == null)
                    _kernelFileName = GetKernelFileName();

                return _kernelFileName;
            }
        }

        public static void EnumKernelModules(EnumKernelModulesDelegate enumCallback)
        {
            int requiredSize = 0;
            IntPtr[] imageBases;

            Win32.EnumDeviceDrivers(null, 0, out requiredSize);
            imageBases = new IntPtr[requiredSize / 4];
            Win32.EnumDeviceDrivers(imageBases, requiredSize, out requiredSize);

            for (int i = 0; i < imageBases.Length; i++)
            {
                if (imageBases[i] == IntPtr.Zero)
                    continue;

                StringBuilder name = new StringBuilder(0x400);
                StringBuilder fileName = new StringBuilder(0x400);

                Win32.GetDeviceDriverBaseName(imageBases[i], name, name.Capacity * 2);
                Win32.GetDeviceDriverFileName(imageBases[i], fileName, name.Capacity * 2);

                if (!enumCallback(
                    new KernelModule(
                        imageBases[i], 
                        name.ToString(), 
                        FileUtils.FixPath(fileName.ToString())
                        )))
                    break;
            }
        }

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
                Win32.ThrowLastError(status);

            return sbi;
        }

        /// <summary>
        /// Enumerates the handles opened by every running process.
        /// </summary>
        /// <returns>An array containing information about the handles.</returns>
        public static SystemHandleInformation[] GetHandles()
        {
            int retLength = 0;
            int handleCount = 0;
            SystemHandleInformation[] returnHandles;

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
                data.Resize(data.Size * 2);

                // Fail if we've resized it to over 16MB - protect from infinite resizing
                if (data.Size > 16 * 1024 * 1024)
                    throw new OutOfMemoryException();
            }

            if (status >= NtStatus.Error)
                Win32.ThrowLastError(status);

            // The structure of the buffer is the handle count plus an array of SYSTEM_HANDLE_INFORMATION 
            // structures.
            handleCount = data.ReadInt32(0);
            returnHandles = new SystemHandleInformation[handleCount];

            for (int i = 0; i < handleCount; i++)
            {
                returnHandles[i] = data.ReadStruct<SystemHandleInformation>(4, i);
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
                System.IO.FileInfo fi = new System.IO.FileInfo(FileUtils.FixPath(module.FileName));
                bool kernel = false;
                string realName;

                realName = fi.FullName;

                foreach (string k in KernelNames)
                {
                    if (realName.Equals(Environment.SystemDirectory + "\\" + k, StringComparison.InvariantCultureIgnoreCase))
                    {
                        kernel = true;

                        break;
                    }
                }

                if (kernel)
                {
                    kernelBase = module.BaseAddress;
                    return false;
                }

                return true;
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
                System.IO.FileInfo fi = new System.IO.FileInfo(FileUtils.FixPath(module.FileName));
                bool kernel = false;
                string realName;

                realName = fi.FullName;

                foreach (string k in KernelNames)
                {
                    if (realName.Equals(Environment.SystemDirectory + "\\" + k, StringComparison.InvariantCultureIgnoreCase))
                    {
                        kernel = true;

                        break;
                    }
                }

                if (kernel)
                {
                    kernelFileName = realName;
                    return false;
                }

                return true;
            });

            return kernelFileName;
        }

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

        public static Dictionary<int, List<NetworkConnection>> GetNetworkConnections()
        {
            var retDict = new Dictionary<int, List<NetworkConnection>>();
            int length = 0;

            Win32.GetExtendedTcpTable(IntPtr.Zero, ref length, false, 2, TcpTableClass.OwnerPidAll, 0);

            using (var mem = new MemoryAlloc(length))
            {
                if (Win32.GetExtendedTcpTable(mem, ref length, false, 2,
                    TcpTableClass.OwnerPidAll, 0) != 0)
                    Win32.ThrowLastError();

                int count = mem.ReadInt32(0);

                for (int i = 0; i < count; i++)
                {
                    var struc = mem.ReadStruct<MibTcpRowOwnerPid>(4, i);

                    if (!retDict.ContainsKey(struc.OwningProcessId))
                        retDict.Add(struc.OwningProcessId, new List<NetworkConnection>());

                    retDict[struc.OwningProcessId].Add(new NetworkConnection()
                    {
                        Protocol = NetworkProtocol.Tcp,
                        Local = new IPEndPoint(struc.LocalAddress, ((ushort)struc.LocalPort).SwapBytes()),
                        Remote = new IPEndPoint(struc.RemoteAddress, ((ushort)struc.RemotePort).SwapBytes()),
                        State = struc.State,
                        Pid = struc.OwningProcessId
                    });
                }
            }

            Win32.GetExtendedUdpTable(IntPtr.Zero, ref length, false, 2, UdpTableClass.OwnerPid, 0);

            using (var mem = new MemoryAlloc(length))
            {
                if (Win32.GetExtendedUdpTable(mem, ref length, false, 2, UdpTableClass.OwnerPid, 0) != 0)
                    Win32.ThrowLastError();

                int count = mem.ReadInt32(0);

                for (int i = 0; i < count; i++)
                {
                    var struc = mem.ReadStruct<MibUdpRowOwnerPid>(4, i);

                    if (!retDict.ContainsKey(struc.OwningProcessId))
                        retDict.Add(struc.OwningProcessId, new List<NetworkConnection>());

                    retDict[struc.OwningProcessId].Add(
                        new NetworkConnection()
                        {
                            Protocol = NetworkProtocol.Udp,
                            Local = new IPEndPoint(struc.LocalAddress, ((ushort)struc.LocalPort).SwapBytes()),
                            Pid = struc.OwningProcessId
                        });
                }
            }

            return retDict;
        }

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
                    data.Resize(data.Size * 2);

                    // Fail if we've resized it to over 16MB - protect from infinite resizing
                    if (data.Size > 16 * 1024 * 1024)
                        throw new OutOfMemoryException();
                }

                if (status >= NtStatus.Error)
                    Win32.ThrowLastError(status);

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
                        FileUtils.FixPath(currentPagefile.PageFileName.Read())
                        ));

                    i += currentPagefile.NextEntryOffset;
                } while (currentPagefile.NextEntryOffset != 0);

                return pagefiles.ToArray();
            }
        }

        public static Dictionary<int, SystemProcess> GetProcesses()
        {
            return GetProcesses(false);
        }

        /// <summary>
        /// Gets a dictionary containing the currently running processes.
        /// </summary>
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
                        Win32.ThrowLastError(status);

                    data.Resize(retLength);
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
                currentProcess.Process = data.ReadStruct<SystemProcessInformation>(i, 0);
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
                        Win32.ThrowLastError(status);

                    data.Resize(retLength);
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
                process = data.ReadStruct<SystemProcessInformation>(i, 0);

                if (process.ProcessId == pid)
                {
                    var threads = new Dictionary<int, SystemThreadInformation>();

                    for (int j = 0; j < process.NumberOfThreads; j++)
                    {
                        var thread = data.ReadStruct<SystemThreadInformation>(i +
                            Marshal.SizeOf(typeof(SystemProcessInformation)), j);

                        threads.Add(thread.ClientId.ThreadId, thread);
                    }

                    return threads;
                }

                i += process.NextEntryOffset;

            } while (process.NextEntryOffset != 0);

            return null;
        }

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
                    data.Resize(requiredSize);

                    if (!Win32.EnumServicesStatusEx(manager, IntPtr.Zero, ServiceQueryType.Win32 | ServiceQueryType.Driver,
                        ServiceQueryState.All, data,
                        data.Size, out requiredSize, out servicesReturned,
                        ref resume, null))
                        Win32.ThrowLastError();
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

        public static void LoadDriver(string serviceName)
        {
            var str = new UnicodeString(
                "\\REGISTRY\\MACHINE\\SYSTEM\\CurrentControlSet\\Services\\" + serviceName);

            try
            {
                NtStatus status;

                if ((status = Win32.NtLoadDriver(ref str)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                str.Dispose();
            }
        }

        public static void UnloadDriver(string serviceName)
        {
            var str = new UnicodeString(
                "\\REGISTRY\\MACHINE\\SYSTEM\\CurrentControlSet\\Services\\" + serviceName);

            try
            {
                NtStatus status;

                if ((status = Win32.NtUnloadDriver(ref str)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);
            }
            finally
            {
                str.Dispose();
            }
        }
    }

    public enum NetworkProtocol
    {
        Tcp, Udp
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
                LocalPort = ((ushort)this.Local.Port).SwapBytes(),
                RemoteAddress = this.Remote != null ? (uint)this.Remote.Address.Address : 0,
                RemotePort = this.Remote != null ? ((ushort)this.Remote.Port).SwapBytes() : 0
            };
            int result = Win32.SetTcpEntry(ref row);

            if (result != 0)
                Win32.ThrowLastError(result);
        }
    }

    public struct SystemProcess
    {
        public string Name;
        public SystemProcessInformation Process;
        public Dictionary<int, SystemThreadInformation> Threads;
    }

    public class KernelModule
    {
        public KernelModule(IntPtr baseAddress, string baseName, string fileName)
        {
            this.BaseAddress = baseAddress;
            this.BaseName = baseName;
            this.FileName = fileName;
        }

        public IntPtr BaseAddress { get; private set; }
        public string BaseName { get; private set; }
        public string FileName { get; private set; }
    }

    public class SystemPagefile
    {
        public SystemPagefile(int totalSize, int totalInUse, int peakUsage, string fileName)
        {
            this.TotalSize = totalSize;
            this.TotalInUse = TotalInUse;
            this.PeakUsage = peakUsage;
            this.FileName = fileName;
        }

        public int TotalSize { get; private set; }
        public int TotalInUse { get; private set; }
        public int PeakUsage { get; private set; }
        public string FileName { get; private set; }
    }
}
