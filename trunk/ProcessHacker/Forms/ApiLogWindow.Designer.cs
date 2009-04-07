namespace ProcessHacker.Forms
{
    partial class ApiLogWindow
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
            this.listLog = new System.Windows.Forms.ListView();
            this.columnTime = new System.Windows.Forms.ColumnHeader();
            this.columnFunction = new System.Windows.Forms.ColumnHeader();
            this.columnInformation = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listLog
            // 
            this.listLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnTime,
            this.columnFunction,
            this.columnInformation});
            this.listLog.Location = new System.Drawing.Point(12, 12);
            this.listLog.Name = "listLog";
            this.listLog.Size = new System.Drawing.Size(587, 371);
            this.listLog.TabIndex = 0;
            this.listLog.UseCompatibleStateImageBehavior = false;
            this.listLog.View = System.Windows.Forms.View.Details;
            // 
            // columnTime
            // 
            this.columnTime.Text = "Time";
            // 
            // columnFunction
            // 
            this.columnFunction.Text = "Function";
            this.columnFunction.Width = 100;
            // 
            // columnInformation
            // 
            this.columnInformation.Text = "Information";
            this.columnInformation.Width = 400;
            // 
            // ApiLogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 494);
            this.Controls.Add(this.listLog);
            this.Name = "ApiLogWindow";
            this.Text = "API Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApiLogWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listLog;
        private System.Windows.Forms.ColumnHeader columnTime;
        private System.Windows.Forms.ColumnHeader columnFunction;
        private System.Windows.Forms.ColumnHeader columnInformation;
    }
}