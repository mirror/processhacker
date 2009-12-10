/*
 * Process Hacker - 
 *   search results window
 * 
 * Copyright (C) 2008 wj32
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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class ResultsWindow : Form
    {
        private delegate bool Matcher(string s1, string s2);

        private int _pid;
        private SearchOptions _so;
        private Thread _searchThread;
        private int _id;

        public string Id
        {
            get { return _pid + "-" + _id; }
        }

        public ResultsWindow(int pid)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            listResults.SetDoubleBuffered(true);
            listResults.SetTheme("explorer");

            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            _pid = pid;

            _id = Program.ResultsIds.Pop();

            Program.ResultsWindows.Add(Id, this);

            if (Program.ProcessProvider.Dictionary.ContainsKey(_pid))
            {
                this.Text = Program.ProcessProvider.Dictionary[_pid].Name + " (PID " + _pid.ToString() +
                    ") - Results - " + _id.ToString();
            }
            else
            {
                this.Text = "PID " + _pid.ToString() + " - Results - " + _id.ToString();
            }

            labelText.Text = "Ready.";

            _so = new SearchOptions(_pid, SearchType.String);

            listResults.AddShortcuts(this.listResults_RetrieveVirtualItem);
        }

        private void ResultsWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindowMenu(windowMenuItem, this);

            listResults.ContextMenu = listResults.GetCopyMenu(listResults_RetrieveVirtualItem);

            this.Size = Settings.Instance.ResultsWindowSize;

            ColumnSettings.LoadSettings(Settings.Instance.ResultsListViewColumns, listResults);
            this.SetPhParent(false);
        }

        private void ResultsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;

            if (this.WindowState == FormWindowState.Normal)
                Settings.Instance.ResultsWindowSize = this.Size;

            Settings.Instance.ResultsListViewColumns = ColumnSettings.SaveSettings(listResults);
        }

        public ListView ResultsList
        {
            get { return listResults; }
        }

        public List<string[]> Results
        {
            get { return _so.Searcher.Results; }
        }

        public SearchOptions SearchOptions
        {
            get { return _so; }
            set { _so = value; }
        }

        public string Label
        {
            get { return labelText.Text; }
            set { labelText.Text = value; }
        }

        public void EditSearch()
        {
            EditSearch(_so.Type);
        }

        public DialogResult EditSearch(SearchType type)
        {
            return EditSearch(type, this.Location, this.Size);
        }

        public DialogResult EditSearch(SearchType type, System.Drawing.Point location, System.Drawing.Size size)
        {
            DialogResult dr = DialogResult.Cancel;

            _so.Type = type;

            SearchWindow sw = new SearchWindow(_pid, _so);

            sw.StartPosition = FormStartPosition.Manual;
            sw.Location = new System.Drawing.Point(
                location.X + (size.Width - sw.Width) / 2,
                location.Y + (size.Height - sw.Height) / 2);

            Rectangle newRect = Utils.FitRectangle(new Rectangle(sw.Location, sw.Size), Screen.GetWorkingArea(sw));

            sw.Location = newRect.Location;
            sw.Size = newRect.Size;

            if ((dr = sw.ShowDialog()) == DialogResult.OK)
            {
                _so = sw.SearchOptions;
            }

            return dr;
        }

        public void StartSearch()
        {
            if (_searchThread != null)
            {
                buttonFind.Enabled = false;
                _searchThread.Abort();
                _searchThread = null;

                Searcher_SearchFinished();
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;

                buttonFind.Image = global::ProcessHacker.Properties.Resources.cross;
                toolTip.SetToolTip(buttonFind, "Cancel");
                buttonEdit.Enabled = false;
                buttonFilter.Enabled = false;
                buttonIntersect.Enabled = false;
                buttonSave.Enabled = false;

                listResults.Items.Clear();
                labelText.Text = "Searching...";

                // refresh
                _so.Type = _so.Type;
                _so.Searcher.SearchFinished += new SearchFinished(Searcher_SearchFinished);
                _so.Searcher.SearchProgressChanged += new SearchProgressChanged(Searcher_SearchProgressChanged);
                _so.Searcher.SearchError += new SearchError(SearchError);

                _searchThread = new Thread(new ThreadStart(_so.Searcher.Search), Utils.SixteenthStackSize);

                _searchThread.Start();
            }
        }

        private void SearchError(string message)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                PhUtils.ShowError("Unable to search memory: " + message);
                _searchThread = null;
                Searcher_SearchFinished();
            }));
        }

        private void Searcher_SearchProgressChanged(string progress)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                labelText.Text = progress;
            }));
        }

        private void Searcher_SearchFinished()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                listResults.VirtualListSize = _so.Searcher.Results.Count;

                labelText.Text = String.Format("{0} results.", listResults.Items.Count);

                buttonFind.Image = global::ProcessHacker.Properties.Resources.arrow_refresh;
                toolTip.SetToolTip(buttonFind, "Search");
                this.Cursor = Cursors.Default;
                buttonEdit.Enabled = true;
                buttonFilter.Enabled = true;
                buttonIntersect.Enabled = true;
                buttonSave.Enabled = true;
                buttonFind.Enabled = true;
            }));

            _searchThread = null;
        }

        private void listResults_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (e.ItemIndex < _so.Searcher.Results.Count)
                    e.Item = new ListViewItem(_so.Searcher.Results[e.ItemIndex]);
                else
                    e.Item = new ListViewItem(new string[4]);
            }
            catch
            {
                e.Item = new ListViewItem(new string[4]);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string filename = "";
            DialogResult dr = DialogResult.Cancel;
            ResultsWindow rw = this;

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Text Document (*.txt)|*.txt|All Files (*.*)|*.*";
            dr = sfd.ShowDialog();
            filename = sfd.FileName;

            if (dr == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);

                foreach (string[] s in _so.Searcher.Results)
                {
                    sw.Write("0x{0:x} ({1}){2}\r\n", Int32.Parse(s[0].Replace("0x", ""),
                        System.Globalization.NumberStyles.HexNumber) + Int32.Parse(s[1].Replace("0x", ""),
                        System.Globalization.NumberStyles.HexNumber), Int32.Parse(s[2]),
                        s[3] != "" ? (": " + s[3]) : "");
                }

                sw.Close();
            }
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            StartSearch();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            EditSearch();
        }

        private void listResults_DoubleClick(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                long s_a = (long)BaseConverter.ToNumberParse(_so.Searcher.Results[listResults.SelectedIndices[0]][0]) +
                    (long)BaseConverter.ToNumberParse(_so.Searcher.Results[listResults.SelectedIndices[0]][1]);

                var lastInfo = new MemoryBasicInformation();
                ProcessHandle phandle;

                try
                {
                    phandle = new ProcessHandle(_pid, ProcessAccess.QueryInformation);
                }
                catch
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                phandle.EnumMemory((info) =>
                    {
                        if (info.BaseAddress.ToInt64() > s_a)
                        {
                            long selectlength =
                                (long)BaseConverter.ToNumberParse(_so.Searcher.Results[listResults.SelectedIndices[0]][2]);

                            MemoryEditor ed = Program.GetMemoryEditor(_pid,
                                lastInfo.BaseAddress,
                                lastInfo.RegionSize.ToInt64(),
                                new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f)
                                {
                                    try
                                    {
                                        f.ReadOnly = false;
                                        f.Activate();
                                        f.Select(s_a - lastInfo.BaseAddress.ToInt64(), selectlength);
                                    }
                                    catch
                                    { }
                                }));

                            return false;
                        }

                        lastInfo = info;

                        return true;
                    });
            }
            catch { }

            this.Cursor = Cursors.Default;
        }

        private void intersectItemClicked(object sender, EventArgs e)
        {
            List<ListViewItem> newitems = new List<ListViewItem>();
            List<long> windowitems = new List<long>();
            string id = ((MenuItem)sender).Tag.ToString();
            ResultsWindow window = Program.ResultsWindows[id];

            this.Cursor = Cursors.WaitCursor;

            foreach (string[] s in window.Results)
            {
                windowitems.Add((long)BaseConverter.ToNumberParse(s[0]) +
                    (long)BaseConverter.ToNumberParse(s[1]));
            }

            ResultsWindow rw = Program.GetResultsWindow(_pid, new Program.ResultsWindowInvokeAction(delegate(ResultsWindow f)
            {
                f.ResultsList.VirtualListSize = 0;

                foreach (string[] s in Results)
                {
                    long location = (long)BaseConverter.ToNumberParse(s[0]) +
                        (long)BaseConverter.ToNumberParse(s[1]);

                    if (windowitems.Contains(location))
                    {
                        f.Results.Add(s);
                        f.ResultsList.VirtualListSize++;
                    }
                }

                f.Label = "Intersection: " + f.Results.Count + " results.";

                f.Show();
            }));

            this.Cursor = Cursors.Default;
        }

        private void buttonIntersect_Click(object sender, EventArgs e)
        {
            // this is a bit complex because the list needs to be sorted as well
            ContextMenu menu = new ContextMenu();
            Dictionary<string, string> TextToId = new Dictionary<string, string>();
            List<string> Texts = new List<string>();

            foreach (string s in Program.ResultsWindows.Keys)
            {
                ResultsWindow window = Program.ResultsWindows[s];

                Texts.Add(window.Text);
                TextToId.Add(window.Text, s);
            }

            Texts.Sort();

            foreach (string s in Texts)
            {
                MenuItem item = new MenuItem(s);

                item.Tag = TextToId[s];
                item.Click += new EventHandler(intersectItemClicked);
                menu.MenuItems.Add(item);

                vistaMenu.SetImage(item, global::ProcessHacker.Properties.Resources.table);
            }

            menu.Show(buttonIntersect, new System.Drawing.Point(buttonIntersect.Size.Width, 0));
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            foreach (ColumnHeader ch in listResults.Columns)
            {
                MenuItem columnMenu = new MenuItem(ch.Text);
                MenuItem item;

                columnMenu.Tag = ch.Index;

                item = new MenuItem("Contains...", new EventHandler(filterMenuItem_Clicked));
                item.Tag = new Matcher(delegate(string s1, string s2)
                {
                    return s1.Contains(s2);
                });
                columnMenu.MenuItems.Add(item);

                item = new MenuItem("Contains (case-insensitive)...", new EventHandler(filterMenuItem_Clicked));
                item.Tag = new Matcher(delegate(string s1, string s2)
                {
                    return s1.ToUpperInvariant().Contains(s2.ToUpperInvariant());
                });
                columnMenu.MenuItems.Add(item);

                item = new MenuItem("Regex...", new EventHandler(filterMenuItem_Clicked));
                item.Tag = new Matcher(delegate(string s1, string s2)
                {
                    try
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(s2);

                        return r.IsMatch(s1);
                    }
                    catch
                    {
                        return false;
                    }
                });
                columnMenu.MenuItems.Add(item);

                item = new MenuItem("Regex (case-insensitive)...", new EventHandler(filterMenuItem_Clicked));
                item.Tag = new Matcher(delegate(string s1, string s2)
                {
                    try
                    {
                        System.Text.RegularExpressions.Regex r =
                            new System.Text.RegularExpressions.Regex(s2, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        return r.IsMatch(s1);
                    }
                    catch
                    {
                        return false;
                    }
                });
                columnMenu.MenuItems.Add(item);

                columnMenu.MenuItems.Add(new MenuItem("-")); 

                item = new MenuItem("Numerical relation...", new EventHandler(filterMenuItem_Clicked));
                item.Tag = new Matcher(delegate(string s1, string s2)
                {
                    if (s2.Contains("!="))
                    {
                        decimal n1 = BaseConverter.ToNumberParse(s1);
                        decimal n2 = BaseConverter.ToNumberParse(s2.Split(new string[] { "!=" }, StringSplitOptions.None)[1]);

                        return n1 != n2;
                    }
                    else if (s2.Contains("<="))
                    {
                        decimal n1 = BaseConverter.ToNumberParse(s1);
                        decimal n2 = BaseConverter.ToNumberParse(s2.Split(new string[] { "<=" }, StringSplitOptions.None)[1]);

                        return n1 <= n2;
                    }
                    else if (s2.Contains(">="))
                    {
                        decimal n1 = BaseConverter.ToNumberParse(s1);
                        decimal n2 = BaseConverter.ToNumberParse(s2.Split(new string[] { ">=" }, StringSplitOptions.None)[1]);

                        return n1 >= n2;
                    }
                    else if (s2.Contains("<"))
                    {
                        decimal n1 = BaseConverter.ToNumberParse(s1);
                        decimal n2 = BaseConverter.ToNumberParse(s2.Split(new string[] { "<" }, StringSplitOptions.None)[1]);

                        return n1 < n2;
                    }
                    else if (s2.Contains(">"))
                    {
                        decimal n1 = BaseConverter.ToNumberParse(s1);
                        decimal n2 = BaseConverter.ToNumberParse(s2.Split(new string[] { ">" }, StringSplitOptions.None)[1]);

                        return n1 > n2;
                    }
                    else if (s2.Contains("="))
                    {
                        decimal n1 = BaseConverter.ToNumberParse(s1);
                        decimal n2 = BaseConverter.ToNumberParse(s2.Split(new string[] { "=" }, StringSplitOptions.None)[1]);

                        return n1 == n2;
                    }
                    else
                    {
                        return false;
                    }
                });
                columnMenu.MenuItems.Add(item);

                menu.MenuItems.Add(columnMenu);
            }

            menu.Show(buttonFilter, new System.Drawing.Point(buttonFilter.Size.Width, 0));
        }

        private void filterMenuItem_Clicked(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            int index = (int)item.Parent.Tag;

            try
            {
                Filter(index, (Matcher)item.Tag);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to filter the search results", ex);
            }
        }

        private void Filter(int index, Matcher m)
        {
            PromptBox prompt = new PromptBox();

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                ResultsWindow rw = Program.GetResultsWindow(_pid, new Program.ResultsWindowInvokeAction(delegate(ResultsWindow f)
                {
                    f.ResultsList.VirtualListSize = 0;

                    foreach (string[] s in Results)
                    {                
                        if (m(s[index], prompt.Value))    
                        {
                            f.Results.Add(s);
                            f.ResultsList.VirtualListSize++;
                        }
                    }

                    f.Label = "Filter: " + f.Results.Count + " results.";
                           
                    f.Show();
                }));

                this.Cursor = Cursors.Default;   
            }
        }
    }
}
