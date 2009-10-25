namespace ProcessHacker.Components
{
    partial class TimerProperties
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

            _timerHandle.Dereference(disposing);

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.labelTimeRemaining = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelSignaled = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Signaled:";
            // 
            // labelTimeRemaining
            // 
            this.labelTimeRemaining.AutoSize = true;
            this.labelTimeRemaining.Location = new System.Drawing.Point(98, 25);
            this.labelTimeRemaining.Name = "labelTimeRemaining";
            this.labelTimeRemaining.Size = new System.Drawing.Size(13, 13);
            this.labelTimeRemaining.TabIndex = 1;
            this.labelTimeRemaining.Text = "0";
            this.labelTimeRemaining.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Time Remaining:";
            this.label3.Visible = false;
            // 
            // labelSignaled
            // 
            this.labelSignaled.AutoSize = true;
            this.labelSignaled.Location = new System.Drawing.Point(98, 3);
            this.labelSignaled.Name = "labelSignaled";
            this.labelSignaled.Size = new System.Drawing.Size(32, 13);
            this.labelSignaled.TabIndex = 1;
            this.labelSignaled.Text = "False";
            // 
            // buttonCancel
            // 
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCancel.Location = new System.Drawing.Point(6, 52);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // TimerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelSignaled);
            this.Controls.Add(this.labelTimeRemaining);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "TimerProperties";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(215, 81);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTimeRemaining;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelSignaled;
        private System.Windows.Forms.Button buttonCancel;
    }
}
