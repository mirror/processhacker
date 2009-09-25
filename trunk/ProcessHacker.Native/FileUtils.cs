/*
 * Process Hacker - 
 *   file-related utility functions
 *
 * Copyright (C) 2009 wj32
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Provides utility methods for managing files.
    /// </summary>
    public static class FileUtils
    {
        static FileUtils()
        {
            RefreshFileNamePrefixes();
        }

        /// <summary>
        /// Used to resolve device prefixes (\Device\Harddisk1) into DOS drive names.
        /// </summary>
        private static Dictionary<string, string> _fileNamePrefixes = new Dictionary<string, string>();

        public static Icon GetFileIcon(string fileName)
        {
            return GetFileIcon(fileName, false);
        }

        public static Icon GetFileIcon(string fileName, bool large)
        {
            ShFileInfo shinfo = new ShFileInfo();

            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name cannot be empty.");

            try
            {
                if (Win32.SHGetFileInfo(fileName, 0, out shinfo,
                      (uint)Marshal.SizeOf(shinfo),
                       Win32.ShgFiIcon |
                       (large ? Win32.ShgFiLargeIcon : Win32.ShgFiSmallIcon)) == 0)
                {
                    return null;
                }
                else
                {
                    return Icon.FromHandle(shinfo.hIcon);
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GetFileName(string fileName)
        {
            return GetFileName(fileName, false);
        }

        public static string GetFileName(string fileName, bool canonicalize)
        {
            bool alreadyCanonicalized = false;

            // If the path starts with "\SystemRoot", we can replace it with C:\ (or whatever it is).
            if (fileName.ToLower().StartsWith("\\systemroot"))
            {
                fileName = System.IO.Path.GetFullPath(Environment.SystemDirectory + "\\.." + fileName.Substring(11));
                alreadyCanonicalized = true;
            }
            // If the path starts with "\??\", we can remove it and we will have the path.
            else if (fileName.StartsWith("\\??\\"))
            {
                fileName = fileName.Substring(4);
            }

            // If the path still starts with a backslash, we probably need to 
            // resolve any native object name to a DOS drive letter.
            if (fileName.StartsWith("\\"))
            {
                var prefixes = _fileNamePrefixes;

                foreach (var pair in prefixes)
                {
                    if (fileName.StartsWith(pair.Key + "\\"))
                    {
                        fileName = pair.Value + "\\" + fileName.Substring(pair.Key.Length + 1);
                        break;
                    }
                    else if (fileName == pair.Key)
                    {
                        fileName = pair.Value;
                        break;
                    }
                }
            }

            if (canonicalize && !alreadyCanonicalized)
                fileName = System.IO.Path.GetFullPath(fileName);

            return fileName;
        }

        public static void RefreshFileNamePrefixes()
        {
            // Just create a new dictionary to avoid having to lock the existing one.
            var newPrefixes = new Dictionary<string, string>();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                using (var data = new MemoryAlloc(1024))
                {
                    int length;

                    if ((length = Win32.QueryDosDevice(c.ToString() + ":", data, data.Size / 2)) > 2)
                    {
                        newPrefixes.Add(data.ReadUnicodeString(0, length - 2), c.ToString() + ":");
                    }
                }
            }

            _fileNamePrefixes = newPrefixes;
        }

        public static void ShowProperties(string fileName)
        {
            var info = new ShellExecuteInfo();

            info.cbSize = Marshal.SizeOf(info);
            info.lpFile = fileName;
            info.nShow = ShowWindowType.Show;
            info.fMask = Win32.SeeMaskInvokeIdList;
            info.lpVerb = "properties";

            Win32.ShellExecuteEx(ref info);
        }
    }
}
