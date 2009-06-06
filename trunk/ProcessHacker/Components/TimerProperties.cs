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
        private NativeHandle<TimerAccess> _dupHandle;

        public TimerProperties(TimerHandle timerHandle)
        {
            InitializeComponent();

            _timerHandle = timerHandle;
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
                _dupHandle = _timerHandle.Duplicate(TimerAccess.QueryState | TimerAccess.ModifyState);
                _timerHandle = TimerHandle.FromHandle(_dupHandle);
                _timerHandle.Cancel();
            }
            catch (Exception ex)
            {
                PhUtils.ShowMessage(ex);
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            this.UpdateInfo();
        }
    }
}
