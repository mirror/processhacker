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
    private Updater.UpdateItem _updateItem;
    private string _fileName;
    private WebClient _webClient;
    private bool _redirected = false;

    public UpdaterDownloadWindow(Updater.UpdateItem updateItem)
    {   
        InitializeComponent();

        _updateItem = updateItem;
    }

    private void UpdaterDownload_Load(object sender, EventArgs e)
    {
        string version;

        version = _updateItem.Version.Major + "." + _updateItem.Version.Minor;
        _fileName = Path.GetTempPath() + "processhacker-" + version + "-setup.exe";

        labelTitle.Text = "Downloading: Process Hacker " + version;
        labelReleased.Text = "Released: " + _updateItem.Date.ToString();

        _webClient = new WebClient();
        _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
        _webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
        _webClient.Headers.Add("User-Agent", "PH/" + version + " (compatible; PH " + 
            version + "; PH " + version + "; .NET CLR " + Environment.Version.ToString() + ";)");

        try
        {
            _webClient.DownloadFileAsync(new Uri(_updateItem.Url), _fileName);
        }
        catch (Exception ex)
        {
            PhUtils.ShowException("Unable to download Process Hacker", ex);
            this.Close();
        }
    }

    private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        // Check if the file is actually an executable file.
        if (!_redirected)
        {
            try
            {
                bool isHtml = false;

                using (var file = new BinaryReader(File.OpenRead(_fileName)))
                {
                    if (!file.ReadChars(2).Equals("MZ".ToCharArray()))
                    {
                        isHtml = true;
                    }
                }

                _redirected = true;

                if (isHtml)
                {
                    string text = File.ReadAllText(_fileName);

                    // Assume this is from Ohloh.
                    int iframeIndex = text.IndexOf("window.delayed_iframe");

                    if (iframeIndex == -1)
                        return;

                    int httpIndex = text.IndexOf("http://", iframeIndex);

                    if (httpIndex == -1)
                        return;

                    int quoteIndex = text.IndexOf("'", httpIndex);

                    if (quoteIndex == -1)
                        return;

                    _webClient.DownloadFileAsync(new Uri(text.Substring(httpIndex, quoteIndex - httpIndex)), _fileName);

                    return;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }
        }

        if (!e.Cancelled)
        {
            verifyWorker.RunWorkerAsync(_fileName);
        }
        else
        {
            var webException = e.Error as WebException;

            if (webException != null && webException.Status != WebExceptionStatus.RequestCanceled)
            {
                PhUtils.ShowException("Unable to download the update", webException);
            }
        }
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
            sb.AppendFormat("{0:x2}", b);
        }

        if (_updateItem.Hash.Equals(sb.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            labelProgress.Text = "Download completed and SHA1 verified successfully.";
            buttonInstall.Enabled = true;
            buttonInstall.Select();
        }
        else
        {
            labelProgress.Text = "SHA1 hash verification failed!";
            labelProgress.Font = new System.Drawing.Font(labelProgress.Font, System.Drawing.FontStyle.Bold);
        }

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
                                _fileName,
                                "",
                                new MethodInvoker(() => success = true),
                                ShowWindowType.Normal,
                                this.Handle
                                );
        }
        else
        {
            try
            {
                System.Diagnostics.Process.Start(_fileName);
                success = true;
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to start the installer", ex);
            }
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
        if (_webClient.IsBusy)
            _webClient.CancelAsync();

        this.Close();
    }
}

