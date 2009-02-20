/*
 * Process Hacker - 
 *   misc. functions
 * 
 * Copyright (C) 2008-2009 wj32
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Aga.Controls.Tree;
using System.Collections.Generic;

namespace ProcessHacker
{
    public static class Misc
    {
        #region Constants

        public static string[] SizeUnitNames = { "B", "kB", "MB", "GB", "TB", "PB", "EB" };

        public static string[] DangerousNames = 
        {
            "csrss.exe", "dwm.exe", "lsass.exe", "lsm.exe", "services.exe",
            "smss.exe", "wininit.exe", "winlogon.exe"
        };

        public static string[] KernelNames = { "ntoskrnl.exe", "ntkrnlpa.exe", "ntkrnlmp.exe", "ntkrpamp.exe" };

        public static string[] PrivilegeNames =
        {
            "SeCreateTokenPrivilege",
            "SeAssignPrimaryTokenPrivilege",
            "SeLockMemoryPrivilege",
            "SeIncreaseQuotaPrivilege",
            "SeUnsolicitedInputPrivilege",
            "SeMachineAccountPrivilege",
            "SeTcbPrivilege",
            "SeSecurityPrivilege",
            "SeTakeOwnershipPrivilege",
            "SeLoadDriverPrivilege",
            "SeSystemProfilePrivilege",
            "SeSystemtimePrivilege",
            "SeProfileSingleProcessPrivilege",
            "SeIncreaseBasePriorityPrivilege",
            "SeCreatePagefilePrivilege",
            "SeCreatePermanentPrivilege",
            "SeBackupPrivilege",
            "SeRestorePrivilege",
            "SeShutdownPrivilege",
            "SeDebugPrivilege",
            "SeAuditPrivilege",
            "SeSystemEnvironmentPrivilege",
            "SeChangeNotifyPrivilege",
            "SeRemoteShutdownPrivilege",
            "SeUndockPrivilege",
            "SeSyncAgentPrivilege",
            "SeEnableDelegationPrivilege",
            "SeManageVolumePrivilege",
            "SeImpersonatePrivilege",
            "SeCreateGlobalPrivilege",
            "SeTrustedCredManAccessPrivilege",
            "SeRelabelPrivilege",
            "SeIncreaseWorkingSetPrivilege",
            "SeTimeZonePrivilege",
            "SeCreateSymbolicLinkPrivilege"
        };

        #endregion

        /// <summary>
        /// Swaps the order of the bytes in the argument.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static int ByteSwap(int v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);
            byte b3 = (byte)(v >> 16);
            byte b4 = (byte)(v >> 24);

            return b4 | (b3 << 8) | (b2 << 16) | (b1 << 24);
        }

        /// <summary>
        /// Swaps the order of the bytes in the argument.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static uint ByteSwap(uint v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);
            byte b3 = (byte)(v >> 16);
            byte b4 = (byte)(v >> 24);

            return (uint)(b4 | (b3 << 8) | (b2 << 16) | (b1 << 24));
        }

        /// <summary>
        /// Swaps the order of the bytes in the argument.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static ushort ByteSwap(ushort v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);

            return (ushort)(b2 | (b1 << 8));
        }

        /// <summary>
        /// Converts a 32-bit Unix time value into a DateTime object.
        /// </summary>
        /// <param name="time">The Unix time value.</param>
        public static DateTime DateTimeFromUnixTime(uint time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).Add(new TimeSpan(0, 0, 0, (int)time));
        }

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
        /// Escapes a string using C-style escaping.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        public static string EscapeString(string str)
        {
            str = str.Replace("\\", "\\\\");
            str = str.Replace("\"", "\\\"");

            return str;
        }

        /// <summary>
        /// Gets the base address of the currently running kernel.
        /// </summary>
        /// <returns>The kernel's base address.</returns>
        public static int GetKernelBase()
        {
            int RequiredSize = 0;
            int[] ImageBases;

            Win32.EnumDeviceDrivers(null, 0, out RequiredSize);
            ImageBases = new int[RequiredSize];
            Win32.EnumDeviceDrivers(ImageBases, RequiredSize * sizeof(int), out RequiredSize);

            for (int i = 0; i < RequiredSize; i++)
            {
                if (ImageBases[i] == 0)
                    continue;

                StringBuilder name = new StringBuilder(256);
                StringBuilder filename = new StringBuilder(256);
                string realname = "";

                Win32.GetDeviceDriverBaseName(ImageBases[i], name, 255);
                Win32.GetDeviceDriverFileName(ImageBases[i], filename, 255);

                try
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(Misc.GetRealPath(filename.ToString()));
                    bool kernel = false;

                    realname = fi.FullName;

                    foreach (string k in Misc.KernelNames)
                    {
                        if (realname.ToLower() == Environment.SystemDirectory.ToLower() + "\\" + k.ToLower())
                        {
                            kernel = true;

                            break;
                        }
                    }

                    if (kernel)
                        return ImageBases[i];
                }
                catch
                { }
            }

            return 0;
        }

        /// <summary>
        /// Gets the file name of the currently running kernel.
        /// </summary>
        /// <returns>The kernel file name.</returns>
        public static string GetKernelFileName()
        {
            int RequiredSize = 0;
            int[] ImageBases;

            Win32.EnumDeviceDrivers(null, 0, out RequiredSize);
            ImageBases = new int[RequiredSize];
            Win32.EnumDeviceDrivers(ImageBases, RequiredSize * sizeof(int), out RequiredSize);

            for (int i = 0; i < RequiredSize; i++)
            {
                if (ImageBases[i] == 0)
                    continue;

                StringBuilder name = new StringBuilder(256);
                StringBuilder filename = new StringBuilder(256);
                string realname = "";

                Win32.GetDeviceDriverBaseName(ImageBases[i], name, 255);
                Win32.GetDeviceDriverFileName(ImageBases[i], filename, 255);

                try
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(Misc.GetRealPath(filename.ToString()));
                    bool kernel = false;

                    realname = fi.FullName;

                    foreach (string k in Misc.KernelNames)
                    {
                        if (realname.ToLower() == Environment.SystemDirectory.ToLower() + "\\" + k.ToLower())
                        {
                            kernel = true;

                            break;
                        }
                    }

                    if (kernel)
                        return realname;
                }
                catch
                { }
            }

            return "";
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
        /// Gets the relative time in nice English.
        /// </summary>
        /// <param name="time">A DateTime.</param>
        /// <returns>A string.</returns>
        public static string GetNiceRelativeDateTime(DateTime time)
        {
            TimeSpan span = DateTime.Now.Subtract(time);
            double weeks = span.TotalDays / 7;
            double fortnights = weeks / 2;
            double months = span.TotalDays * 12 / 365;
            double years = months / 12;
            double centuries = years / 100;
            string str = "";

            if (centuries >= 1)
                str = (int)centuries + " " + ((int)centuries == 1 ? "century" : "centuries");
            else if (years >= 1)
                str = (int)years + " " + ((int)years == 1 ? "year" : "years");
            else if (months >= 1)
                str = (int)months + " " + ((int)months == 1 ? "month" : "months");
            else if (fortnights >= 1)
                str = (int)fortnights + " " + ((int)fortnights == 1 ? "fortnight" : "fortnights");
            else if (weeks >= 1)
                str = (int)weeks + " " + ((int)weeks == 1 ? "week" : "weeks");
            else if (span.TotalDays >= 1)
            {
                str = (int)span.TotalDays + " " + ((int)span.TotalDays == 1 ? "day" : "days");

                if (span.Hours >= 1)
                    str += " and " + span.Hours + " " +
                        (span.Hours == 1 ? "hour" : "hours");
            }
            else if (span.Hours >= 1)
            {
                str = span.Hours + " " + (span.Hours == 1 ? "hour" : "hours");

                if (span.Minutes >= 1)
                    str += " and " + span.Minutes + " " +
                        (span.Minutes == 1 ? "minute" : "minutes");
            }
            else if (span.Minutes >= 1)
            {
                str = span.Minutes + " " + (span.Minutes == 1 ? "minute" : "minutes");

                if (span.Seconds >= 1)
                    str += " and " + span.Seconds + " " +
                        (span.Seconds == 1 ? "second" : "seconds");
            }
            else if (span.Seconds >= 1)
                str = span.Seconds + " " + (span.Seconds == 1 ? "second" : "seconds");
            else if (span.Milliseconds >= 1)
                str = span.Milliseconds + " " + (span.Milliseconds == 1 ? "millisecond" : "milliseconds");
            else
                str = "a very short time";

            // 1 minute -> a minute
            if (str.StartsWith("1 "))
            {
                // a hour -> an hour
                if (str[2] != 'h')
                    str = "a " + str.Substring(2);
                else
                    str = "an " + str.Substring(2);
            }

            return str + " ago";
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        /// <returns></returns>
        public static string GetNiceSizeName(long size)
        {
            return GetNiceSizeName((ulong)size);
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        /// <returns></returns>
        public static string GetNiceSizeName(ulong size)
        {
            int i = 0;
            decimal s = (decimal)size;

            while (s > 1024 && i < SizeUnitNames.Length && i < Properties.Settings.Default.UnitSpecifier)
            {
                s /= 1024;
                i++;
            }

            return (s == 0 ? "0" : s.ToString("#,#.##")) + " " + SizeUnitNames[i];
        }

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> object into a string representation.
        /// </summary>
        /// <param name="time">The <see cref="TimeSpan"/> to format.</param>
        /// <returns></returns>
        public static string GetNiceTimeSpan(TimeSpan time)
        {
            return String.Format("{0:d2}:{1:d2}:{2:d2}.{3:d3}",
                time.Hours,
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
                return (new System.IO.FileInfo(Environment.SystemDirectory + "\\.." + path.Substring(11))).FullName;
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
        /// Determines whether the array is empty (all 0's).
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <returns>True if the array is empty; otherwise false.</returns>
        public static bool IsEmpty(byte[] array)
        {
            bool empty = true;

            foreach (byte b in array)
            {
                if (b != 0)
                {
                    empty = false;
                    break;
                }
            }

            return empty;
        }

        /// <summary>
        /// Adds an ellipsis to a string if it is longer than the specified length.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="len">The maximum length.</param>
        /// <returns>The modified string.</returns>
        public static string MakeEllipsis(string s, int len)
        {
            if (s.Length <= len)
                return s;
            else
                return s.Substring(0, len - 4) + " ...";
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

        /// <summary>
        /// Reads a null-terminated string from a stream.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        /// <returns>The read string.</returns>
        public static string ReadString(Stream s)
        {
            StringBuilder str = new StringBuilder();

            while (true)
            {
                int b = s.ReadByte();

                if (b == 0 || b == -1)
                    break;

                str.Append((char)(byte)b);
            }

            return str.ToString();
        }

        /// <summary>
        /// Selects all of the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public static void SelectAll(ListView.ListViewItemCollection items)
        {
            foreach (ListViewItem item in items)
                item.Selected = true;
        }

        /// <summary>
        /// Selects all of the specified nodes.
        /// </summary>
        /// <param name="items">The nodes.</param>
        public static void SelectAll(IEnumerable<TreeNodeAdv> nodes)
        {
            foreach (TreeNodeAdv node in nodes)
                node.IsSelected = true;
        }

        /// <summary>
        /// Enables or disables double buffering for a control.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="t">The type of the control.</param>
        /// <param name="value">The new setting.</param>
        public static void SetDoubleBuffered(Control c, Type t, bool value)
        {
            PropertyInfo property = t.GetProperty("DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance);

            property.SetValue(c, value, null);
        }

        /// <summary>
        /// Controls whether the UAC shield icon is displayed on the specified control handle.
        /// </summary>
        /// <param name="button">The button to modify.</param>
        /// <param name="show">Whether to show the UAC shield icon.</param>
        public static void SetShieldIcon(Button button, bool show)
        {
            Win32.SendMessage(button.Handle, 
                Win32.WindowMessage.BcmSetShield, IntPtr.Zero, show ? new IntPtr(1) : new IntPtr(0));
        }

        #region Stuff from PNG.Net

        public enum Endianness
        {
            Little, Big
        }

        public static bool ArrayContains<T>(T[] array, T element)
        {
            foreach (T e in array)
                if (e.Equals(element))
                    return true;

            return false;
        }

        public static bool BytesEqual(byte[] b1, byte[] b2)
        {
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;

            return true;
        }

        public static int BytesToInt(byte[] data, Endianness type)
        {
            if (type == Endianness.Little)
            {
                return (data[0]) | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
            }
            else if (type == Endianness.Big)
            {
                return (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | (data[3]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static long BytesToLong(byte[] data, Endianness type)
        {
            if (type == Endianness.Little)
            {
                return (data[0]) | (data[1] << 8) | (data[2] << 16) | (data[3] << 24) | 
                    (data[4] << 32) | (data[5] << 40) | (data[6] << 48) | (data[7] << 56);
            }
            else if (type == Endianness.Big)
            {
                return (data[0] << 56) | (data[1] << 48) | (data[2] << 40) | (data[3] << 32) |
                    (data[4] << 24) | (data[5] << 16) | (data[6] << 8) | (data[7]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static uint BytesToUInt(byte[] data, Endianness type)
        {
            return BytesToUInt(data, 0, type);
        }

        public static uint BytesToUInt(byte[] data, int offset, Endianness type)
        {
            if (type == Endianness.Little)
            {
                return (uint)(data[offset]) | (uint)(data[offset + 1] << 8) |
                    (uint)(data[offset + 2] << 16) | (uint)(data[offset + 3] << 24);
            }
            else if (type == Endianness.Big)
            {
                return (uint)(data[offset] << 24) | (uint)(data[offset + 1] << 16) |
                    (uint)(data[offset + 2] << 8) | (uint)(data[offset + 3]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static ushort BytesToUShort(byte[] data, Endianness type)
        {
            return BytesToUShort(data, 0, type);
        }

        public static ushort BytesToUShort(byte[] data, int offset, Endianness type)
        {
            if (type == Endianness.Little)
            {
                return (ushort)(data[offset] | (data[offset + 1] << 8));
            }
            else if (type == Endianness.Big)
            {
                return (ushort)((data[offset] << 8) | data[offset + 1]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static string FlagsToString(Type e, long value)
        {
            string r = "";

            for (int i = 0; i < 32; i++)
            {
                long fv = 1 << i;
                                  
                if ((value & fv) == fv)
                {
                    r += Enum.GetName(e, fv) + ", ";
                }
            }

            if (r.EndsWith(", "))
                r = r.Remove(r.Length - 2, 2);

            return r;
        }

        public static int IntCeilDiv(int a, int b)
        {
            return (int)Math.Ceiling(((double)a / b));
        }

        public static byte[] IntToBytes(int n, Endianness type)
        {
            byte[] data = new byte[4];

            if (type == Endianness.Little)
            {
                data[0] = (byte)(n & 0xff);
                data[1] = (byte)((n >> 8) & 0xff);
                data[2] = (byte)((n >> 16) & 0xff);
                data[3] = (byte)((n >> 24) & 0xff);
            }
            else if (type == Endianness.Big)
            {
                data[0] = (byte)((n >> 24) & 0xff);
                data[1] = (byte)((n >> 16) & 0xff);
                data[2] = (byte)((n >> 8) & 0xff);
                data[3] = (byte)(n & 0xff);
            }
            else
            {
                throw new ArgumentException();
            }

            return data;
        }

        public static byte[] ReverseBytes(byte[] data)
        {
            byte[] newdata = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
                newdata[i] = data[data.Length - i - 1];

            return newdata;
        }

        public static uint ReverseEndian(uint n)
        {
            uint b0 = n & 0xff;
            uint b1 = (n >> 8) & 0xff;
            uint b2 = (n >> 16) & 0xff;
            uint b3 = (n >> 24) & 0xff;

            b0 <<= 24;
            b1 <<= 16;
            b2 <<= 8;

            return b0 | b1 | b2 | b3;
        }

        public static int ReadInt(Stream s, Endianness type)
        {
            byte[] buffer = new byte[4];

            if (s.Read(buffer, 0, 4) == 0)
                throw new EndOfStreamException();

            return BytesToInt(buffer, type);
        }

        public static string ReadString(Stream s, int length)
        {
            byte[] buffer = new byte[length];

            if (s.Read(buffer, 0, length) == 0)
                throw new EndOfStreamException();

            return System.Text.ASCIIEncoding.ASCII.GetString(buffer);
        }

        public static uint ReadUInt(Stream s, Endianness type)
        {
            byte[] buffer = new byte[4];

            if (s.Read(buffer, 0, 4) == 0)
                throw new EndOfStreamException();

            return BytesToUInt(buffer, type);
        }

        public static uint RoundUpAddress(uint address, uint align)
        {
            uint t = (uint)Math.Ceiling((double)address / align);

            return t * align;
        }

        public static byte[] UIntToBytes(uint n, Endianness type)
        {
            byte[] data = new byte[4];

            if (type == Endianness.Little)
            {
                data[0] = (byte)(n & 0xff);
                data[1] = (byte)((n >> 8) & 0xff);
                data[2] = (byte)((n >> 16) & 0xff);
                data[3] = (byte)((n >> 24) & 0xff);
            }
            else if (type == Endianness.Big)
            {
                data[0] = (byte)((n >> 24) & 0xff);
                data[1] = (byte)((n >> 16) & 0xff);
                data[2] = (byte)((n >> 8) & 0xff);
                data[3] = (byte)(n & 0xff);
            }
            else
            {
                throw new ArgumentException();
            }

            return data;
        }

        public static byte[] UShortToBytes(ushort n, Endianness type)
        {
            byte[] data = new byte[2];

            if (type == Endianness.Little)
            {
                data[0] = (byte)(n & 0xff);
                data[1] = (byte)((n >> 8) & 0xff);
            }
            else if (type == Endianness.Big)
            {
                data[0] = (byte)((n >> 8) & 0xff);
                data[1] = (byte)(n & 0xff);
            }
            else
            {
                throw new ArgumentException();
            }

            return data;
        }

        #endregion
    }
}
