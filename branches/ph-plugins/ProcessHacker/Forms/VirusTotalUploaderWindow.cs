/*
 * Process Hacker - 
 *   ProcessHacker VirusTotal Implementation
 * 
 * Copyright (C) 2009 dmex
 * 
 * ProcessHacker permission to implement VirusTotal service authorized by:
 * Julio Canto | VirusTotal.com | Hispasec Sistemas Lab | Tlf: +34.902.161.025
 * Fax: +34.952.028.694 | PGP Key ID: EF618D2B | jcanto@hispasec.com 
 * 26/09/2009 - 2:39PM
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
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Common.Threading;
using ProcessHacker.Components;
using ProcessHacker.Native;
using TaskbarLib;

namespace ProcessHacker
{
    public partial class VirusTotalUploaderWindow : Form
    {
        string fileName;
        string processName;

        long totalFileSize;
        long bytesPerSecond;
        long bytesTransferred;
        Stopwatch uploadStopwatch;

        ThreadTask uploadTask;

        public VirusTotalUploaderWindow(string procName, string procPath)
        {
            this.SetPhParent();
            InitializeComponent();
            this.AddEscapeToClose();
            this.SetTopMost();

            processName = procName;
            fileName = procPath;
            
            this.Icon = Program.HackerWindow.Icon;
        }

        private void VirusTotalUploaderWindow_Load(object sender, EventArgs e)
        {
            labelFile.Text = string.Format("Uploading: {0}", processName);

            FileInfo finfo = new FileInfo(fileName);
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
                totalFileSize = finfo.Length;
            }

            uploadedLabel.Text = "Uploaded: Initializing";
            speedLabel.Text = "Speed: Initializing";

            ThreadTask getSessionTokenTask = new ThreadTask();

            getSessionTokenTask.RunTask += new ThreadTaskRunTaskDelegate(getSessionTokenTask_RunTask);
            getSessionTokenTask.Completed += new ThreadTaskCompletedDelegate(getSessionTokenTask_Completed);
            getSessionTokenTask.Start();
        }

        private void VirusTotalUploaderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (uploadTask != null)
                uploadTask.Cancel();

            if (OSVersion.HasExtendedTaskbar)
            {
                Windows7Taskbar.SetTaskbarProgressState(
                    Program.HackerWindowHandle,
                    Windows7Taskbar.ThumbnailProgressState.NoProgress
                    );
            }
        }

        private void getSessionTokenTask_RunTask(object param, ref object result)
        {
            try
            {
                HttpWebRequest sessionRequest = (HttpWebRequest)HttpWebRequest.Create("http://www.virustotal.com/vt/en/identificador");
                sessionRequest.ServicePoint.ConnectionLimit = 20;
                sessionRequest.UserAgent = "Process Hacker " + Application.ProductVersion;
                sessionRequest.Timeout = System.Threading.Timeout.Infinite;
                sessionRequest.KeepAlive = true;
                
                using (WebResponse Response = sessionRequest.GetResponse())
                using (Stream WebStream = Response.GetResponseStream())
                using (StreamReader Reader = new StreamReader(WebStream))
                {
                    result = Reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                PhUtils.ShowException("Unable to contact VirusTotal", ex);

                if (this.IsHandleCreated)
                    this.BeginInvoke(new MethodInvoker(this.Close));
            }
        }

        private void getSessionTokenTask_Completed(object result)
        {
            if (result != null) //incase theres an expcetion getting sessiontoken
            {
                uploadTask = new ThreadTask();
                uploadTask.RunTask += uploadTask_RunTask;
                uploadTask.Completed += uploadTask_Completed;
                uploadTask.Start(result);
            }
        }

        private void uploadTask_RunTask(object param, ref object result)
        {
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest uploadRequest = (HttpWebRequest)WebRequest.Create(
                "http://www.virustotal.com/vt/en/recepcionf?" + (string)param);
            uploadRequest.ServicePoint.ConnectionLimit = 20;
            uploadRequest.UserAgent = "ProcessHacker " + Application.ProductVersion;
            uploadRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            uploadRequest.Timeout = System.Threading.Timeout.Infinite;
            uploadRequest.KeepAlive = true;
            uploadRequest.Method = WebRequestMethods.Http.Post;

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

            if (uploadTask.Cancelled)
            {
                uploadRequest.Abort();
                return;
            }

            try
            {
                uploadStopwatch = new Stopwatch();
                uploadStopwatch.Start();

                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    uploadRequest.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;

                    using (Stream requestStream = uploadRequest.GetRequestStream())
                    {
                        // Write out our post header
                        requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                        // Write out the file contents
                        byte[] buffer = new Byte[checked((uint)Math.Min(32, (int)fileStream.Length))];

                        int bytesRead = 0;

                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            if (uploadTask.Cancelled)
                            {
                                uploadRequest.Abort();
                                return;
                            }

                            requestStream.Write(buffer, 0, bytesRead);

                            int progress = (int)(((double)fileStream.Position * 100 / fileStream.Length));

                            if (uploadStopwatch.ElapsedMilliseconds > 0)
                                bytesPerSecond = fileStream.Position * 1000 / uploadStopwatch.ElapsedMilliseconds;

                            bytesTransferred = fileStream.Position;

                            if (this.IsHandleCreated)
                                this.BeginInvoke(new Action<int>(this.ChangeProgress), progress);
                        }

                        if (uploadTask.Cancelled)
                        {
                            uploadRequest.Abort();
                            return;
                        }

                        // Write out the trailing boundary
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

                        requestStream.Close();
                    }
                }
            }
            catch (WebException ex)
            {
                // RequestCanceled will occour when we cancel the WebRequest.
                // Filter out that exception but log all others.
                if (ex != null)
                {
                    if (ex.Status != WebExceptionStatus.RequestCanceled)
                    {
                        PhUtils.ShowException("Unable to upload the file", ex);
                        Logging.Log(ex);

                        if (this.IsHandleCreated)
                            this.BeginInvoke(new MethodInvoker(this.Close));
                    }
                }
            }

            if (uploadTask.Cancelled)
            {
                uploadRequest.Abort();
                return;
            }

            WebResponse response = uploadRequest.GetResponse();

            //Stream s = responce.GetResponseStream(); 
            //StreamReader sr = new StreamReader(s);
            //sr.ReadToEnd();

            //Return the response URL 
            result = response.ResponseUri.AbsoluteUri;
        }

        private void ChangeProgress(int progress)
        {
            uploadedLabel.Text = "Uploaded: " + Utils.FormatSize(bytesTransferred) +
                          " (" + ((double)bytesTransferred * 100 / totalFileSize).ToString("F2") + "%)";
            totalSizeLabel.Text = "Total Size: " + Utils.FormatSize(totalFileSize);
            speedLabel.Text = "Speed: " + Utils.FormatSize(bytesPerSecond) + "/s";
            progressUpload.Value = progress;

            if (OSVersion.HasExtendedTaskbar)
                Windows7Taskbar.SetTaskbarProgress(Program.HackerWindow, this.progressUpload);
        }

        private void uploadTask_Completed(object result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ThreadTaskCompletedDelegate(uploadTask_Completed), result);
                return;
            }

            //TODO: future additions will parse the page and  
            //display the appropriate infomation but for now just mirror  
            //the functionality of the VirusTotal desktop client and
            //launch the URL in the default browser

            var webException = uploadTask.Exception as WebException;

            if (webException != null && webException.Status != WebExceptionStatus.Success)
            {
                if (webException.Status != WebExceptionStatus.RequestCanceled)
                {
                    PhUtils.ShowException("Unable to upload the file", webException);
                    this.Close();
                }
            }
            else if (result != null && !uploadTask.Cancelled) //sanity check
            {
                Program.TryStart(result.ToString());
            }

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}