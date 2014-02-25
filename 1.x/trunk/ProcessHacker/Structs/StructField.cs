/*
 * Process Hacker - 
 *   struct field data
 * 
 * Copyright (C) 2008-2009 wj32
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

        /// <summary>
        /// Gets the alignment of the field, in bytes.
        /// </summary>
        public int Alignment
        {
            get
            {
                if (this.IsPointer)
                {
                    return IntPtr.Size;
                }
                else
                {
                    switch (_type)
                    {
                        case FieldType.Bool32:
                            return 4;
                        case FieldType.Bool8:
                            return 1;
                        case FieldType.CharASCII:
                            return 1;
                        case FieldType.CharUTF16:
                            return 2; // UCS-2
                        case FieldType.Double:
                            return 8;
                        case FieldType.Int16:
                            return 2;
                        case FieldType.Int32:
                            return 4;
                        case FieldType.Int64:
                            return 8;
                        case FieldType.Int8:
                            return 1;
                        case FieldType.PVoid:
                            return IntPtr.Size;
                        case FieldType.Single:
                            return 2;
                        case FieldType.StringASCII:
                            return 1;
                        case FieldType.StringUTF16:
                            return 2;
                        case FieldType.Struct:
                            return IntPtr.Size;
                        case FieldType.UInt16:
                            return 2;
                        case FieldType.UInt32:
                            return 4;
                        case FieldType.UInt64:
                            return 8;
                        case FieldType.UInt8:
                            return 1;
                        default:
                            return 1;
                    }
                }
            }
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
                    return IntPtr.Size;
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
                        case FieldType.Double:
                            size = 8;
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
                            size = IntPtr.Size;
                            break;
                        case FieldType.Single:
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
