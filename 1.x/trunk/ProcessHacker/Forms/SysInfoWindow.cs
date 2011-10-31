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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Symbols;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class SysInfoWindow : Form
    {
        private static IntPtr _mmSizeOfPagedPoolInBytes;
        private static IntPtr _mmMaximumNonPagedPoolInBytes;

        private Plotter[] _cpuPlotters;
        private uint _noOfCPUs = Program.ProcessProvider.System.NumberOfProcessors;
        private int _pages = Program.ProcessProvider.System.NumberOfPhysicalPages;
        private int _pageSize = Program.ProcessProvider.System.PageSize;

        public SysInfoWindow()
        {     
            this.InitializeComponent();

            //if (!Settings.Instance.SysInfoWindowBounds.IsEmpty)
                //this.DesktopBounds = Utils.FitRectangle(Settings.Instance.SysInfoWindowBounds, this);

            // Load the pool limit addresses.
            if (_mmSizeOfPagedPoolInBytes == IntPtr.Zero)
            {
                WorkQueue.GlobalQueueWorkItemTag(new Action(() =>
                {
                    try
                    {
                        using (SymbolProvider symbols = new SymbolProvider())
                        {
                            symbols.LoadModule(Windows.KernelFileName, Windows.KernelBase);

                            _mmSizeOfPagedPoolInBytes = (IntPtr)symbols.GetSymbolFromName("MmSizeOfPagedPoolInBytes").Address;
                            _mmMaximumNonPagedPoolInBytes = (IntPtr)symbols.GetSymbolFromName("MmMaximumNonPagedPoolInBytes").Address;
                        }
                    }
                    catch (Exception) { }

                }), "load-mm-addresses");
            }

            this.trackerMemory.values = Program.ProcessProvider.PhysicalMemoryHistory;
            this.trackerMemory.DrawColor = Settings.Instance.PlotterMemoryPrivateColor;

            this.trackerCommit.Maximum = (int)Program.ProcessProvider.Performance.CommitLimit;
            this.trackerCommit.values = Program.ProcessProvider.CommitHistory;
            this.trackerCommit.DrawColor = Settings.Instance.PlotterMemoryWSColor;

            // Set indicators color
            this.indicatorCpu.Color1 = Settings.Instance.PlotterCPUUserColor;
            this.indicatorCpu.Color2 = Settings.Instance.PlotterCPUKernelColor;
            
            this.indicatorIO.Color1 = Settings.Instance.PlotterIOROColor;
            this.indicatorPhysical.Color1 = Settings.Instance.PlotterMemoryPrivateColor;

            this.plotterCPU.LineColor2 = Settings.Instance.PlotterCPUKernelColor;
            this.plotterCPU.LineColor1 = Settings.Instance.PlotterCPUUserColor;

            this.plotterIO.LineColor1 = Settings.Instance.PlotterIOROColor;
            this.plotterIO.LineColor2 = Settings.Instance.PlotterIOWColor;

            // Maximum physical memory.
            this.indicatorPhysical.Maximum = _pages;

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

            //plotterMemory.Data1 = Program.ProcessProvider.CommitHistory;
            //plotterMemory.Data2 = Program.ProcessProvider.PhysicalMemoryHistory;
            //plotterMemory.GetToolTip = i => "Commit: " + plotterMemory.Data1[i] + "\n" +
            //    "Phys. Memory: " + plotterMemory.Data2[i] + "\n" + Program.ProcessProvider.TimeHistory[i].ToString();

            // Create a plotter per CPU.
            _cpuPlotters = new Plotter[_noOfCPUs];
            tableCPUs.ColumnCount = (int)_noOfCPUs;
            tableCPUs.ColumnStyles.Clear();
            tableCPUs.Dock = DockStyle.Fill;

            for (int i = 0; i < _cpuPlotters.Length; i++)
            {
                Plotter plotter;

                tableCPUs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f / _noOfCPUs));
                _cpuPlotters[i] = plotter = new Plotter();
               
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
               
                this.tableCPUs.Controls.Add(plotter, i, 0);
            }

            this.checkShowOneGraphPerCPU.Checked = Settings.Instance.ShowOneGraphPerCPU;

            if (_noOfCPUs == 1)
                checkShowOneGraphPerCPU.Enabled = false;

            Program.ProcessProvider.Updated += ProcessProvider_Updated;

            //We need todo this here or TopMost property gets over-rided by AlwaysOnTopCheckbox
            this.TopMost = Settings.Instance.AlwaysOnTop;

            this.UpdateGraphs();
            this.UpdateInfo();
        }

        private void UpdateGraphs()
        {
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        // Update the CPU indicator.         
                        this.indicatorCpu.Data1 = (int)(Program.ProcessProvider.CurrentCpuKernelUsage * this.indicatorCpu.Maximum);
                        this.indicatorCpu.Data2 = (int)(Program.ProcessProvider.CurrentCpuUserUsage * this.indicatorCpu.Maximum);
                        this.indicatorCpu.TextValue = (Program.ProcessProvider.CurrentCpuUsage * 100).ToString("F2") + "%";

                        // Update the I/O indicator.  
                        int count = this.plotterIO.Width / this.plotterIO.EffectiveMoveStep;
                        long maxRO = Program.ProcessProvider.IoReadOtherHistory.Take(count).Max();
                        long maxW = Program.ProcessProvider.IoWriteHistory.Take(count).Max();

                        if (maxRO > maxW)
                            this.indicatorIO.Maximum = maxRO;
                        else
                            this.indicatorIO.Maximum = maxW;

                        this.indicatorIO.Data1 = Program.ProcessProvider.IoReadOtherHistory[0];
                        this.indicatorIO.TextValue = Utils.FormatSize(Program.ProcessProvider.IoReadOtherHistory[0]);

                        // Update the plotter settings.
                        this.plotterIO.LongData1 = Program.ProcessProvider.IoReadOtherHistory;
                        this.plotterIO.LongData2 = Program.ProcessProvider.IoWriteHistory;

                        if (this.checkShowOneGraphPerCPU.Checked)
                        {
                            for (int i = 0; i < _cpuPlotters.Length; i++)
                            {
                                _cpuPlotters[i].LineColor2 = Settings.Instance.PlotterCPUKernelColor;
                                _cpuPlotters[i].LineColor1 = Settings.Instance.PlotterCPUUserColor;
                                _cpuPlotters[i].Text = ((_cpuPlotters[i].Data1[0] + _cpuPlotters[i].Data2[0]) * 100).ToString("F2") + "% (K: " + (_cpuPlotters[i].Data1[0] * 100).ToString("F2") + "%, U: " + (_cpuPlotters[i].Data2[0] * 100).ToString("F2") + "%)";
                                _cpuPlotters[i].MoveGrid();
                                _cpuPlotters[i].Draw();
                            }
                        }

                        this.plotterCPU.Text = ((plotterCPU.Data1[0] + plotterCPU.Data2[0]) * 100).ToString("F2") + "% (K: " + (plotterCPU.Data1[0] * 100).ToString("F2") + "%, U: " + (plotterCPU.Data2[0] * 100).ToString("F2") + "%)";

                        // update the I/O graph text
                        this.plotterIO.Text = "R+O: " + Utils.FormatSize(plotterIO.LongData1[0]) + ", W: " + Utils.FormatSize(plotterIO.LongData2[0]);


                        this.plotterCPU.MoveGrid();
                        this.plotterIO.MoveGrid();

                        this.plotterCPU.Draw();
                        this.plotterIO.Draw();

                        this.trackerCommit.Draw();
                        this.trackerMemory.Draw();

                        break;
                    }
            }
        }

        private void UpdateInfo()
        {
            SystemPerformanceInformation perfInfo = Program.ProcessProvider.Performance;
            int retLen;

            PerformanceInformation info = new PerformanceInformation();
            SystemCacheInformation cacheInfo;

            Win32.GetPerformanceInfo(out info, PerformanceInformation.SizeOf);
            Win32.NtQuerySystemInformation(SystemInformationClass.SystemFileCacheInformation, out cacheInfo, SystemCacheInformation.SizeOf, out retLen);

            string physMemText = Utils.FormatSize((_pages - perfInfo.AvailablePages) * _pageSize);

            string commitText = Utils.FormatSize(perfInfo.CommittedPages * _pageSize);
 
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        // Update the physical memory indicator here because we have perfInfo available.
                        this.indicatorPhysical.Data1 = _pages - perfInfo.AvailablePages;
                        this.indicatorPhysical.TextValue = physMemText;


                        long memCount;
                        unchecked
                        {
                            memCount = (Program.ProcessProvider.System.NumberOfPhysicalPages - Program.ProcessProvider.Performance.AvailablePages) * Program.ProcessProvider.System.PageSize;
                        }

                        this.trackerMemory.Text = "Phys. Mem: " + Utils.FormatSize(memCount) + " / " + Utils.FormatSize((long)info.PhysicalTotal * (long)info.PageSize);

                        this.trackerCommit.Text = "Commit: " +
                                                  Utils.FormatSize(Program.ProcessProvider.Performance.CommittedPages * Program.ProcessProvider.System.PageSize)
                                                  + " / " + Utils.FormatSize(Program.ProcessProvider.Performance.CommitLimit * Program.ProcessProvider.System.PageSize);

                        this.indicatorCommit.Color1 = Settings.Instance.PlotterMemoryWSColor;
                        this.indicatorCommit.TextValue = commitText;
                        this.indicatorCommit.Maximum = Program.ProcessProvider.Performance.CommitLimit * Program.ProcessProvider.System.PageSize;
                        this.indicatorCommit.Data1 = Program.ProcessProvider.Performance.CommittedPages * Program.ProcessProvider.System.PageSize;


                        break;
                    }
                case 1:
                    {
                        // Totals
                        this.labelTotalsProcesses.Text = info.ProcessCount.ToString("N0");
                        this.labelTotalsThreads.Text = info.ThreadCount.ToString("N0");
                        this.labelTotalsHandles.Text = info.HandlesCount.ToString("N0");
                        this.labelTotalsUptime.Text = Utils.FormatLongTimeSpan(Windows.GetUptime());

                        // Commit
                        this.labelCCC.Text = commitText;
                        this.labelCCP.Text = Utils.FormatSize(perfInfo.PeakCommitment * _pageSize);
                        this.labelCCL.Text = Utils.FormatSize(perfInfo.CommitLimit * _pageSize);

                        // Physical Memory
                        this.labelPMC.Text = physMemText;
                        this.labelPSC.Text = info.SystemCache + " : " + Utils.FormatSize(info.SystemCache.ToInt32() * _pageSize);
                        this.labelPMT.Text = Utils.FormatSize(_pages * _pageSize);

                        // File cache
                        this.labelCacheCurrent.Text = Utils.FormatSize(cacheInfo.SystemCacheWsSize);
                        this.labelCachePeak.Text = Utils.FormatSize(cacheInfo.SystemCacheWsPeakSize);
                        this.labelCacheMinimum.Text = Utils.FormatSize(cacheInfo.SystemCacheWsMinimum.ToInt32() * _pageSize);
                        this.labelCacheMaximum.Text = Utils.FormatSize(cacheInfo.SystemCacheWsMaximum.ToInt32() * _pageSize);

                        // Paged/Non-paged pools
                        this.labelKPPPU.Text = Utils.FormatSize(perfInfo.ResidentPagedPoolPage * _pageSize);
                        this.labelKPPVU.Text = Utils.FormatSize(perfInfo.PagedPoolPages * _pageSize);
                        this.labelKPPA.Text = ((ulong)perfInfo.PagedPoolAllocs).ToString("N0");
                        this.labelKPPF.Text = ((ulong)perfInfo.PagedPoolFrees).ToString("N0");
                        this.labelKPNPU.Text = Utils.FormatSize(perfInfo.NonPagedPoolPages * _pageSize);
                        this.labelKPNPA.Text = ((ulong)perfInfo.NonPagedPoolAllocs).ToString("N0");
                        this.labelKPNPF.Text = ((ulong)perfInfo.NonPagedPoolFrees).ToString("N0");

                        // Get the pool limits
                        // long pagedLimit = 0;
                        // long nonPagedLimit = 0;

                        //if (_mmSizeOfPagedPoolInBytes != IntPtr.Zero && _mmMaximumNonPagedPoolInBytes != IntPtr.Zero && KProcessHacker.Instance != null)
                        //{
                        //        int pl, npl;

                               // this.GetPoolLimits(out pl, out npl);
                        //        pagedLimit = pl;
                        //        nonPagedLimit = npl;
                        //}

                        //if (pagedLimit != 0)
                        //labelKPPL.Text = Utils.FormatSize(pagedLimit);
                        //else
                        this.labelKPPL.Text = "no symbols";

                        // if (nonPagedLimit != 0)
                        // labelKPNPL.Text = Utils.FormatSize(nonPagedLimit);
                        // else
                        this.labelKPNPL.Text = "no symbols";

                        // Page faults
                        this.labelPFTotal.Text = ((ulong)perfInfo.PageFaultCount).ToString("N0");
                        this.labelPFCOW.Text = ((ulong)perfInfo.CopyOnWriteCount).ToString("N0");
                        this.labelPFTrans.Text = ((ulong)perfInfo.TransitionCount).ToString("N0");
                        this.labelPFCacheTrans.Text = ((ulong)perfInfo.CacheTransitionCount).ToString("N0");
                        this.labelPFDZ.Text = ((ulong)perfInfo.CacheTransitionCount).ToString("N0");
                        this.labelPFCache.Text = ((ulong)cacheInfo.SystemCacheWsFaults).ToString("N0");

                        // I/O
                        this.labelIOR.Text = ((ulong)perfInfo.IoReadOperationCount).ToString("N0");
                        this.labelIORB.Text = Utils.FormatSize(perfInfo.IoReadTransferCount);
                        this.labelIOW.Text = ((ulong)perfInfo.IoWriteOperationCount).ToString("N0");
                        this.labelIOWB.Text = Utils.FormatSize(perfInfo.IoWriteTransferCount);
                        this.labelIOO.Text = ((ulong)perfInfo.IoOtherOperationCount).ToString("N0");
                        this.labelIOOB.Text = Utils.FormatSize(perfInfo.IoOtherTransferCount);

                        // CPU
                        this.labelCPUContextSwitches.Text = ((ulong)perfInfo.ContextSwitches).ToString("N0");
                        this.labelCPUInterrupts.Text = ((ulong)Program.ProcessProvider.ProcessorPerf.InterruptCount).ToString("N0");
                        this.labelCPUSystemCalls.Text = ((ulong)perfInfo.SystemCalls).ToString("N0");

                        break;
                    }
            }
        }

        private void ProcessProvider_Updated()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(this.ProcessProvider_Updated));
            else
            {
                this.UpdateGraphs();
                this.UpdateInfo();
            }
        }

        private void checkShowOneGraphPerCPU_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkShowOneGraphPerCPU.Checked)
            {
                this.tableCPUs.Visible = true;

                //force a redraw, TODO: only required once.
                this.UpdateGraphs();
            }
            else
            {
                this.tableCPUs.Visible = false;
            }
        }

        private void checkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkAlwaysOnTop.Checked;
        }

        private void SysInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (this.WindowState == FormWindowState.Normal)
                //Settings.Instance.SysInfoWindowBounds = this.DesktopBounds;

            Program.ProcessProvider.Updated -= this.ProcessProvider_Updated;
            Settings.Instance.ShowOneGraphPerCPU = checkShowOneGraphPerCPU.Checked;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
