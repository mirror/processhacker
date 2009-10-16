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
using System.ComponentModel;
using System.Reflection;

namespace ProcessHacker.Common.Settings
{
    /// <summary>
    /// Provides the base class used to support settings.
    /// </summary>
    public abstract class SettingsBase
    {
        private ISettingsStore _store;
        private Dictionary<string, object> _settings = new Dictionary<string, object>();
        private Dictionary<string, object> _modifiedSettings = new Dictionary<string, object>();
        private Dictionary<string, string> _defaultsCache = new Dictionary<string, string>();
        private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        /// <summary>
        /// Creates a settings class with the specified storage provider.
        /// </summary>
        /// <param name="store">The storage provider.</param>
        public SettingsBase(ISettingsStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Gets or sets a setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The value of the setting.</returns>
        public object this[string name]
        {
            get { return this.GetValue(name); }
            set { this.SetValue(name, value); }
        }

        /// <summary>
        /// Gets the underlying storage for the settings class.
        /// </summary>
        public ISettingsStore Store
        {
            get { return _store; }
        }

        /// <summary>
        /// Converts a string to an instance of another type.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="valueType">The type of the output value.</param>
        /// <returns>The converted value.</returns>
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
                return converter.ConvertFromInvariantString(value);

            throw new InvalidOperationException("The setting '" + value + "' has an unsupported type.");
        }

        /// <summary>
        /// Converts an object to a string.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="valueType">The type of the input value.</param>
        /// <returns>The string representation of the object.</returns>
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
                return converter.ConvertToInvariantString(value);

            throw new InvalidOperationException("The setting '" + value + "' has an unsupported type.");
        }

        /// <summary>
        /// Gets the default value of a setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>A string representation of the setting's default value.</returns>
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

        /// <summary>
        /// Gets the type of a setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The type of the setting.</returns>
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

        /// <summary>
        /// Gets the value of a setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The value of the setting.</returns>
        private object GetValue(string name)
        {
            object value;
            string settingValue;
            Type settingType;

            lock (_modifiedSettings)
            {
                if (_modifiedSettings.ContainsKey(name))
                    return _modifiedSettings[name];
            }

            lock (_settings)
            {
                if (_settings.ContainsKey(name))
                    return _settings[name];
            }

            settingValue = _store.GetValue(name);

            if (settingValue == null)
                settingValue = this.GetSettingDefault(name);
            if (settingValue == null)
                settingValue = "";

            settingType = this.GetSettingType(name);

            try
            {
                value = this.ConvertFromString(settingValue, settingType);
            }
            catch
            {
                // The stored value must be invalid. Return the default value.
                value = this.ConvertFromString(this.GetSettingDefault(name), settingType);
            }

            lock (_settings)
            {
                if (!_settings.ContainsKey(name))
                    _settings.Add(name, value);
            }

            return value;
        }

        /// <summary>
        /// Causes all cached setting values to be invalidated.
        /// </summary>
        public virtual void Invalidate() { }

        /// <summary>
        /// Discards any unsaved changes made to settings.
        /// </summary>
        public void Reload()
        {
            lock (_modifiedSettings)
                _modifiedSettings.Clear();
            lock (_settings)
                _settings.Clear();

            this.Invalidate();
        }

        /// <summary>
        /// Resets all settings to their default values.
        /// </summary>
        public void Reset()
        {
            lock (_modifiedSettings)
                _modifiedSettings.Clear();
            lock (_settings)
                _settings.Clear();

            _store.Reset();
            this.Invalidate();
        }

        /// <summary>
        /// Saves settings to persistent storage.
        /// </summary>
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

        /// <summary>
        /// Sets the value of a setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The new value of the setting.</param>
        private void SetValue(string name, object value)
        {
            lock (_modifiedSettings)
            {
                if (_modifiedSettings.ContainsKey(name))
                    _modifiedSettings[name] = value;
                else
                    _modifiedSettings.Add(name, value);
            }

            lock (_settings)
            {
                if (_settings.ContainsKey(name))
                    _settings[name] = value;
            }
        }
    }
}
