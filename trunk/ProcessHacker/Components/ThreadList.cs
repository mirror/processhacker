using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProcessHacker
{
    public partial class ThreadList : UserControl
    {
        ThreadProvider _provider;
        public event KeyEventHandler KeyDown;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public ThreadList()
        {
            InitializeComponent();

            listThreads.KeyDown += new KeyEventHandler(ThreadList_KeyDown);
            listThreads.MouseDown += new MouseEventHandler(listThreads_MouseDown);
            listThreads.MouseUp += new MouseEventHandler(listThreads_MouseUp);
            listThreads.SelectedIndexChanged += new System.EventHandler(listThreads_SelectedIndexChanged);
        }

        private void listThreads_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listThreads_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listThreads_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ThreadList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        #region Properties

        public bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listThreads, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listThreads, value, null);
            }
        }

        public override bool Focused
        {
            get
            {
                return listThreads.Focused;
            }
        }

        public override ContextMenu ContextMenu
        {
            get { return listThreads.ContextMenu; }
            set { listThreads.ContextMenu = value; }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return listThreads.ContextMenuStrip; }
            set { listThreads.ContextMenuStrip = value; }
        }

        public ThreadProvider Provider
        {
            get { return _provider; }
            set
            {
                if (_provider != null)
                {
                    _provider.DictionaryAdded -= new ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified -= new ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved -= new ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }

                _provider = value;

                listThreads.Items.Clear();

                if (_provider != null)
                {
                    foreach (ThreadItem item in _provider.Dictionary.Values)
                    {
                        provider_DictionaryAdded(item);
                    }

                    _provider.UseInvoke = true;
                    _provider.Invoke = new ProviderInvokeMethod(this.BeginInvoke);
                    _provider.DictionaryAdded += new ProviderDictionaryAdded(provider_DictionaryAdded);
                    _provider.DictionaryModified += new ProviderDictionaryModified(provider_DictionaryModified);
                    _provider.DictionaryRemoved += new ProviderDictionaryRemoved(provider_DictionaryRemoved);
                }
            }
        }

        #endregion

        #region Core Thread List

        private void provider_DictionaryAdded(object item)
        {
            ThreadItem titem = (ThreadItem)item;
            ListViewItem litem = new ListViewItem();

            litem = listThreads.Items.Add(titem.TID.ToString(), titem.TID.ToString(), 0);

            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, titem.State));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, titem.CPUTime));
            litem.SubItems.Add(new ListViewItem.ListViewSubItem(litem, titem.Priority));
        }

        private void provider_DictionaryModified(object item)
        {
            ThreadItem titem = (ThreadItem)item;
            ListViewItem litem = listThreads.Items[titem.TID.ToString()];

            litem.SubItems[1].Text = titem.State;
            litem.SubItems[2].Text = titem.CPUTime;
            litem.SubItems[3].Text = titem.Priority;
        }

        private void provider_DictionaryRemoved(object item)
        {
            ThreadItem titem = (ThreadItem)item;
            int index = listThreads.Items[titem.TID.ToString()].Index;
            bool selected = listThreads.Items[titem.TID.ToString()].Selected;
            int selectedCount = listThreads.SelectedItems.Count;

            listThreads.Items[titem.TID.ToString()].Remove();

            if (selected && selectedCount == 1)
            {
                if (listThreads.Items.Count == 0)
                { }
                else if (index > (listThreads.Items.Count - 1))
                {
                    listThreads.Items[listThreads.Items.Count - 1].Selected = true;
                }
                else 
                {
                    listThreads.Items[index].Selected = true;
                }
            }
        }

        #endregion

        #region Interfacing

        public void BeginUpdate()
        {
            listThreads.BeginUpdate();
        }

        public void EndUpdate()
        {
            listThreads.EndUpdate();
        }

        public ListView.ListViewItemCollection Items
        {
            get { return listThreads.Items; }
        }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listThreads.SelectedItems; }
        }

        #endregion
    }
}
