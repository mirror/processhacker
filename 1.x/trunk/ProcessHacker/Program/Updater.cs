/*
 * Process Hacker - 
 *   Process Hacker updater
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
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;

namespace ProcessHacker
{
    public enum AppUpdateLevel
    {
        Stable = 0,
        Beta = 1,
        Alpha = 2
    }

    /// <summary>
    /// Application Updater Class for Process Hacker
    /// </summary>
    public static class Updater
    {
        public class UpdateItem
        {
            public UpdateItem()
            {
                this.Version = new Version(Application.ProductVersion);
                this.Date = GetAssemblyBuildDate();
            }

            public UpdateItem(XmlNode node)
            {
                this.Name = node["title"].InnerText;
                this.Version = new Version(node["version"].InnerText);
                this.Date = DateTime.Parse(node["released"].InnerText, DateTimeFormatInfo.InvariantInfo);
                this.Type = GetUpdateLevel(node["type"].InnerText);
                this.Message = node["description"].InnerText;
                this.Url = node["updateurl"].InnerText;
                this.Hash = node["hash"].InnerText;
            }

            public string Name { get; private set; }
            public Version Version { get; private set; }
            public DateTime Date { get; private set; }
            public AppUpdateLevel Type { get; private set; }
            public string Message { get; private set; }
            public string Url { get; private set; }
            public string Hash { get; private set; }

            public bool IsBetterThan(UpdateItem update, AppUpdateLevel preferredType)
            {
                if (update == null)
                    return true;
                if ((int)this.Type > (int)preferredType)
                    return false;

                return this.Version > update.Version || this.Date > update.Date;
            }
        }

        private static AppUpdateLevel GetUpdateLevel(string level)
        {
            switch (level.ToLowerInvariant())
            {
                case "stable":
                    return AppUpdateLevel.Stable;
                case "beta":
                    return AppUpdateLevel.Beta;
                case "alpha":
                default:
                    return AppUpdateLevel.Alpha;
            }
        }

        public static void Update(Form form, bool interactive)
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {
                xDoc.Load(Settings.Instance.AppUpdateUrl);
            }
            catch (Exception ex)
            {
                if (interactive)
                    PhUtils.ShowException("Unable to download update information", ex);
                else
                    Program.HackerWindow.QueueMessage("Unable to download update information: " + ex.Message);

                return;
            }

            UpdateItem currentVersion = new UpdateItem();
            UpdateItem bestUpdate = currentVersion;

            XmlNodeList nodes = xDoc.SelectNodes("//update");
            foreach (XmlNode node in nodes)
            {
                try
                {
                    UpdateItem update = new UpdateItem(node);

                    // Check if this update is better than the one we already have.
                    if (update.IsBetterThan(bestUpdate, (AppUpdateLevel)Settings.Instance.AppUpdateLevel))
                        bestUpdate = update;
                }
                catch (Exception ex)
                {
                    Logging.Log(ex);
                }
            }

            PromptWithUpdate(form, bestUpdate, currentVersion, interactive);
        }

        private static void PromptWithUpdate(Form form, UpdateItem bestUpdate, UpdateItem currentVersion, bool interactive)
        {
            if (form.InvokeRequired)
            {
                form.BeginInvoke(new MethodInvoker(() => PromptWithUpdate(form, bestUpdate, currentVersion, interactive)));
                return;
            }

            if (bestUpdate != currentVersion)
            {
                DialogResult dialogResult;

                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content =
                        "Your Version: " + currentVersion.Version.ToString() +
                        "\nServer Version: " + bestUpdate.Version.ToString() + "\n\n" + "\n" + bestUpdate.Message;
                    td.MainInstruction = "Process Hacker update available";
                    td.WindowTitle = "Update available";
                    td.MainIcon = TaskDialogIcon.SecurityWarning;
                    td.Buttons = new TaskDialogButton[]
                    {
                        new TaskDialogButton((int)DialogResult.Yes, "Download"),
                        new TaskDialogButton((int)DialogResult.No, "Cancel"),
                    };

                    dialogResult = (DialogResult)td.Show(form);
                }
                else
                {
                    dialogResult = MessageBox.Show(
                        form,
                        "Your Version: " + currentVersion.Version.ToString() +
                        "\nServer Version: " + bestUpdate.Version.ToString() + "\n\n" + bestUpdate.Message +
                        "\n\nDo you want to download the update now?",
                        "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation
                        );
                }

                if (dialogResult == DialogResult.Yes)
                {
                    DownloadUpdate(form, bestUpdate);
                }

            }
            else if (interactive)
            {
                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content =
                        "Your Version: " + currentVersion.Version.ToString() +
                        "\nServer Version: " + bestUpdate.Version.ToString();
                    td.MainInstruction = "Process Hacker is up-to-date";
                    td.WindowTitle = "No updates available";
                    td.MainIcon = TaskDialogIcon.SecuritySuccess;
                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show(form);
                }
                else
                {
                    MessageBox.Show(
                        form,
                        "Process Hacker is up-to-date.",
                        "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                }
            }
        }

        private static void DownloadUpdate(Form form, UpdateItem updateItem)
        {
            if (form.InvokeRequired)  
            {
                form.BeginInvoke(new MethodInvoker(() => DownloadUpdate(form, updateItem)));
                return;
            }

            new UpdaterDownloadWindow(updateItem).ShowDialog(form);
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
