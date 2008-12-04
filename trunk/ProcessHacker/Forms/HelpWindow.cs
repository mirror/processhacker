/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class HelpWindow : Form
    {
        public string[][] contents = {
                                         new string[] {"Introduction", "intro"},
                                         new string[] {"Options", "options"},
                                         new string[] {"Number Input", "numberinput"},
                                         new string[] {"Process List", "proclist"},
                                         new string[] {"Thread List", "threadlist"},
                                         new string[] {"Modules List", "modlist"},
                                         new string[] {"Memory List", "memlist"},  
                                         new string[] {"Handle List", "hlist"},
                                         new string[] {"Searching Memory", "memsearch"},
                                         new string[] {"Results Window", "results"},
                                         new string[] {"Tips", "tips"},
                                         new string[] {"Copyright Information", "copyright"}
                                     };

        public HelpWindow()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resource = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Help.htm");

            webBrowser.DocumentStream = resource;

            foreach (string[] s in contents)
            {
                listBoxContents.Items.Add(s[0]);
            }
        }

        private void listBoxContents_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (string[] s in contents)
            {
                if (s[0] == listBoxContents.SelectedItem.ToString())
                {
                    webBrowser.Document.GetElementById(s[1]).ScrollIntoView(true);
                    break;
                }
            }
        }

        public void SelectById(string id)
        {
            webBrowser.Document.GetElementById(id).ScrollIntoView(true);
        }

        private void HelpWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }
    }
}
