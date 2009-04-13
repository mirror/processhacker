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
using System.Threading;
using System.Windows.Forms;
using Microsoft.Samples;

namespace ProcessHacker
{
    public class Symbols : IDisposable
    {
        /// <summary>
        /// Specifies the detail with which the address's name was resolved.
        /// </summary>
        public enum FoundLevel
        {
            /// <summary>
            /// Indicates that the address was resolved to a module, a function and possibly an offset. 
            /// For example: mymodule.dll!MyExportedFunction+0x123
            /// </summary>
            Function,

            /// <summary>
            /// Indicates that the address was resolved to a module and an offset.
            /// For example: mymodule.dll+0x4321
            /// </summary>
            Module,

            /// <summary>
            /// Indicates that the address was not resolved.
            /// For example: 0x12345678
            /// </summary>
            Address,

            /// <summary>
            /// Indicates that the address was invalid (for example, 0x0).
            /// </summary>
            Invalid
        }

        private static object _callLock = new object();
        private static IdGenerator _idGen = new IdGenerator();

        public static Win32.SYMBOL_OPTIONS Options
        {
            get
            {
                lock (_callLock)
                    return Win32.SymGetOptions();
            }

            set
            {
                lock (_callLock)
                    Win32.SymSetOptions(value);
            }
        }

        private bool _disposed = false;
        private object _disposeLock = new object();
        private Win32.ProcessHandle _processHandle;
        private int _handle;
        private List<KeyValuePair<long, string>> _modules = new List<KeyValuePair<long, string>>();

        public Symbols()
        {
            _handle = _idGen.Pop();

            lock (_callLock)
            {
                if (!Win32.SymInitialize(_handle, null, false))
                    Win32.ThrowLastWin32Error();
            }
        }

        public Symbols(Win32.ProcessHandle processHandle)
        {
            _processHandle = processHandle;
            _handle = processHandle;

            lock (_callLock)
            {
                if (!Win32.SymInitialize(_handle, null, false))
                    Win32.ThrowLastWin32Error();
            }
        }

        public int Handle
        {
            get { return _handle; }
        }

        public string SearchPath
        {
            get
            {
                using (var data = new MemoryAlloc(0x1000))
                {
                    lock (_callLock)
                    {
                        if (!Win32.SymGetSearchPath(_handle, data, data.Size))
                            return "";
                    }

                    return Marshal.PtrToStringAnsi(data);
                }
            }

            set
            {
                lock (_callLock)
                    Win32.SymSetSearchPath(_handle, value);
            }
        }

        public bool PreloadModules { get; set; }

