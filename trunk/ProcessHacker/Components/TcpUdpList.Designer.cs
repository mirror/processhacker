namespace ProcessHacker
{
    partial class TcpUdpList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listTcpUdp = new System.Windows.Forms.ListView();
            this.columnProtocol = new System.Windows.Forms.ColumnHeader();
            this.columnLocalAddress = new System.Windows.Forms.ColumnHeader();
            this.columnRemoteAddress = new System.Windows.Forms.ColumnHeader();
            this.columnState = new System.Windows.Forms.ColumnHeader();
            this.vistaMenu = new wyDay.Controls.VistaMenu(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // listTcpUdp
            // 
            this.listTcpUdp.AllowColumnReorder = true;
            this.listTcpUdp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProtocol,
            this.columnLocalAddress,
            this.columnRemoteAddress,
            this.columnState});
            this.listTcpUdp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listTcpUdp.FullRowSelect = true;
            this.listTcpUdp.HideSelection = false;
            this.listTcpUdp.Location = new System.Drawing.Point(0, 0);
            this.listTcpUdp.Name = "listTcpUdp";
            this.listTcpUdp.ShowItemToolTips = true;
            this.listTcpUdp.Size = new System.Drawing.Size(450, 436);
            this.listTcpUdp.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listTcpUdp.TabIndex = 3;
            this.listTcpUdp.UseCompatibleStateImageBehavior = false;
            this.listTcpUdp.View = System.Windows.Forms.View.Details;
            this.listTcpUdp.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // columnProtocol
            // 
            this.columnProtocol.Text = "Protocol";
            this.columnProtocol.Width = 80;
            // 
            // columnLocalAddress
            // 
            this.columnLocalAddress.Text = "Local Address";
            this.columnLocalAddress.Width = 120;
            // 
            // columnRemoteAddress
            // 
            this.columnRemoteAddress.Text = "Remote Address";
            this.columnRemoteAddress.Width = 120;
            // 
            // columnState
            // 
            this.columnState.Text = "State";
            this.columnState.Width = 100;
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // TcpUdpList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listTcpUdp);
            this.DoubleBuffered = true;
            this.Name = "TcpUdpList";
            this.Size = new System.Drawing.Size(450, 436);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listTcpUdp;
        private System.Windows.Forms.ColumnHeader columnProtocol;
        private System.Windows.Forms.ColumnHeader columnLocalAddress;
        private System.Windows.Forms.ColumnHeader columnRemoteAddress;
        private wyDay.Controls.VistaMenu vistaMenu;
        private System.Windows.Forms.ColumnHeader columnState;
    }
}
