using System;
using System.Collections.Generic;
using System.Text; 
using System.Windows.Forms;
using ProcessHacker.Base;

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
            AppInstance.ProcessWindow.AddTabPage("testTabPage", this.MakeTestTabPage);
        }

        public override void OnUnload()
        {
            AppInstance.ProcessWindow.RemoveTabPage("testTabPage");
        }

        private TabPage MakeTestTabPage(int pid)
        {
            TabPage page = new TabPage();

            page.Controls.Add(new Label()
                {
                    AutoSize = true,
                    Text = "This is a test tab page, and you are viewing properties for PID " + pid.ToString()
                });
            page.Name = "testTabPage";
            page.Padding = new Padding(20);
            page.Text = "Test Tab Page";
            page.UseVisualStyleBackColor = true;

            return page;
        }
    }
}
