using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Base
{
    public class PluginSettingsControlBase : UserControl
    {
        public virtual void OnSettingsSaved() { }
    }
}