        public static void ShowWarning(IWin32Window window, bool force)
        {
            if (Properties.Settings.Default.DbgHelpWarningShown && !force)
                return;

            try
            {
                var modules = Win32.ProcessHandle.FromHandle(Program.CurrentProcess).GetModules();

                foreach (var module in modules)
                {
                    if (module.FileName.ToLowerInvariant().EndsWith("dbghelp.dll"))
                    {
                        FileInfo fi = new FileInfo(module.FileName);

                        if (!File.Exists(fi.DirectoryName + "\\symsrv.dll"))
                        {
                            if (!force)
                                Properties.Settings.Default.DbgHelpWarningShown = true;

                            if (Program.WindowsVersion != WindowsVersion.XP)
                            {
                                TaskDialog td = new TaskDialog();
                                bool verificationChecked;

                                td.CommonButtons = TaskDialogCommonButtons.Ok;
                                td.WindowTitle = "Process Hacker";
                                td.MainIcon = TaskDialogIcon.Warning;
                                td.MainInstruction = "Microsoft Symbol Server not supported";
                                td.Content = "The Microsoft Symbol Server is not supported by your version of dbghelp.dll " + 
                                    "or could not be loaded. " +
                                    "To ensure you have the latest version of dbghelp.dll, download " +
                                    "<a href=\"dbghelp\">Debugging " + 
                                    "Tools for Windows</a> and configure Process Hacker to " +
                                    "use its version of dbghelp.dll. If you have the latest version of dbghelp.dll, " +
                                    "ensure that symsrv.dll resides in the same directory as dbghelp.dll.";
                                td.EnableHyperlinks = true;
                                td.Callback = (taskDialog, args, callbackData) =>
                                    {
                                        if (args.Notification == TaskDialogNotification.HyperlinkClicked)
                                        {
                                            try
                                            {
                                                System.Diagnostics.Process.Start(
                                                    "http://www.microsoft.com/whdc/devtools/debugging/default.mspx");
                                            }
                                            catch
                                            { }

                                            return true;
                                        }

                                        return false;
                                    };
                                td.VerificationText = force ? null : "Do not display this warning again";
                                td.VerificationFlagChecked = true;

                                td.Show(window, out verificationChecked);

                                if (!force)
                                    Properties.Settings.Default.DbgHelpWarningShown = verificationChecked;
                            }
                            else
                            {
                                MessageBox.Show(window, "The Microsoft Symbol Server is not supported by your version of dbghelp.dll " +
                                    "or could not be loaded. To ensure you have the latest version of dbghelp.dll, download " +
                                    "Debugging Tools for Windows and configure Process Hacker to use its version of dbghelp.dll. " +
                                    "If you have the latest version of dbghelp.dll, ensure that symsrv.dll resides in the same " +
                                    "directory as dbghelp.dll.", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }

                        break;
                    }
                }
            }
            catch
            { }
        }

        public void LoadModule(string fileName, long baseAddress)
        {
            this.LoadModule(fileName, baseAddress, 0);
        }

        public void LoadModule(string fileName, long baseAddress, int size)
        {
            lock (_callLock)
            {
                if (Win32.SymLoadModule64(_handle, 0, fileName, null, baseAddress, size) == 0)
                    Win32.ThrowLastWin32Error();
            }

            lock (_modules)
            {
                _modules.Add(new KeyValuePair<long, string>(baseAddress, fileName));
                _modules.Sort((kvp1, kvp2) => kvp2.Key.CompareTo(kvp1.Key));
            }
        }

        public void UnloadModule(string fileName)
        {
            var pair = _modules.Find(kvp => string.Compare(kvp.Value, fileName, true) == 0);

            this.UnloadModule(pair.Key);
        }

        public void UnloadModule(long baseAddress)
        {
            lock (_callLock)
            {
                if (!Win32.SymUnloadModule64(_handle, baseAddress))
                    Win32.ThrowLastWin32Error();
            }
        }

        public string GetModuleFromAddress(long address, out long baseAddress)
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

        public string GetSymbolFromAddress(long address)
        {
            Win32.SYMBOL_FLAGS flags;

            return this.GetSymbolFromAddress(address, out flags);
        }

        public string GetSymbolFromAddress(long address, out FoundLevel level)
        {
            Win32.SYMBOL_FLAGS flags;
            string fileName;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(long address, out Win32.SYMBOL_FLAGS flags)
        {
            FoundLevel level;
            string fileName;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(long address, out string fileName)
        {
            FoundLevel level;
            Win32.SYMBOL_FLAGS flags;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(long address, out FoundLevel level, out Win32.SYMBOL_FLAGS flags, out string fileName)
        {
            const int maxNameLen = 0x100;
            long displacement;

            if (address == 0)
            {
                level = FoundLevel.Invalid;
                flags = 0;
                fileName = null;
            }

            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(Win32.SYMBOL_INFO)) + maxNameLen))
            {
                Win32.SYMBOL_INFO info = new Win32.SYMBOL_INFO();

                info.SizeOfStruct = Marshal.SizeOf(info);
                info.MaxNameLen = maxNameLen - 1;

                Marshal.StructureToPtr(info, data, false);

                try
                {
                    if (this.PreloadModules)
                    {
                        long b;

                        this.GetModuleFromAddress(address, out b);
                        Win32.SymFromAddr(_handle, b, out displacement, data);
                        Marshal.StructureToPtr(info, data, false);
                    }
                }
                catch
                { }

                lock (_callLock)
                {
                    if (Win32.SymFromAddr(_handle, address, out displacement, data))
                    {
                        info = data.ReadStruct<Win32.SYMBOL_INFO>();
                    }
                }

                string modFileName;
                long modBase;

                if (info.ModBase == 0)
                {
                    modFileName = this.GetModuleFromAddress(address, out modBase);
                }
                else
                {
                    modBase = info.ModBase;
                    modFileName = _modules.Find(kvp => kvp.Key == info.ModBase).Value;
                }

                if (modFileName == null)
                {
                    level = FoundLevel.Address;
                    flags = 0;
                    fileName = null;

                    return "0x" + address.ToString("x8");
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

                if (info.NameLen == 0)
                {
                    level = FoundLevel.Module;
                    flags = 0;

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

                string name = Marshal.PtrToStringAnsi(
                    new IntPtr(data + Marshal.OffsetOf(typeof(Win32.SYMBOL_INFO), "Name").ToInt32()), info.NameLen);

                level = FoundLevel.Function;
                flags = info.Flags;

                if (displacement == 0)
                    return fi.Name + "!" + name;
                else
                    return fi.Name + "!" + name + "+0x" + displacement.ToString("x");
            }
        }

        ~Symbols()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Monitor.Enter(_disposeLock);
                    Monitor.Enter(_callLock);
                }

                if (!_disposed)
                {
                    _disposed = true;
                    Win32.SymCleanup(_handle);
                    _idGen.Push(_handle);
                }
            }
            finally
            {
                if (disposing)
                {
                    Monitor.Exit(_callLock);
                    Monitor.Exit(_disposeLock);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
