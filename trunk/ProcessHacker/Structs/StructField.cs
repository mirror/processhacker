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
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Structs
{
    public class StructField
    {
        public static int SizeOf(FieldType type)
        {
            switch (type)
            {
                case FieldType.Bool32:
                    return 4;
                case FieldType.Bool8:
                    return 1;
                case FieldType.CharASCII:
                    return 1;
                case FieldType.CharUTF16:
                    return 2; // UCS-2
                case FieldType.CharUTF8:
                    return 1; // fake
                case FieldType.Handle:
                    return 4;
                case FieldType.Int16:
                    return 2;
                case FieldType.Int32:
                    return 4;
                case FieldType.Int64:
                    return 8;
                case FieldType.Int8:
                    return 1;
                case FieldType.StringASCII:
                    return -1;
                case FieldType.StringUTF16:
                    return -1;
                case FieldType.StringUTF8:
                    return -1;
                case FieldType.Struct:
                    return -1;
                case FieldType.UInt16:
                    return 2;
                case FieldType.UInt32:
                    return 4;
                case FieldType.UInt64:
                    return 8;
                case FieldType.UInt8:
                    return 1;
                default:
                    return -1;
            }
        }

        private FieldType _type;
        private bool _ptr;

        /// <summary>
        /// Gets the size of the field, in bytes.
        /// </summary>
        public int Size
        {
            get
            {
                if (_ptr)
                    return 4; // 32-bit only
                else
                    return StructField.SizeOf(_type);
            }
        }
    }
}
