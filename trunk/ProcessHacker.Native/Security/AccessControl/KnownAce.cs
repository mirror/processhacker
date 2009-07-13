using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using System.Runtime.InteropServices;

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

            this.MemoryAlloc = new MemoryAlloc(
                Marshal.SizeOf(typeof(KnownAceStruct)) - // known ace struct size
                sizeof(int) + // minus SidStart field
                sid.Length // plus SID length
                );

            KnownAceStruct knownAce = new KnownAceStruct();

            // Initialize the ACE (minus the SID).
            knownAce.Header.AceType = type;
            knownAce.Header.AceFlags = flags;
            knownAce.Header.AceSize = (ushort)this.MemoryAlloc.Size;
            knownAce.Mask = mask;
            // Write the ACE to memory.
            this.MemoryAlloc.WriteStruct<KnownAceStruct>(knownAce);
            // Write the SID.
            this.MemoryAlloc.WriteMemory(Win32.KnownAceSidStartOffset.ToInt32(), sid, 0, sid.Length);
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
            var memory = new MemoryAlloc(this.Memory, false);
            var knownAce = memory.ReadStruct<KnownAceStruct>();

            _mask = knownAce.Mask;
            _sid = Sid.FromPointer(this.Memory.Increment(Win32.KnownAceSidStartOffset));

            base.Read();
        }
    }
}
