using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Settings
{
    public abstract class SettingsBase
    {
        private SettingsManager _manager;

        public object this[string name]
        {
            get { return _manager.GetProperty(name); }
            set { _manager.SetProperty(name, value); }
        }
    }
}
