/*
 * Process Hacker - 
 *   dump system information
 * 
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Mfs;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public static class Dump
    {
        public static void Write(this BinaryWriter bw, string key, string value)
        {
            if (value == null)
                value = "";

            bw.Write(Encoding.Unicode.GetBytes(key + "=" + value.Replace("\0", "") + "\0"));
        }

        public static void Write(this BinaryWriter bw, string key, int value)
        {
            bw.Write(Encoding.Unicode.GetBytes(key + "=" + value.ToString("x") + "\0"));
        }

        public static void Write(this BinaryWriter bw, string key, long value)
        {
            bw.Write(Encoding.Unicode.GetBytes(key + "=" + value.ToString("x") + "\0"));
        }

        public static void Write(this BinaryWriter bw, string key, IntPtr value)
        {
            bw.Write(Encoding.Unicode.GetBytes(key + "=" + value.ToString("x") + "\0"));
        }      

        public static void Write(this BinaryWriter bw, string key, bool value)
        {
            bw.Write(Encoding.Unicode.GetBytes(key + "=" + (value ? "1" : "0") + "\0"));
        }

        public static void Write(this BinaryWriter bw, string key, DateTime value)
        {
            bw.Write(key, value.ToFileTime());
        }

        public static void WriteListEntry(this BinaryWriter bw, string value)
        {
            if (value == null)
                value = "";

            bw.Write(Encoding.Unicode.GetBytes(value.Replace("\0", "") + "\0"));
        }

        public static void AppendStruct<T>(MemoryObject mo, T s)
            where T : struct
        {
            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(T))))
            {
                data.WriteStruct<T>(s);
                mo.AppendData(data.ReadBytes(data.Size));
            }
        }

        public static IDictionary<string, string> GetDictionary(MemoryObject mo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string str = Encoding.Unicode.GetString(mo.ReadData());
            int i = 0;

            if (str == "")
                return dict;

            while (true)
            {
                int equalsIndex = str.IndexOf('=', i);

                if (equalsIndex == -1)
                    break;

                int nullIndex = str.IndexOf('\0', equalsIndex + 1);

                if (nullIndex == -1)
                    break;

                dict.Add(str.Substring(i, equalsIndex - i), str.Substring(equalsIndex + 1, nullIndex - equalsIndex - 1));

                i = nullIndex + 1;

                if (i >= str.Length)
                    break;
            }

            return dict;
        }

        public static Icon GetIcon(MemoryObject mo)
        {
            byte[] data = mo.ReadData();
            ProcessHacker.Common.ByteStreamReader reader = new ProcessHacker.Common.ByteStreamReader(data);

            using (Bitmap b = new Bitmap(reader))
            {
                return Icon.FromHandle(b.GetHicon());
            }
        }

        public static string[] GetList(MemoryObject mo)
        {
            string str = Encoding.Unicode.GetString(mo.ReadData());

            if (str.Length > 0)
                str = str.Remove(str.Length - 1, 1);

            return str.Split('\0');
        }

        public static T GetStruct<T>(MemoryObject mo)
        {
            byte[] data = mo.ReadData();

            unsafe
            {
                fixed (byte* dataPtr = data)
                    return (T)Marshal.PtrToStructure(new IntPtr(dataPtr), typeof(T));
            }
        }

        public static bool ParseBool(string str)
        {
            return str != "0";
        }

        public static int ParseInt32(string str)
        {
            return int.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static long ParseInt64(string str)
        {
            return long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static IntPtr ParseIntPtr(string str)
        {
            return long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier).ToIntPtr();
        }

        public static ulong ParseUInt64(string str)
        {
            return ulong.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static DateTime ParseDateTime(string str)
        {
            return DateTime.FromFileTime(ParseInt64(str));
        }

        public static MemoryFileSystem BeginDump(string fileName, MfsOpenMode mode)
        {
            MemoryFileSystem mfs = new MemoryFileSystem(fileName, mode);

            using (var sysinfo = mfs.RootObject.CreateChild("SystemInformation"))
            {
                BinaryWriter bw = new BinaryWriter(sysinfo.GetWriteStream());

                bw.Write("ProcessHackerVersion", Application.ProductVersion);
                bw.Write("OSVersion", Environment.OSVersion.VersionString);
                bw.Write("Architecture", (int)OSVersion.Architecture);
                bw.Write("UserName", Sid.CurrentUser.GetFullName(true));
                bw.Write("Time", DateTime.Now);

                bw.Close();
            }

            mfs.RootObject.CreateChild("Processes").Dispose();
            mfs.RootObject.CreateChild("Services").Dispose();
            mfs.RootObject.CreateChild("Network").Dispose();

            return mfs;
        }

        public static void DumpProcesses(MemoryFileSystem mfs, ProcessSystemProvider provider)
        {
            using (var processes = mfs.RootObject.GetChild("Processes"))
            {
                var p = Windows.GetProcesses();

                foreach (var process in p.Values)
                {
                    using (var processChild = processes.CreateChild(process.Process.ProcessId.ToString("x")))
                    {
                        ProcessItem item = null;

                        if (provider != null)
                        {
                            if (provider.Dictionary.ContainsKey(process.Process.ProcessId))
                                item = provider.Dictionary[process.Process.ProcessId];
                        }

                        DumpProcess(processChild, process, item, p, Windows.GetHandles());
                    }
                }

                if (provider != null)
                {
                    int dpcsPid = provider.DpcsProcess.Process.ProcessId;
                    int interruptsPid = provider.InterruptsProcess.Process.ProcessId;

                    using (var dpcsChild = processes.CreateChild(dpcsPid.ToString("x")))
                    {
                        DumpProcess(dpcsChild, provider.DpcsProcess, provider.Dictionary[dpcsPid], null, null);
                    }

                    using (var interruptsChild = processes.CreateChild(interruptsPid.ToString("x")))
                    {
                        DumpProcess(interruptsChild, provider.InterruptsProcess, provider.Dictionary[interruptsPid], null, null);
                    }
                }
            }
        }

        public static void DumpProcess(
            MemoryObject processMo,
            SystemProcess process,
            ProcessItem item,
            Dictionary<int, SystemProcess> processesDict,
            object handles
            )
        {
            int pid = process.Process.ProcessId;

            using (var general = processMo.CreateChild("General"))
            {
                BinaryWriter bw = new BinaryWriter(general.GetWriteStream());

                if (pid < 0)
                {
                    bw.Write("ProcessId", pid);
                    bw.Write("Name", process.Name);
                    bw.Write("ParentPid", 0);
                    bw.Write("HasParent", true);

                    if (item != null)
                        bw.Write("CpuUsage", item.CpuUsage.ToString());

                    bw.Close();

                    return;
                }

                bw.Write("ProcessId", pid);
                bw.Write("Name", pid != 0 ? process.Name : "System Idle Process");
                bw.Write("ParentPid", process.Process.InheritedFromProcessId);
                bw.Write("StartTime", DateTime.FromFileTime(process.Process.CreateTime));
                bw.Write("SessionId", process.Process.SessionId);

                bool hasParent = true;

                if (
                    !processesDict.ContainsKey(process.Process.InheritedFromProcessId) ||
                    process.Process.InheritedFromProcessId == process.Process.ProcessId
                    )
                {
                    hasParent = false;
                }
                else if (processesDict.ContainsKey(process.Process.InheritedFromProcessId))
                {
                    ulong parentStartTime =
                        (ulong)processesDict[process.Process.InheritedFromProcessId].Process.CreateTime;
                    ulong thisStartTime = (ulong)process.Process.CreateTime;

                    if (parentStartTime > thisStartTime)
                        hasParent = false;
                }

                bw.Write("HasParent", hasParent);

                try
                {
                    string fileName;

                    if (pid != 4)
                    {
                        using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                            fileName = FileUtils.GetFileName(phandle.GetImageFileName());
                    }
                    else
                    {
                        fileName = Windows.KernelFileName;
                    }

                    bw.Write("FileName", fileName);

                    var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(fileName);

                    bw.Write("FileDescription", info.FileDescription);
                    bw.Write("FileCompanyName", info.CompanyName);
                    bw.Write("FileVersion", info.FileVersion);

                    try
                    {
                        Icon icon;

                        icon = FileUtils.GetFileIcon(fileName, false);

                        if (icon != null)
                        {
                            using (var smallIcon = processMo.CreateChild("SmallIcon"))
                            {
                                using (var s = smallIcon.GetWriteStream())
                                {
                                    using (var b = icon.ToBitmap())
                                        b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }

                            Win32.DestroyIcon(icon.Handle);
                        }

                        icon = FileUtils.GetFileIcon(fileName, true);

                        if (icon != null)
                        {
                            using (var largeIcon = processMo.CreateChild("LargeIcon"))
                            {
                                using (var s = largeIcon.GetWriteStream())
                                {
                                    using (var b = icon.ToBitmap())
                                        b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }

                            Win32.DestroyIcon(icon.Handle);
                        }
                    }
                    catch
                    { }
                }
                catch
                { }

                try
                {
                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights | ProcessAccess.VmRead))
                    {
                        bw.Write("CommandLine", phandle.GetCommandLine());
                        bw.Write("CurrentDirectory", phandle.GetPebString(PebOffset.CurrentDirectoryPath));
                        bw.Write("IsPosix", phandle.IsPosix());
                    }
                }
                catch
                { }

                try
                {
                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                    {
                        if (OSVersion.Architecture == OSArch.Amd64)
                            bw.Write("IsWow64", phandle.IsWow64());
                    }

                    using (var phandle = new ProcessHandle(pid, ProcessAccess.QueryInformation))
                    {
                        bw.Write("IsBeingDebugged", phandle.IsBeingDebugged());
                        bw.Write("IsCritical", phandle.IsCritical());
                        bw.Write("DepStatus", (int)phandle.GetDepStatus());
                    }
                }
                catch
                { }

                bool userNameWritten = false;

                try
                {
                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                    {
                        using (var thandle = phandle.GetToken(TokenAccess.Query))
                        {
                            bw.Write("UserName", thandle.GetUser().GetFullName(true));
                            userNameWritten = true;

                            if (OSVersion.HasUac)
                                bw.Write("ElevationType", (int)thandle.GetElevationType());
                        }
                    }
                }
                catch
                { }

                if (!userNameWritten && pid <= 4)
                    bw.Write("UserName", "NT AUTHORITY\\SYSTEM");

                if (item != null)
                {
                    bw.Write("CpuUsage", item.CpuUsage.ToString());
                    bw.Write("JobName", item.JobName);
                    bw.Write("IsInJob", item.IsInJob);
                    bw.Write("IsInSignificantJob", item.IsInSignificantJob);
                    bw.Write("Integrity", item.Integrity);
                    bw.Write("IntegrityLevel", item.IntegrityLevel);
                    bw.Write("IsDotNet", item.IsDotNet);
                    bw.Write("IsPacked", item.IsPacked);
                    bw.Write("VerifyResult", (int)item.VerifyResult);
                    bw.Write("VerifySignerName", item.VerifySignerName);
                    bw.Write("ImportFunctions", item.ImportFunctions);
                    bw.Write("ImportModules", item.ImportModules);
                }

                bw.Close();
            }

            using (var vmCounters = processMo.CreateChild("VmCounters"))
                AppendStruct(vmCounters, new VmCountersEx64(process.Process.VirtualMemoryCounters));
            using (var ioCounters = processMo.CreateChild("IoCounters"))
                AppendStruct(ioCounters, process.Process.IoCounters);

            try
            {
                DumpProcessModules(processMo, pid);
            }
            catch
            { }

            try
            {
                DumpProcessToken(processMo, pid);
            }
            catch
            { }

            try
            {
                DumpProcessEnvironment(processMo, pid);
            }
            catch
            { }

            try
            {
                DumpProcessHandles(processMo, pid, handles);
            }
            catch
            { }

            //if (item != null)
            //{
            //    DumpProcessHistory(processMo, item.FloatHistoryManager.GetBuffer(ProcessStats.CpuKernel), "CpuKernel");
            //    DumpProcessHistory(processMo, item.FloatHistoryManager.GetBuffer(ProcessStats.CpuUser), "CpuUser");
            //    DumpProcessHistory(processMo, item.LongHistoryManager.GetBuffer(ProcessStats.IoRead), "IoRead");
            //    DumpProcessHistory(processMo, item.LongHistoryManager.GetBuffer(ProcessStats.IoWrite), "IoWrite");
            //    DumpProcessHistory(processMo, item.LongHistoryManager.GetBuffer(ProcessStats.IoOther), "IoOther");
            //    DumpProcessHistory(processMo, item.LongHistoryManager.GetBuffer(ProcessStats.IoReadOther), "IoReadOther");
            //    DumpProcessHistory(processMo, item.LongHistoryManager.GetBuffer(ProcessStats.PrivateMemory), "PrivateMemory");
            //    DumpProcessHistory(processMo, item.LongHistoryManager.GetBuffer(ProcessStats.WorkingSet), "WorkingSet");
            //}
        }

        private static void DumpProcessModules(MemoryObject processMo, int pid)
        {
            if (pid <= 0)
                return;

            using (var modules = processMo.CreateChild("Modules"))
            {
                if (pid != 4)
                {
                    var baseAddressList = new Dictionary<IntPtr, object>();
                    bool isWow64 = false;

                    using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights | ProcessAccess.VmRead))
                    {
                        if (OSVersion.Architecture == OSArch.Amd64)
                            isWow64 = phandle.IsWow64();

                        phandle.EnumModules((module) =>
                            {
                                if (!baseAddressList.ContainsKey(module.BaseAddress))
                                {
                                    DumpProcessModule(modules, module);
                                    baseAddressList.Add(module.BaseAddress, null);
                                }

                                return true;
                            });
                    }

                    try
                    {
                        using (var phandle = new ProcessHandle(pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                        {
                            phandle.EnumMemory((memory) =>
                                {
                                    if (memory.Type == MemoryType.Mapped)
                                    {
                                        if (!baseAddressList.ContainsKey(memory.BaseAddress))
                                        {
                                            string fileName = phandle.GetMappedFileName(memory.BaseAddress);

                                            if (fileName != null)
                                            {
                                                fileName = FileUtils.GetFileName(fileName);

                                                DumpProcessModule(modules, new ProcessModule(
                                                    memory.BaseAddress,
                                                    memory.RegionSize.ToInt32(),
                                                    IntPtr.Zero,
                                                    0,
                                                    Path.GetFileName(fileName),
                                                    fileName
                                                    ));

                                                baseAddressList.Add(memory.BaseAddress, null);
                                            }
                                        }
                                    }

                                    return true;
                                });
                        }
                    }
                    catch
                    { }

                    if (isWow64)
                    {
                        try
                        {
                            using (var buffer = new ProcessHacker.Native.Debugging.DebugBuffer())
                            {
                                buffer.Query(
                                    pid, 
                                    RtlQueryProcessDebugFlags.Modules32 |
                                    RtlQueryProcessDebugFlags.NonInvasive
                                    );

                                buffer.EnumModules((module) =>
                                    {
                                        if (!baseAddressList.ContainsKey(module.BaseAddress))
                                        {
                                            DumpProcessModule(modules, module);
                                            baseAddressList.Add(module.BaseAddress, null);
                                        }

                                        return true;
                                    });
                            }
                        }
                        catch
                        { }
                    }
                }
                else
                {
                    foreach (var module in Windows.GetKernelModules())
                        DumpProcessModule(modules, module);
                }
            }
        }

        private static void DumpProcessModule(MemoryObject modulesMo, ILoadedModule module)
        {
            using (var child = modulesMo.CreateChild(module.BaseAddress.ToString("x")))
            {
                BinaryWriter bw = new BinaryWriter(child.GetWriteStream());

                bw.Write("Name", module.BaseName);
                bw.Write("FileName", module.FileName);
                bw.Write("Size", module.Size);
                bw.Write("BaseAddress", module.BaseAddress);
                bw.Write("Flags", (int)module.Flags);

                try
                {
                    var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(module.FileName);

                    bw.Write("FileDescription", info.FileDescription);
                    bw.Write("FileCompanyName", info.CompanyName);
                    bw.Write("FileVersion", info.FileVersion);
                }
                catch
                { }

                bw.Close();
            }
        }

        private static void DumpProcessToken(MemoryObject processMo, int pid)
        {
            using (var tokenMo = processMo.CreateChild("Token"))
            {
                using (var phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                {
                    BinaryWriter bw = new BinaryWriter(tokenMo.GetWriteStream());

                    using (var thandle = phandle.GetToken(TokenAccess.Query))
                    {
                        Sid user = thandle.GetUser();

                        bw.Write("UserName", user.GetFullName(true));
                        bw.Write("UserStringSid", user.StringSid);
                        bw.Write("OwnerName", thandle.GetOwner().GetFullName(true));
                        bw.Write("PrimaryGroupName", thandle.GetPrimaryGroup().GetFullName(true));
                        bw.Write("SessionId", thandle.GetSessionId());

                        if (OSVersion.HasUac)
                        {
                            bw.Write("Elevated", thandle.IsElevated());
                            bw.Write("VirtualizationAllowed", thandle.IsVirtualizationAllowed());
                            bw.Write("VirtualizationEnabled", thandle.IsVirtualizationEnabled());
                        }

                        var statistics = thandle.GetStatistics();

                        bw.Write("Type", (int)statistics.TokenType);
                        bw.Write("ImpersonationLevel", (int)statistics.ImpersonationLevel);
                        bw.Write("Luid", statistics.TokenId.QuadPart);
                        bw.Write("AuthenticationLuid", statistics.AuthenticationId.QuadPart);
                        bw.Write("MemoryUsed", statistics.DynamicCharged);
                        bw.Write("MemoryAvailable", statistics.DynamicAvailable);

                        var groups = thandle.GetGroups();

                        using (var groupsMo = tokenMo.CreateChild("Groups"))
                        {
                            BinaryWriter bw2 = new BinaryWriter(groupsMo.GetWriteStream());

                            for (int i = 0; i < groups.Length; i++)
                            {
                                bw2.WriteListEntry(
                                    groups[i].GetFullName(true) + ";" + ((int)groups[i].Attributes).ToString("x")
                                    );
                            }

                            bw2.Close();
                        }

                        var privileges = thandle.GetPrivileges();

                        using (var privilegesMo = tokenMo.CreateChild("Privileges"))
                        {
                            BinaryWriter bw2 = new BinaryWriter(privilegesMo.GetWriteStream());

                            for (int i = 0; i < privileges.Length; i++)
                            {
                                bw2.WriteListEntry(
                                    privileges[i].Name + ";" +
                                    privileges[i].DisplayName + ";" +
                                    ((int)privileges[i].Attributes).ToString("x")
                                    );
                            }

                            bw2.Close();
                        }
                    }

                    try
                    {
                        using (var thandle = phandle.GetToken(TokenAccess.QuerySource))
                        {
                            var source = thandle.GetSource();

                            bw.Write("SourceName", source.SourceName.TrimEnd('\0', '\r', '\n', ' '));
                            bw.Write("SourceLuid", source.SourceIdentifier.QuadPart);
                        }
                    }
                    catch
                    { }
                                                              
                    bw.Close();
                }
            }
        }

        private static void DumpProcessEnvironment(MemoryObject processMo, int pid)
        {
            using (var envMo = processMo.CreateChild("Environment"))
            {
                using (var phandle = new ProcessHandle(pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                {
                    BinaryWriter bw = new BinaryWriter(envMo.GetWriteStream());

                    foreach (var kvp in phandle.GetEnvironmentVariables())
                    {
                        bw.Write(kvp.Key, kvp.Value);
                    }

                    bw.Close();
                }
            }
        }

        private static void DumpProcessHandles(MemoryObject processMo, int pid, object handlesIn)
        {
            using (var handlesChild = processMo.CreateChild("Handles"))
            {
                SystemHandleEntry[] handles = (SystemHandleEntry[])handlesIn;

                foreach (var handle in handles)
                {
                    if (handle.ProcessId != pid)
                        continue;

                    using (var child = handlesChild.CreateChild(handle.Handle.ToString("x")))
                    {
                        BinaryWriter bw = new BinaryWriter(child.GetWriteStream());

                        bw.Write("Handle", handle.Handle);
                        bw.Write("Flags", (int)handle.Flags);
                        bw.Write("Object", handle.Object);
                        bw.Write("GrantedAccess", handle.GrantedAccess);

                        try
                        {
                            var info = handle.GetHandleInfo();

                            bw.Write("TypeName", info.TypeName);
                            bw.Write("ObjectName", info.BestName);
                        }
                        catch
                        { }

                        bw.Close();
                    }
                }
            }
        }

        private static void DumpProcessHistory<T>(MemoryObject processMo, CircularBuffer<T> buffer, string name)
        {
            using (var child = processMo.CreateChild(name + "History"))
            {
                using (var s = child.GetWriteStream())
                    buffer.Save(s);
            }
        }

        public static void DumpServices(MemoryFileSystem mfs)
        {
            using (var services = mfs.RootObject.GetChild("Services"))
            {
                foreach (var service in Windows.GetServices().Values)
                {
                    using (var serviceChild = services.CreateChild(service.ServiceName))
                    {
                        BinaryWriter bw = new BinaryWriter(serviceChild.GetWriteStream());

                        bw.Write("Name", service.ServiceName);
                        bw.Write("DisplayName", service.DisplayName);
                        bw.Write("Type", (int)service.ServiceStatusProcess.ServiceType);
                        bw.Write("State", (int)service.ServiceStatusProcess.CurrentState);
                        bw.Write("ProcessId", service.ServiceStatusProcess.ProcessID);
                        bw.Write("ControlsAccepted", (int)service.ServiceStatusProcess.ControlsAccepted);
                        bw.Write("Flags", (int)service.ServiceStatusProcess.ServiceFlags);

                        try
                        {        
                            QueryServiceConfig config;

                            using (var shandle = new ServiceHandle(service.ServiceName, ServiceAccess.QueryConfig))
                            {
                                config = shandle.GetConfig();

                                bw.Write("StartType", (int)config.StartType);
                                bw.Write("ErrorControl", (int)config.ErrorControl);
                                bw.Write("BinaryPath", FileUtils.GetFileName(config.BinaryPathName));
                                bw.Write("Group", config.LoadOrderGroup);
                                bw.Write("UserName", config.ServiceStartName);

                                bw.Write("Description", shandle.GetDescription());
                            }

                            if (config.ServiceType == ServiceType.Win32ShareProcess)
                            {
                                try
                                {
                                    using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                                        "SYSTEM\\CurrentControlSet\\Services\\" + service.ServiceName + "\\Parameters"))
                                    {
                                        bw.Write(
                                            "ServiceDll",
                                            Environment.ExpandEnvironmentVariables((string)key.GetValue("ServiceDll"))
                                            );
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        catch
                        { }

                        bw.Close();
                    }
                }
            }
        }
    }
}
