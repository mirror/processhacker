using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ProcessHacker.Base;

namespace TestPlugin
{
    public partial class TestPluginSettingsControl : PluginSettingsControlBase
    {
        public TestPluginSettingsControl()
        {
            InitializeComponent();

            textSomeText.Text = Properties.Settings.Default.SomeText;
        }

        public override void OnSettingsSaved()
        {
            Properties.Settings.Default.SomeText = textSomeText.Text;
            Properties.Settings.Default.Save();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test!");
        }
    }
}
