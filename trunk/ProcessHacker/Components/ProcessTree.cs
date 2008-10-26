using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace ProcessHacker
{
    public partial class ProcessTree : UserControl
    {
        ProcessProvider _provider;

        public ProcessTree()
        {
            InitializeComponent();
        }

        public bool DoubleBuffered
        {
            get
            {
                return (bool)typeof(TreeView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).GetValue(treeView, null);
            }
            set
            {
                typeof(TreeView).GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance).SetValue(treeView, value, null);
            }
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

                treeView.Nodes.Clear();

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

        private void provider_DictionaryAdded(object item)
        {
            ProcessItem pitem = (ProcessItem)item;
            TreeNode node;

            if (treeView.Nodes[Win32.GetProcessParent(pitem.PID).ToString()] == null)
            {
                node = treeView.Nodes.Add(pitem.PID.ToString(), "");
            }
            else
            {
                node = treeView.Nodes[Win32.GetProcessParent(pitem.PID).ToString()].Nodes.Add(pitem.PID.ToString(), "");
            }

            node.Text = pitem.PID.ToString() + " - " + pitem.Name;
                           
            if (pitem.Icon == null)
            {
                node.ImageKey = "Generic";
            }
            else
            {
                imageList.Images.Add(pitem.PID.ToString(), pitem.Icon);
                node.ImageKey = pitem.PID.ToString();
            }
        }

        private void provider_DictionaryModified(object item)
        {
            ProcessItem pitem = (ProcessItem)item;
            
        }

        private void provider_DictionaryRemoved(object item)
        {
            ProcessItem pitem = (ProcessItem)item;

        }
    }
}
