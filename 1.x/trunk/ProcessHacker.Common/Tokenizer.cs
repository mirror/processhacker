using System;
using System.Text;

namespace ProcessHacker.Common
{
    public class Tokenizer
    {
        private string _text;
        private int _i = 0;

        public Tokenizer(string text)
        {
            _text = text;
        }

        public int Index
        {
            get { return _i; }
            set { _i = value; }
        }

        public string EatId()
        {
            StringBuilder sb = new StringBuilder();

            while (_i < _text.Length)
            {
                // identifiers can't start with a number-
                if (sb.Length == 0)
                {
                    if (!(char.IsLetter(_text[_i]) || _text[_i] == '_'))
                        break;
                }
                else
                {
                    if (!(char.IsLetterOrDigit(_text[_i]) || _text[_i] == '_'))
                        break;
                }

                sb.Append(_text[_i]);
                _i++;
            }

            return sb.ToString();
        }

        public string EatNumber()
        {
            StringBuilder sb = new StringBuilder();

            while (_i < _text.Length)
            {
                // allow hex numbers and floating-point numbers
                if (sb.Length == 1 && sb[0] == '0')
                {
                    if (!char.IsDigit(_text[_i]) && char.ToLower(_text[_i]) != 'x' && _text[_i] != '.')
                        break;
                }
                else if (sb.Length >= 2 && sb[0] == '0' && char.ToLower(sb[1]) == 'x')
                {
                    if (!(char.IsDigit(_text[_i]) ||
                        char.ToLower(_text[_i]) == 'a' ||
                        char.ToLower(_text[_i]) == 'b' ||
                        char.ToLower(_text[_i]) == 'c' ||
                        char.ToLower(_text[_i]) == 'd' ||
                        char.ToLower(_text[_i]) == 'e' ||
                        char.ToLower(_text[_i]) == 'f'))
                        break;
                }
                else
                {
                    if (!char.IsDigit(_text[_i]))
                        break;
                }

                sb.Append(_text[_i]);
                _i++;
            }

            return sb.ToString();
        }

        public string EatQuotedString()
        {
            StringBuilder sb = new StringBuilder();
            bool inEscape = false;

            if (_text[_i] == '"')
            {
                _i++;
            }
            else
                return "";

            while (_i < _text.Length)
            {
                if (_text[_i] == '\\')
                {
                    inEscape = true;
                    _i++;
                    continue;
                }
                else if (inEscape)
                {
                    if (_text[_i] == '\\')
                        sb.Append('\\');
                    else if (_text[_i] == '"')
                        sb.Append('"');
                    else if (_text[_i] == '\'')
                        sb.Append('\'');
                    else if (_text[_i] == 'r')
                        sb.Append('\r');
                    else if (_text[_i] == 'n')
                        sb.Append('\n');
                    else if (_text[_i] == 't')
                        sb.Append('\t');
                    else
                        throw new Exception("Unrecognized escape sequence '\\" + _text[_i] + "'");

                    _i++;
                    inEscape = false;
                    continue;
                }
                else if (_text[_i] == '"')
                {
                    _i++;
                    break;
                }

                sb.Append(_text[_i]);
                _i++;
            }

            return sb.ToString();
        }

        public string EatSymbol()
        {
            StringBuilder sb = new StringBuilder();

            while (_i < _text.Length && sb.Length < 1) // we need a proper parser to solve this
            {
                char c = _text[_i];

                if (c < ' ' || c > '~') // check if its an ASCII character
                    break;
                if (char.IsLetterOrDigit(c) || c == '_') // check if its eligible to be an identifier
                    break;

                sb.Append(c);
                _i++;
            }

            return sb.ToString();
        }

        public string EatUntil(char c)
        {
            StringBuilder sb = new StringBuilder();

            while (_i < _text.Length && _text[_i] != c)
            {
                sb.Append(_text[_i]);
                _i++;
            }

            return sb.ToString();
        }

        public bool EatWhitespace()
        {
            return this.EatWhitespace(false);
        }

        public bool EatWhitespace(bool comments)
        {
            bool ranOut = true;
            bool preComment = false; // '/'
            bool inComment = false; // '*'
            bool prePostComment = false; // '*'

            while (_i < _text.Length)
            {
                if (comments && inComment && _text[_i] == '*')
                {
                    prePostComment = true;
                    _i++;
                    continue;
                }
                else if (comments && prePostComment && _text[_i] == '/')
                {
                    prePostComment = false;
                    inComment = false;
                    _i++;
                    continue;
                }
                else if (comments && !inComment && _text[_i] == '/')
                {
                    preComment = true;
                    _i++;
                    continue;
                }
                else if (comments && preComment)
                {
                    if (_text[_i] == '*')
                    {
                        preComment = false;
                        inComment = true;
                        _i++;
                        continue;
                    }
                    else
                    {
                        // it's a mistake, revert!
                        _i -= 1;
                        break;
                    }
                }
                else
                {
                    preComment = false;
                    prePostComment = false;
                }

                if (!(_text[_i] == '\r' || _text[_i] == '\n' || _text[_i] == ' ' || _text[_i] == '\t') && !inComment)
                {
                    ranOut = false;
                    break;
                }

                _i++;
            }

            return ranOut;
        }
    }
}
