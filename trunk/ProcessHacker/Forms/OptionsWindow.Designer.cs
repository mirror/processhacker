namespace ProcessHacker
{
    partial class OptionsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.textUpdateInterval = new System.Windows.Forms.NumericUpDown();
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkShowProcessDomains = new System.Windows.Forms.CheckBox();
            this.checkWarnDangerous = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textSearchEngine = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.buttonFont = new System.Windows.Forms.Button();
            this.textImposterNames = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.comboSizeUnits = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkStartHidden = new System.Windows.Forms.CheckBox();
            this.checkHideWhenClosed = new System.Windows.Forms.CheckBox();
            this.checkHideWhenMinimized = new System.Windows.Forms.CheckBox();
            this.checkShowTrayIcon = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textIconMenuProcesses = new System.Windows.Forms.NumericUpDown();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.checkReplaceTaskManager = new System.Windows.Forms.CheckBox();
            this.checkEnableKPH = new System.Windows.Forms.CheckBox();
            this.checkHideHandlesWithNoName = new System.Windows.Forms.CheckBox();
            this.checkVerifySignatures = new System.Windows.Forms.CheckBox();
            this.tabHighlighting = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.listHighlightingColors = new System.Windows.Forms.ListView();
            this.columnDescription = new System.Windows.Forms.ColumnHeader();
            this.textHighlightingDuration = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.colorRemovedProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorNewProcesses = new ProcessHacker.Components.ColorModifier();
            this.tabPlotting = new System.Windows.Forms.TabPage();
            this.checkPlotterAntialias = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.colorIORO = new ProcessHacker.Components.ColorModifier();
            this.colorIOW = new ProcessHacker.Components.ColorModifier();
            this.colorMemoryWS = new ProcessHacker.Components.ColorModifier();
            this.colorMemoryPB = new ProcessHacker.Components.ColorModifier();
            this.colorCPUUT = new ProcessHacker.Components.ColorModifier();
            this.colorCPUKT = new ProcessHacker.Components.ColorModifier();
            this.label22 = new System.Windows.Forms.Label();
            this.toolTipProvider = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.textUpdateInterval)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textIconMenuProcesses)).BeginInit();
            this.tabAdvanced.SuspendLayout();
            this.tabHighlighting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textHighlightingDuration)).BeginInit();
            this.tabPlotting.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Update Interval:";
            this.toolTipProvider.SetToolTip(this.label1, "The number of milliseconds between each check for new, modified or removed object" +
                    "s.");
            // 
            // textUpdateInterval
            // 
            this.textUpdateInterval.Increment = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.textUpdateInterval.Location = new System.Drawing.Point(134, 6);
            this.textUpdateInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textUpdateInterval.Minimum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.textUpdateInterval.Name = "textUpdateInterval";
            this.textUpdateInterval.Size = new System.Drawing.Size(66, 20);
            this.textUpdateInterval.TabIndex = 1;
            this.textUpdateInterval.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.textUpdateInterval.Leave += new System.EventHandler(this.textUpdateInterval_Leave);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(362, 359);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // checkShowProcessDomains
            // 
            this.checkShowProcessDomains.AutoSize = true;
            this.checkShowProcessDomains.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowProcessDomains.Location = new System.Drawing.Point(6, 285);
            this.checkShowProcessDomains.Name = "checkShowProcessDomains";
            this.checkShowProcessDomains.Size = new System.Drawing.Size(156, 18);
            this.checkShowProcessDomains.TabIndex = 3;
            this.checkShowProcessDomains.Text = "Show user/group domains";
            this.toolTipProvider.SetToolTip(this.checkShowProcessDomains, "Shows account domains. For example, \"MyUser\" would be displayed as \"ComputerName\\" +
                    "MyUser\".");
            this.checkShowProcessDomains.UseVisualStyleBackColor = true;
            // 
            // checkWarnDangerous
            // 
            this.checkWarnDangerous.AutoSize = true;
            this.checkWarnDangerous.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkWarnDangerous.Location = new System.Drawing.Point(6, 261);
            this.checkWarnDangerous.Name = "checkWarnDangerous";
            this.checkWarnDangerous.Size = new System.Drawing.Size(228, 18);
            this.checkWarnDangerous.TabIndex = 4;
            this.checkWarnDangerous.Text = "Warn about potentially dangerous actions";
            this.toolTipProvider.SetToolTip(this.checkWarnDangerous, "Warns about any attempts to suspend a system process or inspect a system process\'" +
                    " thread.");
            this.checkWarnDangerous.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Search Engine:";
            this.toolTipProvider.SetToolTip(this.label2, "The URL to open when searching for a process or module. \"%s\" is replaced with the" +
                    " process/module name.");
            // 
            // textSearchEngine
            // 
            this.textSearchEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchEngine.Location = new System.Drawing.Point(134, 58);
            this.textSearchEngine.Name = "textSearchEngine";
            this.textSearchEngine.Size = new System.Drawing.Size(277, 20);
            this.textSearchEngine.TabIndex = 6;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabAdvanced);
            this.tabControl.Controls.Add(this.tabHighlighting);
            this.tabControl.Controls.Add(this.tabPlotting);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(425, 341);
            this.tabControl.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.buttonFont);
            this.tabGeneral.Controls.Add(this.textImposterNames);
            this.tabGeneral.Controls.Add(this.label21);
            this.tabGeneral.Controls.Add(this.comboSizeUnits);
            this.tabGeneral.Controls.Add(this.label18);
            this.tabGeneral.Controls.Add(this.checkStartHidden);
            this.tabGeneral.Controls.Add(this.checkHideWhenClosed);
            this.tabGeneral.Controls.Add(this.checkHideWhenMinimized);
            this.tabGeneral.Controls.Add(this.checkShowTrayIcon);
            this.tabGeneral.Controls.Add(this.label23);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.textSearchEngine);
            this.tabGeneral.Controls.Add(this.textIconMenuProcesses);
            this.tabGeneral.Controls.Add(this.textUpdateInterval);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.checkShowProcessDomains);
            this.tabGeneral.Controls.Add(this.checkWarnDangerous);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(417, 315);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // buttonFont
            // 
            this.buttonFont.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonFont.Location = new System.Drawing.Point(6, 136);
            this.buttonFont.Name = "buttonFont";
            this.buttonFont.Size = new System.Drawing.Size(75, 23);
            this.buttonFont.TabIndex = 15;
            this.buttonFont.Text = "Font...";
            this.toolTipProvider.SetToolTip(this.buttonFont, "The font to use for all lists and trees.");
            this.buttonFont.UseVisualStyleBackColor = true;
            this.buttonFont.Click += new System.EventHandler(this.buttonFont_Click);
            // 
            // textImposterNames
            // 
            this.textImposterNames.Location = new System.Drawing.Point(134, 84);
            this.textImposterNames.Name = "textImposterNames";
            this.textImposterNames.Size = new System.Drawing.Size(277, 20);
            this.textImposterNames.TabIndex = 14;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 87);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(100, 13);
            this.label21.TabIndex = 13;
            this.label21.Text = "Require Signatures:";
            this.toolTipProvider.SetToolTip(this.label21, "A comma-separated list of process names to require valid signatures for.");
            // 
            // comboSizeUnits
            // 
            this.comboSizeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSizeUnits.FormattingEnabled = true;
            this.comboSizeUnits.Items.AddRange(new object[] {
            "B",
            "kB",
            "MB",
            "GB",
            "TB",
            "PB",
            "EB"});
            this.comboSizeUnits.Location = new System.Drawing.Point(134, 110);
            this.comboSizeUnits.Name = "comboSizeUnits";
            this.comboSizeUnits.Size = new System.Drawing.Size(67, 21);
            this.comboSizeUnits.TabIndex = 11;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 113);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(78, 13);
            this.label18.TabIndex = 10;
            this.label18.Text = "Max. Size Unit:";
            this.toolTipProvider.SetToolTip(this.label18, "The maximum unit of size to use.");
            // 
            // checkStartHidden
            // 
            this.checkStartHidden.AutoSize = true;
            this.checkStartHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkStartHidden.Location = new System.Drawing.Point(21, 237);
            this.checkStartHidden.Name = "checkStartHidden";
            this.checkStartHidden.Size = new System.Drawing.Size(89, 18);
            this.checkStartHidden.TabIndex = 9;
            this.checkStartHidden.Text = "Start hidden";
            this.toolTipProvider.SetToolTip(this.checkStartHidden, "Starts Process Hacker hidden. To restore it, double-click on the notification ico" +
                    "n.");
            this.checkStartHidden.UseVisualStyleBackColor = true;
            // 
            // checkHideWhenClosed
            // 
            this.checkHideWhenClosed.AutoSize = true;
            this.checkHideWhenClosed.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenClosed.Location = new System.Drawing.Point(21, 213);
            this.checkHideWhenClosed.Name = "checkHideWhenClosed";
            this.checkHideWhenClosed.Size = new System.Drawing.Size(117, 18);
            this.checkHideWhenClosed.TabIndex = 9;
            this.checkHideWhenClosed.Text = "Hide when closed";
            this.toolTipProvider.SetToolTip(this.checkHideWhenClosed, "Hides Process Hacker when it is closed.");
            this.checkHideWhenClosed.UseVisualStyleBackColor = true;
            // 
            // checkHideWhenMinimized
            // 
            this.checkHideWhenMinimized.AutoSize = true;
            this.checkHideWhenMinimized.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenMinimized.Location = new System.Drawing.Point(21, 189);
            this.checkHideWhenMinimized.Name = "checkHideWhenMinimized";
            this.checkHideWhenMinimized.Size = new System.Drawing.Size(131, 18);
            this.checkHideWhenMinimized.TabIndex = 9;
            this.checkHideWhenMinimized.Text = "Hide when minimized";
            this.toolTipProvider.SetToolTip(this.checkHideWhenMinimized, "Hides Process Hacker when it is minimized.");
            this.checkHideWhenMinimized.UseVisualStyleBackColor = true;
            // 
            // checkShowTrayIcon
            // 
            this.checkShowTrayIcon.AutoSize = true;
            this.checkShowTrayIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowTrayIcon.Location = new System.Drawing.Point(6, 165);
            this.checkShowTrayIcon.Name = "checkShowTrayIcon";
            this.checkShowTrayIcon.Size = new System.Drawing.Size(136, 18);
            this.checkShowTrayIcon.TabIndex = 7;
            this.checkShowTrayIcon.Text = "Show notification icon";
            this.toolTipProvider.SetToolTip(this.checkShowTrayIcon, "A notification icon which can be configured to alert you of events.");
            this.checkShowTrayIcon.UseVisualStyleBackColor = true;
            this.checkShowTrayIcon.CheckedChanged += new System.EventHandler(this.checkShowTrayIcon_CheckedChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 34);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(122, 13);
            this.label23.TabIndex = 0;
            this.label23.Text = "Processes in icon menu:";
            this.toolTipProvider.SetToolTip(this.label23, "The number of processes to display in the Processes menu.");
            // 
            // textIconMenuProcesses
            // 
            this.textIconMenuProcesses.Location = new System.Drawing.Point(134, 32);
            this.textIconMenuProcesses.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textIconMenuProcesses.Name = "textIconMenuProcesses";
            this.textIconMenuProcesses.Size = new System.Drawing.Size(66, 20);
            this.textIconMenuProcesses.TabIndex = 1;
            this.textIconMenuProcesses.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.textIconMenuProcesses.Leave += new System.EventHandler(this.textIconMenuProcesses_Leave);
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.Controls.Add(this.checkReplaceTaskManager);
            this.tabAdvanced.Controls.Add(this.checkEnableKPH);
            this.tabAdvanced.Controls.Add(this.checkHideHandlesWithNoName);
            this.tabAdvanced.Controls.Add(this.checkVerifySignatures);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(417, 315);
            this.tabAdvanced.TabIndex = 3;
            this.tabAdvanced.Text = "Advanced";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // checkReplaceTaskManager
            // 
            this.checkReplaceTaskManager.AutoSize = true;
            this.checkReplaceTaskManager.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkReplaceTaskManager.Location = new System.Drawing.Point(6, 54);
            this.checkReplaceTaskManager.Name = "checkReplaceTaskManager";
            this.checkReplaceTaskManager.Size = new System.Drawing.Size(245, 18);
            this.checkReplaceTaskManager.TabIndex = 17;
            this.checkReplaceTaskManager.Text = "Replace Task Manager with Process Hacker";
            this.toolTipProvider.SetToolTip(this.checkReplaceTaskManager, "Launches Process Hacker instead of Task Manager.");
            this.checkReplaceTaskManager.UseVisualStyleBackColor = true;
            // 
            // checkEnableKPH
            // 
            this.checkEnableKPH.AutoSize = true;
            this.checkEnableKPH.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkEnableKPH.Location = new System.Drawing.Point(6, 6);
            this.checkEnableKPH.Name = "checkEnableKPH";
            this.checkEnableKPH.Size = new System.Drawing.Size(155, 18);
            this.checkEnableKPH.TabIndex = 14;
            this.checkEnableKPH.Text = "Enable kernel-mode driver";
            this.toolTipProvider.SetToolTip(this.checkEnableKPH, "Enables the experimental driver which allows Process Hacker to bypass rootkits an" +
                    "d security software to a certain extent.");
            this.checkEnableKPH.UseVisualStyleBackColor = true;
            // 
            // checkHideHandlesWithNoName
            // 
            this.checkHideHandlesWithNoName.AutoSize = true;
            this.checkHideHandlesWithNoName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideHandlesWithNoName.Location = new System.Drawing.Point(6, 78);
            this.checkHideHandlesWithNoName.Name = "checkHideHandlesWithNoName";
            this.checkHideHandlesWithNoName.Size = new System.Drawing.Size(160, 18);
            this.checkHideHandlesWithNoName.TabIndex = 13;
            this.checkHideHandlesWithNoName.Text = "Hide handles with no name";
            this.toolTipProvider.SetToolTip(this.checkHideHandlesWithNoName, "Hides unnamed handles by default. This can be changed in each process window.");
            this.checkHideHandlesWithNoName.UseVisualStyleBackColor = true;
            // 
            // checkVerifySignatures
            // 
            this.checkVerifySignatures.AutoSize = true;
            this.checkVerifySignatures.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkVerifySignatures.Location = new System.Drawing.Point(6, 30);
            this.checkVerifySignatures.Name = "checkVerifySignatures";
            this.checkVerifySignatures.Size = new System.Drawing.Size(254, 18);
            this.checkVerifySignatures.TabIndex = 4;
            this.checkVerifySignatures.Text = "Verify signatures and perform additional checks";
            this.toolTipProvider.SetToolTip(this.checkVerifySignatures, "Verifies the digital signatures of all processes and highlights certain types of " +
                    "suspicious processes.");
            this.checkVerifySignatures.UseVisualStyleBackColor = true;
            // 
            // tabHighlighting
            // 
            this.tabHighlighting.Controls.Add(this.label5);
            this.tabHighlighting.Controls.Add(this.listHighlightingColors);
            this.tabHighlighting.Controls.Add(this.textHighlightingDuration);
            this.tabHighlighting.Controls.Add(this.label7);
            this.tabHighlighting.Controls.Add(this.label4);
            this.tabHighlighting.Controls.Add(this.label3);
            this.tabHighlighting.Controls.Add(this.colorRemovedProcesses);
            this.tabHighlighting.Controls.Add(this.colorNewProcesses);
            this.tabHighlighting.Location = new System.Drawing.Point(4, 22);
            this.tabHighlighting.Name = "tabHighlighting";
            this.tabHighlighting.Padding = new System.Windows.Forms.Padding(3);
            this.tabHighlighting.Size = new System.Drawing.Size(417, 315);
            this.tabHighlighting.TabIndex = 1;
            this.tabHighlighting.Text = "Highlighting";
            this.tabHighlighting.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(180, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Double-click on an item to change it.";
            // 
            // listHighlightingColors
            // 
            this.listHighlightingColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHighlightingColors.CheckBoxes = true;
            this.listHighlightingColors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDescription});
            this.listHighlightingColors.FullRowSelect = true;
            this.listHighlightingColors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listHighlightingColors.HideSelection = false;
            this.listHighlightingColors.Location = new System.Drawing.Point(6, 60);
            this.listHighlightingColors.MultiSelect = false;
            this.listHighlightingColors.Name = "listHighlightingColors";
            this.listHighlightingColors.ShowItemToolTips = true;
            this.listHighlightingColors.Size = new System.Drawing.Size(405, 234);
            this.listHighlightingColors.TabIndex = 10;
            this.listHighlightingColors.UseCompatibleStateImageBehavior = false;
            this.listHighlightingColors.View = System.Windows.Forms.View.Details;
            this.listHighlightingColors.DoubleClick += new System.EventHandler(this.listHighlightingColors_DoubleClick);
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 160;
            // 
            // textHighlightingDuration
            // 
            this.textHighlightingDuration.Increment = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.textHighlightingDuration.Location = new System.Drawing.Point(127, 7);
            this.textHighlightingDuration.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textHighlightingDuration.Name = "textHighlightingDuration";
            this.textHighlightingDuration.Size = new System.Drawing.Size(66, 20);
            this.textHighlightingDuration.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Highlighting Duration:";
            this.toolTipProvider.SetToolTip(this.label7, "The number of milliseconds for which new or removed objects will be highlighted a" +
                    "s such.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(230, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Removed Objects:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "New Objects:";
            // 
            // colorRemovedProcesses
            // 
            this.colorRemovedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorRemovedProcesses.Location = new System.Drawing.Point(351, 34);
            this.colorRemovedProcesses.Name = "colorRemovedProcesses";
            this.colorRemovedProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorRemovedProcesses.TabIndex = 3;
            // 
            // colorNewProcesses
            // 
            this.colorNewProcesses.Color = System.Drawing.Color.Transparent;
            this.colorNewProcesses.Location = new System.Drawing.Point(127, 33);
            this.colorNewProcesses.Name = "colorNewProcesses";
            this.colorNewProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorNewProcesses.TabIndex = 1;
            // 
            // tabPlotting
            // 
            this.tabPlotting.Controls.Add(this.checkPlotterAntialias);
            this.tabPlotting.Controls.Add(this.label12);
            this.tabPlotting.Controls.Add(this.label13);
            this.tabPlotting.Controls.Add(this.label14);
            this.tabPlotting.Controls.Add(this.label15);
            this.tabPlotting.Controls.Add(this.label16);
            this.tabPlotting.Controls.Add(this.label17);
            this.tabPlotting.Controls.Add(this.colorIORO);
            this.tabPlotting.Controls.Add(this.colorIOW);
            this.tabPlotting.Controls.Add(this.colorMemoryWS);
            this.tabPlotting.Controls.Add(this.colorMemoryPB);
            this.tabPlotting.Controls.Add(this.colorCPUUT);
            this.tabPlotting.Controls.Add(this.colorCPUKT);
            this.tabPlotting.Location = new System.Drawing.Point(4, 22);
            this.tabPlotting.Name = "tabPlotting";
            this.tabPlotting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlotting.Size = new System.Drawing.Size(417, 315);
            this.tabPlotting.TabIndex = 2;
            this.tabPlotting.Text = "Plotting";
            this.tabPlotting.UseVisualStyleBackColor = true;
            // 
            // checkPlotterAntialias
            // 
            this.checkPlotterAntialias.AutoSize = true;
            this.checkPlotterAntialias.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkPlotterAntialias.Location = new System.Drawing.Point(6, 6);
            this.checkPlotterAntialias.Name = "checkPlotterAntialias";
            this.checkPlotterAntialias.Size = new System.Drawing.Size(110, 18);
            this.checkPlotterAntialias.TabIndex = 22;
            this.checkPlotterAntialias.Text = "Use Anti-aliasing";
            this.toolTipProvider.SetToolTip(this.checkPlotterAntialias, "Uses anti-aliasing for all graphs. This can cause higher CPU usage on older compu" +
                    "ters.");
            this.checkPlotterAntialias.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 135);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "I/O Reads+Other:";
            this.toolTipProvider.SetToolTip(this.label12, "I/O reads and other operations - ReadFile, DeviceIoControl");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 160);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "I/O Writes:";
            this.toolTipProvider.SetToolTip(this.label13, "I/O writes - WriteFile");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 109);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Working Set:";
            this.toolTipProvider.SetToolTip(this.label14, "The working set.");
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 83);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "Private Bytes:";
            this.toolTipProvider.SetToolTip(this.label15, "The amount of memory not shared with other processes.");
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 58);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(83, 13);
            this.label16.TabIndex = 14;
            this.label16.Text = "CPU User Time:";
            this.toolTipProvider.SetToolTip(this.label16, "Time spent in user-mode.");
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 32);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "CPU Kernel Time:";
            this.toolTipProvider.SetToolTip(this.label17, "Time spent in kernel-mode.");
            // 
            // colorIORO
            // 
            this.colorIORO.Color = System.Drawing.Color.Transparent;
            this.colorIORO.Location = new System.Drawing.Point(124, 133);
            this.colorIORO.Name = "colorIORO";
            this.colorIORO.Size = new System.Drawing.Size(40, 20);
            this.colorIORO.TabIndex = 17;
            // 
            // colorIOW
            // 
            this.colorIOW.Color = System.Drawing.Color.Transparent;
            this.colorIOW.Location = new System.Drawing.Point(124, 159);
            this.colorIOW.Name = "colorIOW";
            this.colorIOW.Size = new System.Drawing.Size(40, 20);
            this.colorIOW.TabIndex = 18;
            // 
            // colorMemoryWS
            // 
            this.colorMemoryWS.Color = System.Drawing.Color.Transparent;
            this.colorMemoryWS.Location = new System.Drawing.Point(124, 107);
            this.colorMemoryWS.Name = "colorMemoryWS";
            this.colorMemoryWS.Size = new System.Drawing.Size(40, 20);
            this.colorMemoryWS.TabIndex = 19;
            // 
            // colorMemoryPB
            // 
            this.colorMemoryPB.Color = System.Drawing.Color.Transparent;
            this.colorMemoryPB.Location = new System.Drawing.Point(124, 81);
            this.colorMemoryPB.Name = "colorMemoryPB";
            this.colorMemoryPB.Size = new System.Drawing.Size(40, 20);
            this.colorMemoryPB.TabIndex = 15;
            // 
            // colorCPUUT
            // 
            this.colorCPUUT.Color = System.Drawing.Color.Transparent;
            this.colorCPUUT.Location = new System.Drawing.Point(124, 55);
            this.colorCPUUT.Name = "colorCPUUT";
            this.colorCPUUT.Size = new System.Drawing.Size(40, 20);
            this.colorCPUUT.TabIndex = 16;
            // 
            // colorCPUKT
            // 
            this.colorCPUKT.Color = System.Drawing.Color.Transparent;
            this.colorCPUKT.Location = new System.Drawing.Point(124, 29);
            this.colorCPUKT.Name = "colorCPUKT";
            this.colorCPUKT.Size = new System.Drawing.Size(40, 20);
            this.colorCPUKT.TabIndex = 11;
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(12, 364);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(300, 13);
            this.label22.TabIndex = 10;
            this.label22.Text = "Tip: Hover over an item to get a more detailed description of it.";
            // 
            // toolTipProvider
            // 
            this.toolTipProvider.AutomaticDelay = 250;
            this.toolTipProvider.AutoPopDelay = 5000;
            this.toolTipProvider.InitialDelay = 250;
            this.toolTipProvider.IsBalloon = true;
            this.toolTipProvider.ReshowDelay = 50;
            // 
            // OptionsWindow
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 394);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.textUpdateInterval)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textIconMenuProcesses)).EndInit();
            this.tabAdvanced.ResumeLayout(false);
            this.tabAdvanced.PerformLayout();
            this.tabHighlighting.ResumeLayout(false);
            this.tabHighlighting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textHighlightingDuration)).EndInit();
            this.tabPlotting.ResumeLayout(false);
            this.tabPlotting.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown textUpdateInterval;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.CheckBox checkShowProcessDomains;
        private System.Windows.Forms.CheckBox checkWarnDangerous;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textSearchEngine;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabHighlighting;
        private System.Windows.Forms.Label label3;
        private ProcessHacker.Components.ColorModifier colorNewProcesses;
        private ProcessHacker.Components.ColorModifier colorRemovedProcesses;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown textHighlightingDuration;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkShowTrayIcon;
        private System.Windows.Forms.CheckBox checkHideWhenMinimized;
        private System.Windows.Forms.TabPage tabPlotting;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private ProcessHacker.Components.ColorModifier colorIORO;
        private ProcessHacker.Components.ColorModifier colorIOW;
        private ProcessHacker.Components.ColorModifier colorMemoryWS;
        private ProcessHacker.Components.ColorModifier colorMemoryPB;
        private System.Windows.Forms.Label label14;
        private ProcessHacker.Components.ColorModifier colorCPUUT;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private ProcessHacker.Components.ColorModifier colorCPUKT;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox checkPlotterAntialias;
        private System.Windows.Forms.ComboBox comboSizeUnits;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox checkStartHidden;
        private System.Windows.Forms.TextBox textImposterNames;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button buttonFont;
        private System.Windows.Forms.ToolTip toolTipProvider;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.NumericUpDown textIconMenuProcesses;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox checkHideWhenClosed;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.CheckBox checkReplaceTaskManager;
        private System.Windows.Forms.CheckBox checkEnableKPH;
        private System.Windows.Forms.CheckBox checkHideHandlesWithNoName;
        private System.Windows.Forms.CheckBox checkVerifySignatures;
        private System.Windows.Forms.ListView listHighlightingColors;
        private System.Windows.Forms.ColumnHeader columnDescription;
        private System.Windows.Forms.Label label5;
    }
}