namespace ProcessHacker
{
    partial class NetworkList
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

            _highlightingContext.Dispose();
            this.Provider = null;

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
            this.listNetwork = new System.Windows.Forms.ListView();
            this.columnProcess = new System.Windows.Forms.ColumnHeader();
            this.columnLocal = new System.Windows.Forms.ColumnHeader();
            this.columnRemote = new System.Windows.Forms.ColumnHeader();
            this.columnProtocol = new System.Windows.Forms.ColumnHeader();
            this.columnState = new System.Windows.Forms.ColumnHeader();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listNetwork
            // 
            this.listNetwork.AllowColumnReorder = true;
            this.listNetwork.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProcess,
            this.columnLocal,
            this.columnRemote,
            this.columnProtocol,
            this.columnState});
            this.listNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listNetwork.FullRowSelect = true;
            this.listNetwork.HideSelection = false;
            this.listNetwork.Location = new System.Drawing.Point(0, 0);
            this.listNetwork.Name = "listNetwork";
            this.listNetwork.ShowItemToolTips = true;
            this.listNetwork.Size = new System.Drawing.Size(685, 472);
            this.listNetwork.SmallImageList = this.imageList;
            this.listNetwork.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listNetwork.TabIndex = 1;
            this.listNetwork.UseCompatibleStateImageBehavior = false;
            this.listNetwork.View = System.Windows.Forms.View.Details;
            // 
            // columnProcess
            // 
            this.columnProcess.Text = "Process";
            this.columnProcess.Width = 137;
            // 
            // columnLocal
            // 
            this.columnLocal.Text = "Local";
            this.columnLocal.Width = 250;
            // 
            // columnRemote
            // 
            this.columnRemote.Text = "Remote";
            this.columnRemote.Width = 280;
            // 
            // columnProtocol
            // 
            this.columnProtocol.Text = "Protocol";
            this.columnProtocol.Width = 52;
            // 
            // columnState
            // 
            this.columnState.Text = "State";
            this.columnState.Width = 95;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // NetworkList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listNetwork);
            this.DoubleBuffered = true;
            this.Name = "NetworkList";
            this.Size = new System.Drawing.Size(685, 472);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listNetwork;
        private System.Windows.Forms.ColumnHeader columnLocal;
        private System.Windows.Forms.ColumnHeader columnRemote;
        private System.Windows.Forms.ColumnHeader columnProtocol;
        private System.Windows.Forms.ColumnHeader columnState;
        private System.Windows.Forms.ColumnHeader columnProcess;
        private System.Windows.Forms.ImageList imageList;
    }
}
