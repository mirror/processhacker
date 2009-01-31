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
            this.checkHideHandlesWithNoName = new System.Windows.Forms.CheckBox();
            this.checkEnableKPH = new System.Windows.Forms.CheckBox();
            this.comboSizeUnits = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkStartHidden = new System.Windows.Forms.CheckBox();
            this.checkHideWhenMinimized = new System.Windows.Forms.CheckBox();
            this.checkShowTrayIcon = new System.Windows.Forms.CheckBox();
            this.checkVerifySignatures = new System.Windows.Forms.CheckBox();
            this.tabHighlighting = new System.Windows.Forms.TabPage();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textHighlightingDuration = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.colorPackedProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorDotNetProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorJobProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorElevatedProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorServiceProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorBeingDebugged = new ProcessHacker.Components.ColorModifier();
            this.colorSystemProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorOwnProcesses = new ProcessHacker.Components.ColorModifier();
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
            this.toolTipProvider = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.textUpdateInterval)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
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
            this.textUpdateInterval.Location = new System.Drawing.Point(112, 6);
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
            this.buttonClose.Location = new System.Drawing.Point(362, 382);
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
            this.checkShowProcessDomains.Location = new System.Drawing.Point(6, 235);
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
            this.checkWarnDangerous.Location = new System.Drawing.Point(6, 211);
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
            this.label2.Location = new System.Drawing.Point(6, 35);
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
            this.textSearchEngine.Location = new System.Drawing.Point(112, 32);
            this.textSearchEngine.Name = "textSearchEngine";
            this.textSearchEngine.Size = new System.Drawing.Size(299, 20);
            this.textSearchEngine.TabIndex = 6;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabHighlighting);
            this.tabControl.Controls.Add(this.tabPlotting);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(425, 364);
            this.tabControl.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.buttonFont);
            this.tabGeneral.Controls.Add(this.textImposterNames);
            this.tabGeneral.Controls.Add(this.label21);
            this.tabGeneral.Controls.Add(this.checkHideHandlesWithNoName);
            this.tabGeneral.Controls.Add(this.checkEnableKPH);
            this.tabGeneral.Controls.Add(this.comboSizeUnits);
            this.tabGeneral.Controls.Add(this.label18);
            this.tabGeneral.Controls.Add(this.checkStartHidden);
            this.tabGeneral.Controls.Add(this.checkHideWhenMinimized);
            this.tabGeneral.Controls.Add(this.checkShowTrayIcon);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.textSearchEngine);
            this.tabGeneral.Controls.Add(this.textUpdateInterval);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.checkVerifySignatures);
            this.tabGeneral.Controls.Add(this.checkShowProcessDomains);
            this.tabGeneral.Controls.Add(this.checkWarnDangerous);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(417, 338);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // buttonFont
            // 
            this.buttonFont.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonFont.Location = new System.Drawing.Point(6, 110);
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
            this.textImposterNames.Location = new System.Drawing.Point(112, 58);
            this.textImposterNames.Name = "textImposterNames";
            this.textImposterNames.Size = new System.Drawing.Size(299, 20);
            this.textImposterNames.TabIndex = 14;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 61);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(100, 13);
            this.label21.TabIndex = 13;
            this.label21.Text = "Require Signatures:";
            this.toolTipProvider.SetToolTip(this.label21, "A comma-separated list of process names to require valid signatures for.");
            // 
            // checkHideHandlesWithNoName
            // 
            this.checkHideHandlesWithNoName.AutoSize = true;
            this.checkHideHandlesWithNoName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideHandlesWithNoName.Location = new System.Drawing.Point(6, 283);
            this.checkHideHandlesWithNoName.Name = "checkHideHandlesWithNoName";
            this.checkHideHandlesWithNoName.Size = new System.Drawing.Size(160, 18);
            this.checkHideHandlesWithNoName.TabIndex = 12;
            this.checkHideHandlesWithNoName.Text = "Hide handles with no name";
            this.toolTipProvider.SetToolTip(this.checkHideHandlesWithNoName, "Hides unnamed handles by default. This can be changed in each process window.");
            this.checkHideHandlesWithNoName.UseVisualStyleBackColor = true;
            // 
            // checkEnableKPH
            // 
            this.checkEnableKPH.AutoSize = true;
            this.checkEnableKPH.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkEnableKPH.Location = new System.Drawing.Point(6, 307);
            this.checkEnableKPH.Name = "checkEnableKPH";
            this.checkEnableKPH.Size = new System.Drawing.Size(217, 18);
            this.checkEnableKPH.TabIndex = 12;
            this.checkEnableKPH.Text = "Enable experimental kernel-mode driver";
            this.toolTipProvider.SetToolTip(this.checkEnableKPH, "Enables the experimental driver which allows Process Hacker to bypass rootkits an" +
                    "d security software to a certain extent.");
            this.checkEnableKPH.UseVisualStyleBackColor = true;
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
            this.comboSizeUnits.Location = new System.Drawing.Point(112, 84);
            this.comboSizeUnits.Name = "comboSizeUnits";
            this.comboSizeUnits.Size = new System.Drawing.Size(67, 21);
            this.comboSizeUnits.TabIndex = 11;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 87);
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
            this.checkStartHidden.Location = new System.Drawing.Point(21, 187);
            this.checkStartHidden.Name = "checkStartHidden";
            this.checkStartHidden.Size = new System.Drawing.Size(89, 18);
            this.checkStartHidden.TabIndex = 9;
            this.checkStartHidden.Text = "Start hidden";
            this.toolTipProvider.SetToolTip(this.checkStartHidden, "Starts Process Hacker hidden. To restore it, double-click on the notification ico" +
                    "n.");
            this.checkStartHidden.UseVisualStyleBackColor = true;
            // 
            // checkHideWhenMinimized
            // 
            this.checkHideWhenMinimized.AutoSize = true;
            this.checkHideWhenMinimized.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenMinimized.Location = new System.Drawing.Point(21, 163);
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
            this.checkShowTrayIcon.Location = new System.Drawing.Point(6, 139);
            this.checkShowTrayIcon.Name = "checkShowTrayIcon";
            this.checkShowTrayIcon.Size = new System.Drawing.Size(136, 18);
            this.checkShowTrayIcon.TabIndex = 7;
            this.checkShowTrayIcon.Text = "Show notification icon";
            this.toolTipProvider.SetToolTip(this.checkShowTrayIcon, "A notification icon which can be configured to alert you of events.");
            this.checkShowTrayIcon.UseVisualStyleBackColor = true;
            this.checkShowTrayIcon.CheckedChanged += new System.EventHandler(this.checkShowTrayIcon_CheckedChanged);
            // 
            // checkVerifySignatures
            // 
            this.checkVerifySignatures.AutoSize = true;
            this.checkVerifySignatures.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkVerifySignatures.Location = new System.Drawing.Point(6, 259);
            this.checkVerifySignatures.Name = "checkVerifySignatures";
            this.checkVerifySignatures.Size = new System.Drawing.Size(254, 18);
            this.checkVerifySignatures.TabIndex = 3;
            this.checkVerifySignatures.Text = "Verify signatures and perform additional checks";
            this.toolTipProvider.SetToolTip(this.checkVerifySignatures, "Verifies the digital signatures of all processes and highlights certain types of " +
                    "suspicious processes.");
            this.checkVerifySignatures.UseVisualStyleBackColor = true;
            // 
            // tabHighlighting
            // 
            this.tabHighlighting.Controls.Add(this.label20);
            this.tabHighlighting.Controls.Add(this.label19);
            this.tabHighlighting.Controls.Add(this.label11);
            this.tabHighlighting.Controls.Add(this.label10);
            this.tabHighlighting.Controls.Add(this.label9);
            this.tabHighlighting.Controls.Add(this.label8);
            this.tabHighlighting.Controls.Add(this.textHighlightingDuration);
            this.tabHighlighting.Controls.Add(this.label7);
            this.tabHighlighting.Controls.Add(this.label6);
            this.tabHighlighting.Controls.Add(this.label5);
            this.tabHighlighting.Controls.Add(this.label4);
            this.tabHighlighting.Controls.Add(this.label3);
            this.tabHighlighting.Controls.Add(this.colorPackedProcesses);
            this.tabHighlighting.Controls.Add(this.colorDotNetProcesses);
            this.tabHighlighting.Controls.Add(this.colorJobProcesses);
            this.tabHighlighting.Controls.Add(this.colorElevatedProcesses);
            this.tabHighlighting.Controls.Add(this.colorServiceProcesses);
            this.tabHighlighting.Controls.Add(this.colorBeingDebugged);
            this.tabHighlighting.Controls.Add(this.colorSystemProcesses);
            this.tabHighlighting.Controls.Add(this.colorOwnProcesses);
            this.tabHighlighting.Controls.Add(this.colorRemovedProcesses);
            this.tabHighlighting.Controls.Add(this.colorNewProcesses);
            this.tabHighlighting.Location = new System.Drawing.Point(4, 22);
            this.tabHighlighting.Name = "tabHighlighting";
            this.tabHighlighting.Padding = new System.Windows.Forms.Padding(3);
            this.tabHighlighting.Size = new System.Drawing.Size(417, 338);
            this.tabHighlighting.TabIndex = 1;
            this.tabHighlighting.Text = "Highlighting";
            this.tabHighlighting.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(12, 387);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(300, 13);
            this.label22.TabIndex = 10;
            this.label22.Text = "Tip: Hover over an item to get a more detailed description of it.";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(214, 133);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(110, 29);
            this.label20.TabIndex = 9;
            this.label20.Text = "Packed/Dangerous Processes:";
            this.toolTipProvider.SetToolTip(this.label20, "Executables are sometimes \"packed\" to reduce their size.\r\n\"Dangerous processes\" i" +
                    "ncludes processes with invalid signatures and unverified processes with the name" +
                    " of a system process.");
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(214, 113);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 13);
            this.label19.TabIndex = 9;
            this.label19.Text = ".NET Processes:";
            this.toolTipProvider.SetToolTip(this.label19, ".NET, or managed processes. Note that \"mixed\" processes (combining managed and na" +
                    "tive code) are not shown using this color.");
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(214, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Job Processes:";
            this.toolTipProvider.SetToolTip(this.label11, "Processes associated with a job.");
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(214, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Elevated Processes:";
            this.toolTipProvider.SetToolTip(this.label10, "Processes with full privileges on a Windows Vista system with UAC enabled.");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 140);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Service Processes:";
            this.toolTipProvider.SetToolTip(this.label9, "Processes which host one or more services.");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(214, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Debugged Processes:";
            this.toolTipProvider.SetToolTip(this.label8, "Processes that are currently being debugged.");
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "System Processes:";
            this.toolTipProvider.SetToolTip(this.label6, "Processes running under the NT AUTHORITY\\SYSTEM user account.");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Own Processes:";
            this.toolTipProvider.SetToolTip(this.label5, "Processes running under the same user account as Process Hacker.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 61);
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
            // colorPackedProcesses
            // 
            this.colorPackedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorPackedProcesses.Location = new System.Drawing.Point(335, 137);
            this.colorPackedProcesses.Name = "colorPackedProcesses";
            this.colorPackedProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorPackedProcesses.TabIndex = 8;
            // 
            // colorDotNetProcesses
            // 
            this.colorDotNetProcesses.Color = System.Drawing.Color.Transparent;
            this.colorDotNetProcesses.Location = new System.Drawing.Point(335, 111);
            this.colorDotNetProcesses.Name = "colorDotNetProcesses";
            this.colorDotNetProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorDotNetProcesses.TabIndex = 8;
            // 
            // colorJobProcesses
            // 
            this.colorJobProcesses.Color = System.Drawing.Color.Transparent;
            this.colorJobProcesses.Location = new System.Drawing.Point(335, 86);
            this.colorJobProcesses.Name = "colorJobProcesses";
            this.colorJobProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorJobProcesses.TabIndex = 8;
            // 
            // colorElevatedProcesses
            // 
            this.colorElevatedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorElevatedProcesses.Location = new System.Drawing.Point(335, 60);
            this.colorElevatedProcesses.Name = "colorElevatedProcesses";
            this.colorElevatedProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorElevatedProcesses.TabIndex = 8;
            // 
            // colorServiceProcesses
            // 
            this.colorServiceProcesses.Color = System.Drawing.Color.Transparent;
            this.colorServiceProcesses.Location = new System.Drawing.Point(127, 137);
            this.colorServiceProcesses.Name = "colorServiceProcesses";
            this.colorServiceProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorServiceProcesses.TabIndex = 3;
            // 
            // colorBeingDebugged
            // 
            this.colorBeingDebugged.Color = System.Drawing.Color.Transparent;
            this.colorBeingDebugged.Location = new System.Drawing.Point(335, 34);
            this.colorBeingDebugged.Name = "colorBeingDebugged";
            this.colorBeingDebugged.Size = new System.Drawing.Size(40, 20);
            this.colorBeingDebugged.TabIndex = 3;
            // 
            // colorSystemProcesses
            // 
            this.colorSystemProcesses.Color = System.Drawing.Color.Transparent;
            this.colorSystemProcesses.Location = new System.Drawing.Point(127, 111);
            this.colorSystemProcesses.Name = "colorSystemProcesses";
            this.colorSystemProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorSystemProcesses.TabIndex = 3;
            // 
            // colorOwnProcesses
            // 
            this.colorOwnProcesses.Color = System.Drawing.Color.Transparent;
            this.colorOwnProcesses.Location = new System.Drawing.Point(127, 85);
            this.colorOwnProcesses.Name = "colorOwnProcesses";
            this.colorOwnProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorOwnProcesses.TabIndex = 3;
            // 
            // colorRemovedProcesses
            // 
            this.colorRemovedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorRemovedProcesses.Location = new System.Drawing.Point(127, 59);
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
            this.tabPlotting.Size = new System.Drawing.Size(417, 338);
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
            // toolTipProvider
            // 
            this.toolTipProvider.AutomaticDelay = 250;
            this.toolTipProvider.AutoPopDelay = 5000;
            this.toolTipProvider.InitialDelay = 250;
            this.toolTipProvider.IsBalloon = true;
            this.toolTipProvider.ReshowDelay = 50;
            this.toolTipProvider.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipProvider.ToolTipTitle = "Description";
            // 
            // OptionsWindow
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 417);
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
        private ProcessHacker.Components.ColorModifier colorSystemProcesses;
        private ProcessHacker.Components.ColorModifier colorOwnProcesses;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown textHighlightingDuration;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkShowTrayIcon;
        private System.Windows.Forms.Label label8;
        private ProcessHacker.Components.ColorModifier colorBeingDebugged;
        private System.Windows.Forms.Label label9;
        private ProcessHacker.Components.ColorModifier colorServiceProcesses;
        private System.Windows.Forms.CheckBox checkHideWhenMinimized;
        private System.Windows.Forms.Label label10;
        private ProcessHacker.Components.ColorModifier colorElevatedProcesses;
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
        private System.Windows.Forms.Label label11;
        private ProcessHacker.Components.ColorModifier colorJobProcesses;
        private System.Windows.Forms.ComboBox comboSizeUnits;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private ProcessHacker.Components.ColorModifier colorPackedProcesses;
        private ProcessHacker.Components.ColorModifier colorDotNetProcesses;
        private System.Windows.Forms.CheckBox checkVerifySignatures;
        private System.Windows.Forms.CheckBox checkEnableKPH;
        private System.Windows.Forms.CheckBox checkStartHidden;
        private System.Windows.Forms.TextBox textImposterNames;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button buttonFont;
        private System.Windows.Forms.CheckBox checkHideHandlesWithNoName;
        private System.Windows.Forms.ToolTip toolTipProvider;
        private System.Windows.Forms.Label label22;
    }
}