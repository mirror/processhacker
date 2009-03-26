using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BugCheck
{
    public partial class BugCheckWindow : Form
    {
        private KBugCheck _KBugCheck;

        public BugCheckWindow()
        {
            InitializeComponent();

            try
            {
                _KBugCheck = new KBugCheck("KBugCheck");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}
