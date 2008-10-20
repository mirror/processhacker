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

namespace ProcessHacker
{
    public partial class Tokenizer
    {              
        public enum TokenType
        {
            None = 0, 
            Whitespace = 1, 
            Number = 2, 
            Id = 3, 
            Symbol = 4
        }

        [Flags]
        public enum TokenTypeOptions
        {
            None = 0x00,
            TokenPerCharacter = 0x01   
        }

        public struct Token
        {
            public TokenType Type;
            public string Text;
        }

        public TokenTypeOptions[] TokenOptions;

        string _text;
        string _currentContents;
        TokenType _currentType;
        List<Token> _tokens;

        public Tokenizer(string text)
        {
            _text = text;
            _currentContents = "";
            _currentType = TokenType.None;
            _tokens = new List<Token>();

            TokenOptions = new TokenTypeOptions[5]
                { 0, 0, 0, 0, TokenTypeOptions.TokenPerCharacter };
        }

        public List<Token> Tokens
        {
            get { return _tokens; }
        }

        private void ProcessChar(TokenType type, char c)
        {
            if (_currentType == TokenType.None)
            {
                _currentType = type;
            }

            if (_currentType != type || 
                ((TokenOptions[(int)_currentType] & TokenTypeOptions.TokenPerCharacter) != TokenTypeOptions.None))
            {
                // New token 
                Token t = new Token();

                t.Type = _currentType;
                t.Text = _currentContents;

                _tokens.Add(t);

                _currentContents = "";
            }

            _currentType = type;
            _currentContents += c;
        }

        public void Tokenize()
        {
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
                {
                    ProcessChar(TokenType.Whitespace, c);
                }
                /*else if (c >= '0' && c <= '9')
                {
                    ProcessChar(TokenType.Number, c);
                }*/
                else if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    ProcessChar(TokenType.Id, c);
                }
                else
                {
                    ProcessChar(TokenType.Symbol, c);
                }
            }

            // Process last token
            ProcessChar(TokenType.None, ' ');
        }
    }
}
