using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Settings
{
    public abstract class SettingsStore
    {
        public abstract string GetValue(string name);
        public abstract void SetValue(string name, string value);
    }
}
