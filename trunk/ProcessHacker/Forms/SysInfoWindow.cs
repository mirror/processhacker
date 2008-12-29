/*
 * Process Hacker
 *                     
 * Copyright (C) 2008 wj32
 * Copyright (C) 2008 Dean
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
        private int _pageSize = Program.HackerWindow.ProcessProvider.System.PageSize;
        private DeltaManager<string, long> _deltaManager = new DeltaManager<string, long>(new Int64Subtractor());
        private int _runCount = 0;

        public SysInfoWindow()
        {
            InitializeComponent();

            plotterCPU.UseSecondLine = true;

            // intializse delta manager
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
                _cpuPlotters[i].UseSecondLine = true;
                tableCPUs.Controls.Add(_cpuPlotters[i], i, 0);

                _deltaManager.Add("cpu_" + i.ToString() + "_kernel", 0);
                _deltaManager.Add("cpu_" + i.ToString() + "_user", 0);
                _deltaManager.Add("cpu_" + i.ToString() + "_other", 0);
            }
        }

        private void SysInfoWindow_Load(object sender, EventArgs e)
        {
            _runCount = 0;
            checkShowOneGraphPerCPU.Checked = Properties.Settings.Default.ShowOneGraphPerCPU;
            Program.HackerWindow.ProcessProvider.PerformanceEnabled = true;
            Program.HackerWindow.ProcessProvider.Updated += 
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
        }

        private void SysInfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.HackerWindow.ProcessProvider.Updated -=
                new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
            Program.HackerWindow.ProcessProvider.PerformanceEnabled = false;

            Properties.Settings.Default.ShowOneGraphPerCPU = checkShowOneGraphPerCPU.Checked;
            e.Cancel = true;
            this.Hide();        
        }

        private void UpdateInfo()
        {
            Win32.SYSTEM_PERFORMANCE_INFORMATION perfInfo = Program.HackerWindow.ProcessProvider.Performance;

            plotterCPU.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            plotterCPU.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
            plotterIO.LineColor1 = Properties.Settings.Default.PlotterIOROColor;
            plotterIO.LineColor2 = Properties.Settings.Default.PlotterIOWColor;
            plotterMemory.LineColor1 = Properties.Settings.Default.PlotterMemoryPrivateColor;
            plotterMemory.LineColor2 = Properties.Settings.Default.PlotterMemoryWSColor;

            _deltaManager.Update("cpu_kernel", Program.HackerWindow.ProcessProvider.ProcessorPerf.KernelTime);
            _deltaManager.Update("cpu_user", Program.HackerWindow.ProcessProvider.ProcessorPerf.UserTime);
            _deltaManager.Update("cpu_other", Program.HackerWindow.ProcessProvider.ProcessorPerf.IdleTime + 
                Program.HackerWindow.ProcessProvider.ProcessorPerf.DpcTime + 
                Program.HackerWindow.ProcessProvider.ProcessorPerf.InterruptTime);
            _deltaManager.Update("contextswitches", perfInfo.ContextSwitches);
            _runCount++;

            if (_runCount < 1)
                return;

            long sysKernel = _deltaManager.GetDelta("cpu_kernel");
            long sysUser = _deltaManager.GetDelta("cpu_user");
            long sysOther = _deltaManager.GetDelta("cpu_other");
            float kernelUsage = (float)sysKernel / (sysKernel + sysUser + sysOther);
            float userUsage = (float)sysUser / (sysKernel + sysUser + sysOther);

            plotterCPU.Add(kernelUsage, userUsage);
            plotterCPU.Text = ((kernelUsage + userUsage) * 100).ToString("F2") +
                "% (K: " + (kernelUsage * 100).ToString("F2") +
                "%, U: " + (userUsage * 100).ToString("F2") + "%)";

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
        }

        private void ProcessProvider_Updated()
        {
            this.BeginInvoke(new MethodInvoker(this.UpdateInfo));
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
