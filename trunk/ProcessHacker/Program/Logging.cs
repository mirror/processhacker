/*
 * Process Hacker - 
 *   logging
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
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public static class Logging
    {
        public enum Importance : int
        {
            Information = 0,
            Warning,
            Error,
            Critical
        }   

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern void OutputDebugString(string OutputString);

        private static object _logLock = new object();

        [Conditional("DEBUG")]
        public static void Log(Importance importance, string message)
        {
            lock (_logLock)
            {
                string debugMessage =
                    DateTime.Now.ToString("hh:mm:ss:fff:") +
                    " ProcessHacker (T" + System.Threading.Thread.CurrentThread.ManagedThreadId +
                    "): (" + importance.ToString() + ") " + message + "\n" + Environment.StackTrace;

                OutputDebugString(debugMessage);

                try
                {
                    Program.HackerWindow.QueueMessage(debugMessage);
                }
                catch
                { }
            }
        }

        [Conditional("DEBUG")]
        public static void Log(Exception ex)
        {
            string message = ex.Message;

            if (ex.InnerException != null)
                message += "\nInner exception:\n" + ex.InnerException.ToString();

            Log(Importance.Error, message);
        }
    }
}
