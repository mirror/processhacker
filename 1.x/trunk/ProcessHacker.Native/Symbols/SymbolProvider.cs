/*
 * Process Hacker - 
 *   dbghelp.dll wrapper code
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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Symbols
{
    public delegate bool SymbolEnumDelegate(SymbolInformation symbolInfo);

    public sealed class SymbolProvider : IDisposable
    {
        private sealed class SymbolHandle : BaseObject
        {
            private ProcessHandle _processHandle;
            private IntPtr _handle;

            public static implicit operator IntPtr(SymbolHandle symbolHandle)
            {
                return symbolHandle.Handle;
            }

            public SymbolHandle()
            {
                _handle = new IntPtr(_idGen.Pop());

                using (Win32.DbgHelpLock.AcquireContext())
                {
                    if (!Win32.SymInitialize(_handle, null, false))
                        Win32.Throw();
                }
            }

            public SymbolHandle(ProcessHandle processHandle)
            {
                _processHandle = processHandle;
                _handle = processHandle;

                using (Win32.DbgHelpLock.AcquireContext())
                {
                    if (!Win32.SymInitialize(_handle, null, false))
                        Win32.Throw();
                }

                _processHandle.Reference();
            }

            protected override void DisposeObject(bool disposing)
            {
                Win32.DbgHelpLock.Acquire();

                try
                {
                    Win32.SymCleanup(_handle);

                    // If we didn't use a process handle, we got it from the ID generator.
                    if (_processHandle == null)
                        _idGen.Push(_handle.ToInt32());
                    // Otherwise, dereference the process handle.
                    else
                        _processHandle.Dereference(disposing);
                }
                finally
                {
                    Win32.DbgHelpLock.Release();
                }
            }

            public IntPtr Handle
            {
                get { return _handle; }
            }
        }

        private const int _maxNameLen = 0x100;
        private static IdGenerator _idGen = new IdGenerator();

        public static SymbolOptions Options
        {
            get
            {
                using (Win32.DbgHelpLock.AcquireContext())
                    return Win32.SymGetOptions();
            }

            set
            {
                using (Win32.DbgHelpLock.AcquireContext())
                    Win32.SymSetOptions(value);
            }
        }

        private SymbolHandle _handle;
        private List<KeyValuePair<ulong, string>> _modules = new List<KeyValuePair<ulong, string>>();

        public SymbolProvider()
        {
            _handle = new SymbolHandle();
        }

        public SymbolProvider(ProcessHandle processHandle)
        {
            _handle = new SymbolHandle(processHandle);
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public bool Busy
        {
            get
            {
                if (!Win32.DbgHelpLock.TryAcquire())
                {
                    return true;
                }
                else
                {
                    Win32.DbgHelpLock.Release();
                    return false;
                }
            }
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public bool PreloadModules { get; set; }

        public string SearchPath
        {
            get
            {
                StringBuilder data = new StringBuilder(0x1000);

                using (Win32.DbgHelpLock.AcquireContext())
                {
                    if (!Win32.SymGetSearchPath(_handle, data, data.Capacity))
                        return "";
                }

                return data.ToString();
            }

            set
            {
                using (Win32.DbgHelpLock.AcquireContext())
                    Win32.SymSetSearchPath(_handle, value);
            }
        }

        public void EnumSymbols(ulong moduleBase, SymbolEnumDelegate enumDelegate)
        {
            this.EnumSymbols(moduleBase, null, enumDelegate);
        }

        public void EnumSymbols(string mask, SymbolEnumDelegate enumDelegate)
        {
            this.EnumSymbols(0, mask, enumDelegate);
        }

        public void EnumSymbols(ulong moduleBase, string mask, SymbolEnumDelegate enumDelegate)
        {
            using (Win32.DbgHelpLock.AcquireContext())
            {
                if (!Win32.SymEnumSymbols(
                    _handle,
                    moduleBase,
                    mask,
                    (symbolInfo, symbolSize, userContext) =>
                        enumDelegate(new SymbolInformation(symbolInfo, symbolSize)),
                    IntPtr.Zero
                    ))
                    Win32.Throw();
            }
        }

        public string GetLineFromAddress(ulong address)
        {
            string fileName;
            int lineNumber;

            this.GetLineFromAddress(address, out fileName, out lineNumber);

            if (fileName != null)
                return fileName + ": line " + lineNumber.ToString();
            else
                return null;
        }

        public void GetLineFromAddress(ulong address, out string fileName, out int lineNumber)
        {
            int displacement;

            this.GetLineFromAddress(address, out fileName, out lineNumber, out displacement);
        }

        public void GetLineFromAddress(ulong address, out string fileName, out int lineNumber, out int lineDisplacement)
        {
            ImagehlpLine64 line;
            int displacement;

            using (Win32.DbgHelpLock.AcquireContext())
            {
                if (!Win32.SymGetLineFromAddr64(_handle, address, out displacement, out line))
                {
                    fileName = null;
                    lineNumber = 0;
                    lineDisplacement = 0;

                    return;
                }

                fileName = line.FileName;
                lineNumber = line.LineNumber;
                lineDisplacement = displacement;
            }
        }

        public string GetModuleFromAddress(IntPtr address, out IntPtr baseAddress)
        {
            ulong baseAddressULong;
            string fileName = this.GetModuleFromAddress(address.ToUInt64(), out baseAddressULong);

            baseAddress = baseAddressULong.ToIntPtr();

            return fileName;
        }

        public string GetModuleFromAddress(ulong address, out ulong baseAddress)
        {
            lock (_modules)
            {
                foreach (var kvp in _modules)
                {
                    if (address >= kvp.Key)
                    {
                        baseAddress = kvp.Key;
                        return kvp.Value;
                    }
                }
            }

            baseAddress = 0;

            return null;
        }

        public string GetSymbolFromAddress(ulong address)
        {
            SymbolFlags flags;

            return this.GetSymbolFromAddress(address, out flags);
        }

        public string GetSymbolFromAddress(ulong address, out SymbolResolveLevel level)
        {
            SymbolFlags flags;
            string fileName;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(ulong address, out SymbolFlags flags)
        {
            SymbolResolveLevel level;
            string fileName;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(ulong address, out string fileName)
        {
            SymbolResolveLevel level;
            SymbolFlags flags;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(ulong address, out SymbolResolveLevel level, out SymbolFlags flags, out string fileName)
        {
            string symbolName;
            ulong displacement;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName, out symbolName, out displacement);
        }

        public string GetSymbolFromAddress(ulong address, out string fileName, out ulong displacement)
        {
            SymbolResolveLevel level;
            SymbolFlags flags;
            string symbolName;

            this.GetSymbolFromAddress(address, out level, out flags, out fileName, out symbolName, out displacement);

            return symbolName;
        }

        public string GetSymbolFromAddress(ulong address, out SymbolResolveLevel level, out SymbolFlags flags, out string fileName, out string symbolName, out ulong displacement)
        {
            // Assume failure (and stop the compiler from complaining).
            if (address == 0)
            {
                level = SymbolResolveLevel.Invalid;
                flags = 0;
                fileName = null;
            }

            // Allocate some memory for the symbol information.
            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(SymbolInfo)) + _maxNameLen))
            {
                var info = new SymbolInfo();

                info.SizeOfStruct = Marshal.SizeOf(info);
                info.MaxNameLen = _maxNameLen - 1;

                Marshal.StructureToPtr(info, data, false);

                // Hack for drivers, since we don't get their module sizes. 
                // Preloading modules will fix this.
                if (this.PreloadModules)
                {
                    ulong b;

                    this.GetModuleFromAddress(address, out b);

                    using (Win32.DbgHelpLock.AcquireContext())
                        Win32.SymFromAddr(_handle, b, out displacement, data);

                    Marshal.StructureToPtr(info, data, false);
                }

                // Get the symbol name.
                using (Win32.DbgHelpLock.AcquireContext())
                {
                    if (Win32.SymFromAddr(_handle, address, out displacement, data))
                    {
                        info = data.ReadStruct<SymbolInfo>();
                    }
                }

                string modFileName;
                ulong modBase;

                // Get the module name.
                if (info.ModBase == 0)
                {
                    modFileName = this.GetModuleFromAddress(address, out modBase);
                }
                else
                {
                    modBase = info.ModBase;

                    lock (_modules)
                        modFileName = _modules.Find(kvp => kvp.Key == info.ModBase).Value;
                }

                // If we don't have a module name, return an address.
                if (modFileName == null)
                {
                    level = SymbolResolveLevel.Address;
                    flags = 0;
                    fileName = null;
                    symbolName = null;

                    return Utils.FormatAddress(address);
                }

                FileInfo fi = null;

                fileName = modFileName;

                try
                {
                    fi = new FileInfo(modFileName);
                    fileName = fi.FullName;
                }
                catch
                { }

                // If we have a module name but not a symbol name, 
                // return a module plus an offset: module+offset.
                if (info.NameLen == 0)
                {
                    level = SymbolResolveLevel.Module;
                    flags = 0;
                    symbolName = null;

                    if (fi != null)
                    {
                        return fi.Name + "+0x" + (address - modBase).ToString("x");
                    }
                    else
                    {
                        var s = modFileName.Split('\\');

                        return s[s.Length - 1] + "+0x" + (address - modBase).ToString("x");
                    }
                }

                // If we have everything, return the full symbol name: module!symbol+offset.
                string name = data.ReadAnsiString(SymbolInfo.NameOffset, info.NameLen);

                level = SymbolResolveLevel.Function;
                flags = info.Flags;
                symbolName = name;

                if (displacement == 0)
                    return fi.Name + "!" + name;
                else
                    return fi.Name + "!" + name + "+0x" + displacement.ToString("x");
            }
        }

        public SymbolInformation GetSymbolFromName(string symbolName)
        {
            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(SymbolInfo)) + _maxNameLen))
            {
                var info = new SymbolInfo();

                info.SizeOfStruct = Marshal.SizeOf(info);
                info.MaxNameLen = _maxNameLen - 1;

                Marshal.StructureToPtr(info, data, false);

                using (Win32.DbgHelpLock.AcquireContext())
                {
                    if (!Win32.SymFromName(_handle, symbolName, data))
                        Win32.Throw();
                }

                return new SymbolInformation(data, 0);
            }
        }

        public void LoadModule(string fileName, IntPtr baseAddress)
        {
            this.LoadModule(fileName, baseAddress.ToUInt64());
        }

        public void LoadModule(string fileName, ulong baseAddress)
        {
            this.LoadModule(fileName, baseAddress, 0);
        }

        public void LoadModule(string fileName, IntPtr baseAddress, int size)
        {
            this.LoadModule(fileName, baseAddress.ToUInt64(), size);
        }

        public void LoadModule(string fileName, ulong baseAddress, int size)
        {
            using (Win32.DbgHelpLock.AcquireContext())
            {
                if (Win32.SymLoadModule64(_handle, IntPtr.Zero, fileName, null, baseAddress, size) == 0)
                {
                    if (Marshal.GetLastWin32Error() != 0)
                        Win32.Throw();
                }
            }

            lock (_modules)
            {
                _modules.Add(new KeyValuePair<ulong, string>(baseAddress, fileName));
                _modules.Sort((kvp1, kvp2) => kvp2.Key.CompareTo(kvp1.Key));
            }
        }

        public void UnloadModule(string fileName)
        {
            KeyValuePair<ulong, string> pair;

            lock (_modules)
                pair = _modules.Find(kvp => string.Compare(kvp.Value, fileName, true) == 0);

            this.UnloadModule(pair.Key);
        }

        public void UnloadModule(ulong baseAddress)
        {
            using (Win32.DbgHelpLock.AcquireContext())
            {
                if (!Win32.SymUnloadModule64(_handle, baseAddress))
                    Win32.Throw();
            }

            lock (_modules)
                _modules.RemoveAll(kvp => kvp.Key == baseAddress);
        }
    }
}
