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
            if (OSVersion.Architecture == OSArch.Amd64)
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
                Settings.Instance.RefreshInterval = Int32.Parse(textUpdateInterval.Value.ToString());
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
                Settings.Instance.IconMenuProcessCount = Int32.Parse(textIconMenuProcesses.Value.ToString());
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
            _font = Settings.Instance.Font;
            buttonFont.Font = _font;
            textUpdateInterval.Value = Settings.Instance.RefreshInterval;
            textIconMenuProcesses.Value = Settings.Instance.IconMenuProcessCount;
            textMaxSamples.Value = Settings.Instance.MaxSamples;
            textStep.Value = Settings.Instance.PlotterStep;
            textSearchEngine.Text = Settings.Instance.SearchEngine;
            comboSizeUnits.SelectedItem =
                Utils.SizeUnitNames[Settings.Instance.UnitSpecifier];
            checkWarnDangerous.Checked = Settings.Instance.WarnDangerous;
            checkShowProcessDomains.Checked = Settings.Instance.ShowAccountDomains;
            checkHideWhenMinimized.Checked = Settings.Instance.HideWhenMinimized;
            checkHideWhenClosed.Checked = Settings.Instance.HideWhenClosed;
            checkAllowOnlyOneInstance.Checked = Settings.Instance.AllowOnlyOneInstance;
            checkVerifySignatures.Checked = Settings.Instance.VerifySignatures;
            checkHideHandlesWithNoName.Checked = Settings.Instance.HideHandlesWithNoName;
            checkEnableKPH.Enabled = OSVersion.Architecture == OSArch.I386;
            checkEnableKPH.Checked = Settings.Instance.EnableKPH;
            checkEnableExperimentalFeatures.Checked = Settings.Instance.EnableExperimentalFeatures;
            checkStartHidden.Checked = Settings.Instance.StartHidden;
            checkScrollDownProcessTree.Checked = Settings.Instance.ScrollDownProcessTree;
            checkFloatChildWindows.Checked = Settings.Instance.FloatChildWindows;
            checkHidePhConnections.Checked = Settings.Instance.HideProcessHackerNetworkConnections;

            if (OSVersion.HasUac)
            {
                comboElevationLevel.SelectedIndex = Settings.Instance.ElevationLevel;
            }
            else
            {
                comboElevationLevel.SelectedIndex = 0;
            }

            textImposterNames.Text = Settings.Instance.ImposterNames;

            switch (Settings.Instance.ToolStripDisplayStyle)
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
            textHighlightingDuration.Value = Settings.Instance.HighlightingDuration;
            colorNewProcesses.Color = Settings.Instance.ColorNew;
            colorRemovedProcesses.Color = Settings.Instance.ColorRemoved;

            foreach (ListViewItem item in listHighlightingColors.Items)
            {
                Color c = (Color)Settings.Instance[item.Name];
                bool use = (bool)Settings.Instance["Use" + item.Name];

                item.BackColor = c;
                item.ForeColor = TreeNodeAdv.GetForeColor(item.BackColor);
                item.Checked = use;
            }

            // Plotting
            checkPlotterAntialias.Checked = Settings.Instance.PlotterAntialias;
            colorCPUKT.Color = Settings.Instance.PlotterCPUKernelColor;
            colorCPUUT.Color = Settings.Instance.PlotterCPUUserColor;
            colorMemoryPB.Color = Settings.Instance.PlotterMemoryPrivateColor;
            colorMemoryWS.Color = Settings.Instance.PlotterMemoryWSColor;
            colorIORO.Color = Settings.Instance.PlotterIOROColor;
            colorIOW.Color = Settings.Instance.PlotterIOWColor;

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
                    if (!Array.Exists<string>(key.GetSubKeyNames(), s => s.Equals("taskmgr.exe", StringComparison.OrdinalIgnoreCase)))
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
                    if ((_oldTaskMgrDebugger = (string)key.GetValue("Debugger", "")).Trim('"').Equals(
                        ProcessHandle.Current.GetMainModule().FileName, StringComparison.OrdinalIgnoreCase))
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
                _oldDbghelp = textDbghelpPath.Text = Settings.Instance.DbgHelpPath;
                textSearchPath.Text = Settings.Instance.DbgHelpSearchPath;
                checkUndecorate.Checked = Settings.Instance.DbgHelpUndecorate;
            }
            catch
            { }

            checkUpdateAutomatically.Checked = Settings.Instance.AppUpdateAutomatic;

            switch ((AppUpdateLevel)Settings.Instance.AppUpdateLevel)
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
            bool restartRequired = false;

            if (checkVerifySignatures.Checked != Settings.Instance.VerifySignatures)
                restartRequired = true;
            if (checkEnableKPH.Checked != Settings.Instance.EnableKPH)
                restartRequired = true;
            if (checkEnableExperimentalFeatures.Checked != Settings.Instance.EnableExperimentalFeatures)
                restartRequired = true;
            if (textDbghelpPath.Text != _oldDbghelp)
                restartRequired = true;

            Settings.Instance.Font = _font;
            Settings.Instance.SearchEngine = textSearchEngine.Text;
            Settings.Instance.WarnDangerous = checkWarnDangerous.Checked;
            Settings.Instance.ShowAccountDomains = checkShowProcessDomains.Checked;
            Settings.Instance.HideWhenMinimized = checkHideWhenMinimized.Checked;
            Settings.Instance.HideWhenClosed = checkHideWhenClosed.Checked;
            Settings.Instance.AllowOnlyOneInstance = checkAllowOnlyOneInstance.Checked;
            Settings.Instance.UnitSpecifier = Array.IndexOf(Utils.SizeUnitNames, comboSizeUnits.SelectedItem);
            Settings.Instance.VerifySignatures = checkVerifySignatures.Checked;
            Settings.Instance.HideHandlesWithNoName = checkHideHandlesWithNoName.Checked;
            Settings.Instance.ScrollDownProcessTree = checkScrollDownProcessTree.Checked;
            Settings.Instance.FloatChildWindows = checkFloatChildWindows.Checked;
            Settings.Instance.StartHidden = checkStartHidden.Checked;
            Settings.Instance.EnableKPH = checkEnableKPH.Checked;
            Settings.Instance.EnableExperimentalFeatures = checkEnableExperimentalFeatures.Checked;
            Settings.Instance.ImposterNames = textImposterNames.Text.ToLowerInvariant();
            Settings.Instance.HideProcessHackerNetworkConnections = checkHidePhConnections.Checked;
            Settings.Instance.ElevationLevel = comboElevationLevel.SelectedIndex;

            Settings.Instance.MaxSamples = (int)textMaxSamples.Value;
            Program.ProcessProvider.HistoryMaxSize = Settings.Instance.MaxSamples;
            Settings.Instance.PlotterStep = (int)textStep.Value;
            ProcessHacker.Components.Plotter.GlobalMoveStep = Settings.Instance.PlotterStep;

            Settings.Instance.HighlightingDuration = (int)textHighlightingDuration.Value;
            Settings.Instance.ColorNew = colorNewProcesses.Color;
            Settings.Instance.ColorRemoved = colorRemovedProcesses.Color;

            foreach (ListViewItem item in listHighlightingColors.Items)
            {
                Settings.Instance[item.Name] = item.BackColor;
                Settings.Instance["Use" + item.Name] = item.Checked;
            }

            Settings.Instance.PlotterAntialias = checkPlotterAntialias.Checked;
            Settings.Instance.PlotterCPUKernelColor = colorCPUKT.Color;
            Settings.Instance.PlotterCPUUserColor = colorCPUUT.Color;
            Settings.Instance.PlotterMemoryPrivateColor = colorMemoryPB.Color;
            Settings.Instance.PlotterMemoryWSColor = colorMemoryWS.Color;
            Settings.Instance.PlotterIOROColor = colorIORO.Color;
            Settings.Instance.PlotterIOWColor = colorIOW.Color;

            Settings.Instance.DbgHelpPath = _oldDbghelp = textDbghelpPath.Text;
            Settings.Instance.DbgHelpSearchPath = textSearchPath.Text;
            Settings.Instance.DbgHelpUndecorate = checkUndecorate.Checked;

            if (optUpdateStable.Checked)
            {
                Settings.Instance.AppUpdateLevel = (int)AppUpdateLevel.Stable;
            }
            else if (optUpdateBeta.Checked)
            {
                Settings.Instance.AppUpdateLevel = (int)AppUpdateLevel.Beta;
            }
            else if (optUpdateAlpha.Checked)
            {
                Settings.Instance.AppUpdateLevel = (int)AppUpdateLevel.Alpha;
            }

            Settings.Instance.AppUpdateAutomatic = checkUpdateAutomatically.Checked;

            switch (comboToolbarStyle.SelectedIndex)
            {
                case 0:
                    Settings.Instance.ToolStripDisplayStyle = 0;
                    break;
                case 1:
                    Settings.Instance.ToolStripDisplayStyle = 1;
                    break;
                case 2:
                    Settings.Instance.ToolStripDisplayStyle = 2;
                    break;
                default:
                    Settings.Instance.ToolStripDisplayStyle = 0;
                    break;
            }

            Settings.Instance.Save();
            // We manually set settings, so we must invalidate.
            Settings.Instance.Invalidate();

            if (checkReplaceTaskManager.Enabled)
            {
                try
                {
                    string fileName = ProcessHandle.Current.GetMainModule().FileName;

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
                            // If we were replacing Task Manager and the user unchecked the box, 
                            // don't replace Task Manager anymore.
                            if (_oldTaskMgrDebugger.Trim('"').Equals(fileName, StringComparison.OrdinalIgnoreCase))
                            {
                                key.DeleteValue("Debugger");
                                _oldTaskMgrDebugger = "";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    PhUtils.ShowException("Unable to replace Task Manager with Process Hacker", ex);
                }
            }

            if (restartRequired)
                PhUtils.ShowInformation("One or more settings you have changed require a restart of Process Hacker.");
        }

        private void ApplySettings()
        {
            Program.ImposterNames = new System.Collections.Specialized.StringCollection();
            Utils.UnitSpecifier = Settings.Instance.UnitSpecifier;

            foreach (string s in Settings.Instance.ImposterNames.Split(','))
                Program.ImposterNames.Add(s.Trim());

            Program.HackerWindow.ApplyIconVisibilities();
            Program.HackerWindow.LoadFixOSSpecific();

            HighlightingContext.HighlightingDuration = Settings.Instance.HighlightingDuration;
            HighlightingContext.Colors[ListViewItemState.New] = Settings.Instance.ColorNew;
            HighlightingContext.Colors[ListViewItemState.Removed] = Settings.Instance.ColorRemoved;

            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.New] = Settings.Instance.ColorNew;
            TreeNodeAdv.StateColors[TreeNodeAdv.NodeState.Removed] = Settings.Instance.ColorRemoved;

            Program.PrimaryProviderThread.Interval = Settings.Instance.RefreshInterval;
            Program.SecondaryProviderThread.Interval = Settings.Instance.RefreshInterval;

            Program.HackerWindow.ProcessTree.RefreshItems();
            Program.ApplyFont(Settings.Instance.Font);
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
                            Settings.Instance.Reload();
                            this.LoadSettings();
                            if (!_dontApply)
                                this.ApplySettings();
                            buttonApply.Enabled = false;
                            buttonOK.Select();
                        }));
                }, Utils.SixteenthStackSize);

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

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (PhUtils.ShowConfirmMessage("reset", "the settings and restart Process Hacker", null, false))
            {
                Settings.Instance.Reset();
                Program.GlobalMutex.Dispose();
                Program.TryStart(ProcessHandle.Current.GetMainModule().FileName);
                Program.HackerWindow.Exit(false);
            }
        }
    }
}
