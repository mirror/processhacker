using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native.Objects; 
using ProcessHacker.Native.Security;

namespace ProcessHacker.Components
{
    public partial class EventPairProperties : UserControl
    {
        private EventPairHandle _eventPairHandle;

        public EventPairProperties(EventPairHandle eventPairHandle)
        {
            InitializeComponent();

            _eventPairHandle = eventPairHandle;
        }

        private void buttonSetHigh_Click(object sender, EventArgs e)
        {
            try
            {
                _eventPairHandle.SetHigh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSetLow_Click(object sender, EventArgs e)
        {
            try
            {
                _eventPairHandle.SetLow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
