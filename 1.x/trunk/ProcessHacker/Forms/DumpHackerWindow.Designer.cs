namespace ProcessHacker
{
    partial class DumpHackerWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DumpHackerWindow));
            this.treeProcesses = new ProcessHacker.ProcessTree();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProcesses = new System.Windows.Forms.TabPage();
            this.tabServices = new System.Windows.Forms.TabPage();
            this.listServices = new ProcessHacker.Components.ServiceList();
            this.tabControl.SuspendLayout();
            this.tabProcesses.SuspendLayout();
            this.tabServices.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeProcesses
            // 
            this.treeProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProcesses.Draw = true;
            this.treeProcesses.Location = new System.Drawing.Point(3, 3);
            this.treeProcesses.Name = "treeProcesses";
            this.treeProcesses.Provider = null;
            this.treeProcesses.Size = new System.Drawing.Size(889, 493);
            this.treeProcesses.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProcesses);
            this.tabControl.Controls.Add(this.tabServices);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(903, 525);
            this.tabControl.TabIndex = 1;
            // 
            // tabProcesses
            // 
            this.tabProcesses.Controls.Add(this.treeProcesses);
            this.tabProcesses.Location = new System.Drawing.Point(4, 22);
            this.tabProcesses.Name = "tabProcesses";
            this.tabProcesses.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcesses.Size = new System.Drawing.Size(895, 499);
            this.tabProcesses.TabIndex = 0;
            this.tabProcesses.Text = "Processes";
            this.tabProcesses.UseVisualStyleBackColor = true;
            // 
            // tabServices
            // 
            this.tabServices.Controls.Add(this.listServices);
            this.tabServices.Location = new System.Drawing.Point(4, 22);
            this.tabServices.Name = "tabServices";
            this.tabServices.Padding = new System.Windows.Forms.Padding(3);
            this.tabServices.Size = new System.Drawing.Size(895, 499);
            this.tabServices.TabIndex = 1;
            this.tabServices.Text = "Services";
            this.tabServices.UseVisualStyleBackColor = true;
            // 
            // listServices
            // 
            this.listServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listServices.DoubleBuffered = true;
            this.listServices.Location = new System.Drawing.Point(3, 3);
            this.listServices.Name = "listServices";
            this.listServices.Provider = null;
            this.listServices.Size = new System.Drawing.Size(889, 493);
            this.listServices.TabIndex = 0;
            // 
            // DumpHackerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 525);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DumpHackerWindow";
            this.Text = "Dump Viewer";
            this.Load += new System.EventHandler(this.DumpHackerWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DumpHackerWindow_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabProcesses.ResumeLayout(false);
            this.tabServices.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ProcessTree treeProcesses;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProcesses;
        private System.Windows.Forms.TabPage tabServices;
        private ProcessHacker.Components.ServiceList listServices;
    }
}