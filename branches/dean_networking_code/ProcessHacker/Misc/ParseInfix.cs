/*
 * Process Hacker - 
 *   infix-notation parser
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

namespace ProcessHacker
{
    public class ParseInfix
    {
        public char[] Operators = new char[] { '^', '&', '|', '~', '<', '>', '!', '*', '/', '%', '+', '-' };
        public int[] OperatorOrders = new int[] { 0, 1, 1, 2, 3, 3, 4, 5, 5, 6, 7, 7 };
        public bool[] LeftAssociative = new bool[] { true, true, true, true, false, false, false, true, true, true, true, true };

        private List<Tokenizer.Token> _tokens;
        private Stack<Tokenizer.Token> _stack;
        private Queue<Tokenizer.Token> _out;

        public ParseInfix(List<Tokenizer.Token> tokens)
        {
            _tokens = tokens;
            _stack = new Stack<Tokenizer.Token>();
            _out = new Queue<Tokenizer.Token>();
        }

        public Queue<Tokenizer.Token> Out
        {
            get { return _out; }
        }

        private int getOrder(char c)
        {
            int i = 0;

            foreach (char ch in Operators)
            {
                if (ch == c)
                {
                    return OperatorOrders[i];
                }

                i++;
            }

            return 99;
        }

        private bool isLeftAssoc(char c)
        {
            int i = 0;

            foreach (char ch in Operators)
            {
                if (ch == c)
                {
                    return LeftAssociative[i];
                }

                i++;
            }

            return true;
        }

        public void Parse()
        {
             
        }
    }
}
