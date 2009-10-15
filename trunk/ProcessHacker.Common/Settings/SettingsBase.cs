/*
 * Process Hacker - 
 *   settings base class
 * 
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace ProcessHacker.Common.Settings
{
    public abstract class SettingsBase
    {
        private ISettingsStore _store;
        private Dictionary<string, object> _modifiedSettings = new Dictionary<string, object>();
        private Dictionary<string, string> _defaultsCache = new Dictionary<string, string>();
        private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        public SettingsBase(ISettingsStore store)
        {
            _store = store;
        }

        public object this[string name]
        {
            get { return this.GetValue(name); }
            set { this.SetValue(name, value); }
        }

        protected virtual object ConvertFromString(string value, Type valueType)
        {
            if (valueType.IsPrimitive)
                return Convert.ChangeType(value, valueType);
            else if (valueType == typeof(string))
                return value;

            var converter = TypeDescriptor.GetConverter(valueType);

            // Since all types can convert to System.String, we also need to make sure they can 
            // convert from System.String.
            if (converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string)))
                return converter.ConvertFromString(value);

            throw new InvalidOperationException("The setting '" + value + "' has an unsupported type.");
        }

        protected virtual string ConvertToString(object value, Type valueType)
        {
            if (valueType.IsPrimitive)
                return (string)Convert.ChangeType(value, typeof(string));
            else if (valueType == typeof(string))
                return (string)value;

            var converter = TypeDescriptor.GetConverter(valueType);

            // Since all types can convert to System.String, we also need to make sure they can 
            // convert from System.String.
            if (converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string)))
                return converter.ConvertToString(value);

            throw new InvalidOperationException("The setting '" + value + "' has an unsupported type.");
        }

        private string GetSettingDefault(string name)
        {
            lock (_defaultsCache)
            {
                if (!_defaultsCache.ContainsKey(name))
                {
                    var property = this.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                    var attributes = property.GetCustomAttributes(typeof(SettingDefaultAttribute), true);

                    if (attributes.Length == 1)
                    {
                        _defaultsCache.Add(name, (attributes[0] as SettingDefaultAttribute).Value);
                    }
                    else
                    {
                        _defaultsCache.Add(name, null);
                    }
                }

                return _defaultsCache[name];
            }
        }

        private Type GetSettingType(string name)
        {
            lock (_typeCache)
            {
                if (!_typeCache.ContainsKey(name))
                {
                    var property = this.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);

                    _typeCache.Add(name, property.PropertyType);
                }

                return _typeCache[name];
            }
        }

        private object GetValue(string name)
        {
            string value;

            lock (_modifiedSettings)
            {
                if (_modifiedSettings.ContainsKey(name))
                    return _modifiedSettings[name];
            }

            value = _store.GetValue(name);

            if (value == null)
                value = this.GetSettingDefault(name);
            if (value == null)
                value = "";

            return this.ConvertFromString(value, this.GetSettingType(name));
        }

        protected virtual void Invalidate() { }

        public void Reload()
        {
            lock (_modifiedSettings)
                _modifiedSettings.Clear();

            this.Invalidate();
        }

        public void Reset()
        {
            lock (_modifiedSettings)
                _modifiedSettings.Clear();

            _store.Reset();
            this.Invalidate();
        }

        public void Save()
        {
            lock (_modifiedSettings)
            {
                foreach (var pair in _modifiedSettings)
                {
                    _store.SetValue(pair.Key, this.ConvertToString(pair.Value, this.GetSettingType(pair.Key)));
                }

                _modifiedSettings.Clear();
            }

            _store.Flush();
        }

        private void SetValue(string name, object value)
        {
            lock (_modifiedSettings)
            {
                if (_modifiedSettings.ContainsKey(name))
                    _modifiedSettings[name] = value;
                else
                    _modifiedSettings.Add(name, value);
            }
        }
    }
}
