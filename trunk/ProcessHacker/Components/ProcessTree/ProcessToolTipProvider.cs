/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Diagnostics;
using Aga.Controls.Tree;

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
                        (Misc.MakeEllipsis(pNode.ProcessItem.CmdLine, 100) + "\n\n") : "");

                string fileText = "";

                try
                {
                    string filename = "";

                    if (pNode.PID == 4)
                    {
                        filename = Misc.GetKernelFileName();
                    }
                    else
                    {
                        filename = pNode.ProcessItem.Process.MainModule.FileName;
                    }

                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(
                        Misc.GetRealPath(filename));

                    fileText = info.FileName + "\n" +
                        info.FileDescription + " (" + info.FileVersion + ")\n" +
                        info.CompanyName;
                }
                catch
                { }

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
                                    servicesText += service + " (" +
                                    Program.HackerWindow.ServiceProvider.Dictionary[service].Status.DisplayName + ")\n";
                                else
                                    servicesText += service + "\n";
                            }
                            else
                            {
                                servicesText += service + "\n";
                            }
                        }

                        servicesText = "\n\nServices:\n" + servicesText.TrimEnd('\n');
                    }
                }
                catch
                { }

                return (cmdText + fileText + servicesText).Trim(' ', '\n', '\r');
            }
            catch
            { }

            return string.Empty;
        }
    }
}
