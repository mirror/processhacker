/*
 * Process Hacker - 
 *   CPU usage icon
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

using System.Drawing;

namespace ProcessHacker
{
    public class CpuUsageIcon : UsageIcon
    {
        private ProcessSystemProvider _provider = Program.ProcessProvider;
        private bool _enabled = false;

        public CpuUsageIcon()
        { }

        public override void Dispose()
        {
            this.Enabled = false;
            base.Dispose();
        }

        private void ProcessProvider_Updated()
        {
            //this.Parent.BeginInvoke(new MethodInvoker(this.ProviderUpdated));
            this.ProviderUpdated();
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    if (value)
                        Program.ProcessProvider.Updated += ProcessProvider_Updated;
                    else
                        Program.ProcessProvider.Updated -= ProcessProvider_Updated;
                }

                _enabled = value;
            }
        }

        private ProcessSystemProvider Provider
        {
            get { return Program.ProcessProvider; }
        }

        private void ProviderUpdated()
        {
            float k = _provider.CurrentCpuKernelUsage;
            float u = _provider.CurrentCpuUserUsage;
            int height = this.Size.Height;
            int width = this.Size.Width;

            using (Bitmap b = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    int kl = (int)(k * height);
                    int ul = (int)(u * height);
                    Color kline = Settings.Instance.PlotterCPUKernelColor;
                    Color kfill = Color.FromArgb(100, kline);
                    Color uline = Settings.Instance.PlotterCPUUserColor;
                    Color ufill = Color.FromArgb(100, uline);

                    g.FillRectangle(new SolidBrush(Color.Black), g.ClipBounds);

                    if (kl + ul == 0)
                        g.DrawLine(new Pen(uline), 0, height - 1, width - 1, height - 1);

                    g.FillRectangle(new SolidBrush(ufill), 0, height - (ul + kl), width, ul);
                    g.DrawLine(new Pen(uline), 0, height - (ul + kl) - 1, width, height - (ul + kl) - 1);

                    if (kl > 0)
                    {
                        g.FillRectangle(new SolidBrush(kfill), 0, height - kl, width, kl);
                        g.DrawLine(new Pen(kline), 0, height - kl - 1, width, height - kl - 1);
                    }
                }

                var newIcon = Icon.FromHandle(b.GetHicon());
                var oldIcon = this.Icon;

                this.Icon = newIcon;
                ProcessHacker.Native.Api.Win32.DestroyIcon(oldIcon.Handle);
            }

            string mostCpuProcess = _provider.MostCpuHistory[0];

            string text = "CPU Usage: " + ((k + u) * 100).ToString("N2") + "%"
                //+ " (" +
                //"K: " + (k * 100).ToString("G2") +
                //", U: " + (u * 100).ToString("G2") + ")"
                ;

            if (text.Length + mostCpuProcess.Length + 1 < 64)
                text += "\n" + mostCpuProcess;

            this.Text = text;
        }
    }
}
