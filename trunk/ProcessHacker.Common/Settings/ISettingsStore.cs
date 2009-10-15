/*
 * Process Hacker - 
 *   settings store interface
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

namespace ProcessHacker.Common.Settings
{
    public interface ISettingsStore
    {
        /// <summary>
        /// Flushes persistent storage.
        /// </summary>
        void Flush();

        /// <summary>
        /// Gets a setting value from persistent storage.
        /// </summary>
        /// <param name="name">The case-sensitive name of the setting.</param>
        /// <returns>A string if a value was found for the setting, otherwise null.</returns>
        string GetValue(string name);

        /// <summary>
        /// Resets the persistent storage, deleting all stored values.
        /// </summary>
        /// <remarks>
        /// This usually means deleting the settings file. A flush operation 
        /// is also assumed to be performed.
        /// </remarks>
        void Reset();

        /// <summary>
        /// Saves a setting value into persistent storage.
        /// </summary>
        /// <param name="name">The case-sensitive name of the setting.</param>
        /// <param name="value">The new value of the setting.</param>
        void SetValue(string name, string value);
    }
}
