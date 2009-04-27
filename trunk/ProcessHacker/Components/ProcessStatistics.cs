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
using ProcessHacker.Native;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Components
{
    public partial class ProcessStatistics : UserControl
    {
        private int _pid;

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
            labelCPUPriority.Text = "";
            labelCPUCycles.Text = "";
            labelCPUKernelTime.Text = "";
            labelCPUUserTime.Text = "";
            labelCPUTotalTime.Text = "";

            labelMemoryPB.Text = "";
            labelMemoryWS.Text = "";
            labelMemoryPWS.Text = "";
            labelMemoryVS.Text = "";
            labelMemoryPVS.Text = "";
            labelMemoryPU.Text = "";
            labelMemoryPPU.Text = "";
            labelMemoryPF.Text = "";
            labelMemoryPP.Text = "";

            labelIOReads.Text = "";
            labelIOReadBytes.Text = "";
            labelIOWrites.Text = "";
            labelIOWriteBytes.Text = "";
            labelIOOther.Text = "";
            labelIOOtherBytes.Text = "";
            labelIOPriority.Text = "";

            labelOtherHandles.Text = "";
            labelOtherGDIHandles.Text = "";
            labelOtherUSERHandles.Text = "";
        }

        public void UpdateStatistics()
        {
            if (!Program.ProcessProvider.Dictionary.ContainsKey(_pid))
                return;

            ProcessItem item = Program.ProcessProvider.Dictionary[_pid];

            labelCPUPriority.Text = item.Process.BasePriority.ToString();
            labelCPUKernelTime.Text = Misc.GetNiceTimeSpan(new TimeSpan(item.Process.KernelTime));
            labelCPUUserTime.Text = Misc.GetNiceTimeSpan(new TimeSpan(item.Process.UserTime));
            labelCPUTotalTime.Text = Misc.GetNiceTimeSpan(new TimeSpan(item.Process.KernelTime + item.Process.UserTime));

            labelMemoryPB.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PrivateBytes);
            labelMemoryWS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.WorkingSetSize);
            labelMemoryPWS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PeakWorkingSetSize);
            labelMemoryVS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.VirtualSize);
            labelMemoryPVS.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PeakVirtualSize);
            labelMemoryPU.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PagefileUsage);
            labelMemoryPPU.Text = Misc.GetNiceSizeName(item.Process.VirtualMemoryCounters.PeakPagefileUsage);
            labelMemoryPF.Text = ((ulong)item.Process.VirtualMemoryCounters.PageFaultCount).ToString("N0");

            labelIOReads.Text = ((ulong)item.Process.IoCounters.ReadOperationCount).ToString("N0");
            labelIOReadBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.ReadTransferCount);
            labelIOWrites.Text = ((ulong)item.Process.IoCounters.WriteOperationCount).ToString("N0");
            labelIOWriteBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.WriteTransferCount);
            labelIOOther.Text = ((ulong)item.Process.IoCounters.OtherOperationCount).ToString("N0");
            labelIOOtherBytes.Text = Misc.GetNiceSizeName(item.Process.IoCounters.OtherTransferCount);

            labelOtherHandles.Text = ((ulong)item.Process.HandleCount).ToString("N0");

            try
            {
                using (var phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                {
                    labelOtherGDIHandles.Text = phandle.GetGuiResources(false).ToString("N0");
                    labelOtherUSERHandles.Text = phandle.GetGuiResources(true).ToString("N0");

                    if (OSVersion.HasCycleTime)
                        labelCPUCycles.Text = phandle.GetCycleTime().ToString("N0");
                    else
                        labelCPUCycles.Text = "N/A";

                    if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
                    {
                        labelMemoryPP.Text = phandle.GetPagePriority().ToString();
                        labelIOPriority.Text = phandle.GetIoPriority().ToString();
                    }
                }
            }
            catch
            { }
        }
    }
}
