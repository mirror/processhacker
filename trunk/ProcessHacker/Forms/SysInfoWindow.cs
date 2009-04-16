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
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Components;

namespace ProcessHacker
{
    public partial class SysInfoWindow : Form
    {
        private Components.Plotter[] _cpuPlotters;
        private uint _noOfCPUs = Program.ProcessProvider.System.NumberOfProcessors;
        private uint _pages = (uint)Program.ProcessProvider.System.NumberOfPhysicalPages;
        private uint _pageSize = (uint)Program.ProcessProvider.System.PageSize;

        public SysInfoWindow()
        {
            InitializeComponent();

            this.Location = Properties.Settings.Default.SysInfoWindowLocation;
            this.Size = Properties.Settings.Default.SysInfoWindowSize;

            plotterCPU.Data1 = Program.ProcessProvider.FloatHistory["Kernel"];
            plotterCPU.Data2 = Program.ProcessProvider.FloatHistory["User"];
            plotterCPU.GetToolTip = i => 
                Program.ProcessProvider.MostCpuHistory[i] + "\n" + 
                ((plotterCPU.Data1[i] + plotterCPU.Data2[i]) * 100).ToString("N2") + 
                "% (K " + (plotterCPU.Data1[i] * 100).ToString("N2") + 
                "%, U " + (plotterCPU.Data2[i] * 100).ToString("N2") + "%)" + "\n" + 
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterIO.LongData1 = Program.ProcessProvider.LongHistory[SystemStats.IoReadOther];
            plotterIO.LongData2 = Program.ProcessProvider.LongHistory[SystemStats.IoWrite];
            plotterIO.GetToolTip = i =>
                Program.ProcessProvider.MostIoHistory[i] + "\n" +
                "R+O: " + Misc.GetNiceSizeName(plotterIO.LongData1[i]) + "\n" +
                "W: " + Misc.GetNiceSizeName(plotterIO.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterMemory.LongData1 = Program.ProcessProvider.LongHistory[SystemStats.Commit];
            plotterMemory.LongData2 = Program.ProcessProvider.LongHistory[SystemStats.PhysicalMemory];
            plotterMemory.GetToolTip = i =>
                "Commit: " + Misc.GetNiceSizeName(plotterMemory.LongData1[i]) + "\n" +
                "Phys. Memory: " + Misc.GetNiceSizeName(plotterMemory.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();

            // create a plotter per CPU
            _cpuPlotters = new Plotter[_noOfCPUs];
            tableCPUs.ColumnCount = (int)_noOfCPUs;
            tableCPUs.ColumnStyles.Clear();
            tableCPUs.Dock = DockStyle.Fill;

            for (int i = 0; i < _cpuPlotters.Length; i++)
            {
                Plotter plotter;

                tableCPUs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f / _noOfCPUs));
                _cpuPlotters[i] = plotter = new ProcessHacker.Components.Plotter();
                plotter.BackColor = Color.Black;
                plotter.Dock = DockStyle.Fill;
                plotter.Margin = new Padding(i == 0 ? 0 : 3, 0, 0, 0);
                plotter.UseSecondLine = true;
                plotter.Data1 = Program.ProcessProvider.FloatHistory[i.ToString() + " Kernel"];
                plotter.Data2 = Program.ProcessProvider.FloatHistory[i.ToString() + " User"];
                plotter.GetToolTip = j =>
                    Program.ProcessProvider.MostCpuHistory[j] + "\n" +
                    ((plotter.Data1[j] + plotter.Data2[j]) * 100).ToString("N2") +
                    "% (K " + (plotter.Data1[j] * 100).ToString("N2") +
                    "%, U " + (plotter.Data2[j] * 100).ToString("N2") + "%)" + "\n" +
                    Program.ProcessProvider.TimeHistory[j].ToString();
                tableCPUs.Controls.Add(plotter, i, 0);
            }

            tableCPUs.Visible = true;
            tableCPUs.Visible = false;
            checkShowOneGraphPerCPU.Checked = Properties.Settings.Default.ShowOneGraphPerCPU;

            if (_noOfCPUs == 1)
                checkShowOneGraphPerCPU.Enabled = false;

            this.UpdateGraphs();
            this.UpdateInfo();

            Program.ProcessProvider.Updated +=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
        }

