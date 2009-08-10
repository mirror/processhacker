using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native
{
    public static class SystemHandleInformationExtensions
    {
        public static ObjectBasicInformation GetBasicInfo(this SystemHandleEntry thisHandle)
        {
            using (ProcessHandle process = new ProcessHandle(thisHandle.ProcessId, ProcessAccess.DupHandle))
            {
                return thisHandle.GetBasicInfo(process);
            }
        }

        public static ObjectBasicInformation GetBasicInfo(this SystemHandleEntry thisHandle, ProcessHandle process)
        {
            NtStatus status = NtStatus.Success;
            IntPtr handle = new IntPtr(thisHandle.Handle);
            IntPtr objectHandleI;
            GenericHandle objectHandle = null;
            int retLength;
            int baseAddress;

            if (KProcessHacker.Instance == null)
            {
                if ((status = Win32.NtDuplicateObject(
                    process, handle, ProcessHandle.Current, out objectHandleI, 0, 0, 0)) >= NtStatus.Error)
                    Win32.ThrowLastError();

                objectHandle = new GenericHandle(objectHandleI);
            }

            try
            {
                using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(ObjectBasicInformation))))
                {
                    if (KProcessHacker.Instance != null)
                    {
                        KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectBasicInformation,
                            data, data.Size, out retLength, out baseAddress);
                    }
                    else
                    {
                        status = Win32.NtQueryObject(objectHandle, ObjectInformationClass.ObjectBasicInformation,
                            data, data.Size, out retLength);
                    }

                    if (status >= NtStatus.Error)
                        Win32.ThrowLastError(status);

                    return data.ReadStruct<ObjectBasicInformation>();
                }
            }
            finally
            {
                if (objectHandle != null)
                    objectHandle.Dispose();
            }
        }

        private static string GetObjectNameNt(ProcessHandle process, IntPtr handle, GenericHandle dupHandle)
        {
            int retLength;
            int baseAddress = 0;

            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectNameInformation,
                    IntPtr.Zero, 0, out retLength, out baseAddress);
            }
            else
            {
                Win32.NtQueryObject(dupHandle, ObjectInformationClass.ObjectNameInformation,
                    IntPtr.Zero, 0, out retLength);
            }

            if (retLength > 0)
            {
                using (MemoryAlloc oniMem = new MemoryAlloc(retLength))
                {
                    if (KProcessHacker.Instance != null)
                    {
                        if (KProcessHacker.Instance.ZwQueryObject(process, handle, ObjectInformationClass.ObjectNameInformation,
                            oniMem, oniMem.Size, out retLength, out baseAddress) >= NtStatus.Error)
                            throw new Exception("ZwQueryObject failed.");
                    }
                    else
                    {
                        if (Win32.NtQueryObject(dupHandle, ObjectInformationClass.ObjectNameInformation,
                            oniMem, oniMem.Size, out retLength) >= NtStatus.Error)
                            throw new Exception("NtQueryObject failed.");
                    }

                    var oni = oniMem.ReadStruct<ObjectNameInformation>();
                    var str = oni.Name;

                    if (KProcessHacker.Instance != null)
                        str.Buffer = str.Buffer.Increment(-baseAddress + oniMem);

                    return str.Read();
                }
            }

            throw new Exception("NtQueryObject failed.");
        }

        public static ObjectInformation GetHandleInfo(this SystemHandleEntry thisHandle)
        {
            using (ProcessHandle process = new ProcessHandle(thisHandle.ProcessId,
                KProcessHacker.Instance != null ? OSVersion.MinProcessQueryInfoAccess : ProcessAccess.DupHandle))
            {
                return thisHandle.GetHandleInfo(process);
            }
        }

        public static ObjectInformation GetHandleInfo(this SystemHandleEntry thisHandle, ProcessHandle process)
        {
            IntPtr handle = new IntPtr(thisHandle.Handle);
            IntPtr objectHandleI;
            int retLength = 0;
            GenericHandle objectHandle = null;

            if (thisHandle.Handle == 0 || thisHandle.Handle == -1 || thisHandle.Handle == -2)
                throw new WindowsException(NtStatus.InvalidHandle);

            // Duplicate the handle if we're not using KPH
            if (KProcessHacker.Instance == null)
            {
                NtStatus status;

                if ((status = Win32.NtDuplicateObject(
                    process, handle, ProcessHandle.Current, out objectHandleI, 0, 0, 0)) >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                objectHandle = new GenericHandle(objectHandleI);
            }

            ObjectInformation info = new ObjectInformation();

            // If the cache contains the object type's name, use it. Otherwise, query the type 
            // for its name.
            lock (Windows.ObjectTypes)
            {
                if (Windows.ObjectTypes.ContainsKey(thisHandle.ObjectTypeNumber))
                {
                    info.TypeName = Windows.ObjectTypes[thisHandle.ObjectTypeNumber];
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
                                    otiMem, otiMem.Size, out retLength, out baseAddress) >= NtStatus.Error)
                                    throw new Exception("ZwQueryObject failed.");
                            }
                            else
                            {
                                if (Win32.NtQueryObject(objectHandle, ObjectInformationClass.ObjectTypeInformation,
                                    otiMem, otiMem.Size, out retLength) >= NtStatus.Error)
                                    throw new Exception("NtQueryObject failed.");
                            }

                            var oti = otiMem.ReadStruct<ObjectTypeInformation>();
                            var str = oti.Name;

                            if (KProcessHacker.Instance != null)
                                str.Buffer = str.Buffer.Increment(-baseAddress + otiMem);

                            info.TypeName = str.Read();
                            Windows.ObjectTypes.Add(thisHandle.ObjectTypeNumber, info.TypeName);
                        }
                    }
                }
            }

            // Get the object's name. If the object is a file we must take special 
            // precautions so that we don't hang.
            if (info.TypeName == "File")
            {
                if (KProcessHacker.Instance != null)
                {
                    // Use KProcessHacker for files to avoid hangs.
                    info.OrigName = KProcessHacker.Instance.GetHandleObjectName(process, handle);
                }
                else
                {
                    // 0: No hack, query the thing normally.
                    // 1: No hack, use NProcessHacker.
                    // 2: Hack.
                    int hackLevel = 1;

                    // Can't use NPH because XP had a bug where a thread hanging 
                    // on NtQueryObject couldn't be terminated.
                    if (OSVersion.IsBelowOrEqual(WindowsVersion.XP))
                        hackLevel = 2;

                    // On Windows 7 and above the hanging bug appears to have 
                    // been fixed. Query the object normally.
                    if (OSVersion.IsAboveOrEqual(WindowsVersion.Seven))
                        hackLevel = 0;

                    if (hackLevel == 1)
                    {
                        try
                        {
                            // Use NProcessHacker.
                            using (MemoryAlloc oniMem = new MemoryAlloc(0x4000))
                            {
                                if (NProcessHacker.PhQueryNameFileObject(
                                    objectHandle, oniMem, oniMem.Size, out retLength) >= NtStatus.Error)
                                    throw new Exception("PhQueryNameFileObject failed.");

                                var oni = oniMem.ReadStruct<ObjectNameInformation>();

                                info.OrigName = oni.Name.Read();
                            }
                        }
                        catch (DllNotFoundException)
                        {
                            hackLevel = 2;
                        }
                    }

                    if (hackLevel == 0)
                    {
                        info.OrigName = GetObjectNameNt(process, handle, objectHandle);
                    }
                    else if (hackLevel == 2)
                    {
                        // KProcessHacker and NProcessHacker not available. Fall back to using hack
                        // (i.e. not querying the name at all if the access is 0x0012019f).
                        if ((int)thisHandle.GrantedAccess != 0x0012019f)
                            info.OrigName = GetObjectNameNt(process, handle, objectHandle);
                    }
                }
            }
            else
            {
                // Not a file. Query the object normally.
                info.OrigName = GetObjectNameNt(process, handle, objectHandle);
            }

            // Get a better name for the handle.
            try
            {
                switch (info.TypeName)
                {
                    case "File":
                        // Resolves \Device\Harddisk1 into C:, for example.
                        if (!string.IsNullOrEmpty(info.OrigName))
                            info.BestName = FileUtils.DeviceFileNameToDos(info.OrigName);

                        break;

                    case "Key":
                        info.BestName = NativeUtils.FormatNativeKeyName(info.OrigName);

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
                                using (var processHandle =
                                    new NativeHandle<ProcessAccess>(process, handle, OSVersion.MinProcessQueryInfoAccess))
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
                                using (var threadHandle =
                                    new NativeHandle<ThreadAccess>(process, handle, OSVersion.MinThreadQueryInfoAccess))
                                {
                                    var basicInfo = ThreadHandle.FromHandle(threadHandle).GetBasicInformation();

                                    threadId = basicInfo.ClientId.ThreadId;
                                    processId = basicInfo.ClientId.ProcessId;
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

                    case "TmEn":
                        {
                            using (var enHandleDup =
                                new NativeHandle<EnlistmentAccess>(process, handle, EnlistmentAccess.QueryInformation))
                            {
                                var enHandle = EnlistmentHandle.FromHandle(enHandleDup);

                                info.BestName = enHandle.GetBasicInformation().EnlistmentId.ToString("B");
                            }
                        }
                        break;

                    case "TmRm":
                        {
                            using (var rmHandleDup =
                                new NativeHandle<ResourceManagerAccess>(process, handle, ResourceManagerAccess.QueryInformation))
                            {
                                var rmHandle = ResourceManagerHandle.FromHandle(rmHandleDup);

                                info.BestName = rmHandle.GetDescription();

                                if (string.IsNullOrEmpty(info.BestName))
                                    info.BestName = rmHandle.GetGuid().ToString("B");
                            }
                        }
                        break;

                    case "TmTm":
                        {
                            using (var tmHandleDup =
                                new NativeHandle<TmAccess>(process, handle, TmAccess.QueryInformation))
                            {
                                var tmHandle = TmHandle.FromHandle(tmHandleDup);

                                info.BestName = FileUtils.FixPath(FileUtils.DeviceFileNameToDos(tmHandle.GetLogFileName()));

                                if (string.IsNullOrEmpty(info.BestName))
                                    info.BestName = tmHandle.GetBasicInformation().TmIdentity.ToString("B");
                            }
                        }
                        break;

                    case "TmTx":
                        {
                            using (var transactionHandleDup =
                                new NativeHandle<TransactionAccess>(process, handle, TransactionAccess.QueryInformation))
                            {
                                var transactionHandle = TransactionHandle.FromHandle(transactionHandleDup);

                                info.BestName = transactionHandle.GetDescription();

                                if (string.IsNullOrEmpty(info.BestName))
                                    info.BestName = transactionHandle.GetBasicInformation().TransactionId.ToString("B");
                            }
                        }
                        break;

                    case "Token":
                        {
                            using (var tokenHandleDup =
                                new NativeHandle<TokenAccess>(process, handle, TokenAccess.Query))
                            {
                                var tokenHandle = TokenHandle.FromHandle(tokenHandleDup);
                                var sid = tokenHandle.GetUser();

                                using (sid)
                                    info.BestName = sid.GetFullName(true) + ": 0x" +
                                        tokenHandle.GetStatistics().AuthenticationId.ToString();
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
}
