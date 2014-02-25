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
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class JobProperties : UserControl
    {
        private JobObjectHandle _jobObject;

        public JobProperties(JobObjectHandle jobObject)
        {
            InitializeComponent();

            listProcesses.SetTheme("explorer");
            listProcesses.AddShortcuts();
            listProcesses.ContextMenu = listProcesses.GetCopyMenu();

            listLimits.SetTheme("explorer");
            listLimits.AddShortcuts();
            listLimits.ContextMenu = listLimits.GetCopyMenu();

            _jobObject = jobObject;
            _jobObject.Reference();
            timerUpdate.Interval = Settings.Instance.RefreshInterval;
            this.UpdateStatistics();

            try
            {
                string name = _jobObject.GetObjectName();

                if (string.IsNullOrEmpty(name))
                    textJobName.Text = "(unnamed job)";
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

                    if (Program.ProcessProvider.Dictionary.ContainsKey(pid))
                        item.Text = Program.ProcessProvider.Dictionary[pid].Name;
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
                var extendedLimits = _jobObject.GetExtendedLimitInformation();
                var uiRestrictions = _jobObject.GetBasicUiRestrictions();
                var flags = extendedLimits.BasicLimitInformation.LimitFlags;

                if ((flags & JobObjectLimitFlags.ActiveProcess) != 0)
                    this.AddLimit("Active Processes", extendedLimits.BasicLimitInformation.ActiveProcessLimit.ToString());
                if ((flags & JobObjectLimitFlags.Affinity) != 0)
                    this.AddLimit("Affinity", extendedLimits.BasicLimitInformation.Affinity.ToString("x"));
                if ((flags & JobObjectLimitFlags.BreakawayOk) != 0)
                    this.AddLimit("Breakaway OK", "Enabled");
                if ((flags & JobObjectLimitFlags.DieOnUnhandledException) != 0)
                    this.AddLimit("Die on Unhandled Exception", "Enabled");
                if ((flags & JobObjectLimitFlags.JobMemory) != 0)
                    this.AddLimit("Job Memory", Utils.FormatSize(extendedLimits.JobMemoryLimit));
                if ((flags & JobObjectLimitFlags.JobTime) != 0)
                    this.AddLimit("Job Time",
                        Utils.FormatTimeSpan(new TimeSpan(extendedLimits.BasicLimitInformation.PerJobUserTimeLimit)));
                if ((flags & JobObjectLimitFlags.KillOnJobClose) != 0)
                    this.AddLimit("Kill on Job Close", "Enabled");
                if ((flags & JobObjectLimitFlags.PriorityClass) != 0)
                    this.AddLimit("Priority Class",
                        ((System.Diagnostics.ProcessPriorityClass)extendedLimits.BasicLimitInformation.PriorityClass).ToString());
                if ((flags & JobObjectLimitFlags.ProcessMemory) != 0)
                    this.AddLimit("Process Memory", Utils.FormatSize(extendedLimits.ProcessMemoryLimit));
                if ((flags & JobObjectLimitFlags.ProcessTime) != 0)
                    this.AddLimit("Process Time",
                        Utils.FormatTimeSpan(new TimeSpan(extendedLimits.BasicLimitInformation.PerProcessUserTimeLimit)));
                if ((flags & JobObjectLimitFlags.SchedulingClass) != 0)
                    this.AddLimit("Scheduling Class", extendedLimits.BasicLimitInformation.SchedulingClass.ToString());
                if ((flags & JobObjectLimitFlags.SilentBreakawayOk) != 0)
                    this.AddLimit("Silent Breakaway OK", "Enabled");
                if ((flags & JobObjectLimitFlags.WorkingSet) != 0)
                {
                    this.AddLimit("Minimum Working Set", Utils.FormatSize(extendedLimits.BasicLimitInformation.MinimumWorkingSetSize));
                    this.AddLimit("Maximum Working Set", Utils.FormatSize(extendedLimits.BasicLimitInformation.MaximumWorkingSetSize));
                }

                if ((uiRestrictions & JobObjectBasicUiRestrictions.Desktop) != 0)
                    this.AddLimit("Desktop", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.DisplaySettings) != 0)
                    this.AddLimit("Display Settings", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.ExitWindows) != 0)
                    this.AddLimit("Exit Windows", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.GlobalAtoms) != 0)
                    this.AddLimit("Global Atoms", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.Handles) != 0)
                    this.AddLimit("Handles", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.ReadClipboard) != 0)
                    this.AddLimit("Read Clipboard", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.SystemParameters) != 0)
                    this.AddLimit("System Parameters", "Limited");
                if ((uiRestrictions & JobObjectBasicUiRestrictions.WriteClipboard) != 0)
                    this.AddLimit("Write Clipboard", "Limited");
            }
            catch 
            { }
        }

        private void AddLimit(string name, string value)
        {
            listLimits.Items.Add(new ListViewItem(new string[] { name, value }));
        }

        public JobObjectHandle JobObject
        {
            get { return _jobObject; }
        }

        public bool UpdateEnabled
        {
            get { return timerUpdate.Enabled; }
            set { timerUpdate.Enabled = value; }
        }

        public void SaveSettings()
        {
            
        }

        private void UpdateStatistics()
        {
            try
            {
                var accounting = _jobObject.GetBasicAndIoAccountingInformation();
                var limits = _jobObject.GetExtendedLimitInformation();

                labelGeneralActiveProcesses.Text = accounting.BasicInfo.ActiveProcesses.ToString("N0");
                labelGeneralTotalProcesses.Text = accounting.BasicInfo.TotalProcesses.ToString("N0");
                labelGeneralTerminatedProcesses.Text = accounting.BasicInfo.TotalTerminatedProcesses.ToString("N0");

                labelTimeUserTime.Text = Utils.FormatTimeSpan(new TimeSpan(accounting.BasicInfo.TotalUserTime));
                labelTimeKernelTime.Text = Utils.FormatTimeSpan(new TimeSpan(accounting.BasicInfo.TotalKernelTime));   
                labelTimeUserTimePeriod.Text = Utils.FormatTimeSpan(new TimeSpan(accounting.BasicInfo.ThisPeriodTotalUserTime));
                labelTimeKernelTimePeriod.Text = Utils.FormatTimeSpan(new TimeSpan(accounting.BasicInfo.ThisPeriodTotalKernelTime));

                labelMemoryPageFaults.Text = accounting.BasicInfo.TotalPageFaultCount.ToString("N0");
                labelMemoryPeakProcessUsage.Text = Utils.FormatSize(limits.PeakProcessMemoryUsed);
                labelMemoryPeakJobUsage.Text = Utils.FormatSize(limits.PeakJobMemoryUsed);

                labelIOReads.Text = accounting.IoInfo.ReadOperationCount.ToString("N0");
                labelIOReadBytes.Text = Utils.FormatSize(accounting.IoInfo.ReadTransferCount);
                labelIOWrites.Text = accounting.IoInfo.WriteOperationCount.ToString("N0");
                labelIOWriteBytes.Text = Utils.FormatSize(accounting.IoInfo.WriteTransferCount);
                labelIOOther.Text = accounting.IoInfo.OtherOperationCount.ToString("N0");
                labelIOOtherBytes.Text = Utils.FormatSize(accounting.IoInfo.OtherTransferCount);
            }
            catch
            { }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            this.UpdateStatistics();
        }

        private void buttonTerminate_Click(object sender, EventArgs e)
        {
            if (OSVersion.HasTaskDialogs)
            {
                TaskDialog td = new TaskDialog();

                td.WindowTitle = "Process Hacker";
                td.MainIcon = TaskDialogIcon.Warning;
                td.MainInstruction = "Do you want to terminate the job?";
                td.Content = "Terminating a job will terminate all processes assigned to it. Are you sure " +
                    "you want to continue?";
                td.Buttons = new TaskDialogButton[] 
                {
                    new TaskDialogButton((int)DialogResult.Yes, "Terminate"),
                    new TaskDialogButton((int)DialogResult.No, "Cancel")
                };
                td.DefaultButton = (int)DialogResult.No;

                if (td.Show(this) == (int)DialogResult.No)
                    return;
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to terminate the job? This action will " +
                    "terminate all processes associated with the job.", "Process Hacker",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return;
            }

            try
            {
                using (var jhandle2 = _jobObject.Duplicate(JobObjectAccess.Terminate))
                    JobObjectHandle.FromHandle(jhandle2).Terminate();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to terminate the job", ex);
            }
        }
    }
}