        private void SysInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.SysInfoWindowLocation = this.Location;
                Properties.Settings.Default.SysInfoWindowSize = this.Size;
            }
            
            Program.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
            Properties.Settings.Default.ShowOneGraphPerCPU = checkShowOneGraphPerCPU.Checked;   
        }

        private void UpdateGraphs()
        {
            plotterCPU.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            plotterCPU.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
            plotterIO.LineColor1 = Properties.Settings.Default.PlotterIOROColor;
            plotterIO.LineColor2 = Properties.Settings.Default.PlotterIOWColor;
            plotterMemory.LineColor1 = Properties.Settings.Default.PlotterMemoryPrivateColor;
            plotterMemory.LineColor2 = Properties.Settings.Default.PlotterMemoryWSColor;

            for (int i = 0; i < _cpuPlotters.Length; i++)
            {
                _cpuPlotters[i].LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
                _cpuPlotters[i].LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
                _cpuPlotters[i].Text = ((_cpuPlotters[i].Data1[0] + _cpuPlotters[i].Data2[0]) * 100).ToString("F2") +
                    "% (K: " + (_cpuPlotters[i].Data1[0] * 100).ToString("F2") +
                    "%, U: " + (_cpuPlotters[i].Data2[0] * 100).ToString("F2") + "%)";
                _cpuPlotters[i].MoveGrid();
                _cpuPlotters[i].Draw();
            }

            plotterCPU.Text = ((plotterCPU.Data1[0] + plotterCPU.Data2[0]) * 100).ToString("F2") +
                "% (K: " + (plotterCPU.Data1[0] * 100).ToString("F2") +
                "%, U: " + (plotterCPU.Data2[0] * 100).ToString("F2") + "%)";

            // update the I/O graph text
            plotterIO.Text = "R+O: " + Misc.GetNiceSizeName(plotterIO.LongData1[0]) +
                ", W: " + Misc.GetNiceSizeName(plotterIO.LongData2[0]);

            // update the memory graph text
            plotterMemory.Text = "Commit: " + Misc.GetNiceSizeName(plotterMemory.LongData1[0]) +
                ", Phys. Mem: " + Misc.GetNiceSizeName(plotterMemory.LongData2[0]);

            plotterCPU.MoveGrid();
            plotterCPU.Draw();
            plotterIO.MoveGrid();
            plotterIO.Draw();
            plotterMemory.MoveGrid();
            plotterMemory.Draw();
        }

        private void UpdateInfo()
        {
            Win32.SYSTEM_PERFORMANCE_INFORMATION perfInfo = Program.ProcessProvider.Performance;
            Win32.PERFORMANCE_INFORMATION info = new Win32.PERFORMANCE_INFORMATION();

            Win32.GetPerformanceInfo(ref info, System.Runtime.InteropServices.Marshal.SizeOf(info));

            Win32.SYSTEM_CACHE_INFORMATION cacheInfo = new Win32.SYSTEM_CACHE_INFORMATION();
            int retLen;

            Win32.ZwQuerySystemInformation(Win32.SYSTEM_INFORMATION_CLASS.SystemFileCacheInformation,
                ref cacheInfo, System.Runtime.InteropServices.Marshal.SizeOf(cacheInfo), out retLen);

            labelTotalsProcesses.Text = ((ulong)info.ProcessCount).ToString("N0");
            labelTotalsThreads.Text = ((ulong)info.ThreadCount).ToString("N0");
            labelTotalsHandles.Text = ((ulong)info.HandlesCount).ToString("N0");

            labelCCC.Text = Misc.GetNiceSizeName((ulong)perfInfo.CommittedPages * _pageSize);
            labelCCP.Text = Misc.GetNiceSizeName((ulong)perfInfo.PeakCommitment * _pageSize);
            labelCCL.Text = Misc.GetNiceSizeName((ulong)perfInfo.CommitLimit * _pageSize);

            labelPMC.Text = Misc.GetNiceSizeName((ulong)(_pages - perfInfo.AvailablePages) * _pageSize);
            labelPSC.Text = Misc.GetNiceSizeName((ulong)info.SystemCache * _pageSize);
            labelPMT.Text = Misc.GetNiceSizeName((ulong)_pages * _pageSize);

            labelCacheCurrent.Text = Misc.GetNiceSizeName(cacheInfo.SystemCacheWsSize);
            labelCachePeak.Text = Misc.GetNiceSizeName(cacheInfo.SystemCacheWsPeakSize);
            labelCacheMinimum.Text = Misc.GetNiceSizeName((ulong)cacheInfo.SystemCacheWsMinimum * _pageSize);
            labelCacheMaximum.Text = Misc.GetNiceSizeName((ulong)cacheInfo.SystemCacheWsMaximum * _pageSize);

            labelKPPPU.Text = Misc.GetNiceSizeName((ulong)perfInfo.PagedPoolPages * _pageSize);
            labelKPPVU.Text = Misc.GetNiceSizeName((ulong)perfInfo.PagedPoolUsage * _pageSize);
            labelKPPA.Text = ((ulong)perfInfo.PagedPoolAllocs).ToString("N0");
            labelKPPF.Text = ((ulong)perfInfo.PagedPoolFrees).ToString("N0");
            labelKPNPU.Text = Misc.GetNiceSizeName((ulong)perfInfo.NonPagedPoolUsage * _pageSize);
            labelKPNPA.Text = ((ulong)perfInfo.NonPagedPoolAllocs).ToString("N0");
            labelKPNPF.Text = ((ulong)perfInfo.NonPagedPoolFrees).ToString("N0");

            labelPFTotal.Text = ((ulong)perfInfo.PageFaults).ToString("N0");
            labelPFCOW.Text = ((ulong)perfInfo.CopyOnWriteFaults).ToString("N0");
            labelPFTrans.Text = ((ulong)perfInfo.TransitionFaults).ToString("N0");
            labelPFCacheTrans.Text = ((ulong)perfInfo.CacheTransitionFaults).ToString("N0");
            labelPFDZ.Text = ((ulong)perfInfo.CacheTransitionFaults).ToString("N0");
            labelPFCache.Text = ((ulong)cacheInfo.SystemCacheWsFaults).ToString("N0");

            labelIOR.Text = ((ulong)perfInfo.IoReadOperationCount).ToString("N0");
            labelIORB.Text = Misc.GetNiceSizeName(perfInfo.IoReadTransferCount);
            labelIOW.Text = ((ulong)perfInfo.IoWriteOperationCount).ToString("N0");
            labelIOWB.Text = Misc.GetNiceSizeName(perfInfo.IoWriteTransferCount);
            labelIOO.Text = ((ulong)perfInfo.IoOtherOperationCount).ToString("N0");
            labelIOOB.Text = Misc.GetNiceSizeName(perfInfo.IoOtherTransferCount);

            labelCPUContextSwitches.Text = ((ulong)perfInfo.ContextSwitches).ToString("N0");
            labelCPUInterrupts.Text = ((ulong)Program.ProcessProvider.ProcessorPerf.InterruptCount).ToString("N0");
            labelCPUSystemCalls.Text = ((ulong)perfInfo.SystemCalls).ToString("N0");
        }

        private void ProcessProvider_Updated()
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
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

        private void checkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkAlwaysOnTop.Checked;
        }
    }
}
