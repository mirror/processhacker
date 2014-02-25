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
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Symbols;

namespace ProcessHacker
{
    public partial class SysInfoWindow : Form
    {
        private static IntPtr _mmSizeOfPagedPoolInBytes;
        private static IntPtr _mmMaximumNonPagedPoolInBytes;

        private bool _isFirstPaint = true;
        private Components.Plotter[] _cpuPlotters;
        private uint _noOfCPUs = Program.ProcessProvider.System.NumberOfProcessors;
        private uint _pages = (uint)Program.ProcessProvider.System.NumberOfPhysicalPages;
        private uint _pageSize = (uint)Program.ProcessProvider.System.PageSize;

        public SysInfoWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();

            this.Size = Settings.Instance.SysInfoWindowSize;
            this.Location = Utils.FitRectangle(new Rectangle(
                Settings.Instance.SysInfoWindowLocation, this.Size), this).Location;

            // Load the pool limit addresses.
            if (
                _mmSizeOfPagedPoolInBytes == IntPtr.Zero && 
                KProcessHacker.Instance != null
                )
            {
                WorkQueue.GlobalQueueWorkItemTag(new Action(() =>
                    {
                        try
                        {
                            SymbolProvider symbols = new SymbolProvider();

                            symbols.LoadModule(Windows.KernelFileName, Windows.KernelBase);
                            _mmSizeOfPagedPoolInBytes = 
                                symbols.GetSymbolFromName("MmSizeOfPagedPoolInBytes").Address.ToIntPtr();
                            _mmMaximumNonPagedPoolInBytes = 
                                symbols.GetSymbolFromName("MmMaximumNonPagedPoolInBytes").Address.ToIntPtr();
                        }
                        catch
                        { }
                    }), "load-mm-addresses");
            }
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

            // Set indicators color
            indicatorCpu.Color1 = Settings.Instance.PlotterCPUKernelColor;
            indicatorCpu.Color2 = Settings.Instance.PlotterCPUUserColor;
            indicatorIO.Color1 = Settings.Instance.PlotterIOROColor;
            indicatorPhysical.Color1 = Settings.Instance.PlotterMemoryWSColor;  


            // Set up the plotter controls.
            plotterCPU.Data1 = Program.ProcessProvider.CpuKernelHistory;
            plotterCPU.Data2 = Program.ProcessProvider.CpuUserHistory;
            plotterCPU.GetToolTip = i =>
                Program.ProcessProvider.MostCpuHistory[i] + "\n" +
                ((plotterCPU.Data1[i] + plotterCPU.Data2[i]) * 100).ToString("N2") +
                "% (K " + (plotterCPU.Data1[i] * 100).ToString("N2") +
                "%, U " + (plotterCPU.Data2[i] * 100).ToString("N2") + "%)" + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterIO.LongData1 = Program.ProcessProvider.IoReadOtherHistory;
            plotterIO.LongData2 = Program.ProcessProvider.IoWriteHistory;
            plotterIO.GetToolTip = i =>
                Program.ProcessProvider.MostIoHistory[i] + "\n" +
                "R+O: " + Utils.FormatSize(plotterIO.LongData1[i]) + "\n" +
                "W: " + Utils.FormatSize(plotterIO.LongData2[i]) + "\n" +
                Program.ProcessProvider.TimeHistory[i].ToString();
            plotterMemory.LongData1 = Program.ProcessProvider.CommitHistory;
            plotterMemory.LongData2 = Program.ProcessProvider.PhysicalMemoryHistory;
            plotterMemory.GetToolTip = i =>
                "Commit: " + Utils.FormatSize(plotterMemory.LongData1[i]) + "\n" +
                "Phys. Memory: " + Utils.FormatSize(plotterMemory.LongData2[i]) + "\n" +
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
                plotter.Data1 = Program.ProcessProvider.CpusKernelHistory[i];
                plotter.Data2 = Program.ProcessProvider.CpusUserHistory[i];
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
            checkShowOneGraphPerCPU.Checked = Settings.Instance.ShowOneGraphPerCPU;

            if (_noOfCPUs == 1)
                checkShowOneGraphPerCPU.Enabled = false;

            this.UpdateGraphs();
            this.UpdateInfo();

            Program.ProcessProvider.Updated +=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
            
