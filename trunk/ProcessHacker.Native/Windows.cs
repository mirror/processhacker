using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    public class Windows
    {
        public delegate bool EnumKernelModulesDelegate(KernelModule kernelModule);

        public delegate string GetProcessNameCallback(int pid);

        public static GetProcessNameCallback GetProcessName;

        /// <summary>
        /// A cache for type names; QuerySystemInformation with ALL_TYPES_INFORMATION fails for some 
        /// reason. The dictionary relates object type numbers to their names.
        /// </summary>
        internal static Dictionary<byte, string> ObjectTypes = new Dictionary<byte, string>();

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
                    new KernelModule((uint)imageBases[i], 
                        name.ToString(), 
                        FileUtils.FixPath(fileName.ToString()))))
                    break;
            }
        }

        public static string GetAccountName(IntPtr sid, bool includeDomain)
        {
            StringBuilder name = new StringBuilder(255);
            StringBuilder domain = new StringBuilder(255);
            int namelen = 255;
            int domainlen = 255;
            SidNameUse use = SidNameUse.User;

            try
            {
                if (!Win32.LookupAccountSid(null, sid, name, ref namelen, domain, ref domainlen, out use))
                {
                    // if the name is longer than 255 characters, increase the capacity.
                    name.EnsureCapacity(namelen);
                    domain.EnsureCapacity(domainlen);

                    if (!Win32.LookupAccountSid(null, sid, name, ref namelen, domain, ref domainlen, out use))
                    {
                        if (name.ToString() == "" && domain.ToString() == "")
                            Win32.ThrowLastError();
                    }
                }
            }
            catch
            {
                // if we didn't find a name, then return the string SID version.
                return (new System.Security.Principal.SecurityIdentifier(sid)).ToString();
            }

            if (includeDomain)
            {
                return ((domain.ToString() != "") ? domain.ToString() + "\\" : "") + name.ToString();
            }
            else
            {
                return name.ToString();
            }
        }

        public static SidNameUse GetAccountType(IntPtr SID)
        {
            StringBuilder name = new StringBuilder(255);
            StringBuilder domain = new StringBuilder(255);
            int namelen = 255;
            int domainlen = 255;
            SidNameUse use = SidNameUse.User;

            // we don't actually need to get the account name
            if (!Win32.LookupAccountSid(null, SID, name, ref namelen, domain, ref domainlen, out use))
            {
                name.EnsureCapacity(namelen);
                domain.EnsureCapacity(domainlen);

                if (!Win32.LookupAccountSid(null, SID, name, ref namelen, domain, ref domainlen, out use))
                {
                    if (name.ToString() == "" && domain.ToString() == "")
                        throw new Exception("Could not lookup account SID: " + Win32.GetLastErrorMessage());
                }
            }

            return use;
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

            using (MemoryAlloc data = new MemoryAlloc(0x1000))
            {
                int status;

                // This is needed because NtQuerySystemInformation with SystemHandleInformation doesn't 
                // actually give a real return length when called with an insufficient buffer. This code 
                // tries repeatedly to call the function, doubling the buffer size each time it fails.
                while ((uint)(status = Win32.NtQuerySystemInformation(
                    SystemInformationClass.SystemHandleInformation,
                    data, 
                    data.Size, 
                    out retLength)
                    ) == Win32.STATUS_INFO_LENGTH_MISMATCH)
                {
                    data.Resize(data.Size * 2);

                    // Fail if we've resized it to over 16MB - protect from infinite resizing
                    if (data.Size > 16 * 1024 * 1024)
                        throw new OutOfMemoryException();
                }

                if (status < 0)
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
        }

        public static KernelModule[] GetKernelModules()
        {
            int requiredSize = 0;
            IntPtr[] imageBases;

            Win32.EnumDeviceDrivers(null, 0, out requiredSize);
            imageBases = new IntPtr[requiredSize / 4];
            Win32.EnumDeviceDrivers(imageBases, requiredSize, out requiredSize);

            KernelModule[] kernelModules = new KernelModule[imageBases.Length];

            for (int i = 0; i < imageBases.Length; i++)
            {
                if (imageBases[i] == IntPtr.Zero)
                    continue;

                StringBuilder name = new StringBuilder(0x400);
                StringBuilder fileName = new StringBuilder(0x400);

                Win32.GetDeviceDriverBaseName(imageBases[i], name, name.Capacity * 2);
                Win32.GetDeviceDriverFileName(imageBases[i], fileName, name.Capacity * 2);

                kernelModules[i] = new KernelModule((uint)imageBases[i], name.ToString(), FileUtils.FixPath(fileName.ToString()));
            }

            return kernelModules;
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

        public static string GetPrivilegeDisplayName(string PrivilegeName)
        {
            StringBuilder sb = null;
            int size = 0;
            int languageId = 0;

            Win32.LookupPrivilegeDisplayName(null, PrivilegeName, sb, ref size, out languageId);
            sb = new StringBuilder(size);
            Win32.LookupPrivilegeDisplayName(null, PrivilegeName, sb, ref size, out languageId);

            return sb.ToString();
        }

        public static string GetPrivilegeName(Luid Luid)
        {
            StringBuilder sb = null;
            int size = 0;

            Win32.LookupPrivilegeName(null, ref Luid, sb, ref size);
            sb = new StringBuilder(size);
            Win32.LookupPrivilegeName(null, ref Luid, sb, ref size);

            return sb.ToString();
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

            using (MemoryAlloc data = new MemoryAlloc(0x4000))
            {
                int status;
                int attempts = 0;

                while (true)
                {
                    attempts++;

                    if ((status = Win32.NtQuerySystemInformation(SystemInformationClass.SystemProcessInformation, data.Memory,
                        data.Size, out retLength)) < 0)
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

                returnProcesses = new Dictionary<int, SystemProcess>();

                int i = 0;
                SystemProcess currentProcess = new SystemProcess();

                while (true)
                {
                    currentProcess.Process = data.ReadStruct<SystemProcessInformation>(i, 0);
                    currentProcess.Name = Utils.ReadUnicodeString(currentProcess.Process.ImageName);

                    if (getThreads &&
                        currentProcess.Process.ProcessId != 0)
                    {
                        currentProcess.Threads = new Dictionary<int, SystemThreadInformation>();

                        for (int j = 0; j < currentProcess.Process.NumberOfThreads; j++)
                        {
                            var thread = data.ReadStruct<SystemThreadInformation>(i +
                                Marshal.SizeOf(typeof(SystemProcessInformation)), j);

                            currentProcess.Threads.Add(thread.ClientId.UniqueThread, thread);
                        }
                    }

                    returnProcesses.Add(currentProcess.Process.ProcessId, currentProcess);

                    if (currentProcess.Process.NextEntryOffset == 0)
                        break;

                    i += currentProcess.Process.NextEntryOffset;
                }

                return returnProcesses;
            }
        }

        public static Dictionary<int, SystemThreadInformation> GetProcessThreads(int pid)
        {
            int retLength;

            using (MemoryAlloc data = new MemoryAlloc(0x4000))
            {
                int status;
                int attempts = 0;

                while (true)
                {
                    attempts++;

                    if ((status = Win32.NtQuerySystemInformation(SystemInformationClass.SystemProcessInformation, data.Memory,
                        data.Size, out retLength)) < 0)
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

                while (true)
                {
                    process = data.ReadStruct<SystemProcessInformation>(i, 0);

                    if (process.ProcessId == pid)
                    {
                        var threads = new Dictionary<int, SystemThreadInformation>();

                        for (int j = 0; j < process.NumberOfThreads; j++)
                        {
                            var thread = data.ReadStruct<SystemThreadInformation>(i +
                                Marshal.SizeOf(typeof(SystemProcessInformation)), j);

                            threads.Add(thread.ClientId.UniqueThread, thread);
                        }

                        return threads;
                    }

                    if (process.NextEntryOffset == 0)
                        break;

                    i += process.NextEntryOffset;
                }
            }

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

                // get required size
                Win32.EnumServicesStatusEx(manager, IntPtr.Zero, ServiceQueryType.Win32 | ServiceQueryType.Driver,
                    ServiceQueryState.All, IntPtr.Zero, 0, out requiredSize, out servicesReturned,
                    ref resume, null);

                using (MemoryAlloc data = new MemoryAlloc(requiredSize))
                {
                    var dictionary = new Dictionary<string, EnumServiceStatusProcess>();

                    if (!Win32.EnumServicesStatusEx(manager, IntPtr.Zero, ServiceQueryType.Win32 | ServiceQueryType.Driver,
                        ServiceQueryState.All, data,
                        data.Size, out requiredSize, out servicesReturned,
                        ref resume, null))
                        Win32.ThrowLastError();

                    for (int i = 0; i < servicesReturned; i++)
                    {
                        var service = data.ReadStruct<EnumServiceStatusProcess>(i);

                        dictionary.Add(service.ServiceName, service);
                    }

                    return dictionary;
                }
            }
        }
    }

    public class KernelModule
    {
        public KernelModule(uint baseAddress, string baseName, string fileName)
        {
            this.BaseAddress = baseAddress;
            this.BaseName = baseName;
            this.FileName = fileName;
        }

        public uint BaseAddress { get; private set; }
        public string BaseName { get; private set; }
        public string FileName { get; private set; }
    }

    public struct ObjectInformation
    {
        public string OrigName;
        public string BestName;
        public string TypeName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemHandleInformation
    {
        public int ProcessId;
        public byte ObjectTypeNumber;
        public HandleFlags Flags;
        public short Handle;
        public IntPtr Object;
        public int GrantedAccess;

        public ObjectInformation GetHandleInfo()
        {
            using (ProcessHandle process = new ProcessHandle(this.ProcessId, ProcessAccess.DupHandle))
            {
                return this.GetHandleInfo(process);
            }
        }

        public ObjectInformation GetHandleInfo(ProcessHandle process)
        {
            IntPtr handle = new IntPtr(this.Handle);
            IntPtr objectHandleI;
            int retLength = 0;
            Win32Handle objectHandle = null;

            if (this.Handle == 0 || this.Handle == -1 || this.Handle == -2)
                throw new WindowsException(6);

            // Duplicate the handle if we're not using KPH
            if (KProcessHacker.Instance == null)
            {
                int status;

                if ((status = Win32.NtDuplicateObject(
                    process, handle, ProcessHandle.GetCurrent(), out objectHandleI, 0, 0, 0)) < 0)
                    Win32.ThrowLastError();

                objectHandle = new Win32Handle(objectHandleI);
            }

            ObjectInformation info = new ObjectInformation();

            // If the cache contains the object type's name, use it. Otherwise, query the type 
            // for its name.
            lock (Windows.ObjectTypes)
            {
                if (Windows.ObjectTypes.ContainsKey(this.ObjectTypeNumber))
                {
                    info.TypeName = Windows.ObjectTypes[this.ObjectTypeNumber];
                }
                else
                {
                    int baseAddress = 0;

                    if (KProcessHacker.Instance != null)
                    {
                        KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectTypeInformation,
                            IntPtr.Zero, 0, out retLength, out baseAddress);
                    }
                    else
                    {
                        Win32.NtQueryObject(objectHandle, ObjectInformationClass.ObjectTypeInformation,
                            IntPtr.Zero, 0, out retLength);
                    }

                    if (retLength > 0)
                    {
                        using (MemoryAlloc otiMem = new MemoryAlloc(retLength))
                        {
                            if (KProcessHacker.Instance != null)
                            {
                                if (KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectTypeInformation,
                                    otiMem, otiMem.Size, out retLength, out baseAddress) < 0)
                                    throw new Exception("ZwQueryObject failed.");
                            }
                            else
                            {
                                if (Win32.NtQueryObject(objectHandle, ObjectInformationClass.ObjectTypeInformation,
                                    otiMem, otiMem.Size, out retLength) < 0)
                                    throw new Exception("NtQueryObject failed.");
                            }

                            var oti = otiMem.ReadStruct<ObjectTypeInformation>();
                            var str = oti.Name;

                            if (KProcessHacker.Instance != null)
                                str.Buffer = str.Buffer.Increment(-baseAddress + otiMem);

                            info.TypeName = Utils.ReadUnicodeString(str);
                            Windows.ObjectTypes.Add(this.ObjectTypeNumber, info.TypeName);
                        }
                    }
                }
            }

            if (KProcessHacker.Instance != null && info.TypeName == "File")
            {
                // use KProcessHacker for files
                info.OrigName = KProcessHacker.Instance.GetFileObjectName(this);
            }
            else if (info.TypeName == "File" && (int)this.GrantedAccess == 0x0012019f)
            {
                // KProcessHacker not available, fall back to using hack (i.e. not querying the name at all)
            }
            else
            {
                int baseAddress = 0;

                if (KProcessHacker.Instance != null)
                {
                    KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectNameInformation,
                        IntPtr.Zero, 0, out retLength, out baseAddress);
                }
                else
                {
                    Win32.NtQueryObject(objectHandle, ObjectInformationClass.ObjectNameInformation,
                        IntPtr.Zero, 0, out retLength);
                }

                if (retLength > 0)
                {
                    using (MemoryAlloc oniMem = new MemoryAlloc(retLength))
                    {
                        if (KProcessHacker.Instance != null)
                        {
                            if (KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectNameInformation,
                                oniMem, oniMem.Size, out retLength, out baseAddress) < 0)
                                throw new Exception("ZwQueryObject failed.");
                        }
                        else
                        {
                            if (Win32.NtQueryObject(objectHandle, ObjectInformationClass.ObjectNameInformation,
                                oniMem, oniMem.Size, out retLength) < 0)
                                throw new Exception("NtQueryObject failed.");
                        }

                        var oni = oniMem.ReadStruct<ObjectNameInformation>();
                        var str = oni.Name;

                        if (KProcessHacker.Instance != null)
                            str.Buffer = str.Buffer.Increment(-baseAddress + oniMem);

                        info.OrigName = Utils.ReadUnicodeString(str);
                    }
                }
            }

            // get a better name for the handle
            try
            {
                switch (info.TypeName)
                {
                    case "File":
                        // resolves \Device\Harddisk1 into C:, for example
                        info.BestName = FileUtils.DeviceFileNameToDos(info.OrigName);

                        break;

                    case "Key":
                        string hklmString = "\\registry\\machine";
                        string hkcrString = "\\registry\\machine\\software\\classes";
                        string hkcuString = "\\registry\\user\\" +
                            System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString().ToLower();
                        string hkcucrString = "\\registry\\user\\" +
                            System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString().ToLower() + "_classes";
                        string hkuString = "\\registry\\user";

                        if (info.OrigName.ToLower().StartsWith(hkcrString))
                            info.BestName = "HKCR" + info.OrigName.Substring(hkcrString.Length);
                        else if (info.OrigName.ToLower().StartsWith(hklmString))
                            info.BestName = "HKLM" + info.OrigName.Substring(hklmString.Length);
                        else if (info.OrigName.ToLower().StartsWith(hkcucrString))
                            info.BestName = "HKCU\\Software\\Classes" + info.OrigName.Substring(hkcucrString.Length);
                        else if (info.OrigName.ToLower().StartsWith(hkcuString))
                            info.BestName = "HKCU" + info.OrigName.Substring(hkcuString.Length);
                        else if (info.OrigName.ToLower().StartsWith(hkuString))
                            info.BestName = "HKU" + info.OrigName.Substring(hkuString.Length);
                        else
                            info.BestName = info.OrigName;

                        break;

                    case "Process":
                        {
                            int processId;

                            if (KProcessHacker.Instance != null)
                            {
                                processId = KProcessHacker.Instance.KphGetProcessId(process, handle);

                                if (processId == 0)
                                    throw new Exception("Invalid PID");
                            }
                            else
                            {
                                using (Win32Handle processHandle =
                                    new Win32Handle(process, handle, (int)OSVersion.MinProcessQueryInfoAccess))
                                {
                                    if ((processId = Win32.GetProcessId(processHandle)) == 0)
                                        Win32.ThrowLastError();
                                }
                            }

                            string processName = Windows.GetProcessName(processId);

                            if (processName != null)
                                info.BestName = processName + " (" + processId.ToString() + ")";
                            else
                                info.BestName = "Non-existent process (" + processId.ToString() + ")";
                        }

                        break;

                    case "Thread":
                        {
                            int processId;
                            int threadId;

                            if (KProcessHacker.Instance != null)
                            {
                                threadId = KProcessHacker.Instance.KphGetThreadId(process, handle, out processId);

                                if (threadId == 0 || processId == 0)
                                    throw new Exception("Invalid TID or PID");
                            }
                            else
                            {
                                using (Win32Handle threadHandle =
                                    new Win32Handle(process, handle, (int)OSVersion.MinThreadQueryInfoAccess))
                                {
                                    if ((threadId = Win32.GetThreadId(threadHandle)) == 0)
                                        Win32.ThrowLastError();

                                    if ((processId = Win32.GetProcessIdOfThread(threadHandle)) == 0)
                                        Win32.ThrowLastError();
                                }
                            }

                            string processName = Windows.GetProcessName(processId);

                            if (processName != null)
                                info.BestName = processName + " (" + processId.ToString() + "): " +
                                    threadId.ToString();
                            else
                                info.BestName = "Non-existent process (" + processId.ToString() + "): " +
                                    threadId.ToString();
                        }

                        break;

                    case "Token":
                        {
                            using (Win32Handle tokenHandle = 
                                new Win32Handle(process, handle, (int)TokenAccess.Query))
                            {
                                info.BestName = TokenHandle.FromHandle(tokenHandle).GetUser().GetName(true);
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
            catch (Exception ex)
            {
                if (info.OrigName != null && info.OrigName != "")
                {
                    info.BestName = info.OrigName;
                }
                else
                {
                    info.BestName = null;
                }
            }

            if (objectHandle != null)
                objectHandle.Dispose();

            return info;
        }
    }

    public enum NetworkProtocol
    {
        Tcp, Udp
    }

    public struct NetworkConnection
    {
        public string Id;
        public int Pid;
        public NetworkProtocol Protocol;
        public string LocalString;
        public IPEndPoint Local;
        public string RemoteString;
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
                Win32.ThrowLastError(result, false);
        }
    }

    public struct SystemProcess
    {
        public string Name;
        public SystemProcessInformation Process;
        public Dictionary<int, SystemThreadInformation> Threads;
    }
}
