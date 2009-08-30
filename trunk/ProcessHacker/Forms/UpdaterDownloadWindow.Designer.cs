    partial class UpdaterDownloadWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdaterDownloadWindow));
            this.lblProgress = new System.Windows.Forms.Label();
            this.proDownload = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblReleased = new System.Windows.Forms.Label();
            this.installBtn = new System.Windows.Forms.Button();
            this.verifyWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(7, 65);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(97, 13);
            this.lblProgress.TabIndex = 17;
            this.lblProgress.Text = "Initiating Download";
            // 
            // proDownload
            // 
            this.proDownload.Location = new System.Drawing.Point(10, 84);
            this.proDownload.Name = "proDownload";
            this.proDownload.Size = new System.Drawing.Size(372, 23);
            this.proDownload.TabIndex = 13;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ProcessHacker.Properties.Resources.ProcessHacker;
            this.pictureBox1.Location = new System.Drawing.Point(10, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(57, 52);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ProcessHacker.Properties.Resources.sflogo;
            this.pictureBox2.Location = new System.Drawing.Point(291, 9);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(86, 18);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 21;
            this.pictureBox2.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(73, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(169, 13);
            this.lblTitle.TabIndex = 22;
            this.lblTitle.Text = "Downloading: Process Hacker 0.0";
            // 
            // lblReleased
            // 
            this.lblReleased.AutoSize = true;
            this.lblReleased.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReleased.Location = new System.Drawing.Point(73, 32);
            this.lblReleased.Name = "lblReleased";
            this.lblReleased.Size = new System.Drawing.Size(180, 13);
            this.lblReleased.TabIndex = 23;
            this.lblReleased.Text = "Released: 00/00/0000 00:00:00 AM";
            // 
            // installBtn
            // 
            this.installBtn.Location = new System.Drawing.Point(302, 55);
            this.installBtn.Name = "installBtn";
            this.installBtn.Size = new System.Drawing.Size(75, 23);
            this.installBtn.TabIndex = 24;
            this.installBtn.Text = "Stop";
            this.installBtn.UseVisualStyleBackColor = true;
            this.installBtn.Click += new System.EventHandler(this.installBtn_Click);
            // 
            // verifyWorker
            // 
            this.verifyWorker.WorkerReportsProgress = true;
            this.verifyWorker.WorkerSupportsCancellation = true;
            this.verifyWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.verifyWorker_DoWork);
            this.verifyWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.verifyWorker_RunWorkerCompleted);
            this.verifyWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.verifyWorker_ProgressChanged);
            // 
            // UpdaterDownloadWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(391, 114);
            this.Controls.Add(this.installBtn);
            this.Controls.Add(this.lblReleased);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.proDownload);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "UpdaterDownloadWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Process Hacker Update";
            this.Load += new System.EventHandler(this.UpdaterDownload_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar proDownload;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblReleased;
        private System.Windows.Forms.Button installBtn;
        private System.ComponentModel.BackgroundWorker verifyWorker;
    }
