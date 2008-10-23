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
        public static List<string> Prefixes;

        public static List<Tokenizer.Token> FixNumbers(List<Tokenizer.Token> tokens)
        {
            List<Tokenizer.Token> newtokens = new List<Tokenizer.Token>();
            Tokenizer.Token prev1 = new Token();
            Tokenizer.Token prev2 = new Token();

            Prefixes = new List<string>();
            Prefixes.AddRange(new string[] { "b", "t", "q", "w", "r" });

            foreach (Tokenizer.Token token in tokens)
            {
                newtokens.Add(token);

                if (prev1.Type != TokenType.None)
                {
                    if (prev2.Type != TokenType.None)
                    {
                        if (token.Type == Tokenizer.TokenType.Number && prev1.Text == "x" && prev2.Text == "0")
                        {
                            Tokenizer.Token newtoken = new Tokenizer.Token();

                            newtoken.Type = Tokenizer.TokenType.Number;
                            newtoken.Text = "0x" + token.Text;

                            newtokens.Remove(prev1);
                            newtokens.Remove(prev2);
                            newtokens.Add(newtoken);
                        }
                    }
                    else
                    {
                        if (token.Type == Tokenizer.TokenType.Number && Prefixes.Contains(prev1.Text))
                        {
                            Tokenizer.Token newtoken = new Tokenizer.Token();

                            newtoken.Type = Tokenizer.TokenType.Number;
                            newtoken.Text = prev1.Text + token.Text;

                            newtokens.Remove(prev1);
                            newtokens.Add(newtoken);
                        }
                    }
                }

                prev2 = prev1;
                prev1 = token;
            }

            return newtokens;
        }
    }
}
