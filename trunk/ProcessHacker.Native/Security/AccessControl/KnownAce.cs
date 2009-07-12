using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security.AccessControl
{
    public class KnownAce : Ace
    {
        private int _mask;
        private Sid _sid;

        protected KnownAce()
        { }

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
