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

using System.Collections.Generic;
using System.Drawing;
using Aga.Controls.Tree;

namespace ProcessHacker
{
    public class ProcessNode : Node
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
                _icon = _pitem.Icon.ToBitmap();
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
                    _icon = _pitem.Icon.ToBitmap();
                    _wasNoIcon = false;
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

        public string Memory
        {
            get { return Misc.GetNiceSizeName(_pitem.MemoryUsage); }
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

        public string Description
        {
            get { return _pitem.FileDescription != null ? _pitem.FileDescription : ""; }
        }

        public Bitmap Icon
        {
            get { return _icon; }
        }
    }
}
