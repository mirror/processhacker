using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Settings
{
    public class SettingDefaultAttribute
    {
        private string _value;

        public SettingDefaultAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
