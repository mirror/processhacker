/*
 * Process Hacker - 
 *   help window
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
                                         new string[] {"Process Tree", "proctree"},    
                                         new string[] {"Process Properties", "procprops"},
                                         new string[] {"Searching Memory", "memsearch"},
                                         new string[] {"Results Window", "results"},
                                         new string[] {"Glossary", "glossary"},
                                         new string[] {"Copyright Information", "copyright"}
                                     };

        public HelpWindow()
        {
            InitializeComponent();
            this.AddEscapeToClose();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resource = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Help.htm");

            webBrowser.DocumentStream = resource;

            foreach (string[] s in contents)
            {
                listBoxContents.Items.Add(s[0]);
            }

            webBrowser.Navigating += (sender, e) => e.Cancel = true;
        }

        private void listBoxContents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContents.SelectedItems.Count == 1)
            {
                foreach (string[] s in contents)
                {
                    if (s[0] == listBoxContents.SelectedItem.ToString())
                    {
                        HtmlElement element = webBrowser.Document.GetElementById(s[1]);

                        if (element != null)
                            element.ScrollIntoView(true);

                        break;
                    }
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

        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // HACK
            if (e.KeyData != Keys.F5)
            {
                webBrowser.WebBrowserShortcutsEnabled = true;
            }
            else
            {
                webBrowser.WebBrowserShortcutsEnabled = false;
            }
        }
    }
}
