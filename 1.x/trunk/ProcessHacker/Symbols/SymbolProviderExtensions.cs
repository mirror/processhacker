/*
 * Process Hacker - 
 *   symbols extension functions
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
using System.IO;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Symbols
{
    public static class SymbolProviderExtensions
    {
        public static void ShowWarning(IWin32Window window, bool force)
        {
            // The message is too clumsy to display with a standard 
            // message box.
            if (!OSVersion.HasTaskDialogs)
                return;

            if (Settings.Instance.DbgHelpWarningShown && !force)
                return;

            try
            {
                var modules = ProcessHandle.Current.GetModules();

                foreach (var module in modules)
                {
                    if (module.FileName.ToLowerInvariant().EndsWith("dbghelp.dll"))
                    {
                        if (!File.Exists(Path.GetDirectoryName(module.FileName) + "\\symsrv.dll"))
                        {
                            if (!force)
                                Settings.Instance.DbgHelpWarningShown = true;

                            TaskDialog td = new TaskDialog();
                            bool verificationChecked;

                            td.CommonButtons = TaskDialogCommonButtons.Ok;
                            td.WindowTitle = "Process Hacker";
                            td.MainIcon = TaskDialogIcon.Warning;
                            td.MainInstruction = "Microsoft Symbol Server not supported";
                            td.Content =
                                "Process Hacker is not configured correctly to obtain debugging symbols. " +
                                "If you do not require proper symbol support, you may ignore this warning.";
                            td.ExpandedByDefault = false;
                            td.ExpandedControlText = "More information";
                            td.ExpandedInformation =
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
                                Settings.Instance.DbgHelpWarningShown = verificationChecked;
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

        public static void LoadKernelModules(this SymbolProvider symbols)
        {
            // hack for drivers, whose sizes never load properly because of dbghelp.dll's dumb guessing
            symbols.PreloadModules = true;

            // load driver symbols
            foreach (var module in Windows.GetKernelModules())
            {
                try
                {
                    symbols.LoadModule(module.FileName, module.BaseAddress);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }
        }

        public static void LoadProcessModules(this SymbolProvider symbols, ProcessHandle phandle)
        {
            foreach (var module in phandle.GetModules())
            {
                try
                {
                    symbols.LoadModule(module.FileName, module.BaseAddress, module.Size);
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }
        }

        public static void LoadProcessWow64Modules(this SymbolProvider symbols, int pid)
        {
            using (var buffer = new ProcessHacker.Native.Debugging.DebugBuffer())
            {
                buffer.Query(
                    pid,
                    ProcessHacker.Native.Api.RtlQueryProcessDebugFlags.Modules32 |
                    ProcessHacker.Native.Api.RtlQueryProcessDebugFlags.NonInvasive
                    );

                foreach (var module in buffer.GetModules())
                {
                    try
                    {
                        symbols.LoadModule(module.FileName, module.BaseAddress, module.Size);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }
            }
        }
    }
}
