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
    public partial class EventProperties : UserControl
    {
        private EventHandle _eventHandle;
        private NativeHandle<EventAccess> _dupHandle;

        public EventProperties(EventHandle eventHandle)
        {
            InitializeComponent();

            _eventHandle = eventHandle;
            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            var basicInfo = _eventHandle.GetBasicInformation();

            labelType.Text = basicInfo.EventType.ToString();
            labelSignaled.Text = (basicInfo.EventState != 0).ToString();
        }

        private void TryExecute(MethodInvoker action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpgradeExecute(MethodInvoker action)
        {
            this.TryExecute(() =>
                {
                    _dupHandle = _eventHandle.Duplicate(EventAccess.QueryState | EventAccess.ModifyState);
                    _eventHandle = EventHandle.FromHandle(_dupHandle);

                    action();
                });
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            this.UpgradeExecute(() => _eventHandle.Set());
            this.UpdateInfo();
        }

        private void buttonPulse_Click(object sender, EventArgs e)
        {
            this.UpgradeExecute(() => _eventHandle.Pulse());
            this.UpdateInfo();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.UpgradeExecute(() => _eventHandle.Clear());
            this.UpdateInfo();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.UpgradeExecute(() => _eventHandle.Reset());
            this.UpdateInfo();
        }
    }
}
