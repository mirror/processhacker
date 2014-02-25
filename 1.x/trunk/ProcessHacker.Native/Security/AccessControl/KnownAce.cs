/*
 * Process Hacker - 
 *   known access control entry
 *   (access allowed, access denied, system alarm, system audit)
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
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.AccessControl
{
    public class KnownAce : Ace
    {
        private int _mask;
        private Sid _sid;

        protected KnownAce()
        { }

        public KnownAce(AceType type, AceFlags flags, int mask, Sid sid)
        {
            if (
                type != AceType.AccessAllowed &&
                type != AceType.AccessDenied &&
                type != AceType.SystemAlarm &&
                type != AceType.SystemAudit
                )
                throw new ArgumentException("Invalid ACE type.");

            this.MemoryRegion = new MemoryAlloc(
                Marshal.SizeOf(typeof(KnownAceStruct)) - // known ace struct size
                sizeof(int) + // minus SidStart field
                sid.Length // plus SID length
                );

            KnownAceStruct knownAce = new KnownAceStruct();

            // Initialize the ACE (minus the SID).
            knownAce.Header.AceType = type;
            knownAce.Header.AceFlags = flags;
            knownAce.Header.AceSize = (ushort)this.MemoryRegion.Size;
            knownAce.Mask = mask;
            // Write the ACE to memory.
            this.MemoryRegion.WriteStruct<KnownAceStruct>(knownAce);
            // Write the SID.
            this.MemoryRegion.WriteMemory(Win32.KnownAceSidStartOffset.ToInt32(), sid, sid.Length);
            // Update the cached info.
            this.Read();
        }

        public KnownAce(IntPtr memory)
            : base(memory)
        { }

        public int Mask
        {
            get { return _mask; }
        }

        public Sid Sid
        {
            get { return _sid; }
        }

        protected override void Read()
        {
            var knownAce = this.MemoryRegion.ReadStruct<KnownAceStruct>();

            _mask = knownAce.Mask;
            _sid = Sid.FromPointer(this.Memory.Increment(Win32.KnownAceSidStartOffset));

            base.Read();
        }
    }
}
