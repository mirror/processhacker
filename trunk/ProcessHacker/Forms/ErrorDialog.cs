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
using System.Windows.Forms;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Net;
using ProcessHacker.Native;
using ProcessHacker.Components;

namespace ProcessHacker
{
    public partial class ErrorDialog : Form
    {
        Exception eX;
        string trackerItem;

        public ErrorDialog(Exception ex)
        {
            InitializeComponent();

            eX = ex;

            //eX.GetType().ToString() + ": " + eX.Message;

            textException.AppendText(eX.ToString());

            try
            {
                textException.AppendText("\r\n\r\nDIAGNOSTIC INFORMATION\r\n" + Program.GetDiagnosticInformation());
            }
            catch
            { }
        }

        private void statusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PhUtils.ShowInformation("Bookmark the url for later reference!");
            
            if (trackerItem != null && trackerItem.Length >= 1)
                Program.TryStart(trackerItem);
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

                //Remove the icons or they remain in the systray
                Program.HackerWindow.ExecuteOnIcons((icon) => icon.Visible = false);
                Program.HackerWindow.ExecuteOnIcons((icon) => icon.Dispose());

                //Make sure KPH connection is closed
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
            this.submitReportButton.Enabled = false;
            this.statusLinkLabel.Visible = true;

            SFBugReporter wc = new SFBugReporter();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);

            NameValueCollection qc = new NameValueCollection();
            qc.Add("group_id", "24252"); //PH BugTracker ID: Required Do Not Change!
            qc.Add("atid", "1119665"); //PH BugTracker ID: Required Do Not Change!
            qc.Add("func", "postadd"); //PH BugTracker Function: Required Do Not Change!
            qc.Add("category_id", "100"); //100 = null
            qc.Add("artifact_group_id", "100"); //100 = null
            qc.Add("assigned_to", "100"); //User this report is to be assigned, 100 = null
            qc.Add("priority", "1"); //Bug Report Priority, 1 = Low (Blue) 5 = default (Green)
            qc.Add("summary", Uri.EscapeDataString(eX.Message) + " - " + System.Guid.NewGuid().ToString()); //Must be unique
            qc.Add("details", Uri.EscapeDataString(textException.Text));
            //qc.Add("input_file", FileName);
            //qc.Add("file_description", "Error-Report");
            qc.Add("submit", "Add Artifact"); //PH BugTracker Function: Required Do Not Change!

            wc.QueryString = qc;
            wc.DownloadStringAsync(new Uri("https://sourceforge.net/tracker/index.php"));
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            buttonContinue.Enabled = true;
            buttonQuit.Enabled = true;

            if (this.GetTitle(e.Result).Contains("ERROR"))
            {
                submitReportButton.Enabled = true;

                PhUtils.ShowError(this.GetTitle(e.Result)); //temporary
                PhUtils.ShowError(this.GetResult(e.Result)); //temporary
            }
            else
            {
                statusLinkLabel.Enabled = true;
                statusLinkLabel.Text = "Click here to view SourceForge Error Report";
               
                trackerItem = GetUrl(Regex.Replace(this.GetResult(e.Result), @"<(.|\n)*?>", string.Empty).Replace("&amp;", "&"));
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

            protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
            {
                WebResponse resp = base.GetWebResponse(request);
                return resp;
            }
        }

    }
}
