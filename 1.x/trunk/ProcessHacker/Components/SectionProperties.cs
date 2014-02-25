using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Components
{
    public partial class SectionProperties : UserControl
    {
        private SectionHandle _sectionHandle;

        public SectionProperties(SectionHandle sectionHandle)
        {
            InitializeComponent();

            _sectionHandle = sectionHandle;
            _sectionHandle.Reference();

            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            var basicInfo = _sectionHandle.GetBasicInformation();

            labelAttributes.Text = basicInfo.SectionAttributes.ToString();
            labelSize.Text = Utils.FormatSize(basicInfo.SectionSize);
        }
    }
}
