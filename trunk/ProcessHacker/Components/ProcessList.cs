using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker.Components
{
    public partial class ProcessList : UserControl
    {
        ProcessProvider _provider;
        public event KeyEventHandler KeyDown;
        public event EventHandler SelectedIndexChanged;

        public ProcessList()
        {
            InitializeComponent();

            typeof(ListView).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listProcesses, true, null);

            listProcesses.KeyDown += new KeyEventHandler(ProcessList_KeyDown);
            listProcesses.SelectedIndexChanged += new System.EventHandler(listProcesses_SelectedIndexChanged);
        }

        public override bool Focused
        {
            get
            {
                return listProcesses.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listProcesses.ContextMenu; }
            set { listProcesses.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listProcesses.ContextMenuStrip; }
            set { listProcesses.ContextMenuStrip = value; }
        }

        public void listProcesses_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        public void ProcessList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        #region Core Process List

        public ProcessProvider Provider
        {
            get { return _provider; }
            set
            {
                _provider = value;

                listProcesses.Items.Clear();

                if (_provider != null)
                {
                    foreach (ProcessItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new ProviderInvokeMethod(this.BeginInvoke);
                    _provider.ClearEvents();
                    _provider.DictionaryAdded += new ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }
            }
        }

        private void provider_DictionaryAdded(object item)
        {
            ProcessItem pitem = (ProcessItem)item;
            ListViewItem litem = new ListViewItem();

            litem = listProcesses.Items.Add(pitem.PID.ToString(), pitem.Name, 0);

            litem.Text = pitem.Name;
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, pitem.PID.ToString()));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, pitem.MemoryUsage));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, pitem.Username));

            try
            {
                litem.ToolTipText = FileVersionInfo.GetVersionInfo(
                    Misc.GetRealPath(pitem.Process.MainModule.FileName)).ToString();
            }
            catch
            { }

            if (pitem.Icon == null)
            {
                litem.ImageKey = "Generic";
            }
            else
            {
                imageList.Images.Add(pitem.PID.ToString(), pitem.Icon);
                litem.ImageKey = pitem.PID.ToString();
            }
        }

        private void provider_DictionaryModified(object item)
        {
            ProcessItem pitem = (ProcessItem)item;
            ListViewItem litem = listProcesses.Items[pitem.PID.ToString()];

            litem.SubItems[2].Text = pitem.MemoryUsage;
            litem.SubItems[3].Text = pitem.Username;
        }

        private void provider_DictionaryRemoved(object item)
        {
            ProcessItem pitem = (ProcessItem)item;
            int index = listProcesses.Items[pitem.PID.ToString()].Index;
            int selected = listProcesses.SelectedItems.Count;

            listProcesses.Items[pitem.PID.ToString()].Remove();

            if (selected == 1)
            {
                if (listProcesses.Items.Count == 0)
                { }
                else if (index > (listProcesses.Items.Count - 1))
                {
                    listProcesses.Items[listProcesses.Items.Count - 1].Selected = true;
                }
                else 
                {
                    listProcesses.Items[index].Selected = true;
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listProcesses.BeginUpdate();
        }

        public void EndUpdate()
        {
            listProcesses.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listProcesses.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listProcesses.SelectedItems; }
        }

        #endregion
    }
}
