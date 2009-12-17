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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Provides methods for manipulating various types of data.
    /// </summary>
    public static class Utils
    {
        public enum Endianness
        {
            Little, Big
        }

        #region Constants

        public const string MsgFailedToWaitIndefinitely =
            "Failed to wait indefinitely on an object.";

        public const int OneStackSize = 1024 * 1024;
        public const int HalfStackSize = OneStackSize / 2;
        public const int QuarterStackSize = HalfStackSize / 2;
        public const int EighthStackSize = QuarterStackSize / 2;
        public const int SixteenthStackSize = EighthStackSize / 2;

        public static int[] Primes = 
        {
            3, 7, 11, 0x11, 0x17, 0x1d, 0x25, 0x2f, 0x3b, 0x47, 0x59, 0x6b, 0x83, 0xa3, 0xc5, 0xef, 
            0x125, 0x161, 0x1af, 0x209, 0x277, 0x2f9, 0x397, 0x44f, 0x52f, 0x63d, 0x78b, 0x91d, 0xaf1,
            0xd2b, 0xfd1, 0x12fd, 0x16cf, 0x1b65, 0x20e3, 0x2777, 0x2f6f, 0x38ff, 0x446f, 0x521f, 0x628d,
            0x7655, 0x8e01, 0xaa6b, 0xcc89, 0xf583, 0x126a7, 0x1619b, 0x1a857, 0x1fd3b, 0x26315, 0x2dd67,
            0x3701b, 0x42023, 0x4f361, 0x5f0ed, 0x72125, 0x88e31, 0xa443b, 0xc51eb, 0xec8c1, 0x11bdbf,
            0x154a3f, 0x198c4f, 0x1ea867, 0x24ca19, 0x2c25c1, 0x34fa1b, 0x3f928f, 0x4c4987, 0x5b8b6f, 0x6dda89
        };

        public static string[] SizeUnitNames = { "B", "kB", "MB", "GB", "TB", "PB", "EB" };

        #endregion

        /// <summary>
        /// The maximum unit specifier to use when formatting sizes.
        /// </summary>
        public static int UnitSpecifier = 4;

        private static PropertyInfo _doubleBufferedProperty;

        /// <summary>
        /// Aligns a number to the specified power-of-two alignment value.
        /// </summary>
        /// <param name="value">The number to align.</param>
        /// <param name="alignment">A power-of-two alignment value.</param>
        /// <returns>
        /// The nearest multiple of the alignment greater than or equal to the number.
        /// </returns>
        public static int Align(int value, int alignment)
        {
            return (value + alignment + 1) & ~(alignment - 1);
        }

        public static void Break(string logMessage)
        {
            System.Diagnostics.Debugger.Log(0, "Error", logMessage);
            System.Diagnostics.Debugger.Break();
        }

        /// <summary>
        /// Flattens an array of arrays into a single array.
        /// </summary>
        /// <typeparam name="T">The type of each element in the arrays.</typeparam>
        /// <param name="ap">
        /// An array of arrays. If an array in the array is null, it will be ignored.
        /// </param>
        /// <returns>An array containing elements from each array.</returns>
        public static T[] Concat<T>(params T[][] ap)
        {
            int tl = 0;

            foreach (var array in ap)
                if (array != null)
                    tl += array.Length;

            T[] na = new T[tl];
            int i = 0;

            foreach (var array in ap)
            {
                if (array != null)
                {
                    Array.Copy(array, 0, na, i, array.Length);
                    i += array.Length;
                }
            }

            return na;
        }

        /// <summary>
        /// Determines whether the specified value is contained 
        /// within an array.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if the array contains the value, otherwise false.</returns>
        public static bool Contains<T>(this T[] array, T value)
        {
            return Array.IndexOf<T>(array, value) != -1;
        }

        /// <summary>
        /// Counts the number of bits in the specified number.
        /// </summary>
        /// <param name="value">The number to process.</param>
        /// <returns>The number of bits in the specified number.</returns>
        public static int CountBits(this int value)
        {
            int count = 0;

            while (value != 0)
            {
                count++;
                value &= value - 1;
            }

            return count;
        }

        /// <summary>
        /// Counts the number of bits in the specified number.
        /// </summary>
        /// <param name="value">The number to process.</param>
        /// <returns>The number of bits in the specified number.</returns>
        public static int CountBits(this long value)
        {
            int count = 0;

            while (value != 0)
            {
                count++;
                value &= value - 1;
            }

            return count;
        }

        /// <summary>
        /// Creates an array of bytes from the specified byte pointer.
        /// </summary>
        /// <param name="ptr">A pointer to an array of bytes.</param>
        /// <param name="length">The length of the array.</param>
        /// <returns>A new byte array.</returns>
        public unsafe static byte[] Create(byte* ptr, int length)
        {
            byte[] array = new byte[length];

            for (int i = 0; i < length; i++)
                array[i] = ptr[i];

            return array;
        }

        /// <summary>
        /// Adds an ellipsis to a string if it is longer than the specified length.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="len">The maximum length.</param>
        /// <returns>The modified string.</returns>
        public static string CreateEllipsis(string s, int len)
        {
            if (s.Length <= len)
                return s;
            else
                return s.Substring(0, len - 4) + " ...";
        }

        /// <summary>
        /// Creates a string containing random uppercase characters.
        /// </summary>
        /// <param name="length">The number of characters to generate.</param>
        /// <returns>The generated string.</returns>
        public static string CreateRandomString(int length)
        {
            Random r = new Random((int)(DateTime.Now.ToFileTime() & 0xffffffff));
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append((char)('A' + r.Next(25)));

            return sb.ToString();
        }

        /// <summary>
        /// Clears and cleans up resources held by the menu items.
        /// </summary>
        public static void DisposeAndClear(this Menu.MenuItemCollection items)
        {
            //foreach (MenuItem item in items)
            //{
            //    item.Dispose();
            //}

            items.Clear();
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
        /// Disables all menu items. 
        /// </summary>
        public static void DisableAll(this Menu menu)
        {
            DisableAllMenuItems(menu);
        }

        /// <summary>
        /// Performs a divide operation, rounding up.
        /// </summary>
        /// <param name="dividend">
        /// The positive number to divide. The result is undefined if the dividend 
        /// is negative or zero.
        /// </param>
        /// <param name="divisor">
        /// The positive number to divide by. The result is undefined if the divisor 
        /// is negative or zero.
        /// </param>
        /// <returns>A rounded-up quotient.</returns>
        public static int DivideUp(int dividend, int divisor)
        {
            return (dividend - 1) / divisor + 1;
        }

        /// <summary>
        /// Performs an action on a control after its handle has been created. 
        /// If the control's handle has already been created, the action is 
        /// executed immediately.
        /// </summary>
        /// <param name="control">The control is execute the action on.</param>
        /// <param name="action">The action to execute.</param>
        public static void DoDelayed(this Control control, Action<Control> action)
        {
            if (control.IsHandleCreated)
            {
                action(control);
            }
            else
            {
                LayoutEventHandler handler = null;

                handler = (sender, e) =>
                {
                    if (control.IsHandleCreated)
                    {
                        control.Layout -= handler;
                        action(control);
                    }
                };

                control.Layout += handler;
            }
        }

        /// <summary>
        /// Duplicates the specified array.
        /// </summary>
        /// <typeparam name="T">The type of array to duplicate.</typeparam>
        /// <param name="array">The array to duplicate.</param>
        /// <returns>A copy of the specified array.</returns>
        public static T[] Duplicate<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];

            array.CopyTo(newArray, 0);

            return newArray;
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
        /// Enables all menu items. 
        /// </summary>
        public static void EnableAll(this Menu menu)
        {
            EnableAllMenuItems(menu);
        }

        /// <summary>
        /// Compares two arrays and determines whether they are equal.
        /// </summary>
        /// <typeparam name="T">The type of each element in the arrays.</typeparam>
        /// <param name="array">The first array.</param>
        /// <param name="other">The second array.</param>
        /// <returns>Whether the two arrays are considered to be equal.</returns>
        public static bool Equals<T>(this T[] array, T[] other)
        {
            return Equals(array, other, 0);
        }

        /// <summary>
        /// Compares two arrays and determines whether they are equal.
        /// </summary>
        /// <typeparam name="T">The type of each element in the arrays.</typeparam>
        /// <param name="array">The first array.</param>
        /// <param name="other">The second array.</param>
        /// <param name="startIndex">The index from which to begin comparing.</param>
        /// <returns>Whether the two arrays are considered to be equal.</returns>
        public static bool Equals<T>(this T[] array, T[] other, int startIndex)
        {
            return Equals(array, other, startIndex, array.Length);
        }

        /// <summary>
        /// Compares two arrays and determines whether they are equal.
        /// </summary>
        /// <typeparam name="T">The type of each element in the arrays.</typeparam>
        /// <param name="array">The first array.</param>
        /// <param name="other">The second array.</param>
        /// <param name="startIndex">The index from which to begin comparing.</param>
        /// <param name="length">The number of elements to compare.</param>
        /// <returns>Whether the two arrays are considered to be equal.</returns>
        public static bool Equals<T>(this T[] array, T[] other, int startIndex, int length)
        {
            for (int i = startIndex; i < startIndex + length; i++)
                if (!array[i].Equals(other[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Escapes a string using C-style escaping.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        public static string Escape(this string str)
        {
            str = str.Replace("\\", "\\\\");
            str = str.Replace("\"", "\\\"");

            return str;
        }

        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }

        /// <summary>
        /// Fills a combobox with enum value names.
        /// </summary>
        /// <param name="box">The combobox to modify.</param>
        /// <param name="t">The type of the enum.</param>
        public static void Fill(this ComboBox box, Type t)
        {
            foreach (string s in Enum.GetNames(t))
                box.Items.Add(s);
        }

        /// <summary>
        /// Moves the specified rectangle to fit inside the working area 
        /// of the display containing the specified control.
        /// </summary>
        /// <param name="rect">The rectangle to process.</param>
        /// <param name="c">The control from which to get the display.</param>
        /// <returns>A new rectangle with its location modified.</returns>
        public static Rectangle FitRectangle(Rectangle rect, Control c)
        {
            return FitRectangle(rect, Screen.GetWorkingArea(c));
        }

        /// <summary>
        /// Moves the specified rectangle to fit inside the specified bounds.
        /// </summary>
        /// <param name="rect">The rectangle to process.</param>
        /// <param name="bounds">The bounds in which the rectangle should be.</param>
        /// <returns>A new rectangle with its location modified.</returns>
        public static Rectangle FitRectangle(Rectangle rect, Rectangle bounds)
        {
            if (rect.X < bounds.Left)
                rect.X = bounds.Left;
            if (rect.Y < bounds.Top)
                rect.Y = bounds.Top;
            if (rect.X + rect.Width > bounds.Width)
                rect.X = bounds.Width - rect.Width;
            if (rect.Y + rect.Height > bounds.Height)
                rect.Y = bounds.Height - rect.Height;

            return rect;
        }

        /// <summary>
        /// Gets a string representation for an address.
        /// </summary>
        /// <param name="address">An address.</param>
        /// <returns>A string representation of the specified address.</returns>
        public static string FormatAddress(int address)
        {
            return "0x" + address.ToString("x");
        }

        /// <summary>
        /// Gets a string representation for an address.
        /// </summary>
        /// <param name="address">An address.</param>
        /// <returns>A string representation of the specified address.</returns>
        public static string FormatAddress(uint address)
        {
            return "0x" + address.ToString("x");
        }

        /// <summary>
        /// Gets a string representation for an address.
        /// </summary>
        /// <param name="address">An address.</param>
        /// <returns>A string representation of the specified address.</returns>
        public static string FormatAddress(long address)
        {
            return "0x" + address.ToString("x");
        }

        /// <summary>
        /// Gets a string representation for an address.
        /// </summary>
        /// <param name="address">An address.</param>
        /// <returns>A string representation of the specified address.</returns>
        public static string FormatAddress(ulong address)
        {
            return "0x" + address.ToString("x");
        }

        /// <summary>
        /// Gets a string representation for an address.
        /// </summary>
        /// <param name="address">An address.</param>
        /// <returns>A string representation of the specified address.</returns>
        public static string FormatAddress(IntPtr address)
        {
            return "0x" + address.ToString("x");
        }

        public static string FormatFlags(Type e, long value)
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

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> object into a string representation.
        /// </summary>
        /// <param name="time">The <see cref="TimeSpan"/> to format.</param>
        /// <returns></returns>
        public static string FormatLongTimeSpan(TimeSpan time)
        {
            return String.Format(
                "{0}{1:d2}:{2:d2}:{3:d2}",
                time.Days != 0 ? (time.Days.ToString() + ".") : "",
                time.Hours,
                time.Minutes,
                time.Seconds
                );
        }

        /// <summary>
        /// Gets the relative time in nice English.
        /// </summary>
        /// <param name="time">A DateTime.</param>
        /// <returns>A string.</returns>
        public static string FormatRelativeDateTime(DateTime time)
        {
            // Get the time span from the time to now.
            TimeSpan span = DateTime.Now.Subtract(time);
            // The partial number of weeks.
            double weeks = span.TotalDays / 7;
            // The partial number of fortnights.
            double fortnights = weeks / 2;
            // ...
            double months = span.TotalDays * 12 / 365;
            double years = months / 12;
            double centuries = years / 100;
            string str = "";

            // Start from the most general time unit and see if they can be used 
            // without any fractional component.
            // x centur(y|ies)
            if (centuries >= 1)
                str = (int)centuries + " " + ((int)centuries == 1 ? "century" : "centuries");
            // x year(s)
            else if (years >= 1)
                str = (int)years + " " + ((int)years == 1 ? "year" : "years");
            // x month(s)
            else if (months >= 1)
                str = (int)months + " " + ((int)months == 1 ? "month" : "months");
            // x fortnight(s)
            else if (fortnights >= 1)
                str = (int)fortnights + " " + ((int)fortnights == 1 ? "fortnight" : "fortnights");
            // x week(s)
            else if (weeks >= 1)
                str = (int)weeks + " " + ((int)weeks == 1 ? "week" : "weeks");
            // x day(s) (and y hour(s))
            else if (span.TotalDays >= 1)
            {
                str = (int)span.TotalDays + " " + ((int)span.TotalDays == 1 ? "day" : "days");

                if (span.Hours >= 1)
                    str += " and " + span.Hours + " " +
                        (span.Hours == 1 ? "hour" : "hours");
            }
            // x hour(s) (and y minute(s))
            else if (span.Hours >= 1)
            {
                str = span.Hours + " " + (span.Hours == 1 ? "hour" : "hours");

                if (span.Minutes >= 1)
                    str += " and " + span.Minutes + " " +
                        (span.Minutes == 1 ? "minute" : "minutes");
            }
            // x minute(s) (and y second(s))
            else if (span.Minutes >= 1)
            {
                str = span.Minutes + " " + (span.Minutes == 1 ? "minute" : "minutes");

                if (span.Seconds >= 1)
                    str += " and " + span.Seconds + " " +
                        (span.Seconds == 1 ? "second" : "seconds");
            }
            // x second(s)
            else if (span.Seconds >= 1)
                str = span.Seconds + " " + (span.Seconds == 1 ? "second" : "seconds");
            // x millisecond(s)
            else if (span.Milliseconds >= 1)
                str = span.Milliseconds + " " + (span.Milliseconds == 1 ? "millisecond" : "milliseconds");
            else
                str = "a very short time";

            // Turn 1 into "a", e.g. 1 minute -> a minute
            if (str.StartsWith("1 "))
            {
                // Special vowel case: a hour -> an hour
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
        public static string FormatSize(int size)
        {
            return FormatSize((uint)size);
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        public static string FormatSize(uint size)
        {
            int i = 0;
            double s = (double)size;

            while (s > 1024 && i < SizeUnitNames.Length && i < UnitSpecifier)
            {
                s /= 1024;
                i++;
            }

            return (s == 0 ? "0" : s.ToString("#,#.##")) + " " + SizeUnitNames[i];
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        public static string FormatSize(IntPtr size)
        {
            unchecked
            {
                return FormatSize((ulong)size.ToInt64());
            }
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        public static string FormatSize(long size)
        {
            return FormatSize((ulong)size);
        }

        /// <summary>
        /// Formats a size into a string representation, postfixing it with the correct unit.
        /// </summary>
        /// <param name="size">The size to format.</param>
        public static string FormatSize(ulong size)
        {
            int i = 0;
            double s = (double)size;

            while (s > 1024 && i < SizeUnitNames.Length && i < UnitSpecifier)
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
        public static string FormatTimeSpan(TimeSpan time)
        {
            return String.Format("{0:d2}:{1:d2}:{2:d2}.{3:d3}",
                time.Hours,
                time.Minutes,
                time.Seconds,
                time.Milliseconds);
        }

        // <summary>
        // Gets a System.DateTime indicating the time the specified assembly was last built. 
        // This will attempt to calculate the time from the build number, if possible. 
        // Otherwise, the last write time of the assembly will be used.
        // </summary>
        // <param name="assembly">The assembly to get the build date for.</param>
        // <param name="forceFileDate">True to always use the last write time of the assembly, otherwise false.</param>
        // <returns>The time this assembly was built.</returns>
        public static DateTime GetAssemblyBuildDate(Assembly assembly, bool forceFileDate)
        {
            Version AssemblyVersion = assembly.GetName().Version;
            DateTime dt;

            if (forceFileDate)
            {
                dt = GetAssemblyLastWriteTime(assembly);
            }
            else
            {
                dt = DateTime.Parse("01/01/2000").AddDays(AssemblyVersion.Build).AddSeconds(AssemblyVersion.Revision * 2);
                if (TimeZone.IsDaylightSavingTime(dt, TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year)))
                {
                    dt = dt.AddHours(1);
                }
                if (dt > DateTime.Now || AssemblyVersion.Build < 730 || AssemblyVersion.Revision == 0)
                {
                    dt = GetAssemblyLastWriteTime(assembly);
                }
            }

            return dt;
        }

        // <summary>
        // Returns the last write time of the specified assembly.
        // </summary>
        // <returns>The last write time of the assembly, or DateTime.MaxValue if an exception occurred.</returns>
        public static DateTime GetAssemblyLastWriteTime(Assembly assembly)
        {
            if (assembly.Location == null || assembly.Location == "")
                return DateTime.MaxValue;

            try
            {
                return File.GetLastWriteTime(assembly.Location);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }

        public static byte[] GetBytes(this int n)
        {
            return n.GetBytes(Endianness.Little);
        }

        public static byte[] GetBytes(this int n, Endianness type)
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

        public static byte[] GetBytes(this uint n)
        {
            return n.GetBytes(Endianness.Little);
        }

        public static byte[] GetBytes(this uint n, Endianness type)
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

        public static byte[] GetBytes(this ushort n)
        {
            return n.GetBytes(Endianness.Little);
        }

        public static byte[] GetBytes(this ushort n, Endianness type)
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

        /// <summary>
        /// Converts a 32-bit Unix time value into a DateTime object.
        /// </summary>
        /// <param name="time">The Unix time value.</param>
        public static DateTime GetDateTimeFromUnixTime(uint time)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).Add(new TimeSpan(0, 0, 0, (int)time));
        }

        public static int GetPrime(int minimum)
        {
            if (minimum < 0)
                throw new ArgumentOutOfRangeException("minimum");

            for (int i = 0; i < Primes.Length; i++)
            {
                if (Primes[i] >= minimum)
                    return Primes[i];
            }

            for (int i = minimum | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i))
                    return i;
            }

            return minimum;
        }

        /// <summary>
        /// Parses a string and produces a rectangle.
        /// </summary>
        /// <param name="s">
        /// A string describing a rectangle in the following format: 
        /// x,y,width,height (with no spaces).
        /// </param>
        /// <returns>A rectangle.</returns>
        public static Rectangle GetRectangle(string s)
        {
            var split = s.Split(',');

            return new Rectangle(int.Parse(split[0]), int.Parse(split[1]),
                int.Parse(split[2]), int.Parse(split[3]));
        }

        /// <summary>
        /// Returns a <see cref="System.Diagnostics.ProcessThread"/> object of the specified thread ID.
        /// </summary>
        /// <param name="p">The process which the thread belongs to.</param>
        /// <param name="id">The ID of the thread.</param>
        /// <returns></returns>
        public static System.Diagnostics.ProcessThread GetThreadFromId(System.Diagnostics.Process p, int id)
        {
            foreach (System.Diagnostics.ProcessThread t in p.Threads)
                if (t.Id == id)
                    return t;

            return null;
        }

        /// <summary>
        /// Determines whether the array is empty (all 0's).
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <returns>True if the array is empty; otherwise false.</returns>
        public static bool IsEmpty(this byte[] array)
        {
            foreach (byte b in array)
            {
                if (b != 0)
                    return false;
            }

            return true;
        }

        public static bool IsPrime(this int number)
        {
            int x;

            // Is the number even?
            if ((number & 1) == 0)
                return number == 2;

            x = (int)Math.Sqrt(number);

            for (int i = 3; i <= x; i += 2)
            {
                if ((number % i) == 0)
                    return false;
            }

            return true;
        }

        public static string JoinCommandLine(Dictionary<string, string> args)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in args)
            {
                if (string.IsNullOrEmpty(kvp.Value))
                {
                    sb.Append(kvp.Key + " ");
                }
                else
                {
                    sb.Append(kvp.Key + " \"" + kvp.Value + "\" ");
                }
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// Makes a character printable by converting unprintable characters to a dot ('.').
        /// </summary>
        /// <param name="c">The character to convert.</param>
        /// <returns></returns>
        public static char MakePrintable(char c)
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
                sb.Append(MakePrintable(s[i]));

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether a string matches according to a wildcard expression.
        /// </summary>
        /// <param name="pattern">The wildcard expression.</param>
        /// <param name="text">The string to match.</param>
        /// <returns>Whether the string matches.</returns>
        public static bool MatchWildcards(string pattern, string text)
        {
            return MatchWildcards(pattern, 0, text, 0);
        }

        private static bool MatchWildcards(string pattern, int patternStart, string text, int textStart)
        {
            // Note: this algorithm is currently recursive for easy understanding. 
            // It should be re-implemented without recursion...

            int patternIndex = patternStart;
            int textIndex = textStart;

            // If we have a zero-length pattern, the string matches.
            if (pattern.Length == 0 || patternIndex >= pattern.Length)
                return true;
            // If we have a zero-length string, the string doesn't match.
            if (text.Length == 0 || textIndex >= text.Length)
                return false;

            // Match up to the first asterisk (or maybe a number of them).

            while (true)
            {
                // Did we reach the end of the pattern? If so, check if we 
                // have also reached the end of the text.
                if (patternIndex >= pattern.Length)
                    return textIndex >= text.Length;

                if (pattern[patternIndex] == '*')
                {
                    patternIndex++;

                    // Skip duplicate asterisks.
                    while (patternIndex < pattern.Length)
                    {
                        if (pattern[patternIndex] != '*')
                            break;

                        patternIndex++;
                    }

                    break;
                }

                // Did we reach the end of the text? If so, the match fails.
                if (textIndex >= text.Length)
                    return false;

                if (pattern[patternIndex] != text[textIndex] && pattern[patternIndex] != '?')
                    return false;

                patternIndex++;
                textIndex++;
            }

            // We reached an asterisk (otherwise we would have returned by now). 
            // Keep incrementing the text index until we get a match.

            // Shortcut: if we are at the end of the pattern, it means the pattern 
            // has trailing asterisk(s). The string matches.
            if (patternIndex >= pattern.Length)
                return true;

            while (textIndex < text.Length)
            {
                if (MatchWildcards(pattern, patternIndex, text, textIndex))
                    return true;

                textIndex++;
            }

            return false;
        }

        public static Dictionary<string, string> ParseCommandLine(string[] args)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string argPending = null;

            foreach (string s in args)
            {
                if (s.StartsWith("-"))
                {
                    if (dict.ContainsKey(s))
                        throw new ArgumentException("Option already specified.");

                    dict.Add(s, "");
                    argPending = s;
                }
                else
                {
                    if (argPending != null)
                    {
                        dict[argPending] = s;
                        argPending = null;
                    }
                    else
                    {
                        if (!dict.ContainsKey(""))
                            dict.Add("", s);
                    }
                }
            }

            return dict;
        }

        public static int ReadInt32(Stream s, Endianness type)
        {
            byte[] buffer = new byte[4];

            if (s.Read(buffer, 0, 4) == 0)
                throw new EndOfStreamException();

            return ToInt32(buffer, type);
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

        public static string ReadString(Stream s, int length)
        {
            byte[] buffer = new byte[length];

            if (s.Read(buffer, 0, length) == 0)
                throw new EndOfStreamException();

            return System.Text.Encoding.ASCII.GetString(buffer);
        }

        public static uint ReadUInt32(Stream s, Endianness type)
        {
            byte[] buffer = new byte[4];

            if (s.Read(buffer, 0, 4) == 0)
                throw new EndOfStreamException();

            return ToUInt32(buffer, type);
        }

        /// <summary>
        /// Reads a null-terminated Unicode string from a stream.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        /// <returns>The read string.</returns>
        public static string ReadUnicodeString(Stream s)
        {
            StringBuilder str = new StringBuilder();

            while (true)
            {
                int b = s.ReadByte();

                if (b == -1)
                    break;

                int b2 = s.ReadByte();

                if (b2 == -1)
                    break;

                if (b == 0 && b2 == 0)
                    break;

                str.Append(Encoding.Unicode.GetChars(new byte[] { (byte)b, (byte)b2 }));
            }

            return str.ToString();
        }

        /// <summary>
        /// Reads a Unicode string from a stream.
        /// </summary>
        /// <param name="s">The stream to read from.</param>
        /// <param name="length">The length, in bytes, of the string.</param>
        /// <returns>The read string.</returns>
        public static string ReadUnicodeString(Stream s, int length)
        {
            StringBuilder str = new StringBuilder();
            int i = 0;

            while (i < length)
            {
                int b = s.ReadByte();

                if (b == -1)
                    break;

                int b2 = s.ReadByte();

                if (b2 == -1)
                    break;

                str.Append(Encoding.Unicode.GetChars(new byte[] { (byte)b, (byte)b2 }));
                i += 2;
            }

            return str.ToString();
        }

        /// <summary>
        /// Swaps the order of the bytes.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static int Reverse(this int v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);
            byte b3 = (byte)(v >> 16);
            byte b4 = (byte)(v >> 24);

            return b4 | (b3 << 8) | (b2 << 16) | (b1 << 24);
        }

        /// <summary>
        /// Swaps the order of the bytes.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static uint Reverse(this uint v)
        {
            uint b0 = v & 0xff;
            uint b1 = (v >> 8) & 0xff;
            uint b2 = (v >> 16) & 0xff;
            uint b3 = (v >> 24) & 0xff;

            b0 <<= 24;
            b1 <<= 16;
            b2 <<= 8;

            return b0 | b1 | b2 | b3;
        }

        /// <summary>
        /// Swaps the order of the bytes.
        /// </summary>
        /// <param name="v">The number to change.</param>
        /// <returns>A number.</returns>
        public static ushort Reverse(this ushort v)
        {
            byte b1 = (byte)v;
            byte b2 = (byte)(v >> 8);

            return (ushort)(b2 | (b1 << 8));
        }

        /// <summary>
        /// Reverses an array.
        /// </summary>
        /// <param name="data">The array to reverse.</param>
        /// <returns>A new array.</returns>
        public static T[] Reverse<T>(this T[] data)
        {
            T[] newData = new T[data.Length];

            for (int i = 0; i < data.Length; i++)
                newData[i] = data[data.Length - i - 1];

            return newData;
        }

        public static int RoundUpTwo(this int value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;

            return value;
        }

        public static long RoundUpTwo(this long value)
        {
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            value++;

            return value;
        }

        /// <summary>
        /// Selects all of the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        public static void SelectAll(this ListView.ListViewItemCollection items)
        {
            foreach (ListViewItem item in items)
                item.Selected = true;
        }

        /// <summary>
        /// Selects all of the items in the specified ListView.
        /// </summary>
        /// <param name="items">The ListView to process.</param>
        public static void SelectAll(this ListView items)
        {
            if (items.VirtualMode)
            {
                for (int i = 0; i < items.VirtualListSize; i++)
                    if (!items.SelectedIndices.Contains(i))
                        items.SelectedIndices.Add(i);
            }
            else
            {
                SelectAll(items.Items);
            }
        }

        /// <summary>
        /// Enables or disables double buffering for a control.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="t">The type of the control.</param>
        /// <param name="value">The new setting.</param>
        public static void SetDoubleBuffered(this Control c, Type t, bool value)
        {
            PropertyInfo doubleBufferedProperty = _doubleBufferedProperty;

            if (doubleBufferedProperty == null)
            {
                _doubleBufferedProperty = doubleBufferedProperty = t.GetProperty("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            }

            doubleBufferedProperty.SetValue(c, value, null);
        }

        /// <summary>
        /// Enables or disables double buffering for a control.
        /// </summary>
        /// <param name="c">The control to set the property on.</param>
        /// <param name="value">The new value.</param>
        public static void SetDoubleBuffered(this Control c, bool value)
        {
            c.SetDoubleBuffered(c.GetType(), value);
        }

        /// <summary>
        /// Shows a file in Windows Explorer.
        /// </summary>
        /// <param name="fileName">The file to show.</param>
        public static void ShowFileInExplorer(string fileName)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + fileName);
        }

        /// <summary>
        /// Calculates the size of a structure.
        /// </summary>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <returns>The size of the structure.</returns>
        public static int SizeOf<T>()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        }

        /// <summary>
        /// Calculates the size of a structure.
        /// </summary>
        /// <typeparam name="T">The structure type.</typeparam>
        /// <param name="alignment">A power-of-two whole-structure alignment to apply.</param>
        /// <returns>The size of the structure.</returns>
        public static int SizeOf<T>(int alignment)
        {
            // HACK: This is wrong, but it works.
            return SizeOf<T>() + alignment;
        }

        /// <summary>
        /// Returns a sorted list of the names in a given enum type.
        /// </summary>
        /// <param name="enumType">The enum type to process.</param>
        /// <returns>A list of key-value pairs, sorted based on the number of bits in the value.</returns>
        public static List<KeyValuePair<string, long>> SortFlagNames(Type enumType)
        {
            List<KeyValuePair<string, long>> nameList = new List<KeyValuePair<string, long>>();

            foreach (string name in Enum.GetNames(enumType))
            {
                long nameLong = Convert.ToInt64(Enum.Parse(enumType, name));

                nameList.Add(new KeyValuePair<string, long>(name, nameLong));
            }

            nameList.Sort((kvp1, kvp2) => kvp2.Value.CountBits().CompareTo(kvp1.Value.CountBits()));

            return nameList;
        }

        public unsafe static void StrCpy(char* dest, string src, int maxChars)
        {
            for (int i = 0; i < src.Length && i < maxChars; i++)
            {
                dest[i] = src[i];
            }
        }

        public static Bitmap ToBitmap(IntPtr iconHandle, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawIcon(Icon.FromHandle(iconHandle), new Rectangle(0, 0, width, height));
            }

            return b;
        }

        public static Bitmap ToBitmap(this Icon icon, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawIcon(icon, new Rectangle(0, 0, width, height));
            }

            return b;
        }

        public static int ToInt32(this byte[] data)
        {
            return data.ToInt32(Endianness.Little);
        }

        public static int ToInt32(this byte[] data, Endianness type)
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

        public static long ToInt64(this byte[] data)
        {
            return data.ToInt64(Endianness.Little);
        }

        public static long ToInt64(this byte[] data, Endianness type)
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

        public static IntPtr ToIntPtr(this byte[] data)
        {
            if (IntPtr.Size != data.Length)
                throw new ArgumentException("data");

            if (IntPtr.Size == sizeof(int))
                return new IntPtr(data.ToInt32(Endianness.Little));
            else if (IntPtr.Size == sizeof(long))
                return new IntPtr(data.ToInt64(Endianness.Little));
            else
                throw new ArgumentException("data");
        }

        public static ushort ToUInt16(this byte[] data, Endianness type)
        {
            return ToUInt16(data, 0, type);
        }

        public static ushort ToUInt16(this byte[] data, int offset, Endianness type)
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

        public static uint ToUInt32(this byte[] data, Endianness type)
        {
            return ToUInt32(data, 0, type);
        }

        public static uint ToUInt32(this byte[] data, int offset, Endianness type)
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

        public static void ValidateBuffer(byte[] buffer, int offset, int length)
        {
            ValidateBuffer(buffer, offset, length, false);
        }

        public static void ValidateBuffer(byte[] buffer, int offset, int length, bool canBeNull)
        {
            // Make sure the offset isn't negative.
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            // Make sure the length isn't negative.
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            // Make sure we won't overrun the buffer.
            if (buffer != null)
            {
                if (buffer.Length - offset < length)
                    throw new ArgumentOutOfRangeException("The buffer is too small for the specified offset and length.");
            }
            else
            {
                if (!canBeNull)
                    throw new ArgumentException("The buffer cannot be null.");

                // We don't have a buffer, so make sure the offset and length are zero.
                if (offset != 0 || length != 0)
                    throw new ArgumentOutOfRangeException("The offset and length must be zero for a null buffer.");
            }
        }
    }
}
