/*
 * Process Hacker - 
 *   struct definition class and reader
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
using ProcessHacker.Common;
using ProcessHacker.Native;

namespace ProcessHacker.Structs
{
    public class StructDef
    {
        private List<StructField> _fields = new List<StructField>();
        private Dictionary<string, StructField> _fieldsByName = new Dictionary<string, StructField>();

        public IStructIOProvider IOProvider { get; set; }

        public int Size
        {
            get
            {
                int size = 0;

                foreach (StructField field in _fields)
                    size += field.Size;

                return size;
            }
        }

        public IntPtr Offset { get; set; }

        public Dictionary<string, StructDef> Structs { get; set; }

        public StructField AddField(StructField field)
        {
            _fieldsByName.Add(field.Name, field);
            _fields.Add(field);

            return field;
        }

        public bool ContainsField(string name)
        {
            return _fieldsByName.ContainsKey(name);
        }

        public StructField GetField(int index)
        {
            return _fields[index];
        }

        public StructField GetField(string name)
        {
            return _fieldsByName[name];
        }

        public void RemoveField(int index)
        {
            _fieldsByName.Remove(_fields[index].Name);
            _fields.RemoveAt(index);
        }

        public void RemoveField(string name)
        {
            _fields.Remove(_fieldsByName[name]);
            _fieldsByName.Remove(name);
        }

        public void RemoveField(StructField field)
        {
            _fieldsByName.Remove(field.Name);
            _fields.Remove(field);
        }

        public FieldValue[] Read()
        {
            FieldValue[] values;

            this.Read(out values);

            return values;
        }

        private int Read(StructField field, IntPtr offset, out FieldValue valueOut)
        {
            if (!field.IsArray)
                return this.ReadOnce(field, offset, out valueOut);

            // read array
            FieldValue value = new FieldValue() { FieldType = field.RawType, Name = field.Name };
            int readSize = 0;
            List<FieldValue> valueArray = new List<FieldValue>();

            for (int i = 0; i < field.VarArrayLength; i++)
            {
                FieldValue elementValue;

                readSize = offset.Increment(readSize).Align(field.Alignment).Decrement(offset).ToInt32();
                readSize += this.ReadOnce(field, offset.Increment(readSize), out elementValue);
                elementValue.Name = "[" + i.ToString() + "]";

                valueArray.Add(elementValue);
            }

            value.Value = valueArray.ToArray();
            value.StructName = field.StructName;
            valueOut = value;

            return readSize;
        }

        private unsafe int ReadOnce(StructField field, IntPtr offset, out FieldValue valueOut)
        {
            FieldValue value = new FieldValue() { FieldType = field.Type, Name = field.Name };
            int readSize = 0;

            switch (field.Type)
            {
                case FieldType.Bool32:
                    value.Value = Utils.ToInt32(IOProvider.ReadBytes(offset, 4),
                        Utils.Endianness.Little) != 0;
                    readSize = 4;
                    break;
                case FieldType.Bool8:
                    value.Value = IOProvider.ReadBytes(offset, 1)[0] != 0;
                    readSize = 1;
                    break;
                case FieldType.CharASCII:
                    value.Value = (char)IOProvider.ReadBytes(offset, 1)[0];
                    readSize = 1;
                    break;
                case FieldType.CharUTF16:
                    value.Value = Encoding.Unicode.GetString(IOProvider.ReadBytes(offset, 2))[0];
                    readSize = 2;
                    break;
                case FieldType.Double:
                    {
                        long data = Utils.ToInt64(
                            IOProvider.ReadBytes(offset, 8), Utils.Endianness.Little);

                        value.Value = *(double*)&data;
                        readSize = 8;
                    }
                    break;
                case FieldType.Int16:
                    value.Value = (short)Utils.ToUInt16(
                        IOProvider.ReadBytes(offset, 2), Utils.Endianness.Little);
                    readSize = 2;
                    break;
                case FieldType.Int32:
                    value.Value = Utils.ToInt32(
                        IOProvider.ReadBytes(offset, 4), Utils.Endianness.Little);
                    readSize = 4;
                    break;
                case FieldType.Int64:
                    value.Value = Utils.ToInt64(
                        IOProvider.ReadBytes(offset, 8), Utils.Endianness.Little);
                    readSize = 8;
                    break;
                case FieldType.Int8:
                    value.Value = (sbyte)IOProvider.ReadBytes(offset, 1)[0];
                    readSize = 1;
                    break;
                case FieldType.PVoid:
                    value.Value = IOProvider.ReadBytes(offset, IntPtr.Size).ToIntPtr();
                    readSize = IntPtr.Size;
                    break;
                case FieldType.Single:
                    {
                        int data = Utils.ToInt32(
                            IOProvider.ReadBytes(offset, 4), Utils.Endianness.Little);

                        value.Value = *(float*)&data;
                        readSize = 4;
                    }
                    break;
                case FieldType.StringASCII:
                    {
                        StringBuilder str = new StringBuilder();

                        if (field.VarLength == -1)
                        {
                            int i;

                            for (i = 0; ; i++)
                            {
                                byte b = IOProvider.ReadBytes(offset.Increment(i), 1)[0];

                                if (b == 0)
                                    break;

                                str.Append((char)b);
                            }

                            readSize = i;
                        }
                        else
                        {
                            str.Append(Encoding.ASCII.GetString(
                                IOProvider.ReadBytes(offset, field.VarLength)));
                            readSize = field.VarLength;
                        }

                        value.Value = str.ToString();
                    }

                    break;
                case FieldType.StringUTF16:
                    {
                        StringBuilder str = new StringBuilder();

                        if (field.VarLength == -1)
                        {
                            int i;

                            for (i = 0; ; i += 2)
                            {
                                byte[] b = IOProvider.ReadBytes(offset.Increment(i), 2);

                                if (Utils.IsEmpty(b))
                                    break;

                                str.Append(Encoding.Unicode.GetString(b));
                            }

                            readSize = i;
                        }
                        else
                        {
                            str.Append(Encoding.Unicode.GetString(
                                IOProvider.ReadBytes(offset, field.VarLength * 2))); // each char is 2 bytes
                            readSize = field.VarLength;
                        }

                        value.Value = str.ToString();
                    }

                    break;
                case FieldType.Struct:
                    {
                        FieldValue[] valuesOut;
                        StructDef struc = Structs[field.StructName];

                        struc.IOProvider = this.IOProvider;
                        struc.Offset = offset;
                        struc.Structs = this.Structs;
                        readSize = struc.Read(out valuesOut);
                        value.Value = valuesOut;
                        value.StructName = field.StructName;
                    }

                    break;
                case FieldType.UInt16:
                    value.Value = Utils.ToUInt16(
                        IOProvider.ReadBytes(offset, 2), Utils.Endianness.Little);
                    readSize = 2;
                    break;
                case FieldType.UInt32:
                    value.Value = Utils.ToUInt32(
                        IOProvider.ReadBytes(offset, 4), Utils.Endianness.Little);
                    readSize = 4;
                    break;
                case FieldType.UInt64:
                    value.Value = (ulong)Utils.ToInt64(
                        IOProvider.ReadBytes(offset, 8), Utils.Endianness.Little);
                    readSize = 8;
                    break;
                case FieldType.UInt8:
                    value.Value = IOProvider.ReadBytes(offset, 1)[0];
                    readSize = 1;
                    break;
                default:
                    readSize = 0;
                    break;
            }

            valueOut = value;

            return readSize;
        }

        public int Read(out FieldValue[] values)
        {
            List<FieldValue> list = new List<FieldValue>();
            int localOffset = 0;

            foreach (StructField field in _fields)
            {
                FieldValue value;

                // Perform alignment.
                localOffset = Offset.Increment(localOffset).Align(field.Alignment).Decrement(Offset).ToInt32();

                // Resolve pointer, if needed.
                if (field.IsPointer)
                {
                    IntPtr pointingTo = IOProvider.ReadBytes(Offset.Increment(localOffset), IntPtr.Size).ToIntPtr();

                    localOffset += IntPtr.Size;

                    if (pointingTo == IntPtr.Zero)
                        value = new FieldValue() { Name = field.Name, FieldType = field.RawType, Value = null };
                    else
                        Read(field, pointingTo, out value);

                    value.PointerValue = pointingTo;
                }
                else
                {
                    localOffset += Read(field, Offset.Increment(localOffset), out value);
                }

                if (field.SetsVarOn != null)
                {
                    _fieldsByName[field.SetsVarOn].VarLength = 
                        field.SetsVarOnAdd + (int)(int.Parse(value.Value.ToString()) * (decimal)field.SetsVarOnMultiply);
                    _fieldsByName[field.SetsVarOn].VarArrayLength =
                        field.SetsVarOnAdd + (int)(int.Parse(value.Value.ToString()) * (decimal)field.SetsVarOnMultiply);
                }

                list.Add(value);
            }

            values = list.ToArray();

            return localOffset;
        }
    }
}
