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
using System.Diagnostics;
using System.Windows.Forms;

namespace ProcessHacker
{
    public static class Misc
    {
        public static string[] DangerousNames = { "csrss.exe", "dwm.exe", "lsass.exe", "lsm.exe", "services.exe",
                                      "smss.exe", "wininit.exe", "winlogon.exe" };

        public static string[] KernelNames = { "ntoskrnl.exe", "ntkrnlpa.exe", "ntkrnlmp.exe", "ntkrpamp.exe" };

        /// <summary>
        /// Disables the menu items contained in the specified menu. 
        /// </summary>
        /// <param name="menu">The menu.</param>
        public static void DisableAllMenuItems(Menu menu)
        {
            foreach (MenuItem item in menu.MenuItems)
                item.Enabled = false;
        }

        /// <summary>
        /// Enables the menu items contained in the specified menu. 
        /// </summary>
        /// <param name="menu">The menu.</param>
        public static void EnableAllMenuItems(Menu menu)
        {
            foreach (MenuItem item in menu.MenuItems)
                item.Enabled = true;
        }

        /// <summary>
        /// Formats a <see cref="DateTime"/> object into a string representation using the format "dd/MM/yy hh:mm:ss".
        /// </summary>
        /// <param name="time">The <see cref="DateTime"/> object to format.</param>
        /// <returns></returns>
        public static string GetNiceDateTime(DateTime time)
        {         
            return time.ToString("dd/MM/yy hh:mm:ss");
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> object into a string representation.
        /// </summary>
        /// <param name="time">The <see cref="TimeSpan"/> to format.</param>
        /// <returns></returns>
        public static string GetNiceTimeSpan(TimeSpan time)
        {
            return String.Format("{0:d3}:{1:d2}:{2:d3}",
                                time.Minutes,
                                time.Seconds,
                                time.Milliseconds);
        }

        /// <summary>
        /// Parses a path string and returns the actual path name, removing \SystemRoot and \??\.
        /// </summary>
        /// <param name="path">The path to parse.</param>
        /// <returns></returns>
        public static string GetRealPath(string path)
        {
            if (path.ToLower().StartsWith("\\systemroot"))
                return Environment.SystemDirectory + "\\.." + path.Substring(11);
            else if (path.StartsWith("\\??\\"))
                return path.Substring(4);
            else
                return path;
        }
        
        /// <summary>
        /// Returns a <see cref="ProcessThread"/> object of the specified thread ID.
        /// </summary>
        /// <param name="p">The process which the thread belongs to.</param>
        /// <param name="id">The ID of the thread.</param>
        /// <returns></returns>
        public static ProcessThread GetThreadById(Process p, int id)
        {
            foreach (ProcessThread t in p.Threads)
                if (t.Id == id)
                    return t;

            return null;
        }

        /// <summary>
        /// Makes a character printable by converting unprintable characters to a dot ('.').
        /// </summary>
        /// <param name="c">The character to convert.</param>
        /// <returns></returns>
        public static char MakePrintableChar(char c)
        {
            if (c >= ' ' && c <= '~')
                return c;
            else
                return '.';
        }

        /// <summary>
        /// Makes a string printable by converting unprintable characters to a dot ('.').
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns></returns>
        public static string MakePrintable(string s)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
                sb.Append(MakePrintableChar(s[i]));

            return sb.ToString();
        }
    }
}
