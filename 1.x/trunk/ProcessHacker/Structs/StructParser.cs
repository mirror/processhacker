/*
 * Process Hacker - 
 *   struct definition file parser
 * 
 * Copyright (C) 2008 wj32
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
    public class ParserException : Exception
    {
        public ParserException(string fileName, int line, string message) : 
            base((new System.IO.FileInfo(fileName)).Name + ": " + line.ToString() + ": " + message) { }
    }

    public class StructParser
    {
        private Dictionary<string, StructDef> _structs;

        private string _fileName = "";
        private int _lineNumber = 1;
        private Dictionary<string, FieldType> _typeDefs = new Dictionary<string, FieldType>();
        private Dictionary<string, object> _defines = new Dictionary<string, object>();
        private bool _eatResult = false;

        public Dictionary<string, StructDef> Structs
        {
            get { return _structs; }
        }

        public StructParser(Dictionary<string, StructDef> structs)
        {
            _structs = structs;

            // Core types
            foreach (string s in Enum.GetNames(typeof(FieldType)))
                if (s != "Pointer")
                    _typeDefs.Add(s.ToLowerInvariant(), (FieldType)Enum.Parse(typeof(FieldType), s));

            // Core defines
            if (OSVersion.Architecture == OSArch.I386)
                _defines.Add("_X86_", null);
            else if (OSVersion.Architecture == OSArch.Amd64)
                _defines.Add("_AMD64_", null);
        }

        private FieldType GetType(string typeName)
        {
            if (_typeDefs.ContainsKey(typeName))
                return _typeDefs[typeName];
            else
                throw new ParserException(_fileName, _lineNumber, "Unknown identifier '" + typeName + "' (type name)");
        }

        private bool IsTypePointer(FieldType type)
        {
            return (type & FieldType.Pointer) != 0;
        }

        private string Preprocess(string text)
        {
            List<string> lines = new List<string>(text.Split('\n'));
            Stack<bool> includeStack = new Stack<bool>();
            bool include = true;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim(' ', '\t', '\r');

                if (line.StartsWith("#if"))
                {
                    string conditionText = line.Remove(0, "#if".Length).Trim(' ', '\t', '\r');

                    includeStack.Push(include);
                    include = _defines.ContainsKey(conditionText);
                }
                else if (line.StartsWith("#elseif"))
                {
                    if (!include)
                    {
                        string conditionText = line.Remove(0, "#elseif".Length).Trim(' ', '\t', '\r');

                        include = _defines.ContainsKey(conditionText);
                    }
                    else
                    {
                        include = false;
                    }
                }
                else if (line.StartsWith("#else"))
                {
                    include = !include;
                }
                else if (line.StartsWith("#define"))
                {
                    string conditionText = line.Remove(0, "#define".Length).Trim(' ', '\t', '\r');

                    if (!_defines.ContainsKey(conditionText))
                    {
                        _defines.Add(conditionText, null);
                    }
                }
                else if (line.StartsWith("#endif"))
                {
                    include = includeStack.Pop();
                }

                if (!include || line.StartsWith("#"))
                {
                    lines[i] = "";
                }
            }

            return string.Join("\n", lines.ToArray());
        }

        public void Parse(string fileName)
        {
            List<StructDef> defs = new List<StructDef>();
            int i = 0;
            string text = System.IO.File.ReadAllText(fileName);

            _lineNumber = 1;
            _fileName = fileName;

            text = Preprocess(text);

            while (true)
            {
                if (EatWhitespace(text, ref i)) break;

                string modeName = EatId(text, ref i);

                if (modeName == "")
                    throw new ParserException(_fileName, _lineNumber, "Expected keyword");

                if (modeName == "typedef")
                {
                    this.ParseTypeDef(text, ref i);
                }
                else if (modeName == "struct")
                {
                    this.ParseStructDef(text, ref i);
                }
                else if (modeName == "include")
                {
                    _eatResult = EatWhitespace(text, ref i);
                    string includeFile = EatQuotedString(text, ref i);

                    if (_eatResult || includeFile == "")
                        throw new ParserException(_fileName, _lineNumber, "String expected (file name)");

                    _eatResult = EatWhitespace(text, ref i);
                    string endSemicolon = EatSymbol(text, ref i);

                    if (_eatResult || endSemicolon != ";")
                        throw new ParserException(_fileName, _lineNumber, "Expected ';'");

                    System.IO.FileInfo info = new System.IO.FileInfo(_fileName);

                    // if the filename contains ':', use the absolute path. otherwise, append it to the 
                    // current filename's directory
                    string oldFileName = _fileName;
                    int oldLine = _lineNumber;

                    try
                    {
                        if (includeFile.Contains(":"))
                            this.Parse(includeFile);
                        else
                            this.Parse(info.DirectoryName + "\\" + includeFile);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        throw new ParserException(_fileName, _lineNumber, "Could not find the file '" + includeFile + "'");
                    }

                    _fileName = oldFileName;
                    _lineNumber = oldLine;
                }
                else
                {
                    throw new ParserException(_fileName, _lineNumber, "Expected keyword");
                }
            }
        }

        private void ParseTypeDef(string text, ref int i)
        {
            _eatResult = EatWhitespace(text, ref i);
            string existingType = EatId(text, ref i);

            if (_eatResult || existingType == "")
                throw new ParserException(_fileName, _lineNumber, "Expected identifier (type name)");

            if (!_typeDefs.ContainsKey(existingType))
                throw new ParserException(_fileName, _lineNumber, "Unknown identifier '" + existingType + "' (type name)");

            // check for asterisk (pointer)
            _eatResult = EatWhitespace(text, ref i);
            string asterisk = EatSymbol(text, ref i);

            if (asterisk != "*" && asterisk.Length > 0)
                throw new ParserException(_fileName, _lineNumber, "Unexpected '" + asterisk + "'");

            _eatResult = EatWhitespace(text, ref i);
            string newType = EatId(text, ref i);

            if (_eatResult || existingType == "")
                throw new ParserException(_fileName, _lineNumber, "Expected identifier (new type name)");

            if (_typeDefs.ContainsKey(newType))
                throw new ParserException(_fileName, _lineNumber, "Type name '" + newType + "' already used");

            if (this.IsTypePointer(this.GetType(existingType)) && asterisk == "*")
                throw new ParserException(_fileName, _lineNumber, "Invalid '*'; type '" + existingType + "' is already a pointer");

            _typeDefs.Add(newType, this.GetType(existingType) | (asterisk == "*" ? FieldType.Pointer : 0));

            _eatResult = EatWhitespace(text, ref i);
            string endSemicolon = EatSymbol(text, ref i);

            if (_eatResult || endSemicolon != ";")
                throw new ParserException(_fileName, _lineNumber, "Expected ';'");
        }

        private void ParseStructDef(string text, ref int i)
        {
            StructDef def = new StructDef();

            _eatResult = EatWhitespace(text, ref i);
            string structName = EatId(text, ref i);

            if (_eatResult || structName == "")
                throw new ParserException(_fileName, _lineNumber, "Expected identifier (struct name)");

            if (_structs.ContainsKey(structName))
                throw new ParserException(_fileName, _lineNumber, "Struct name '" + structName + "' already used");

            // add it first so that structs can be self-referential
            _structs.Add(structName, null);

            // {
            _eatResult = EatWhitespace(text, ref i);
            string openingBrace = EatSymbol(text, ref i);

            if (_eatResult || openingBrace != "{")
                throw new ParserException(_fileName, _lineNumber, "Expected '{'");

            while (true)
            {
                // }
                _eatResult = EatWhitespace(text, ref i);
                string endBrace = EatSymbol(text, ref i);

                if (_eatResult)
                    throw new ParserException(_fileName, _lineNumber, "Expected type name or '}'");
                if (endBrace == "}")
                    break;
                if (endBrace.Length > 0)
                    throw new ParserException(_fileName, _lineNumber, "Unexpected '" + endBrace + "'");

                // TYPE
                _eatResult = EatWhitespace(text, ref i);
                string typeName = EatId(text, ref i);

                if (_eatResult || typeName == "")
                    throw new ParserException(_fileName, _lineNumber, "Expected type name");

                FieldType type;

                if (_typeDefs.ContainsKey(typeName))
                {
                    type = this.GetType(typeName);
                }
                else
                {
                    type = FieldType.Struct;

                    if (!_structs.ContainsKey(typeName))
                        throw new ParserException(_fileName, _lineNumber, "Unknown identifier '" + typeName + "' (type or struct name)");
                }

                // type, without the pointer or array flag
                FieldType justType = type;

                // TYPE*
                // optional asterisk (pointer)
                _eatResult = EatWhitespace(text, ref i);

                if (EatSymbol(text, ref i) == "*")
                {
                    if (this.IsTypePointer(type))
                        throw new ParserException(_fileName, _lineNumber, "Invalid '*'; type '" + typeName + "' is already a pointer");

                    type |= FieldType.Pointer;
                }

                // TYPE* FIELDNAME
                _eatResult = EatWhitespace(text, ref i);
                string fieldName = EatId(text, ref i);

                if (_eatResult || fieldName == "")
                    throw new ParserException(_fileName, _lineNumber, "Expected identifier (struct field name)");

                if (def.ContainsField(fieldName))
                    throw new ParserException(_fileName, _lineNumber, "Field name '" + fieldName + "' already used");

                _eatResult = EatWhitespace(text, ref i);
                string leftSqBracket = EatSymbol(text, ref i);
                int varLength = 0;

                if (leftSqBracket == "[")
                {
                    _eatResult = EatWhitespace(text, ref i);
                    string fieldRefName = EatId(text, ref i);
                    string fieldSizeSpec = EatNumber(text, ref i);

                    if (fieldRefName != "")
                    {
                        if (!def.ContainsField(fieldRefName))
                            throw new ParserException(_fileName, _lineNumber, "Unknown identifier '" + fieldRefName + "' (field name)");

                        def.GetField(fieldRefName).SetsVarOn = fieldName;

                        // const add/multiply
                        int iSave = i;

                        _eatResult = EatWhitespace(text, ref i);
                        string plusOrMulOrDivSign = EatSymbol(text, ref i);

                        if (plusOrMulOrDivSign == "+")
                        {
                            def.GetField(fieldRefName).SetsVarOnAdd = EatParseInt(text, ref i);
                        }
                        else if (plusOrMulOrDivSign == "*")
                        {
                            def.GetField(fieldRefName).SetsVarOnMultiply = EatParseFloat(text, ref i);

                            int iSave2 = i;
                            _eatResult = EatWhitespace(text, ref i);
                            string plusSign = EatSymbol(text, ref i);

                            if (plusSign == "+")
                                def.GetField(fieldRefName).SetsVarOnAdd = EatParseInt(text, ref i);
                            else if (plusSign == "-")
                                def.GetField(fieldRefName).SetsVarOnAdd = -EatParseInt(text, ref i);
                            else
                                i = iSave2;
                        }
                        else if (plusOrMulOrDivSign == "/")
                        {
                            // here we just set SetsVarOnMultiply to 1 / value
                            def.GetField(fieldRefName).SetsVarOnMultiply = 1 / EatParseFloat(text, ref i);

                            int iSave2 = i;
                            _eatResult = EatWhitespace(text, ref i);
                            string plusSign = EatSymbol(text, ref i);

                            if (plusSign == "+")
                                def.GetField(fieldRefName).SetsVarOnAdd = EatParseInt(text, ref i);
                            else if (plusSign == "-")
                                def.GetField(fieldRefName).SetsVarOnAdd = -EatParseInt(text, ref i);
                            else
                                i = iSave2;
                        }
                        else
                        {
                            // that didn't work; restore the index
                            i = iSave;
                        }
                    }
                    else if (fieldSizeSpec != "")
                    {
                        try
                        {
                            varLength = (int)BaseConverter.ToNumberParse(fieldSizeSpec);
                            varLength = (int)BaseConverter.ToNumberParse(fieldSizeSpec);
                        }
                        catch
                        {
                            throw new ParserException(_fileName, _lineNumber, "Could not parse number '" + fieldSizeSpec + "'");
                        }
                    }
                    else
                    {
                        throw new ParserException(_fileName, _lineNumber, "Number or identifier expected (size specifier)");
                    }

                    // if it's not a string, it's an array
                    if (justType != FieldType.StringASCII && justType != FieldType.StringUTF16)
                        type |= FieldType.Array;

                    _eatResult = EatWhitespace(text, ref i);
                    string rightSqBracket = EatSymbol(text, ref i);

                    if (_eatResult || rightSqBracket != "]")
                        throw new ParserException(_fileName, _lineNumber, "Expected ']'");

                    // fix up the semicolon
                    _eatResult = EatWhitespace(text, ref i);
                    leftSqBracket = EatSymbol(text, ref i);
                }

                // TYPE* FIELDNAME;
                string endSemicolon = leftSqBracket;

                if (_eatResult || endSemicolon != ";")
                    throw new ParserException(_fileName, _lineNumber, "Expected ';'");

                StructField field = new StructField(fieldName, type);

                if (field.Type == FieldType.Struct)
                    field.StructName = typeName;

                field.VarArrayLength = varLength;
                field.VarLength = varLength;

                def.AddField(field);
            }

            _structs[structName] = def;
        }

        private float EatParseFloat(string text, ref int i)
        {
            _eatResult = EatWhitespace(text, ref i);
            string number = EatNumber(text, ref i);

            if (_eatResult || number == "")
                throw new ParserException(_fileName, _lineNumber, "Expected floating-point number");

            try
            {
                return float.Parse(number);
            }
            catch
            {
                throw new ParserException(_fileName, _lineNumber, "Could not parse number '" + number + "'");
            }
        }

        private int EatParseInt(string text, ref int i)
        {
            _eatResult = EatWhitespace(text, ref i);
            string number = EatNumber(text, ref i);

            if (_eatResult || number == "")
                throw new ParserException(_fileName, _lineNumber, "Expected integer");

            try
            {
                return (int)BaseConverter.ToNumberParse(number);
            }
            catch
            {
                throw new ParserException(_fileName, _lineNumber, "Could not parse number '" + number + "'");
            }
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
                else if (preComment)
                {
                    if (text[i] == '*')
                    {
                        preComment = false;
                        inComment = true;
                        i++;
                        continue;
                    }
                    else
                    {
                        // it's a mistake, revert!
                        i -= 1;
                        break;
                    }
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

        private string EatQuotedString(string text, ref int i)
        {
            StringBuilder sb = new StringBuilder();
            bool inEscape = false;

            if (text[i] == '"')
            {
                i++;
            }
            else
                return "";

            while (i < text.Length)
            {
                if (text[i] == '\\')
                {
                    inEscape = true; 
                    i++;
                    continue;
                }
                else if (inEscape)
                {
                    if (text[i] == '\\')
                        sb.Append('\\');
                    else if (text[i] == '"')
                        sb.Append('"');
                    else if (text[i] == '\'')
                        sb.Append('\'');
                    else if (text[i] == 'r')
                        sb.Append('\r');
                    else if (text[i] == 'n')
                        sb.Append('\n');
                    else if (text[i] == 't')
                        sb.Append('\t');
                    else
                        throw new ParserException(_fileName, _lineNumber, "Unrecognized escape sequence '\\" + text[i] + "'");

                    i++;
                    inEscape = false;
                    continue;
                }
                else if (text[i] == '"')
                {
                    i++;
                    break;
                }

                sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
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
                // allow hex numbers and floating-point numbers
                if (sb.Length == 1 && sb[0] == '0')
                {
                    if (!char.IsDigit(text[i]) && char.ToLower(text[i]) != 'x' && text[i] != '.')
                        break;
                }
                else if (sb.Length >= 2 && sb[0] == '0' && char.ToLower(sb[1]) == 'x')
                {
                    if (!(char.IsDigit(text[i]) ||
                        char.ToLower(text[i]) == 'a' ||
                        char.ToLower(text[i]) == 'b' ||
                        char.ToLower(text[i]) == 'c' ||
                        char.ToLower(text[i]) == 'd' ||
                        char.ToLower(text[i]) == 'e' ||
                        char.ToLower(text[i]) == 'f'))
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
                char c = text[i];

                if (c < ' ' || c > '~') // check if its an ASCII character
                    break;
                if (char.IsLetterOrDigit(c) || c == '_') // check if its eligible to be an identifier
                    break;

                sb.Append(c);
                i++;
            }

            return sb.ToString();
        }
    }
}
