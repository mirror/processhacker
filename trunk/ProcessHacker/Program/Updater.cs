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
using System.Windows.Forms;
using System.Xml;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;
using ProcessHacker.UI;
using System.Threading;
using System.Globalization;

namespace ProcessHacker
{
    public enum AppUpdateLevel: int
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
                this.appUpdateVersion = new Version(Application.ProductVersion);
            }

            public string appUpdateName { get; private set; }
            public Version appUpdateVersion { get; private set; }
            public DateTime appUpdateDate { get; private set; }
            public AppUpdateLevel appUpdateType { get; private set; }
            public string appUpdateMessage { get; private set; }
            public string appUpdateUrl { get; private set; }
            public string appUpdateHash { get; private set; }

            public UpdateItem(XmlNode node)
            {
                this.appUpdateName = node["title"].InnerText;
                this.appUpdateVersion = new Version(node["version"].InnerText);
                this.appUpdateDate = DateTime.Parse(node["released"].InnerText, DateTimeFormatInfo.InvariantInfo);
                this.appUpdateType = GetUpdateLevel(node["type"].InnerText);
                this.appUpdateMessage = node["description"].InnerText;
                this.appUpdateUrl = node["updateurl"].InnerText;
                this.appUpdateHash = node["hash"].InnerText;
            }

            public bool IsBetterThan(UpdateItem update, AppUpdateLevel preferredUpdateType)
            {
                if (this.appUpdateType == preferredUpdateType && this.appUpdateVersion > update.appUpdateVersion) 
                    return true; 
                else if (this.appUpdateType == AppUpdateLevel.Stable && this.appUpdateVersion > update.appUpdateVersion)
                    return true; 
                else
                    return false; 
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

        public static void Update(Form form)
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {   
                xDoc.Load(Properties.Settings.Default.AppUpdateURL);
            }
            catch (Exception ex)
            {
                Program.HackerWindow.QueueMessage("Update check exception: " + ex.Message);
                return;
            }

            UpdateItem currentVersion = new UpdateItem();
            UpdateItem bestUpdate = currentVersion;

            XmlNodeList nodes = xDoc.SelectNodes("//update");
            foreach (XmlNode node in nodes)
            {                
                UpdateItem update = new UpdateItem(node);

                // Check if this update is better than the one we already have.            
                if (update.IsBetterThan(bestUpdate, (AppUpdateLevel)Properties.Settings.Default.AppUpdateLevel))
                {      
                    bestUpdate = update;    
                }
            }
            PromptWithUpdate(form, bestUpdate, currentVersion);
        }

        private static void PromptWithUpdate(Form form, UpdateItem bestUpdate, UpdateItem currentVersion)
        {
            if (form.InvokeRequired)
            {
                form.BeginInvoke(new MethodInvoker(() => PromptWithUpdate(form, bestUpdate, currentVersion)));
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
                        "Your Version: " + currentVersion.appUpdateVersion.ToString() +
                        "\nServer Version: " + bestUpdate.appUpdateVersion.ToString() + "\n\n" + "\n" + bestUpdate.appUpdateMessage;
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
                        "Your Version: " + currentVersion.appUpdateVersion.ToString() +
                        "\nServer Version: " + bestUpdate.appUpdateVersion.ToString() + "\n\n" + bestUpdate.appUpdateMessage +
                        "\n\nDo you want to download the update now?",
                        "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation
                        );
                }

                if (dialogResult == DialogResult.Yes)
                {
                    DownloadUpdate(form, bestUpdate);
                }
            }
            else
            {
                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content =
                        "Your Version: " + currentVersion.appUpdateVersion.ToString() +
                        "\nServer Version: " + bestUpdate.appUpdateVersion.ToString();
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
    }
}
