/*
 * Process Hacker - 
 *   run-time library debug buffer
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
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Debugging
{
    public delegate bool DebugEnumHeapsDelegate(HeapInformation heapInfo);
    public delegate bool DebugEnumLocksDelegate(LockInformation lockInfo);
    public delegate bool DebugEnumModulesDelegate(ModuleInformation moduleInfo);

    /// <summary>
    /// Represents a debug buffer managed by the run-time library.
    /// </summary>
    public sealed class DebugBuffer : BaseObject
    {
        private IntPtr _buffer;

        /// <summary>
        /// Creates a new debug buffer.
        /// </summary>
        public DebugBuffer()
        {
            _buffer = Win32.RtlCreateQueryDebugBuffer(0, false);

            if (_buffer == IntPtr.Zero)
            {
                this.DisableOwnership(false);
                throw new WindowsException(NtStatus.Unsuccessful);
            }
        }

        protected override void DisposeObject(bool disposing)
        {
            Win32.RtlDestroyQueryDebugBuffer(_buffer);
        }

        /// <summary>
        /// Enumerates heap information.
        /// </summary>
        /// <param name="callback">The callback for the enumeration.</param>
        public void EnumHeaps(DebugEnumHeapsDelegate callback)
        {
            var debugInfo = this.GetDebugInformation();

            if (debugInfo.Heaps == IntPtr.Zero)
                throw new InvalidOperationException("Heap information does not exist.");

            MemoryRegion heapInfo = new MemoryRegion(debugInfo.Heaps);
            var heaps = heapInfo.ReadStruct<RtlProcessHeaps>();

            for (int i = 0; i < heaps.NumberOfHeaps; i++)
            {
                var heap = heapInfo.ReadStruct<RtlHeapInformation>(RtlProcessHeaps.HeapsOffset, i);

                if (!callback(new HeapInformation(heap)))
                    break;
            }
        }

        /// <summary>
        /// Enumerates lock information.
        /// </summary>
        /// <param name="callback">The callback for the enumeration.</param>
        public void EnumLocks(DebugEnumLocksDelegate callback)
        {
            var debugInfo = this.GetDebugInformation();

            if (debugInfo.Locks == IntPtr.Zero)
                throw new InvalidOperationException("Lock information does not exist.");

            MemoryRegion locksInfo = new MemoryRegion(debugInfo.Locks);
            var locks = locksInfo.ReadStruct<RtlProcessLocks>();

            for (int i = 0; i < locks.NumberOfLocks; i++)
            {
                var lock_ = locksInfo.ReadStruct<RtlProcessLockInformation>(sizeof(int), i);

                if (!callback(new LockInformation(lock_)))
                    break;
            }
        }

        /// <summary>
        /// Enumerates module information.
        /// </summary>
        /// <param name="callback">The callback for the enumeration.</param>
        public void EnumModules(DebugEnumModulesDelegate callback)
        {
            var debugInfo = this.GetDebugInformation();

            if (debugInfo.Modules == IntPtr.Zero)
                throw new InvalidOperationException("Module information does not exist.");

            MemoryRegion modulesInfo = new MemoryRegion(debugInfo.Modules);
            var modules = modulesInfo.ReadStruct<RtlProcessModules>();

            for (int i = 0; i < modules.NumberOfModules; i++)
            {
                var module = modulesInfo.ReadStruct<RtlProcessModuleInformation>(RtlProcessModules.ModulesOffset, i);

                if (!callback(new ModuleInformation(module)))
                    break;
            }
        }

        /// <summary>
        /// Reads the debug information structure from the buffer.
        /// </summary>
        /// <returns>A RtlDebugInformation structure.</returns>
        private RtlDebugInformation GetDebugInformation()
        {
            MemoryRegion data = new MemoryRegion(_buffer);

            return data.ReadStruct<RtlDebugInformation>();
        }

        /// <summary>
        /// Gets heap information.
        /// </summary>
        /// <returns>An array of heap information objects.</returns>
        public HeapInformation[] GetHeaps()
        {
            List<HeapInformation> heaps = new List<HeapInformation>();

            this.EnumHeaps((heap) =>
            {
                heaps.Add(heap);
                return true;
            });

            return heaps.ToArray();
        }

        /// <summary>
        /// Gets lock information.
        /// </summary>
        /// <returns>An array of lock information objects.</returns>
        public LockInformation[] GetLocks()
        {
            List<LockInformation> locks = new List<LockInformation>();

            this.EnumLocks((lock_) =>
            {
                locks.Add(lock_);
                return true;
            });

            return locks.ToArray();
        }

        /// <summary>
        /// Gets module information.
        /// </summary>
        /// <returns>An array of module information objects.</returns>
        public ModuleInformation[] GetModules()
        {
            List<ModuleInformation> modules = new List<ModuleInformation>();

            this.EnumModules((module) =>
            {
                modules.Add(module);
                return true;
            });

            return modules.ToArray();
        }

        /// <summary>
        /// Queries debug information for the current process.
        /// </summary>
        /// <param name="flags">The information to query.</param>
        public void Query(RtlQueryProcessDebugFlags flags)
        {
            this.Query(ProcessHandle.GetCurrentId(), flags);
        }

        /// <summary>
        /// Queries debug information for the specified process.
        /// </summary>
        /// <param name="pid">The PID of the process to query.</param>
        /// <param name="flags">The information to query.</param>
        public void Query(int pid, RtlQueryProcessDebugFlags flags)
        {
            NtStatus status;

            if ((status = Win32.RtlQueryProcessDebugInformation(
                pid.ToIntPtr(),
                flags,
                _buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Queries back trace information for the current process.
        /// </summary>
        public void QueryBackTraces()
        {
            NtStatus status;

            if ((status = Win32.RtlQueryProcessBackTraceInformation(_buffer)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Queries heap information for the current process.
        /// </summary>
        public void QueryHeaps()
        {
            NtStatus status;

            if ((status = Win32.RtlQueryProcessHeapInformation(_buffer)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        /// <summary>
        /// Queries lock information for the current process.
        /// </summary>
        public void QueryLocks()
        {
            NtStatus status;

            if ((status = Win32.RtlQueryProcessLockInformation(_buffer)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        //public void QueryModules()
        //{
        //    this.QueryModules(null, RtlQueryProcessDebugFlags.Modules);
        //}

        //public void QueryModules(ProcessHandle processHandle, RtlQueryProcessDebugFlags flags)
        //{
        //    NtStatus status;

        //    if ((status = Win32.RtlQueryProcessModuleInformation(
        //        processHandle ?? IntPtr.Zero,
        //        flags,
        //        _buffer
        //        )) >= NtStatus.Error)
        //        Win32.ThrowLastError(status);
        //}
    }
}
