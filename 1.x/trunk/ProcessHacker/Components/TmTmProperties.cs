using System;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native;

namespace ProcessHacker.Components
{
    public partial class TmTmProperties : UserControl
    {
        private readonly TmHandle _tmHandle;

        public TmTmProperties(TmHandle tmHandle)
        {
            InitializeComponent();

            _tmHandle = tmHandle;
            _tmHandle.Reference();

            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            try
            {
                textGuid.Text = _tmHandle.BasicInformation.TmIdentity.ToString("B");
                textLogFileName.Text = FileUtils.GetFileName(FileUtils.GetFileName(_tmHandle.LogFileName));
            }
            catch (Exception ex)
            {
                textGuid.Text = "(" + ex.Message + ")";
                textLogFileName.Text = "(" + ex.Message + ")";
            }
        }
    }
}
