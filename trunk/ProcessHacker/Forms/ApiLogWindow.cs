using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ProcessHacker.Forms
{
    public partial class ApiLogWindow : Form
    {
        private struct HOOK
        {
            public int Address;
            public char Hooked;
            public unsafe fixed byte ReplacedBytes[16];
            public int ReplacedLength;
        }

        private struct NT_HOOK
        {
            public HOOK Hook;
            public int SystemCallIndex;
            public short ArgumentLength;
        }

        private Thread _pipeThread;
        private Win32.NamedPipeHandle _namedPipe = new Win32.NamedPipeHandle("\\\\.\\Pipe\\AlLogPipe",
            Win32.PIPE_ACCESS_MODE.FILE_FLAG_FIRST_PIPE_INSTANCE | Win32.PIPE_ACCESS_MODE.PIPE_ACCESS_INBOUND,
            0,
            255, 0, 512, 1000);

        public ApiLogWindow()
        {
            InitializeComponent();

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
                _namedPipe.Connect();

                int length = Misc.BytesToInt(_namedPipe.Read(4), Misc.Endianness.Little);
                byte[] data = new byte[length - 4];

                _namedPipe.Read(data);

                PNGNet.ByteStreamReader bsr = new PNGNet.ByteStreamReader(data);
                BinaryReader br = new BinaryReader(bsr);

                int pid = br.ReadInt32();
                string function = Misc.ReadUnicodeString(bsr);
                bsr.Seek(System.Runtime.InteropServices.Marshal.SizeOf(typeof(NT_HOOK)), SeekOrigin.Current);
                Dictionary<string, uint> dictionary = new Dictionary<string, uint>();

                while (bsr.Position < bsr.Length)
                {
                    string key = Misc.ReadUnicodeString(bsr);
                    uint value = br.ReadUInt32();

                    dictionary.Add(key, value);
                }

                StringBuilder args = new StringBuilder();

                foreach (var kvp in dictionary)
                    args.Append(kvp.Key + ": 0x" + kvp.Value.ToString("x8") + ", ");

                if (args.Length > 0)
                    args.Remove(args.Length - 2, 2);

                br.Close();

                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        listLog.Items.Add(new ListViewItem(new string[]
                        {
                            DateTime.Now.ToString(),
                            function,
                            args.ToString()
                        }));
                    }));

                _namedPipe.Disconnect();
            }
        }
    }
}
