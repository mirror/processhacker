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
    public class ParserException : Exception
    {
        public ParserException(int line, string message) : base("Line " + line.ToString() + ": " + message) { }
    }

    public class StructParser
    {
        private Dictionary<string, StructDef> _structs;

        private int _lineNumber = 1;
        private Dictionary<string, FieldType> _typeDefs = new Dictionary<string, FieldType>();
        private bool _eatResult = false;

        public Dictionary<string, StructDef> Structs
        {
            get { return _structs; }
        }

        public StructParser(Dictionary<string, StructDef> structs)
        {
            _structs = structs;

            foreach (string s in Enum.GetNames(typeof(FieldType)))
                if (s != "Pointer")
                    _typeDefs.Add(s.ToLower(), (FieldType)Enum.Parse(typeof(FieldType), s));

            _typeDefs.Add("bool", FieldType.Bool32);
            _typeDefs.Add("boolean", FieldType.Bool8);

            _typeDefs.Add("char", FieldType.CharASCII);
            _typeDefs.Add("wchar", FieldType.CharUTF16);

            _typeDefs.Add("sbyte", FieldType.Int8);
            _typeDefs.Add("byte", FieldType.UInt8);

            _typeDefs.Add("short", FieldType.Int16);
            _typeDefs.Add("word", FieldType.Int16);
            _typeDefs.Add("ushort", FieldType.UInt16);

            _typeDefs.Add("int", FieldType.Int32);
            _typeDefs.Add("dword", FieldType.Int32);
            _typeDefs.Add("long", FieldType.Int32);
            _typeDefs.Add("uint", FieldType.UInt32);
            _typeDefs.Add("ulong", FieldType.UInt32);

            _typeDefs.Add("large_integer", FieldType.Int64);
            _typeDefs.Add("longlong", FieldType.Int64);
            _typeDefs.Add("qword", FieldType.Int64);
            _typeDefs.Add("ulonglong", FieldType.UInt64);

            _typeDefs.Add("str", FieldType.StringASCII);
            _typeDefs.Add("string", FieldType.StringASCII);
            _typeDefs.Add("wstr", FieldType.StringUTF16);
            _typeDefs.Add("wstring", FieldType.StringUTF16);
        }

        private FieldType GetType(string typeName)
        {
            if (_typeDefs.ContainsKey(typeName))
                return _typeDefs[typeName];
            else
                throw new ParserException(_lineNumber, "Unknown identifier '" + typeName + "' (type name)");
        }

        private bool IsTypePointer(FieldType type)
        {
            return (type & FieldType.Pointer) != 0;
        }

        public void Parse(string text)
        {
            List<StructDef> defs = new List<StructDef>();
            int i = 0;

            while (true)
            {
                if (EatWhitespace(text, ref i)) break;

                string structName = EatId(text, ref i);

                if (structName == "")
                    throw new ParserException(_lineNumber, "Expected identifier (struct name or 'typedef')");
                if (_structs.ContainsKey(structName))
                    throw new ParserException(_lineNumber, "Struct name '" + structName + "' already used");

                if (structName == "typedef")
                {
                    _eatResult = EatWhitespace(text, ref i);
                    string existingType = EatId(text, ref i);

                    if (_eatResult || existingType == "")
                        throw new ParserException(_lineNumber, "Expected identifier (type name)");

                    if (!_typeDefs.ContainsKey(existingType))
                        throw new ParserException(_lineNumber, "Unknown identifier '" + existingType + "' (type name)");

                    // check for asterisk (pointer)
                    _eatResult = EatWhitespace(text, ref i);
                    string asterisk = EatSymbol(text, ref i);

                    if (asterisk != "*" && asterisk.Length > 0)
                        throw new ParserException(_lineNumber, "Unexpected '" + asterisk + "'");

                    _eatResult = EatWhitespace(text, ref i);
                    string newType = EatId(text, ref i);

                    if (_eatResult || existingType == "")
                        throw new ParserException(_lineNumber, "Expected identifier (new type name)");

                    if (_typeDefs.ContainsKey(newType))
                        throw new ParserException(_lineNumber, "Type name '" + newType + "' already used");

                    if (this.IsTypePointer(this.GetType(existingType)) && asterisk == "*")
                        throw new ParserException(_lineNumber, "Invalid '*'; type '" + existingType + "' is already a pointer");

                    _typeDefs.Add(newType, this.GetType(existingType) | (asterisk == "*" ? FieldType.Pointer : 0));

                    _eatResult = EatWhitespace(text, ref i);
                    string endSemicolon = EatSymbol(text, ref i);

                    if (_eatResult || endSemicolon != ";")
                        throw new ParserException(_lineNumber, "Expected ';'");
                }
                else
                {
                    StructDef def = this.ParseDef(text, ref i);
                    
                    _structs.Add(structName, def);
                }
            }
        }

        private StructDef ParseDef(string text, ref int i)
        {
            StructDef def = new StructDef();

            // {
            _eatResult = EatWhitespace(text, ref i);
            string openingBrace = EatSymbol(text, ref i);

            if (_eatResult || openingBrace != "{")
                throw new ParserException(_lineNumber, "Expected '{'");

            while (true)
            {
                // }
                _eatResult = EatWhitespace(text, ref i);
                string endBrace = EatSymbol(text, ref i);

                if (_eatResult)
                    throw new ParserException(_lineNumber, "Expected type name or '}'");
                if (endBrace == "}")
                    break;
                if (endBrace.Length > 0)
                    throw new ParserException(_lineNumber, "Unexpected '" + endBrace + "'");

                // TYPE
                _eatResult = EatWhitespace(text, ref i);
                string typeName = EatId(text, ref i);

                if (_eatResult || typeName == "")
                    throw new ParserException(_lineNumber, "Expected type name");

                FieldType type;

                if (_typeDefs.ContainsKey(typeName))
                {
                    type = this.GetType(typeName);
                }
                else
                {
                    type = FieldType.Struct;

                    if (!_structs.ContainsKey(typeName))
                        throw new ParserException(_lineNumber, "Unknown identifier '" + typeName + "' (type or struct name)");
                }

                // TYPE*
                // optional asterisk (pointer)
                _eatResult = EatWhitespace(text, ref i);

                if (EatSymbol(text, ref i) == "*")
                {
                    if (this.IsTypePointer(type))
                        throw new ParserException(_lineNumber, "Invalid '*'; type '" + typeName + "' is already a pointer");

                    type |= FieldType.Pointer;
                }

                // TYPE* FIELDNAME
                _eatResult = EatWhitespace(text, ref i);
                string fieldName = EatId(text, ref i);

                if (_eatResult || fieldName == "")
                    throw new ParserException(_lineNumber, "Expected identifier (struct field name)");

                if (def.ContainsField(fieldName))
                    throw new ParserException(_lineNumber, "Field name '" + fieldName + "' already used");

                StructField field = new StructField(fieldName, type);

                if (field.Type == FieldType.Struct)
                    field.StructName = typeName;

                _eatResult = EatWhitespace(text, ref i);
                string leftSqBracket = EatSymbol(text, ref i);

                if (leftSqBracket == "[")
                {
                    _eatResult = EatWhitespace(text, ref i);
                    string fieldRefName = EatId(text, ref i);
                    string fieldSizeSpec = EatNumber(text, ref i);

                    if (fieldRefName != "")
                    {
                        if (!def.ContainsField(fieldRefName))
                            throw new ParserException(_lineNumber, "Unknown identifier '" + fieldRefName + "' (field name)");

                        def.GetField(fieldRefName).SetsVarOn = fieldName;
                    }
                    else if (fieldSizeSpec != "")
                    {
                        try
                        {
                            field.VarArrayLength = (int)BaseConverter.ToNumberParse(fieldSizeSpec);
                            field.VarLength = (int)BaseConverter.ToNumberParse(fieldSizeSpec);
                        }
                        catch
                        {
                            throw new ParserException(_lineNumber, "Could not parse number '" + fieldSizeSpec + "'");
                        }
                    }
                    else
                    {
                        throw new ParserException(_lineNumber, "Number or identifier expected (size specifier)");
                    }

                    _eatResult = EatWhitespace(text, ref i);
                    string rightSqBracket = EatSymbol(text, ref i);

                    if (_eatResult || rightSqBracket != "]")
                        throw new ParserException(_lineNumber, "Expected ']'");

                    // fix up the semicolon
                    _eatResult = EatWhitespace(text, ref i);
                    leftSqBracket = EatSymbol(text, ref i);
                }

                // TYPE* FIELDNAME;
                string endSemicolon = leftSqBracket;

                if (_eatResult || endSemicolon != ";")
                    throw new ParserException(_lineNumber, "Expected ';'");

                def.AddField(field);
            }

            return def;
        }

        // my idea of a tokenizer follows...
        private bool EatWhitespace(string text, ref int i) // and comments
        {
            bool ranOut = true;
            bool preComment = false; // '/'
            bool inComment = false; // '*'
            bool prePostComment = false; // '*'

            while (i < text.Length)
            {
                if (inComment && text[i] == '*')
                {
                    prePostComment = true;
                    i++;
                    continue;
                }
                else if (prePostComment && text[i] == '/')
                {
                    prePostComment = false;
                    inComment = false;
                    i++;
                    continue;
                }
                else if (!inComment && text[i] == '/')
                {
                    preComment = true;
                    i++;
                    continue;
                }
                else if (preComment && text[i] == '*')
                {
                    preComment = false;
                    inComment = true;
                    i++;
                    continue;
                }
                else
                {
                    preComment = false;
                    prePostComment = false;
                }

                if (text[i] == '\n')
                    _lineNumber++;

                if (!(text[i] == '\r' || text[i] == '\n' || text[i] == ' ' || text[i] == '\t') && !inComment)
                {
                    ranOut = false;
                    break;
                }

                i++;
            }

            return ranOut;
        }

        private string EatId(string text, ref int i)
        {
            StringBuilder sb = new StringBuilder();

            while (i < text.Length)
            {
                // identifiers can't start with a number-
                if (sb.Length == 0)
                {
                    if (!(char.IsLetter(text[i]) || text[i] == '_'))
                        break;
                }
                else
                {
                    if (!(char.IsLetterOrDigit(text[i]) || text[i] == '_'))
                        break;
                }

                sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }

        private string EatNumber(string text, ref int i)
        {
            StringBuilder sb = new StringBuilder();

            while (i < text.Length)
            {
                // allow hex numbers
                if (sb.Length == 1 && sb[0] == '0')
                {
                    if (!char.IsDigit(text[i]) && text[i] != 'x')
                        break;
                }
                else
                {
                    if (!char.IsDigit(text[i]))
                        break;
                }

                sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }

        private string EatSymbol(string text, ref int i)
        {
            StringBuilder sb = new StringBuilder();

            while (i < text.Length && sb.Length < 1) // we need a proper parser to solve this
            {
                if (!char.IsPunctuation(text[i]) && !char.IsSymbol(text[i]))
                    break;

                sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }
    }
}
