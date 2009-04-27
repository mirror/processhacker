/*
 * Process Hacker - 
 *   I/O history icon
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
    public class IoHistoryIcon : ProviderIcon
    {
        public IoHistoryIcon()
        {
            this.UseSecondLine = true;
            this.UseLongData = true;
            this.OverlaySecondLine = true;
            this.MinMaxValue = 128 * 1024; // 128KB
        }

        protected override void ProviderUpdated()
        {
            if (this.Provider.RunCount < 2)
                return;

            this.LineColor1 = Properties.Settings.Default.PlotterIOROColor;
            this.LineColor2 = Properties.Settings.Default.PlotterIOWColor;

            this.Update(
                this.Provider.LongDeltas[SystemStats.IoRead] +
                this.Provider.LongDeltas[SystemStats.IoOther],
                this.Provider.LongDeltas[SystemStats.IoWrite]
                );

            this.Redraw();

            this.Text = "R: " + Misc.GetNiceSizeName(this.Provider.LongDeltas[SystemStats.IoRead]) + 
                "\nO: " + Misc.GetNiceSizeName(this.Provider.LongDeltas[SystemStats.IoOther]) + 
                "\nW: " + Misc.GetNiceSizeName(this.Provider.LongDeltas[SystemStats.IoWrite]);

            if (this.Provider.Dictionary.ContainsKey(this.Provider.PIDWithMostIoActivity))
            {
                string mostIoName = this.Provider.Dictionary[this.Provider.PIDWithMostIoActivity].Name;

                if (this.Text.Length + mostIoName.Length + 1 < 64) // 1 char for the LF
                    this.Text += "\n" + mostIoName;
            }
        }
    }
}
