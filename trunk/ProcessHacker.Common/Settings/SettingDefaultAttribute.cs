/*
 * Process Hacker - 
 *   settings default attribute
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

namespace ProcessHacker.Common.Settings
{
    /// <summary>
    /// Specifies a default value for a setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SettingDefaultAttribute : Attribute
    {
        private string _value;

        /// <summary>
        /// Initializes a new instance of the SettingDefaultAttribute class.
        /// </summary>
        /// <param name="value">
        /// The default value of the setting specified as a string. 
        /// This value must be convertible to the setting's type.
        /// </param>
        public SettingDefaultAttribute(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the default value specified for the setting.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }
    }
}
