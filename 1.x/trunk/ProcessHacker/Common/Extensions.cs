/*
 * Process Hacker - 
 *   long extensions
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
using System.Drawing;
using System.Windows.Forms;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Common
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }

    public static class LongExtensions
    {
        /// <summary>
        /// Gets the largest value in the specified array of longs.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <returns>The largest value in the specified array.</returns>
        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            long max = 0;
            bool afterFirst = false;

            foreach (long number in source)
            {
                if (afterFirst)
                {
                    if (number > max)
                        max = number;
                }
                else
                {
                    max = number;
                    afterFirst = true;
                }
            }

            return max;
        }

        /// <summary>
        /// Takes a number of elements from the specified array.
        /// </summary>
        /// <param name="source">The list to process.</param>
        /// <param name="count">The number of elements to take.</param>
        /// <returns>
        /// A new list containing the first <paramref name="count" /> 
        /// elements from the specified array.
        /// </returns>
        public static IList<long> Take(this IList<long> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // If we're trying to take more than we have, return the original set.
            if (count >= source.Count)
                return source;

            // Create a new list containing the elements.
            IList<long> newList = new List<long>();

            for (int i = 0; i < count; i++)
                newList.Add(source[i]);

            return newList;
        }
    }

    /// <summary>
    /// MeasureText cache.
    /// Cache the calls to MeasureText as this only needs to be called once.
    /// </summary>
    /// <remarks>Invalidate the cache if UI changes (i.e. DPI)</remarks>
    public static class TextSizeCache
    {
        private static readonly Dictionary<String, Size> SizeCache = new Dictionary<String, Size>();

        public static Size GetCachedSize(this Graphics device, string str, Font font)
        {
            if (SizeCache.ContainsKey(str))
            {
                return SizeCache[str];
            }

            // we only need to Measure the string once, so we cache it.
            Size strSize = TextRenderer.MeasureText(device, str, font);

            SizeCache.Add(str, strSize);

            return strSize;
        }

        public static void InvalidateCache()
        {
            SizeCache.Clear();
        }
    }

    public static class ControlExtensions
    {
        /// <summary>
        /// An application sends the WM_CHANGEUISTATE message to indicate that the UI state should be changed.
        /// </summary>
        /// <remarks>Value: 0x0127</remarks>
        public const int WM_CHANGEUISTATE = 295;

        /// <summary>
        /// An application sends the WM_UPDATEUISTATE message to change the UI state for the specified window and all its child windows.
        /// </summary>
        /// <remarks>Value: 0x0128</remarks>
        public const int WM_UPDATEUISTATE = 296;

        public enum UIS_Flags
        {
            /// <summary>
            /// The UI state flags specified by the high-order word should be set.
            /// </summary>
            UIS_SET = 1,

            /// <summary>
            /// The UI state flags specified by the high-order word should be cleared.
            /// </summary>
            UIS_CLEAR = 2,

            /// <summary>
            /// The UI state flags specified by the high-order word should be changed based on the last input event. For more information, see Remarks.
            /// </summary>
            UIS_INITIALIZE = 3
        }

        [Flags]
        public enum UISF_Flags
        {
            UISF_HIDEFOCUS = 0x1,
            UISF_HIDEACCEL = 0x2,
            UISF_ACTIVE = 0x4
        }

        public static void MakeAcceleratorsVisible(this Control c)
        {
            Win32.SendMessage(c.Handle, (WindowMessage)WM_CHANGEUISTATE, (IntPtr)MakeLong((int)UIS_Flags.UIS_CLEAR, (int)UISF_Flags.UISF_HIDEACCEL), IntPtr.Zero);
        }

        public static void MakeAcceleratorsInvisible(this Control c)
        {
            Win32.SendMessage(c.Handle, (WindowMessage)WM_CHANGEUISTATE, (IntPtr)MakeLong((int)UIS_Flags.UIS_SET, (int)UISF_Flags.UISF_HIDEACCEL), IntPtr.Zero);
        }

        public static void MakeFocusVisible(this Control c)
        {
            Win32.SendMessage(c.Handle, (WindowMessage)WM_CHANGEUISTATE, (IntPtr)MakeLong((int)UIS_Flags.UIS_CLEAR, (int)UISF_Flags.UISF_HIDEFOCUS), IntPtr.Zero);
        }

        public static void MakeFocusInvisible(this Control c)
        {
            Win32.SendMessage(c.Handle, (WindowMessage)WM_CHANGEUISTATE, (IntPtr)MakeLong((int)UIS_Flags.UIS_SET, (int)UISF_Flags.UISF_HIDEFOCUS), IntPtr.Zero);
        }

        public static void MakeActiveVisible(this Control c)
        {
            Win32.SendMessage(c.Handle, (WindowMessage)WM_CHANGEUISTATE, (IntPtr)MakeLong((int)UIS_Flags.UIS_SET, (int)UISF_Flags.UISF_ACTIVE), IntPtr.Zero);
        }

        public static void MakeActiveInvisible(this Control c)
        {
            Win32.SendMessage(c.Handle, (WindowMessage)WM_CHANGEUISTATE, (IntPtr)MakeLong((int)UIS_Flags.UIS_CLEAR, (int)UISF_Flags.UISF_ACTIVE), IntPtr.Zero);
        }

        private static int MakeLong(int loWord, int hiWord)
        {
            return (hiWord << 16) | (loWord & 0xffff);
        }

        public static int LoWord(int number)
        {
            return number & 0xffff;
        }

        //private int WM_CHANGEUISTATE = 0x127;

        //private enum WM_CHANGEUISTATE_low : short
        //{
        //    UIS_CLEAR = 2,
        //    UIS_INITIALIZE = 3,
        //    UIS_SET = 1
        //}

        //private enum WM_CHANGEUISTATE_high : short
        //{
        //    UISF_HIDEACCEL = 0x2,
        //    UISF_HIDEFOCUS = 0x1,
        //    UISF_ACTIVE = 4
        //}

        //private enum WM_CHANGEUISTATE : int
        //{
        //    UIS_CLEAR = WM_CHANGEUISTATE_low.UIS_CLEAR,
        //    UIS_INITIALIZE = WM_CHANGEUISTATE_low.UIS_INITIALIZE,
        //    UIS_SET = WM_CHANGEUISTATE_low.UIS_SET,
        //    UISF_HIDEACCEL = Convert.ToInt32(WM_CHANGEUISTATE_high.UISF_HIDEACCEL) << 16,
        //    UISF_HIDEFOCUS = Convert.ToInt32(WM_CHANGEUISTATE_high.UISF_HIDEFOCUS) << 16,
        //    UISF_ACTIVE = Convert.ToInt32(WM_CHANGEUISTATE_high.UISF_ACTIVE) << 16
        //}
    }
}
