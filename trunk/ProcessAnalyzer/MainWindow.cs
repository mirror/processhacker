using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Ui;
using ProcessHacker.Native.Symbols;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Common;

namespace ProcessAnalyzer
{
    public partial class MainWindow : Form
    {
        private int _pid;
        private SymbolProvider _symbols;
        private ProcessHandleTraceCollection _currentHtCollection;

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
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            this.ChooseProcess();
        }

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
                        "0x" + trace.Handle.ToString("x"),
                        trace.Type.ToString(),
                        trace.ClientId.ThreadId.ToString()
                    });

                item.Tag = i;
                listHandleTraces.Items.Add(item);
            }

            listHandleTraces.EndUpdate();
        }

        private void ShowException(string operation, Exception ex)
        {
            MessageBox.Show(operation + ": " + ex.Message, "Process Analyzer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void openProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChooseProcess();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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

                            foreach (var module in phandle.GetModules())
                            {
                                try
                                {
                                    symbols.LoadModule(module.FileName, module.BaseAddress);
                                }
                                catch
                                { }
                            }

                            foreach (var module in Windows.GetKernelModules())
                            {
                                try
                                {
                                    symbols.LoadModule(module.FileName, module.BaseAddress);
                                }
                                catch
                                { }
                            }
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
    }
}
