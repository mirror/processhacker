using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class EditDEPWindow : Form
    {
        private int _pid;

        public EditDEPWindow(int PID)
        {
            InitializeComponent();

            _pid = PID;

            try
            {
                using (Win32.ProcessHandle phandle
                  = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION))
                {
                    var depStatus = phandle.GetDEPStatus();
                    string str;

                    if ((depStatus & Win32.ProcessHandle.DEPStatus.Enabled) != 0)
                    {
                        str = "Enabled";

                        if ((depStatus & Win32.ProcessHandle.DEPStatus.ATLThunkEmulationDisabled) != 0)
                            str += ", DEP-ATL thunk emulation disabled";
                    }
                    else
                    {
                        str = "Disabled";
                    }

                    comboStatus.SelectedItem = str;
                }
            }
            catch
            { }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboStatus.SelectedItem.ToString().StartsWith("Enabled"))
                if (
                    MessageBox.Show("Are you sure you want to set the DEP status of the process? This action is permanent.",
                    "Process Hacker", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return;

            Win32.DEPFLAGS flags = Win32.DEPFLAGS.PROCESS_DEP_ENABLE;

            if (comboStatus.SelectedItem.ToString() == "Disabled")
                flags = Win32.DEPFLAGS.PROCESS_DEP_DISABLE;
            else if (comboStatus.SelectedItem.ToString() == "Enabled")
                flags = Win32.DEPFLAGS.PROCESS_DEP_ENABLE;
            else if (comboStatus.SelectedItem.ToString() == "Enabled, DEP-ATL thunk emulation disabled")
                flags = Win32.DEPFLAGS.PROCESS_DEP_ENABLE | Win32.DEPFLAGS.PROCESS_DEP_DISABLE_ATL_THUNK_EMULATION;
            else
            {
                MessageBox.Show("Invalid value!", "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int kernel32 = Win32.GetModuleHandle("kernel32.dll");
                int setProcessDEPPolicy = Win32.GetProcAddress(kernel32, "SetProcessDEPPolicy");

                if (setProcessDEPPolicy == 0)
                    throw new Exception("This feature is not supported on your version of Windows.");

                using (Win32.ProcessHandle phandle = new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_CREATE_THREAD))
                    phandle.CreateThread(setProcessDEPPolicy, (int)flags);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
