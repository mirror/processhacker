/*
 * Process Hacker - 
 *   symbol resolve-level
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

namespace ProcessHacker.Native.Symbols
{
    /// <summary>
    /// Specifies the detail with which the address's name was resolved.
    /// </summary>
    public enum SymbolResolveLevel
    {
        /// <summary>
        /// Indicates that the address was resolved to a module, a function and possibly an offset. 
        /// For example: mymodule.dll!MyExportedFunction+0x123
        /// </summary>
        Function,

        /// <summary>
        /// Indicates that the address was resolved to a module and an offset.
        /// For example: mymodule.dll+0x4321
        /// </summary>
        Module,

        /// <summary>
        /// Indicates that the address was not resolved.
        /// For example: 0x12345678
        /// </summary>
        Address,

        /// <summary>
        /// Indicates that the address was invalid (for example, 0x0).
        /// </summary>
        Invalid
    }
}
