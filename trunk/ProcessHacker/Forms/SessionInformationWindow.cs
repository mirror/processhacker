using System;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;

namespace ProcessHacker
{
    public partial class SessionInformationWindow : Form
    {
        public SessionInformationWindow(TerminalServerSession session)
        {
            InitializeComponent();
            this.AddEscapeToClose();

            labelUsername.Text = session.DomainName + "\\" + session.UserName;
            labelSessionId.Text = session.SessionId.ToString();
            labelState.Text = session.State.ToString();

            if (!string.IsNullOrEmpty(session.ClientName))
                labelClientName.Text = session.ClientName;
            if (session.ClientAddress != null)
                labelClientAddress.Text = session.ClientAddress.ToString();
            if (session.ClientDisplay.ColorDepth != 0 && 
                session.ClientDisplay.ColorDepth != 2) // HACK
                labelClientDisplayResolution.Text =
                    session.ClientDisplay.HorizontalResolution + "x" +
                    session.ClientDisplay.VerticalResolution + "@" +
                    session.ClientDisplay.ColorDepth;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
