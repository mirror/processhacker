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
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ProcessHacker;
using ProcessHacker.Common;

/* ProcessHacker VirusTotal Implementation Authorized by:
 * Julio Canto | VirusTotal.com | Hispasec Sistemas Lab | Tlf: +34.902.161.025
 * Fax: +34.952.028.694 | PGP Key ID: EF618D2B | jcanto@hispasec.com 
 * 26/09/2009 - 2:39PM
 */

public partial class VirusTotalUploaderWindow : Form
{  
    string url;  
    string filepath;
    string processName;
     
    DateTime StartTime;
    long bytesPerSecond;
 
    public VirusTotalUploaderWindow(string procName, string procPath)  
    {
        InitializeComponent();
 
        processName = procName;
        filepath = procPath;

        this.Icon = Program.HackerWindow.Icon;
    }
      
    private void VirusTotalUploaderWindow_Load(object sender, EventArgs e)
    {
        fileLabel.Text = string.Format("Uploading {0} to VirusTotal", processName);
  
        WebClient vtId = new WebClient();
        vtId.Headers.Add("User-Agent", "Process Hacker " + Application.ProductVersion);
 
        url = "http://www.virustotal.com/vt/en/recepcionf?" + vtId.DownloadString("http://www.virustotal.com/vt/en/identificador");
 
        StartTime = DateTime.Now;
 
        UploadWorker.RunWorkerAsync();  
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
            return;

        using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
        {
            webrequest.ContentLength = postHeaderBytes.Length + fileStream.Length + boundaryBytes.Length;

            using (Stream requestStream = webrequest.GetRequestStream())
            {
                // Write out our post header
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                // Write out the file contents
                byte[] buffer = new Byte[checked((uint)Math.Min(256, (int)fileStream.Length))];

                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    if (UploadWorker.CancellationPending)
                        break;

                    requestStream.Write(buffer, 0, bytesRead);

                    int progress = (int)(((float)fileStream.Position / (float)fileStream.Length) * 100);

                    TimeSpan totalTime = DateTime.Now - StartTime;

                    if (totalTime.TotalMilliseconds > 0)
                        bytesPerSecond = fileStream.Position * 1000 / (long)totalTime.TotalMilliseconds;

                    //todo: add transfered data/filesize

                    UploadWorker.ReportProgress(progress);
                }

                // Write out the trailing boundary
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            }
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
        speedLabel.Text = Utils.FormatSize(bytesPerSecond) + "/s"; 
        progressBar2.Value = e.ProgressPercentage;  
    }

    private void UploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)  
    {
        //TODO: future additions will parse the page and  
        //display the appropriate infomation but for now just mirror  
        //the functionality of the VirusTotal desktop client and
        //launch the URL in the default browser
        System.Diagnostics.Process.Start(e.Result.ToString());

        this.Close();
    }
}
