/*
 * Process Hacker - 
 *   ProcessHacker VirusTotal Implementation
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;

/* ProcessHacker VirusTotal Implementation Authorized by:
 * Julio Canto | VirusTotal.com | Hispasec Sistemas Lab | Tlf: +34.902.161.025
 * Fax: +34.952.028.694 | PGP Key ID: EF618D2B | jcanto@hispasec.com 
 * 26/09/2009 - 2:39PM
 */

namespace ProcessHacker
{
    public partial class VirusTotalUploaderWindow : Form
    {
        string url;
        string filepath;
        string processName;
        
        long totalfilesize;
        long bytesPerSecond;
        long bytesTransferred;

        public VirusTotalUploaderWindow(string procName, string procPath)
        {
            InitializeComponent();

            ProgressBarEx.XPProgressBarRenderer CustomRenderer = new ProgressBarEx.XPProgressBarRenderer(System.Drawing.Color.Blue);
            CustomRenderer.MarqueeStyle = ProgressBarEx.MarqueeStyle.LeftRight;
            progressUpload.Renderer = CustomRenderer;

            processName = procName;
            filepath = procPath;

            this.Icon = Program.HackerWindow.Icon;
        }

        private void VirusTotalUploaderWindow_Load(object sender, EventArgs e)
        {
            labelFile.Text = string.Format("Uploading: {0}", processName);

            FileInfo finfo = new FileInfo(filepath);
            if (!finfo.Exists)
            {
                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content = "The selected file doesn't exist or couldnt be found!";
                    td.MainInstruction = "File Location not Available!";
                    td.WindowTitle = "System Error";
                    td.MainIcon = TaskDialogIcon.CircleX;
                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show(Program.HackerWindow.Handle);
                }
                else
                {
                    MessageBox.Show(
                       this, "The selected file doesn't exist or couldnt be found!",
                       "System Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation
                       );
                }

                this.Close();
            }
            else if (finfo.Length >= 20971520 /* 20MB */)
            {
                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content = "This file is larger than 20MB, above the VirusTotal limit!";
                    td.MainInstruction = "File is too large";
                    td.WindowTitle = "VirusTotal Error";
                    td.MainIcon = TaskDialogIcon.CircleX;
                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show(Program.HackerWindow.Handle);
                }
                else
                {
                     MessageBox.Show(
                        this, "This file is larger than 20MB and is above the VirusTotal size limit!",
                        "VirusTotal Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation
                        );
                }

                this.Close();
            }
            else
            {
                totalfilesize = finfo.Length;
            }

            WebClient vtId = new WebClient();
            vtId.Headers.Add("User-Agent", "Process Hacker " + Application.ProductVersion);

            url = "http://www.virustotal.com/vt/en/recepcionf?" + vtId.DownloadString("http://www.virustotal.com/vt/en/identificador");

            UploadWorker.RunWorkerAsync();
        }

        private void VirusTotalUploaderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UploadWorker.IsBusy)
                UploadWorker.CancelAsync();
        }

        private void UploadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Uri uri = new Uri(url);

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);

            webrequest.UserAgent = "ProcessHacker " + Application.ProductVersion;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Timeout = System.Threading.Timeout.Infinite;
            webrequest.KeepAlive = true;
            webrequest.Method = WebRequestMethods.Http.Post;

            // Build up the 'post' message header
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append(@"Content-Disposition: form-data; name=""archivo""; filename=" + processName + "");
            sb.Append("\r\n");
            sb.Append("Content-Type: application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            if (UploadWorker.CancellationPending)
            {
                webrequest.Abort();
                UploadWorker.CancelAsync();
                return;
            }

            try
            {
                using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    webrequest.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;

                    using (Stream requestStream = webrequest.GetRequestStream())
                    {
                        // Write out our post header
                        requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                        // Write out the file contents
                        byte[] buffer = new Byte[checked((uint)Math.Min(32, (int)fileStream.Length))];

                        int bytesRead = 0;
                        Stopwatch stopwatch = new Stopwatch();

                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            if (UploadWorker.CancellationPending)
                            {
                                webrequest.Abort();
                                UploadWorker.CancelAsync();
                                break;
                            }

                            stopwatch.Start();
                            requestStream.Write(buffer, 0, bytesRead);
                            stopwatch.Stop();

                            int progress = (int)(((float)fileStream.Position / (float)fileStream.Length) * 100);

                            if (stopwatch.ElapsedMilliseconds > 0)
                                bytesPerSecond = fileStream.Position * 1000 / stopwatch.ElapsedMilliseconds;

                            bytesTransferred = fileStream.Position;

                            stopwatch.Reset();

                            UploadWorker.ReportProgress(progress);
                        }

                        if (UploadWorker.CancellationPending)
                        {
                            webrequest.Abort();
                            UploadWorker.CancelAsync();
                            return;
                        }

                        // Write out the trailing boundary
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                        // Write all data before we close the stream.
                        requestStream.Flush();
                        requestStream.Close();
                    }
                }
            }
            catch (WebException ex)
            {   //RequestCanceled will occour when we cancel the WebRequest
                //filter that exception but log all others
                if (ex.Status != WebExceptionStatus.RequestCanceled)
                {
                    Logging.Log(ex);
                    return;
                }
            }

            if (UploadWorker.CancellationPending)
            {
                webrequest.Abort();
                UploadWorker.CancelAsync();
                return;
            }

            WebResponse response = webrequest.GetResponse();

            //Stream s = responce.GetResponseStream(); 
            //StreamReader sr = new StreamReader(s);
            //sr.ReadToEnd();

            //Return the response URL 
            e.Result = response.ResponseUri.AbsoluteUri;
        }

        private void UploadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            uploadedLabel.Text = "Uploaded: " + Utils.FormatSize(bytesTransferred);
            totalSizeLabel.Text = "Total Size: " + Utils.FormatSize(totalfilesize);
            speedLabel.Text = "Speed: " + Utils.FormatSize(bytesPerSecond) + "/s";
            progressUpload.Value = e.ProgressPercentage;
        }

        private void UploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //TODO: future additions will parse the page and  
            //display the appropriate infomation but for now just mirror  
            //the functionality of the VirusTotal desktop client and
            //launch the URL in the default browser

            if (e.Result != null) //Result will be null if theres an error
            {
                if (!e.Cancelled) //sanity check
                {
                    Program.TryStart(e.Result.ToString());
                }
            }

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}