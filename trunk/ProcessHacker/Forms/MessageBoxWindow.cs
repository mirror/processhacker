using System.Windows.Forms;

namespace ProcessHacker
{
    public delegate bool DialogButtonClickedDelegate();

    public partial class MessageBoxWindow : Form
    {
        public event DialogButtonClickedDelegate OkButtonClicked;

        public MessageBoxWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();

            comboIcon.SelectedItem = "None";
        }

        public MessageBoxIcon MessageBoxIcon
        {
            get
            {
                switch (comboIcon.SelectedItem.ToString())
                {
                    case "None":
                        return MessageBoxIcon.None;
                    case "Error":
                        return MessageBoxIcon.Error;
                    case "Information":
                        return MessageBoxIcon.Information;
                    case "Question":
                        return MessageBoxIcon.Question;
                    case "Warning":
                        return MessageBoxIcon.Warning;
                    default:
                        return MessageBoxIcon.None;
                }
            }
        }

        public string MessageBoxText
        {
            get { return textText.Text; }
            set { textText.Text = value; }
        }

        public int MessageBoxTimeout
        {
            get
            {
                int timeout = 0;

                int.TryParse(textTimeout.Text, out timeout);

                return timeout;
            }
        }

        public string MessageBoxTitle
        {
            get { return textTitle.Text; }
            set { textTitle.Text = value; }
        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (OkButtonClicked != null)
            {
                if (OkButtonClicked())
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void buttonCancel_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
