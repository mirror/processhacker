/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
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
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class HackerWindow : Form
    {
        double lastTotalMilliseconds = 0;
        Hashtable processCPUTime = new Hashtable();

        private void CPUTimeUpdater()
        {
            while (true)
            {
                double totalMilliseconds = 0;

                foreach (Process p in Process.GetProcesses())
                {
                    try
                    {
                        totalMilliseconds += p.TotalProcessorTime.TotalMilliseconds;
                    }
                    catch { }
                }

                double tmp = totalMilliseconds;
                totalMilliseconds -= lastTotalMilliseconds;
                lastTotalMilliseconds = tmp;

                lock (pids)
                {
                    foreach (int pid in pids)
                    {
                        try
                        {
                            Process p = Process.GetProcessById(pid);

                            double processMilliseconds = p.TotalProcessorTime.TotalMilliseconds;
                            double ptmp = processMilliseconds;
                            processMilliseconds -= Double.Parse(processTotalMilliseconds[p.Id].ToString());
                            processTotalMilliseconds[p.Id] = ptmp;

                            string result;

                            result = String.Format("{0:f2}", 100 * processMilliseconds / totalMilliseconds);

                            lock (processCPUTime)
                                processCPUTime[p.Id] = result;
                        }
                        catch { }
                    }
                }

                Thread.Sleep(RefreshInterval);
            }
        }
	}
}
