/*
 * Process Hacker - 
 *   process handle
 * 
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
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows process.
    /// </summary>
    /// <remarks>
    /// The idea of a ProcessHandle class is 
    /// different to the <see cref="System.Diagnostics.Process"/> class; 
    /// instead of opening the process with the right permissions every 
    /// time a query or set function is called, this lets the users control 
    /// when they want to open handles with certain permissions. This 
    /// means that handles can be cached (by the users).
    /// </remarks>
    public sealed class ProcessHandle : NativeHandle<ProcessAccess>, IWithToken
    {
        /// <summary>
        /// The callback for enumerating process memory regions.
        /// </summary>
        /// <param name="info">The basic information for the memory region.</param>
        /// <returns>Return true to continue enumerating; return false to stop.</returns>
        public delegate bool EnumMemoryDelegate(MemoryBasicInformation info);

        /// <summary>
        /// The callback for enumerating process modules.
        /// </summary>
        /// <param name="module">The module information.</param>
        /// <returns>Return true to continue enumerating; return false to stop.</returns>
        public delegate bool EnumModulesDelegate(ProcessModule module);

        private static readonly ProcessHandle _current = new ProcessHandle(new IntPtr(-1), false);

        /// <summary>
        /// Gets a handle to the current process.
        /// </summary>
        public static ProcessHandle Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Creates a process.
        /// </summary>
        /// <param name="access">The desired access to the new process.</param>
        /// <param name="parentProcess">The process to inherit the address space and handles from.</param>
        /// <param name="inheritHandles">Specify true to inherit handles, otherwise false.</param>
        /// <param name="sectionHandle">A section of an executable image.</param>
        /// <returns>A handle to the new process.</returns>
        public static ProcessHandle Create(
            ProcessAccess access,
            ProcessHandle parentProcess,
            bool inheritHandles,
            SectionHandle sectionHandle)
        {
            return Create(access, null, 0, null, parentProcess, inheritHandles, sectionHandle, null);
        }

        /// <summary>
        /// Creates a process.
        /// </summary>
        /// <param name="access">The desired access to the new process.</param>
        /// <param name="name">The name of the process.</param>
        /// <param name="objectFlags">The flags to use when creating the object.</param>
        /// <param name="rootDirectory">A handle to the directory in which to place the object.</param>
        /// <param name="parentProcess">The process to inherit the address space and handles from.</param>
        /// <param name="inheritHandles">Specify true to inherit handles, otherwise false.</param>
        /// <param name="sectionHandle">A section of an executable image.</param>
        /// <param name="debugPort">A debug object to attach the process to.</param>
        /// <returns>A handle to the new process.</returns>
        public static ProcessHandle Create(
            ProcessAccess access,
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            ProcessHandle parentProcess,
            bool inheritHandles,
            SectionHandle sectionHandle,
            DebugObjectHandle debugPort
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                if ((status = Win32.NtCreateProcess(
                    out handle,
                    access,
                    ref oa,
                    parentProcess ?? IntPtr.Zero,
                    inheritHandles,
                    sectionHandle ?? IntPtr.Zero,
                    debugPort ?? IntPtr.Zero,
                    IntPtr.Zero
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oa.Dispose();
            }

            return new ProcessHandle(handle, true);
        }

        public static ProcessHandle CreateExtended(
            string fileName,
            ProcessHandle parentProcess,
            ProcessCreationFlags creationFlags,
            bool inheritHandles,
            string currentDirectory,
            StartupInfo startupInfo,
            out ClientId clientId,
            out ThreadHandle threadHandle
            )
        {
            return CreateExtended(
                fileName,
                parentProcess,
                creationFlags,
                true,
                inheritHandles,
                EnvironmentBlock.GetCurrent(),
                currentDirectory,
                startupInfo,
                out clientId,
                out threadHandle
                );
        }

        public static ProcessHandle CreateExtended(
            string fileName,
            ProcessHandle parentProcess,
            ProcessCreationFlags creationFlags,
            bool notifyCsr,
            bool inheritHandles,
            EnvironmentBlock environment,
            string currentDirectory,
            StartupInfo startupInfo,
            out ClientId clientId,
            out ThreadHandle threadHandle
            )
        {
            ProcessHandle phandle;
            ThreadHandle thandle;
            SectionImageInformation imageInfo;

            // If we don't have a desktop, use the current one.
            if (startupInfo.Desktop == null)
                startupInfo.Desktop = ProcessHandle.Current.GetPebString(PebOffset.DesktopName);

            // Open the file, create a section, and create a process.
            using (var fhandle = new FileHandle(
                fileName,
                FileShareMode.Read | FileShareMode.Delete,
                FileAccess.Execute | (FileAccess)StandardRights.Synchronize
                ))
            {
                using (var shandle = SectionHandle.Create(
                    SectionAccess.All,
                    SectionAttributes.Image,
                    MemoryProtection.Execute,
                    fhandle
                    ))
                {
                    imageInfo = shandle.GetImageInformation();

                    phandle = Create(
                        ProcessAccess.All,
                        parentProcess,
                        inheritHandles,
                        shandle
                        );
                }
            }

            IntPtr peb = phandle.GetBasicInformation().PebBaseAddress;

            // Copy the process parameters across.
            NativeUtils.CopyProcessParameters(
                phandle,
                peb,
                creationFlags,
                FileUtils.GetFileName(fileName),
                ProcessHandle.Current.GetPebString(PebOffset.DllPath),
                currentDirectory,
                fileName,
                environment,
                startupInfo.Title != null ? startupInfo.Title : fileName,
                startupInfo.Desktop != null ? startupInfo.Desktop : "",
                startupInfo.Reserved != null ? startupInfo.Reserved : "",
                "",
                ref startupInfo
                );

            // TODO: Duplicate the console handles (stdin, stdout, stderr).

            // Create the initial thread.
            thandle = ThreadHandle.CreateUserThread(
                phandle,
                true,
                imageInfo.StackCommit.Increment(imageInfo.StackReserved).ToInt32(),
                imageInfo.StackCommit.ToInt32(),
                imageInfo.TransferAddress,
                IntPtr.Zero,
                out clientId
                );

            // Notify CSR.

            if (notifyCsr)
            {
                BaseCreateProcessMsg processMsg = new BaseCreateProcessMsg();

                processMsg.ProcessHandle = phandle;
                processMsg.ThreadHandle = thandle;
                processMsg.ClientId = clientId;
                processMsg.CreationFlags = creationFlags;

                if ((creationFlags & (ProcessCreationFlags.DebugProcess |
                    ProcessCreationFlags.DebugOnlyThisProcess)) != 0)
                {
                    NtStatus status;

                    status = Win32.DbgUiConnectToDbg();

                    if (status >= NtStatus.Error)
                    {
                        phandle.Terminate(status);
                        Win32.Throw(status);
                    }

                    processMsg.DebuggerClientId = ThreadHandle.GetCurrentCid();
                }

                // If this is a GUI program, set the 1 and 2 bits to turn the 
                // hourglass cursor on.
                if (imageInfo.ImageSubsystem == 2)
                    processMsg.ProcessHandle = processMsg.ProcessHandle.Or((1 | 2).ToIntPtr());
                // We still have to honor the startup info settings, though.
                if ((startupInfo.Flags & StartupFlags.ForceOnFeedback) ==
                    StartupFlags.ForceOnFeedback)
                    processMsg.ProcessHandle = processMsg.ProcessHandle.Or((1).ToIntPtr());
                if ((startupInfo.Flags & StartupFlags.ForceOffFeedback) ==
                    StartupFlags.ForceOffFeedback)
                    processMsg.ProcessHandle = processMsg.ProcessHandle.And((1).ToIntPtr().Not());

                using (var data = new MemoryAlloc(
                    CsrApiMsg.ApiMessageDataOffset + Marshal.SizeOf(typeof(BaseCreateProcessMsg))
                    ))
                {
                    data.WriteStruct<BaseCreateProcessMsg>(CsrApiMsg.ApiMessageDataOffset, 0, processMsg);

                    Win32.CsrClientCallServer(
                        data,
                        IntPtr.Zero,
                        Win32.CsrMakeApiNumber(Win32.BaseSrvServerDllIndex, (int)BaseSrvApiNumber.BasepCreateProcess),
                        Marshal.SizeOf(typeof(BaseCreateProcessMsg))
                        );

                    NtStatus status = (NtStatus)data.ReadStruct<CsrApiMsg>().ReturnValue;

                    if (status >= NtStatus.Error)
                    {
                        phandle.Terminate(status);
                        Win32.Throw(status);
                    }
                }
            }

            if ((creationFlags & ProcessCreationFlags.CreateSuspended) == 0)
                thandle.Resume();

            threadHandle = thandle;

            return phandle;
        }

        public static ProcessHandle CreateUserProcess(string fileName, out ClientId clientId, out ThreadHandle threadHandle)
        {
            NtStatus status;
            UnicodeString fileNameStr = new UnicodeString(fileName);
            RtlUserProcessParameters processParams = new RtlUserProcessParameters();
            RtlUserProcessInformation processInfo;

            processParams.Length = Marshal.SizeOf(processParams);
            processParams.MaximumLength = processParams.Length;
            processParams.ImagePathName = new UnicodeString(fileName);
            processParams.CommandLine = new UnicodeString(fileName);

            Win32.RtlCreateEnvironment(true, out processParams.Environment);

            try
            {
                if ((status = Win32.RtlCreateUserProcess(
                    ref fileNameStr,
                    0,
                    ref processParams,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out processInfo
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                clientId = processInfo.ClientId;
                threadHandle = new ThreadHandle(processInfo.Thread, true);

                return new ProcessHandle(processInfo.Process, true);
            }
            finally
            {
                fileNameStr.Dispose();
                processParams.ImagePathName.Dispose();
                processParams.CommandLine.Dispose();
                Win32.RtlDestroyEnvironment(processParams.Environment);
            }
        }

        public static ProcessHandle CreateWin32(
            string applicationName,
            string commandLine,
            bool inheritHandles,
            ProcessCreationFlags creationFlags,
            EnvironmentBlock environment,
            string currentDirectory,
            StartupInfo startupInfo,
            out ClientId clientId,
            out ThreadHandle threadHandle
            )
        {
            ProcessInformation processInformation;

            startupInfo.Size = Marshal.SizeOf(typeof(StartupInfo));

            if (!Win32.CreateProcess(
                applicationName,
                commandLine,
                IntPtr.Zero,
                IntPtr.Zero,
                inheritHandles,
                creationFlags,
                environment,
                currentDirectory,
                ref startupInfo,
                out processInformation
                ))
                Win32.Throw();

            clientId = new ClientId(processInformation.ProcessId, processInformation.ThreadId);
            threadHandle = new ThreadHandle(processInformation.ThreadHandle, true);

            return new ProcessHandle(processInformation.ProcessHandle, true);
        }

        public static ProcessHandle CreateWin32(
            TokenHandle tokenHandle,
            string applicationName,
            string commandLine,
            bool inheritHandles,
            ProcessCreationFlags creationFlags,
            EnvironmentBlock environment,
            string currentDirectory,
            StartupInfo startupInfo,
            out ClientId clientId,
            out ThreadHandle threadHandle
            )
        {
            ProcessInformation processInformation;

            startupInfo.Size = Marshal.SizeOf(typeof(StartupInfo));

            if (!Win32.CreateProcessAsUser(
                tokenHandle,
                applicationName,
                commandLine,
                IntPtr.Zero,
                IntPtr.Zero,
                inheritHandles,
                creationFlags,
                environment,
                currentDirectory,
                ref startupInfo,
                out processInformation
                ))
                Win32.Throw();

            clientId = new ClientId(processInformation.ProcessId, processInformation.ThreadId);
            threadHandle = new ThreadHandle(processInformation.ThreadHandle, true);

            return new ProcessHandle(processInformation.ProcessHandle, true);
        }

        /// <summary>
        /// Creates a process handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="handle">The handle value.</param>
        /// <returns>The process handle.</returns>
        public static ProcessHandle FromHandle(IntPtr handle)
        {
            return new ProcessHandle(handle, false);
        }

        /// <summary>
        /// Gets a handle to the current process.
        /// </summary>
        /// <returns>A process handle.</returns>
        public static ProcessHandle GetCurrent()
        {
            return Current;
        }

        /// <summary>
        /// Gets the ID of the current process.
        /// </summary>
        /// <returns>The ID of the current process.</returns>
        public static int GetCurrentId()
        {
            return Win32.GetCurrentProcessId();
        }

        /// <summary>
        /// Gets a pointer to the current process' environment block.
        /// </summary>
        /// <returns>A pointer to the current PEB.</returns>
        public unsafe static Peb* GetCurrentPeb()
        {
            return (Peb*)ThreadHandle.GetCurrentTeb()->ProcessEnvironmentBlock;
        }

        public unsafe static RtlUserProcessParameters* GetCurrentProcessParameters()
        {
            return (RtlUserProcessParameters*)GetCurrentPeb()->ProcessParameters;
        }

        private static int GetPebOffset(PebOffset offset)
        {
            switch (offset)
            {
                case PebOffset.CommandLine:
                    return RtlUserProcessParameters.CommandLineOffset;
                case PebOffset.CurrentDirectoryPath:
                    return RtlUserProcessParameters.CurrentDirectoryOffset;
                case PebOffset.DesktopName:
                    return RtlUserProcessParameters.DesktopInfoOffset;
                case PebOffset.DllPath:
                    return RtlUserProcessParameters.DllPathOffset;
                case PebOffset.ImagePathName:
                    return RtlUserProcessParameters.ImagePathNameOffset;
                case PebOffset.RuntimeData:
                    return RtlUserProcessParameters.RuntimeDataOffset;
                case PebOffset.ShellInfo:
                    return RtlUserProcessParameters.ShellInfoOffset;
                case PebOffset.WindowTitle:
                    return RtlUserProcessParameters.WindowTitleOffset;
                default:
                    throw new ArgumentException("offset");
            }
        }

        /// <summary>
        /// Opens processes with the specified name.
        /// </summary>
        /// <param name="processName">The names of the processes to open.</param>
        /// <param name="access">The desired access to the processes.</param>
        /// <returns>An array of process handles.</returns>
        public static ProcessHandle[] OpenByName(string processName, ProcessAccess access)
        {
            var processes = Windows.GetProcesses();
            List<ProcessHandle> processHandles = new List<ProcessHandle>();

            foreach (var process in processes.Values)
            {
                if (string.Equals(process.Name, processName, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        processHandles.Add(new ProcessHandle(process.Process.ProcessId, access));
                    }
                    catch
                    { }
                }
            }

            return processHandles.ToArray();
        }

        /// <summary>
        /// Opens a handle to the current process.
        /// </summary>
        /// <param name="access">The desired access to the current process.</param>
        /// <returns>A handle.</returns>
        public static ProcessHandle OpenCurrent(ProcessAccess access)
        {
            return new ProcessHandle(GetCurrentId(), access);
        }

        public static ProcessHandle OpenWithAnyAccess(int pid)
        {
            try
            {
                return new ProcessHandle(pid, OSVersion.MinProcessQueryInfoAccess);
            }
            catch
            {
                try
                {
                    return new ProcessHandle(pid, (ProcessAccess)StandardRights.Synchronize);
                }
                catch
                {
                    try
                    {
                        return new ProcessHandle(pid, (ProcessAccess)StandardRights.ReadControl);
                    }
                    catch
                    {
                        try
                        {
                            return new ProcessHandle(pid, (ProcessAccess)StandardRights.WriteDac);
                        }
                        catch
                        {
                            return new ProcessHandle(pid, (ProcessAccess)StandardRights.WriteOwner);
                        }
                    }
                }
            }
        }

        private ProcessHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Opens a process.
        /// </summary>
        /// <param name="pid">The ID of the process to open.</param>
        public ProcessHandle(int pid)
            : this(pid, ProcessAccess.All)
        { }

        /// <summary>
        /// Opens a process.
        /// </summary>
        /// <param name="pid">The ID of the process to open.</param>
        /// <param name="access">The desired access to the process.</param>
        public ProcessHandle(int pid, ProcessAccess access)
        {
            // If we have KPH, use it.
            if (KProcessHacker.Instance != null)
            {
                try
                {
                    this.Handle = new IntPtr(KProcessHacker.Instance.KphOpenProcess(pid, access));
                }
                catch (WindowsException)
                {
                    // This would only happen if the process is DRM-protected or if 
                    // some part of ObReferenceObjectByHandle is hooked. We can 
                    // open the process with SYNCHRONIZE access and set the granted access 
                    // using KPH.
                    this.Handle = new IntPtr(KProcessHacker.Instance.KphOpenProcess(pid,
                        (ProcessAccess)StandardRights.Synchronize));
                    KProcessHacker.Instance.KphSetHandleGrantedAccess(this.Handle, (int)access);
                }
            }
            else
            {
                this.Handle = Win32.OpenProcess(access, false, pid);
            }

            if (this.Handle == IntPtr.Zero)
            {
                this.MarkAsInvalid();
                Win32.Throw();
            }
        }

        /// <summary>
        /// Opens a thread's process.
        /// </summary>
        /// <param name="threadHandle">A handle to a thread.</param>
        /// <param name="access">The desired access to the process.</param>
        public ProcessHandle(ThreadHandle threadHandle, ProcessAccess access)
        {
            if (KProcessHacker.Instance == null)
                throw new NotSupportedException();

            this.Handle = new IntPtr(KProcessHacker.Instance.KphOpenThreadProcess(threadHandle, access));
        }

        /// <summary>
        /// Opens a process.
        /// </summary>
        /// <param name="name">The name of the process.</param>
        /// <param name="objectFlags">The flags to use when opening the object.</param>
        /// <param name="rootDirectory">
        /// A handle to the directory in which the object is located.
        /// </param>
        /// <param name="clientId">A Client ID structure describing the process.</param>
        /// <param name="access">The desired access to the process.</param>
        public ProcessHandle(
            string name,
            ObjectFlags objectFlags,
            DirectoryHandle rootDirectory,
            ClientId clientId,
            ProcessAccess access
            )
        {
            NtStatus status;
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);
            IntPtr handle;

            try
            {
                // NtOpenProcess fails when both a client ID and a name is specified.
                if (name != null)
                {
                    // Name specified, don't specify a CID.
                    if ((status = Win32.NtOpenProcess(
                        out handle,
                        access,
                        ref oa,
                        IntPtr.Zero
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
                else
                {
                    // No name, specify a CID.
                    if ((status = Win32.NtOpenProcess(
                        out handle,
                        access,
                        ref oa,
                        ref clientId
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
            }
            finally
            {
                oa.Dispose();
            }

            this.Handle = handle;
        }

        /// <summary>
        /// Opens a process.
        /// </summary>
        /// <param name="name">The name of the process.</param>
        /// <param name="access">The desired access to the process.</param>
        public ProcessHandle(string name, ProcessAccess access)
            : this(name, 0, null, new ClientId(), access)
        { }

        /// <summary>
        /// Opens a process.
        /// </summary>
        /// <param name="clientId">A Client ID structure describing the process.</param>
        /// <param name="access">The desired access to the process.</param>
        public ProcessHandle(ClientId clientId, ProcessAccess access)
            : this(null, 0, null, clientId, access)
        { }

        /// <summary>
        /// Allocates a memory region in the process' virtual memory. The function decides where 
        /// to allocate the memory.
        /// </summary>
        /// <param name="size">The size of the region.</param>
        /// <param name="protection">The protection of the region.</param>
        /// <returns>The base address of the allocated pages.</returns>
        public IntPtr AllocateMemory(int size, MemoryProtection protection)
        {
            return this.AllocateMemory(size, MemoryFlags.Commit, protection);
        }

        /// <summary>
        /// Allocates a memory region in the process' virtual memory. The function decides where 
        /// to allocate the memory.
        /// </summary>
        /// <param name="size">The size of the region.</param>
        /// <param name="type">The type of allocation.</param>
        /// <param name="protection">The protection of the region.</param>
        /// <returns>The base address of the allocated pages.</returns>
        public IntPtr AllocateMemory(int size, MemoryFlags type, MemoryProtection protection)
        {
            return this.AllocateMemory(IntPtr.Zero, size, type, protection);
        }

        /// <summary>
        /// Allocates a memory region in the process' virtual memory.
        /// </summary>      
        /// <param name="baseAddress">The base address of the region.</param>
        /// <param name="size">The size of the region.</param>
        /// <param name="type">The type of allocation.</param>
        /// <param name="protection">The protection of the region.</param>
        /// <returns>The base address of the allocated pages.</returns>
        public IntPtr AllocateMemory(IntPtr baseAddress, int size, MemoryFlags type, MemoryProtection protection)
        {
            IntPtr sizeIntPtr = new IntPtr(size);

            return this.AllocateMemory(baseAddress, ref sizeIntPtr, type, protection);
        }

        /// <summary>
        /// Allocates a memory region in the process' virtual memory.
        /// </summary>      
        /// <param name="baseAddress">The base address of the region.</param>
        /// <param name="size">
        /// The size of the region. This variable will be modified to contain 
        /// the actual allocated size.
        /// </param>
        /// <param name="type">The type of allocation.</param>
        /// <param name="protection">The protection of the region.</param>
        /// <returns>The base address of the allocated pages.</returns>
        public IntPtr AllocateMemory(IntPtr baseAddress, ref IntPtr size, MemoryFlags type, MemoryProtection protection)
        {
            NtStatus status;

            if ((status = Win32.NtAllocateVirtualMemory(
                this,
                ref baseAddress,
                IntPtr.Zero,
                ref size,
                type,
                protection
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return baseAddress;
        }

        /// <summary>
        /// Assigns the process to a job object. The job handle must have the 
        /// JOB_OBJECT_ASSIGN_PROCESS permission and the process handle must have 
        /// the PROCESS_SET_QUOTA and PROCESS_TERMINATE permissions.
        /// </summary>
        /// <param name="job">The job object to assign the process to.</param>
        public void AssignToJobObject(JobObjectHandle job)
        {
            if (!Win32.AssignProcessToJobObject(job, this))
                Win32.Throw();
        }

        /// <summary>
        /// Creates a thread in the process.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution.</param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <returns>A handle to the new thread.</returns>
        /// <remarks>This function will work across sessions, unlike CreateThreadWin32.</remarks>
        public ThreadHandle CreateThread(IntPtr startAddress, IntPtr parameter)
        {
            return this.CreateThread(startAddress, parameter, false);
        }

        /// <summary>
        /// Creates a thread in the process.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution.</param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <param name="createSuspended">Whether to create the thread suspended.</param>
        /// <returns>A handle to the new thread.</returns>
        /// <remarks>This function will work across sessions, unlike CreateThreadWin32.</remarks>
        public ThreadHandle CreateThread(IntPtr startAddress, IntPtr parameter, bool createSuspended)
        {
            int threadId;

            return this.CreateThread(startAddress, parameter, createSuspended, out threadId);
        }

        /// <summary>
        /// Creates a thread in the process.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution.</param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <param name="createSuspended">Whether to create the thread suspended.</param>
        /// <param name="threadId">The ID of the new thread.</param>
        /// <returns>A handle to the new thread.</returns>
        /// <remarks>This function will work across sessions, unlike CreateThreadWin32.</remarks>
        public ThreadHandle CreateThread(IntPtr startAddress, IntPtr parameter, bool createSuspended, out int threadId)
        {
            ClientId cid;

            ThreadHandle thandle = ThreadHandle.CreateUserThread(
                this,
                createSuspended,
                0,
                0,
                startAddress,
                parameter,
                out cid
                );

            threadId = cid.ThreadId;

            return thandle;
        }

        /// <summary>
        /// Creates a thread in the process and notifies the Win32 subsystem.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution.</param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <returns>A handle to the new thread.</returns>
        public ThreadHandle CreateThreadWin32(IntPtr startAddress, IntPtr parameter)
        {
            return this.CreateThreadWin32(startAddress, parameter, false);
        }

        /// <summary>
        /// Creates a thread in the process and notifies the Win32 subsystem.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution.</param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <param name="createSuspended">Whether to create the thread suspended.</param>
        /// <returns>A handle to the new thread.</returns>
        public ThreadHandle CreateThreadWin32(IntPtr startAddress, IntPtr parameter, bool createSuspended)
        {
            int threadId;

            return this.CreateThreadWin32(startAddress, parameter, createSuspended, out threadId);
        }

        /// <summary>
        /// Creates a thread in the process and notifies the Win32 subsystem.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution.</param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <param name="createSuspended">Whether to create the thread suspended.</param>
        /// <param name="threadId">The ID of the new thread.</param>
        /// <returns>A handle to the new thread.</returns>
        public ThreadHandle CreateThreadWin32(IntPtr startAddress, IntPtr parameter, bool createSuspended, out int threadId)
        {
            IntPtr threadHandle;

            if ((threadHandle = Win32.CreateRemoteThread(
                this,
                IntPtr.Zero,
                IntPtr.Zero,
                startAddress,
                parameter,
                createSuspended ? ProcessCreationFlags.CreateSuspended : 0,
                out threadId
                )) == IntPtr.Zero)
                Win32.Throw();

            return new ThreadHandle(threadHandle, true);
        }

        /// <summary>
        /// Debugs the process with the specified debug object. This requires 
        /// PROCESS_SUSPEND_RESUME access.
        /// </summary>
        /// <param name="debugObjectHandle">A handle to a debug object.</param>
        public void Debug(DebugObjectHandle debugObjectHandle)
        {
            NtStatus status;

            if ((status = Win32.NtDebugActiveProcess(this, debugObjectHandle)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Disables the collection of handle stack traces. This requires 
        /// PROCESS_SET_INFORMATION access. Note that this function is only 
        /// available on Windows Vista and above.
        /// </summary>
        public void DisableHandleTracing()
        {
            NtStatus status;

            // Length 0 and NULL disables handle tracing.
            if ((status = Win32.NtSetInformationProcess(
                this,
                ProcessInformationClass.ProcessHandleTracing,
                IntPtr.Zero,
                0
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Removes as many pages as possible from the process' working set. This requires the 
        /// PROCESS_QUERY_INFORMATION and PROCESS_SET_INFORMATION permissions.
        /// </summary>
        public void EmptyWorkingSet()
        {
            if (!Win32.EmptyWorkingSet(this))
                Win32.Throw();
        }

        /// <summary>
        /// Enables the collection of handle stack traces. This requires 
        /// PROCESS_SET_INFORMATION access.
        /// </summary>
        public void EnableHandleTracing()
        {
            NtStatus status;
            ProcessHandleTracingEnable phte = new ProcessHandleTracingEnable();

            if ((status = Win32.NtSetInformationProcess(
                this,
                ProcessInformationClass.ProcessHandleTracing,
                ref phte,
                Marshal.SizeOf(phte)
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Enumerates the memory regions of the process.
        /// </summary>
        /// <param name="enumMemoryCallback">The callback for the enumeration.</param>
        public void EnumMemory(EnumMemoryDelegate enumMemoryCallback)
        {
            IntPtr address = IntPtr.Zero;
            MemoryBasicInformation mbi = new MemoryBasicInformation();
            int mbiSize = Marshal.SizeOf(mbi);

            while (Win32.VirtualQueryEx(this, address, out mbi, mbiSize) != 0)
            {
                if (!enumMemoryCallback(mbi))
                    break;

                address = address.Increment(mbi.RegionSize);
            }
        }

        /// <summary>
        /// Enumerates the modules loaded by the process.
        /// </summary>
        /// <param name="enumModulesCallback">The callback for the enumeration.</param>
        public void EnumModules(EnumModulesDelegate enumModulesCallback)
        {
            this.EnumModulesNative(enumModulesCallback);
        }

        /// <summary>
        /// Enumerates the modules loaded by the process using PSAPI.
        /// </summary>
        /// <param name="enumModulesCallback">The callback for the enumeration.</param>
        private void EnumModulesApi(EnumModulesDelegate enumModulesCallback)
        {
            IntPtr[] moduleHandles;
            int requiredSize;

            Win32.EnumProcessModules(this, null, 0, out requiredSize);
            moduleHandles = new IntPtr[requiredSize / 4];

            if (!Win32.EnumProcessModules(this, moduleHandles, requiredSize, out requiredSize))
                Win32.Throw();

            for (int i = 0; i < moduleHandles.Length; i++)
            {
                ModuleInfo moduleInfo = new ModuleInfo();
                StringBuilder baseName = new StringBuilder(0x400);
                StringBuilder fileName = new StringBuilder(0x400);

                if (!Win32.GetModuleInformation(this, moduleHandles[i], moduleInfo, Marshal.SizeOf(moduleInfo)))
                    Win32.Throw();
                if (Win32.GetModuleBaseName(this, moduleHandles[i], baseName, baseName.Capacity * 2) == 0)
                    Win32.Throw();
                if (Win32.GetModuleFileNameEx(this, moduleHandles[i], fileName, fileName.Capacity * 2) == 0)
                    Win32.Throw();

                if (!enumModulesCallback(new ProcessModule(
                    moduleInfo.BaseOfDll, moduleInfo.SizeOfImage, moduleInfo.EntryPoint, 0,
                    baseName.ToString(), FileUtils.GetFileName(fileName.ToString())
                    )))
                    break;
            }
        }

        /// <summary>
        /// Enumerates the modules loaded by the process by reading the NT loader data.
        /// </summary>
        /// <param name="enumModulesCallback">The callback for the enumeration.</param>
        private unsafe void EnumModulesNative(EnumModulesDelegate enumModulesCallback)
        {
            byte* buffer = stackalloc byte[IntPtr.Size];

            // Get the loader data table address.
            this.ReadMemory(this.GetBasicInformation().PebBaseAddress.Increment(Peb.LdrOffset), buffer, IntPtr.Size);

            IntPtr loaderData = *(IntPtr*)buffer;

            PebLdrData data;
            // Read the loader data table structure.
            this.ReadMemory(loaderData, &data, Marshal.SizeOf(typeof(PebLdrData)));

            if (!data.Initialized)
                throw new Exception("Loader data is not initialized.");

            IntPtr currentLink = data.InLoadOrderModuleList.Flink;
            IntPtr startLink = currentLink;
            LdrDataTableEntry currentEntry;
            int i = 0;

            while (currentLink != IntPtr.Zero)
            {
                // Stop when we have reached the beginning of the linked list.
                if (i > 0 && currentLink == startLink)
                    break;
                // Safety guard.
                if (i > 0x800)
                    break;

                // Read the loader data table entry.
                this.ReadMemory(currentLink, &currentEntry, Marshal.SizeOf(typeof(LdrDataTableEntry)));

                // Check if the entry is valid.
                if (currentEntry.DllBase != IntPtr.Zero)
                {
                    string baseDllName = null;
                    string fullDllName = null;

                    // Read the two strings.
                    try
                    {
                        baseDllName = currentEntry.BaseDllName.Read(this).TrimEnd('\0');
                    }
                    catch
                    { }

                    try
                    {
                        fullDllName = FileUtils.GetFileName(currentEntry.FullDllName.Read(this).TrimEnd('\0'));
                    }
                    catch
                    { }

                    // Execute the callback.
                    if (!enumModulesCallback(new ProcessModule(
                        currentEntry.DllBase,
                        currentEntry.SizeOfImage,
                        currentEntry.EntryPoint,
                        currentEntry.Flags,
                        baseDllName,
                        fullDllName
                        )))
                        break;
                }

                currentLink = currentEntry.InLoadOrderLinks.Flink;
                i++;
            }
        }

        /// <summary>
        /// Flushes the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The base address of the region to flush.</param>
        /// <param name="size">The size of the region to flush.</param>
        /// <returns>A NT status value.</returns>
        public NtStatus FlushMemory(IntPtr baseAddress, int size)
        {
            NtStatus status;
            IntPtr sizeIntPtr = size.ToIntPtr();
            IoStatusBlock isb;

            if ((status = Win32.NtFlushVirtualMemory(
                this,
                ref baseAddress,
                ref sizeIntPtr,
                out isb
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return isb.Status;
        }

        /// <summary>
        /// Frees a memory region in the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The address of the region to free.</param>
        /// <param name="size">The size to free.</param>
        public void FreeMemory(IntPtr baseAddress, int size)
        {
            this.FreeMemory(baseAddress, size, false);
        }

        /// <summary>
        /// Frees a memory region in the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The address of the region to free.</param>
        /// <param name="size">The size to free.</param>
        /// <param name="reserveOnly">Specifies whether or not to only 
        /// reserve the memory instead of freeing it.</param>
        public void FreeMemory(IntPtr baseAddress, int size, bool reserveOnly)
        {
            NtStatus status;
            IntPtr sizeIntPtr = size.ToIntPtr();

            // Size needs to be 0 if we're freeing.
            if (!reserveOnly)
                sizeIntPtr = IntPtr.Zero;

            if ((status = Win32.NtFreeVirtualMemory(
                this,
                ref baseAddress,
                ref sizeIntPtr,
                reserveOnly ? MemoryFlags.Decommit : MemoryFlags.Release
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Gets the processor affinity for the process.
        /// </summary>
        /// <returns>The processor affinity for the process.</returns>
        public long GetAffinityMask()
        {
            long systemMask;

            return this.GetAffinityMask(out systemMask);
        }

        /// <summary>
        /// Gets the processor affinity for the process.
        /// </summary>
        /// <param name="systemMask">Receives the processor affinity mask for the system.</param>
        /// <returns>The processor affinity for the process.</returns>
        public long GetAffinityMask(out long systemMask)
        {
            IntPtr processMaskTemp;
            IntPtr systemMaskTemp;

            if (!Win32.GetProcessAffinityMask(this, out processMaskTemp, out systemMaskTemp))
                Win32.Throw();

            systemMask = systemMaskTemp.ToInt64();

            return processMaskTemp.ToInt64();
        }

        /// <summary>
        /// Gets the base priority of the process.
        /// </summary>
        public int GetBasePriority()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessBasePriority);
        }

        /// <summary>
        /// Gets the process' basic information. This requires QueryLimitedInformation 
        /// access.
        /// </summary>
        /// <returns>A PROCESS_BASIC_INFORMATION structure.</returns>
        public ProcessBasicInformation GetBasicInformation()
        {
            NtStatus status;
            ProcessBasicInformation pbi;
            int retLen;

            if ((status = Win32.NtQueryInformationProcess(this, ProcessInformationClass.ProcessBasicInformation,
                out pbi, Marshal.SizeOf(typeof(ProcessBasicInformation)), out retLen)) >= NtStatus.Error)
                Win32.Throw(status);

            return pbi;
        }

        /// <summary>
        /// Gets the command line used to start the process. This requires 
        /// the PROCESS_QUERY_LIMITED_INFORMATION and PROCESS_VM_READ permissions.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetCommandLine()
        {
            if (!this.IsPosix())
                return this.GetPebString(PebOffset.CommandLine);
            else
                return this.GetPosixCommandLine();
        }

        /// <summary>
        /// Gets the process' cookie (a random value).
        /// </summary>
        public int GetCookie()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessCookie);
        }

        /// <summary>
        /// Gets the creation time of the process.
        /// </summary>
        public DateTime GetCreateTime()
        {
            return DateTime.FromFileTime(this.GetTimes()[0]);
        }

        /// <summary>
        /// Gets the number of processor cycles consumed by the process' threads.
        /// </summary>
        public ulong GetCycleTime()
        {
            ulong cycles;

            if (!Win32.QueryProcessCycleTime(this, out cycles))
                Win32.Throw();

            return cycles;
        }

        /// <summary>
        /// Opens the debug object associated with the process.
        /// </summary>
        /// <returns>A debug object handle.</returns>
        public DebugObjectHandle GetDebugObject()
        {
            IntPtr handle;

            handle = this.GetDebugObjectHandle();

            // Check if we got a handle. If we didn't the process is not being debugged.
            if (handle == IntPtr.Zero)
                return null;

            return new DebugObjectHandle(handle, true);
        }

        internal IntPtr GetDebugObjectHandle()
        {
            return this.GetInformationIntPtr(ProcessInformationClass.ProcessDebugObjectHandle);
        }

        /// <summary>
        /// Gets the process' DEP policy.
        /// </summary>
        /// <returns>A DepStatus enum.</returns>
        public DepStatus GetDepStatus()
        {
            MemExecuteOptions options;

            // If we're on 64-bit and the process isn't under 
            // WOW64, it must be under permanent DEP.
            if (OSVersion.Architecture == OSArch.Amd64)
            {
                if (!this.IsWow64())
                    return DepStatus.Enabled | DepStatus.Permanent;
            }

            options = (MemExecuteOptions)this.GetInformationInt32(ProcessInformationClass.ProcessExecuteFlags);

            DepStatus depStatus = 0;

            // Check if execution of data pages is enabled.
            if ((options & MemExecuteOptions.ExecuteEnable) == MemExecuteOptions.ExecuteEnable)
                return 0;

            // Check if execution of data pages is disabled.
            if ((options & MemExecuteOptions.ExecuteDisable) == MemExecuteOptions.ExecuteDisable)
                depStatus = DepStatus.Enabled;
            // ExecuteDisable and ExecuteEnable are both disabled in OptOut mode.
            else if ((options & MemExecuteOptions.ExecuteDisable) == 0 &&
                (options & MemExecuteOptions.ExecuteEnable) == 0)
                depStatus = DepStatus.Enabled;

            if ((options & MemExecuteOptions.DisableThunkEmulation) == MemExecuteOptions.DisableThunkEmulation)
                depStatus |= DepStatus.AtlThunkEmulationDisabled;
            if ((options & MemExecuteOptions.Permanent) == MemExecuteOptions.Permanent)
                depStatus |= DepStatus.Permanent;

            return depStatus;
        }

        /// <summary>
        /// Gets the process' environment variables. This requires the 
        /// PROCESS_QUERY_INFORMATION and PROCESS_VM_READ permissions.
        /// </summary>
        /// <returns>A dictionary of variables.</returns>
        public unsafe IDictionary<string, string> GetEnvironmentVariables()
        {
            IntPtr pebBaseAddress = this.GetBasicInformation().PebBaseAddress;
            byte* buffer = stackalloc byte[IntPtr.Size];

            // Get a pointer to the process parameters block.
            this.ReadMemory(pebBaseAddress.Increment(Peb.ProcessParametersOffset), buffer, IntPtr.Size);
            IntPtr processParameters = *(IntPtr*)buffer;

            // Get a pointer to the environment block.
            this.ReadMemory(processParameters.Increment(RtlUserProcessParameters.EnvironmentOffset), buffer, IntPtr.Size);
            IntPtr envBase = *(IntPtr*)buffer;
            int length = 0;

            {
                MemoryBasicInformation mbi = this.QueryMemory(envBase);

                if (mbi.Protect == MemoryProtection.NoAccess)
                    throw new WindowsException();

                length = mbi.RegionSize.Decrement(envBase.Decrement(mbi.BaseAddress)).ToInt32();
            }

            // Now we read in the entire region of memory
            // And yes, some memory is wasted.
            using (var memoryAlloc = new MemoryAlloc(length))
            {
                byte* memory = (byte*)memoryAlloc.Memory;

                this.ReadMemory(envBase, memoryAlloc.Memory, length);

                /* The environment variables block is a series of Unicode strings separated by 
                 * two null bytes. The entire block is terminated by four null bytes.
                 */
                Dictionary<string, string> vars = new Dictionary<string, string>();
                StringBuilder currentVariable = new StringBuilder();
                int i = 0;

                while (true)
                {
                    if (i >= length)
                        break;

                    char currentChar;

                    Encoding.Unicode.GetChars(&memory[i], 2, &currentChar, 1);

                    i += 2;

                    if (currentChar == '\0')
                    {
                        // Two nulls in a row, the env. block is finished.
                        if (currentVariable.Length == 0)
                            break;

                        string[] s = currentVariable.ToString().Split(new char[] { '=' }, 2);

                        if (!vars.ContainsKey(s[0]) && s.Length > 1)
                            vars.Add(s[0], s[1]);

                        currentVariable = new StringBuilder();
                    }
                    else
                    {
                        currentVariable.Append(currentChar);
                    }
                }

                return vars;
            }
        }

        /// <summary>
        /// Gets the process' exit code.
        /// </summary>
        /// <returns>A number.</returns>
        public int GetExitCode()
        {
            int exitCode;

            if (!Win32.GetExitCodeProcess(this, out exitCode))
                Win32.Throw();

            return exitCode;
        }

        /// <summary>
        /// Gets the process' exit status.
        /// </summary>
        /// <returns>A NT status value.</returns>
        public NtStatus GetExitStatus()
        {
            return this.GetBasicInformation().ExitStatus;
        }

        /// <summary>
        /// Gets the exit time of the process.
        /// </summary>
        public DateTime GetExitTime()
        {
            return DateTime.FromFileTime(this.GetTimes()[1]);
        }

        /// <summary>
        /// Gets a GUI handle count.
        /// </summary>
        /// <param name="userObjects">If true, returns the number of USER handles. Otherwise, returns 
        /// the number of GDI handles.</param>
        /// <returns>A handle count.</returns>
        public int GetGuiResources(bool userObjects)
        {
            return Win32.GetGuiResources(this, userObjects ? 1 : 0);
        }

        /// <summary>
        /// Gets the number of handles opened by the process.
        /// </summary>
        public int GetHandleCount()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessHandleCount);
        }

        /// <summary>
        /// Gets the handles owned by the process.
        /// </summary>
        /// <returns>An array of handle information structures.</returns>
        public ProcessHandleInformation[] GetHandles()
        {
            int returnLength = 0;
            int attempts = 0;

            using (var data = new MemoryAlloc(0x1000))
            {
                while (true)
                {
                    try
                    {
                        KProcessHacker.Instance.KphQueryProcessHandles(this, data, data.Size, out returnLength);
                    }
                    catch (WindowsException ex)
                    {
                        if (attempts > 3)
                            throw ex;

                        if (
                            ex.Status == NtStatus.BufferTooSmall &&
                            returnLength > data.Size
                            )
                            data.ResizeNew(returnLength);

                        attempts++;

                        continue;
                    }

                    int handleCount = data.ReadInt32(0);
                    ProcessHandleInformation[] handles = new ProcessHandleInformation[handleCount];

                    for (int i = 0; i < handleCount; i++)
                        handles[i] = data.ReadStruct<ProcessHandleInformation>(sizeof(int), i);

                    return handles;
                }
            }
        }

        /// <summary>
        /// Gets a collection of handle stack traces. This requires 
        /// PROCESS_QUERY_INFORMATION access.
        /// </summary>
        /// <returns>A collection of handle stack traces.</returns>
        public ProcessHandleTraceCollection GetHandleTraces()
        {
            return this.GetHandleTraces(IntPtr.Zero);
        }

        /// <summary>
        /// Gets a collection of handle stack traces. This requires 
        /// PROCESS_QUERY_INFORMATION access.
        /// </summary>
        /// <param name="handle">
        /// A handle to the stack trace to retrieve. If this parameter is 
        /// zero, all stack traces will be retrieved.
        /// </param>
        /// <returns>A collection of handle stack traces.</returns>
        public ProcessHandleTraceCollection GetHandleTraces(IntPtr handle)
        {
            NtStatus status = NtStatus.Success;
            int retLength;

            using (var data = new MemoryAlloc(0x10000))
            {
                var query = new ProcessHandleTracingQuery();

                // If Handle is not NULL, NtQueryInformationProcess will 
                // get a specific stack trace. Otherwise, it will get 
                // all of the stack traces.
                query.Handle = handle;
                data.WriteStruct<ProcessHandleTracingQuery>(query);

                for (int i = 0; i < 8; i++)
                {
                    status = Win32.NtQueryInformationProcess(
                        this,
                        ProcessInformationClass.ProcessHandleTracing,
                        data,
                        data.Size,
                        out retLength
                        );

                    if (status == NtStatus.InfoLengthMismatch)
                    {
                        data.ResizeNew(data.Size * 4);
                        continue;
                    }

                    if (status >= NtStatus.Error)
                        Win32.Throw(status);

                    return new ProcessHandleTraceCollection(data);
                }

                Win32.Throw(status);
                return null; // Silences the compiler.
            }
        }

        /// <summary>
        /// Gets the process' default heap.
        /// </summary>
        /// <returns>A pointer to a heap.</returns>
        public unsafe IntPtr GetHeap()
        {
            IntPtr heap;

            this.ReadMemory(
                this.GetBasicInformation().PebBaseAddress.Increment(Peb.ProcessHeapOffset),
                &heap,
                IntPtr.Size
                );

            return heap;
        }

        /// <summary>
        /// Gets the file name of the process' image. This requires 
        /// QueryLimitedInformation access.
        /// </summary>
        /// <returns>A file name, in native format.</returns>
        public string GetImageFileName()
        {
            return this.GetInformationUnicodeString(ProcessInformationClass.ProcessImageFileName);
        }

        /// <summary>
        /// Gets the file name of the process' image. This requires 
        /// QueryLimitedInformation access.
        /// </summary>
        /// <returns>A file name, in DOS format.</returns>
        public string GetImageFileNameWin32()
        {
            return this.GetInformationUnicodeString(ProcessInformationClass.ProcessImageFileNameWin32);
        }

        /// <summary>
        /// Gets information about the process in an Int32.
        /// </summary>
        /// <param name="infoClass">The class of information to retrieve.</param>
        /// <returns>An int.</returns>
        private int GetInformationInt32(ProcessInformationClass infoClass)
        {
            if (
                KProcessHacker.Instance != null &&
                infoClass == ProcessInformationClass.ProcessIoPriority
                )
            {
                unsafe
                {
                    int value;
                    int retLength;

                    KProcessHacker.Instance.KphQueryInformationProcess(
                        this, infoClass, new IntPtr(&value), sizeof(int), out retLength
                        );

                    return value;
                }
            }
            else
            {
                NtStatus status;
                int value;
                int retLength;

                if ((status = Win32.NtQueryInformationProcess(
                    this, infoClass, out value, sizeof(int), out retLength)) >= NtStatus.Error)
                    Win32.Throw(status);

                return value;
            }
        }

        /// <summary>
        /// Gets information about the process in an IntPtr.
        /// </summary>
        /// <param name="infoClass">The class of information to retrieve.</param>
        /// <returns>An IntPtr.</returns>
        private IntPtr GetInformationIntPtr(ProcessInformationClass infoClass)
        {
            NtStatus status;
            IntPtr value;
            int retLength;

            if ((status = Win32.NtQueryInformationProcess(
                this, infoClass, out value, IntPtr.Size, out retLength)) >= NtStatus.Error)
                Win32.Throw(status);

            return value;
        }

        private string GetInformationUnicodeString(ProcessInformationClass infoClass)
        {
            NtStatus status;
            int retLen;

            Win32.NtQueryInformationProcess(this, infoClass, IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if ((status = Win32.NtQueryInformationProcess(this, infoClass, data, retLen, out retLen)) >= NtStatus.Error)
                    Win32.Throw(status);

                return data.ReadStruct<UnicodeString>().Read();
            }
        }

        /// <summary>
        /// Gets the process' I/O priority, ranging from 0-7.
        /// </summary>
        /// <returns></returns>
        public int GetIoPriority()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessIoPriority);
        }

        /// <summary>
        /// Gets I/O statistics for the process.
        /// </summary>
        /// <returns>A IoCounters structure.</returns>
        public IoCounters GetIoStatistics()
        {
            NtStatus status;
            IoCounters counters;
            int retLength;

            if ((status = Win32.NtQueryInformationProcess(
                this,
                ProcessInformationClass.ProcessIoCounters,
                out counters,
                Marshal.SizeOf(typeof(IoCounters)),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return counters;
        }

        /// <summary>
        /// Opens the job object associated with the process.
        /// </summary>
        /// <returns>A job object handle.</returns>
        public JobObjectHandle GetJobObject(JobObjectAccess access)
        {
            IntPtr handle;

            handle = JobObjectHandle.Open(this, access);

            if (handle != IntPtr.Zero)
                return new JobObjectHandle(handle, true);
            else
                return null;
        }

        /// <summary>
        /// Gets the type of well-known process.
        /// </summary>
        /// <returns>A known process type.</returns>
        public KnownProcess GetKnownProcessType()
        {
            if (this.GetBasicInformation().UniqueProcessId.Equals(4))
                return KnownProcess.System;

            string fileName = FileUtils.GetFileName(this.GetImageFileName());

            if (fileName.StartsWith(Environment.SystemDirectory, StringComparison.OrdinalIgnoreCase))
            {
                string baseName = fileName.Remove(0, Environment.SystemDirectory.Length).TrimStart('\\').ToLowerInvariant();

                switch (baseName)
                {
                    case "smss.exe":
                        return KnownProcess.SessionManager;
                    case "csrss.exe":
                        return KnownProcess.WindowsSubsystem;
                    case "wininit.exe":
                        return KnownProcess.WindowsStartup;
                    case "services.exe":
                        return KnownProcess.ServiceControlManager;
                    case "lsass.exe":
                        return KnownProcess.LocalSecurityAuthority;
                    case "lsm.exe":
                        return KnownProcess.LocalSessionManager;
                    default:
                        return KnownProcess.None;
                }
            }
            else
            {
                return KnownProcess.None;
            }
        }

        /// <summary>
        /// Gets the main module of the process. This requires the 
        /// PROCESS_QUERY_INFORMATION and PROCESS_VM_READ permissions.
        /// </summary>
        /// <returns>A ProcessModule.</returns>
        public ProcessModule GetMainModule()
        {
            ProcessModule mainModule = null;

            this.EnumModules((module) =>
            {
                mainModule = module;
                return false;
            });

            return mainModule;
        }

        /// <summary>
        /// Gets the name of a file which the process has mapped.
        /// </summary>
        /// <param name="address">The address of the mapped section.</param>
        /// <returns>A filename.</returns>
        public string GetMappedFileName(IntPtr address)
        {
            NtStatus status;
            IntPtr retLength;

            using (var data = new MemoryAlloc(20))
            {
                if ((status = Win32.NtQueryVirtualMemory(
                    this,
                    address,
                    MemoryInformationClass.MemoryMappedFilenameInformation,
                    data,
                    data.Size.ToIntPtr(),
                    out retLength
                    )) == NtStatus.BufferOverflow)
                {
                    data.ResizeNew(retLength.ToInt32());

                    status = Win32.NtQueryVirtualMemory(
                        this,
                        address,
                        MemoryInformationClass.MemoryMappedFilenameInformation,
                        data,
                        data.Size.ToIntPtr(),
                        out retLength
                        );
                }

                if (status >= NtStatus.Error)
                    return null;

                return FileUtils.GetFileName(data.ReadStruct<UnicodeString>().Read());
            }
        }

        /// <summary>
        /// Gets memory statistics for the process.
        /// </summary>
        /// <returns>A VmCounters structure.</returns>
        public VmCounters GetMemoryStatistics()
        {
            NtStatus status;
            VmCounters counters;
            int retLength;

            if ((status = Win32.NtQueryInformationProcess(
                this,
                ProcessInformationClass.ProcessVmCounters,
                out counters,
                Marshal.SizeOf(typeof(VmCounters)),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return counters;
        }

        /// <summary>
        /// Gets the modules loaded by the process. This requires the 
        /// PROCESS_QUERY_INFORMATION and PROCESS_VM_READ permissions.
        /// </summary>
        /// <returns>An array of ProcessModule objects.</returns>
        public ProcessModule[] GetModules()
        {
            List<ProcessModule> modules = new List<ProcessModule>();

            this.EnumModules((module) =>
            {
                modules.Add(module);
                return true;
            });

            return modules.ToArray();
        }

        /// <summary>
        /// Opens the next linked process.
        /// </summary>
        /// <param name="access">The desired access to the next process.</param>
        /// <returns>A process handle.</returns>
        public ProcessHandle GetNextProcess(ProcessAccess access)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.NtGetNextProcess(
                this,
                access,
                0,
                0,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            if (handle != IntPtr.Zero)
                return new ProcessHandle(handle, true);
            else
                return null;
        }

        /// <summary>
        /// Opens the next linked thread belonging to the process.
        /// </summary>
        /// <param name="threadHandle">A thread handle. You may specify null.</param>
        /// <param name="access">The desired access to the next thread.</param>
        /// <returns>A thread handle.</returns>
        public ThreadHandle GetNextThread(ThreadHandle threadHandle, ThreadAccess access)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.NtGetNextThread(
                this,
                threadHandle != null ? threadHandle : IntPtr.Zero,
                access,
                0,
                0,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            if (handle != IntPtr.Zero)
                return new ThreadHandle(handle, true);
            else
                return null;
        }

        /// <summary>
        /// Gets the process' page priority, ranging from 0-7.
        /// </summary>
        public int GetPagePriority()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessPagePriority);
        }

        /// <summary>
        /// Gets the process' parent's process ID. This requires 
        /// the PROCESS_QUERY_LIMITED_INFORMATION permission.
        /// </summary>
        /// <returns>The process ID.</returns>
        public int GetParentPid()
        {
            return this.GetBasicInformation().InheritedFromUniqueProcessId.ToInt32();
        }

        /// <summary>
        /// Reads a UNICODE_STRING from the process' process environment block.
        /// </summary>
        /// <param name="offset">The offset to the UNICODE_STRING structure.</param>
        /// <returns>A string.</returns>
        public unsafe string GetPebString(PebOffset offset)
        {
            byte* buffer = stackalloc byte[IntPtr.Size];
            IntPtr pebBaseAddress = this.GetBasicInformation().PebBaseAddress;

            // Read the address of parameter information block.
            this.ReadMemory(pebBaseAddress.Increment(Peb.ProcessParametersOffset), buffer, IntPtr.Size);
            IntPtr processParameters = *(IntPtr*)buffer;

            // The offset of the UNICODE_STRING structure is specified in the enum.
            int realOffset = GetPebOffset(offset);

            // Read the UNICODE_STRING structure.
            UnicodeString pebStr;

            this.ReadMemory(processParameters.Increment(realOffset), &pebStr, Marshal.SizeOf(typeof(UnicodeString)));

            // read string and decode it
            return Encoding.Unicode.GetString(this.ReadMemory(pebStr.Buffer, pebStr.Length), 0, pebStr.Length);
        }

        /// <summary>
        /// Gets the command line used to start the process. This 
        /// function is only valid for POSIX processes.
        /// </summary>
        /// <returns>A command line string.</returns>
        public unsafe string GetPosixCommandLine()
        {
            byte* buffer = stackalloc byte[IntPtr.Size];
            IntPtr pebBaseAddress = this.GetBasicInformation().PebBaseAddress;

            this.ReadMemory(pebBaseAddress.Increment(Peb.ProcessParametersOffset), buffer, IntPtr.Size);
            IntPtr processParameters = *(IntPtr*)buffer;

            // Read the command line UNICODE_STRING structure.
            UnicodeString commandLineUs;

            this.ReadMemory(
                processParameters.Increment(GetPebOffset(PebOffset.CommandLine)),
                &commandLineUs,
                Marshal.SizeOf(typeof(UnicodeString))
                );
            IntPtr stringAddr = commandLineUs.Buffer;

            /*
             * In the POSIX subsystem the command line is actually split up into bits, as in 
             * argv. In the command line string we don't actually have the command line - 
             * instead, it is filled with pointers to each command line part. For example:
             * CommandLine.Buffer = 0x12345678
             * at 0x12345678 we have:
             * 0x12346000 0x12347000 0x12348000 0x00000000 0x12349000
             * ^ at 0x12346000: "cat" (ASCII)
             *            ^ at 0x12347000: "-o" (ASCII)
             *                       ^ at 0x12348000: "myfile" (ASCII)
             *                                  ^ signifies that there are no more pointers
             *                                             ^ pointer to environment block
             *                                               - from this we can work out 
             *                                               how much memory to read
             */
            // Get the list of pointers.
            List<IntPtr> strPointers = new List<IntPtr>();
            bool zeroReached = false;
            int i = 0;

            while (true)
            {
                this.ReadMemory(stringAddr.Increment(i), buffer, IntPtr.Size);
                IntPtr value = *(IntPtr*)buffer;

                if (value != IntPtr.Zero)
                    strPointers.Add(value);

                i += IntPtr.Size;

                if (zeroReached)
                    break;
                else if (value == IntPtr.Zero)
                    zeroReached = true;
            }

            // Work out the size of the command line and read the data.
            IntPtr lastPointer = strPointers[strPointers.Count - 1];
            int partsSize = lastPointer.Decrement(strPointers[0]).ToInt32();

            // FIXME: Lazy; optimize later.
            StringBuilder commandLine = new StringBuilder();

            for (i = 0; i < strPointers.Count - 1; i++)
            {
                byte[] data = this.ReadMemory(strPointers[i], partsSize);

                commandLine.Append(Encoding.ASCII.GetString(data, 0, Array.IndexOf<byte>(data, 0)) + " ");
            }

            string commandLineStr = commandLine.ToString();

            if (commandLineStr.EndsWith(" "))
                commandLineStr = commandLineStr.Remove(commandLineStr.Length - 1, 1);

            return commandLineStr;
        }

        /// <summary>
        /// Gets the process' priority class.
        /// </summary>
        /// <returns>A ProcessPriorityClass enum.</returns>
        public ProcessPriorityClass GetPriorityClass()
        {
            //switch (Win32.GetPriorityClass(this))
            //{
            //    case ProcessPriorityClassWin32.AboveNormal:
            //        return ProcessPriorityClass.AboveNormal;
            //    case ProcessPriorityClassWin32.BelowNormal:
            //        return ProcessPriorityClass.BelowNormal;
            //    case ProcessPriorityClassWin32.High:
            //        return ProcessPriorityClass.High;
            //    case ProcessPriorityClassWin32.Idle:
            //        return ProcessPriorityClass.Idle;
            //    case ProcessPriorityClassWin32.Normal:
            //        return ProcessPriorityClass.Normal;
            //    case ProcessPriorityClassWin32.RealTime:
            //        return ProcessPriorityClass.RealTime;
            //    default:
            //        Win32.Throw();
            //        // Stupid compiler
            //        return ProcessPriorityClass.Unknown;
            //}

            NtStatus status;
            ProcessPriorityClassStruct priorityClass;
            int retLength;

            if ((status = Win32.NtQueryInformationProcess(
                this,
                ProcessInformationClass.ProcessPriorityClass,
                out priorityClass,
                Marshal.SizeOf(typeof(ProcessPriorityClassStruct)),
                out retLength
                )) != NtStatus.Success)
                Win32.Throw(status);

            return (ProcessPriorityClass)priorityClass.PriorityClass;
        }

        /// <summary>
        /// Gets the process' unique identifier.
        /// </summary>
        public int GetProcessId()
        {
            return this.GetBasicInformation().UniqueProcessId.ToInt32();
        }

        /// <summary>
        /// Gets the process' session ID.
        /// </summary>
        public int GetSessionId()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessSessionInformation);
        }

        /// <summary>
        /// Gets an array of times for the process.
        /// </summary>
        /// <returns>An array of times: creation time, exit time, kernel time, user time.</returns>
        private LargeInteger[] GetTimes()
        {
            LargeInteger[] times = new LargeInteger[4];

            if (!Win32.GetProcessTimes(this, out times[0], out times[1], out times[2], out times[3]))
                Win32.Throw();

            return times;
        }

        /// <summary>
        /// Opens and returns a handle to the process' token. This requires 
        /// PROCESS_QUERY_LIMITED_INFORMATION access.
        /// </summary>
        /// <returns>A handle to the process' token.</returns>
        public TokenHandle GetToken()
        {
            return this.GetToken(TokenAccess.All);
        }

        /// <summary>
        /// Opens and returns a handle to the process' token. This requires 
        /// PROCESS_QUERY_LIMITED_INFORMATION access.
        /// </summary>
        /// <param name="access">The desired access to the token.</param>
        /// <returns>A handle to the process' token.</returns>
        public TokenHandle GetToken(TokenAccess access)
        {
            return new TokenHandle(this, access);
        }

        /// <summary>
        /// Forces the process to load the specified library.
        /// </summary>
        /// <param name="path">The path to the library.</param>
        public void InjectDll(string path)
        {
            this.InjectDll(path, 0xffffffff);
        }

        /// <summary>
        /// Forces the process to load the specified library.
        /// </summary>
        /// <param name="path">The path to the library.</param>
        /// <param name="timeout">The timeout, in milliseconds, for the process to load the library.</param>
        public void InjectDll(string path, uint timeout)
        {
            IntPtr stringPage = this.AllocateMemory(path.Length * 2 + 2, MemoryProtection.ReadWrite);

            this.WriteMemory(stringPage, Encoding.Unicode.GetBytes(path));

            // Vista seems to support non-Win32 threads better than XP can.
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                using (var thandle = this.CreateThread(
                    Loader.GetProcedure("kernel32.dll", "LoadLibraryW"),
                    stringPage
                    ))
                    thandle.Wait(timeout * Win32.TimeMsTo100Ns);
            }
            else
            {
                using (var thandle = this.CreateThreadWin32(
                    Loader.GetProcedure("kernel32.dll", "LoadLibraryW"),
                    stringPage
                    ))
                    thandle.Wait(timeout * Win32.TimeMsTo100Ns);
            }

            this.FreeMemory(stringPage, path.Length * 2 + 2, false);
        }

        /// <summary>
        /// Gets whether the process is currently being debugged. This requires 
        /// QueryInformation access.
        /// </summary>
        public bool IsBeingDebugged()
        {
            return this.GetInformationIntPtr(ProcessInformationClass.ProcessDebugPort) != IntPtr.Zero;
        }

        /// <summary>
        /// Gets whether the system will crash upon the process being terminated.
        /// </summary>
        public bool IsCritical()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessBreakOnTermination) != 0;
        }

        /// <summary>
        /// Determines whether the process is running in a job.
        /// </summary>
        /// <returns>A boolean.</returns>
        public bool IsInJob()
        {
            bool result;

            if (!Win32.IsProcessInJob(this, IntPtr.Zero, out result))
                Win32.Throw();

            return result;
        }

        /// <summary>
        /// Determines whether the process is running in the specified job.
        /// </summary>
        /// <param name="jobObjectHandle">The job object to check.</param>
        /// <returns>A boolean.</returns>
        public bool IsInJob(JobObjectHandle jobObjectHandle)
        {
            bool result;

            if (!Win32.IsProcessInJob(this, jobObjectHandle, out result))
                Win32.Throw();

            return result;
        }

        /// <summary>
        /// Gets whether the process is a NTVDM process.
        /// </summary>
        public bool IsNtVdmProcess()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessWx86Information) != 0;
        }

        /// <summary>
        /// Gets whether the process is using the POSIX subsystem.
        /// </summary>
        public unsafe bool IsPosix()
        {
            int subsystem;
            IntPtr pebBaseAddress = this.GetBasicInformation().PebBaseAddress;

            this.ReadMemory(pebBaseAddress.Increment(Peb.ImageSubsystemOffset), &subsystem, sizeof(int));

            return subsystem == 7;
        }

        /// <summary>
        /// Gets whether the process has priority boost enabled.
        /// </summary>
        public bool IsPriorityBoostEnabled()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessPriorityBoost) == 0;
        }

        /// <summary>
        /// Gets whether the process is running under WOW64.
        /// </summary>
        public bool IsWow64()
        {
            return this.GetInformationIntPtr(ProcessInformationClass.ProcessWow64Information) != IntPtr.Zero;
        }

        /// <summary>
        /// Sets the protection for a page in the process.
        /// </summary>
        /// <param name="baseAddress">The address to modify.</param>
        /// <param name="size">The number of bytes to modify.</param>
        /// <param name="protection">The new memory protection.</param>
        /// <returns>The old memory protection.</returns>
        public MemoryProtection ProtectMemory(IntPtr baseAddress, int size, MemoryProtection protection)
        {
            NtStatus status;
            IntPtr sizeIntPtr = size.ToIntPtr();
            MemoryProtection oldProtection;

            if ((status = Win32.NtProtectVirtualMemory(
                this,
                ref baseAddress,
                ref sizeIntPtr,
                protection,
                out oldProtection
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return oldProtection;
        }

        /// <summary>
        /// Gets information about the memory region starting at the specified address.
        /// </summary>
        /// <param name="baseAddress">The address to query.</param>
        /// <returns>A MEMORY_BASIC_INFORMATION structure.</returns>
        public MemoryBasicInformation QueryMemory(IntPtr baseAddress)
        {
            NtStatus status;
            MemoryBasicInformation mbi;
            IntPtr retLength;

            if ((status = Win32.NtQueryVirtualMemory(
                this,
                baseAddress,
                MemoryInformationClass.MemoryBasicInformation,
                out mbi,
                Marshal.SizeOf(typeof(MemoryBasicInformation)).ToIntPtr(),
                out retLength
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return mbi;
        }

        /// <summary>
        /// Reads data from the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin reading.</param>
        /// <param name="length">The length, in bytes, to read.</param>
        /// <returns>An array of bytes.</returns>
        public byte[] ReadMemory(IntPtr baseAddress, int length)
        {
            byte[] buffer = new byte[length];

            this.ReadMemory(baseAddress, buffer, length);

            return buffer;
        }

        /// <summary>
        /// Reads data from the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin reading.</param>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="length">The length to read.</param>
        /// <returns>The number of bytes read.</returns>
        public unsafe int ReadMemory(IntPtr baseAddress, byte[] buffer, int length)
        {
            fixed (byte* bufferPtr = buffer)
                return this.ReadMemory(baseAddress, bufferPtr, length);
        }

        /// <summary>
        /// Reads data from the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin reading.</param>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="length">The length to read.</param>
        /// <returns>The number of bytes read.</returns>
        public unsafe int ReadMemory(IntPtr baseAddress, void* buffer, int length)
        {
            return this.ReadMemory(baseAddress, new IntPtr(buffer), length);
        }

        /// <summary>
        /// Reads data from the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin reading.</param>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="length">The length to read.</param>
        /// <returns>The number of bytes read.</returns>
        public int ReadMemory(IntPtr baseAddress, IntPtr buffer, int length)
        {
            int retLength;

            if (this.Handle == Current)
            {
                Win32.RtlMoveMemory(buffer, baseAddress, length.ToIntPtr());
                return length;
            }

            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphReadVirtualMemory(this, baseAddress.ToInt32(), buffer, length, out retLength);
            }
            else
            {
                NtStatus status;
                IntPtr retLengthIntPtr;

                if ((status = Win32.NtReadVirtualMemory(
                    this,
                    baseAddress,
                    buffer,
                    length.ToIntPtr(),
                    out retLengthIntPtr
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                retLength = retLengthIntPtr.ToInt32();
            }

            return retLength;
        }

        /// <summary>
        /// Calls the specified function in the context of the process.
        /// </summary>
        /// <param name="address">The function to call.</param>
        /// <param name="arguments">The arguments to pass to the function.</param>
        public ThreadHandle RemoteCall(IntPtr address, IntPtr[] arguments)
        {
            IntPtr rtlExitUserThread = Loader.GetProcedure("ntdll.dll", "RtlExitUserThread");

            // Create a suspended thread at RtlExitUserThread.
            var thandle = this.CreateThread(rtlExitUserThread, IntPtr.Zero, true);

            // Do the remote call on this thread.
            thandle.RemoteCall(this, address, arguments, true);
            // Resume the thread. It will execute the remote call then exit.
            thandle.Resume();

            return thandle;
        }

        /// <summary>
        /// Stops debugging the process attached to the specified debug object. This requires 
        /// PROCESS_SUSPEND_RESUME access.
        /// </summary>
        /// <param name="debugObjectHandle">The debug object which was used to debug the process.</param>
        public void RemoveDebug(DebugObjectHandle debugObjectHandle)
        {
            NtStatus status;

            if ((status = Win32.NtRemoveProcessDebug(this, debugObjectHandle)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Resumes the process. This requires PROCESS_SUSPEND_RESUME access.
        /// </summary>
        public void Resume()
        {
            if (KProcessHacker.Instance != null && OSVersion.HasPsSuspendResumeProcess)
            {
                KProcessHacker.Instance.KphResumeProcess(this);
            }
            else
            {
                NtStatus status;

                if ((status = Win32.NtResumeProcess(this)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        /// <summary>
        /// Sets the processor affinity for the process.
        /// </summary>
        /// <param name="processMask">The processor affinity mask.</param>
        public void SetAffinityMask(long processMask)
        {
            if (!Win32.SetProcessAffinityMask(this, new IntPtr(processMask)))
                Win32.Throw();
        }

        /// <summary>
        /// Sets the process' base priority.
        /// </summary>
        /// <param name="basePriority">The process' base priority.</param>
        public void SetBasePriority(int basePriority)
        {
            this.SetInformationInt32(ProcessInformationClass.ProcessBasePriority, basePriority);
        }

        /// <summary>
        /// Sets whether the system will crash upon the process being terminated. 
        /// This function requires SeTcbPrivilege.
        /// </summary>
        /// <param name="critical">Whether the system will crash upon the process being terminated.</param>
        public void SetCritical(bool critical)
        {
            this.SetInformationInt32(ProcessInformationClass.ProcessBreakOnTermination, critical ? 1 : 0);
        }

        /// <summary>
        /// Sets the process' DEP policy.
        /// </summary>
        /// <param name="depStatus">The DEP options.</param>
        public void SetDepStatus(DepStatus depStatus)
        {
            MemExecuteOptions executeOptions = 0;

            if ((depStatus & DepStatus.Enabled) == DepStatus.Enabled)
                executeOptions |= MemExecuteOptions.ExecuteDisable;
            else
                executeOptions |= MemExecuteOptions.ExecuteEnable;

            if ((depStatus & DepStatus.AtlThunkEmulationDisabled) == DepStatus.AtlThunkEmulationDisabled)
                executeOptions |= MemExecuteOptions.DisableThunkEmulation;
            if ((depStatus & DepStatus.Permanent) == DepStatus.Permanent)
                executeOptions |= MemExecuteOptions.Permanent;

            KProcessHacker.Instance.SetExecuteOptions(this, executeOptions);
        }

        /// <summary>
        /// Sets information about the process in an Int32.
        /// </summary>
        /// <param name="infoClass">The class of information to set.</param>
        /// <param name="value">The value to set.</param>
        private void SetInformationInt32(ProcessInformationClass infoClass, int value)
        {
            if (
                KProcessHacker.Instance != null &&
                infoClass == ProcessInformationClass.ProcessIoPriority
                )
            {
                unsafe
                {
                    KProcessHacker.Instance.KphSetInformationProcess(
                        this, infoClass, new IntPtr(&value), sizeof(int)
                        );
                }
            }
            else
            {
                NtStatus status;

                if ((status = Win32.NtSetInformationProcess(
                    this, infoClass, ref value, sizeof(int))) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        public void SetIoPriority(int ioPriority)
        {
            this.SetInformationInt32(ProcessInformationClass.ProcessIoPriority, ioPriority);
        }

        /// <summary>
        /// Sets the reference count of a module.
        /// </summary>
        /// <param name="baseAddress">The base address of the module.</param>
        /// <param name="count">The new reference count.</param>
        public unsafe void SetModuleReferenceCount(IntPtr baseAddress, ushort count)
        {
            byte* buffer = stackalloc byte[IntPtr.Size];

            this.ReadMemory(
                this.GetBasicInformation().PebBaseAddress.Increment(Peb.LdrOffset),
                buffer,
                IntPtr.Size
                );

            IntPtr loaderData = *(IntPtr*)buffer;

            PebLdrData data;
            this.ReadMemory(loaderData, &data, Marshal.SizeOf(typeof(PebLdrData)));

            if (!data.Initialized)
                throw new Exception("Loader data is not initialized.");

            List<ProcessModule> modules = new List<ProcessModule>();
            IntPtr currentLink = data.InLoadOrderModuleList.Flink;
            IntPtr startLink = currentLink;
            LdrDataTableEntry currentEntry;
            int i = 0;

            while (currentLink != IntPtr.Zero)
            {
                if (modules.Count > 0 && currentLink == startLink)
                    break;
                if (i > 0x800)
                    break;

                this.ReadMemory(currentLink, &currentEntry, Marshal.SizeOf(typeof(LdrDataTableEntry)));

                if (currentEntry.DllBase == baseAddress)
                {
                    this.WriteMemory(currentLink.Increment(LdrDataTableEntry.LoadCountOffset), &count, 2);
                    break;
                }

                currentLink = currentEntry.InLoadOrderLinks.Flink;
                i++;
            }
        }

        public void SetPagePriority(int pagePriority)
        {
            this.SetInformationInt32(ProcessInformationClass.ProcessPagePriority, pagePriority);
        }

        /// <summary>
        /// Sets the process' priority boost.
        /// </summary>
        /// <param name="enabled">Whether priority boost will be enabled.</param>
        public void SetPriorityBoost(bool enabled)
        {
            // If priority boost is being enabled, we have to not disable it (hence the value of 0).
            this.SetInformationInt32(ProcessInformationClass.ProcessPriorityBoost, enabled ? 0 : 1);
        }

        /// <summary>
        /// Sets the process' priority class.
        /// </summary>
        /// <param name="priorityClass">The process' priority class.</param>
        public void SetPriorityClass(ProcessPriorityClass priorityClass)
        {
            //ProcessPriorityClassWin32 pcWin32;

            //switch (priorityClass)
            //{
            //    case ProcessPriorityClass.AboveNormal:
            //        pcWin32 = ProcessPriorityClassWin32.AboveNormal;
            //        break;
            //    case ProcessPriorityClass.BelowNormal:
            //        pcWin32 = ProcessPriorityClassWin32.BelowNormal;
            //        break;
            //    case ProcessPriorityClass.High:
            //        pcWin32 = ProcessPriorityClassWin32.High;
            //        break;
            //    case ProcessPriorityClass.Idle:
            //        pcWin32 = ProcessPriorityClassWin32.Idle;
            //        break;
            //    case ProcessPriorityClass.Normal:
            //        pcWin32 = ProcessPriorityClassWin32.Normal;
            //        break;
            //    case ProcessPriorityClass.RealTime:
            //        pcWin32 = ProcessPriorityClassWin32.RealTime;
            //        break;
            //    default:
            //        throw new ArgumentException("priorityClass");
            //}

            //if (!Win32.SetPriorityClass(this, pcWin32))
            //    Win32.Throw();

            NtStatus status;
            ProcessPriorityClassStruct processPriority;

            processPriority.Foreground = '\0';
            processPriority.PriorityClass = Convert.ToChar(priorityClass);

            if ((status = Win32.NtSetInformationProcess(
                this,
                ProcessInformationClass.ProcessPriorityClass,
                ref processPriority,
                Marshal.SizeOf(typeof(ProcessPriorityClassStruct))
                )) != NtStatus.Success)
                Win32.Throw(status);
        }

        /// <summary>
        /// Suspends the process. This requires PROCESS_SUSPEND_RESUME access.
        /// </summary>
        public void Suspend()
        {
            if (KProcessHacker.Instance != null && OSVersion.HasPsSuspendResumeProcess)
            {
                KProcessHacker.Instance.KphSuspendProcess(this);
            }
            else
            {
                NtStatus status;

                if ((status = Win32.NtSuspendProcess(this)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        /// <summary>
        /// Terminates the process. This requires PROCESS_TERMINATE access.
        /// </summary>
        public void Terminate()
        {
            this.Terminate(NtStatus.Success);
        }

        /// <summary>
        /// Terminates the process. This requires PROCESS_TERMINATE access.
        /// </summary>
        /// <param name="exitStatus">The exit status.</param>
        public void Terminate(NtStatus exitStatus)
        {
            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphTerminateProcess(this, exitStatus);
            }
            else
            {
                NtStatus status;

                if ((status = Win32.NtTerminateProcess(this, exitStatus)) >= NtStatus.Error)
                    Win32.Throw(status);
            }
        }

        /// <summary>
        /// Writes a minidump of the process to the specified file.
        /// </summary>
        /// <param name="fileName">The destination file.</param>
        public void WriteDump(string fileName)
        {
            // taskmgr uses these flags
            this.WriteDump(fileName,
                MinidumpType.WithFullMemory |
                MinidumpType.WithHandleData |
                MinidumpType.WithUnloadedModules |
                MinidumpType.WithFullMemoryInfo |
                MinidumpType.WithThreadInfo
                );
        }

        /// <summary>
        /// Writes a minidump of the process to the specified file.
        /// </summary>
        /// <param name="fileName">The destination file.</param>
        /// <param name="type">The type of minidump to write.</param>
        public void WriteDump(string fileName, MinidumpType type)
        {
            using (var fhandle = FileHandle.CreateWin32(fileName, FileAccess.GenericWrite))
                this.WriteDump(fhandle, type);
        }

        /// <summary>
        /// Writes a minidump of the process to the specified file.
        /// </summary>
        /// <param name="fileHandle">A handle to the destination file.</param>
        /// <param name="type">The type of minidump to write.</param>
        public void WriteDump(FileHandle fileHandle, MinidumpType type)
        {
            if (!Win32.MiniDumpWriteDump(
                this,
                this.GetProcessId(),
                fileHandle,
                type,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                ))
                Win32.Throw();
        }

        /// <summary>
        /// Writes data to the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin writing.</param>
        /// <param name="buffer">The data to write.</param>
        /// <returns>The length, in bytes, that was written.</returns>
        public int WriteMemory(IntPtr baseAddress, byte[] buffer)
        {
            unsafe
            {
                fixed (byte* dataPtr = buffer)
                {
                    return WriteMemory(baseAddress, dataPtr, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Writes data to the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin writing.</param>
        /// <param name="buffer">The data to write.</param>
        /// <param name="length">The length to be written.</param>
        /// <returns>The length, in bytes, that was written.</returns>
        public unsafe int WriteMemory(IntPtr baseAddress, void* buffer, int length)
        {
            return this.WriteMemory(baseAddress, new IntPtr(buffer), length);
        }

        /// <summary>
        /// Writes data to the process' virtual memory.
        /// </summary>
        /// <param name="baseAddress">The offset at which to begin writing.</param>
        /// <param name="buffer">The data to write.</param>
        /// <param name="length">The length to be written.</param>
        /// <returns>The length, in bytes, that was written.</returns>
        public int WriteMemory(IntPtr baseAddress, IntPtr buffer, int length)
        {
            int retLength;

            if (this.Handle == Current)
            {
                Win32.RtlMoveMemory(baseAddress, buffer, length.ToIntPtr());
                return length;
            }

            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphWriteVirtualMemory(this, baseAddress.ToInt32(), buffer, length, out retLength);
            }
            else
            {
                NtStatus status;
                IntPtr retLengthIntPtr;

                if ((status = Win32.NtWriteVirtualMemory(
                    this,
                    baseAddress,
                    buffer,
                    length.ToIntPtr(),
                    out retLengthIntPtr
                    )) >= NtStatus.Error)
                    Win32.Throw(status);

                retLength = retLengthIntPtr.ToInt32();
            }

            return retLength;
        }
    }

    /// <summary>
    /// Represents a stack trace collected during a handle trace event.
    /// </summary>
    public class ProcessHandleTrace
    {
        private ClientId _clientId;
        private IntPtr _handle;
        private IntPtr[] _stack;
        private HandleTraceType _type;

        internal ProcessHandleTrace(ProcessHandleTracingEntry entry)
        {
            _clientId = entry.ClientId;
            _handle = entry.Handle;
            _type = entry.Type;

            // Find the first occurrence of a NULL to find where the trace stops.
            int zeroIndex = Array.IndexOf<IntPtr>(entry.Stacks, IntPtr.Zero);

            // If there was no NULL, copy the entire array.
            if (zeroIndex == -1)
                zeroIndex = entry.Stacks.Length;

            // Copy the actual stack trace, excluding NULLs.
            _stack = new IntPtr[zeroIndex];
            Array.Copy(entry.Stacks, 0, _stack, 0, zeroIndex);
        }

        /// <summary>
        /// The client ID of the thread which produced the event.
        /// </summary>
        public ClientId ClientId
        {
            get { return _clientId; }
        }

        /// <summary>
        /// The handle value associated with the event.
        /// </summary>
        public IntPtr Handle
        {
            get { return _handle; }
        }

        /// <summary>
        /// A stack trace of the thread at the time of the event.
        /// </summary>
        public IntPtr[] Stack
        {
            get { return _stack; }
        }

        /// <summary>
        /// The type of handle trace event.
        /// </summary>
        public HandleTraceType Type
        {
            get { return _type; }
        }
    }

    /// <summary>
    /// Represents a collection of handle trace events.
    /// </summary>
    public class ProcessHandleTraceCollection : ReadOnlyCollection<ProcessHandleTrace>
    {
        private IntPtr _handle;

        internal ProcessHandleTraceCollection(MemoryAlloc data)
            : base(new List<ProcessHandleTrace>())
        {
            if (data.Size < Marshal.SizeOf(typeof(ProcessHandleTracingQuery)))
                throw new ArgumentException("Data memory allocation is too small.");

            // Read the structure.
            var query = data.ReadStruct<ProcessHandleTracingQuery>();

            _handle = query.Handle;

            // Get the handle traces.
            IList<ProcessHandleTrace> traces = this.Items;

            for (int i = 0; i < query.TotalTraces; i++)
            {
                var entry = data.ReadStruct<ProcessHandleTracingEntry>(
                    ProcessHandleTracingQuery.HandleTraceOffset,
                    i
                    );

                traces.Add(new ProcessHandleTrace(entry));
            }
        }

        /// <summary>
        /// A unique handle representing the collection.
        /// </summary>
        public IntPtr Handle
        {
            get { return _handle; }
        }
    }

    /// <summary>
    /// Represents a module loaded by a process.
    /// </summary>
    public class ProcessModule : ILoadedModule
    {
        public ProcessModule(
            IntPtr baseAddress,
            int size,
            IntPtr entryPoint,
            LdrpDataTableEntryFlags flags,
            string baseName,
            string fileName
            )
        {
            this.BaseAddress = baseAddress;
            this.Size = size;
            this.EntryPoint = entryPoint;
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
        /// The entry point of the module (usually its DllMain function).
        /// </summary>
        public IntPtr EntryPoint { get; private set; }
        /// <summary>
        /// The flags set by the NT loader for this module.
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

    /// <summary>
    /// Specifies the DEP status of a process.
    /// </summary>
    [Flags]
    public enum DepStatus
    {
        /// <summary>
        /// DEP is enabled.
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// DEP is permanently enabled or disabled and cannot
        /// be enabled or disabled.
        /// </summary>
        Permanent = 0x2,

        /// <summary>
        /// DEP is enabled with DEP-ATL thunk emulation disabled.
        /// </summary>
        AtlThunkEmulationDisabled = 0x4
    }

    /// <summary>
    /// A well-known Windows process.
    /// </summary>
    public enum KnownProcess
    {
        /// <summary>
        /// The process is not well-known.
        /// </summary>
        None,
        /// <summary>
        /// System Idle Process.
        /// </summary>
        Idle,
        /// <summary>
        /// NT Kernel &amp; System.
        /// </summary>
        System,
        /// <summary>
        /// Windows Session Manager (smss)
        /// </summary>
        SessionManager,
        /// <summary>
        /// Client Server Runtime Process (csrss)
        /// </summary>
        WindowsSubsystem,
        /// <summary>
        /// Windows Start-Up Application (wininit)
        /// </summary>
        WindowsStartup,
        /// <summary>
        /// Services and Controller app (services)
        /// </summary>
        ServiceControlManager,
        /// <summary>
        /// Local Security Authority Process (lsass)
        /// </summary>
        LocalSecurityAuthority,
        /// <summary>
        /// Local Session Manager Service (lsm)
        /// </summary>
        LocalSessionManager
    }

    /// <summary>
    /// Specifies an offset in a process' process environment block (PEB).
    /// </summary>
    public enum PebOffset
    {
        /// <summary>
        /// The current directory of the process. This may, as the name 
        /// implies, change very often.
        /// </summary>
        CurrentDirectoryPath,
        /// <summary>
        /// A copy of the PATH environment variable for the process.
        /// </summary>
        DllPath,
        /// <summary>
        /// The image file name, in kernel format (e.g. \\?\C:\...,
        /// \SystemRoot\..., \Device\Harddisk1\...).
        /// </summary>
        ImagePathName,
        /// <summary>
        /// The command used to start the program, including arguments.
        /// </summary>
        CommandLine,
        /// <summary>
        /// Usually blank.
        /// </summary>
        WindowTitle,
        /// <summary>
        /// For interactive programs, contains the window station and 
        /// desktop name of the first thread that was started, e.g. 
        /// WinSta0\Default.
        /// </summary>
        DesktopName,
        /// <summary>
        /// Usually blank.
        /// </summary>
        ShellInfo,
        /// <summary>
        /// Usually blank.
        /// </summary>
        RuntimeData
    }
}
