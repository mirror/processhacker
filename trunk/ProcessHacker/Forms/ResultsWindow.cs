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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

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

        public ResultsWindow(int PID)
        {
            InitializeComponent();

            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            typeof(ListView).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance).SetValue(listResults, true, null);

            _pid = PID;

            _id = Program.ResultsIds.Pop();

            Program.ResultsWindows.Add(Id, this);

            this.Text = Win32.GetNameFromPID(_pid) + " (PID " + _pid.ToString() +
                ") - Results - " + _id;

            labelText.Text = "Ready.";

            _so = new SearchOptions(_pid, SearchType.String);
        }

        private void ResultsWindow_Load(object sender, EventArgs e)
        {
            Program.UpdateWindows();

            listResults.ContextMenu = ListViewMenu.GetMenu(listResults, 
                new RetrieveVirtualItemEventHandler(listResults_RetrieveVirtualItem));

            this.Size = Properties.Settings.Default.ResultsWindowSize;

            ColumnSettings.LoadSettings(Properties.Settings.Default.ResultsListViewColumns, listResults);
        }

        private void ResultsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                Properties.Settings.Default.ResultsWindowSize = this.Size;

            Properties.Settings.Default.ResultsListViewColumns = ColumnSettings.SaveSettings(listResults);
        }

        public MenuItem WindowMenuItem
        {
            get { return windowMenuItem; }
        }

        public wyDay.Controls.VistaMenu VistaMenu
        {
            get { return vistaMenu; }
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
            System.Drawing.Rectangle workingArea = Screen.GetWorkingArea(sw);

            sw.StartPosition = FormStartPosition.Manual;
            sw.Location = new System.Drawing.Point(
                location.X + (size.Width - sw.Width) / 2,
                location.Y + (size.Height - sw.Height) / 2);

            if (sw.Location.X < workingArea.Left)
                sw.Location = new System.Drawing.Point(workingArea.Left, sw.Location.Y);
            if (sw.Location.Y < workingArea.Top)
                sw.Location = new System.Drawing.Point(sw.Location.X, workingArea.Top);

            if (sw.Location.X + sw.Size.Width > workingArea.Width)
                sw.Location = new System.Drawing.Point(workingArea.Width - sw.Size.Width, sw.Location.Y);
            if (sw.Location.Y + sw.Size.Height > workingArea.Height)
                sw.Location = new System.Drawing.Point(sw.Location.X, workingArea.Height - sw.Size.Height);

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
                buttonEdit.Enabled = false;
                buttonIntersect.Enabled = false;
                buttonSave.Enabled = false;

                listResults.Items.Clear();
                labelText.Text = "Searching...";

                // refresh
                _so.Type = _so.Type;
                _so.Searcher.SearchFinished += new SearchFinished(Searcher_SearchFinished);
                _so.Searcher.SearchProgressChanged += new SearchProgressChanged(Searcher_SearchProgressChanged);
                _so.Searcher.SearchError += new SearchError(SearchError);

                _searchThread = new Thread(new ThreadStart(_so.Searcher.Search));

                _searchThread.Start();
            }
        }

        private void SearchError(string message)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                MessageBox.Show("Error searching:\n\n" + message,
                            "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                this.Cursor = Cursors.Default;
                buttonEdit.Enabled = true;
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
                    sw.Write("0x{0:x8} ({1}){2}\r\n", Int32.Parse(s[0].Replace("0x", ""),
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
                int s_a = (int)BaseConverter.ToNumberParse(_so.Searcher.Results[listResults.SelectedIndices[0]][0]) +
                    (int)BaseConverter.ToNumberParse(_so.Searcher.Results[listResults.SelectedIndices[0]][1]);

                Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
                Win32.MEMORY_BASIC_INFORMATION info2 = new Win32.MEMORY_BASIC_INFORMATION();
                int address = 0;
                int handle = Win32.OpenProcess(Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0, _pid);

                if (handle == 0)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }

                while (true)
                {
                    if (Win32.VirtualQueryEx(handle, address, ref info,
                        Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))) == 0)
                    {
                        break;
                    }
                    else
                    {
                        if (address > s_a)
                        {
                            int selectlength = 
                                (int)BaseConverter.ToNumberParse(_so.Searcher.Results[listResults.SelectedIndices[0]][2]);

                            MemoryEditor ed = Program.GetMemoryEditor(_pid, info2.BaseAddress, info2.RegionSize,
                                new Program.MemoryEditorInvokeAction(delegate(MemoryEditor f)
                            {
                                try
                                {
                                    f.ReadOnly = false;
                                    f.Activate();
                                    f.Select(s_a - info2.BaseAddress, selectlength);
                                }
                                catch
                                { }
                            }));

                            break;
                        }

                        info2 = info;
                        address += info.RegionSize;
                    }
                }
            }
            catch { }

            this.Cursor = Cursors.Default;
        }

        private void intersectItemClicked(object sender, EventArgs e)
        {
            List<ListViewItem> newitems = new List<ListViewItem>();
            List<int> windowitems = new List<int>();
            string id = ((MenuItem)sender).Tag.ToString();
            ResultsWindow window = Program.ResultsWindows[id];

            this.Cursor = Cursors.WaitCursor;

            foreach (string[] s in window.Results)
            {
                windowitems.Add((int)BaseConverter.ToNumberParse(s[0]) +
                    (int)BaseConverter.ToNumberParse(s[1]));
            }

            ResultsWindow rw = Program.GetResultsWindow(_pid, new Program.ResultsWindowInvokeAction(delegate(ResultsWindow f)
            {
                f.ResultsList.VirtualListSize = 0;

                foreach (string[] s in Results)
                {
                    int location = (int)BaseConverter.ToNumberParse(s[0]) +
                        (int)BaseConverter.ToNumberParse(s[1]);

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
                    return s1.ToLower().Contains(s2.ToLower());
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
                MessageBox.Show("Error filtering:\n\n" + ex.Message, "Process Hacker",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
