/*
 * Process Hacker - 
 *   search options window
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class SearchWindow : Form
    {
        private SearchOptions _so;
        private List<string[]> _oldresults;
        private int _pid;

        public SearchWindow(int PID, SearchOptions so)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            foreach (string s in Program.Structs.Keys)
                listStructName.Items.Add(s);

            if (listStructName.Items.Count > 0)
                listStructName.SelectedItem = listStructName.Items[0];

            _pid = PID;

            hexBoxSearch.ByteProvider = new Be.Windows.Forms.DynamicByteProvider((byte[])so.Searcher.Params["text"]);
            textRegex.Text = (string)so.Searcher.Params["regex"];
            textStringMS.Text = (string)so.Searcher.Params["s_ms"];
            checkUnicode.Checked = (bool)so.Searcher.Params["unicode"];
            textHeapMS.Text = (string)so.Searcher.Params["h_ms"];
            checkNoOverlap.Checked = (bool)so.Searcher.Params["nooverlap"];
            checkIgnoreCase.Checked = (bool)so.Searcher.Params["ignorecase"];
            checkPrivate.Checked = (bool)so.Searcher.Params["private"];
            checkImage.Checked = (bool)so.Searcher.Params["image"];
            checkMapped.Checked = (bool)so.Searcher.Params["mapped"];
            listStructName.SelectedItem = so.Searcher.Params["struct"];
            textStructAlign.Text = (string)so.Searcher.Params["struct_align"];

            switch (so.Type)
            {
                case SearchType.Literal:
                    tabControl.SelectedTab = tabLiteral;
                    break;
                case SearchType.Regex:
                    tabControl.SelectedTab = tabRegex;
                    break;
                case SearchType.String:
                    tabControl.SelectedTab = tabString;
                    break;
                case SearchType.Heap:
                    tabControl.SelectedTab = tabHeap;
                    break;
                case SearchType.Struct:
                    tabControl.SelectedTab = tabStruct;
                    break;
            }

            _oldresults = so.Searcher.Results;

            FocusTab();
        }

        public SearchOptions SearchOptions
        {
            get { return _so; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            byte[] text = new byte[hexBoxSearch.ByteProvider.Length];

            for (int i = 0; i < hexBoxSearch.ByteProvider.Length; i++)
                text[i] = hexBoxSearch.ByteProvider.ReadByte(i);

            if (tabControl.SelectedTab == tabLiteral)
            {
                _so = new SearchOptions(_pid, SearchType.Literal);
            }
            else if (tabControl.SelectedTab == tabRegex)
            {
                _so = new SearchOptions(_pid, SearchType.Regex);
            }
            else if (tabControl.SelectedTab == tabString)
            {
                _so = new SearchOptions(_pid, SearchType.String);
            }
            else if (tabControl.SelectedTab == tabHeap)
            {
                _so = new SearchOptions(_pid, SearchType.Heap);
            }
            else if (tabControl.SelectedTab == tabStruct)
            {
                _so = new SearchOptions(_pid, SearchType.Struct);
            }

            _so.Searcher.Params["text"] = text;
            _so.Searcher.Params["regex"] = textRegex.Text;
            _so.Searcher.Params["s_ms"] = textStringMS.Text;
            _so.Searcher.Params["unicode"] = checkUnicode.Checked;
            _so.Searcher.Params["h_ms"] = textHeapMS.Text;
            _so.Searcher.Params["nooverlap"] = checkNoOverlap.Checked;
            _so.Searcher.Params["ignorecase"] = checkIgnoreCase.Checked;
            _so.Searcher.Params["private"] = checkPrivate.Checked;
            _so.Searcher.Params["image"] = checkImage.Checked;
            _so.Searcher.Params["mapped"] = checkMapped.Checked;
            if (listStructName.SelectedItem != null)
                _so.Searcher.Params["struct"] = listStructName.SelectedItem.ToString();
            _so.Searcher.Params["struct_align"] = textStructAlign.Text;

            _so.Searcher.Results = _oldresults;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            FocusTab();
        }

        private void FocusTab()
        {
            if (tabControl.SelectedTab != tabLiteral && tabControl.SelectedTab != tabRegex)
                this.AcceptButton = buttonOK;
            else
                this.AcceptButton = null;

            if (tabControl.SelectedTab == tabLiteral)
            {
                hexBoxSearch.Select();
            }
            else if (tabControl.SelectedTab == tabRegex)
            {
                // HACK
                textRegex.Focus();
                textRegex.Select();
                textRegex.Select(textRegex.Text.Length, 0);
                textRegex.Focus();
            }
            else if (tabControl.SelectedTab == tabString)
            {
                textStringMS.SelectAll();
                textStringMS.Select();
            }
            else if (tabControl.SelectedTab == tabHeap)
            {
                textHeapMS.SelectAll();
                textHeapMS.Select();
            }
        }
    }
}
