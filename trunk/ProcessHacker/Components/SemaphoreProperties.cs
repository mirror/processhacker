using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;        
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Components
{
    public partial class SemaphoreProperties : UserControl
    {
        private SemaphoreHandle _semaphoreHandle;
        private NativeHandle<SemaphoreAccess> _dupHandle;

        public SemaphoreProperties(SemaphoreHandle semaphoreHandle)
        {
            InitializeComponent();

            _semaphoreHandle = semaphoreHandle;
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
                _dupHandle = _semaphoreHandle.Duplicate((SemaphoreAccess)StandardRights.Synchronize);
                _semaphoreHandle = SemaphoreHandle.FromHandle(_dupHandle);
                if (_semaphoreHandle.Wait(0) != NtStatus.Success)
                    throw new Exception("Could not acquire the semaphore.");
                this.UpdateInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRelease_Click(object sender, EventArgs e)
        {
            try
            {
                _dupHandle = _semaphoreHandle.Duplicate(SemaphoreAccess.QueryState | SemaphoreAccess.ModifyState);
                _semaphoreHandle = SemaphoreHandle.FromHandle(_dupHandle);
                _semaphoreHandle.Release();
                this.UpdateInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
