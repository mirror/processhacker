/*
 * Process Hacker - 
 *   base converter
 * 
 * Copyright (C) 2006-2008 wj32
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
using System.Text;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Contains methods to parse numbers from string representations using different bases.
    /// </summary>
    public static class BaseConverter
    {
        private static int[] _reverseChars = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 36, 37, 38, 39, 40, 41,
            42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 52,
            53, 54, 55, 56, 57, 58, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 59, 60, 61, 62, 63, 64, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32,
            33, 34, 35, 65, 66, 67, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// Reverses a string.
        /// </summary>
        /// <param name="str">The string to be reversed</param>
        /// <returns>The reversed string.</returns>
        public static string ReverseString(string str)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = str.Length - 1; i >= 0; i--)
            {
                sb.Append(str[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a string to a number using the specified base.
        /// </summary>
        /// <remarks>
        /// This function does not parse prefixes; to do so, use <see cref="ToNumberParse(string)"/>
        /// </remarks>
        /// <param name="number">The string to convert</param>
        /// <param name="b">The base to use</param>
        /// <returns></returns>
        public static decimal ToNumber(string number, int b)
        {
            if (b > 70)
                return 0;

            if (number == "")
                return 0;

            bool negative = number[0] == '-';
            int length = number.Length;
            long result = 0;

            if (negative)
            {
                length -= 1;
            }

            number = ReverseString(number).ToLowerInvariant();

            for (int i = 0; i < length; i++)
            {
                result += _reverseChars[number[i]] * ((long)Math.Pow(b, i));
            }

            if (negative)
                return -result;
            else
                return result;
        }

        /// <summary>
        /// Converts a string to a number, parsing prefixes to determine the base.
        /// </summary>
        /// <param name="number">The string to convert.</param>
        /// <returns></returns>
        public static decimal ToNumberParse(string number)
        {
            return ToNumberParse(number, true);
        }

        /// <summary>
        /// Converts a string to a number, parsing prefixes to determine the base.
        /// </summary>
        /// <param name="number">The string to convert.</param>
        /// <param name="allowNonStandardExts">Enables or disables non-standard prefixes for 
        /// bases 2 (b), 3 (t), 4 (q), 12 (w) and 32 (r).</param>
        /// <returns></returns>
        public static decimal ToNumberParse(string number, bool allowNonStandardExts)
        {                              
            if (number == "")
                return 0;

            bool negative = number[0] == '-';
            decimal result = 0;

            if (negative)
                number = number.Substring(1);

            if (number.Length > 2 && (number.Substring(0, 2) == "0x"))                  // hexadecimal
            {
                result = ToNumber(number.Substring(2), 16);
            }
            else if (number.Length > 1)
            {
                if (number[0] == '0')                           // octal
                {
                    result = ToNumber(number.Substring(1), 8);
                }
                else if (number[0] == 'b' && allowNonStandardExts)                      // binary
                {
                    result = ToNumber(number.Substring(1), 2);
                }
                else if (number[0] == 't' && allowNonStandardExts)                      // ternary
                {
                    result = ToNumber(number.Substring(1), 3);
                }
                else if (number[0] == 'q' && allowNonStandardExts)                      // quaternary
                {
                    result = ToNumber(number.Substring(1), 4);
                }
                else if (number[0] == 'w' && allowNonStandardExts)                      // base 12
                {
                    result = ToNumber(number.Substring(1), 12);
                }
                else if (number[0] == 'r' && allowNonStandardExts)                      // base 32
                {
                    result = ToNumber(number.Substring(1), 32);
                }
                else                                            // base 10
                {
                    result = ToNumber(number, 10);
                }
            }
            else                                                // base 10
            {
                result = ToNumber(number, 10);
            }

            if (negative)
                return -result;
            else
                return result;
        }
    }
}
