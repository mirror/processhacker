/*
 * Process Hacker - 
 *   ProcessHacker Updater Download
 * 
 * Copyright (C) 2009 wj32
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
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ProcessHacker;
using ProcessHacker.Common;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
  
public partial class UpdaterDownloadWindow : Form  
{
    private string appUpdateName;
    private string appUpdateURL;
    private string appUpdateVersion;
    private string appUpdateReleased;
    private string appUpdateMD5;
    private string appUpdateFilenamePath;
    private WebClient webClient;

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
            appUpdateFilenamePath = Path.GetTempPath() + "processhacker-" + appUpdateVersion + "-setup.exe";

            labelTitle.Text = "Downloading: Process Hacker " + appUpdateVersion;
            labelReleased.Text = "Released: " + appUpdateReleased;

            webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);

            try
            {
                webClient.DownloadFileAsync(new Uri(appUpdateURL), appUpdateFilenamePath);
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to download Process Hacker", ex);
                this.Close();
            }
        }
    }

    private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        verifyWorker.RunWorkerAsync(appUpdateFilenamePath);
    }

    private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        labelProgress.Text = 
            "Downloaded " + 
            Utils.FormatSize(e.BytesReceived) + "/" + 
            Utils.FormatSize(e.TotalBytesToReceive) + 
            " (" + e.ProgressPercentage.ToString() + "%)"; 

        progressDownload.Value = e.ProgressPercentage;
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
        this.progressDownload.Value = e.ProgressPercentage;
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
            labelProgress.Text = "Download completed and SHA1 verified successfully.";
        }
        else
        {
            labelProgress.Text = "SHA1 hash verification failed!";
            labelProgress.Font = new System.Drawing.Font(labelProgress.Font, System.Drawing.FontStyle.Bold);
        }

        buttonInstall.Enabled = true;
        buttonInstall.Select();
        buttonStop.Text = "Close";
    }

    private void buttonInstall_Click(object sender, EventArgs e)
    {
        // We need to close our handle to the PH mutex in order to 
        // let the installer continue.
        if (Program.GlobalMutex != null)
        {
            Program.GlobalMutex.Dispose();
            Program.GlobalMutex = null;
        }

        bool success = false;

        // Force elevation if required to prevent an exception if the user 
        // clicks no. Otherwise, start it normally.
        if (OSVersion.HasUac && Program.ElevationType == TokenElevationType.Limited)
        {
            Program.StartProgramAdmin(
                                appUpdateFilenamePath,
                                "",
                                new MethodInvoker(() => success = true),
                                ShowWindowType.Normal,
                                this.Handle
                                );
        }
        else
        {
            Program.TryStart(appUpdateFilenamePath);
        }

        if (success)
        {
            Program.HackerWindow.Exit();
        }
        else
        {
            // User canceled. Re-open the mutex.
            try
            {
                Program.GlobalMutex = new ProcessHacker.Native.Threading.Mutant(Program.GlobalMutexName);
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }
    }

    private void buttonStop_Click(object sender, EventArgs e)
    {
        if (webClient.IsBusy)
            webClient.CancelAsync();

        this.Close();
    }
}

