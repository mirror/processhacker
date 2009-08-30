/*
 * Process Hacker - 
 *   ProcessHacker Updater
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
using System.Windows.Forms;
using System.Xml;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.UI;
using System.Threading;

namespace ProcessHacker
{
    /// <summary>
    /// Application Updater Class for Process Hacker
    /// </summary>
    public static class Updater
    {
        public static void Update(Form mainFrm)
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {   
                xDoc.Load("http://processhacker.sourceforge.net/AppUpdate.xml");
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Program.HackerWindow.QueueMessage("Update Check SocketException: " + ex.Message);
                return;
            }
            catch (System.Net.WebException ex)
            {
                Program.HackerWindow.QueueMessage("Update Check WebException: " + ex.Message + " - " + ex.Status);
                return;
            }
            catch (Exception ex)
            {
                Program.HackerWindow.QueueMessage("Update Check Exception: " + ex.Message + " - " + ex.StackTrace);
                return;
            }

            string appUpdateUrl = null;
            string appUpdateHash = null;
            string appUpdateName = null;
            string appUpdateMessage = null;     
            string appUpdateVersion = null;
            string appUpdateRelease = null;

            int Release = 7;
            int Beta = 4;
            int Alpha = 5;

            XmlNodeList items = xDoc.SelectNodes("//update");
            foreach (XmlNode xn in items)
            {
                if (DateTime.Parse(xn["released"].InnerText) > GetAssemblyBuildDate() &&
                    new Version(xn["version"].InnerText) > new Version(Application.ProductVersion) &&
                    xn["type"].InnerText.Length == Release)
                {
                    Program.HackerWindow.QueueMessage(
                        "Found New " + xn["type"].InnerText + 
                        " Version: " + xn["version"].InnerText +
                        " - Current Version: " + Application.ProductVersion.ToString());
                   
                    appUpdateUrl = xn["updateurl"].InnerText;
                    appUpdateHash = xn["hash"].InnerText;
                    appUpdateName = xn["title"].InnerText;
                    appUpdateMessage = xn["description"].InnerText;
                    appUpdateVersion = xn["version"].InnerText;
                    appUpdateRelease = xn["released"].InnerText;
                }
                else if (Properties.Settings.Default.AppUpdateBeta)
                {
                    if (DateTime.Parse(xn["released"].InnerText) > GetAssemblyBuildDate() &&
                        new Version(xn["version"].InnerText) > new Version(Application.ProductVersion) &&
                        xn["type"].InnerText.Length == Release || 
                        xn["type"].InnerText.Length == Beta) 
                    {
                        Program.HackerWindow.QueueMessage(
                            "Found New " + xn["type"].InnerText + 
                            " Version: " + xn["version"].InnerText +
                            " - Current Version: " + Application.ProductVersion.ToString());

                        appUpdateUrl = xn["updateurl"].InnerText;
                        appUpdateHash = xn["hash"].InnerText;
                        appUpdateName = xn["title"].InnerText;
                        appUpdateMessage = xn["description"].InnerText;
                        appUpdateVersion = xn["version"].InnerText;
                        appUpdateRelease = xn["released"].InnerText;
                    }
                }
                else if (Properties.Settings.Default.AppUpdateAlpha)
                {
                    if (DateTime.Parse(xn["released"].InnerText) > GetAssemblyBuildDate() &&
                        new Version(xn["version"].InnerText) > new Version(Application.ProductVersion) &&
                        xn["type"].InnerText.Length == Release || 
                        xn["type"].InnerText.Length == Beta || 
                        xn["type"].InnerText.Length == Alpha)
                    {
                        Program.HackerWindow.QueueMessage(
                            "Found New " + xn["type"].InnerText + 
                            " Version: " + xn["version"].InnerText + 
                            " - Current Version: " + Application.ProductVersion.ToString());

                        appUpdateUrl = xn["updateurl"].InnerText;
                        appUpdateHash = xn["hash"].InnerText;
                        appUpdateName = xn["title"].InnerText;
                        appUpdateMessage = xn["description"].InnerText;
                        appUpdateVersion = xn["version"].InnerText;
                        appUpdateRelease = xn["released"].InnerText;
                    }
                }
            }

            if (appUpdateVersion != null && appUpdateMessage != null)
            {
                DialogResult dialogResult;

                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content = 
                        "Your Version: " + Application.ProductVersion + Environment.NewLine +
                        "Server Version: " + appUpdateVersion + Environment.NewLine + Environment.NewLine + appUpdateMessage;
                    td.MainInstruction = "Process Hacker Update Available";
                    td.WindowTitle = "Update Available";
                    td.MainIcon = TaskDialogIcon.SecurityWarning;
                    td.Buttons = new TaskDialogButton[]
                    {
                        new TaskDialogButton((int)DialogResult.Yes, "Download"),
                        new TaskDialogButton((int)DialogResult.No, "Cancel"),
                    };

                    dialogResult = (DialogResult)td.Show(Program.HackerWindowHandle);
                }
                else
                {
                    dialogResult = MessageBox.Show(
                        new WindowFromHandle(Program.HackerWindowHandle),
                        "Your Version: " + Application.ProductVersion + Environment.NewLine +
                        "Server Version: " + appUpdateVersion + Environment.NewLine + Environment.NewLine +
                        appUpdateMessage + Environment.NewLine + Environment.NewLine +
                        "Do you want to download the update now?",
                        "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation
                        );
                }

                if (dialogResult == DialogResult.Yes)
                {
                    DoDownload(
                        mainFrm, 
                        appUpdateName,
                        appUpdateUrl,
                        appUpdateVersion,
                        appUpdateRelease,
                        appUpdateHash);
                }
                
            }
            else
            {
                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content = 
                        "Your Version: " + Application.ProductVersion  + Environment.NewLine + Environment.NewLine + 
                        "Server Version: " + appUpdateVersion;
                    td.MainInstruction = "Process Hacker is up-to-date";
                    td.WindowTitle = "No Updates Available";
                    td.MainIcon = TaskDialogIcon.SecuritySuccess;
                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show(Program.HackerWindowHandle);
                }
                else
                {
                    MessageBox.Show(
                        new WindowFromHandle(Program.HackerWindowHandle),
                        "Process Hacker is up-to-date", 
                        "No Updates Available", MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                }
            }
        }

        private delegate void Delegate(Form mainFrm, string appUpdateName, string appUpdateURL, string appUpdateVersion, string appUpdateReleased, string appUpdateHash);
        private static void DoDownload(Form mainFrm, string appUpdateName, string appUpdateURL, string appUpdateVersion, string appUpdateReleased, string appUpdateHash)
        {
            //Updater executed on BackGround thread, We need to Invoke before calling DownloadWindow
            if (mainFrm.InvokeRequired)  
            {
                mainFrm.BeginInvoke(new Delegate(DoDownload), new object[] { mainFrm, appUpdateName, appUpdateURL, appUpdateVersion, appUpdateReleased, appUpdateHash });
                return;
            }
            //Executed by HackerWindow thread
                new UpdaterDownloadWindow(
                    appUpdateName, 
                    appUpdateURL, 
                    appUpdateVersion, 
                    appUpdateReleased, 
                    appUpdateHash).ShowDialog(new WindowFromHandle(Program.HackerWindowHandle));
        }

        private static DateTime? AssemblyBuildDate = null;
        private static DateTime GetAssemblyBuildDate()
        {
            if (AssemblyBuildDate != null) //Performance fix - Prevent reading current Assembly multiple times
            { 
                return (DateTime)AssemblyBuildDate;              
            }
            else
            {
                const int PeHeaderOffset = 60;
                const int LinkerTimestampOffset = 8;

                byte[] b = new byte[2048];
                System.IO.Stream s = default(System.IO.Stream);
                try
                {
                    s = new System.IO.FileStream(System.Reflection.Assembly.GetExecutingAssembly().Location, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    s.Read(b, 0, 2048);
                }
                finally
                {
                    if ((s != null))
                        s.Close();
                }

                int i = BitConverter.ToInt32(b, PeHeaderOffset);
                int SecondsSince1970 = BitConverter.ToInt32(b, i + LinkerTimestampOffset);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
                dt = dt.AddSeconds(SecondsSince1970);
                dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);

                AssemblyBuildDate = dt;

                return (DateTime)AssemblyBuildDate;
            }
        }

    }
}
