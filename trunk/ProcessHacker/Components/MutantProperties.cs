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
    public partial class MutantProperties : UserControl
    {
        private MutantHandle _mutantHandle;

        public MutantProperties(MutantHandle mutantHandle)
        {
            InitializeComponent();

            _mutantHandle = mutantHandle;
            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            var basicInfo = _mutantHandle.GetBasicInformation();

            labelCurrentCount.Text = basicInfo.CurrentCount.ToString();
            labelAbandoned.Text = (basicInfo.AbandonedState != 0).ToString();
        }
    }
}
