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
            RefreshDriveDevicePrefixes();
        }

        /// <summary>
        /// Used to resolve device prefixes (\Device\Harddisk1) into DOS drive names.
        /// </summary>
        private static Dictionary<string, string> _driveDevicePrefixes = new Dictionary<string, string>();

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

        public static string FixPath(string path)
        {
            if (path.ToLower().StartsWith("\\systemroot"))
                return (new System.IO.FileInfo(Environment.SystemDirectory + "\\.." + path.Substring(11))).FullName;
            else if (path.StartsWith("\\??\\"))
                return path.Substring(4);
            else
                return path;
        }

        public static string DeviceFileNameToDos(string fileName)
        {
            // don't know if this is really necessary...
            var prefixes = _driveDevicePrefixes;

            foreach (var pair in prefixes)
            {
                if (fileName.StartsWith(pair.Key + "\\"))
                    return pair.Value + "\\" + fileName.Substring(pair.Key.Length + 1);
                else if (fileName == pair.Key)
                    return pair.Value;
            }

            return fileName;
        }

        public static void RefreshDriveDevicePrefixes()
        {
            // just create a new dictionary to avoid having to lock the existing one
            var newPrefixes = new Dictionary<string, string>();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                StringBuilder target = new StringBuilder(1024);

                if (Win32.QueryDosDevice(c.ToString() + ":", target, 1024) != 0)
                {
                    newPrefixes.Add(target.ToString(), c.ToString() + ":");
                }
            }

            _driveDevicePrefixes = newPrefixes;
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
