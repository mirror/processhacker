using System.Windows.Forms;
using ProcessHacker.Native;
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
            _mutantHandle.Reference();

            this.UpdateInfo();
        }

        private void UpdateInfo()
        {
            var basicInfo = _mutantHandle.GetBasicInformation();

            labelCurrentCount.Text = basicInfo.CurrentCount.ToString();
            labelAbandoned.Text = basicInfo.AbandonedState.ToString();

            // Windows Vista and above have owner information.
            if (OSVersion.IsAboveOrEqual(WindowsVersion.Vista))
            {
                var ownerInfo = _mutantHandle.GetOwnerInformation();

                if (ownerInfo.ClientId.ProcessId != 0)
                {
                    labelOwner.Text = ownerInfo.ClientId.GetName(true);
                }
                else
                {
                    labelOwner.Text = "N/A";
                }

                labelLabelOwner.Visible = true;
                labelOwner.Visible = true;
            }
            else
            {
                labelLabelOwner.Visible = false;
                labelOwner.Visible = false;
            }
        }
    }
}
