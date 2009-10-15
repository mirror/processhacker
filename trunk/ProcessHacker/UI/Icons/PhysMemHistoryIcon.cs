/*
 * Process Hacker - 
 *   physical memory history icon
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

using System.Runtime.InteropServices;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public class PhysMemHistoryIcon : ProviderIcon
    {
        public PhysMemHistoryIcon()
        {
            this.UseSecondLine = false;
            this.UseLongData = true;

            PerformanceInformation info = new PerformanceInformation();

            info.Size = Marshal.SizeOf(info);
            Win32.GetPerformanceInfo(out info, info.Size);
            this.MinMaxValue = info.PhysicalTotal.ToInt64();
        }

        protected override void ProviderUpdated()
        {
            this.LineColor1 = Settings.Instance.PlotterMemoryWSColor;
            this.Update(this.MinMaxValue - this.Provider.Performance.AvailablePages, 0);
            this.Redraw();

            this.Text = "Physical Memory: " + Utils.FormatSize(
                (long)(this.MinMaxValue - this.Provider.Performance.AvailablePages) *
                this.Provider.System.PageSize);
        }
    }
}
