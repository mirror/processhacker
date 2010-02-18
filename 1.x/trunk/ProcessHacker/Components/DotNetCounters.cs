/*
 * Process Hacker - 
 *   .NET counters control
 * 
 * Copyright (C) 2009-2010 wj32
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
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.UI;

namespace ProcessHacker.Components
{
    public partial class DotNetCounters : UserControl
    {
        private int _pid;
        private bool _initialized = false;
        private string _name;
        private string _instanceName;
        private string _categoryName;
        private PerformanceCounter[] _counters;

        public DotNetCounters(int pid, string name)
        {
            InitializeComponent();

            listAppDomains.SetTheme("explorer");

            listValues.SetDoubleBuffered(true);
            listValues.SetTheme("explorer");
            listValues.ContextMenu = listValues.GetCopyMenu();
            listValues.AddShortcuts();

            _pid = pid;
            _name = name;
        }

        public void Initialize()
        {
            // The .NET counters remove the file name extension in the 
            // instance names, even if the extension is something other than 
            // ".exe".
            {
                int indexOfDot = _name.LastIndexOf('.');

                if (indexOfDot != -1)
                    _instanceName = _name.Substring(0, indexOfDot);
                else
                    _instanceName = _name;
            }

            try
            {
                var publish = new Debugger.Core.Wrappers.CorPub.ICorPublish();
                var process = publish.GetProcess(_pid);
                Debugger.Interop.CorPub.ICorPublishAppDomainEnum appDomainEnum;

                try
                {
                    process.WrappedObject.EnumAppDomains(out appDomainEnum);

                    if (appDomainEnum != null)
                    {
                        uint count;
                        Debugger.Interop.CorPub.ICorPublishAppDomain appDomain;

                        try
                        {
                            while (true)
                            {
                                appDomainEnum.Next(1, out appDomain, out count);

                                if (count == 0)
                                    break;

                                try
                                {
                                    StringBuilder sb = new StringBuilder(0x100);
                                    uint strCount;

                                    appDomain.GetName((uint)sb.Capacity, out strCount, sb);
                                    listAppDomains.Items.Add(new ListViewItem(sb.ToString(0, (int)strCount - 1)));
                                }
                                finally
                                {
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(appDomain);
                                }
                            }
                        }
                        finally
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(appDomainEnum);
                        }
                    }
                }
                finally
                {
                    Debugger.Wrappers.ResourceManager.ReleaseCOMObject(process.WrappedObject, process.GetType());
                }
            }
            catch
            { }

            var categories = PerformanceCounterCategory.GetCategories();
            List<string> names = new List<string>();

            foreach (var category in categories)
            {
                if (category.CategoryName.StartsWith(".NET CLR"))
                    names.Add(category.CategoryName);
            }

            names.Sort();
            comboCategories.Items.AddRange(names.ToArray());
            comboCategories.SelectedItem = comboCategories.Items[0];

            _initialized = true;
        }

        private void comboCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
                return;

            _categoryName = (string)comboCategories.SelectedItem;

            this.UpdateCounters();
            this.UpdateInfo();
        }

        private void UpdateCounters()
        {             
            listValues.Items.Clear();

            try
            {
                _counters = (new PerformanceCounterCategory(_categoryName)).GetCounters(_instanceName);
            }
            catch
            {
                _counters = null;
                return;
            }

            for (int i = 0; i < _counters.Length; i++)
            {
                var counter = _counters[i];

                if (
                    (counter.CounterType == PerformanceCounterType.NumberOfItems32 ||
                    counter.CounterType == PerformanceCounterType.NumberOfItems64 ||
                    counter.CounterType == PerformanceCounterType.RawFraction) &&
                    counter.CounterName != "Not Displayed"
                    )
                {
                    listValues.Items.Add(new ListViewItem(
                        new string[] 
                    {
                        _counters[i].CounterName,
                        ""
                    }));
                }
            }
        }

        public void InitialColumnAutoSize()
        {
            columnName.Width = (listValues.Width - 30) * 6 / 10;
            columnValue.Width = (listValues.Width - 30) * 4 / 10;
        }

        public void UpdateInfo()
        {
            if (_counters == null)
                return;

            for (int i = 0, listIndex = 0; i < _counters.Length; i++, listIndex++)
            {
                var counter = _counters[i];
                string value;

                if (
                    (counter.CounterType == PerformanceCounterType.NumberOfItems32 ||
                    counter.CounterType == PerformanceCounterType.NumberOfItems64 ||
                    counter.CounterType == PerformanceCounterType.RawFraction) &&
                    counter.CounterName != "Not Displayed"
                    )
                {
                    if (counter.CounterType == PerformanceCounterType.RawFraction)
                        value = counter.NextValue().ToString("N2");
                    else
                        value = counter.RawValue.ToString("N0");

                    listValues.Items[listIndex].SubItems[1].Text = value;
                }
                else
                {
                    listIndex--;
                }
            }
        }
    }
}
