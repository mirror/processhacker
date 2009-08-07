using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.SsLogging;
using ProcessHacker.Native.Symbols;

namespace SysCallHacker
{
    public partial class MainWindow : Form
    {
        private static Dictionary<int, string> _sysCallNames = new Dictionary<int, string>();

        public static Dictionary<int, string> SysCallNames
        {
            get { return _sysCallNames; }
        }

        private SsLogger _logger;
        private List<LogEvent> _events = new List<LogEvent>();
        private LogEvent _lastEvent;
        private Dictionary<int, SystemProcess> _processes;

        public MainWindow()
        {
            InitializeComponent();

            Win32.LoadLibrary("C:\\Program Files\\Debugging Tools for Windows (x86)\\dbghelp.dll");

            SymbolProvider symbols = new SymbolProvider(ProcessHandle.Current);

            SymbolProvider.Options |= SymbolOptions.PublicsOnly;

            ProcessHandle.Current.EnumModules((module) =>
                {
                    if (module.BaseName.Equals("ntdll.dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        symbols.LoadModule(module.FileName, module.BaseAddress, module.Size);
                        return false;
                    }

                    return true;
                });

            symbols.EnumSymbols("ntdll!Nt*", (symbol) =>
                {
                    if (!symbol.Name.StartsWith("Ntdll"))
                    {
                        _sysCallNames.Add(
                            Marshal.ReadInt32(symbol.Address.ToIntPtr().Increment(1)),
                            symbol.Name
                            );
                    }

                    return true;
                });

            symbols.Dispose();

            KProcessHacker.Instance = new KProcessHacker();

            _logger = new SsLogger(4096, true);
            _logger.EventBlockReceived += new EventBlockReceivedDelegate(logger_EventBlockReceived);
            _logger.ArgumentBlockReceived += new ArgumentBlockReceivedDelegate(logger_ArgumentBlockReceived);
            _logger.AddPreviousModeRule(FilterType.Exclude, KProcessorMode.KernelMode);
            _logger.AddProcessIdRule(FilterType.Exclude, ProcessHandle.GetCurrentId());
            //_logger.Start();

            listEvents.SetDoubleBuffered(true);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProcessHandle.Current.Terminate();
        }

        private void logger_EventBlockReceived(SsEvent eventBlock)
        {
            LogEvent logEvent = new LogEvent(eventBlock);

            lock (_events)
                _events.Add(logEvent);

            _lastEvent = logEvent;
        }

        private void logger_ArgumentBlockReceived(SsData argBlock)
        {
            if (_lastEvent != null)
            {
                if (argBlock.Index < _lastEvent.Arguments.Length)
                    _lastEvent.Arguments[argBlock.Index] = argBlock;
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            _processes = Windows.GetProcesses();

            lock (_events)
                listEvents.VirtualListSize = _events.Count;

            int blocksWritten, blocksDropped;

            _logger.GetStatistics(out blocksWritten, out blocksDropped);

            if (blocksWritten > 0 || blocksDropped > 0)
            {
                lock (_events)
                {
                    statusBar.Text = _events.Count.ToString("N0") + " events, " +
                        blocksWritten.ToString("N0") + " blocks, " +
                        blocksDropped.ToString("N0") + " dropped (" +
                        ((double)blocksDropped / (blocksWritten + blocksDropped) * 100).ToString("F2") + "%)";
                }
            }
        }

        private void listEvents_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            LogEvent logEvent;
            ListViewItem item;

            lock (_events)
                logEvent = _events[e.ItemIndex];

            string objectName = "";

            if (logEvent.Arguments.Length > 2)
            {
                SsObjectAttributes oa = logEvent.Arguments[2] as SsObjectAttributes;

                if (oa != null)
                {
                    if (oa.ObjectName != null)
                        objectName = oa.ObjectName.String;
                }
            }

            item = new ListViewItem(new string[]
            {
                logEvent.Event.Time.ToString(),
                _processes.ContainsKey(logEvent.Event.ProcessId) ? _processes[logEvent.Event.ProcessId].Name : logEvent.Event.ProcessId.ToString(),
                _sysCallNames[logEvent.Event.CallNumber],
                logEvent.Event.Mode == KProcessorMode.UserMode ? "User" : "Kernel",
                objectName
            });
            e.Item = item;
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button == toolBarButtonStart)
            {
                _logger.Start();
            }
            else if (e.Button == toolBarButtonStop)
            {
                _logger.Stop();
            }
        }

        private void ShowProperties(int index)
        {
            (new EventProperties(_events[index])).ShowDialog();
        }

        private void listEvents_DoubleClick(object sender, EventArgs e)
        {
            this.ShowProperties(listEvents.SelectedIndices[0]);
        }
    }
}
