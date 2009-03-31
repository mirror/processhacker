/*
 * Process Hacker - 
 *   job properties
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class JobProperties : UserControl
    {
        private Win32.JobHandle _jobObject;

        public JobProperties(Win32.JobHandle jobObject)
        {
            InitializeComponent();

            _jobObject = jobObject;

            try
            {
                string name = _jobObject.GetHandleName();

                if (string.IsNullOrEmpty(name))
                    textJobName.Text = "Unnamed";
                else
                    textJobName.Text = name;
            }
            catch
            { }

            try
            {
                foreach (int pid in _jobObject.GetProcessIdList())
                {
                    ListViewItem item = new ListViewItem();

                    if (Program.HackerWindow.ProcessProvider.Dictionary.ContainsKey(pid))
                        item.Text = Program.HackerWindow.ProcessProvider.Dictionary[pid].Name;
                    else
                        item.Text = "(unknown)";

                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, pid.ToString()));

                    listProcesses.Items.Add(item);
                }
            }
            catch
            { }

            try
            {
                var extendedLimits = _jobObject.GetExtendedLimitInformatin();
                var uiRestrictions = _jobObject.GetBasicUiRestrictions();
                var flags = extendedLimits.BasicLimitInformation.LimitFlags;

                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_ACTIVE_PROCESS) != 0)
                    this.AddLimit("Active Processes", extendedLimits.BasicLimitInformation.ActiveProcessLimit.ToString());
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_AFFINITY) != 0)
                    this.AddLimit("Affinity", extendedLimits.BasicLimitInformation.Affinity.ToString("x"));
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_BREAKAWAY_OK) != 0)
                    this.AddLimit("Breakaway OK", "");
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION) != 0)
                    this.AddLimit("Die on Unhandled Exception", "");
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_JOB_MEMORY) != 0)
                    this.AddLimit("Job Memory", Misc.GetNiceSizeName(extendedLimits.JobMemoryLimit));
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_JOB_TIME) != 0)
                    this.AddLimit("Job Time",
                        Misc.GetNiceTimeSpan(new TimeSpan(extendedLimits.BasicLimitInformation.PerJobUserTimeLimit)));
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE) != 0)
                    this.AddLimit("Kill on Job Close", "");
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_PRIORITY_CLASS) != 0)
                    this.AddLimit("Priority Class",
                        ((System.Diagnostics.ProcessPriorityClass)extendedLimits.BasicLimitInformation.PriorityClass).ToString());
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_PROCESS_MEMORY) != 0)
                    this.AddLimit("Process Memory", Misc.GetNiceSizeName(extendedLimits.ProcessMemoryLimit));
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_PROCESS_TIME) != 0)
                    this.AddLimit("Process Time",
                        Misc.GetNiceTimeSpan(new TimeSpan(extendedLimits.BasicLimitInformation.PerProcessUserTimeLimit)));
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_SCHEDULING_CLASS) != 0)
                    this.AddLimit("Scheduling Class", extendedLimits.BasicLimitInformation.SchedulingClass.ToString());
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK) != 0)
                    this.AddLimit("Silent Breakaway OK", "");
                if ((flags & Win32.JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_WORKINGSET) != 0)
                {
                    this.AddLimit("Minimum Working Set", Misc.GetNiceSizeName(extendedLimits.BasicLimitInformation.MinimumWorkingSetSize));
                    this.AddLimit("Maximum Working Set", Misc.GetNiceSizeName(extendedLimits.BasicLimitInformation.MaximumWorkingSetSize));
                }

                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_DESKTOP) != 0)
                    this.AddLimit("Desktop", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_DISPLAYSETTINGS) != 0)
                    this.AddLimit("Display Settings", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_EXITWINDOWS) != 0)
                    this.AddLimit("Exit Windows", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_GLOBALATOMS) != 0)
                    this.AddLimit("Global Atoms", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_HANDLES) != 0)
                    this.AddLimit("Handles", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_READCLIPBOARD) != 0)
                    this.AddLimit("Read Clipboard", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_SYSTEMPARAMETERS) != 0)
                    this.AddLimit("System Parameters", "Limited");
                if ((uiRestrictions & Win32.JOB_OBJECT_BASIC_UI_RESTRICTIONS.JOB_OBJECT_UILIMIT_WRITECLIPBOARD) != 0)
                    this.AddLimit("Write Clipboard", "Limited");
            }
            catch 
            { }
        }

        private void AddLimit(string name, string value)
        {
            listLimits.Items.Add(new ListViewItem(new string[] { name, value }));
        }

        public Win32.JobHandle JobObject
        {
            get { return _jobObject; }
        }

        public void SaveSettings()
        {
            
        }
    }
}
