using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Components
{
    public partial class DotNetCounters : UserControl
    {
        private int _pid;
        private string _name;
        private string _instanceName;
        private string _categoryName;
        private PerformanceCounter[] _counters;

        public DotNetCounters(int pid, string name)
        {
            InitializeComponent();

            _pid = pid;
            _name = name;

            if (_name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                _instanceName = _name.Remove(_name.Length - 4, 4);

            try
            {
                var publish = new Debugger.Core.Wrappers.CorPub.ICorPublish();
                var process = publish.GetProcess(_pid);
                Debugger.Interop.CorPub.ICorPublishAppDomainEnum appDomainEnum;

                process.WrappedObject.EnumAppDomains(out appDomainEnum);

                if (appDomainEnum != null)
                {
                    uint count;
                    Debugger.Interop.CorPub.ICorPublishAppDomain appDomain;

                    while (true)
                    {
                        appDomainEnum.Next(1, out appDomain, out count);

                        if (count == 0)
                            break;

                        StringBuilder sb = new StringBuilder(0x100);
                        uint strCount;

                        appDomain.GetName((uint)sb.Capacity, out strCount, sb);
                        listAppDomains.Items.Add(new ListViewItem(sb.ToString(0, (int)strCount)));
                    }
                }
            }
            catch
            { }

            var categories = PerformanceCounterCategory.GetCategories();

            foreach (var category in categories)
            {
                if (category.CategoryName.StartsWith(".NET CLR"))
                    comboCategories.Items.Add(category.CategoryName);
            }

            comboCategories.SelectedItem = comboCategories.Items[0];
        }

        private void comboCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            _categoryName = (string)comboCategories.SelectedItem;

            this.UpdateCounters();
            this.UpdateInfo();
        }

        private void UpdateCounters()
        {
            _counters = (new PerformanceCounterCategory(_categoryName)).GetCounters(_instanceName);

            listValues.Items.Clear();

            for (int i = 0; i < _counters.Length; i++)
            {
                listValues.Items.Add(new ListViewItem(
                    new string[] 
                    {
                        counter.CounterName,
                        ""
                    }));
            }
        }

        public void UpdateInfo()
        {
            for (int i = 0; i < _counters.Length; i++)
            {
                var counter = _counters[i];
                string result;
            }
        }
    }
}
