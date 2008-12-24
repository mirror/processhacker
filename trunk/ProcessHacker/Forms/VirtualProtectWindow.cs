using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class VirtualProtectWindow : Form
    {
        private int _pid, _address, _size;

        public VirtualProtectWindow(int pid, int address, int size)
        {
            InitializeComponent();

            _pid = pid;
            _address = address;
            _size = size;
        }

        private void textNewProtection_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = buttonVirtualProtect;
        }

        private void buttonCloseVirtualProtect_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonVirtualProtect_Click(object sender, EventArgs e)
        {
            try
            {
                int old = 0;
                int newprotect;

                try
                {
                    newprotect = (int)BaseConverter.ToNumberParse(textNewProtection.Text);
                }
                catch
                {
                    return;
                }

                using (Win32.ProcessHandle phandle =
                    new Win32.ProcessHandle(_pid, Win32.PROCESS_RIGHTS.PROCESS_VM_OPERATION))
                {
                    if (!Win32.VirtualProtectEx(phandle, _address,
                        _size, newprotect, out old))
                    {
                        MessageBox.Show("There was an error setting memory protection:\n\n" +
                            Win32.GetLastErrorMessage(), "Process Hacker",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting memory protection:\n\n" + ex.Message, "Process Hacker",
                 MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textNewProtection_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = null;
        }
    }
}
