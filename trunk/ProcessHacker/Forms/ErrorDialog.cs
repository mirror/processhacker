/*
 * Process Hacker - 
 *   unhandled exception dialog
 * 
 * Copyright (C) 2008-2009 wj32
 * Copyright (C) 2008-2009 dmex
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
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;

namespace ProcessHacker
{
    public partial class ErrorDialog : Form
    {
        private Exception _exception;
        private string _trackerItem;
        private bool _isTerminating;

        public ErrorDialog(Exception ex, bool terminating)
        {
            InitializeComponent();

            _exception = ex;
            _isTerminating = terminating;

            textException.AppendText(_exception.ToString());

            if (_isTerminating)
                buttonContinue.Enabled = false;

            textException.AppendText("\r\n\r\nDIAGNOSTIC INFORMATION\r\n" + Program.GetDiagnosticInformation());
        }

        private void statusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_trackerItem))
                Program.TryStart(_trackerItem);
            else
                Program.TryStart("http://sourceforge.net/tracker/?atid=1119665&group_id=242527&func=browse");
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.Save();

                // Remove the icons or they remain in the system try.
                Program.HackerWindow.ExecuteOnIcons((icon) => icon.Visible = false);
                Program.HackerWindow.ExecuteOnIcons((icon) => icon.Dispose());

                // Make sure KPH connection is closed.
                if (ProcessHacker.Native.KProcessHacker.Instance != null)
                    ProcessHacker.Native.KProcessHacker.Instance.Close();
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
            }

            Win32.ExitProcess(1);
        }

        private void submitReportButton_Click(object sender, EventArgs e)
        {
            this.buttonContinue.Enabled = false;
            this.buttonQuit.Enabled = false;
            this.buttonSubmitReport.Enabled = false;
            this.statusLinkLabel.Visible = true;

            //SFBugReporter wc = new SFBugReporter();
            //wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            //wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);

            //NameValueCollection qc = new NameValueCollection();
            //qc.Add("group_id", "242527"); //PH BugTracker ID: Required Do Not Change!
            //qc.Add("atid", "1119665"); //PH BugTracker ID: Required Do Not Change!
            //qc.Add("func", "postadd"); //PH BugTracker Function: Required Do Not Change!
            //qc.Add("category_id", "100"); //100 = null
            //qc.Add("artifact_group_id", "100"); //100 = null
            //qc.Add("assigned_to", "100"); //User this report is to be assigned, 100 = null
            //qc.Add("priority", "5"); //Bug Report Priority, 1 = Low (Blue) 5 = default (Green)
            //summary must be completly unique to prevent duplicate submission errors.
            //qc.Add("summary", Uri.EscapeDataString(_exception.Message));
                //+ " - " + DateTime.Now.ToString("F")
                //+ " - " + System.Guid.NewGuid().ToString()); 
            //qc.Add("details", Uri.EscapeDataString(textException.Text));
            //qc.Add("input_file", FileName);
            //qc.Add("file_description", "Error-Report");
            //qc.Add("submit", "Add Artifact"); //PH BugTracker Function: Required Do Not Change!

            //wc.QueryString = qc;
            //wc.DownloadStringAsync(new Uri("https://sourceforge.net/tracker/index.php"));

            System.Windows.Forms.OpenFileDialog fd = new OpenFileDialog();

            fd.ShowDialog();


            HttpWebRequest sessionRequest = (HttpWebRequest)HttpWebRequest.Create(
                "https://sourceforge.net/tracker/index.php?" +  // https://sourceforge.net/tracker/index.php?
                "group_id=276861" + 
                "&atid=1175841" + 
                "&func=add" + 
                "&category_id=100" + 
                "&artifact_group_id=100" +
                "&assigned_to=100" + 
                "&priority=5" + 
                "&summary=" + Uri.EscapeDataString(_exception.Message) + DateTime.Now.ToString("F") +
                "&details=" + Uri.EscapeDataString(textException.Text) + 
                "&submit=Add Artifact");

            string boundary = "-----------------------------" + DateTime.Now.Ticks.ToString("x");

            sessionRequest.ServicePoint.Expect100Continue = true;
            sessionRequest.UserAgent = "ProcessHacker " + Application.ProductVersion;
            sessionRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            sessionRequest.Timeout = System.Threading.Timeout.Infinite;
            sessionRequest.KeepAlive = true;
            sessionRequest.Method = WebRequestMethods.Http.Post;

            // Build up the 'post' message header
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append(@"Content-Disposition: form-data; name=""group_id""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("276861");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""atid""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("1175841");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""func""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("postadd");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""category_id""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("100");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""artifact_group_id""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("100");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""assigned_to""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("100");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""priority""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append("5");
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""summary""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append(Uri.EscapeDataString(_exception.Message));
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""details""");
            sb.Append("\r\n");
            sb.Append("\r\n");
            sb.Append(Uri.EscapeDataString(textException.Text));
            sb.Append("\r\n");
            sb.Append(boundary);

            sb.Append(@"Content-Disposition: form-data; name=""input_file""; filename=" + fd.SafeFileName + "");
            sb.Append("\r\n");
            sb.Append(boundary);


            string postHeader = sb.ToString();
            byte[] postHeaderBytes = System.Text.Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself

            System.Text.StringBuilder sc = new System.Text.StringBuilder();
            sc.Append("\r\n");
            sc.Append("\r\n");
            sc.Append("\r\n");
            sc.Append(boundary);
            sc.Append("\r\n");

            sc.Append(@"Content-Disposition: form-data; name=""file_description""");
            sc.Append("\r\n");
            sc.Append("\r\n");
            sc.Append("");
            sc.Append("\r\n");
            sb.Append(boundary);

            sc.Append(@"Content-Disposition: form-data; name=""submit""");
            sc.Append("\r\n");
            sc.Append("\r\n");
            sc.Append("Add Artifact");
            sc.Append("\r\n");
            sc.Append(boundary);
            sc.Append("--");
            sc.Append("\r\n");

            string postFooter = sc.ToString();
            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes(postFooter);

            try
            {
                using (System.IO.FileStream fileStream = new System.IO.FileStream(fd.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    sessionRequest.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;

                    using (System.IO.Stream requestStream = sessionRequest.GetRequestStream())
                    {
                        // Write out our post header
                        requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                        // Write out the file contents
                        byte[] buffer = new Byte[checked((uint)Math.Min(32, (int)fileStream.Length))];

                        int bytesRead = 0;

                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
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
                        PhUtils.ShowException("Unable to upload the Report", ex);
                        Logging.Log(ex);
                    }
                }
            }


            WebResponse response = sessionRequest.GetResponse();

            System.IO.Stream s = response.GetResponseStream(); 
            System.IO.StreamReader sr = new System.IO.StreamReader(s);
          
            string htmlreply = sr.ReadToEnd();

            MessageBox.Show(GetTitle(htmlreply));
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!_isTerminating)
                buttonContinue.Enabled = true;
            buttonQuit.Enabled = true;

            if (e.Error != null || this.GetTitle(e.Result).Contains("ERROR"))
            {
                buttonSubmitReport.Enabled = true;

                if (e.Error != null)
                {
                    if (e.Error.InnerException != null)
                        PhUtils.ShowError("Unable to submit the error report: " + e.Error.InnerException.Message);
                    else
                        PhUtils.ShowError("Unable to submit the error report: " + e.Error.Message);
                }
                else
                {
                    PhUtils.ShowError("Unable to submit the error report: " + this.GetTitle(e.Result));
                }
            }
            else
            {
                statusLinkLabel.Enabled = true;
                statusLinkLabel.Text = "View SourceForge error report";

                _trackerItem = GetUrl(Regex.Replace(this.GetResult(e.Result), @"<(.|\n)*?>", string.Empty).Replace("&amp;", "&"));
            }
        }

        private string GetTitle(string data)
        {
            //http://regexlib.com/
            Match m = Regex.Match(data, @"<title>\s*(.+?)\s*</title>", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        private string GetResult(string data)
        {
            //http://regexlib.com/
            Match m = Regex.Match(data, @"<small>\s*(.+?)\s*</small>", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        private string GetUrl(string data)
        {
            //http://regexlib.com/
            Match m = Regex.Match(data, @"\b([\d\w\.\/\+\-\?\:]*)((ht|f)tp(s|)\:\/\/|[\d\d\d|\d\d]\.[\d\d\d|\d\d]\.|www\.|\.com|\.net|\.org)([\d\w\.\/\%\+\-\=\&amp;\?\:\\\&quot;\'\,\|\~\;]*)\b", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return "";
            }
        }

        public partial class SFBugReporter : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
                webRequest.UserAgent = "Process Hacker " + Application.ProductVersion;
                webRequest.Timeout = System.Threading.Timeout.Infinite;
                webRequest.ServicePoint.Expect100Continue = true; //fix for Sourceforge's lighttpd Server
                webRequest.KeepAlive = true;
                return webRequest;
            }
        }
    }
}
