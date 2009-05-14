/*
 * Process Hacker - 
 *   provider-based plotter icon
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

using System.Windows.Forms;

namespace ProcessHacker
{
    public class ProviderIcon : PlotterIcon
    {
        private bool _enabled = false;

        public ProviderIcon()
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

        protected ProcessSystemProvider Provider
        {
            get { return Program.ProcessProvider; }
        }

        protected virtual void ProviderUpdated()
        { }
    }
}
