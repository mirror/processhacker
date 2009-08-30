/*
 * Process Hacker - 
 *   ProcessHacker Updater Download
 * 
 * Copyright (C) 2009 dmex
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using ProcessHacker.Common;
  
public partial class UpdaterDownloadWindow : Form  
{
    private string appUpdateName;
    private string appUpdateURL;
    private string appUpdateVersion;
    private string appUpdateReleased;
    private string appUpdateMD5;
    private string appUpdateFilenamePath;
    
    public UpdaterDownloadWindow(string appName, string appURL, string appVer, string appReleased, string appHash)
    {   
        InitializeComponent();

        appUpdateName = appName;
        appUpdateURL = appURL;
        appUpdateVersion = appVer;
        appUpdateReleased = appReleased; 
        appUpdateMD5 = appHash;
    }

    private void UpdaterDownload_Load(object sender, EventArgs e)
    {
        if (appUpdateName != null && appUpdateURL != null && appUpdateVersion != null && appUpdateReleased != null && appUpdateMD5 != null)
        {
            appUpdateVersion = new Version(appUpdateVersion).Major + "." + new Version(appUpdateVersion).MajorRevision;
            appUpdateFilenamePath = Path.GetTempPath() + "ProcessHacker " + appUpdateVersion + " Setup.exe";

            lblTitle.Text = "Downloading: Process Hacker " + appUpdateVersion;
            lblReleased.Text = "Released: " + appUpdateReleased;

            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);

            try
            {
                wc.DownloadFileAsync(new Uri(appUpdateURL), appUpdateFilenamePath);
            }
            catch (WebException ex)
            {
                MessageBox.Show(null, ex.Message, "Update Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Update Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }
        else
        { 
       
        }
    }

    private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        verifyWorker.RunWorkerAsync(appUpdateFilenamePath);
    }

    private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        lblProgress.Text = 
            "Downloaded " + 
            Utils.FormatSize(e.BytesReceived) + "/" + 
            Utils.FormatSize(e.TotalBytesToReceive) + 
            " (" + e.ProgressPercentage.ToString() + "%)"; 

        proDownload.Value = e.ProgressPercentage;
    }

    private void verifyWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        byte[] buffer;
        byte[] oldBuffer;
        int bytesRead;
        int oldBytesRead;
        long size;
        long totalBytesRead = 0;

        using (Stream stream = File.OpenRead((string)e.Argument)) //Change to MD5.Create for MD5 verification
        using (System.Security.Cryptography.HashAlgorithm hashAlgorithm = System.Security.Cryptography.SHA1.Create())
        {
            size = stream.Length;

            buffer = new byte[4096];

            bytesRead = stream.Read(buffer, 0, buffer.Length);
            totalBytesRead += bytesRead;

            do
            {
                oldBytesRead = bytesRead;
                oldBuffer = buffer;

                buffer = new byte[4096];
                bytesRead = stream.Read(buffer, 0, buffer.Length);

                totalBytesRead += bytesRead;

                if (bytesRead == 0)
                {
                    hashAlgorithm.TransformFinalBlock(oldBuffer, 0, oldBytesRead);
                }
                else
                {
                    hashAlgorithm.TransformBlock(oldBuffer, 0, oldBytesRead, oldBuffer, 0);
                }

                this.verifyWorker.ReportProgress((int)((double)totalBytesRead * 100 / size));
            } while (bytesRead != 0);

            e.Result = hashAlgorithm.Hash;
        }
    }

    private void verifyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        this.proDownload.Value = e.ProgressPercentage;
    }

    private void verifyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in (byte[])e.Result)
        {
            sb.AppendFormat("{0:X2}", b);
        }

        if (appUpdateMD5 == sb.ToString())
        {
            lblProgress.Text = "Download Completed and SHA1 Verified Successfully";
            installBtn.Text = "Install";
        }
        else
        {
            lblProgress.Text = "SHA1 Hash Verification Failed";
            installBtn.Text = "Close";
        }
    }

    private void installBtn_Click(object sender, EventArgs e)
    {
        if (installBtn.Text.Length == 7 /*Install*/) 
        {
            System.Diagnostics.Process.Start(appUpdateFilenamePath);
            new ProcessHacker.HackerWindow().Exit();        
        }
        else
        {  
            this.Close();
        }

    }
}

