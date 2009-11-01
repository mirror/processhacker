using System;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Components
{
    public partial class TimerProperties : UserControl
    {
        private TimerHandle _timerHandle;

        public TimerProperties(TimerHandle timerHandle)
        {
            InitializeComponent();

            _timerHandle = timerHandle;
            _timerHandle.Reference();

            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            try
            {
                var basicInfo = _timerHandle.GetBasicInformation();

                labelSignaled.Text = basicInfo.TimerState.ToString();
                labelTimeRemaining.Text = (new TimeSpan(-basicInfo.RemainingTime)).ToString();
            }
            catch (Exception ex)
            {
                labelSignaled.Text = "(" + ex.Message + ")";
                labelTimeRemaining.Text = "(" + ex.Message + ")";
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                _timerHandle.ChangeAccess(TimerAccess.QueryState | TimerAccess.ModifyState);
                _timerHandle.Cancel();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to cancel the timer", ex);
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            this.UpdateInfo();
        }
    }
}
