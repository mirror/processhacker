using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ScratchpadWindow : Form
    {
        public static void Create(string text)
        {
            // Create the window on the main thread.
            Program.HackerWindow.BeginInvoke(new MethodInvoker(delegate
                {
                    (new ScratchpadWindow(text)).Show();
                }));
        }

        public ScratchpadWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();
        }

        public ScratchpadWindow(string text)
        {
            InitializeComponent();

            textText.Text = text;
            textText.Select(0, 0);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (textText.Text.Length == 0)
                return;

            if (textText.SelectionLength == 0)
            {
                Clipboard.SetText(textText.Text);
                textText.Select();
                textText.SelectAll();
            }
            else
            {
                Clipboard.SetText(textText.SelectedText);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "scratchpad.txt";
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
                System.IO.File.WriteAllText(sfd.FileName, textText.Text);
        }

        private void ScratchpadWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                textText.SelectAll();
                e.Handled = true;
            }
        }
    }
}
