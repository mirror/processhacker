/*
 * Process Hacker - 
 *   mini-graph
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class MiniSysInfo : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", SetLastError = true)]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS inset);

        MARGINS margins = new MARGINS() { Left = -1, Right = -1, Top = -1, Bottom = -1 };

        public MiniSysInfo()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            DwmExtendFrameIntoClientArea(this.Handle, ref margins);

            plotterCPU.BackColor = Color.FromArgb(255, 0, 0, 0);
            plotterCPU.Draw();
            plotterCPU.Data1 = Program.ProcessProvider.CpuKernelHistory;      
            plotterCPU.Data2 = Program.ProcessProvider.CpuUserHistory;

            plotterIO.BackColor = Color.FromArgb(255, 0, 0, 0);
            plotterIO.Draw();
            plotterIO.LongData1 = Program.ProcessProvider.IoReadOtherHistory;
            plotterIO.LongData2 = Program.ProcessProvider.IoWriteHistory;

            Program.ProcessProvider.Updated += new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
        }

        protected override void WndProc(ref Message m)
        {        
            base.WndProc(ref m);

            if (m.Msg == (int)WindowMessage.NcCalcSize)
            {
                if (m.WParam.ToInt32() != 0)
                {
                    m.Result = new IntPtr(0);
                }
            }
        }

        private void MiniSysInfo_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MiniSysInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.ProcessProvider.Updated -= new ProcessSystemProvider.ProviderUpdateOnce(ProcessProvider_Updated);
        }

        private void ProcessProvider_Updated()
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                plotterCPU.LineColor1 = Settings.Instance.PlotterCPUKernelColor;
                plotterCPU.LineColor2 = Settings.Instance.PlotterCPUUserColor;
                plotterCPU.MoveGrid();
                plotterCPU.Draw();

                plotterIO.LineColor1 = Settings.Instance.PlotterIOROColor;
                plotterIO.LineColor2 = Settings.Instance.PlotterIOWColor;
                plotterIO.MoveGrid();
                plotterIO.Draw();
            }));
        }
    }
}
