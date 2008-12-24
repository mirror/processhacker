/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */                   
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ProcessHacker
{
    /// <summary>
    /// Represents a Windows security identifier.
    /// </summary>
    public class WindowsSID
    {
        private SecurityIdentifier _id;
        private string _name;
        private string _nameNoDomain;

        /// <summary>
        /// Creates a new WindowsSID instance, specifying a pointer to a SID.
        /// </summary>
        /// <param name="SID">A pointer to a SID.</param>
        public WindowsSID(int SID)
        {
            _id = new SecurityIdentifier(new IntPtr(SID));
            _name = Win32.GetAccountName(SID, true);
            _nameNoDomain = Win32.GetAccountName(SID, false);
        }

        /// <summary>
        /// Gets the name of the user or group which corresponds with this SID.
        /// </summary>
        /// <param name="IncludeDomain">Specifies whether the domain is to be included in the name.</param>
        /// <returns>A string.</returns>
        public string GetName(bool IncludeDomain)
        {
            if (IncludeDomain)
                return _name;
            else
                return _nameNoDomain;
        } 

        /// <summary>
        /// Gets a SecurityIdentifier instance.
        /// </summary>
        public SecurityIdentifier GetSecurityIdentifier()
        {
            return _id;
        }

        /// <summary>
        /// Gets the SID's value using Security Descriptor Definition Language (SDDL).
        /// </summary>
        /// <returns>A string.</returns>
        public string GetStringSID()
        {
            return _id.ToString();
        }
    }
}
