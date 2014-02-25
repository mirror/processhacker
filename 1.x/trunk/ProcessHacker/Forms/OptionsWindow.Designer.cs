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
            this.label1 = new System.Windows.Forms.Label();
            this.textUpdateInterval = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkShowProcessDomains = new System.Windows.Forms.CheckBox();
            this.checkWarnDangerous = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textSearchEngine = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.label20 = new System.Windows.Forms.Label();
            this.comboToolbarStyle = new System.Windows.Forms.ComboBox();
            this.checkFloatChildWindows = new System.Windows.Forms.CheckBox();
            this.checkScrollDownProcessTree = new System.Windows.Forms.CheckBox();
            this.checkAllowOnlyOneInstance = new System.Windows.Forms.CheckBox();
            this.buttonFont = new System.Windows.Forms.Button();
            this.textImposterNames = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.comboSizeUnits = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkStartHidden = new System.Windows.Forms.CheckBox();
            this.checkHideWhenClosed = new System.Windows.Forms.CheckBox();
            this.checkHideWhenMinimized = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textIconMenuProcesses = new System.Windows.Forms.NumericUpDown();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.textMaxSamples = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.comboElevationLevel = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.checkEnableExperimentalFeatures = new System.Windows.Forms.CheckBox();
            this.checkHidePhConnections = new System.Windows.Forms.CheckBox();
            this.buttonChangeReplaceTaskManager = new System.Windows.Forms.Button();
            this.checkReplaceTaskManager = new System.Windows.Forms.CheckBox();
            this.checkEnableKPH = new System.Windows.Forms.CheckBox();
            this.checkHideHandlesWithNoName = new System.Windows.Forms.CheckBox();
            this.checkVerifySignatures = new System.Windows.Forms.CheckBox();
            this.tabHighlighting = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonDisableAll = new System.Windows.Forms.Button();
            this.buttonEnableAll = new System.Windows.Forms.Button();
            this.listHighlightingColors = new ProcessHacker.Components.ExtendedListView();
            this.columnDescription = new System.Windows.Forms.ColumnHeader();
            this.textHighlightingDuration = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.colorRemovedProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorNewProcesses = new ProcessHacker.Components.ColorModifier();
            this.tabPlotting = new System.Windows.Forms.TabPage();
            this.textStep = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
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
            this.tabSymbols = new System.Windows.Forms.TabPage();
            this.checkUndecorate = new System.Windows.Forms.CheckBox();
            this.textSearchPath = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonDbghelpBrowse = new System.Windows.Forms.Button();
            this.textDbghelpPath = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabUpdates = new System.Windows.Forms.TabPage();
            this.checkUpdateAutomatically = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.optUpdateStable = new System.Windows.Forms.RadioButton();
            this.optUpdateBeta = new System.Windows.Forms.RadioButton();
            this.optUpdateAlpha = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.textUpdateInterval)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textIconMenuProcesses)).BeginInit();
            this.tabAdvanced.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textMaxSamples)).BeginInit();
            this.tabHighlighting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textHighlightingDuration)).BeginInit();
            this.tabPlotting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textStep)).BeginInit();
            this.tabSymbols.SuspendLayout();
            this.tabUpdates.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Update Interval:";
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
            this.textUpdateInterval.TabIndex = 0;
            this.textUpdateInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.textUpdateInterval.Leave += new System.EventHandler(this.textUpdateInterval_Leave);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(264, 350);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkShowProcessDomains
            // 
            this.checkShowProcessDomains.AutoSize = true;
            this.checkShowProcessDomains.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowProcessDomains.Location = new System.Drawing.Point(211, 246);
            this.checkShowProcessDomains.Name = "checkShowProcessDomains";
            this.checkShowProcessDomains.Size = new System.Drawing.Size(156, 18);
            this.checkShowProcessDomains.TabIndex = 12;
            this.checkShowProcessDomains.Text = "Show user/group domains";
            this.checkShowProcessDomains.UseVisualStyleBackColor = true;
            // 
            // checkWarnDangerous
            // 
            this.checkWarnDangerous.AutoSize = true;
            this.checkWarnDangerous.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkWarnDangerous.Location = new System.Drawing.Point(6, 102);
            this.checkWarnDangerous.Name = "checkWarnDangerous";
            this.checkWarnDangerous.Size = new System.Drawing.Size(228, 18);
            this.checkWarnDangerous.TabIndex = 5;
            this.checkWarnDangerous.Text = "Warn about potentially dangerous actions";
            this.checkWarnDangerous.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Search Engine:";
            // 
            // textSearchEngine
            // 
            this.textSearchEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchEngine.Location = new System.Drawing.Point(134, 58);
            this.textSearchEngine.Name = "textSearchEngine";
            this.textSearchEngine.Size = new System.Drawing.Size(341, 20);
            this.textSearchEngine.TabIndex = 2;
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
            this.tabControl.Controls.Add(this.tabSymbols);
            this.tabControl.Controls.Add(this.tabUpdates);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(489, 332);
            this.tabControl.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.label20);
            this.tabGeneral.Controls.Add(this.comboToolbarStyle);
            this.tabGeneral.Controls.Add(this.checkFloatChildWindows);
            this.tabGeneral.Controls.Add(this.checkScrollDownProcessTree);
            this.tabGeneral.Controls.Add(this.checkAllowOnlyOneInstance);
            this.tabGeneral.Controls.Add(this.buttonFont);
            this.tabGeneral.Controls.Add(this.textImposterNames);
            this.tabGeneral.Controls.Add(this.label21);
            this.tabGeneral.Controls.Add(this.comboSizeUnits);
            this.tabGeneral.Controls.Add(this.label18);
            this.tabGeneral.Controls.Add(this.checkStartHidden);
            this.tabGeneral.Controls.Add(this.checkHideWhenClosed);
            this.tabGeneral.Controls.Add(this.checkHideWhenMinimized);
            this.tabGeneral.Controls.Add(this.label23);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.textSearchEngine);
            this.tabGeneral.Controls.Add(this.textIconMenuProcesses);
            this.tabGeneral.Controls.Add(this.textUpdateInterval);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.checkShowProcessDomains);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(481, 306);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 140);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(109, 13);
            this.label20.TabIndex = 19;
            this.label20.Text = "Toolbar Display Style:";
            // 
            // comboToolbarStyle
            // 
            this.comboToolbarStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboToolbarStyle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboToolbarStyle.FormattingEnabled = true;
            this.comboToolbarStyle.Items.AddRange(new object[] {
            "Show Only Icons",
            "Show Selective Text",
            "Show All Text Labels"});
            this.comboToolbarStyle.Location = new System.Drawing.Point(134, 137);
            this.comboToolbarStyle.Name = "comboToolbarStyle";
            this.comboToolbarStyle.Size = new System.Drawing.Size(135, 21);
            this.comboToolbarStyle.TabIndex = 18;
            // 
            // checkFloatChildWindows
            // 
            this.checkFloatChildWindows.AutoSize = true;
            this.checkFloatChildWindows.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkFloatChildWindows.Location = new System.Drawing.Point(211, 199);
            this.checkFloatChildWindows.Name = "checkFloatChildWindows";
            this.checkFloatChildWindows.Size = new System.Drawing.Size(124, 18);
            this.checkFloatChildWindows.TabIndex = 10;
            this.checkFloatChildWindows.Text = "Float child windows";
            this.checkFloatChildWindows.UseVisualStyleBackColor = true;
            // 
            // checkScrollDownProcessTree
            // 
            this.checkScrollDownProcessTree.AutoSize = true;
            this.checkScrollDownProcessTree.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkScrollDownProcessTree.Location = new System.Drawing.Point(211, 222);
            this.checkScrollDownProcessTree.Name = "checkScrollDownProcessTree";
            this.checkScrollDownProcessTree.Size = new System.Drawing.Size(213, 18);
            this.checkScrollDownProcessTree.TabIndex = 11;
            this.checkScrollDownProcessTree.Text = "Scroll down the process tree at startup";
            this.checkScrollDownProcessTree.UseVisualStyleBackColor = true;
            // 
            // checkAllowOnlyOneInstance
            // 
            this.checkAllowOnlyOneInstance.AutoSize = true;
            this.checkAllowOnlyOneInstance.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkAllowOnlyOneInstance.Location = new System.Drawing.Point(6, 270);
            this.checkAllowOnlyOneInstance.Name = "checkAllowOnlyOneInstance";
            this.checkAllowOnlyOneInstance.Size = new System.Drawing.Size(143, 18);
            this.checkAllowOnlyOneInstance.TabIndex = 9;
            this.checkAllowOnlyOneInstance.Text = "Allow only one instance";
            this.checkAllowOnlyOneInstance.UseVisualStyleBackColor = true;
            // 
            // buttonFont
            // 
            this.buttonFont.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonFont.Location = new System.Drawing.Point(6, 169);
            this.buttonFont.Name = "buttonFont";
            this.buttonFont.Size = new System.Drawing.Size(75, 23);
            this.buttonFont.TabIndex = 5;
            this.buttonFont.Text = "Font...";
            this.buttonFont.UseVisualStyleBackColor = true;
            this.buttonFont.Click += new System.EventHandler(this.buttonFont_Click);
            // 
            // textImposterNames
            // 
            this.textImposterNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textImposterNames.Location = new System.Drawing.Point(134, 84);
            this.textImposterNames.Name = "textImposterNames";
            this.textImposterNames.Size = new System.Drawing.Size(341, 20);
            this.textImposterNames.TabIndex = 3;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 87);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(100, 13);
            this.label21.TabIndex = 16;
            this.label21.Text = "Require Signatures:";
            // 
            // comboSizeUnits
            // 
            this.comboSizeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSizeUnits.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
            this.comboSizeUnits.TabIndex = 4;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 113);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(78, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "Max. Size Unit:";
            // 
            // checkStartHidden
            // 
            this.checkStartHidden.AutoSize = true;
            this.checkStartHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkStartHidden.Location = new System.Drawing.Point(6, 246);
            this.checkStartHidden.Name = "checkStartHidden";
            this.checkStartHidden.Size = new System.Drawing.Size(89, 18);
            this.checkStartHidden.TabIndex = 8;
            this.checkStartHidden.Text = "Start hidden";
            this.checkStartHidden.UseVisualStyleBackColor = true;
            // 
            // checkHideWhenClosed
            // 
            this.checkHideWhenClosed.AutoSize = true;
            this.checkHideWhenClosed.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenClosed.Location = new System.Drawing.Point(6, 222);
            this.checkHideWhenClosed.Name = "checkHideWhenClosed";
            this.checkHideWhenClosed.Size = new System.Drawing.Size(117, 18);
            this.checkHideWhenClosed.TabIndex = 7;
            this.checkHideWhenClosed.Text = "Hide when closed";
            this.checkHideWhenClosed.UseVisualStyleBackColor = true;
            // 
            // checkHideWhenMinimized
            // 
            this.checkHideWhenMinimized.AutoSize = true;
            this.checkHideWhenMinimized.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenMinimized.Location = new System.Drawing.Point(6, 198);
            this.checkHideWhenMinimized.Name = "checkHideWhenMinimized";
            this.checkHideWhenMinimized.Size = new System.Drawing.Size(131, 18);
            this.checkHideWhenMinimized.TabIndex = 6;
            this.checkHideWhenMinimized.Text = "Hide when minimized";
            this.checkHideWhenMinimized.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 34);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(122, 13);
            this.label23.TabIndex = 14;
            this.label23.Text = "Processes in icon menu:";
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
            this.tabAdvanced.Controls.Add(this.textMaxSamples);
            this.tabAdvanced.Controls.Add(this.label6);
            this.tabAdvanced.Controls.Add(this.comboElevationLevel);
            this.tabAdvanced.Controls.Add(this.label22);
            this.tabAdvanced.Controls.Add(this.checkEnableExperimentalFeatures);
            this.tabAdvanced.Controls.Add(this.checkHidePhConnections);
            this.tabAdvanced.Controls.Add(this.buttonChangeReplaceTaskManager);
            this.tabAdvanced.Controls.Add(this.checkReplaceTaskManager);
            this.tabAdvanced.Controls.Add(this.checkWarnDangerous);
            this.tabAdvanced.Controls.Add(this.checkEnableKPH);
            this.tabAdvanced.Controls.Add(this.checkHideHandlesWithNoName);
            this.tabAdvanced.Controls.Add(this.checkVerifySignatures);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(481, 333);
            this.tabAdvanced.TabIndex = 3;
            this.tabAdvanced.Text = "Advanced";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // textMaxSamples
            // 
            this.textMaxSamples.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.textMaxSamples.Location = new System.Drawing.Point(118, 174);
            this.textMaxSamples.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textMaxSamples.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textMaxSamples.Name = "textMaxSamples";
            this.textMaxSamples.Size = new System.Drawing.Size(72, 20);
            this.textMaxSamples.TabIndex = 22;
            this.textMaxSamples.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Max. Sample History:";
            // 
            // comboElevationLevel
            // 
            this.comboElevationLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboElevationLevel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboElevationLevel.FormattingEnabled = true;
            this.comboElevationLevel.Items.AddRange(new object[] {
            "Never elevate",
            "Prompt for elevation",
            "Always elevate"});
            this.comboElevationLevel.Location = new System.Drawing.Point(66, 200);
            this.comboElevationLevel.Name = "comboElevationLevel";
            this.comboElevationLevel.Size = new System.Drawing.Size(194, 21);
            this.comboElevationLevel.TabIndex = 11;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 203);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(54, 13);
            this.label22.TabIndex = 10;
            this.label22.Text = "Elevation:";
            // 
            // checkEnableExperimentalFeatures
            // 
            this.checkEnableExperimentalFeatures.AutoSize = true;
            this.checkEnableExperimentalFeatures.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkEnableExperimentalFeatures.Location = new System.Drawing.Point(6, 30);
            this.checkEnableExperimentalFeatures.Name = "checkEnableExperimentalFeatures";
            this.checkEnableExperimentalFeatures.Size = new System.Drawing.Size(168, 18);
            this.checkEnableExperimentalFeatures.TabIndex = 1;
            this.checkEnableExperimentalFeatures.Text = "Enable experimental features";
            this.checkEnableExperimentalFeatures.UseVisualStyleBackColor = true;
            // 
            // checkHidePhConnections
            // 
            this.checkHidePhConnections.AutoSize = true;
            this.checkHidePhConnections.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHidePhConnections.Location = new System.Drawing.Point(6, 150);
            this.checkHidePhConnections.Name = "checkHidePhConnections";
            this.checkHidePhConnections.Size = new System.Drawing.Size(235, 18);
            this.checkHidePhConnections.TabIndex = 7;
            this.checkHidePhConnections.Text = "Hide Process Hacker network connections";
            this.checkHidePhConnections.UseVisualStyleBackColor = true;
            // 
            // buttonChangeReplaceTaskManager
            // 
            this.buttonChangeReplaceTaskManager.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonChangeReplaceTaskManager.Location = new System.Drawing.Point(257, 75);
            this.buttonChangeReplaceTaskManager.Name = "buttonChangeReplaceTaskManager";
            this.buttonChangeReplaceTaskManager.Size = new System.Drawing.Size(89, 23);
            this.buttonChangeReplaceTaskManager.TabIndex = 4;
            this.buttonChangeReplaceTaskManager.Text = "Change...";
            this.buttonChangeReplaceTaskManager.UseVisualStyleBackColor = true;
            this.buttonChangeReplaceTaskManager.Click += new System.EventHandler(this.buttonChangeReplaceTaskManager_Click);
            // 
            // checkReplaceTaskManager
            // 
            this.checkReplaceTaskManager.AutoSize = true;
            this.checkReplaceTaskManager.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkReplaceTaskManager.Location = new System.Drawing.Point(6, 78);
            this.checkReplaceTaskManager.Name = "checkReplaceTaskManager";
            this.checkReplaceTaskManager.Size = new System.Drawing.Size(245, 18);
            this.checkReplaceTaskManager.TabIndex = 3;
            this.checkReplaceTaskManager.Text = "Replace Task Manager with Process Hacker";
            this.checkReplaceTaskManager.UseVisualStyleBackColor = true;
            // 
            // checkEnableKPH
            // 
            this.checkEnableKPH.AutoSize = true;
            this.checkEnableKPH.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkEnableKPH.Location = new System.Drawing.Point(6, 6);
            this.checkEnableKPH.Name = "checkEnableKPH";
            this.checkEnableKPH.Size = new System.Drawing.Size(155, 18);
            this.checkEnableKPH.TabIndex = 0;
            this.checkEnableKPH.Text = "Enable kernel-mode driver";
            this.checkEnableKPH.UseVisualStyleBackColor = true;
            // 
            // checkHideHandlesWithNoName
            // 
            this.checkHideHandlesWithNoName.AutoSize = true;
            this.checkHideHandlesWithNoName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideHandlesWithNoName.Location = new System.Drawing.Point(6, 126);
            this.checkHideHandlesWithNoName.Name = "checkHideHandlesWithNoName";
            this.checkHideHandlesWithNoName.Size = new System.Drawing.Size(160, 18);
            this.checkHideHandlesWithNoName.TabIndex = 6;
            this.checkHideHandlesWithNoName.Text = "Hide handles with no name";
            this.checkHideHandlesWithNoName.UseVisualStyleBackColor = true;
            // 
            // checkVerifySignatures
            // 
            this.checkVerifySignatures.AutoSize = true;
            this.checkVerifySignatures.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkVerifySignatures.Location = new System.Drawing.Point(6, 54);
            this.checkVerifySignatures.Name = "checkVerifySignatures";
            this.checkVerifySignatures.Size = new System.Drawing.Size(254, 18);
            this.checkVerifySignatures.TabIndex = 2;
            this.checkVerifySignatures.Text = "Verify signatures and perform additional checks";
            this.checkVerifySignatures.UseVisualStyleBackColor = true;
            // 
            // tabHighlighting
            // 
            this.tabHighlighting.Controls.Add(this.label11);
            this.tabHighlighting.Controls.Add(this.buttonDisableAll);
            this.tabHighlighting.Controls.Add(this.buttonEnableAll);
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
            this.tabHighlighting.Size = new System.Drawing.Size(481, 333);
            this.tabHighlighting.TabIndex = 1;
            this.tabHighlighting.Text = "Highlighting";
            this.tabHighlighting.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 309);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(165, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Double-click an item to change it.";
            // 
            // buttonDisableAll
            // 
            this.buttonDisableAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDisableAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDisableAll.Location = new System.Drawing.Point(400, 304);
            this.buttonDisableAll.Name = "buttonDisableAll";
            this.buttonDisableAll.Size = new System.Drawing.Size(75, 23);
            this.buttonDisableAll.TabIndex = 5;
            this.buttonDisableAll.Text = "&Disable All";
            this.buttonDisableAll.UseVisualStyleBackColor = true;
            this.buttonDisableAll.Click += new System.EventHandler(this.buttonDisableAll_Click);
            // 
            // buttonEnableAll
            // 
            this.buttonEnableAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnableAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonEnableAll.Location = new System.Drawing.Point(319, 304);
            this.buttonEnableAll.Name = "buttonEnableAll";
            this.buttonEnableAll.Size = new System.Drawing.Size(75, 23);
            this.buttonEnableAll.TabIndex = 4;
            this.buttonEnableAll.Text = "&Enable All";
            this.buttonEnableAll.UseVisualStyleBackColor = true;
            this.buttonEnableAll.Click += new System.EventHandler(this.buttonEnableAll_Click);
            // 
            // listHighlightingColors
            // 
            this.listHighlightingColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listHighlightingColors.CheckBoxes = true;
            this.listHighlightingColors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDescription});
            this.listHighlightingColors.DoubleClickChecks = false;
            this.listHighlightingColors.FullRowSelect = true;
            this.listHighlightingColors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listHighlightingColors.HideSelection = false;
            this.listHighlightingColors.Location = new System.Drawing.Point(6, 60);
            this.listHighlightingColors.MultiSelect = false;
            this.listHighlightingColors.Name = "listHighlightingColors";
            this.listHighlightingColors.ShowItemToolTips = true;
            this.listHighlightingColors.Size = new System.Drawing.Size(469, 238);
            this.listHighlightingColors.TabIndex = 3;
            this.listHighlightingColors.UseCompatibleStateImageBehavior = false;
            this.listHighlightingColors.View = System.Windows.Forms.View.Details;
            this.listHighlightingColors.DoubleClick += new System.EventHandler(this.listHighlightingColors_DoubleClick);
            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 250;
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
            this.textHighlightingDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textHighlightingDuration.Name = "textHighlightingDuration";
            this.textHighlightingDuration.Size = new System.Drawing.Size(66, 20);
            this.textHighlightingDuration.TabIndex = 0;
            this.textHighlightingDuration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Highlighting Duration:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(230, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Removed Objects:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "New Objects:";
            // 
            // colorRemovedProcesses
            // 
            this.colorRemovedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorRemovedProcesses.Location = new System.Drawing.Point(351, 34);
            this.colorRemovedProcesses.Name = "colorRemovedProcesses";
            this.colorRemovedProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorRemovedProcesses.TabIndex = 2;
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
            this.tabPlotting.Controls.Add(this.textStep);
            this.tabPlotting.Controls.Add(this.label8);
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
            this.tabPlotting.Size = new System.Drawing.Size(481, 333);
            this.tabPlotting.TabIndex = 2;
            this.tabPlotting.Text = "Plotting";
            this.tabPlotting.UseVisualStyleBackColor = true;
            // 
            // textStep
            // 
            this.textStep.Location = new System.Drawing.Point(44, 30);
            this.textStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textStep.Name = "textStep";
            this.textStep.Size = new System.Drawing.Size(69, 20);
            this.textStep.TabIndex = 1;
            this.textStep.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Step:";
            // 
            // checkPlotterAntialias
            // 
            this.checkPlotterAntialias.AutoSize = true;
            this.checkPlotterAntialias.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkPlotterAntialias.Location = new System.Drawing.Point(6, 6);
            this.checkPlotterAntialias.Name = "checkPlotterAntialias";
            this.checkPlotterAntialias.Size = new System.Drawing.Size(110, 18);
            this.checkPlotterAntialias.TabIndex = 0;
            this.checkPlotterAntialias.Text = "Use Anti-aliasing";
            this.checkPlotterAntialias.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 165);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "I/O Reads+Other:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 190);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 13);
            this.label13.TabIndex = 14;
            this.label13.Text = "I/O Writes:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 139);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Working Set:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 113);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Private Bytes:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 88);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(83, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "CPU User Time:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 62);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 13);
            this.label17.TabIndex = 9;
            this.label17.Text = "CPU Kernel Time:";
            // 
            // colorIORO
            // 
            this.colorIORO.Color = System.Drawing.Color.Transparent;
            this.colorIORO.Location = new System.Drawing.Point(124, 163);
            this.colorIORO.Name = "colorIORO";
            this.colorIORO.Size = new System.Drawing.Size(40, 20);
            this.colorIORO.TabIndex = 6;
            // 
            // colorIOW
            // 
            this.colorIOW.Color = System.Drawing.Color.Transparent;
            this.colorIOW.Location = new System.Drawing.Point(124, 189);
            this.colorIOW.Name = "colorIOW";
            this.colorIOW.Size = new System.Drawing.Size(40, 20);
            this.colorIOW.TabIndex = 7;
            // 
            // colorMemoryWS
            // 
            this.colorMemoryWS.Color = System.Drawing.Color.Transparent;
            this.colorMemoryWS.Location = new System.Drawing.Point(124, 137);
            this.colorMemoryWS.Name = "colorMemoryWS";
            this.colorMemoryWS.Size = new System.Drawing.Size(40, 20);
            this.colorMemoryWS.TabIndex = 5;
            // 
            // colorMemoryPB
            // 
            this.colorMemoryPB.Color = System.Drawing.Color.Transparent;
            this.colorMemoryPB.Location = new System.Drawing.Point(124, 111);
            this.colorMemoryPB.Name = "colorMemoryPB";
            this.colorMemoryPB.Size = new System.Drawing.Size(40, 20);
            this.colorMemoryPB.TabIndex = 4;
            // 
            // colorCPUUT
            // 
            this.colorCPUUT.Color = System.Drawing.Color.Transparent;
            this.colorCPUUT.Location = new System.Drawing.Point(124, 85);
            this.colorCPUUT.Name = "colorCPUUT";
            this.colorCPUUT.Size = new System.Drawing.Size(40, 20);
            this.colorCPUUT.TabIndex = 3;
            // 
            // colorCPUKT
            // 
            this.colorCPUKT.Color = System.Drawing.Color.Transparent;
            this.colorCPUKT.Location = new System.Drawing.Point(124, 59);
            this.colorCPUKT.Name = "colorCPUKT";
            this.colorCPUKT.Size = new System.Drawing.Size(40, 20);
            this.colorCPUKT.TabIndex = 2;
            // 
            // tabSymbols
            // 
            this.tabSymbols.Controls.Add(this.checkUndecorate);
            this.tabSymbols.Controls.Add(this.textSearchPath);
            this.tabSymbols.Controls.Add(this.label10);
            this.tabSymbols.Controls.Add(this.buttonDbghelpBrowse);
            this.tabSymbols.Controls.Add(this.textDbghelpPath);
            this.tabSymbols.Controls.Add(this.label9);
            this.tabSymbols.Location = new System.Drawing.Point(4, 22);
            this.tabSymbols.Name = "tabSymbols";
            this.tabSymbols.Padding = new System.Windows.Forms.Padding(3);
            this.tabSymbols.Size = new System.Drawing.Size(481, 333);
            this.tabSymbols.TabIndex = 4;
            this.tabSymbols.Text = "Symbols";
            this.tabSymbols.UseVisualStyleBackColor = true;
            // 
            // checkUndecorate
            // 
            this.checkUndecorate.AutoSize = true;
            this.checkUndecorate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkUndecorate.Location = new System.Drawing.Point(6, 60);
            this.checkUndecorate.Name = "checkUndecorate";
            this.checkUndecorate.Size = new System.Drawing.Size(128, 18);
            this.checkUndecorate.TabIndex = 3;
            this.checkUndecorate.Text = "Undecorate symbols";
            this.checkUndecorate.UseVisualStyleBackColor = true;
            // 
            // textSearchPath
            // 
            this.textSearchPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchPath.Location = new System.Drawing.Point(99, 34);
            this.textSearchPath.Name = "textSearchPath";
            this.textSearchPath.Size = new System.Drawing.Size(376, 20);
            this.textSearchPath.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Search path:";
            // 
            // buttonDbghelpBrowse
            // 
            this.buttonDbghelpBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDbghelpBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDbghelpBrowse.Location = new System.Drawing.Point(400, 6);
            this.buttonDbghelpBrowse.Name = "buttonDbghelpBrowse";
            this.buttonDbghelpBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonDbghelpBrowse.TabIndex = 1;
            this.buttonDbghelpBrowse.Text = "Browse...";
            this.buttonDbghelpBrowse.UseVisualStyleBackColor = true;
            this.buttonDbghelpBrowse.Click += new System.EventHandler(this.buttonDbghelpBrowse_Click);
            // 
            // textDbghelpPath
            // 
            this.textDbghelpPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDbghelpPath.Location = new System.Drawing.Point(99, 8);
            this.textDbghelpPath.Name = "textDbghelpPath";
            this.textDbghelpPath.Size = new System.Drawing.Size(295, 20);
            this.textDbghelpPath.TabIndex = 0;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Dbghelp.dll path:";
            // 
            // tabUpdates
            // 
            this.tabUpdates.Controls.Add(this.checkUpdateAutomatically);
            this.tabUpdates.Controls.Add(this.label5);
            this.tabUpdates.Controls.Add(this.optUpdateStable);
            this.tabUpdates.Controls.Add(this.optUpdateBeta);
            this.tabUpdates.Controls.Add(this.optUpdateAlpha);
            this.tabUpdates.Location = new System.Drawing.Point(4, 22);
            this.tabUpdates.Name = "tabUpdates";
            this.tabUpdates.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpdates.Size = new System.Drawing.Size(481, 333);
            this.tabUpdates.TabIndex = 5;
            this.tabUpdates.Text = "Updates";
            this.tabUpdates.UseVisualStyleBackColor = true;
            // 
            // checkUpdateAutomatically
            // 
            this.checkUpdateAutomatically.AutoSize = true;
            this.checkUpdateAutomatically.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkUpdateAutomatically.Location = new System.Drawing.Point(6, 6);
            this.checkUpdateAutomatically.Name = "checkUpdateAutomatically";
            this.checkUpdateAutomatically.Size = new System.Drawing.Size(186, 18);
            this.checkUpdateAutomatically.TabIndex = 27;
            this.checkUpdateAutomatically.Text = " Check for updates automatically";
            this.checkUpdateAutomatically.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Check for:";
            // 
            // optUpdateStable
            // 
            this.optUpdateStable.AutoSize = true;
            this.optUpdateStable.Checked = true;
            this.optUpdateStable.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.optUpdateStable.Location = new System.Drawing.Point(68, 30);
            this.optUpdateStable.Name = "optUpdateStable";
            this.optUpdateStable.Size = new System.Drawing.Size(103, 18);
            this.optUpdateStable.TabIndex = 21;
            this.optUpdateStable.TabStop = true;
            this.optUpdateStable.Text = "Stable releases";
            this.optUpdateStable.UseVisualStyleBackColor = true;
            // 
            // optUpdateBeta
            // 
            this.optUpdateBeta.AutoSize = true;
            this.optUpdateBeta.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.optUpdateBeta.Location = new System.Drawing.Point(68, 54);
            this.optUpdateBeta.Name = "optUpdateBeta";
            this.optUpdateBeta.Size = new System.Drawing.Size(95, 18);
            this.optUpdateBeta.TabIndex = 22;
            this.optUpdateBeta.Text = "Beta releases";
            this.optUpdateBeta.UseVisualStyleBackColor = true;
            // 
            // optUpdateAlpha
            // 
            this.optUpdateAlpha.AutoSize = true;
            this.optUpdateAlpha.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.optUpdateAlpha.Location = new System.Drawing.Point(68, 78);
            this.optUpdateAlpha.Name = "optUpdateAlpha";
            this.optUpdateAlpha.Size = new System.Drawing.Size(100, 18);
            this.optUpdateAlpha.TabIndex = 23;
            this.optUpdateAlpha.Text = "Alpha releases";
            this.optUpdateAlpha.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(345, 350);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Enabled = false;
            this.buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonApply.Location = new System.Drawing.Point(426, 350);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = "&Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReset.Location = new System.Drawing.Point(12, 350);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 4;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // OptionsWindow
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 385);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.textUpdateInterval)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textIconMenuProcesses)).EndInit();
            this.tabAdvanced.ResumeLayout(false);
            this.tabAdvanced.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textMaxSamples)).EndInit();
            this.tabHighlighting.ResumeLayout(false);
            this.tabHighlighting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textHighlightingDuration)).EndInit();
            this.tabPlotting.ResumeLayout(false);
            this.tabPlotting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textStep)).EndInit();
            this.tabSymbols.ResumeLayout(false);
            this.tabSymbols.PerformLayout();
            this.tabUpdates.ResumeLayout(false);
            this.tabUpdates.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown textUpdateInterval;
        private System.Windows.Forms.Button buttonOK;
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
        private System.Windows.Forms.NumericUpDown textIconMenuProcesses;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.CheckBox checkHideWhenClosed;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.CheckBox checkReplaceTaskManager;
        private System.Windows.Forms.CheckBox checkEnableKPH;
        private System.Windows.Forms.CheckBox checkHideHandlesWithNoName;
        private System.Windows.Forms.CheckBox checkVerifySignatures;
        private ProcessHacker.Components.ExtendedListView listHighlightingColors;
        private System.Windows.Forms.ColumnHeader columnDescription;
        private System.Windows.Forms.Button buttonChangeReplaceTaskManager;
        private System.Windows.Forms.CheckBox checkAllowOnlyOneInstance;
        private System.Windows.Forms.NumericUpDown textStep;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonDisableAll;
        private System.Windows.Forms.Button buttonEnableAll;
        private System.Windows.Forms.TabPage tabSymbols;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textSearchPath;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonDbghelpBrowse;
        private System.Windows.Forms.TextBox textDbghelpPath;
        private System.Windows.Forms.CheckBox checkUndecorate;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.CheckBox checkHidePhConnections;
        private System.Windows.Forms.CheckBox checkEnableExperimentalFeatures;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkScrollDownProcessTree;
        private System.Windows.Forms.CheckBox checkFloatChildWindows;
        private System.Windows.Forms.TabPage tabUpdates;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton optUpdateAlpha;
        private System.Windows.Forms.RadioButton optUpdateBeta;
        private System.Windows.Forms.RadioButton optUpdateStable;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comboToolbarStyle;
        private System.Windows.Forms.CheckBox checkUpdateAutomatically;
        private System.Windows.Forms.ComboBox comboElevationLevel;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.NumericUpDown textMaxSamples;
        private System.Windows.Forms.Label label6;
    }
}