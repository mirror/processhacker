using System;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Ui;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Symbols;
using ProcessHacker.Native.Ui;

namespace ProcessAnalyzer
{
    public partial class MainWindow : Form
    {
        private int _pid;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                KProcessHacker.Instance = new KProcessHacker();
            }
            catch
            { }

            Win32.LoadLibrary("C:\\Program Files\\Debugging Tools for Windows (x86)\\dbghelp.dll");

            listHandleTraces.ListViewItemSorter = new SortedListViewComparer(listHandleTraces);
        }

        private void openProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChooseProcess();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowException(string operation, Exception ex)
        {
            MessageBox.Show(operation + ": " + ex.Message, "Process Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region Handle Tracing

        private SymbolProvider _symbols;
        private ProcessHandleTraceCollection _currentHtCollection;

        private void ChooseProcess()
        {
            ChooseProcessDialog cpd = new ChooseProcessDialog();

            if (cpd.ShowDialog() == DialogResult.OK)
                _pid = cpd.SelectedPid;
        }

        private void PopulateHandleTraceList()
        {
            listHandleTraces.BeginUpdate();
            listHandleTraces.Items.Clear();

            for (int i = 0; i < _currentHtCollection.Count; i++)
            {
                var trace = _currentHtCollection[i];
                ListViewItem item = new ListViewItem(
                    new string[]
                    {
                        i.ToString(),
                        "0x" + trace.Handle.ToString("x"),
                        trace.Type.ToString(),
                        trace.ClientId.ThreadId.ToString()
                    });

                item.Tag = i;
                listHandleTraces.Items.Add(item);
            }

            listHandleTraces.EndUpdate();
        }

        private void buttonEnableHandleTracing_Click(object sender, EventArgs e)
        {
            try
            {
                using (var phandle = new ProcessHandle(_pid, ProcessAccess.SetInformation))
                    phandle.EnableHandleTracing();
            }
            catch (Exception ex)
            {
                this.ShowException("Error enabling handle tracing", ex);
            }
        }

        private void buttonDisableHandleTracing_Click(object sender, EventArgs e)
        {
            try
            {
                using (var phandle = new ProcessHandle(_pid, ProcessAccess.SetInformation))
                    phandle.DisableHandleTracing();
            }
            catch (Exception ex)
            {
                this.ShowException("Error disabling handle tracing", ex);
            }
        }

        private void buttonSnapshot_Click(object sender, EventArgs e)
        {
            try
            {
                using (var phandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                {
                    _currentHtCollection = phandle.GetHandleTraces();

                    if (_symbols != null)
                        _symbols.Dispose();

                    SymbolProvider.Options |= SymbolOptions.DeferredLoads;
                    _symbols = new SymbolProvider(phandle);

                    WorkQueue.GlobalQueueWorkItem(new Action(() =>
                        {
                            var symbols = _symbols;

                            _symbols.PreloadModules = true;

                            try
                            {
                                foreach (var module in phandle.GetModules())
                                {
                                    try
                                    {
                                        symbols.LoadModule(module.FileName, module.BaseAddress);
                                    }
                                    catch
                                    { }
                                }
                            }
                            catch
                            { }

                            try
                            {
                                foreach (var module in Windows.GetKernelModules())
                                {
                                    try
                                    {
                                        symbols.LoadModule(module.FileName, module.BaseAddress);
                                    }
                                    catch
                                    { }
                                }
                            }
                            catch
                            { }
                        }));
                }

                this.PopulateHandleTraceList();
            }
            catch (Exception ex)
            {
                this.ShowException("Error getting the handle trace snapshot", ex);
            }
        }

        private void listHandleTraces_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentHtCollection == null || listHandleTraces.SelectedItems.Count != 1)
                return;

            var trace = _currentHtCollection[(int)listHandleTraces.SelectedItems[0].Tag];

            listHandleStack.BeginUpdate();
            listHandleStack.Items.Clear();

            foreach (var address in trace.Stack)
            {
                ListViewItem item = new ListViewItem();

                item.Text = "0x" + address.ToInt32().ToString("x8");
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, _symbols.GetSymbolFromAddress(address.ToUInt64())));
                listHandleStack.Items.Add(item);
            }

            listHandleStack.EndUpdate();
        }

        #endregion

        #region Hidden Objects

        private struct KVars
        {
            public IntPtr NonPagedPoolStartAddress;
            public IntPtr NonPagedPoolSizeAddress;
            public IntPtr NonPagedPoolStart;
            public uint NonPagedPoolSize;
            public IntPtr PsProcessTypeAddress;
            public IntPtr PsProcessType;
            public IntPtr PsThreadTypeAddress;
            public IntPtr PsThreadType;
        }

        private unsafe KVars GetKVars()
        {
            SymbolProvider symbols = new SymbolProvider();

            symbols.LoadModule(Windows.KernelFileName, Windows.KernelBase);

            KVars vars = new KVars();

            vars.NonPagedPoolStartAddress = symbols.GetSymbolFromName("MmNonPagedPoolStart").Address.ToIntPtr();
            vars.NonPagedPoolSizeAddress = symbols.GetSymbolFromName("MmMaximumNonPagedPoolInBytes").Address.ToIntPtr();
            vars.PsProcessTypeAddress = symbols.GetSymbolFromName("PsProcessType").Address.ToIntPtr();
            vars.PsThreadTypeAddress = symbols.GetSymbolFromName("PsThreadType").Address.ToIntPtr();

            int bytesRead;

            KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                ProcessHandle.Current,
                vars.NonPagedPoolStartAddress.ToInt32(),
                &vars.NonPagedPoolStart,
                IntPtr.Size,
                out bytesRead
                );
            KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                ProcessHandle.Current,
                vars.NonPagedPoolSizeAddress.ToInt32(),
                &vars.NonPagedPoolSize,
                sizeof(uint),
                out bytesRead
                );
            KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                ProcessHandle.Current,
                vars.PsProcessTypeAddress.ToInt32(),
                &vars.PsProcessType,
                IntPtr.Size,
                out bytesRead
                );
            KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                ProcessHandle.Current,
                vars.PsThreadTypeAddress.ToInt32(),
                &vars.PsThreadType,
                IntPtr.Size,
                out bytesRead
                );

            symbols.Dispose();

            return vars;
        }

        private unsafe void ScanHiddenObjects()
        {
            KVars vars = this.GetKVars();
            int bytesRead;

            throw new NotSupportedException();

            listHiddenObjects.Items.Clear();

            using (var currentPage = new MemoryAlloc(Windows.PageSize))
            {
                for (
                    IntPtr address = vars.NonPagedPoolStart;
                    address.CompareTo(vars.NonPagedPoolStart.Increment(vars.NonPagedPoolSize)) == -1;
                    address = address.Increment(Windows.PageSize)
                    )
                {
                    try
                    {
                        KProcessHacker.Instance.KphReadVirtualMemoryUnsafe(
                            ProcessHandle.Current,
                            address.ToInt32(),
                            (IntPtr)currentPage,
                            Windows.PageSize,
                            out bytesRead
                            );
                    }
                    catch
                    {
                        continue;
                    }

                    for (
                        IntPtr inner = address;
                        inner.CompareTo(address.Increment(Windows.PageSize)) == -1;
                        inner = inner.Increment(8)
                        )
                    {
                    }

                    labelObjectsScanProgress.Text = string.Format("Scanned 0x{0:x8}", address.ToInt32());
                    Application.DoEvents();
                }
            }

            labelObjectsScanProgress.Text = "Finished.";
        }

        private void buttonScanHiddenObjects_Click(object sender, EventArgs e)
        {
            this.ScanHiddenObjects();
        }

        #endregion
    }
}
