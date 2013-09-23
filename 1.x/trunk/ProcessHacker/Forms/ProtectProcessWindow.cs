using System;
using System.Windows.Forms;
using ProcessHacker.Native.Security;

namespace ProcessHacker
{
    public partial class ProtectProcessWindow : Form
    {
        private readonly int _pid;
        //private readonly bool _isProtected;

        public ProtectProcessWindow(int pid)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _pid = pid;

            //bool allowKernelMode;
            //ProcessAccess processAccess;
            //ThreadAccess threadAccess;

            //if (ProtectQuery(_pid, out allowKernelMode, out processAccess, out threadAccess))
            //{
            //    checkProtect.Checked = _isProtected = true;
            //    checkDontAllowKernelMode.Checked = !allowKernelMode;
            //}

            foreach (string value in Enum.GetNames(typeof(ProcessAccess)))
            {
                if (value == "All")
                    continue;

                //listProcessAccess.Items.Add(value,
                    //(processAccess & (ProcessAccess)Enum.Parse(typeof(ProcessAccess), value)) != 0);
            }

            foreach (string value in Enum.GetNames(typeof(ThreadAccess)))
            {
                if (value == "All")
                    continue;

                //listThreadAccess.Items.Add(value,
                    //(threadAccess & (ThreadAccess)Enum.Parse(typeof(ThreadAccess), value)) != 0);
            }

            checkProtect_CheckedChanged(null, null);
        }

        //private bool ProtectQuery(int pid, out bool allowKernelMode, out ProcessAccess processAccess, out ThreadAccess threadAccess)
        //{
        //    try
        //    {
                //using (ProcessHandle phandle = new ProcessHandle(pid, Program.MinProcessQueryRights))
                    //KProcessHacker.Instance.ProtectQuery(phandle, out allowKernelMode, out processAccess, out threadAccess);

        //        return true;
        //    }
        //    catch
        //    {
        //        allowKernelMode = true;
        //        processAccess = 0;
        //        threadAccess = 0;

        //        return false;
        //    }
        //}

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // remove protection
            //if (_isProtected)
            //{
            //    try
            //    {
            //        //using (ProcessHandle phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
            //            //KProcessHacker.Instance.ProtectRemove(phandle);
            //    }
            //    catch
            //    { }
            //}

            // re-add protection (with new masks)
            if (checkProtect.Checked)
            {
                ProcessAccess processAccess = 0;
                ThreadAccess threadAccess = 0;

                foreach (string value in listProcessAccess.CheckedItems)
                    processAccess |= (ProcessAccess)Enum.Parse(typeof(ProcessAccess), value);
                foreach (string value in listThreadAccess.CheckedItems)
                    threadAccess |= (ThreadAccess)Enum.Parse(typeof(ThreadAccess), value);

                try
                {
                    //using (ProcessHandle phandle = new ProcessHandle(_pid, Program.MinProcessQueryRights))
                        //KProcessHacker.Instance.ProtectAdd(
                            //phandle,
                            //!checkDontAllowKernelMode.Checked,
                            //processAccess, 
                            //threadAccess
                            //);
                }
                catch
                { }
            }

            this.Close();
        }

        private void checkProtect_CheckedChanged(object sender, EventArgs e)
        {
            checkDontAllowKernelMode.Enabled = checkProtect.Checked;
            listProcessAccess.Enabled = checkProtect.Checked;
            listThreadAccess.Enabled = checkProtect.Checked;
        }
    }
}
