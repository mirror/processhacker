
    partial class VirusTotalUploaderWindow
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
            this.LogoImg = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.fileLabel = new System.Windows.Forms.Label();
            this.speedLabel = new System.Windows.Forms.Label();
            this.UploadWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.LogoImg)).BeginInit();
            this.SuspendLayout();
            // 
            // LogoImg
            // 
            this.LogoImg.Image = global::ProcessHacker.Properties.Resources.VirusTotal_logo;
            this.LogoImg.Location = new System.Drawing.Point(379, 12);
            this.LogoImg.Name = "LogoImg";
            this.LogoImg.Size = new System.Drawing.Size(103, 36);
            this.LogoImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LogoImg.TabIndex = 0;
            this.LogoImg.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 54);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(470, 17);
            this.progressBar1.TabIndex = 1;
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(12, 12);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(23, 13);
            this.fileLabel.TabIndex = 2;
            this.fileLabel.Text = "File";
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.Location = new System.Drawing.Point(12, 35);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(38, 13);
            this.speedLabel.TabIndex = 3;
            this.speedLabel.Text = "Speed";
            // 
            // UploadWorker
            // 
            this.UploadWorker.WorkerReportsProgress = true;
            this.UploadWorker.WorkerSupportsCancellation = true;
            this.UploadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.UploadWorker_DoWork);
            this.UploadWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UploadWorker_RunWorkerCompleted);
            this.UploadWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.UploadWorker_ProgressChanged);
            // 
            // VirusTotalUploaderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 79);
            this.Controls.Add(this.speedLabel);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.LogoImg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VirusTotalUploaderWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VirusTotal Uploader";
            this.Load += new System.EventHandler(this.VirusTotalUploaderWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LogoImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox LogoImg;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.Label speedLabel;
        private System.ComponentModel.BackgroundWorker UploadWorker;
    }
