using System;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Components
{
    public partial class EventProperties : UserControl
    {
        private EventHandle _eventHandle;

        public EventProperties(EventHandle eventHandle)
        {
            InitializeComponent();

            _eventHandle = eventHandle;
            _eventHandle.Reference();
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
                PhUtils.ShowException("Unable to perform the operation", ex);
            }
        }

        private void UpgradeExecute(MethodInvoker action)
        {
            this.TryExecute(() =>
                {
                    _eventHandle.ChangeAccess(EventAccess.QueryState | EventAccess.ModifyState);

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
