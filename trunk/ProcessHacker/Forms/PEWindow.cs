using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.PE;

namespace ProcessHacker
{
    public partial class PEWindow : Form
    {
        private string _path;
        private PEFile _peFile;

        public PEWindow(string path)
        {
            InitializeComponent();

            _path = path;

            if (!this.Read(path))
            {
                this.Close();
            }
        }

        public string Id
        {
            get { return _path; } 
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
        }

        private bool Read(string path)
        {
            PEFile peFile;
     
            try
            {
                peFile = new PEFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading the specified file:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            _peFile = peFile;

            return true;
        }
    }
}
