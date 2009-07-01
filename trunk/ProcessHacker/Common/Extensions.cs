/*
 * Process Hacker - 
 *   misc. functions
 * 
 * Copyright (C) 2009 Dean
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
using System.Windows.Forms;
using Aga.Controls.Tree;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.UI;
using System.Drawing;

namespace ProcessHacker.Extensions
{
    public static class LongExtension
    {
        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            long num = 0;
            bool flag = false;
            foreach (int num2 in source)
            {
                if (flag)
                {
                    if (num2 > num)
                    {
                        num = num2;
                    }
                }
                else
                {
                    num = num2;
                    flag = true;
                }
            }
            //if (!flag)
            //{
            //    throw new ArgumentException("source");
            //}
            return num;
        }
        public static IList<long> Take(this IList<long> source,int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            
            if(count>=source.Count)
                return source;

            IList<long> list=new List<long>();                 
            for (int i=0 ;i<count;i++)
            {
                list.Add(source[i]);
            }          
            return list;
        }
    }
}
