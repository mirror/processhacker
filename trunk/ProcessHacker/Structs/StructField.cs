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
        private FieldType _type;

        public StructField(string name, FieldType type)
        {
            VarLength = -1;
            VarArrayLength = 0;
            Name = name;
            _type = type;
        }

        public bool IsArray
        {
            get { return (_type & FieldType.Array) != 0; }
        }

        public bool IsPointer
        {
            get { return (_type & FieldType.Pointer) != 0; }
        }

        /// <summary>
        /// Gets the size of the field, in bytes.
        /// </summary>
        public int Size
        {
            get
            {
                if (this.IsPointer)
                {
                    return 4; // 32-bit only
                }
                else
                {
                    int size;

                    switch (_type)
                    {
                        case FieldType.Bool32:
                            size = 4;
                            break;
                        case FieldType.Bool8:
                            size = 1;
                            break;
                        case FieldType.CharASCII:
                            size = 1;
                            break;
                        case FieldType.CharUTF16:
                            size = 2; // UCS-2 
                            break;
                        case FieldType.Int16:
                            size = 2;
                            break;
                        case FieldType.Int32:
                            size = 4;
                            break;
                        case FieldType.Int64:
                            size = 8;
                            break;
                        case FieldType.Int8:
                            size = 1;
                            break;
                        case FieldType.PVoid:
                            size = 4;
                            break;
                        case FieldType.StringASCII:
                            size = VarLength;
                            break;
                        case FieldType.StringUTF16:
                            size = VarLength * 2;
                            break;
                        case FieldType.Struct:
                            size = 0;
                            break;
                        case FieldType.UInt16:
                            size = 2;
                            break;
                        case FieldType.UInt32:
                            size = 4;
                            break;
                        case FieldType.UInt64:
                            size = 8;
                            break;
                        case FieldType.UInt8:
                            size = 1;
                            break;
                        default:
                            size = 0;
                            break;
                    }

                    if (this.IsArray)
                        return size * VarArrayLength;
                    else
                        return size;
                }
            }
        }

        public string Name { get; set; }

        internal int VarArrayLength { get; set; }

        internal int VarLength { get; set; }

        public string SetsVarOn { get; set; }

        public int SetsVarOnAdd { get; set; }

        public float SetsVarOnMultiply { get; set; }

        public string StructName { get; set; }

        public FieldType Type
        {
            get { return _type & (~FieldType.Pointer) & (~FieldType.Array); }
        }

        public FieldType RawType
        {
            get { return _type; }
        }
    }
}
