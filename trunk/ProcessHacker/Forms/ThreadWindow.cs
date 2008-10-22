using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class ThreadWindow : Form
    {
        private int _pid;
        private int _tid;
        private int _phandle;
        private int _thandle;

        public const string DisplayFormat = "0x{0:x8}";

        public string[] Registers = 
            new string[] { "eax", "ebx", "ecx", "edx", "esi", "edi", "esp", "ebp", "cs", "ds", "es", "fs", "gs", "ss" };

        public string Id
        {
            get { return _pid + "-" + _tid; }
        }

        public ThreadWindow(int PID, int TID)
        {
            InitializeComponent();

            _pid = PID;
            _tid = TID;

            Program.ThreadWindows.Add(Id, this);

            this.Text = Win32.GetNameFromPID(_pid) + " (PID " + _pid.ToString() +
    ") - Thread " + _tid.ToString();

            PropertyInfo property = typeof(ListView).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance);

            property.SetValue(listViewCallStack, true, null);
            property.SetValue(listViewRegisters, true, null);

            foreach (string s in Registers)
            {
                ListViewItem item = new ListViewItem(s);
                ListViewItem.ListViewSubItem subitem = new ListViewItem.ListViewSubItem();

                subitem.Font = new Font(FontFamily.GenericMonospace, 10);

                item.SubItems.Add(subitem);
                
                listViewRegisters.Items.Add(item);
            }
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        private void ThreadWindow_Load(object sender, EventArgs e)
        {
            _phandle = Win32.OpenProcess(Win32.PROCESS_VM_READ, 0, _pid);

            if (_phandle == 0)
            {
                MessageBox.Show("Could not open process!", "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.Close();

                return;
            }

            _thandle = Win32.OpenThread(Win32.THREAD_GET_CONTEXT |
                Win32.THREAD_TERMINATE | Win32.THREAD_SUSPEND_RESUME, 0, _tid);

            if (_thandle == 0)
            {
                MessageBox.Show("Could not open thread!", "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                this.Close();

                return;
            }

            Program.UpdateWindows();
        }

        private void AddOrModify(ListView lv, ListViewItem item)
        {
            ListViewItem existing = null;
            bool exists = false;

            foreach (ListViewItem it in lv.Items)
            {
                if (it.Text == item.Text)
                {
                    exists = true;
                    existing = it;

                    break;
                }
            }

            if (exists)
            {
                existing.SubItems[1].Text = item.SubItems[1].Text;
            }
            else
            {
                lv.Items.Add(item);  
            }
        }

        private int BytesToInt32(byte[] b)
        {
            return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | (b[3] << 0);
        }

        private void WalkCallStack(Win32.CONTEXT context)
        {
            listViewCallStack.BeginUpdate();
            listViewCallStack.Items.Clear();

            Win32.STACKFRAME64 stackframe = new Win32.STACKFRAME64();

            stackframe.AddrPC.Mode = Win32.ADDRESS_MODE.AddrModeFlat;
            stackframe.AddrPC.Offset = context.Eip;
            stackframe.AddrFrame.Mode = Win32.ADDRESS_MODE.AddrModeFlat;
            stackframe.AddrFrame.Offset = context.Ebp;
            stackframe.AddrStack.Mode = Win32.ADDRESS_MODE.AddrModeFlat;
            stackframe.AddrStack.Offset = context.Esp;

            while (true)
            {
                if (Win32.StackWalk64(Win32.MachineType.IMAGE_FILE_MACHINE_i386, _phandle, _thandle,
                    ref stackframe, ref context, 0, new Win32.FunctionTableAccessProc64(Win32.SymFunctionTableAccess64),
                    new Win32.GetModuleBaseProc64(Win32.SymGetModuleBase64), 0) == 0)
                {
                    break;
                }
            }

            listViewCallStack.EndUpdate();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            Win32.CONTEXT context = new Win32.CONTEXT();

            context.ContextFlags = Win32.CONTEXT_FLAGS.CONTEXT_ALL;

            if (Win32.GetThreadContext(_thandle, ref context) == 0)
            {
                if (listViewCallStack.Enabled)
                {
                    listViewCallStack.Enabled = false;
                    listViewRegisters.Enabled = false;
                }

                return;
            }

            listViewCallStack.Enabled = true;
            listViewRegisters.Enabled = true;

            foreach (ListViewItem item in listViewRegisters.Items)
            {
                FieldInfo field;
                
                field = context.GetType().GetField(
                    item.Text[0].ToString().ToUpper() + item.Text.Substring(1));

                if (field == null)
                {
                    field = context.GetType().GetField(
                        "Seg" + item.Text[0].ToString().ToUpper() + item.Text.Substring(1));
                }

                if (field != null)
                {
                    item.SubItems[1].Text =
                        String.Format(DisplayFormat,
                        (int)field.GetValue(context));
                }
            }

            WalkCallStack(context);
        }

        private void suspendMenuItem_Click(object sender, EventArgs e)
        {
            if (Win32.SuspendThread(_thandle) == -1)
            {
                MessageBox.Show("Error suspending thread!", "Process Hacker", MessageBoxButtons.OK,
                 MessageBoxIcon.Error);     
            }
        }

        private void resumeMenuItem_Click(object sender, EventArgs e)
        {
            if (Win32.ResumeThread(_thandle) == -1)
            {
                MessageBox.Show("Error resuming thread!", "Process Hacker", MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
            }
        }

        private void terminateMenuItem_Click(object sender, EventArgs e)
        {
            if (Win32.TerminateThread(_thandle, 0) == 0)
            {
                MessageBox.Show("Error terminating thread!", "Process Hacker", MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
            }
            else
            {
                this.Close();   
            }
        }
    }
}
