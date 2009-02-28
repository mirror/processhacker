/*
 * Process Hacker - 
 *   Node implementation for the process tree
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
using System.Drawing;
using Aga.Controls.Tree;

namespace ProcessHacker
{
    public class ProcessNode : Node, IDisposable
    {
        private List<ProcessNode> _children = new List<ProcessNode>();
        private ProcessItem _pitem;
        private bool _wasNoIcon = false;
        private Bitmap _icon;

        public ProcessNode(ProcessItem pitem)
        {
            _pitem = pitem;
            this.Tag = pitem.PID;

            if (_pitem.Icon == null)
            {
                _wasNoIcon = true;
                _icon = global::ProcessHacker.Properties.Resources.Process_small.ToBitmap();
            }
            else
            {
                try
                {
                    _icon = _pitem.Icon.ToBitmap();
                }
                catch
                {
                    _wasNoIcon = true;
                    _icon = global::ProcessHacker.Properties.Resources.Process_small.ToBitmap();
                }
            }
        }

        ~ProcessNode()
        {
            if (_icon != null)
                this.Dispose();
        }

        public void Dispose()
        {
            if (_icon != null)
            {
                _icon.Dispose();
                _icon = null;
            }
        }

        public ProcessItem ProcessItem
        {
            get { return _pitem; }
            set
            {
                _pitem = value;

                if (_wasNoIcon && _pitem.Icon != null)
                {
                    if (_icon != null)
                        _icon.Dispose();

                    _icon = new Bitmap(16, 16);

                    try
                    {
                        using (Graphics g = Graphics.FromImage(_icon))
                            g.DrawIcon(_pitem.Icon, new Rectangle(0, 0, 16, 16));

                        _wasNoIcon = false;
                    }
                    catch
                    {
                        _icon.Dispose();
                        _icon = null;
                    }
                }
            }
        }

        public List<ProcessNode> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _pitem.Name != null ? _pitem.Name : ""; }
        }

        public string DisplayPID
        {
            get
            {
                if (_pitem.PID >= 0)
                    return _pitem.PID.ToString();
                else
                    return "";
            }
        }

        public int PID
        {
            get { return _pitem.PID; }
        }

        public int PPID
        {
            get { if (_pitem.PID == _pitem.ParentPID) return -1; else return _pitem.ParentPID; }
        }

        public string PvtMemory
        {
            get { return Misc.GetNiceSizeName(_pitem.MemoryUsage); }
        }

        public string WorkingSet
        {
            get { return Misc.GetNiceSizeName(_pitem.Process.VirtualMemoryCounters.WorkingSetSize); }
        }

        public string CPU
        {
            get
            {
                if (_pitem.CPUUsage == 0)
                    return "";
                else
                    return _pitem.CPUUsage.ToString("F2");
            }
        }

        private string GetBestUsername(string username, bool includeDomain)
        {
            if (username == null)
                return "";

            if (!username.Contains("\\"))
                return username;

            string[] split = username.Split(new char[] { '\\' }, 2);
            string domain = split[0];
            string user = split[1];

            if (includeDomain)
                return domain + "\\" + user;
            else
                return user;
        }

        public string Username
        {
            get { return this.GetBestUsername(_pitem.Username, Properties.Settings.Default.ShowAccountDomains); }
        }

        public string SessionId
        {
            get
            {
                if (PID < 4)
                    return "";
                else
                    return _pitem.SessionId.ToString();
            }
        }

        public string Description
        {
            get
            {
                if (PID == 0)
                    return "System Idle Process";
                else if (PID == -2)
                    return "Deferred Procedure Calls";
                else if (PID == -3)
                    return "Interrupts";
                else if (_pitem.VersionInfo != null)
                    return _pitem.VersionInfo.FileDescription;
                else
                    return "";
            }
        }

        public string Company
        {
            get
            {
                if (_pitem.VersionInfo != null)
                    return _pitem.VersionInfo.CompanyName;
                else
                    return "";
            }
        }

        public string FileName
        {
            get
            {
                if (_pitem.FileName == null)
                    return "";
                else
                    return _pitem.FileName;
            }
        }

        public string CommandLine
        {
            get
            {
                if (_pitem.CmdLine == null)
                    return "";
                else
                    return _pitem.CmdLine.Replace("\0", "");
            }
        }

        public Bitmap Icon
        {
            get { return _icon; }
        }
    }
}
