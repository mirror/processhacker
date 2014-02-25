using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Components
{
    public partial class TmRmProperties : UserControl
    {
        private ResourceManagerHandle _rmHandle;

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
                textDescription.Text = _rmHandle.GetDescription();
                textGuid.Text = _rmHandle.GetGuid().ToString("B");
            }
            catch (Exception ex)
            {
                textDescription.Text = "(" + ex.Message + ")";
                textGuid.Text = "(" + ex.Message + ")";
            }
        }
    }
}
