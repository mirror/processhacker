/*
 * Process Hacker - 
 *   options window
 * 
 * Copyright (C) 2009 dmex
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
using ProcessHacker.Base;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProcessHacker
{
    public partial class OptionsWindow : Form
    {
        private bool _isFirstPaint = true;
        private string _oldDbghelp;
        private string _oldTaskMgrDebugger;
        private Font _font;
        private bool _dontApply;

        private Dictionary<PluginBase, PluginSettingsControlBase> _pluginSettings =
            new Dictionary<PluginBase, PluginSettingsControlBase>();

        public OptionsWindow()
            : this(false)
        { }

        public OptionsWindow(bool dontApply)
        {
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            _dontApply = dontApply;
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

            listPlugins.SetDoubleBuffered(true);
            listPlugins.SetTheme("explorer");
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)WindowMessage.Paint:
                    {
                        if (_isFirstPaint)
                        {
                            this.LoadStage1();
                        }

                        _isFirstPaint = false;
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void LoadStage1()
        {
            this.InitializeHighlightingColors();
            this.LoadSettings();

            if (!OSVersion.HasUac)
                comboElevationLevel.Enabled = false;

            labelPluginDescription.Text = "";

            foreach (var plugin in Program.AppInstance.GetPlugins())
            {
                listPlugins.Items.Add(new ListViewItem() { Tag = plugin.Name, Text = plugin.Title });
            }

            bool visualStyles = Application.RenderWithVisualStyles;

            foreach (TabPage tab in tabControl.TabPages)
            {
                foreach (Control c in tab.Controls)
                {
                    // If we don't have visual styles or we're on XP, fix control backgrounds.
                    if (!visualStyles || OSVersion.IsBelowOrEqual(WindowsVersion.XP))
                    {
                        if (c is CheckBox)
                            (c as CheckBox).FlatStyle = FlatStyle.Standard;
                        if (c is RadioButton)
                            (c as RadioButton).FlatStyle = FlatStyle.Standard;
                    }

                    // Add event handlers to enable the apply button.
                    if (c is CheckBox || c is ListView || c is NumericUpDown)
                        c.Click += (sender, e) => this.EnableApplyButton();
                    else if (c is TextBox)
                        (c as TextBox).TextChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is ComboBox)
                        (c as ComboBox).SelectedIndexChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is NumericUpDown)
                        (c as NumericUpDown).ValueChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is ColorModifier)
                        (c as ColorModifier).ColorChanged += (sender, e) => this.EnableApplyButton();
                    else if (c is Button || c is Label || c is GroupBox)
                        Program.Void(); // Nothing
                    else
                        c.Click += (sender, e) => this.EnableApplyButton();
                }
            }

            foreach (Control c in UpdaterSettingsGroupBox.Controls)
            {
                // If we don't have visual styles or we're on XP, fix control backgrounds.
                if (!visualStyles || OSVersion.IsBelowOrEqual(WindowsVersion.XP))
                {
                    if (c is CheckBox)
                        (c as CheckBox).FlatStyle = FlatStyle.Standard;
                    if (c is RadioButton)
                        (c as RadioButton).FlatStyle = FlatStyle.Standard;
                }

                c.Click += (sender, e) => this.EnableApplyButton();
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
            AddToList("ColorDotNetProcesses", ".NET Processes and DLLs",
                ".NET, or managed processes and DLLs.");
            AddToList("ColorPosixProcesses", "POSIX Processes",
                "Processes running under the POSIX subsystem.");
            AddToList("ColorPackedProcesses", "Packed/Dangerous Processes",
                "Executables are sometimes \"packed\" to reduce their size.\n" +
                "\"Dangerous processes\" includes processes with invalid signatures and unverified " + 
                "processes with the name of a system process.");

            // WOW64, 64-bit only.
            if (IntPtr.Size == 8)
            {
                AddToList("ColorWow64Processes", "32-bit Processes",
                    "Processes running under WOW64, i.e. 32-bit.");
            }

            AddToList("ColorSuspended", "Suspended Threads",
                "Threads that are suspended from execution.");
            AddToList("ColorGuiThreads", "GUI Threads",
                "Threads that have made at least one GUI-related system call.");
            AddToList("ColorRelocatedDlls", "Relocated DLLs",
                "DLLs that were not loaded at their preferred image bases.");
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
            cd.FullOpen = true;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                listHighlightingColors.SelectedItems[0].BackColor = cd.Color;
                listHighlightingColors.SelectedItems[0].ForeColor = TreeNodeAdv.GetForeColor(cd.Color);
            }
        }

        private void textUpdateInterval_Leave(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.RefreshInterval = Int32.Parse(textUpdateInterval.Value.ToString());
            }
            catch
            {
                PhUtils.ShowError("The entered value is not valid.");
                textUpdateInterval.Select();
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
                PhUtils.ShowError("The entered value is not valid.");
                textIconMenuProcesses.Select();
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
                Utils.SizeUnitNames[Properties.Settings.Default.UnitSpecifier];
            checkWarnDangerous.Checked = Properties.Settings.Default.WarnDangerous;
            checkShowProcessDomains.Checked = Properties.Settings.Default.ShowAccountDomains;
            checkHideWhenMinimized.Checked = Properties.Settings.Default.HideWhenMinimized;
            checkHideWhenClosed.Checked = Properties.Settings.Default.HideWhenClosed;
            checkAllowOnlyOneInstance.Checked = Properties.Settings.Default.AllowOnlyOneInstance;
            checkVerifySignatures.Checked = Properties.Settings.Default.VerifySignatures;
            checkHideHandlesWithNoName.Checked = Properties.Settings.Default.HideHandlesWithNoName;
            checkEnableKPH.Checked = Properties.Settings.Default.EnableKPH;
            checkEnableExperimentalFeatures.Checked = Properties.Settings.Default.EnableExperimentalFeatures;
            checkStartHidden.Checked = Properties.Settings.Default.StartHidden;
            checkScrollDownProcessTree.Checked = Properties.Settings.Default.ScrollDownProcessTree;
            checkFloatChildWindows.Checked = Properties.Settings.Default.FloatChildWindows;
            checkHidePhConnections.Checked = Properties.Settings.Default.HideProcessHackerNetworkConnections;

            if (OSVersion.HasUac)
            {
                comboElevationLevel.SelectedIndex = Properties.Settings.Default.ElevationLevel;
            }
            else
            {
                comboElevationLevel.SelectedIndex = 0;
            }

            textImposterNames.Text = Properties.Settings.Default.ImposterNames;

            switch (Properties.Settings.Default.ToolStripDisplayStyle)
            {
                case 0:
                    comboToolbarStyle.SelectedIndex = 0;
                    break;
                case 1:
                    comboToolbarStyle.SelectedIndex = 1;
                    break;
                case 2:
                    comboToolbarStyle.SelectedIndex = 2;
                    break;
                default:
                    comboToolbarStyle.SelectedIndex = 1;
                    break;
            }

            // Highlighting       
            textHighlightingDuration.Value = Properties.Settings.Default.HighlightingDuration;
            colorNewProcesses.Color = Properties.Settings.Default.ColorNew;
            colorRemovedProcesses.Color = Properties.Settings.Default.ColorRemoved;

            foreach (ListViewItem item in listHighlightingColors.Items)
            {
                Color c = (Color)Properties.Settings.Default[item.Name];
                bool use = (bool)Properties.Settings.Default["Use" + item.Name];

                item.BackColor = c;
                item.ForeColor = TreeNodeAdv.GetForeColor(item.BackColor);
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
                        ProcessHandle.GetCurrent().GetMainModule().FileName.ToLower())
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

            checkUpdateAutomatically.Checked = Properties.Settings.Default.AppUpdateAutomatic;

            switch ((AppUpdateLevel)Properties.Settings.Default.AppUpdateLevel)
            {
                case AppUpdateLevel.Stable:
                default:
                    optUpdateStable.Checked = true;
                    break;
                case AppUpdateLevel.Beta:
                    optUpdateBeta.Checked = true;
                    break;
                case AppUpdateLevel.Alpha:
                    optUpdateAlpha.Checked = true;
                    break;
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Font = _font;
            Properties.Settings.Default.SearchEngine = textSearchEngine.Text;
            Properties.Settings.Default.WarnDangerous = checkWarnDangerous.Checked;
            Properties.Settings.Default.ShowAccountDomains = checkShowProcessDomains.Checked;
            Properties.Settings.Default.HideWhenMinimized = checkHideWhenMinimized.Checked;
            Properties.Settings.Default.HideWhenClosed = checkHideWhenClosed.Checked;
            Properties.Settings.Default.AllowOnlyOneInstance = checkAllowOnlyOneInstance.Checked;
            Properties.Settings.Default.UnitSpecifier =
                Array.IndexOf(Utils.SizeUnitNames, comboSizeUnits.SelectedItem);
            Properties.Settings.Default.VerifySignatures = checkVerifySignatures.Checked;
            Properties.Settings.Default.HideHandlesWithNoName = checkHideHandlesWithNoName.Checked;
            Properties.Settings.Default.ScrollDownProcessTree = checkScrollDownProcessTree.Checked;
            Properties.Settings.Default.FloatChildWindows = checkFloatChildWindows.Checked;
            Properties.Settings.Default.StartHidden = checkStartHidden.Checked;
            Properties.Settings.Default.EnableKPH = checkEnableKPH.Checked;
            Properties.Settings.Default.EnableExperimentalFeatures = checkEnableExperimentalFeatures.Checked;
            Properties.Settings.Default.ImposterNames = textImposterNames.Text.ToLower();
            Properties.Settings.Default.HideProcessHackerNetworkConnections = checkHidePhConnections.Checked;
            Properties.Settings.Default.ElevationLevel = comboElevationLevel.SelectedIndex;

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

            if (optUpdateStable.Checked)
            {
                Properties.Settings.Default.AppUpdateLevel = (int)AppUpdateLevel.Stable;
            }
            else if (optUpdateBeta.Checked)
            {
                Properties.Settings.Default.AppUpdateLevel = (int)AppUpdateLevel.Beta;
            }
            else if (optUpdateAlpha.Checked)
            {
                Properties.Settings.Default.AppUpdateLevel = (int)AppUpdateLevel.Alpha;
            }

            Properties.Settings.Default.AppUpdateAutomatic = checkUpdateAutomatically.Checked;

            switch (comboToolbarStyle.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.ToolStripDisplayStyle = 0;
                    break;
                case 1:
                    Properties.Settings.Default.ToolStripDisplayStyle = 1;
                    break;
                case 2:
                    Properties.Settings.Default.ToolStripDisplayStyle = 2;
                    break;
                default:
                    Properties.Settings.Default.ToolStripDisplayStyle = 0;
                    break;
            }
         
            Properties.Settings.Default.Save();

            if (checkReplaceTaskManager.Enabled)
            {
                try
                {
                    string fileName = ProcessHandle.GetCurrent().GetMainModule().FileName;

                    using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\taskmgr.exe",
                        true
                        ))
                    {
                        if (checkReplaceTaskManager.Checked)
                        {
                            key.SetValue("Debugger", "\"" + fileName + "\"");
                            // In case the user presses Apply and then OK.
                            _oldTaskMgrDebugger = "\"" + fileName + "\"";
                        }
                        else
                        {
                            if (_oldTaskMgrDebugger.ToLower().Trim('"') == fileName.ToLower())
                            {
                                key.DeleteValue("Debugger");
                                _oldTaskMgrDebugger = "";
                            }
                            else if (_oldTaskMgrDebugger != "")
                            {
                                key.SetValue("Debugger", _oldTaskMgrDebugger);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to replace Task Manager with Process Hacker", ex);
                }
            }

            foreach (var control in _pluginSettings.Values)
                control.OnSettingsSaved();
        }

        private void ApplySettings()
        {
            Settings.Refresh();

            Program.ImposterNames = new System.Collections.Specialized.StringCollection();
            Utils.UnitSpecifier = Properties.Settings.Default.UnitSpecifier;

            foreach (string s in Properties.Settings.Default.ImposterNames.Split(','))
                Program.ImposterNames.Add(s.Trim());

            Program.HackerWindow.ApplyIconVisibilities();
            Program.HackerWindow.LoadFixMenuItems();
            Program.ProcessProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.ServiceProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.NetworkProvider.Interval = Properties.Settings.Default.RefreshInterval;

            HighlightingContext.HighlightingDuration = Properties.Settings.Default.HighlightingDuration;
            HighlightingContext.Colors[ListViewItemState.New] = Properties.Settings.Default.ColorNew;
            HighlightingContext.Colors[ListViewItemState.Removed] = Properties.Settings.Default.ColorRemoved;

            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Properties.Settings.Default.ColorNew;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Properties.Settings.Default.ColorRemoved;

            Program.ProcessProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.ServiceProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.NetworkProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.SharedThreadProvider.Interval = Properties.Settings.Default.RefreshInterval;
            Program.SecondarySharedThreadProvider.Interval = Properties.Settings.Default.RefreshInterval;

            Program.HackerWindow.ProcessTree.RefreshItems();
            Program.ApplyFont(Properties.Settings.Default.Font);

            if (_oldDbghelp != textDbghelpPath.Text)
                PhUtils.ShowInformation("One or more options you have changed require a restart of Process Hacker.");
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SaveSettings();

            if (!this._dontApply)
                this.ApplySettings();

            DialogResult = DialogResult.OK;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            this.Close();
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

            // Avoid cross-thread operation.
            IntPtr thisHandle = this.Handle;

            Thread t = new Thread(() =>
                {
                    Program.StartProcessHackerAdminWait(args, thisHandle, 0xffffffff);

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

            this.EnableApplyButton();
        }

        private void buttonDisableAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listHighlightingColors.Items)
                item.Checked = false;

            this.EnableApplyButton();
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
            this.buttonApply.Enabled = false;
            this.buttonOK.Select();
        }

        private void listPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listPlugins.SelectedItems.Count == 0)
            {
                labelPluginTitle.Text = "No plugin selected";
                labelPluginDescription.Text = "";
                panelPluginOptions.Controls.Clear();
            }
            else
            {
                string pluginName = (string)listPlugins.SelectedItems[0].Tag;
                PluginBase plugin = Program.AppInstance.GetPlugin(pluginName);

                labelPluginTitle.Text = plugin.Title + " (" + plugin.Author + ")";
                labelPluginDescription.Text = plugin.Description;

                if (_pluginSettings.ContainsKey(plugin))
                {
                    panelPluginOptions.Controls.Add(_pluginSettings[plugin]);
                }
                else
                {
                    PluginSettingsControlBase settingsControl = plugin.OnRetrieveSettingsControl();

                    if (settingsControl != null)
                    {
                        settingsControl.Dock = DockStyle.Fill;
                        settingsControl.Location = new Point(0, 0);
                        _pluginSettings.Add(plugin, settingsControl);
                        panelPluginOptions.Controls.Add(_pluginSettings[plugin]);
                    }
                }
            }
        }
    }
}
