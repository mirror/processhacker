/*
 * Process Hacker - 
 *   process statistics control
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

namespace ProcessHacker.Components
{
    public partial class ProcessStatistics : UserControl
    {
        private readonly int _pid;

        public ProcessStatistics(int pid)
        {
            InitializeComponent();

            _pid = pid;

            if (OSVersion.HasCycleTime)
            {
                labelCPUCyclesText.Text = "Cycles";
            }
            else
            {
                labelCPUCyclesText.Text = "N/A";
            }
        }

        public void ClearStatistics()
        {
            labelCPUPriority.Text = string.Empty;
            labelCPUCycles.Text = string.Empty;
            labelCPUKernelTime.Text = string.Empty;
            labelCPUUserTime.Text = string.Empty;
            labelCPUTotalTime.Text = string.Empty;

            labelMemoryPB.Text = string.Empty;
            labelMemoryWS.Text = string.Empty;
            labelMemoryPWS.Text = string.Empty;
            labelMemoryVS.Text = string.Empty;
            labelMemoryPVS.Text = string.Empty;
            labelMemoryPU.Text = string.Empty;
            labelMemoryPPU.Text = string.Empty;
            labelMemoryPF.Text = string.Empty;
            labelMemoryPP.Text = string.Empty;

            labelIOReads.Text = string.Empty;
            labelIOReadBytes.Text = string.Empty;
            labelIOWrites.Text = string.Empty;
            labelIOWriteBytes.Text = string.Empty;
            labelIOOther.Text = string.Empty;
            labelIOOtherBytes.Text = string.Empty;
            labelIOPriority.Text = string.Empty;

            labelOtherHandles.Text = string.Empty;
            labelOtherGDIHandles.Text = string.Empty;
            labelOtherUSERHandles.Text = string.Empty;
        }

        public void UpdateStatistics()
        {
            if (!Program.ProcessProvider.Dictionary.ContainsKey(_pid))
                return;

            ProcessItem item = Program.ProcessProvider.Dictionary[_pid];

            labelCPUPriority.Text = item.Process.BasePriority.ToString();
            labelCPUKernelTime.Text = Utils.FormatTimeSpan(new TimeSpan(item.Process.KernelTime));
            labelCPUUserTime.Text = Utils.FormatTimeSpan(new TimeSpan(item.Process.UserTime));
            labelCPUTotalTime.Text = Utils.FormatTimeSpan(new TimeSpan(item.Process.KernelTime + item.Process.UserTime));

            labelMemoryPB.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.PrivatePageCount);
            labelMemoryWS.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.WorkingSetSize);
            labelMemoryPWS.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.PeakWorkingSetSize);
            labelMemoryVS.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.VirtualSize);
            labelMemoryPVS.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.PeakVirtualSize);
            labelMemoryPU.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.PagefileUsage);
            labelMemoryPPU.Text = Utils.FormatSize(item.Process.VirtualMemoryCounters.PeakPagefileUsage);
            labelMemoryPF.Text = ((ulong)item.Process.VirtualMemoryCounters.PageFaultCount).ToString("N0");

            labelIOReads.Text = item.Process.IoCounters.ReadOperationCount.ToString("N0");
            labelIOReadBytes.Text = Utils.FormatSize(item.Process.IoCounters.ReadTransferCount);
            labelIOWrites.Text = item.Process.IoCounters.WriteOperationCount.ToString("N0");
            labelIOWriteBytes.Text = Utils.FormatSize(item.Process.IoCounters.WriteTransferCount);
            labelIOOther.Text = item.Process.IoCounters.OtherOperationCount.ToString("N0");
            labelIOOtherBytes.Text = Utils.FormatSize(item.Process.IoCounters.OtherTransferCount);

            labelOtherHandles.Text = ((ulong)item.Process.HandleCount).ToString("N0");
            
            if (_pid > 0)
            {
                try
                {
                    labelOtherGDIHandles.Text = item.ProcessQueryHandle.GetGuiResources(false).ToString("N0");
                    labelOtherUSERHandles.Text = item.ProcessQueryHandle.GetGuiResources(true).ToString("N0");

                    if (OSVersion.HasCycleTime)
                        labelCPUCycles.Text = item.ProcessQueryHandle.GetCycleTime().ToString("N0");
                    else
                        labelCPUCycles.Text = "N/A";

                    if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    {
                        labelMemoryPP.Text = item.ProcessQueryHandle.PagePriority.ToString();
                        labelIOPriority.Text = item.ProcessQueryHandle.IoPriority.ToString();
                    }
                }
                catch
                { }
            }
            else
            {
                labelOtherGDIHandles.Text = "0";
                labelOtherUSERHandles.Text = "0";
                labelCPUCycles.Text = "0";
                labelMemoryPP.Text = "0";
                labelIOPriority.Text = "0";
            }
        }

        private void buttonHandleDetails_Click(object sender, EventArgs e)
        {
            try
            {
                (new HandleStatisticsWindow(_pid)).ShowDialog();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to show handle statistics", ex);
            }
        }
    }
}
