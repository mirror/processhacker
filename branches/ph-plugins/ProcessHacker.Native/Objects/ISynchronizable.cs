/*
 * Process Hacker - 
 *   object with synchronize functions
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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a Windows object that can be synchronized with.
    /// </summary>
    public interface ISynchronizable
    {
        IntPtr Handle { get; }

        //NtStatus SignalAndWait(ISynchronizable waitObject);
        //NtStatus SignalAndWait(ISynchronizable waitObject, bool alertable);
        //NtStatus SignalAndWait(ISynchronizable waitObject, bool alertable, long timeout);
        NtStatus Wait();
        NtStatus Wait(bool alertable);
        NtStatus Wait(bool alertable, long timeout);
    }
}
