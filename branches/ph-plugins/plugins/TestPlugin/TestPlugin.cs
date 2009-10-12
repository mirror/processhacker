using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text; 
using System.Windows.Forms;
using ProcessHacker.Base;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace TestPlugin
{
    public class TestPlugin : PluginBase
    {
        public override string Author
        {
            get { return "Internal"; }
        }

        public override string Description
        {
            get { return "A plugin to test the plugin system."; }
        }

        public override string Name
        {
            get { return "TestPlugin"; }
        }

        public override string Title
        {
            get { return "Test plugin"; }
        }

        public override void OnLoad()
        {
            AppInstance.ProcessHighlighting.AddFilter("sandboxieFilter", this.SandboxieFilter);
            AppInstance.ProcessWindow.AddTabPage("testTabPage", this.MakeTestTabPage);
        }

        public override PluginSettingsControlBase OnRetrieveSettingsControl()
        {
            return new TestPluginSettingsControl();
        }

        public override void OnUnload()
        {
            AppInstance.ProcessWindow.RemoveTabPage("testTabPage");
            Properties.Settings.Default.Save();
        }

        private bool SandboxieFilter(int pid, ref Color color)
        {
            try
            {
                using (var phandle = new ProcessHandle(pid, ProcessAccess.QueryInformation | ProcessAccess.VmRead))
                {
                    bool isSandboxie = false;

                    phandle.EnumModules((module) =>
                        {
                            if (module.BaseName.Equals("SbieDll.dll", StringComparison.InvariantCultureIgnoreCase))
                            {
                                isSandboxie = true;
                                return false;
                            }

                            return true;
                        });

                    if (isSandboxie)
                    {
                        color = Color.Black;
                        return true;
                    }
                }
            }
            catch
            { }

            return false;
        }

        private TabPage MakeTestTabPage(int pid)
        {
            TabPage page = new TabPage();

            page.Controls.Add(new Label()
                {
                    AutoSize = true,
                    Text = "This is a test tab page, and you are viewing properties for PID " + pid.ToString() + 
                    ".\r\nAlso note that the value of MySetting is now " + (++Properties.Settings.Default.MySetting).ToString() + "!"
                });
            page.Name = "testTabPage";
            page.Padding = new Padding(20);
            page.Text = "Test Tab Page";
            page.UseVisualStyleBackColor = true;

            return page;
        }
    }
}
