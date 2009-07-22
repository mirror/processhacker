using System;
using System.Windows.Forms;
using ProcessHacker.Common;        
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Components
{
    public partial class SemaphoreProperties : UserControl
    {
        private SemaphoreHandle _semaphoreHandle;

        public SemaphoreProperties(SemaphoreHandle semaphoreHandle)
        {
            InitializeComponent();

            _semaphoreHandle = semaphoreHandle;
            _semaphoreHandle.Reference();

            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            var basicInfo = _semaphoreHandle.GetBasicInformation();

            labelCurrentCount.Text = basicInfo.CurrentCount.ToString();
            labelMaximumCount.Text = basicInfo.MaximumCount.ToString();
        }

        private void buttonAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                _semaphoreHandle.ChangeAccess((SemaphoreAccess)StandardRights.Synchronize);
                // Try to acquire the semaphore. We don't want to wait on it though, 
                // so we specify a timeout of 0.
                if (_semaphoreHandle.Wait(0) != NtStatus.Success)
                    throw new Exception("Could not acquire the semaphore.");
                this.UpdateInfo();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to acquire the semaphore", ex);
            }
        }

        private void buttonRelease_Click(object sender, EventArgs e)
        {
            try
            {
                _semaphoreHandle.ChangeAccess(SemaphoreAccess.QueryState | SemaphoreAccess.ModifyState);
                _semaphoreHandle.Release();
                this.UpdateInfo();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to release the semaphore", ex);
            }
        }
    }
}
