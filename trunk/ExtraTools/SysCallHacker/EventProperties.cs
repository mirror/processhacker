using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.SsLogging;
using ProcessHacker.Native.Symbols;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Common;

namespace SysCallHacker
{
    public partial class EventProperties : Form
    {
        private LogEvent _event;
        private SymbolProvider _symbols;

        public EventProperties(LogEvent even)
        {
            InitializeComponent();

            _event = even;

            textSystemCall.Text = MainWindow.SysCallNames.ContainsKey(even.Event.CallNumber) ? MainWindow.SysCallNames[even.Event.CallNumber] : "(unknown)";
            textTime.Text = _event.Event.Time.ToString();
            textMode.Text = _event.Event.Mode == KProcessorMode.UserMode ? "User-mode" : "Kernel-mode";

            for (int i = 0; i < _event.Event.Arguments.Length; i++)
            {
                ListViewItem item = new ListViewItem();

                item.Text = i.ToString();
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, "0x" + _event.Event.Arguments[i].ToString("x")));

                if (_event.Arguments[i] != null)
                {
                    string text = "";
                    SsData data = _event.Arguments[i];

                    if (data is SsSimple)
                    {
                        text = (data as SsSimple).Argument.ToString();
                    }
                    else if (data is SsHandle)
                    {
                        SsHandle handle = data as SsHandle;

                        if (!string.IsNullOrEmpty(handle.Name))
                            text = handle.TypeName + ": " + handle.Name;
                        else
                            text = handle.TypeName + ": PID: " + handle.ProcessId.ToString() +
                                ", TID: " + handle.ThreadId.ToString();
                    }
                    else if (data is SsUnicodeString)
                    {
                        text = (data as SsUnicodeString).String;
                    }
                    else if (data is SsObjectAttributes)
                    {
                        SsObjectAttributes oa = data as SsObjectAttributes;
                        text = "";

                        if (oa.RootDirectory != null)
                            text = oa.RootDirectory.Name;

                        if (oa.ObjectName != null)
                        {
                            if (!string.IsNullOrEmpty(text))
                                text = text + "\\" + oa.ObjectName.String;
                            else
                                text = oa.ObjectName.String;
                        }
                    }
                    else if (data is SsClientId)
                    {
                        text = "PID: " + (data as SsClientId).Original.ProcessId.ToString() +
                            ", TID: " + (data as SsClientId).Original.ThreadId.ToString();
                    }

                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, text));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, _event.Arguments[i].GetType().Name.Remove(0, 2)));
                }
                else
                {
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ""));
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, ""));
                }

                listArguments.Items.Add(item);
            }

            SymbolProvider.Options = SymbolOptions.DeferredLoads | SymbolOptions.UndName;

            try
            {
                using (var phandle = new ProcessHandle(_event.Event.ProcessId, 
                    ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                {
                    _symbols = new SymbolProvider(phandle);

                    phandle.EnumModules((module) =>
                        {
                            _symbols.LoadModule(module.FileName, module.BaseAddress, module.Size);
                            return true;
                        });
                    Windows.EnumKernelModules((module) =>
                        {
                            _symbols.LoadModule(module.FileName, module.BaseAddress);
                            return true;
                        });
                    _symbols.PreloadModules = true;

                    for (int i = 0; i < _event.Event.StackTrace.Length; i++)
                    {
                        var address = _event.Event.StackTrace[i];
                        string fileName;
                        IntPtr baseAddress;

                        fileName = _symbols.GetModuleFromAddress(address, out baseAddress);

                        listStackTrace.Items.Add(new ListViewItem(new string[]
                        {
                            "0x" + address.ToString("x"),
                            (new System.IO.FileInfo(fileName)).Name + "+0x" + address.Decrement(baseAddress).ToString("x")
                        }));

                        WorkQueue.GlobalQueueWorkItemTag(new Action<int, IntPtr>((i_, address_) =>
                            {
                                string symbol = _symbols.GetSymbolFromAddress(address_.ToUInt64());

                                if (this.IsHandleCreated)
                                    this.BeginInvoke(new Action(() => listStackTrace.Items[i_].SubItems[1].Text = symbol));
                            }), "resolve-symbol", i, address);
                    }
                }
            }
            catch
            { }

            listArguments.SetDoubleBuffered(true);
            listStackTrace.SetDoubleBuffered(true);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EventProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }
    }
}
