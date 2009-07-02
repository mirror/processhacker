/*
 * Process Hacker - 
 *   system information window
 * 
 * Copyright (C) 2008-2009 wj32
 * Copyright (C) 2008-2009 Dean
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class SysInfoWindow : Form
    {
        private bool _isFirstPaint = true;
        private Components.Plotter[] _cpuPlotters;
        private uint _noOfCPUs = Program.ProcessProvider.System.NumberOfProcessors;
        private uint _pages = (uint)Program.ProcessProvider.System.NumberOfPhysicalPages;
        private uint _pageSize = (uint)Program.ProcessProvider.System.PageSize;

        public SysInfoWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();

            this.Size = Properties.Settings.Default.SysInfoWindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Properties.Settings.Default.SysInfoWindowLocation, this.Size), this).Location;
        }

        private void SysInfoWindow_Paint(object sender, PaintEventArgs e)
        {
            if (_isFirstPaint)
            {
                this.LoadStage1();
            }

            _isFirstPaint = false;
        }

        private void LoadStage1()
        {   
            // Maximum physical memory.
            indicatorPhysical.Maximum = (int)_pages;

            // Set up the plotter controls.
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
                "R+O: " + Utils.GetNiceSizeName(plotterIO.LongData1[i]) + "\n" +
                "W: " + Utils.GetNiceSizeName(plotterIO.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterMemory.LongData1 = Program.ProcessProvider.LongHistory[SystemStats.Commit];
            plotterMemory.LongData2 = Program.ProcessProvider.LongHistory[SystemStats.PhysicalMemory];
            plotterMemory.GetToolTip = i =>
                "Commit: " + Utils.GetNiceSizeName(plotterMemory.LongData1[i]) + "\n" +
                "Phys. Memory: " + Utils.GetNiceSizeName(plotterMemory.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();

            // Create a plotter per CPU.
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
                plotter.Margin = new Padding(i == 0 ? 0 : 3, 0, 0, 0); // nice spacing
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
            // Update the CPU indicator.
            indicatorCpu.Color1 = Properties.Settings.Default.PlotterCPUKernelColor;
            indicatorCpu.Color2 = Properties.Settings.Default.PlotterCPUUserColor;
            indicatorCpu.Data1 = (int)(Program.ProcessProvider.CurrentCpuKernelUsage * indicatorCpu.Maximum);
            indicatorCpu.Data2 = (int)(Program.ProcessProvider.CurrentCpuUserUsage * indicatorCpu.Maximum);
            indicatorCpu.TextValue = (Program.ProcessProvider.CurrentCpuUsage * 100).ToString("F2") + "%";

            // Update the I/O indicator.
            indicatorIO.Color1 = Properties.Settings.Default.PlotterIOROColor;
            long max = Program.ProcessProvider.LongHistory[SystemStats.IoReadOther].Take(
                plotterIO.Width / plotterIO.EffectiveMoveStep).Max();
            indicatorIO.Maximum = (int)max;
            indicatorIO.Data1 = (int)(Program.ProcessProvider.LongHistory[SystemStats.IoReadOther][0]);
            indicatorIO.TextValue = Utils.GetNiceSizeName(Program.ProcessProvider.LongHistory[SystemStats.IoReadOther][0]);

            // Update the plotter settings.
            plotterIO.LongData1 = Program.ProcessProvider.LongHistory[SystemStats.IoReadOther];
            plotterIO.LongData2 = Program.ProcessProvider.LongHistory[SystemStats.IoWrite];

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
            plotterIO.Text = "R+O: " + Utils.GetNiceSizeName(plotterIO.LongData1[0]) +
                ", W: " + Utils.GetNiceSizeName(plotterIO.LongData2[0]);

            // update the memory graph text
            plotterMemory.Text = "Commit: " + Utils.GetNiceSizeName(plotterMemory.LongData1[0]) +
                ", Phys. Mem: " + Utils.GetNiceSizeName(plotterMemory.LongData2[0]);

            plotterCPU.MoveGrid();
            plotterCPU.Draw();
            plotterIO.MoveGrid();
            plotterIO.Draw();
            plotterMemory.MoveGrid();
            plotterMemory.Draw();
        }

        private void UpdateInfo()
        {
            var perfInfo = Program.ProcessProvider.Performance;
            var info = new PerformanceInformation();

            Win32.GetPerformanceInfo(out info, System.Runtime.InteropServices.Marshal.SizeOf(info));

            SystemCacheInformation cacheInfo;
            int retLen;

            Win32.NtQuerySystemInformation(SystemInformationClass.SystemFileCacheInformation,
                out cacheInfo, Marshal.SizeOf(typeof(SystemCacheInformation)), out retLen);

            // Totals
            labelTotalsProcesses.Text = ((ulong)info.ProcessCount).ToString("N0");
            labelTotalsThreads.Text = ((ulong)info.ThreadCount).ToString("N0");
            labelTotalsHandles.Text = ((ulong)info.HandlesCount).ToString("N0");

            // Commit
            labelCCC.Text = Utils.GetNiceSizeName((ulong)perfInfo.CommittedPages * _pageSize);
            labelCCP.Text = Utils.GetNiceSizeName((ulong)perfInfo.PeakCommitment * _pageSize);
            labelCCL.Text = Utils.GetNiceSizeName((ulong)perfInfo.CommitLimit * _pageSize);

            // Physical Memory
            string physMemText = Utils.GetNiceSizeName((ulong)(_pages - perfInfo.AvailablePages) * _pageSize);

            labelPMC.Text = physMemText;
            labelPSC.Text = Utils.GetNiceSizeName((ulong)info.SystemCache * _pageSize);
            labelPMT.Text = Utils.GetNiceSizeName((ulong)_pages * _pageSize);

            // Update the physical memory indicator here because we have perfInfo available.
            indicatorPhysical.Color1 = Properties.Settings.Default.PlotterMemoryWSColor;
            indicatorPhysical.Data1 = (int)(_pages - perfInfo.AvailablePages);
            indicatorPhysical.TextValue = physMemText;

            // File cache
            labelCacheCurrent.Text = Utils.GetNiceSizeName(cacheInfo.SystemCacheWsSize);
            labelCachePeak.Text = Utils.GetNiceSizeName(cacheInfo.SystemCacheWsPeakSize);
            labelCacheMinimum.Text = Utils.GetNiceSizeName((ulong)cacheInfo.SystemCacheWsMinimum * _pageSize);
            labelCacheMaximum.Text = Utils.GetNiceSizeName((ulong)cacheInfo.SystemCacheWsMaximum * _pageSize);

            // Paged/Non-paged pools
            labelKPPPU.Text = Utils.GetNiceSizeName((ulong)perfInfo.PagedPoolPages * _pageSize);
            labelKPPVU.Text = Utils.GetNiceSizeName((ulong)perfInfo.PagedPoolUsage * _pageSize);
            labelKPPA.Text = ((ulong)perfInfo.PagedPoolAllocs).ToString("N0");
            labelKPPF.Text = ((ulong)perfInfo.PagedPoolFrees).ToString("N0");
            labelKPNPU.Text = Utils.GetNiceSizeName((ulong)perfInfo.NonPagedPoolUsage * _pageSize);
            labelKPNPA.Text = ((ulong)perfInfo.NonPagedPoolAllocs).ToString("N0");
            labelKPNPF.Text = ((ulong)perfInfo.NonPagedPoolFrees).ToString("N0");

            // Page faults
            labelPFTotal.Text = ((ulong)perfInfo.PageFaults).ToString("N0");
            labelPFCOW.Text = ((ulong)perfInfo.CopyOnWriteFaults).ToString("N0");
            labelPFTrans.Text = ((ulong)perfInfo.TransitionFaults).ToString("N0");
            labelPFCacheTrans.Text = ((ulong)perfInfo.CacheTransitionFaults).ToString("N0");
            labelPFDZ.Text = ((ulong)perfInfo.CacheTransitionFaults).ToString("N0");
            labelPFCache.Text = ((ulong)cacheInfo.SystemCacheWsFaults).ToString("N0");

            // I/O
            labelIOR.Text = ((ulong)perfInfo.IoReadOperationCount).ToString("N0");
            labelIORB.Text = Utils.GetNiceSizeName(perfInfo.IoReadTransferCount);
            labelIOW.Text = ((ulong)perfInfo.IoWriteOperationCount).ToString("N0");
            labelIOWB.Text = Utils.GetNiceSizeName(perfInfo.IoWriteTransferCount);
            labelIOO.Text = ((ulong)perfInfo.IoOtherOperationCount).ToString("N0");
            labelIOOB.Text = Utils.GetNiceSizeName(perfInfo.IoOtherTransferCount);

            // CPU
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
