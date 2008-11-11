using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

namespace ProcessHacker
{
    public partial class ProcessList : UserControl
    {
        ProcessProvider _provider;
        public event KeyEventHandler KeyDown;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event EventHandler SelectedIndexChanged;

        public ProcessList()
        {
            InitializeComponent();

            listProcesses.KeyDown += new KeyEventHandler(ProcessList_KeyDown);
            listProcesses.MouseDown += new MouseEventHandler(listProcesses_MouseDown);
            listProcesses.MouseUp += new MouseEventHandler(listProcesses_MouseUp);
            listProcesses.SelectedIndexChanged += new System.EventHandler(listProcesses_SelectedIndexChanged);
        }

        private void listProcesses_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(sender, e);
        }

        private void listProcesses_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(sender, e);
        }

        private void listProcesses_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);
        }

        private void ProcessList_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.KeyDown != null)
                this.KeyDown(sender, e);
        }

        private string GetKernelFileName()
        {
            int RequiredSize = 0;
            int[] ImageBases;

            Win32.EnumDeviceDrivers(null, 0, ref RequiredSize);
            ImageBases = new int[RequiredSize];
            Win32.EnumDeviceDrivers(ImageBases, RequiredSize * sizeof(int), ref RequiredSize);

            for (int i = 0; i < RequiredSize; i++)
            {
                if (ImageBases[i] == 0)
                    continue;

                StringBuilder name = new StringBuilder(256);
                StringBuilder filename = new StringBuilder(256);
                string realname = "";

                Win32.GetDeviceDriverBaseName(ImageBases[i], name, 255);
                Win32.GetDeviceDriverFileName(ImageBases[i], filename, 255);

                try
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(Misc.GetRealPath(filename.ToString()));
                    bool kernel = false;

                    realname = fi.FullName;

                    foreach (string k in Misc.KernelNames)
                    {
                        if (realname.ToLower() == Environment.SystemDirectory.ToLower() + "\\" + k.ToLower())
                        {
                            kernel = true;

                            break;
                        }
                    }

                    if (kernel)
                        return realname;
                }
                catch
                { }
            }

            return "";
        }

        #region Properties

        public bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(listProcesses, null);
            }
            set
            {
                typeof(ListView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listProcesses, value, null);
            }
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

        public ListView List
        {
            get { return listProcesses; }
        }

        public ProcessProvider Provider
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
                
                listProcesses.Items.Clear();

                foreach (string k in imageList.Images.Keys)
                    if (k != "Generic")
                        imageList.Images.RemoveByKey(k);

                if (_provider != null)
                {
                    foreach (ProcessItem item in _provider.Dictionary.Values)
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

        #region Core Process List

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
                string filename = "";

                if (pitem.PID == 4)
                {
                    filename = GetKernelFileName();
                }
                else
                {
                    filename = pitem.Process.MainModule.FileName;
                }

                FileVersionInfo info = FileVersionInfo.GetVersionInfo(
                    Misc.GetRealPath(filename));

                litem.ToolTipText = info.FileName + "\n" + 
                    info.FileDescription + " (" + info.FileVersion + ")\n" + 
                    info.CompanyName;
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
            bool selected = listProcesses.Items[pitem.PID.ToString()].Selected;
            int selectedCount = listProcesses.SelectedItems.Count;
            ListViewItem litem = listProcesses.Items[pitem.PID.ToString()];

            imageList.Images.RemoveByKey(litem.ImageKey);
            litem.Remove();

            if (selected && selectedCount == 1)
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
