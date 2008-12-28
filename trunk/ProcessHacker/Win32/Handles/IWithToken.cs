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

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a Windows object that contains a token.
        /// </summary>
        /// <remarks>
        /// This interface is useful because both processes and threads have 
        /// tokens, but the method used to open their tokens are different.
        /// </remarks>
        public interface IWithToken
        {
            /// <summary>
            /// Opens and returns the object's token.
            /// </summary>
            /// <returns>A handle to the token.</returns>
            TokenHandle GetToken();

            /// <summary>
            /// Opens and returns the object's token.
            /// </summary>
            /// <param name="access">Specifies the desired access to the token.</param>
            /// <returns>A handle to the token.</returns>
            TokenHandle GetToken(TOKEN_RIGHTS access);
        }
    }
}
