using System;
using System.Windows.Forms;
using ProcessHacker.Components;
using ProcessHacker.Native.Objects;

namespace ProcessHacker
{
    public partial class JobWindow : Form
    {
        readonly JobProperties _jobProps;

        public JobWindow(JobObjectHandle jobHandle)
        {
            InitializeComponent();

            this.AddEscapeToClose();
            this.SetTopMost();

            _jobProps = new JobProperties(jobHandle)
            {
                Dock = DockStyle.Fill
            };

            panelJob.Controls.Add(_jobProps);
        }

        private void JobWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _jobProps.SaveSettings();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
