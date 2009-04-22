/*
 * Process Hacker - 
 *   options window
 * 
 * Copyright (C) 2008 Dean
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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Components;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;

namespace ProcessHacker
{
    public partial class OptionsWindow : Form
    {
        private string _oldDbghelp;
        private string _oldTaskMgrDebugger;
        private Font _font;
        private bool _dontApply;

        public OptionsWindow()
            : this(false)
        { }

        public OptionsWindow(bool dontApply)
        {
            InitializeComponent();

            _dontApply = dontApply;
            this.InitializeHighlightingColors();
            this.LoadSettings();

            foreach (TabPage tab in tabControl.TabPages)
            {
                foreach (Control c in tab.Controls)
                {
                    if (c is CheckBox || c is ListView)
                        c.Click += (sender, e) => this.EnableApplyButton();
                    else if (c is TextBox)
                        (c as TextBox).TextChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is ComboBox)
                        (c as ComboBox).SelectedIndexChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is NumericUpDown)
                        (c as NumericUpDown).ValueChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is ColorModifier)
                        (c as ColorModifier).ColorChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is Button || c is Label)
                        ; // Nothing
                    else
                        c.Click += (sender, e) => this.EnableApplyButton();
                }
            }
        }

        public TabPage SelectedTab
        {
            get { return tabControl.SelectedTab; }
            set { tabControl.SelectedTab = value; }
        }

        public TabControl.TabPageCollection TabPages
        {
            get { return tabControl.TabPages; }
        }

        private void OptionsWindow_Load(object sender, EventArgs e)
        {
            if (Program.ElevationType == TokenElevationType.Limited)
            {
                buttonChangeReplaceTaskManager.SetShieldIcon(true);
            }
            else
            {
                buttonChangeReplaceTaskManager.Visible = false;
            }
        }

        private void OptionsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = !buttonCancel.Enabled;
        }

        private void EnableApplyButton()
        {
            if (!_dontApply)
                buttonApply.Enabled = true;
        }

        private void AddToList(string key, string description, string longDescription)
        {
            listHighlightingColors.Items.Add(new ListViewItem()
                {
                    Name = key,
                    Text = description,
                    ToolTipText = longDescription
                });
        }

        private void InitializeHighlightingColors()
        {
            AddToList("ColorOwnProcesses", "Own Processes",
                "Processes running under the same user account as Process Hacker.");
            AddToList("ColorSystemProcesses", "System Processes",
                "Processes running under the NT AUTHORITY\\SYSTEM user account.");
            AddToList("ColorServiceProcesses", "Service Processes",
                "Processes which host one or more services.");
            AddToList("ColorDebuggedProcesses", "Debugged Processes",
                "Processes that are currently being debugged.");
            AddToList("ColorElevatedProcesses", "Elevated Processes",
                "Processes with full privileges on a Windows Vista system with UAC enabled.");
            AddToList("ColorJobProcesses", "Job Processes",
                "Processes associated with a job.");
            AddToList("ColorDotNetProcesses", ".NET Processes",
                ".NET, or managed processes.");
            AddToList("ColorPackedProcesses", "Packed/Dangerous Processes",
                "Executables are sometimes \"packed\" to reduce their size.\n" +
                "\"Dangerous processes\" includes processes with invalid signatures and unverified " + 
                "processes with the name of a system process.");
            AddToList("ColorSuspended", "Suspended Threads",
                "Threads that are suspended from execution.");
            AddToList("ColorGuiThreads", "GUI Threads",
                "Threads that have made at least one GUI-related system call.");
            AddToList("ColorProtectedHandles", "Protected Handles",
                "Handles that are protected from being closed.");
            AddToList("ColorInheritHandles", "Inherit Handles",
                "Handles that are to be inherited by any child processes.");
        }

        private void listHighlightingColors_DoubleClick(object sender, EventArgs e)
        {
            listHighlightingColors.SelectedItems[0].Checked = !listHighlightingColors.SelectedItems[0].Checked;

            ColorDialog cd = new ColorDialog();

            cd.Color = listHighlightingColors.SelectedItems[0].BackColor;

            if (cd.ShowDialog() == DialogResult.OK)
                listHighlightingColors.SelectedItems[0].BackColor = cd.Color;
        }

        private void textUpdateInterval_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.RefreshInterval = Int32.Parse(textUpdateInterval.Value.ToString());
            }
            catch
            {
                MessageBox.Show("The entered value is not valid.", "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                textUpdateInterval.Focus();
            }
        }

        private void textIconMenuProcesses_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.IconMenuProcessCount = Int32.Parse(textIconMenuProcesses.Value.ToString());
            }
            catch
            {
                MessageBox.Show("The entered value is not valid.", "Process Hacker", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                textIconMenuProcesses.Focus();
            }
        }

        private void LoadSettings()
        {
            // General
            _font = Properties.Settings.Default.Font;
            buttonFont.Font = _font;
            textUpdateInterval.Value = Properties.Settings.Default.RefreshInterval;
            textIconMenuProcesses.Value = Properties.Settings.Default.IconMenuProcessCount;
            textMaxSamples.Value = Properties.Settings.Default.MaxSamples;
            textStep.Value = Properties.Settings.Default.PlotterStep;
            textSearchEngine.Text = Properties.Settings.Default.SearchEngine;
            comboSizeUnits.SelectedItem =
                Misc.SizeUnitNames[Properties.Settings.Default.UnitSpecifier];
            checkWarnDangerous.Checked = Properties.Settings.Default.WarnDangerous;
            checkShowProcessDomains.Checked = Properties.Settings.Default.ShowAccountDomains;
            checkShowTrayIcon.Checked = Properties.Settings.Default.ShowIcon;
            checkHideWhenMinimized.Checked = Properties.Settings.Default.HideWhenMinimized;
            checkHideWhenClosed.Checked = Properties.Settings.Default.HideWhenClosed;
            checkAllowOnlyOneInstance.Checked = Properties.Settings.Default.AllowOnlyOneInstance;
            checkVerifySignatures.Checked = Properties.Settings.Default.VerifySignatures;
            checkHideHandlesWithNoName.Checked = Properties.Settings.Default.HideHandlesWithNoName;
            checkEnableKPH.Checked = Properties.Settings.Default.EnableKPH;
            checkStartHidden.Checked = Properties.Settings.Default.StartHidden;

            textImposterNames.Text = Properties.Settings.Default.ImposterNames;

            // Highlighting       
            textHighlightingDuration.Value = Properties.Settings.Default.HighlightingDuration;
            colorNewProcesses.Color = Properties.Settings.Default.ColorNew;
            colorRemovedProcesses.Color = Properties.Settings.Default.ColorRemoved;

            foreach (ListViewItem item in listHighlightingColors.Items)
            {
                Color c = (Color)Properties.Settings.Default[item.Name];
                bool use = (bool)Properties.Settings.Default["Use" + item.Name];

                item.BackColor = c;
                item.Checked = use;
            }

            // Plotting
            checkPlotterAntialias.Checked = Properties.Settings.Default.PlotterAntialias;
            colorCPUKT.Color = Properties.Settings.Default.PlotterCPUKernelColor;
            colorCPUUT.Color = Properties.Settings.Default.PlotterCPUUserColor;
            colorMemoryPB.Color = Properties.Settings.Default.PlotterMemoryPrivateColor;
            colorMemoryWS.Color = Properties.Settings.Default.PlotterMemoryWSColor;
            colorIORO.Color = Properties.Settings.Default.PlotterIOROColor;
            colorIOW.Color = Properties.Settings.Default.PlotterIOWColor;

            // Replace Task Manager
            // See if we can write to the key.
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options",
                    true
                    );

                try
                {
                    if (!Array.Exists<string>(key.GetSubKeyNames(), s => s.Equals("taskmgr.exe", StringComparison.InvariantCultureIgnoreCase)))
                        key.CreateSubKey("taskmgr.exe");

                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskmgr.exe",
                        true
                        ).Close();
                }
                finally
                {
                    key.Close();
                }
            }
            catch
            {
                checkReplaceTaskManager.Enabled = false;
            }

            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskmgr.exe",
                    false
                    ))
                {
                    if ((_oldTaskMgrDebugger = (string)key.GetValue("Debugger", "")).ToLower().Trim('"') ==
                        ProcessHandle.FromHandle(Program.CurrentProcess).GetMainModule().FileName.ToLower())
                    {
                        checkReplaceTaskManager.Checked = true;
                    }
                    else
                    {
                        checkReplaceTaskManager.Checked = false;
                    }
                }
            }
            catch
            {
                checkReplaceTaskManager.Enabled = false;
            }

            // Symbols
            try
            {
                _oldDbghelp = textDbghelpPath.Text = Properties.Settings.Default.DbgHelpPath;
                textSearchPath.Text = Properties.Settings.Default.DbgHelpSearchPath;
                checkUndecorate.Checked = Properties.Settings.Default.DbgHelpUndecorate;
            }
            catch
            { }

            checkShowTrayIcon_CheckedChanged(null, null);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Font = _font;
            Properties.Settings.Default.SearchEngine = textSearchEngine.Text;
            Properties.Settings.Default.WarnDangerous = checkWarnDangerous.Checked;
            Properties.Settings.Default.ShowAccountDomains = checkShowProcessDomains.Checked;
            Properties.Settings.Default.ShowIcon = checkShowTrayIcon.Checked;
            Properties.Settings.Default.HideWhenMinimized = checkHideWhenMinimized.Checked;
            Properties.Settings.Default.HideWhenClosed = checkHideWhenClosed.Checked;
            Properties.Settings.Default.AllowOnlyOneInstance = checkAllowOnlyOneInstance.Checked;
            Properties.Settings.Default.UnitSpecifier =
                Array.IndexOf(Misc.SizeUnitNames, comboSizeUnits.SelectedItem);
            Properties.Settings.Default.VerifySignatures = checkVerifySignatures.Checked;
            Properties.Settings.Default.HideHandlesWithNoName = checkHideHandlesWithNoName.Checked;
            Properties.Settings.Default.StartHidden = checkStartHidden.Checked;
            Properties.Settings.Default.EnableKPH = checkEnableKPH.Checked;
            Properties.Settings.Default.ImposterNames = textImposterNames.Text.ToLower();

            Properties.Settings.Default.MaxSamples = (int)textMaxSamples.Value;
            HistoryManager.GlobalMaxCount = Properties.Settings.Default.MaxSamples;
            Properties.Settings.Default.PlotterStep = (int)textStep.Value;
            ProcessHacker.Components.Plotter.GlobalMoveStep = Properties.Settings.Default.PlotterStep;

            Properties.Settings.Default.HighlightingDuration = (int)textHighlightingDuration.Value;
            Properties.Settings.Default.ColorNew = colorNewProcesses.Color;
            Properties.Settings.Default.ColorRemoved = colorRemovedProcesses.Color;

            foreach (ListViewItem item in listHighlightingColors.Items)
            {
                Properties.Settings.Default[item.Name] = item.BackColor;
                Properties.Settings.Default["Use" + item.Name] = item.Checked;
            }

            Properties.Settings.Default.PlotterAntialias = checkPlotterAntialias.Checked;
            Properties.Settings.Default.PlotterCPUKernelColor = colorCPUKT.Color;
            Properties.Settings.Default.PlotterCPUUserColor = colorCPUUT.Color;
            Properties.Settings.Default.PlotterMemoryPrivateColor = colorMemoryPB.Color;
            Properties.Settings.Default.PlotterMemoryWSColor = colorMemoryWS.Color;
            Properties.Settings.Default.PlotterIOROColor = colorIORO.Color;
            Properties.Settings.Default.PlotterIOWColor = colorIOW.Color;

            Properties.Settings.Default.DbgHelpPath = textDbghelpPath.Text;
            Properties.Settings.Default.DbgHelpSearchPath = textSearchPath.Text;
            Properties.Settings.Default.DbgHelpUndecorate = checkUndecorate.Checked;

            Properties.Settings.Default.Save();

            if (checkReplaceTaskManager.Enabled)
            {
                try
                {
                    string fileName = ProcessHandle.FromHandle(Program.CurrentProcess).GetMainModule().FileName;

                    using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskmgr.exe",
                        true
                        ))
                    {
                        if (checkReplaceTaskManager.Checked)
                        {
                            key.SetValue("Debugger", "\"" + fileName + "\"");
                        }
                        else
                        {
                            if (_oldTaskMgrDebugger.ToLower().Trim('"') == fileName.ToLower())
                                key.DeleteValue("Debugger");
                            else if (_oldTaskMgrDebugger != "")
                                key.SetValue("Debugger", _oldTaskMgrDebugger);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not replace Task Manager with Process Hacker: " + ex.Message,
                        "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ApplySettings()
        {
            Program.ImposterNames = new System.Collections.Specialized.StringCollection();

            foreach (string s in Properties.Settings.Default.ImposterNames.Split(','))
                Program.ImposterNames.Add(s.Trim());

            Program.HackerWindow.NotifyIcon.Visible = Properties.Settings.Default.ShowIcon;
            Program.ProcessProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.HackerWindow.ServiceProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.HackerWindow.NetworkProvider.Interval = Properties.Settings.Default.RefreshInterval;

            HighlightingContext.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            HighlightingContext.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNew;
            HighlightingContext.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemoved;

            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Properties.Settings.Default.ColorNew;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Properties.Settings.Default.ColorRemoved;

            Program.ProcessProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.HackerWindow.ServiceProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.HackerWindow.NetworkProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.SharedThreadProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.SecondarySharedThreadProvider.Interval = Properties.Settings.Default.RefreshInterval;

            Program.HackerWindow.ProcessTree.RefreshItems();
            Program.ApplyFont(Properties.Settings.Default.Font);

            if (_oldDbghelp != textDbghelpPath.Text)
                MessageBox.Show("One or more options you have changed require a restart of Process Hacker.",
                    "Process Hacker", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SaveSettings();

            if (!_dontApply)
                this.ApplySettings();

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkShowTrayIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (checkShowTrayIcon.Checked)
            {
                checkHideWhenMinimized.Enabled = true;
                checkHideWhenClosed.Enabled = true;
                checkStartHidden.Enabled = true;
            }
            else
            {
                checkHideWhenMinimized.Enabled = false;
                checkHideWhenMinimized.Checked = false;
                checkHideWhenClosed.Enabled = false;
                checkHideWhenClosed.Checked = false;
                checkStartHidden.Enabled = false;
                checkStartHidden.Checked = false;
            }
        }

        private void buttonFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();

            fd.Font = _font;
            fd.FontMustExist = true;
            fd.ShowEffects = true;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                _font = fd.Font;
                buttonFont.Font = _font;
                this.EnableApplyButton();
            }
        }

        private void buttonChangeReplaceTaskManager_Click(object sender, EventArgs e)
        {
            this.SaveSettings();
            if (!_dontApply)
                this.ApplySettings();
            buttonApply.Enabled = false;

            string args = "-o -hwnd " + this.Handle.ToString() +
                " -rect " + this.Location.X.ToString() + "," + this.Location.Y.ToString() + "," +
                this.Size.Width.ToString() + "," + this.Size.Height.ToString();

            Thread t = new Thread(() =>
                {
                    Program.StartProcessHackerAdminWait(args, this.Handle, 0xffffffff);

                    this.BeginInvoke(new MethodInvoker(() =>
                        {
                            Properties.Settings.Default.Reload();
                            this.LoadSettings();
                            if (!_dontApply)
                                this.ApplySettings();
                            buttonApply.Enabled = false;
                            buttonOK.Select();
                        }));
                });

            t.Start();
        }

        private void buttonEnableAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listHighlightingColors.Items)
                item.Checked = true;
        }

        private void buttonDisableAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listHighlightingColors.Items)
                item.Checked = false;
        }

        private void buttonDbghelpBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "dbghelp.dll|dbghelp.dll|DLL files (*.dll)|*.dll|All files (*.*)|*.*";
            ofd.FileName = textDbghelpPath.Text;

            if (ofd.ShowDialog() == DialogResult.OK)
                textDbghelpPath.Text = ofd.FileName;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            this.SaveSettings();
            this.ApplySettings();
            buttonApply.Enabled = false;
            buttonOK.Select();
        }
    }
}
