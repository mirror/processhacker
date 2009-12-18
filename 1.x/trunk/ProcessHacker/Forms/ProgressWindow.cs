using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ProgressWindow : Form
    {
        public event EventHandler CloseButtonClick
        {
            add { buttonClose.Click += value; }
            remove { buttonClose.Click -= value; }
        }

        private bool _completed = false;

        public ProgressWindow()
        {
            InitializeComponent();

            this.CloseButtonVisible = true;
        }

        public bool CloseButtonEnabled
        {
            get { return buttonClose.Enabled; }
            set { buttonClose.Enabled = value; }
        }

        public string CloseButtonText
        {
            get { return buttonClose.Text; }
            set { buttonClose.Text = value; }
        }

        public bool CloseButtonVisible
        {
            get { return buttonClose.Visible; }
            set
            {
                buttonClose.Visible = value;

                if (value)
                    progressBar.Width = this.Width - progressBar.Left -
                        buttonClose.Right - buttonClose.Width - 20;
                else
                    progressBar.Width = this.Width - progressBar.Left - 20;
            }
        }

        public ProgressBarStyle ProgressBarStyle
        {
            get { return progressBar.Style; }
            set { progressBar.Style = value; }
        }

        public int ProgressBarMinimum
        {
            get { return progressBar.Minimum; }
            set { progressBar.Minimum = value; }
        }

        public int ProgressBarMaximum
        {
            get { return progressBar.Maximum; }
            set { progressBar.Maximum = value; }
        }

        public int ProgressBarValue
        {
            get { return progressBar.Value; }
            set { progressBar.Value = value; }
        }

        public string ProgressText
        {
            get { return labelProgressText.Text; }
            set { labelProgressText.Text = value; }
        }

        private void ProgressWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_completed;
        }

        public void SetCompleted()
        {
            _completed = true;
        }
    }
}
