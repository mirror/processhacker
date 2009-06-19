/*
 * Process Hacker - 
 *   IToolTipProvider implementation for the process tree
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
using System.Diagnostics;
using Aga.Controls.Tree;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public class ProcessToolTipProvider : IToolTipProvider
    {
        private ProcessTree _tree;

        public ProcessToolTipProvider(ProcessTree owner)
        {
            _tree = owner;
        }

        public string GetToolTip(TreeNodeAdv node, Aga.Controls.Tree.NodeControls.NodeControl nodeControl)
        {
            try
            {
                ProcessNode pNode = _tree.FindNode(node);

                string cmdText = (pNode.ProcessItem.CmdLine != null ?
                        (Utils.MakeEllipsis(pNode.ProcessItem.CmdLine.Replace("\0", ""), 100) + "\n") : "");

                string fileText = "";

                try
                {
                    string filename = "";

                    if (pNode.Pid == 4)
                    {
                        filename = FileUtils.FixPath(Windows.KernelFileName);
                    }
                    else
                    {
                        filename = pNode.ProcessItem.FileName;
                    }

                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(filename);

                    fileText = "File:\n    " + info.FileName + "\n    " +
                        info.FileDescription + " " + info.FileVersion + "\n    " +
                        info.CompanyName;
                }
                catch
                {
                    if (pNode.ProcessItem.FileName != null)
                        fileText = "File:\n    " + pNode.ProcessItem.FileName;
                }

                string runDllText = "";

                if (pNode.ProcessItem.FileName != null &&
                    pNode.ProcessItem.FileName.Equals(Environment.SystemDirectory + "\\rundll32.exe",
                    StringComparison.InvariantCultureIgnoreCase) &&
                    pNode.ProcessItem.CmdLine != null)
                {
                    try
                    {
                        // TODO: fix crappy method
                        string targetFile = pNode.ProcessItem.CmdLine.Split(new char[] { ' ' }, 2)[1].Split(',')[0];

                        // if it doesn't specify an absolute path, assume it's in system32.
                        if (!targetFile.Contains(":"))
                            targetFile = Environment.SystemDirectory + "\\" + targetFile;

                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(targetFile);

                        runDllText = "\nRunDLL target:\n    " + info.FileName + "\n    " +
                            info.FileDescription + " " + info.FileVersion + "\n    " +
                            info.CompanyName;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }

                string dllhostText = "";

                if (pNode.ProcessItem.FileName != null &&
                    pNode.ProcessItem.FileName.Equals(Environment.SystemDirectory + "\\dllhost.exe",
                    StringComparison.InvariantCultureIgnoreCase) &&
                    pNode.ProcessItem.CmdLine != null)
                {
                    try
                    {
                        string clsid = pNode.ProcessItem.CmdLine.ToLowerInvariant().Split(
                            new string[] { "/processid:" }, StringSplitOptions.None)[1].Split(' ')[0];
                        var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid);
                        var inprocServer32 = key.OpenSubKey("InprocServer32");
                        string name = key.GetValue("") as string;
                        string fileName = inprocServer32.GetValue("") as string;

                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(Environment.ExpandEnvironmentVariables(fileName));

                        dllhostText = "\nCOM Target:\n    " + name + " (" + clsid.ToUpper() + ")\n    " +
                            info.FileName + "\n    " +
                            info.FileDescription + " " + info.FileVersion + "\n    " + info.CompanyName;
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex);
                    }
                }

                string servicesText = "";

                try
                {
                    if (Program.HackerWindow.ProcessServices.ContainsKey(pNode.Pid))
                    {
                        foreach (string service in Program.HackerWindow.ProcessServices[pNode.Pid])
                        {
                            if (Program.HackerWindow.ServiceProvider.Dictionary.ContainsKey(service))
                            {
                                if (Program.HackerWindow.ServiceProvider.Dictionary[service].Status.DisplayName != "")
                                    servicesText += "    " + service + " (" +
                                    Program.HackerWindow.ServiceProvider.Dictionary[service].Status.DisplayName + ")\n";
                                else
                                    servicesText += "    " + service + "\n";
                            }
                            else
                            {
                                servicesText += "    " + service + "\n";
                            }
                        }

                        servicesText = "\nServices:\n" + servicesText.TrimEnd('\n');
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                string otherNotes = "";

                try
                {
                    if (pNode.ProcessItem.IsPacked && pNode.ProcessItem.ImportModules > 0)
                        otherNotes += "\n    Image is probably packed - has " +
                            pNode.ProcessItem.ImportFunctions.ToString() + " imports over " +
                            pNode.ProcessItem.ImportModules.ToString() + " modules.";
                    else if (pNode.ProcessItem.IsPacked)
                        otherNotes += "\n    Image is probably packed - error reading PE file.";

                    if (pNode.ProcessItem.FileName != null)
                    {
                        if (pNode.ProcessItem.VerifyResult == VerifyResult.Trusted)
                            otherNotes += "\n    Signature present and verified.";
                        else if (pNode.ProcessItem.VerifyResult == VerifyResult.TrustedInstaller)
                            otherNotes += "\n    Verified Windows component.";
                        else if (pNode.ProcessItem.VerifyResult == VerifyResult.Unknown &&
                            !Properties.Settings.Default.VerifySignatures)
                            otherNotes += "";
                        else if (pNode.ProcessItem.VerifyResult == VerifyResult.Unknown &&
                            Properties.Settings.Default.VerifySignatures)
                            otherNotes += "\n    File has not been processed yet. Please wait...";
                        else if (pNode.ProcessItem.VerifyResult != VerifyResult.NoSignature)
                            otherNotes += "\n    Signature present but invalid.";

                        if (Program.ImposterNames.Contains(pNode.Name.ToLower()) &&
                            pNode.ProcessItem.VerifyResult != VerifyResult.Trusted &&
                            pNode.ProcessItem.VerifyResult != VerifyResult.TrustedInstaller &&
                            pNode.ProcessItem.VerifyResult != VerifyResult.Unknown)
                            otherNotes += "\n    Process is using the name of a known process but its signature could not be verified.";
                    }

                    if (pNode.ProcessItem.IsInJob)
                        otherNotes += "\n    Process is in a job.";
                    if (pNode.ProcessItem.ElevationType == TokenElevationType.Full)
                        otherNotes += "\n    Process is elevated.";
                    if (pNode.ProcessItem.IsDotNet)
                        otherNotes += "\n    Process is managed (.NET).";
                    if (pNode.ProcessItem.IsPosix)
                        otherNotes += "\n    Process is POSIX.";

                    if (otherNotes != "")
                        otherNotes = "\nNotes:" + otherNotes;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }

                return (cmdText + fileText + otherNotes + runDllText + dllhostText + servicesText).Trim(' ', '\n', '\r');
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            return string.Empty;
        }
    }
}
