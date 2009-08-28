using System;
using System.Windows.Forms;
using System.Xml;
using ProcessHacker.Common;
using ProcessHacker.Components;
using ProcessHacker.Native;

namespace ProcessHacker
{
    public static class Updater
    {
        public static void Update()
        {
            XmlDocument xDoc = new XmlDocument();

            try
            {
                xDoc.Load("http://processhacker.sourceforge.net/AppUpdate.xml");
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return;
            }

            XmlNodeList items = xDoc.SelectNodes("//update");
            string appUpdateMessage = null;
            string appUpdateUrl = null;
            string appUpdateVersion = null;

            foreach (XmlNode xn in items)
            {
                if (
                    DateTime.Parse(xn["released"].InnerText) > GetAssemblyBuildDate() &&
                    new Version(xn["version"].InnerText) > new Version(Application.ProductVersion) &&
                    xn["type"].InnerText == "release"
                    )
                {
                    Program.HackerWindow.QueueMessage("Found New Release Version: " + xn["version"].InnerText +
                        " - Current Version: " + Application.ProductVersion.ToString());

                    appUpdateVersion = xn["version"].InnerText;
                    appUpdateMessage = xn["description"].InnerText;
                    appUpdateUrl = xn["updateurl"].InnerText;
                }
                else if (Properties.Settings.Default.AppUpdateBeta)
                {
                    if (
                        DateTime.Parse(xn["released"].InnerText) > GetAssemblyBuildDate() &&
                        new Version(xn["version"].InnerText) > new Version(Application.ProductVersion) &&
                        xn["type"].InnerText == "release" || xn["type"].InnerText == "beta"
                        )
                    {
                        Program.HackerWindow.QueueMessage("Found New Beta Version: " + xn["version"].InnerText +
                            " - Current Version: " + Application.ProductVersion.ToString());

                        appUpdateVersion = xn["version"].InnerText;
                        appUpdateMessage = xn["description"].InnerText;
                        appUpdateUrl = xn["updateurl"].InnerText;
                    }
                }
                else if (Properties.Settings.Default.AppUpdateAlpha)
                {
                    if (
                        DateTime.Parse(xn["released"].InnerText) > GetAssemblyBuildDate() &&
                        new Version(xn["version"].InnerText) > new Version(Application.ProductVersion) &&
                        xn["type"].InnerText == "release" || xn["type"].InnerText == "beta" || xn["type"].InnerText == "alpha"
                        )
                    {
                        Program.HackerWindow.QueueMessage("Found New Alpha Version: " + xn["version"].InnerText + " - Current Version: " + Application.ProductVersion.ToString());

                        appUpdateVersion = xn["version"].InnerText;
                        appUpdateMessage = xn["description"].InnerText;
                        appUpdateUrl = xn["updateurl"].InnerText;
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
                    td.Content = "Your Version: " + Application.ProductVersion + Environment.NewLine +
                        "Server Version: " + appUpdateVersion + Environment.NewLine + Environment.NewLine +
                        appUpdateMessage;
                    td.MainInstruction = "Process Hacker Update Available";
                    td.WindowTitle = "Update Available";
                    td.MainIcon = TaskDialogIcon.SecurityWarning;
                    td.Buttons = new TaskDialogButton[]
                    {
                        new TaskDialogButton((int)DialogResult.Yes, "Download"),
                        new TaskDialogButton((int)DialogResult.No, "Cancel"),
                    };

                    dialogResult = (DialogResult)td.Show(Form.ActiveForm);
                }
                else
                {
                    dialogResult = MessageBox.Show(
                        Form.ActiveForm,
                        "Your Version: " + Application.ProductVersion + Environment.NewLine +
                        "Server Version: " + appUpdateVersion + Environment.NewLine + Environment.NewLine +
                        appUpdateMessage + Environment.NewLine + Environment.NewLine +
                        "Do you want to download the update now?",
                        "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation
                        );
                }

                if (dialogResult == DialogResult.Yes)
                    Program.TryStart(appUpdateUrl);
            }
            else
            {
                if (OSVersion.HasTaskDialogs)
                {
                    TaskDialog td = new TaskDialog();
                    td.PositionRelativeToWindow = true;
                    td.Content = "Your Version: " + Application.ProductVersion + Environment.NewLine + Environment.NewLine + 
                        "Server Version: " + appUpdateVersion;
                    td.MainInstruction = "Process Hacker is up-to-date";
                    td.WindowTitle = "No Updates Available";
                    td.MainIcon = TaskDialogIcon.SecuritySuccess;
                    td.CommonButtons = TaskDialogCommonButtons.Ok;
                    td.Show(Form.ActiveForm);
                }
                else
                {
                    MessageBox.Show(Form.ActiveForm, "Process Hacker is up-to-date", 
                        "No Updates Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static DateTime GetAssemblyBuildDate()
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
            return dt;

        }
    }
}
