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
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkShowProcessDomains = new System.Windows.Forms.CheckBox();
            this.checkWarnDangerous = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textSearchEngine = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.comboSizeUnits = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkHideWhenMinimized = new System.Windows.Forms.CheckBox();
            this.checkShowTrayIcon = new System.Windows.Forms.CheckBox();
            this.checkVerifySignatures = new System.Windows.Forms.CheckBox();
            this.tabHighlighting = new System.Windows.Forms.TabPage();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.colorPackedProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorDotNetProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorJobProcesses = new ProcessHacker.Components.ColorModifier();
            this.label10 = new System.Windows.Forms.Label();
            this.colorElevatedProcesses = new ProcessHacker.Components.ColorModifier();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textHighlightingDuration = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.colorServiceProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorBeingDebugged = new ProcessHacker.Components.ColorModifier();
            this.colorSystemProcesses = new ProcessHacker.Components.ColorModifier();
            this.colorOwnProcesses = new ProcessHacker.Components.ColorModifier();
            this.label6 = new System.Windows.Forms.Label();
            this.colorRemovedProcesses = new ProcessHacker.Components.ColorModifier();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.colorNewProcesses = new ProcessHacker.Components.ColorModifier();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPlotting = new System.Windows.Forms.TabPage();
            this.checkPlotterAntialias = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.colorIORO = new ProcessHacker.Components.ColorModifier();
            this.colorIOW = new ProcessHacker.Components.ColorModifier();
            this.colorMemoryWS = new ProcessHacker.Components.ColorModifier();
            this.colorMemoryPB = new ProcessHacker.Components.ColorModifier();
            this.label14 = new System.Windows.Forms.Label();
            this.colorCPUUT = new ProcessHacker.Components.ColorModifier();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.colorCPUKT = new ProcessHacker.Components.ColorModifier();
            this.label17 = new System.Windows.Forms.Label();
            this.checkEnableKPH = new System.Windows.Forms.CheckBox();
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
            // 
            // textUpdateInterval
            // 
            this.textUpdateInterval.Increment = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.textUpdateInterval.Location = new System.Drawing.Point(95, 7);
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
            this.buttonClose.Location = new System.Drawing.Point(362, 276);
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
            this.checkShowProcessDomains.Location = new System.Drawing.Point(6, 159);
            this.checkShowProcessDomains.Name = "checkShowProcessDomains";
            this.checkShowProcessDomains.Size = new System.Drawing.Size(156, 18);
            this.checkShowProcessDomains.TabIndex = 3;
            this.checkShowProcessDomains.Text = "Show user/group domains";
            this.checkShowProcessDomains.UseVisualStyleBackColor = true;
            // 
            // checkWarnDangerous
            // 
            this.checkWarnDangerous.AutoSize = true;
            this.checkWarnDangerous.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkWarnDangerous.Location = new System.Drawing.Point(6, 135);
            this.checkWarnDangerous.Name = "checkWarnDangerous";
            this.checkWarnDangerous.Size = new System.Drawing.Size(228, 18);
            this.checkWarnDangerous.TabIndex = 4;
            this.checkWarnDangerous.Text = "Warn about potentially dangerous actions";
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
            // 
            // textSearchEngine
            // 
            this.textSearchEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchEngine.Location = new System.Drawing.Point(95, 33);
            this.textSearchEngine.Name = "textSearchEngine";
            this.textSearchEngine.Size = new System.Drawing.Size(316, 20);
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
            this.tabControl.Size = new System.Drawing.Size(425, 258);
            this.tabControl.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.checkEnableKPH);
            this.tabGeneral.Controls.Add(this.comboSizeUnits);
            this.tabGeneral.Controls.Add(this.label18);
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
            this.tabGeneral.Size = new System.Drawing.Size(417, 232);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
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
            this.comboSizeUnits.Location = new System.Drawing.Point(95, 59);
            this.comboSizeUnits.Name = "comboSizeUnits";
            this.comboSizeUnits.Size = new System.Drawing.Size(67, 21);
            this.comboSizeUnits.TabIndex = 11;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 62);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(78, 13);
            this.label18.TabIndex = 10;
            this.label18.Text = "Max. Size Unit:";
            // 
            // checkHideWhenMinimized
            // 
            this.checkHideWhenMinimized.AutoSize = true;
            this.checkHideWhenMinimized.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenMinimized.Location = new System.Drawing.Point(6, 110);
            this.checkHideWhenMinimized.Name = "checkHideWhenMinimized";
            this.checkHideWhenMinimized.Size = new System.Drawing.Size(131, 18);
            this.checkHideWhenMinimized.TabIndex = 9;
            this.checkHideWhenMinimized.Text = "Hide when minimized";
            this.checkHideWhenMinimized.UseVisualStyleBackColor = true;
            // 
            // checkShowTrayIcon
            // 
            this.checkShowTrayIcon.AutoSize = true;
            this.checkShowTrayIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowTrayIcon.Location = new System.Drawing.Point(6, 86);
            this.checkShowTrayIcon.Name = "checkShowTrayIcon";
            this.checkShowTrayIcon.Size = new System.Drawing.Size(102, 18);
            this.checkShowTrayIcon.TabIndex = 7;
            this.checkShowTrayIcon.Text = "Show tray icon";
            this.checkShowTrayIcon.UseVisualStyleBackColor = true;
            this.checkShowTrayIcon.CheckedChanged += new System.EventHandler(this.checkShowTrayIcon_CheckedChanged);
            // 
            // checkVerifySignatures
            // 
            this.checkVerifySignatures.AutoSize = true;
            this.checkVerifySignatures.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkVerifySignatures.Location = new System.Drawing.Point(6, 183);
            this.checkVerifySignatures.Name = "checkVerifySignatures";
            this.checkVerifySignatures.Size = new System.Drawing.Size(254, 18);
            this.checkVerifySignatures.TabIndex = 3;
            this.checkVerifySignatures.Text = "Verify signatures and perform additional checks";
            this.checkVerifySignatures.UseVisualStyleBackColor = true;
            // 
            // tabHighlighting
            // 
            this.tabHighlighting.Controls.Add(this.label20);
            this.tabHighlighting.Controls.Add(this.label19);
            this.tabHighlighting.Controls.Add(this.label11);
            this.tabHighlighting.Controls.Add(this.colorPackedProcesses);
            this.tabHighlighting.Controls.Add(this.colorDotNetProcesses);
            this.tabHighlighting.Controls.Add(this.colorJobProcesses);
            this.tabHighlighting.Controls.Add(this.label10);
            this.tabHighlighting.Controls.Add(this.colorElevatedProcesses);
            this.tabHighlighting.Controls.Add(this.label9);
            this.tabHighlighting.Controls.Add(this.label8);
            this.tabHighlighting.Controls.Add(this.textHighlightingDuration);
            this.tabHighlighting.Controls.Add(this.label7);
            this.tabHighlighting.Controls.Add(this.colorServiceProcesses);
            this.tabHighlighting.Controls.Add(this.colorBeingDebugged);
            this.tabHighlighting.Controls.Add(this.colorSystemProcesses);
            this.tabHighlighting.Controls.Add(this.colorOwnProcesses);
            this.tabHighlighting.Controls.Add(this.label6);
            this.tabHighlighting.Controls.Add(this.colorRemovedProcesses);
            this.tabHighlighting.Controls.Add(this.label5);
            this.tabHighlighting.Controls.Add(this.label4);
            this.tabHighlighting.Controls.Add(this.colorNewProcesses);
            this.tabHighlighting.Controls.Add(this.label3);
            this.tabHighlighting.Location = new System.Drawing.Point(4, 22);
            this.tabHighlighting.Name = "tabHighlighting";
            this.tabHighlighting.Padding = new System.Windows.Forms.Padding(3);
            this.tabHighlighting.Size = new System.Drawing.Size(417, 213);
            this.tabHighlighting.TabIndex = 1;
            this.tabHighlighting.Text = "Highlighting";
            this.tabHighlighting.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(214, 133);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(110, 29);
            this.label20.TabIndex = 9;
            this.label20.Text = "Packed/Dangerous Processes:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(214, 113);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(87, 13);
            this.label19.TabIndex = 9;
            this.label19.Text = ".NET Processes:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(214, 88);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Job Processes:";
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
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(214, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Elevated Processes:";
            // 
            // colorElevatedProcesses
            // 
            this.colorElevatedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorElevatedProcesses.Location = new System.Drawing.Point(335, 60);
            this.colorElevatedProcesses.Name = "colorElevatedProcesses";
            this.colorElevatedProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorElevatedProcesses.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 140);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Service Processes:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(214, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Debugged Processes:";
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
            this.label7.Location = new System.Drawing.Point(6, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Highlighting Duration:";
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "System Processes:";
            // 
            // colorRemovedProcesses
            // 
            this.colorRemovedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorRemovedProcesses.Location = new System.Drawing.Point(127, 59);
            this.colorRemovedProcesses.Name = "colorRemovedProcesses";
            this.colorRemovedProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorRemovedProcesses.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Own Processes:";
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
            // colorNewProcesses
            // 
            this.colorNewProcesses.Color = System.Drawing.Color.Transparent;
            this.colorNewProcesses.Location = new System.Drawing.Point(127, 33);
            this.colorNewProcesses.Name = "colorNewProcesses";
            this.colorNewProcesses.Size = new System.Drawing.Size(40, 20);
            this.colorNewProcesses.TabIndex = 1;
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
            // tabPlotting
            // 
            this.tabPlotting.Controls.Add(this.checkPlotterAntialias);
            this.tabPlotting.Controls.Add(this.label12);
            this.tabPlotting.Controls.Add(this.label13);
            this.tabPlotting.Controls.Add(this.colorIORO);
            this.tabPlotting.Controls.Add(this.colorIOW);
            this.tabPlotting.Controls.Add(this.colorMemoryWS);
            this.tabPlotting.Controls.Add(this.colorMemoryPB);
            this.tabPlotting.Controls.Add(this.label14);
            this.tabPlotting.Controls.Add(this.colorCPUUT);
            this.tabPlotting.Controls.Add(this.label15);
            this.tabPlotting.Controls.Add(this.label16);
            this.tabPlotting.Controls.Add(this.colorCPUKT);
            this.tabPlotting.Controls.Add(this.label17);
            this.tabPlotting.Location = new System.Drawing.Point(4, 22);
            this.tabPlotting.Name = "tabPlotting";
            this.tabPlotting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlotting.Size = new System.Drawing.Size(417, 213);
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
            this.checkPlotterAntialias.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 135);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "IO Reads+Other:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 160);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "IO Writes:";
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
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 109);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Working Set:";
            // 
            // colorCPUUT
            // 
            this.colorCPUUT.Color = System.Drawing.Color.Transparent;
            this.colorCPUUT.Location = new System.Drawing.Point(124, 55);
            this.colorCPUUT.Name = "colorCPUUT";
            this.colorCPUUT.Size = new System.Drawing.Size(40, 20);
            this.colorCPUUT.TabIndex = 16;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 83);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "Private Bytes:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 58);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(83, 13);
            this.label16.TabIndex = 14;
            this.label16.Text = "CPU User Time:";
            // 
            // colorCPUKT
            // 
            this.colorCPUKT.Color = System.Drawing.Color.Transparent;
            this.colorCPUKT.Location = new System.Drawing.Point(124, 29);
            this.colorCPUKT.Name = "colorCPUKT";
            this.colorCPUKT.Size = new System.Drawing.Size(40, 20);
            this.colorCPUKT.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 32);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "CPU Kernel Time:";
            // 
            // checkEnableKPH
            // 
            this.checkEnableKPH.AutoSize = true;
            this.checkEnableKPH.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkEnableKPH.Location = new System.Drawing.Point(6, 207);
            this.checkEnableKPH.Name = "checkEnableKPH";
            this.checkEnableKPH.Size = new System.Drawing.Size(217, 18);
            this.checkEnableKPH.TabIndex = 12;
            this.checkEnableKPH.Text = "Enable experimental kernel-mode driver";
            this.checkEnableKPH.UseVisualStyleBackColor = true;
            // 
            // OptionsWindow
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 311);
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
    }
}