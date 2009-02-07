/*
 * Process Hacker - 
 *   system information window
 * 
 * Copyright (C) 2008-2009 wj32
 * Copyright (C) 2008 Dean
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class SysInfoWindow : Form
    {
        private Components.Plotter[] _cpuPlotters;
        private int _noOfCPUs = Program.HackerWindow.ProcessProvider.System.NumberOfProcessors;
        private int _pages = Program.HackerWindow.ProcessProvider.System.NumberOfPhysicalPages;
        private int _pageSize = Program.HackerWindow.ProcessProvider.System.PageSize;
        private DeltaManager<string, long> _deltaManager = new DeltaManager<string, long>(new Int64Subtractor());
        private int _runCount = 0;

        public SysInfoWindow()
        {
            InitializeComponent();

            // intialize delta manager        
            _deltaManager.Add("clock", (long)Win32.GetTickCount() * 10000);
            _deltaManager.Add("cpu_kernel", 0);
            _deltaManager.Add("cpu_user", 0);
            _deltaManager.Add("cpu_other", 0);
            _deltaManager.Add("contextswitches", 0);
            _deltaManager.Add("io_r", 0);
            _deltaManager.Add("io_w", 0);
            _deltaManager.Add("io_o", 0);

            // create a plotter per CPU
            _cpuPlotters = new ProcessHacker.Components.Plotter[_noOfCPUs];
            tableCPUs.ColumnCount = _noOfCPUs;
            tableCPUs.ColumnStyles.Clear();
            tableCPUs.Dock = DockStyle.Fill;

            for (int i = 0; i < _cpuPlotters.Length; i++)
            {
                tableCPUs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f / _noOfCPUs));
                _cpuPlotters[i] = new ProcessHacker.Components.Plotter();
                _cpuPlotters[i].BackColor = Color.Black;
                _cpuPlotters[i].Dock = DockStyle.Fill;
                _cpuPlotters[i].UseSecondLine = true;
                tableCPUs.Controls.Add(_cpuPlotters[i], i, 0);

                _deltaManager.Add("cpu_" + i.ToString() + "_kernel", 0);
                _deltaManager.Add("cpu_" + i.ToString() + "_user", 0);
                _deltaManager.Add("cpu_" + i.ToString() + "_other", 0);
            }

            tableCPUs.Visible = true;
            tableCPUs.Visible = false;
            checkShowOneGraphPerCPU.Checked = Properties.Settings.Default.ShowOneGraphPerCPU;

            if (_noOfCPUs == 1)
                checkShowOneGraphPerCPU.Enabled = false;
        }

        public bool Started { get; private set; }

        public void Start()
        {
            if (this.Started)
                throw new Exception("Already started");

            this.Started = true;
            _runCount = 0;
            Program.HackerWindow.ProcessProvider.Updated +=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
        }

        public void Stop()
        {
            if (!this.Started)
                throw new Exception("Not started");

            Program.HackerWindow.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
            this.Started = false;
        }

        private void SysInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Stop();
            Properties.Settings.Default.ShowOneGraphPerCPU = checkShowOneGraphPerCPU.Checked;
            e.Cancel = true;
            this.Hide();        
        }

        private void UpdateDeltas()
        {
            Win32.SYSTEM_PERFORMANCE_INFORMATION perfInfo = Program.HackerWindow.ProcessProvider.Performance;

            _deltaManager.Update("cpu_kernel", Program.HackerWindow.ProcessProvider.ProcessorPerf.KernelTime);
            _deltaManager.Update("cpu_user", Program.HackerWindow.ProcessProvider.ProcessorPerf.UserTime);
            _deltaManager.Update("cpu_other", Program.HackerWindow.ProcessProvider.ProcessorPerf.IdleTime +
                Program.HackerWindow.ProcessProvider.ProcessorPerf.DpcTime +
                Program.HackerWindow.ProcessProvider.ProcessorPerf.InterruptTime);
            _deltaManager.Update("contextswitches", perfInfo.ContextSwitches);
            _deltaManager.Update("io_r", perfInfo.IoReadTransferCount);
            _deltaManager.Update("io_w", perfInfo.IoWriteTransferCount);
            _deltaManager.Update("io_o", perfInfo.IoOtherTransferCount);
        }

        private void UpdateGraphs()
        {
            Win32.SYSTEM_PERFORMANCE_INFORMATION perfInfo = Program.HackerWindow.ProcessProvider.Performance;

            plotterCPU.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            plotterCPU.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
            plotterIO.LineColor1 = Properties.Settings.Default.PlotterIOROColor;
            plotterIO.LineColor2 = Properties.Settings.Default.PlotterIOWColor;
            plotterMemory.LineColor1 = Properties.Settings.Default.PlotterMemoryPrivateColor;
            plotterMemory.LineColor2 = Properties.Settings.Default.PlotterMemoryWSColor;

            // update the CPU graph
            long sysKernel = _deltaManager.GetDelta("cpu_kernel");
            long sysUser = _deltaManager.GetDelta("cpu_user");
            long sysOther = _deltaManager.GetDelta("cpu_other");
            float kernelUsage = (float)sysKernel / (sysKernel + sysUser + sysOther);
            float userUsage = (float)sysUser / (sysKernel + sysUser + sysOther);

            plotterCPU.Add(kernelUsage, userUsage);
            plotterCPU.Text = ((kernelUsage + userUsage) * 100).ToString("F2") +
                "% (K: " + (kernelUsage * 100).ToString("F2") +
                "%, U: " + (userUsage * 100).ToString("F2") + "%)";

            // update the individual CPU graphs
            for (int i = 0; i < _noOfCPUs; i++)
            {
                _cpuPlotters[i].LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
                _cpuPlotters[i].LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;

                _deltaManager.Update("cpu_" + i.ToString() + "_kernel",
                    Program.HackerWindow.ProcessProvider.ProcessorPerfArray[i].KernelTime);
                _deltaManager.Update("cpu_" + i.ToString() + "_user",
                    Program.HackerWindow.ProcessProvider.ProcessorPerfArray[i].UserTime);
                _deltaManager.Update("cpu_" + i.ToString() + "_other",
                    Program.HackerWindow.ProcessProvider.ProcessorPerfArray[i].IdleTime + 
                    Program.HackerWindow.ProcessProvider.ProcessorPerfArray[i].DpcTime +
                    Program.HackerWindow.ProcessProvider.ProcessorPerfArray[i].InterruptTime);

                long cpuKernel = _deltaManager.GetDelta("cpu_" + i.ToString() + "_kernel");
                long cpuUser = _deltaManager.GetDelta("cpu_" + i.ToString() + "_user");
                long cpuOther = _deltaManager.GetDelta("cpu_" + i.ToString() + "_other");
                float cpuKernelUsage = (float)cpuKernel / (cpuKernel + cpuUser + cpuOther);
                float cpuUserUsage = (float)cpuUser / (cpuKernel + cpuUser + cpuOther);

                _cpuPlotters[i].Add(cpuKernelUsage, cpuUserUsage);
                _cpuPlotters[i].Text = ((cpuKernelUsage + cpuUserUsage) * 100).ToString("F2") +
                    "% (K: " + (cpuKernelUsage * 100).ToString("F2") +
                    "%, U: " + (cpuUserUsage * 100).ToString("F2") + "%)";
            }

            // this hack is needed for some reason
            if (_runCount < 4)
            {
                _deltaManager.SetDelta("io_r", 0);
                _deltaManager.SetDelta("io_w", 0);
                _deltaManager.SetDelta("io_o", 0);
            }

            // update the I/O graph
            long ioR = _deltaManager.GetDelta("io_r");
            long ioW = _deltaManager.GetDelta("io_w");
            long ioO = _deltaManager.GetDelta("io_o");
            plotterIO.Add(ioR + ioO, ioW);
            plotterIO.Text = "R+O: " + Misc.GetNiceSizeName(ioR + ioO) +
                ", W: " + Misc.GetNiceSizeName(ioW);

            // update the memory graph
            long commitSize = perfInfo.CommittedPages * _pageSize;
            long physMemoryUsage = (_pages - perfInfo.AvailablePages) * _pageSize;
            plotterMemory.Add(commitSize, physMemoryUsage);
            plotterMemory.Text = "Commit: " + Misc.GetNiceSizeName(commitSize) + 
                ", Phys. Mem: " + Misc.GetNiceSizeName(physMemoryUsage);

            _runCount++;
        }

        private void UpdateInfo()
        {
            Win32.SYSTEM_PERFORMANCE_INFORMATION perfInfo = Program.HackerWindow.ProcessProvider.Performance;
            Win32.PERFORMANCE_INFORMATION info = new Win32.PERFORMANCE_INFORMATION();

            Win32.GetPerformanceInfo(ref info, System.Runtime.InteropServices.Marshal.SizeOf(info));

            Win32.SYSTEM_CACHE_INFORMATION cacheInfo = new Win32.SYSTEM_CACHE_INFORMATION();
            int retLen;

            Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemFileCacheInformation,
                ref cacheInfo, System.Runtime.InteropServices.Marshal.SizeOf(cacheInfo), out retLen);

            labelTotalsProcesses.Text = info.ProcessCount.ToString("N0");
            labelTotalsThreads.Text = info.ThreadCount.ToString("N0");
            labelTotalsHandles.Text = info.HandlesCount.ToString("N0");

            labelCCC.Text = Misc.GetNiceSizeName(perfInfo.CommittedPages * _pageSize);
            labelCCP.Text = Misc.GetNiceSizeName(perfInfo.PeakCommitment * _pageSize);
            labelCCL.Text = Misc.GetNiceSizeName(perfInfo.CommitLimit * _pageSize);

            labelPMC.Text = Misc.GetNiceSizeName((_pages - perfInfo.AvailablePages) * _pageSize);
            labelPMT.Text = Misc.GetNiceSizeName(_pages * _pageSize);

            labelCacheCurrent.Text = Misc.GetNiceSizeName(cacheInfo.SystemCacheWsSize);
            labelCachePeak.Text = Misc.GetNiceSizeName(cacheInfo.SystemCacheWsPeakSize);
            labelCacheMinimum.Text = Misc.GetNiceSizeName(cacheInfo.SystemCacheWsMinimum * _pageSize);
            labelCacheMaximum.Text = Misc.GetNiceSizeName(cacheInfo.SystemCacheWsMaximum * _pageSize);

            labelKPPPU.Text = Misc.GetNiceSizeName(perfInfo.PagedPoolPages * _pageSize);
            labelKPPVU.Text = Misc.GetNiceSizeName(perfInfo.PagedPoolUsage * _pageSize);
            labelKPPA.Text = perfInfo.PagedPoolAllocs.ToString("N0");
            labelKPPF.Text = perfInfo.PagedPoolFrees.ToString("N0");
            labelKPNPU.Text = Misc.GetNiceSizeName(perfInfo.NonPagedPoolUsage * _pageSize);
            labelKPNPA.Text = perfInfo.NonPagedPoolAllocs.ToString("N0");
            labelKPNPF.Text = perfInfo.NonPagedPoolFrees.ToString("N0");

            labelPFTotal.Text = perfInfo.PageFaults.ToString("N0");
            labelPFCOW.Text = perfInfo.CopyOnWriteFaults.ToString("N0");
            labelPFTrans.Text = perfInfo.TransitionFaults.ToString("N0");
            labelPFCacheTrans.Text = perfInfo.CacheTransitionFaults.ToString("N0");
            labelPFDZ.Text = perfInfo.CacheTransitionFaults.ToString("N0");
            labelPFCache.Text = cacheInfo.SystemCacheWsFaults.ToString("N0");

            labelIOR.Text = perfInfo.IoReadOperationCount.ToString("N0");
            labelIORB.Text = Misc.GetNiceSizeName(perfInfo.IoReadTransferCount);
            labelIOW.Text = perfInfo.IoWriteOperationCount.ToString("N0");
            labelIOWB.Text = Misc.GetNiceSizeName(perfInfo.IoWriteTransferCount);
            labelIOO.Text = perfInfo.IoOtherOperationCount.ToString("N0");
            labelIOOB.Text = Misc.GetNiceSizeName(perfInfo.IoOtherTransferCount);

            labelCPUContextSwitches.Text = perfInfo.ContextSwitches.ToString("N0");
            labelCPUInterrupts.Text = Program.HackerWindow.ProcessProvider.ProcessorPerf.InterruptCount.ToString("N0");
            labelCPUSystemCalls.Text = perfInfo.SystemCalls.ToString("N0");
        }

        private void ProcessProvider_Updated()
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.UpdateDeltas();
                this.UpdateGraphs();
                this.UpdateInfo();
            }));
        }

        private void checkShowOneGraphPerCPU_CheckedChanged(object sender, EventArgs e)
        {
            if (checkShowOneGraphPerCPU.Checked)
            {
                tableCPUs.Visible = true;
            }
            else
            {
                tableCPUs.Visible = false;
            }
        }
    }
}
