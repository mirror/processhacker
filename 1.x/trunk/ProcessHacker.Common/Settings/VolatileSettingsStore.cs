using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Settings
{
    public sealed class VolatileSettingsStore : ISettingsStore
    {
        public void Flush()
        {
            // Nothing to flush.
        }

        public string GetValue(string name)
        {
            // No settings saved, return null.
            return null;
        }

        public void Reset()
        {
            // Nothing to reset.
        }

        public void SetValue(string name, string value)
        {
            // Don't save anything.
        }
    }
}
