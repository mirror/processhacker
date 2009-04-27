/*
 * Process Hacker - 
 *   CPU history icon
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
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public class CpuHistoryIcon : ProviderIcon
    {
        public CpuHistoryIcon()
        {
            this.UseSecondLine = true;
        }

        protected override void ProviderUpdated()
        {
            this.LineColor1 = Properties.Settings.Default.PlotterCPUKernelColor;
            this.LineColor2 = Properties.Settings.Default.PlotterCPUUserColor;
            this.Update(this.Provider.CurrentCpuKernelUsage, this.Provider.CurrentCpuUserUsage);
            this.Redraw();

            this.Text = "CPU Usage: " + (this.Provider.CurrentCpuUsage * 100).ToString("F2") + "%";

            string mostCpuText = this.Provider.MostCpuHistory[0];

            if (this.Text.Length + mostCpuText.Length + 1 < 64) // 1 char for the LF
                this.Text += "\n" + mostCpuText;
        }
    }
}
