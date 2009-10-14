/*
 * Process Hacker - 
 *   about window
 * 
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;
using ProcessHacker.Common;

namespace ProcessHacker
{
    partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            labelVersion.Text = Application.ProductVersion;

            buttonChangelog.Visible = System.IO.File.Exists(Application.StartupPath + "\\CHANGELOG.txt");
        }

        private void flowCredits_MouseEnter(object sender, EventArgs e)
        {
            flowCredits.Select();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonChangelog_Click(object sender, EventArgs e)
        {
            try
            {
                InformationBox box = new InformationBox(System.IO.File.ReadAllText(Application.StartupPath + "\\CHANGELOG.txt"));

                box.ShowSaveButton = false;
                box.Title = "Process Hacker Changelog";
                box.ShowDialog();
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to view the changelog", ex);
            }
        }

        private void buttonDiagnostics_Click(object sender, EventArgs e)
        {
            InformationBox box = new InformationBox(Program.GetDiagnosticInformation());
            box.DefaultFileName = "Process Hacker Diagnostics Info";    
            box.ShowDialog();
        }

        private void linkHexBox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://sourceforge.net/projects/hexbox");
        }

        private void linkVistaMenu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://wyday.com");
        }

        private void linkFamFamFam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://www.famfamfam.com/lab/icons/silk/");
        }

        private void linkSourceforge_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://sourceforge.net/projects/processhacker");
        }

        private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://sourceforge.net/tracker2/?group_id=242527");
        }

        private void linkAsm_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://www.ollydbg.de");
        }

        private void linkTreeViewAdv_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://sourceforge.net/projects/treeviewadv");
        }

        private void linkICSharpCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://www.icsharpcode.net");
        }

        private void linkTaskDialog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://www.codeproject.com/KB/vista/TaskDialogWinForms.aspx");
        }

        private void linkSysinternals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://forum.sysinternals.com");
        }

        private void linkNtInternals_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://undocumented.ntinternals.net");
        }

        private void linkReactOS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://www.reactos.org");
        }

        private void linkGamingMasteR_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://www.at4re.com/download.php?view.1");
        }

        private void linkKerem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Program.TryStart("http://entwicklung.junetz.de");
        }
    }
}
