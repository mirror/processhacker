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
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Symbols
{
    public class SymbolProvider : IDisposable
    {
        private static object _callLock = new object();
        private static IdGenerator _idGen = new IdGenerator();

        public static SymbolOptions Options
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
        private ProcessHandle _processHandle;
        private int _handle;
        private List<KeyValuePair<long, string>> _modules = new List<KeyValuePair<long, string>>();

        public SymbolProvider()
        {
            _handle = _idGen.Pop();

            lock (_callLock)
            {
                if (!Win32.SymInitialize(_handle, null, false))
                    Win32.ThrowLastError();
            }
        }

        public SymbolProvider(ProcessHandle processHandle)
        {
            _processHandle = processHandle;
            _handle = processHandle;

            lock (_callLock)
            {
                if (!Win32.SymInitialize(_handle, null, false))
                    Win32.ThrowLastError();
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

        public bool Busy
        {
            get
            {
                if (!Monitor.TryEnter(_callLock))
                {
                    return true;
                }
                else
                {
                    Monitor.Exit(_callLock);
                    return false;
                }
            }
        }

        public static void ShowWarning(IWin32Window window, bool force)
        {
            if (Properties.Settings.Default.DbgHelpWarningShown && !force)
                return;

            try
            {
                var modules = ProcessHandle.GetCurrent().GetModules();

                foreach (var module in modules)
                {
                    if (module.FileName.ToLowerInvariant().EndsWith("dbghelp.dll"))
                    {
                        FileInfo fi = new FileInfo(module.FileName);

                        if (!File.Exists(fi.DirectoryName + "\\symsrv.dll"))
                        {
                            if (!force)
                                Properties.Settings.Default.DbgHelpWarningShown = true;

                            if (OSVersion.HasTaskDialogs)
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
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show("Could not open the hyperlink: " + ex.ToString(),
                                                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }

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
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
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
                    Win32.ThrowLastError();
            }

            lock (_modules)
            {
                _modules.Add(new KeyValuePair<long, string>(baseAddress, fileName));
                _modules.Sort((kvp1, kvp2) => kvp2.Key.CompareTo(kvp1.Key));
            }
        }

        public void UnloadModule(string fileName)
        {
            KeyValuePair<long, string> pair;

            lock (_modules)
                pair = _modules.Find(kvp => string.Compare(kvp.Value, fileName, true) == 0);

            this.UnloadModule(pair.Key);
        }

        public void UnloadModule(long baseAddress)
        {
            lock (_callLock)
            {
                if (!Win32.SymUnloadModule64(_handle, baseAddress))
                    Win32.ThrowLastError();
            }

            lock (_modules)
                _modules.RemoveAll(kvp => kvp.Key == baseAddress);
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
            SymbolFlags flags;

            return this.GetSymbolFromAddress(address, out flags);
        }

        public string GetSymbolFromAddress(long address, out SymbolResolveLevel level)
        {
            SymbolFlags flags;
            string fileName;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(long address, out SymbolFlags flags)
        {
            SymbolResolveLevel level;
            string fileName;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(long address, out string fileName)
        {
            SymbolResolveLevel level;
            SymbolFlags flags;

            return this.GetSymbolFromAddress(address, out level, out flags, out fileName);
        }

        public string GetSymbolFromAddress(long address, out SymbolResolveLevel level, out SymbolFlags flags, out string fileName)
        {
            const int maxNameLen = 0x100;
            long displacement;

            if (address == 0)
            {
                level = SymbolResolveLevel.Invalid;
                flags = 0;
                fileName = null;
            }

            using (var data = new MemoryAlloc(Marshal.SizeOf(typeof(SymbolInfo)) + maxNameLen))
            {
                var info = new SymbolInfo();

                info.SizeOfStruct = Marshal.SizeOf(info);
                info.MaxNameLen = maxNameLen - 1;

                Marshal.StructureToPtr(info, data, false);

                try
                {
                    if (this.PreloadModules)
                    {
                        long b;

                        this.GetModuleFromAddress(address, out b);

                        lock (_callLock)
                            Win32.SymFromAddr(_handle, b, out displacement, data);

                        Marshal.StructureToPtr(info, data, false);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                lock (_callLock)
                {
                    if (Win32.SymFromAddr(_handle, address, out displacement, data))
                    {
                        info = data.ReadStruct<SymbolInfo>();
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

                    lock (_modules)
                        modFileName = _modules.Find(kvp => kvp.Key == info.ModBase).Value;
                }

                if (modFileName == null)
                {
                    level = SymbolResolveLevel.Address;
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
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                if (info.NameLen == 0)
                {
                    level = SymbolResolveLevel.Module;
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
                    new IntPtr(data + Marshal.OffsetOf(typeof(SymbolInfo), "Name").ToInt32()), info.NameLen);

                level = SymbolResolveLevel.Function;
                flags = info.Flags;

                if (displacement == 0)
                    return fi.Name + "!" + name;
                else
                    return fi.Name + "!" + name + "+0x" + displacement.ToString("x");
            }
        }

        ~SymbolProvider()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                Logging.Log(Logging.Importance.Information, "SymbolProvider: disposing (" + disposing.ToString() + ")");

                if (disposing)
                {
                    Monitor.Enter(_disposeLock);
                    Monitor.Enter(_callLock);
                }

                if (!_disposed)
                {
                    if (!Win32.SymCleanup(_handle))
                        Win32.ThrowLastError();

                    // If we didn't use a process handle, we got it from the ID generator
                    if (_processHandle == null)
                        _idGen.Push(_handle);
 
                    _disposed = true;
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

            Logging.Log(Logging.Importance.Information, "SymbolProvider: finished disposing (" + disposing.ToString() + ")");
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
