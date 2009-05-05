using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Forms
{
    public partial class ApiLogWindow : Form
    {
        [Flags]
        private enum CM_TYPE : uint
        {
            CmVoid = 0,
            CmBool,
            CmByte,
            CmInt16,
            CmInt32,
            CmPVoid,
            CmBytes,
            CmString,
            CmType = 0x00ffffff,

            CmHex = 0x01000000,
            CmDisplayHint = 0xff000000
        }

        private struct HOOK
        {
            public IntPtr Address;
            public char Hooked;
            public unsafe fixed byte ReplacedBytes[16];
            public int ReplacedLength;
        }

        private List<string[]> _items = new List<string[]>();
        private Thread _pipeThread;
        private static NamedPipeHandle _namedPipe = new NamedPipeHandle("\\\\.\\Pipe\\AlLogPipe",
            PipeAccessMode.Inbound,
            0,
            255, 0, 512, 1000);

        public ApiLogWindow()
        {
            InitializeComponent();

            listLog.SetDoubleBuffered(true);
            listLog.SetTheme("explorer");

            try
            {
                _namedPipe.Disconnect();
            }
            catch
            { }

            _pipeThread = new Thread(new ThreadStart(this.ProcessPipe));
            _pipeThread.Start();
        }

        private void ApiLogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pipeThread.Abort();
        }

        private void ProcessPipe()
        {
            while (true)
            {
                try
                {
                    _namedPipe.Connect();
                }
                catch
                {
                    Thread.Sleep(1000);
                    continue;
                }

                int length = Misc.BytesToInt(_namedPipe.Read(4), Misc.Endianness.Little);
                byte[] data = new byte[length - 4];

                _namedPipe.Read(data);

                PNGNet.ByteStreamReader bsr = new PNGNet.ByteStreamReader(data);
                BinaryReader br = new BinaryReader(bsr);

                int pid = br.ReadInt32();
                string function = Misc.ReadUnicodeString(bsr);
                bsr.Seek(System.Runtime.InteropServices.Marshal.SizeOf(typeof(HOOK)), SeekOrigin.Current);
                var dictionary = new Dictionary<string, KeyValuePair<CM_TYPE, object>>();

                while (bsr.Position < bsr.Length)
                {
                    string key = Misc.ReadUnicodeString(bsr);
                    CM_TYPE type = (CM_TYPE)br.ReadUInt32();
                    int valueLength = br.ReadInt32();
                    object value = null;

                    switch (type & CM_TYPE.CmType)
                    {
                        case CM_TYPE.CmBool:
                            value = br.ReadChar() != 0;
                            break;
                        case CM_TYPE.CmByte:
                            value = br.ReadByte();
                            break;
                        case CM_TYPE.CmInt16:
                            value = br.ReadInt16();
                            break;
                        case CM_TYPE.CmInt32:
                            value = br.ReadInt32();
                            break;
                        case CM_TYPE.CmPVoid:
                            value = br.ReadInt32();
                            break;
                        case CM_TYPE.CmBytes:
                            value = br.ReadBytes(valueLength);
                            break;
                        case CM_TYPE.CmString:
                            value = Misc.ReadUnicodeString(bsr, valueLength);
                            break;
                        case CM_TYPE.CmVoid:
                        default:
                            break;
                    }

                    dictionary.Add(key, new KeyValuePair<CM_TYPE, object>(type, value));
                }

                StringBuilder args = new StringBuilder();

                foreach (var kvp in dictionary)
                {
                    string str = null;

                    switch (kvp.Value.Key & CM_TYPE.CmType)
                    {
                        case CM_TYPE.CmBool:
                            str = ((bool)kvp.Value.Value).ToString();
                            break;
                        case CM_TYPE.CmByte:
                        case CM_TYPE.CmInt16:
                        case CM_TYPE.CmInt32:
                            if ((kvp.Value.Key & CM_TYPE.CmHex) != 0)
                                str = "0x" + ((int)kvp.Value.Value).ToString("x");
                            else
                                str = ((int)kvp.Value.Value).ToString();
                            break;
                        case CM_TYPE.CmPVoid:
                            str = "0x" + ((int)kvp.Value.Value).ToString("x8");
                            break;
                        case CM_TYPE.CmString:
                            str = ((string)kvp.Value.Value).Replace("\0", "");
                            break;
                        default:
                            str = "?";
                            break;
                    }

                    args.Append(kvp.Key + ": " + str + ", ");
                }

                if (args.Length > 0)
                    args.Remove(args.Length - 2, 2);

                br.Close();

                _items.Add(new string[]
                {
                    DateTime.Now.ToString(),
                    Program.ProcessProvider.Dictionary.ContainsKey(pid) ? 
                    Program.ProcessProvider.Dictionary[pid].Name + " (" + pid.ToString() + ")" : 
                    "(" + pid.ToString() + ")",
                    function,
                    args.ToString()
                });

                _namedPipe.Disconnect();
            }
        }

        private void listLog_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(_items[e.ItemIndex]);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            listLog.VirtualListSize = _items.Count;
        }
    }
}
