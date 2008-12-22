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
            this.checkHideWhenMinimized = new System.Windows.Forms.CheckBox();
            this.checkHideHandlesNoName = new System.Windows.Forms.CheckBox();
            this.checkShowTrayIcon = new System.Windows.Forms.CheckBox();
            this.checkUseToolhelpModules = new System.Windows.Forms.CheckBox();
            this.tabHighlighting = new System.Windows.Forms.TabPage();
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
            this.label10 = new System.Windows.Forms.Label();
            this.colorElevatedProcesses = new ProcessHacker.Components.ColorModifier();
            ((System.ComponentModel.ISupportInitialize)(this.textUpdateInterval)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabHighlighting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textHighlightingDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
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
            this.textUpdateInterval.Location = new System.Drawing.Point(95, 6);
            this.textUpdateInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textUpdateInterval.Name = "textUpdateInterval";
            this.textUpdateInterval.Size = new System.Drawing.Size(66, 21);
            this.textUpdateInterval.TabIndex = 1;
            this.textUpdateInterval.Leave += new System.EventHandler(this.textUpdateInterval_Leave);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonClose.Location = new System.Drawing.Point(284, 253);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 21);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // checkShowProcessDomains
            // 
            this.checkShowProcessDomains.AutoSize = true;
            this.checkShowProcessDomains.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowProcessDomains.Location = new System.Drawing.Point(6, 144);
            this.checkShowProcessDomains.Name = "checkShowProcessDomains";
            this.checkShowProcessDomains.Size = new System.Drawing.Size(168, 17);
            this.checkShowProcessDomains.TabIndex = 3;
            this.checkShowProcessDomains.Text = "Show user/group domains";
            this.checkShowProcessDomains.UseVisualStyleBackColor = true;
            // 
            // checkWarnDangerous
            // 
            this.checkWarnDangerous.AutoSize = true;
            this.checkWarnDangerous.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkWarnDangerous.Location = new System.Drawing.Point(6, 100);
            this.checkWarnDangerous.Name = "checkWarnDangerous";
            this.checkWarnDangerous.Size = new System.Drawing.Size(270, 17);
            this.checkWarnDangerous.TabIndex = 4;
            this.checkWarnDangerous.Text = "Warn about potentially dangerous actions";
            this.checkWarnDangerous.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Search Engine:";
            // 
            // textSearchEngine
            // 
            this.textSearchEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchEngine.Location = new System.Drawing.Point(92, 30);
            this.textSearchEngine.Name = "textSearchEngine";
            this.textSearchEngine.Size = new System.Drawing.Size(241, 21);
            this.textSearchEngine.TabIndex = 6;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabHighlighting);
            this.tabControl.Location = new System.Drawing.Point(12, 11);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(347, 236);
            this.tabControl.TabIndex = 7;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.checkHideWhenMinimized);
            this.tabGeneral.Controls.Add(this.checkHideHandlesNoName);
            this.tabGeneral.Controls.Add(this.checkShowTrayIcon);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.textSearchEngine);
            this.tabGeneral.Controls.Add(this.textUpdateInterval);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.checkUseToolhelpModules);
            this.tabGeneral.Controls.Add(this.checkShowProcessDomains);
            this.tabGeneral.Controls.Add(this.checkWarnDangerous);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(339, 197);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // checkHideWhenMinimized
            // 
            this.checkHideWhenMinimized.AutoSize = true;
            this.checkHideWhenMinimized.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideWhenMinimized.Location = new System.Drawing.Point(6, 77);
            this.checkHideWhenMinimized.Name = "checkHideWhenMinimized";
            this.checkHideWhenMinimized.Size = new System.Drawing.Size(144, 17);
            this.checkHideWhenMinimized.TabIndex = 9;
            this.checkHideWhenMinimized.Text = "Hide when minimized";
            this.checkHideWhenMinimized.UseVisualStyleBackColor = true;
            // 
            // checkHideHandlesNoName
            // 
            this.checkHideHandlesNoName.AutoSize = true;
            this.checkHideHandlesNoName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkHideHandlesNoName.Location = new System.Drawing.Point(6, 166);
            this.checkHideHandlesNoName.Name = "checkHideHandlesNoName";
            this.checkHideHandlesNoName.Size = new System.Drawing.Size(180, 17);
            this.checkHideHandlesNoName.TabIndex = 8;
            this.checkHideHandlesNoName.Text = "Hide handles with no name";
            this.checkHideHandlesNoName.UseVisualStyleBackColor = true;
            // 
            // checkShowTrayIcon
            // 
            this.checkShowTrayIcon.AutoSize = true;
            this.checkShowTrayIcon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkShowTrayIcon.Location = new System.Drawing.Point(6, 54);
            this.checkShowTrayIcon.Name = "checkShowTrayIcon";
            this.checkShowTrayIcon.Size = new System.Drawing.Size(114, 17);
            this.checkShowTrayIcon.TabIndex = 7;
            this.checkShowTrayIcon.Text = "Show tray icon";
            this.checkShowTrayIcon.UseVisualStyleBackColor = true;
            // 
            // checkUseToolhelpModules
            // 
            this.checkUseToolhelpModules.AutoSize = true;
            this.checkUseToolhelpModules.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkUseToolhelpModules.Location = new System.Drawing.Point(6, 122);
            this.checkUseToolhelpModules.Name = "checkUseToolhelpModules";
            this.checkUseToolhelpModules.Size = new System.Drawing.Size(234, 17);
            this.checkUseToolhelpModules.TabIndex = 3;
            this.checkUseToolhelpModules.Text = "Use Toolhelp-based module listings";
            this.checkUseToolhelpModules.UseVisualStyleBackColor = true;
            // 
            // tabHighlighting
            // 
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
            this.tabHighlighting.Size = new System.Drawing.Size(339, 210);
            this.tabHighlighting.TabIndex = 1;
            this.tabHighlighting.Text = "Highlighting";
            this.tabHighlighting.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 12);
            this.label9.TabIndex = 7;
            this.label9.Text = "Service Processes:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 12);
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
            this.textHighlightingDuration.Location = new System.Drawing.Point(127, 6);
            this.textHighlightingDuration.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textHighlightingDuration.Name = "textHighlightingDuration";
            this.textHighlightingDuration.Size = new System.Drawing.Size(66, 21);
            this.textHighlightingDuration.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(137, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "Highlighting Duration:";
            // 
            // colorServiceProcesses
            // 
            this.colorServiceProcesses.Color = System.Drawing.Color.Transparent;
            this.colorServiceProcesses.Location = new System.Drawing.Point(127, 126);
            this.colorServiceProcesses.Name = "colorServiceProcesses";
            this.colorServiceProcesses.Size = new System.Drawing.Size(40, 18);
            this.colorServiceProcesses.TabIndex = 3;
            // 
            // colorBeingDebugged
            // 
            this.colorBeingDebugged.Color = System.Drawing.Color.Transparent;
            this.colorBeingDebugged.Location = new System.Drawing.Point(127, 150);
            this.colorBeingDebugged.Name = "colorBeingDebugged";
            this.colorBeingDebugged.Size = new System.Drawing.Size(40, 18);
            this.colorBeingDebugged.TabIndex = 3;
            // 
            // colorSystemProcesses
            // 
            this.colorSystemProcesses.Color = System.Drawing.Color.Transparent;
            this.colorSystemProcesses.Location = new System.Drawing.Point(127, 102);
            this.colorSystemProcesses.Name = "colorSystemProcesses";
            this.colorSystemProcesses.Size = new System.Drawing.Size(40, 18);
            this.colorSystemProcesses.TabIndex = 3;
            // 
            // colorOwnProcesses
            // 
            this.colorOwnProcesses.Color = System.Drawing.Color.Transparent;
            this.colorOwnProcesses.Location = new System.Drawing.Point(127, 78);
            this.colorOwnProcesses.Name = "colorOwnProcesses";
            this.colorOwnProcesses.Size = new System.Drawing.Size(40, 18);
            this.colorOwnProcesses.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "System Processes:";
            // 
            // colorRemovedProcesses
            // 
            this.colorRemovedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorRemovedProcesses.Location = new System.Drawing.Point(127, 54);
            this.colorRemovedProcesses.Name = "colorRemovedProcesses";
            this.colorRemovedProcesses.Size = new System.Drawing.Size(40, 18);
            this.colorRemovedProcesses.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Own Processes:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Removed Objects:";
            // 
            // colorNewProcesses
            // 
            this.colorNewProcesses.Color = System.Drawing.Color.Transparent;
            this.colorNewProcesses.Location = new System.Drawing.Point(127, 30);
            this.colorNewProcesses.Name = "colorNewProcesses";
            this.colorNewProcesses.Size = new System.Drawing.Size(40, 18);
            this.colorNewProcesses.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "New Objects:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 176);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "Elevated Processes:";
            // 
            // colorElevatedProcesses
            // 
            this.colorElevatedProcesses.Color = System.Drawing.Color.Transparent;
            this.colorElevatedProcesses.Location = new System.Drawing.Point(127, 174);
            this.colorElevatedProcesses.Name = "colorElevatedProcesses";
            this.colorElevatedProcesses.Size = new System.Drawing.Size(40, 18);
            this.colorElevatedProcesses.TabIndex = 8;
            // 
            // OptionsWindow
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 285);
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
        private System.Windows.Forms.CheckBox checkUseToolhelpModules;
        private System.Windows.Forms.CheckBox checkShowTrayIcon;
        private System.Windows.Forms.CheckBox checkHideHandlesNoName;
        private System.Windows.Forms.Label label8;
        private ProcessHacker.Components.ColorModifier colorBeingDebugged;
        private System.Windows.Forms.Label label9;
        private ProcessHacker.Components.ColorModifier colorServiceProcesses;
        private System.Windows.Forms.CheckBox checkHideWhenMinimized;
        private System.Windows.Forms.Label label10;
        private ProcessHacker.Components.ColorModifier colorElevatedProcesses;
    }
}