using System;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Components
{
    public partial class TmRmProperties : UserControl
    {
        private readonly ResourceManagerHandle _rmHandle;

        public TmRmProperties(ResourceManagerHandle rmHandle)
        {
            InitializeComponent();

            _rmHandle = rmHandle;
            _rmHandle.Reference();

            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            try
            {
                textDescription.Text = _rmHandle.Description;
                textGuid.Text = _rmHandle.Guid.ToString("B");
            }
            catch (Exception ex)
            {
                textDescription.Text = "(" + ex.Message + ")";
                textGuid.Text = "(" + ex.Message + ")";
            }
        }
    }
}
