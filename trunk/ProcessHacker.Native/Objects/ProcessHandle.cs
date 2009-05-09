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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a Windows process.
    /// </summary>
    /// <remarks>The idea of a ProcessHandle class is 
    /// different to the <see cref="System.Diagnostics.Process"/> class; 
    /// instead of opening the process with the right permissions every 
    /// time a query or set function is called, this lets the users control 
    /// when they want to open handles with certain permissions. This 
    /// means that handles can be cached (by the users).</remarks>
    public class ProcessHandle : Win32Handle<ProcessAccess>, IWithToken
    {
        /// <summary>
        /// The callback for enumerating process memory regions.
        /// </summary>
        /// <param name="mbi">The basic information for the memory region.</param>
        /// <returns>Return true to continue enumerating; return false to stop.</returns>
        public delegate bool EnumMemoryDelegate(MemoryBasicInformation info);

        /// <summary>
        /// The callback for enumerating process modules.
        /// </summary>
        /// <param name="module">The module information.</param>
        /// <returns>Return true to continue enumerating; return false to stop.</returns>
        public delegate bool EnumModulesDelegate(ProcessModule module);

        public static ProcessHandle Create(SectionHandle sectionHandle, ProcessAccess access, ProcessHandle parent, bool inheritHandles)
        {
            int status;
            IntPtr process;

            if ((status = Win32.NtCreateProcess(
                out process,
                access,
                IntPtr.Zero,
                parent,
                inheritHandles,
                sectionHandle,
                IntPtr.Zero,
                IntPtr.Zero)) < 0)
                Win32.ThrowLastError(status);

            return new ProcessHandle(process, true);
        }

        public static ProcessHandle Create(string fileName, ProcessAccess access, bool inheritHandles)
        {
            using (var fhandle = new FileHandle(
                fileName,
                (FileAccess)StandardRights.Synchronize | FileAccess.Execute | FileAccess.ReadData,
                FileShareMode.Delete | FileShareMode.Read, FileCreationDisposition.OpenAlways))
            {
                using (var shandle = new SectionHandle(
                    SectionAccess.All, fhandle,
                    SectionAttributes.Image, MemoryProtection.Execute))
                {
                    return Create(shandle, access, ProcessHandle.GetCurrent(), inheritHandles);
                }
            }
        }

        /// <summary>
        /// Creates a process handle using an existing handle. 
        /// The handle will not be closed automatically.
        /// </summary>
        /// <param name="Handle">The handle value.</param>
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
            return new ProcessHandle(new IntPtr(-1), false);
        }

        internal ProcessHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        /// <summary>
        /// Creates a new process handle.
        /// </summary>
        /// <param name="PID">The ID of the process to open.</param>
        public ProcessHandle(int pid)
            : this(pid, ProcessAccess.All)
        { }

        /// <summary>
        /// Creates a new process handle.
        /// </summary>
        /// <param name="PID">The ID of the process to open.</param>
        /// <param name="access">The desired access to the process.</param>
        public ProcessHandle(int pid, ProcessAccess access)
        {
            if (KProcessHacker.Instance != null)
                this.Handle = new IntPtr(KProcessHacker.Instance.KphOpenProcess(pid, access));
            else
                this.Handle = Win32.OpenProcess(access, false, pid);

            if (this.Handle == IntPtr.Zero)
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Allocates a memory region in the process' virtual memory.
        /// </summary>      
        /// <param name="address">The base address of the region.</param>
        /// <param name="size">The size of the region.</param>
        /// <param name="protection">The protection of the region.</param>
        /// <returns>The base address of the allocated pages.</returns>
        public IntPtr AllocMemory(IntPtr address, int size, MemoryProtection protection)
        {
            IntPtr newAddress;

            if ((newAddress = Win32.VirtualAllocEx(this, address, size, MemoryState.Commit, protection))
                == IntPtr.Zero)
                Win32.ThrowLastError();

            return newAddress;
        }

        /// <summary>
        /// Allocates a memory region in the process' virtual memory. The function decides where 
        /// to allocate the memory.
        /// </summary>
        /// <param name="size">The size of the region.</param>
        /// <param name="protection">The protection of the region.</param>
        /// <returns>The base address of the allocated pages.</returns>
        public IntPtr AllocMemory(int size, MemoryProtection protection)
        {
            return this.AllocMemory(IntPtr.Zero, size, protection);
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
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Creates a remote thread in the process.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution (e.g. a function). The 
        /// function must be accessible from the remote process; that is, it must be in its 
        /// virtual address space, either copied using AllocMemory or loaded as module using 
        /// LoadLibrary.
        /// </param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <returns>The ID of the new thread.</returns>
        public int CreateThread(IntPtr startAddress, IntPtr parameter)
        {
            int threadId;

            if (!Win32.CreateRemoteThread(this, IntPtr.Zero, 0, startAddress, parameter, 0, out threadId))
                Win32.ThrowLastError();

            return threadId;
        }

        /// <summary>
        /// Creates a remote thread in the process, returning a handle to the new thread.
        /// </summary>
        /// <param name="startAddress">The address at which to begin execution (e.g. a function). The 
        /// function must be accessible from the remote process; that is, it must be in its 
        /// virtual address space, either copied using AllocMemory or loaded as module using 
        /// LoadLibrary.
        /// </param>
        /// <param name="parameter">The parameter to pass to the function.</param>
        /// <param name="access">The desired access to the new thread.</param>
        /// <returns>A handle to the new thread.</returns>
        public ThreadHandle CreateThread(IntPtr startAddress, IntPtr parameter, ThreadAccess access)
        {
            return new ThreadHandle(this.CreateThread(startAddress, parameter), access);
        }

        /// <summary>
        /// Removes as many pages as possible from the process' working set. This requires the 
        /// PROCESS_QUERY_INFORMATION and PROCESS_SET_INFORMATION permissions.
        /// </summary>
        public void EmptyWorkingSet()
        {
            if (!Win32.EmptyWorkingSet(this))
                Win32.ThrowLastError();
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
                //address += mbi.RegionSize;
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

        private void EnumModulesApi(EnumModulesDelegate enumModulesCallback)
        {
            IntPtr[] moduleHandles;
            int requiredSize;

            Win32.EnumProcessModules(this, null, 0, out requiredSize);
            moduleHandles = new IntPtr[requiredSize / 4];

            if (!Win32.EnumProcessModules(this, moduleHandles, requiredSize, out requiredSize))
                Win32.ThrowLastError();

            for (int i = 0; i < moduleHandles.Length; i++)
            {
                ModuleInfo moduleInfo = new ModuleInfo();
                StringBuilder baseName = new StringBuilder(0x400);
                StringBuilder fileName = new StringBuilder(0x400);

                if (!Win32.GetModuleInformation(this, moduleHandles[i], moduleInfo, Marshal.SizeOf(moduleInfo)))
                    Win32.ThrowLastError();
                if (Win32.GetModuleBaseName(this, moduleHandles[i], baseName, baseName.Capacity * 2) == 0)
                    Win32.ThrowLastError();
                if (Win32.GetModuleFileNameEx(this, moduleHandles[i], fileName, fileName.Capacity * 2) == 0)
                    Win32.ThrowLastError();

                if (!enumModulesCallback(new ProcessModule(
                    moduleInfo.BaseOfDll, moduleInfo.SizeOfImage, moduleInfo.EntryPoint,
                    baseName.ToString(), FileUtils.FixPath(fileName.ToString())
                    )))
                    break;
            }
        }

        private unsafe void EnumModulesNative(EnumModulesDelegate enumModulesCallback)
        {
            byte* buffer = stackalloc byte[4];

            this.ReadMemory(this.GetBasicInformation().PebBaseAddress.Increment(0xc), buffer, 4);

            IntPtr loaderData = new IntPtr(*(int*)buffer);

            PebLdrData* data = stackalloc PebLdrData[1];
            this.ReadMemory(loaderData, data, Marshal.SizeOf(typeof(PebLdrData)));

            if (data->Initialized == 0)
                throw new Exception("Loader data is not initialized.");

            IntPtr currentLink = data->InLoadOrderModuleList.Flink;
            IntPtr startLink = currentLink;
            LdrModule* currentModule = stackalloc LdrModule[1];
            int i = 0;

            while (currentLink != IntPtr.Zero)
            {
                // Stop when we have reached the beginning of the linked list
                if (i > 0 && currentLink == startLink)
                    break;
                // Safety guard
                if (i > 0x800)
                    break;

                this.ReadMemory(currentLink, currentModule, Marshal.SizeOf(typeof(LdrModule)));

                if (currentModule->BaseAddress != IntPtr.Zero)
                {
                    string baseDllName = null;
                    string fullDllName = null;

                    try
                    {
                        baseDllName = Utils.ReadUnicodeString(this, currentModule->BaseDllName).TrimEnd('\0');
                    }
                    catch
                    { }

                    try
                    {
                        fullDllName =
                            FileUtils.FixPath(Utils.ReadUnicodeString(this, currentModule->FullDllName).TrimEnd('\0'));
                    }
                    catch
                    { }

                    if (!enumModulesCallback(new ProcessModule(
                        currentModule->BaseAddress,
                        currentModule->SizeOfImage,
                        currentModule->EntryPoint,
                        baseDllName,
                        fullDllName
                        )))
                        break;
                }

                currentLink = currentModule->InLoadOrderModuleList.Flink;
                i++;
            }
        }

        /// <summary>
        /// Frees a memory region in the process' virtual memory.
        /// </summary>
        /// <param name="address">The address of the region to free.</param>
        /// <param name="size">The size to free.</param>
        /// <param name="reserveOnly">Specifies whether or not to only 
        /// reserve the memory instead of freeing it.</param>
        public void FreeMemory(IntPtr address, int size, bool reserveOnly)
        {
            // size needs to be 0 if we're freeing
            if (!reserveOnly)
                size = 0;

            if (!Win32.VirtualFreeEx(this, address, size,
                reserveOnly ? MemoryState.Decommit : MemoryState.Release))
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Gets the base priority of the process.
        /// </summary>
        public int GetBasePriority()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessBasePriority);
        }

        /// <summary>
        /// Gets the process' basic information through the undocumented Native API function 
        /// NtQueryInformationProcess. This function requires the PROCESS_QUERY_LIMITED_INFORMATION 
        /// permission.
        /// </summary>
        /// <returns>A PROCESS_BASIC_INFORMATION structure.</returns>
        public ProcessBasicInformation GetBasicInformation()
        {
            int status;
            ProcessBasicInformation pbi;
            int retLen;

            if ((status = Win32.NtQueryInformationProcess(this, ProcessInformationClass.ProcessBasicInformation,
                out pbi, Marshal.SizeOf(typeof(ProcessBasicInformation)), out retLen)) < 0)
                Win32.ThrowLastError(status);

            return pbi;
        }

        /// <summary>
        /// Gets the command line used to start the process. This requires 
        /// the PROCESS_QUERY_LIMITED_INFORMATION and PROCESS_VM_READ permissions.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetCommandLine()
        {
            return this.GetPebString(PebOffset.CommandLine);
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
        public FileTime GetCreateTime()
        {
            return this.GetTimes()[0];
        }

        /// <summary>
        /// Gets the number of processor cycles consumed by the process' threads.
        /// </summary>
        public ulong GetCycleTime()
        {
            ulong cycles;

            if (!Win32.QueryProcessCycleTime(this, out cycles))
                Win32.ThrowLastError();

            return cycles;
        }

        /// <summary>
        /// Gets the process' DEP policy.
        /// </summary>
        /// <returns>A DEPStatus enum.</returns>
        public DepStatus GetDepStatus()
        {
            int status;
            MemExecuteOptions options;
            int retLength;

            if ((status = Win32.NtQueryInformationProcess(this, ProcessInformationClass.ProcessExecuteFlags,
                out options, 4, out retLength)) < 0)
                Win32.ThrowLastError(status);

            DepStatus depStatus = 0;

            // Check if execution of data pages is enabled.
            if ((options & MemExecuteOptions.ExecuteEnable) != 0)
                return 0;

            // Check if execution of data pages is disabled.
            if ((options & MemExecuteOptions.ExecuteDisable) != 0)
                depStatus = DepStatus.Enabled;
            // ExecuteDisable and ExecuteEnable are both disabled in OptOut mode.
            else if ((options & MemExecuteOptions.ExecuteDisable) == 0 &&
                (options & MemExecuteOptions.ExecuteEnable) == 0)
                depStatus = DepStatus.Enabled;

            if ((options & MemExecuteOptions.DisableThunkEmulation) != 0)
                depStatus |= DepStatus.AtlThunkEmulationDisabled;
            if ((options & MemExecuteOptions.Permanent) != 0)
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

            this.ReadMemory(pebBaseAddress.Increment(0x10), buffer, IntPtr.Size);
            IntPtr processParameters = *(IntPtr*)buffer;

            /*
             * RTL_USER_PROCESS_PARAMETERS
             * off field
             * +00 ULONG MaximumLength
             * +04 ULONG Length
             * +08 ULONG Flags
             * +0c ULONG DebugFlags
             * +10 PVOID ConsoleHandle
             * +14 ULONG ConsoleFlags
             * +18 HANDLE StdInputHandle
             * +1c HANDLE StdOutputHandle
             * +20 HANDLE StdErrorHandle
             * +24 UNICODE_STRING CurrentDirectoryPath
             * +2c HANDLE CurrentDirectoryHandle
             * +30 UNICODE_STRING DllPath
             * +38 UNICODE_STRING ImagePathName
             * +40 UNICODE_STRING CommandLine
             * +48 PVOID Environment
             */
            this.ReadMemory(processParameters.Increment(0x48), buffer, IntPtr.Size);
            IntPtr envBase = *(IntPtr*)buffer;
            int length = 0;

            {
                MemoryBasicInformation mbi = this.QueryMemory(envBase);

                if (mbi.Protect == MemoryProtection.NoAccess)
                    throw new WindowsException();

                length = mbi.RegionSize - envBase.Decrement(mbi.BaseAddress).ToInt32();
            }

            // Now we read in the entire region of memory
            // And yes, some memory is wasted.
            byte[] memory = this.ReadMemory(envBase, length);

            /* The environment variables block is a series of Unicode strings separated by 
             * two null bytes. The entire block is terminated by four null bytes.
             */
            Dictionary<string, string> vars = new Dictionary<string, string>();
            StringBuilder currentVariable = new StringBuilder();
            int i = 0;

            while (true)
            {
                char currentChar =
                    UnicodeEncoding.Unicode.GetChars(memory, i, 2)[0];

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

        /// <summary>
        /// Gets the process' exit code.
        /// </summary>
        /// <returns>A number.</returns>
        public int GetExitCode()
        {
            int exitCode;

            if (!Win32.GetExitCodeProcess(this, out exitCode))
                Win32.ThrowLastError();

            return exitCode;
        }

        /// <summary>
        /// Gets the exit time of the process.
        /// </summary>
        /// <returns></returns>
        public FileTime GetExitTime()
        {
            return this.GetTimes()[1];
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
        /// Gets the file name of the process' image. This requires the
        /// PROCESS_QUERY_LIMITED_INFORMATION permission.
        /// </summary>
        /// <returns>A file name, in DOS (normal) format.</returns>
        public string GetImageFileName()
        {
            var sb = new StringBuilder(1024);
            int len = 1024;

            if (!Win32.QueryFullProcessImageName(this, false, sb, ref len))
                Win32.ThrowLastError();

            return FileUtils.FixPath(sb.ToString(0, len));
        }

        private int GetInformationInt32(ProcessInformationClass infoClass)
        {
            int status;
            MemoryAlloc value = new MemoryAlloc(4);
            int retLength;

            if ((status = Win32.NtQueryInformationProcess(
                this, infoClass, value, 4, out retLength)) < 0)
                Win32.ThrowLastError(status);

            return value.ReadInt32(0);
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
        /// Opens the job associated with the process.
        /// </summary>
        /// <returns>A job handle.</returns>
        public JobObjectHandle GetJob(JobObjectAccess access)
        {
            return new JobObjectHandle(this, access);
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

        public string GetMappedFileName(IntPtr address)
        {
            StringBuilder sb = new StringBuilder(0x400);
            int length = Win32.GetMappedFileName(this, address, sb, sb.Capacity);

            if (length > 0)
            {
                string fileName = sb.ToString(0, length);

                if (fileName.StartsWith("\\"))
                    fileName = FileUtils.DeviceFileNameToDos(fileName);

                System.IO.FileInfo fi = new System.IO.FileInfo(fileName);

                return fi.ToString();
            }

            return null;
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
        /// Gets the file name of the process' image, in device name format. This 
        /// requires the PROCESS_QUERY_LIMITED_INFORMATION permission.
        /// </summary>
        /// <returns>A file name, in device/native format.</returns>
        public string GetNativeImageFileName()
        {
            int status;
            int retLen;

            Win32.NtQueryInformationProcess(this, ProcessInformationClass.ProcessImageFileName,
                IntPtr.Zero, 0, out retLen);

            using (MemoryAlloc data = new MemoryAlloc(retLen))
            {
                if ((status = Win32.NtQueryInformationProcess(this, ProcessInformationClass.ProcessImageFileName,
                    data, retLen, out retLen)) < 0)
                    Win32.ThrowLastError(status);

                UnicodeString str = data.ReadStruct<UnicodeString>();

                return Utils.ReadUnicodeString(str);
            }
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
            return this.GetBasicInformation().InheritedFromUniqueProcessId;
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

            /* read address of parameter information block
             *
             * PEB
             * off field
             * +00 BOOLEAN InheritedAddressSpace;
             * +01 BOOLEAN ReadImageFileExecOptions;
             * +02 BOOLEAN BeingDebugged;
             * +03 BOOLEAN Spare;
             * +04 HANDLE Mutant;
             * +08 PVOID ImageBaseAddress;
             * +0c PVOID LoaderData;
             * +10 PRTL_USER_PROCESS_PARAMETERS ProcessParameters; 
             */
            this.ReadMemory(pebBaseAddress.Increment(0x10), buffer, IntPtr.Size);
            IntPtr processParameters = *(IntPtr*)buffer;

            // Read length (in bytes) of string. The offset of the UNICODE_STRING structure is 
            // specified in the enum.
            //
            // UNICODE_STRING
            // off field
            // +00 USHORT Length;
            // +02 USHORT MaximumLength;
            // +04 PWSTR Buffer;
            this.ReadMemory(processParameters.Increment((int)offset), buffer, 2);
            ushort stringLength = *(ushort*)buffer;
            byte[] stringData = new byte[stringLength];

            // read address of string
            this.ReadMemory(processParameters.Increment((int)offset + 0x4), buffer, 4);
            IntPtr stringAddr = *(IntPtr*)buffer;

            // read string and decode it
            return UnicodeEncoding.Unicode.GetString(
                this.ReadMemory(stringAddr, stringLength)).TrimEnd('\0');
        }

        /// <summary>
        /// Gets the process' priority class.
        /// </summary>
        /// <returns>A ProcessPriorityClass enum.</returns>
        public ProcessPriorityClass GetPriorityClass()
        {
            int priority = Win32.GetPriorityClass(this);

            if (priority == 0)
                Win32.ThrowLastError();

            return (ProcessPriorityClass)priority;
        }

        /// <summary>
        /// Gets the process' unique identifier.
        /// </summary>
        public int GetProcessId()
        {
            return this.GetBasicInformation().UniqueProcessId;
        }

        /// <summary>
        /// Gets the process' session ID.
        /// </summary>
        public int GetSessionId()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessSessionInformation);
        }

        public FileTime[] GetTimes()
        {
            FileTime[] times = new FileTime[4];

            if (!Win32.GetProcessTimes(this, out times[0], out times[1], out times[2], out times[3]))
                Win32.ThrowLastError();

            return times;
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
        /// <param name="timeout">The timeout, in seconds, for the process to load the library.</param>
        public void InjectDll(string path, uint timeout)
        {
            IntPtr stringPage = this.AllocMemory(path.Length * 2 + 2, MemoryProtection.ExecuteReadWrite);

            this.WriteMemory(stringPage, UnicodeEncoding.Unicode.GetBytes(path));

            this.CreateThread(Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryW"),
                stringPage, ThreadAccess.All).Wait(timeout);

            this.FreeMemory(stringPage, path.Length * 2 + 2, false);
        }

        /// <summary>
        /// Gets whether the process is currently being debugged. This requires 
        /// the PROCESS_QUERY_INFORMATION permission.
        /// </summary>
        public bool IsBeingDebugged()
        {
            bool debugged = false;

            if (!Win32.CheckRemoteDebuggerPresent(this, ref debugged))
                Win32.ThrowLastError();

            return debugged;
        }

        /// <summary>
        /// Gets whether the process is being debugged.
        /// </summary>
        public bool IsBeingDebuggedNative()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessDebugFlags) != 0;
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
        /// <remarks>According to this function, almost every single 
        /// process is in a job! This function does not tell us 
        /// the name of the job though.</remarks>
        public bool IsInJob()
        {
            bool result;

            if (!Win32.IsProcessInJob(this, IntPtr.Zero, out result))
                Win32.ThrowLastError();

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
        /// Gets whether the process has priority boost enabled.
        /// </summary>
        public bool IsPriorityBoostEnabled()
        {
            return this.GetInformationInt32(ProcessInformationClass.ProcessPriorityBoost) == 0;
        }

        /// <summary>
        /// Sets the protection for a page in the process.
        /// </summary>
        /// <param name="address">The address to modify.</param>
        /// <param name="size">The number of bytes to modify.</param>
        /// <param name="protection">The new memory protection.</param>
        /// <returns>The old memory protection.</returns>
        public MemoryProtection ProtectMemory(IntPtr address, int size, MemoryProtection protection)
        {
            MemoryProtection oldProtection;

            if (!Win32.VirtualProtectEx(this,address, size, protection, out oldProtection))
                Win32.ThrowLastError();

            return oldProtection;
        }

        /// <summary>
        /// Gets information about the memory region starting at the specified address.
        /// </summary>
        /// <param name="address">The address to query.</param>
        /// <returns>A MEMORY_BASIC_INFORMATION structure.</returns>
        public MemoryBasicInformation QueryMemory(IntPtr address)
        {
            MemoryBasicInformation mbi = new MemoryBasicInformation();

            if (Win32.VirtualQueryEx(this, address, out mbi, Marshal.SizeOf(mbi)) == 0)
                Win32.ThrowLastError();

            return mbi;
        }

        /// <summary>
        /// Reads data from the process' virtual memory.
        /// </summary>
        /// <param name="offset">The offset at which to begin reading.</param>
        /// <param name="length">The length, in bytes, to read.</param>
        /// <returns>An array of bytes</returns>
        public byte[] ReadMemory(IntPtr offset, int length)
        {
            byte[] buffer = new byte[length];

            this.ReadMemory(offset, buffer, length);

            return buffer;
        }

        public unsafe int ReadMemory(IntPtr offset, byte[] buffer, int length)
        {
            fixed (byte* bufferPtr = buffer)
                return this.ReadMemory(offset, bufferPtr, length);
        }

        public unsafe int ReadMemory(IntPtr offset, void* buffer, int length)
        {
            int readLen;

            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphReadVirtualMemory(this, offset.ToInt32(), buffer, length, out readLen);
            }
            else
            {
                if (!Win32.ReadProcessMemory(this, offset, buffer, length, out readLen))
                    Win32.ThrowLastError();
            }

            return readLen;
        }

        /// <summary>
        /// Resumes the process. This requires the PROCESS_SUSPEND_RESUME permission.
        /// </summary>
        public void Resume()
        {
            if (KProcessHacker.Instance != null && OSVersion.HasPsSuspendResumeProcess)
            {
                KProcessHacker.Instance.KphResumeProcess(this);
            }
            else
            {
                int status;

                if ((status = Win32.NtResumeProcess(this)) < 0)
                    Win32.ThrowLastError(status);
            }
        }

        public unsafe void SetModuleReferenceCount(IntPtr baseAddress, ushort count)
        {
            byte* buffer = stackalloc byte[4];

            this.ReadMemory(this.GetBasicInformation().PebBaseAddress.Increment(0xc), buffer, 4);

            IntPtr loaderData = new IntPtr(*(int*)buffer);

            PebLdrData* data = stackalloc PebLdrData[1];
            this.ReadMemory(loaderData, data, Marshal.SizeOf(typeof(PebLdrData)));

            if (data->Initialized == 0)
                throw new Exception("Loader data is not initialized.");

            List<ProcessModule> modules = new List<ProcessModule>();
            IntPtr currentLink = data->InLoadOrderModuleList.Flink;
            IntPtr startLink = currentLink;
            LdrModule* currentModule = stackalloc LdrModule[1];
            int i = 0;

            while (currentLink != IntPtr.Zero)
            {
                if (modules.Count > 0 && currentLink == startLink)
                    break;
                if (i > 0x800)
                    break;

                this.ReadMemory(currentLink, currentModule, Marshal.SizeOf(typeof(LdrModule)));

                if (currentModule->BaseAddress == baseAddress)
                {
                    this.WriteMemory(currentLink.Increment(0x38), &count, 2);
                    break;
                }

                currentLink = currentModule->InLoadOrderModuleList.Flink;
                i++;
            }
        }

        /// <summary>
        /// Sets the process' priority class.
        /// </summary>
        /// <param name="priority">The process' priority.</param>
        public void SetPriorityClass(ProcessPriorityClass priority)
        {
            if (!Win32.SetPriorityClass(this, (int)priority))
                Win32.ThrowLastError();
        }

        /// <summary>
        /// Suspends the process. This requires the PROCESS_SUSPEND_RESUME permission.
        /// </summary>
        public void Suspend()
        {
            if (KProcessHacker.Instance != null && OSVersion.HasPsSuspendResumeProcess)
            {
                KProcessHacker.Instance.KphSuspendProcess(this);
            }
            else
            {
                int status;

                if ((status = Win32.NtSuspendProcess(this)) < 0)
                    Win32.ThrowLastError(status);
            }
        }

        /// <summary>
        /// Terminates the process. This requires the PROCESS_TERMINATE permission.
        /// </summary>
        public void Terminate()
        {
            this.Terminate(0);
        }

        /// <summary>
        /// Terminates the process, specifying the exit code. This requires the 
        /// PROCESS_TERMINATE permission.
        /// </summary>
        /// <param name="ExitCode">The exit code.</param>
        public void Terminate(int ExitCode)
        {
            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphTerminateProcess(this, ExitCode);
            }
            else
            {
                if (!Win32.TerminateProcess(this, ExitCode))
                    Win32.ThrowLastError();
            }
        }

        /// <summary>
        /// Writes data to the process' virtual memory.
        /// </summary>
        /// <param name="offset">The offset at which to begin writing.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>The length, in bytes, that was written.</returns>
        public unsafe int WriteMemory(IntPtr offset, byte[] data)
        {
            fixed (byte* dataPtr = data)
            {
                return WriteMemory(offset, dataPtr, data.Length);
            }
        }

        /// <summary>
        /// Writes data to the process' virtual memory.
        /// </summary>
        /// <param name="offset">The offset at which to begin writing.</param>
        /// <param name="data">The data to write.</param>
        /// <param name="length">The length to be written.</param>
        /// <returns>The length, in bytes, that was written.</returns>
        public unsafe int WriteMemory(IntPtr offset, void* data, int length)
        {
            int writtenLen;

            if (KProcessHacker.Instance != null)
            {
                KProcessHacker.Instance.KphWriteVirtualMemory(this, offset.ToInt32(), data, length, out writtenLen);
            }
            else
            {
                if (!Win32.WriteProcessMemory(this, offset, data, length, out writtenLen))
                    Win32.ThrowLastError();
            }

            return writtenLen;
        }

        /// <summary>
        /// Opens and returns a handle to the process' token. This requires the 
        /// PROCESS_QUERY_LIMITED_INFORMATION permission.
        /// </summary>
        /// <returns>A handle to the process' token.</returns>
        public TokenHandle GetToken()
        {
            return GetToken(TokenAccess.All);
        }

        /// <summary>
        /// Opens and returns a handle to the process' token. This requires the 
        /// PROCESS_QUERY_LIMITED_INFORMATION permission.
        /// </summary>
        /// <param name="access">The desired access to the token.</param>
        /// <returns>A handle to the process' token.</returns>
        public TokenHandle GetToken(TokenAccess access)
        {
            return new TokenHandle(this, access);
        }
    }

    public class ProcessModule
    {
        public ProcessModule(IntPtr baseAddress, int size, IntPtr entryPoint, string baseName, string fileName)
        {
            this.BaseAddress = baseAddress;
            this.Size = size;
            this.EntryPoint = entryPoint;
            this.BaseName = baseName;
            this.FileName = fileName;
        }

        public IntPtr BaseAddress { get; private set; }
        public int Size { get; private set; }
        public IntPtr EntryPoint { get; private set; }
        public string BaseName { get; private set; }
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
    /// Specifies an offset in a process' process environment block (PEB).
    /// </summary>
    public enum PebOffset
    {
        /// <summary>
        /// The current directory of the process. This may, as the name 
        /// implies, change very often.
        /// </summary>
        CurrentDirectoryPath = 0x24,

        /// <summary>
        /// A copy of the PATH environment variable for the process.
        /// </summary>
        DllPath = 0x30,

        /// <summary>
        /// The image file name, in kernel format (e.g. \\?\C:\...,
        /// \SystemRoot\..., \Device\Harddisk1\...).
        /// </summary>
        ImagePathName = 0x38,

        /// <summary>
        /// The command used to start the program, including arguments.
        /// </summary>
        CommandLine = 0x40,

        /// <summary>
        /// Usually blank.
        /// </summary>
        WindowTitle = 0x70,

        /// <summary>
        /// For interactive programs, contains the window station and 
        /// desktop name of the first thread that was started, e.g. 
        /// WinSta0\Default.
        /// </summary>
        DesktopName = 0x78,

        /// <summary>
        /// Usually blank.
        /// </summary>
        ShellInfo = 0x80,

        /// <summary>
        /// Usually blank.
        /// </summary>
        RuntimeData = 0x88
    }
}
