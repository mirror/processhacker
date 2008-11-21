using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ErrorDialog : Form
    {
        public ErrorDialog(Exception ex)
        {
            InitializeComponent();

            textException.Text = ex.ToString();
        }

        private void TryStart(string command)
        {
            try
            {
                Process.Start(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start process:\n\n" + ex.Message, "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void labelLink_Click(object sender, EventArgs e)
        {
            TryStart("http://sourceforge.net/projects/processhacker");
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