            //We need todo this here or TopMost property gets over-rided
            //by AlwaysOnTopCheckbox
            this.SetTopMost();
        }

        private void SysInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Instance.SysInfoWindowLocation = this.Location;
                Settings.Instance.SysInfoWindowSize = this.Size;
            }
            
            Program.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
            Settings.Instance.ShowOneGraphPerCPU = checkShowOneGraphPerCPU.Checked;   
        }

        private void UpdateGraphs()
        {
            // Update the CPU indicator.         
            indicatorCpu.Data1 = (int)(Program.ProcessProvider.CurrentCpuKernelUsage * indicatorCpu.Maximum);
            indicatorCpu.Data2 = (int)(Program.ProcessProvider.CurrentCpuUserUsage * indicatorCpu.Maximum);
            indicatorCpu.TextValue = (Program.ProcessProvider.CurrentCpuUsage * 100).ToString("F2") + "%";

            // Update the I/O indicator.  
            int count = plotterIO.Width / plotterIO.EffectiveMoveStep;
            long maxRO = Program.ProcessProvider.IoReadOtherHistory.Take(count).Max();
            long maxW = Program.ProcessProvider.IoWriteHistory.Take(count).Max();
            if(maxRO>maxW)
                indicatorIO.Maximum = maxRO;
            else
                indicatorIO.Maximum = maxW;
            indicatorIO.Data1 = Program.ProcessProvider.IoReadOtherHistory[0];
            indicatorIO.TextValue = Utils.FormatSize(Program.ProcessProvider.IoReadOtherHistory[0]);

            // Update the plotter settings.
            plotterIO.LongData1 = Program.ProcessProvider.IoReadOtherHistory;
            plotterIO.LongData2 = Program.ProcessProvider.IoWriteHistory;

            plotterCPU.LineColor1 = Settings.Instance.PlotterCPUKernelColor;
            plotterCPU.LineColor2 = Settings.Instance.PlotterCPUUserColor;
            plotterIO.LineColor1 = Settings.Instance.PlotterIOROColor;
            plotterIO.LineColor2 = Settings.Instance.PlotterIOWColor;
            plotterMemory.LineColor1 = Settings.Instance.PlotterMemoryPrivateColor;
            plotterMemory.LineColor2 = Settings.Instance.PlotterMemoryWSColor;

            for (int i = 0; i < _cpuPlotters.Length; i++)
            {
                _cpuPlotters[i].LineColor1 = Settings.Instance.PlotterCPUKernelColor;
                _cpuPlotters[i].LineColor2 = Settings.Instance.PlotterCPUUserColor;
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
            plotterIO.Text = "R+O: " + Utils.FormatSize(plotterIO.LongData1[0]) +
                ", W: " + Utils.FormatSize(plotterIO.LongData2[0]);

            // update the memory graph text
            plotterMemory.Text = "Commit: " + Utils.FormatSize(plotterMemory.LongData1[0]) +
                ", Phys. Mem: " + Utils.FormatSize(plotterMemory.LongData2[0]);

            plotterCPU.MoveGrid();
            plotterCPU.Draw();
            plotterIO.MoveGrid();
            plotterIO.Draw();
            plotterMemory.MoveGrid();
            plotterMemory.Draw();
        }

        private unsafe void GetPoolLimits(out int paged, out int nonPaged)
        {
            int pagedLocal, nonPagedLocal;
            int retLength;

            // Read the two variables, stored in kernel-mode memory.
            KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                ProcessHacker.Native.Objects.ProcessHandle.Current,
                _mmSizeOfPagedPoolInBytes.ToInt32(),
                &pagedLocal,
                sizeof(int),
                out retLength
                );
            KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                ProcessHacker.Native.Objects.ProcessHandle.Current,
                _mmMaximumNonPagedPoolInBytes.ToInt32(),
                &nonPagedLocal,
                sizeof(int),
                out retLength
                );

            paged = pagedLocal;
            nonPaged = nonPagedLocal;
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
            labelTotalsUptime.Text = Utils.FormatLongTimeSpan(Windows.GetUptime());

            // Commit
            labelCCC.Text = Utils.FormatSize((ulong)perfInfo.CommittedPages * _pageSize);
            labelCCP.Text = Utils.FormatSize((ulong)perfInfo.PeakCommitment * _pageSize);
            labelCCL.Text = Utils.FormatSize((ulong)perfInfo.CommitLimit * _pageSize);

            // Physical Memory
            string physMemText = Utils.FormatSize((ulong)(_pages - perfInfo.AvailablePages) * _pageSize);

            labelPMC.Text = physMemText;
            labelPSC.Text = Utils.FormatSize((ulong)info.SystemCache * _pageSize);
            labelPMT.Text = Utils.FormatSize((ulong)_pages * _pageSize);

            // Update the physical memory indicator here because we have perfInfo available.

            indicatorPhysical.Data1 = _pages - perfInfo.AvailablePages;
            indicatorPhysical.TextValue = physMemText;

            // File cache
            labelCacheCurrent.Text = Utils.FormatSize(cacheInfo.SystemCacheWsSize);
            labelCachePeak.Text = Utils.FormatSize(cacheInfo.SystemCacheWsPeakSize);
            labelCacheMinimum.Text = Utils.FormatSize((ulong)cacheInfo.SystemCacheWsMinimum * _pageSize);
            labelCacheMaximum.Text = Utils.FormatSize((ulong)cacheInfo.SystemCacheWsMaximum * _pageSize);

            // Paged/Non-paged pools
            labelKPPPU.Text = Utils.FormatSize((ulong)perfInfo.ResidentPagedPoolPage * _pageSize);
            labelKPPVU.Text = Utils.FormatSize((ulong)perfInfo.PagedPoolPages * _pageSize);
            labelKPPA.Text = ((ulong)perfInfo.PagedPoolAllocs).ToString("N0");
            labelKPPF.Text = ((ulong)perfInfo.PagedPoolFrees).ToString("N0");
            labelKPNPU.Text = Utils.FormatSize((ulong)perfInfo.NonPagedPoolPages * _pageSize);
            labelKPNPA.Text = ((ulong)perfInfo.NonPagedPoolAllocs).ToString("N0");
            labelKPNPF.Text = ((ulong)perfInfo.NonPagedPoolFrees).ToString("N0");

            // Get the pool limits
            long pagedLimit = 0;
            long nonPagedLimit = 0;

            if (
                _mmSizeOfPagedPoolInBytes != IntPtr.Zero &&
                _mmMaximumNonPagedPoolInBytes != IntPtr.Zero &&
                KProcessHacker.Instance != null
                )
            {
                try
                {
                    int pl, npl;

                    this.GetPoolLimits(out pl, out npl);
                    pagedLimit = pl;
                    nonPagedLimit = npl;
                }
                catch
                { }
            }

            if (pagedLimit != 0)
                labelKPPL.Text = Utils.FormatSize(pagedLimit);
            else if (KProcessHacker.Instance == null)
                labelKPPL.Text = "no driver";
            else
                labelKPPL.Text = "no symbols";

            if (nonPagedLimit != 0)
                labelKPNPL.Text = Utils.FormatSize(nonPagedLimit);
            else if (KProcessHacker.Instance == null)
                labelKPNPL.Text = "no driver";
            else
                labelKPNPL.Text = "no symbols";

            // Page faults
            labelPFTotal.Text = ((ulong)perfInfo.PageFaultCount).ToString("N0");
            labelPFCOW.Text = ((ulong)perfInfo.CopyOnWriteCount).ToString("N0");
            labelPFTrans.Text = ((ulong)perfInfo.TransitionCount).ToString("N0");
            labelPFCacheTrans.Text = ((ulong)perfInfo.CacheTransitionCount).ToString("N0");
            labelPFDZ.Text = ((ulong)perfInfo.CacheTransitionCount).ToString("N0");
            labelPFCache.Text = ((ulong)cacheInfo.SystemCacheWsFaults).ToString("N0");

            // I/O
            labelIOR.Text = ((ulong)perfInfo.IoReadOperationCount).ToString("N0");
            labelIORB.Text = Utils.FormatSize(perfInfo.IoReadTransferCount);
            labelIOW.Text = ((ulong)perfInfo.IoWriteOperationCount).ToString("N0");
            labelIOWB.Text = Utils.FormatSize(perfInfo.IoWriteTransferCount);
            labelIOO.Text = ((ulong)perfInfo.IoOtherOperationCount).ToString("N0");
            labelIOOB.Text = Utils.FormatSize(perfInfo.IoOtherTransferCount);

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
