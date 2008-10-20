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
using System.Text;

namespace ProcessHacker
{
    public static class Misc
    {
        public static string GetNiceDateTime(DateTime time)
        {
            return time.ToString("dd/MM/yy hh:mm:ss");
        }

        public static string GetNiceSizeName(long size)
        {
            string[] names = { "B", "kB", "MB", "GB", "TB", "PB", "EB" };
            int i = 0;
            double s = (double)size;

            while (s > 1024 && i < names.Length)
            {
                s /= 1024;
                i++;
            }

            return String.Format("{0:f2}", s) + " " + names[i];
        }

        public static string GetNiceTimeSpan(TimeSpan time)
        {
            return String.Format("{0:d3}:{1:d2}:{2:d3}",
                                time.Minutes,
                                time.Seconds,
                                time.Milliseconds);
        }

        public static string GetRealPath(string path)
        {
            if (path.ToLower().StartsWith("\\systemroot"))
                return Environment.SystemDirectory + "\\.." + path.Substring(11);
            else if (path.StartsWith("\\??\\"))
                return path.Substring(4);
            else
                return path;
        }

        public static char MakePrintableChar(char c)
        {
            if (c >= ' ' && c <= '~')
                return c;
            else
                return '.';
        }

        public static string MakePrintable(string s)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
                sb.Append(MakePrintableChar(s[i]));

            return sb.ToString();
        }
    }
}
