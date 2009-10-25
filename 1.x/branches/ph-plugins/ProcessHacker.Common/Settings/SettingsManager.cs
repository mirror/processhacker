using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Settings
{
    public sealed class SettingsManager
    {
        private SettingsStore _store;

        public SettingsManager(SettingsStore store)
        {
            _store = store;
        }

        public T GetProperty<T>(string name)
        {
            return (T)this.GetProperty(name);
        }

        public object GetProperty(string name)
        {
            return null;
        }

        public void SetProperty<T>(string name, T value)
        {
            this.SetProperty(name, value);
        }

        public void SetProperty(string name, object value)
        {

        }
    }
}
