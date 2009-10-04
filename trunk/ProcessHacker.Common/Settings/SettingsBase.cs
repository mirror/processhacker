using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Settings
{
    public abstract class SettingsBase
    {
        private SettingsStore _store;
        private Dictionary<string, object> _settings = new Dictionary<string, object>();

        public SettingsBase(SettingsStore store)
        {
            _store = store;
        }

        public object this[string name]
        {
            get { return this.GetValue(name); }
            set { this.SetValue(name, value); }
        }

        private object GetValue(string name)
        {
            return null;
        }

        private void SetValue(string name, object value)
        {
            
        }
    }
}
