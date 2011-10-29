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
using System.Diagnostics;

namespace ProcessHacker.Common
{
    public delegate void LoggingDelegate(string message);

    public static class Logging
    {
        public enum Importance
        {
            Information = 0,
            Warning,
            Error,
            Critical
        }

        public static event LoggingDelegate Logged;

        [Conditional("DEBUG")]
        public static void Log(Importance importance, string message)
        {
            string debugMessage =
                DateTime.Now.ToString("hh:mm:ss:fff:") +
                " ProcessHacker (T" + System.Threading.Thread.CurrentThread.ManagedThreadId +
                "): (" + importance.ToString() + ") " + message + "\r\n\r\n" + Environment.StackTrace;

            Debugger.Log(0, "DEBUG", debugMessage);

            if (Logged != null)
                Logged(debugMessage);
        }

        [Conditional("DEBUG")]
        public static void Log(Exception ex)
        {
            string message = ex.Message;

            if (ex.InnerException != null)
                message += "\r\nInner exception:\r\n" + ex.InnerException;
            if (ex.StackTrace != null)
                message += "\r\n" + ex.StackTrace;

            Log(Importance.Error, message);
        }
    }
}
