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

using System.Diagnostics;
using Aga.Controls.Tree;
using System;

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
                        (Misc.MakeEllipsis(pNode.ProcessItem.CmdLine.Replace("\0", ""), 100) + "\n") : "");

                string fileText = "";

                try
                {
                    string filename = "";
                                                                                          
                    if (pNode.PID == 4)
                    {
                        filename = Misc.GetRealPath(Misc.GetKernelFileName());
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
                    pNode.ProcessItem.FileName.ToLower() == (Environment.SystemDirectory + "\\rundll32.exe").ToLower() && 
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
                    catch
                    { }
                }

                string servicesText = "";

                try
                {
                    if (Program.HackerWindow.ProcessServices.ContainsKey(pNode.PID))
                    {
                        foreach (string service in Program.HackerWindow.ProcessServices[pNode.PID])
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
                catch
                { }

                string otherNotes = "";

                try
                {
                    if (pNode.ProcessItem.IsPacked && pNode.ProcessItem.ImportModules > 0)
                        otherNotes += "\n    Image is probably packed - has " +
                            pNode.ProcessItem.ImportFunctions.ToString() + " imports over " +
                            pNode.ProcessItem.ImportModules.ToString() + " modules.";
                    else if (pNode.ProcessItem.IsPacked)
                        otherNotes += "\n    Image is probably packed - error reading PE file.";

                    if (Properties.Settings.Default.VerifySignatures && pNode.ProcessItem.FileName != null)
                    {
                        if (pNode.ProcessItem.VerifyResult == Win32.VerifyResult.Trusted)
                            otherNotes += "\n    Signature present and verified.";
                        else if (pNode.ProcessItem.VerifyResult == Win32.VerifyResult.TrustedInstaller)
                            otherNotes += "\n    Verified Windows component.";
                        else if (pNode.ProcessItem.VerifyResult == Win32.VerifyResult.Unknown)
                            otherNotes += "\n    File has not been processed yet. Please wait...";
                        else if (pNode.ProcessItem.VerifyResult != Win32.VerifyResult.NoSignature)
                            otherNotes += "\n    Signature present but invalid.";

                        if (Program.ImposterNames.Contains(pNode.Name.ToLower()) &&
                            pNode.ProcessItem.VerifyResult != Win32.VerifyResult.Trusted &&
                            pNode.ProcessItem.VerifyResult != Win32.VerifyResult.TrustedInstaller &&
                            pNode.ProcessItem.VerifyResult != Win32.VerifyResult.Unknown)
                            otherNotes += "\n    Process is using the name of a known process but its signature could not be verified.";
                    }

                    if (otherNotes != "")
                        otherNotes = "\nNotes:" + otherNotes;
                }
                catch
                { }

                return (cmdText + fileText + otherNotes + runDllText + servicesText).Trim(' ', '\n', '\r');
            }
            catch
            { }

            return string.Empty;
        }
    }
}
