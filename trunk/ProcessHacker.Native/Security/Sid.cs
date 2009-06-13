/*
 * Process Hacker - 
 *   SID object
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
using System.Text;
using ProcessHacker.Common;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security
{
    /// <summary>
    /// Represents a Windows Security Identifier (SID).
    /// </summary>
    public class Sid : BaseObject, IEquatable<Sid>
    {
        public enum WellKnownSidIdentifierAuthority
        {
            None = 0,
            Null,
            World,
            Local,
            Creator,
            NonUnique,
            NtAuthority,
            ResourceManager
        }

        private static readonly byte[] _nullSidAuthority = { 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] _worldSidAuthority = { 0, 0, 0, 0, 0, 1 };
        private static readonly byte[] _localSidAuthority = { 0, 0, 0, 0, 0, 2 };
        private static readonly byte[] _creatorSidAuthority = { 0, 0, 0, 0, 0, 3 };
        private static readonly byte[] _nonUniqueAuthority = { 0, 0, 0, 0, 0, 4 };
        private static readonly byte[] _ntAuthority = { 0, 0, 0, 0, 0, 5 };
        private static readonly byte[] _resourceManagerAuthority = { 0, 0, 0, 0, 0, 9 };

        public static Sid FromAccountName(string accountName)
        {
            MemoryAlloc memory = new MemoryAlloc(Win32.SecurityMaxSidSize);
            int memorySize = memory.Size;
            SidNameUse nameUse;

            if (!Win32.LookupAccountName(null, accountName, memory, ref memorySize, 
                IntPtr.Zero, IntPtr.Zero, out nameUse))
                Win32.ThrowLastError();

            using (memory)
                return new Sid(memory);
        }

        public static Sid FromPointer(IntPtr sid)
        {
            return new Sid(sid, false);
        }

        public static Sid GetWellKnownSid(WellKnownSidType sidType)
        {
            MemoryAlloc memory = new MemoryAlloc(Win32.SecurityMaxSidSize);
            int memorySize = memory.Size;

            if (!Win32.CreateWellKnownSid(sidType, IntPtr.Zero, memory, ref memorySize))
                Win32.ThrowLastError();

            using (memory)
                return new Sid(memory);
        }

        public static byte[] GetWellKnownSidIdentifierAuthority(WellKnownSidIdentifierAuthority sidAuthority)
        {
            return GetWellKnownSidIdentifierAuthority(sidAuthority, true);
        }

        private static byte[] GetWellKnownSidIdentifierAuthority(WellKnownSidIdentifierAuthority sidAuthority, bool copy)
        {
            byte[] array;

            switch (sidAuthority)
            {
                case WellKnownSidIdentifierAuthority.Null:
                    array = _nullSidAuthority;
                    break;
                case WellKnownSidIdentifierAuthority.World:
                    array = _worldSidAuthority;
                    break;
                case WellKnownSidIdentifierAuthority.Local:
                    array = _localSidAuthority;
                    break;
                case WellKnownSidIdentifierAuthority.Creator:
                    array = _creatorSidAuthority;
                    break;
                case WellKnownSidIdentifierAuthority.NonUnique:
                    array = _nonUniqueAuthority;
                    break;
                case WellKnownSidIdentifierAuthority.NtAuthority:
                    array = _ntAuthority;
                    break;
                case WellKnownSidIdentifierAuthority.ResourceManager:
                    array = _resourceManagerAuthority;
                    break;
                default:
                    throw new ArgumentException("sidAuthority");
            }

            if (copy)
                return array.Duplicate();
            else
                return array;
        }

        public static implicit operator IntPtr(Sid sid)
        {
            return sid.Memory;
        }

        private MemoryAlloc _memory;
        private bool _hasAttributes;
        private SidAttributes _attributes;

        private string _stringSid;
        private string _domain;
        private string _name;
        private SidNameUse _nameUse = 0;

        private Sid(IntPtr sid, bool owned)
            : base(owned)
        {
            _memory = new MemoryAlloc(sid, owned);
        }

        /// <summary>
        /// Creates a SID from a string representation.
        /// </summary>
        /// <param name="stringSid">The SID string.</param>
        public Sid(string stringSid)
        {
            IntPtr sidMemory;

            if (!Win32.ConvertStringSidToSid(stringSid, out sidMemory))
                Win32.ThrowLastError();

            _memory = new LocalMemoryAlloc(sidMemory, true);
            _hasAttributes = false;
        }

        /// <summary>
        /// Copies the specified SID.
        /// </summary>
        /// <param name="sid">A pointer to an existing SID.</param>
        public Sid(IntPtr sid)
            : this(sid, false, 0)
        { }

        /// <summary>
        /// Copies the specified SID.
        /// </summary>
        /// <param name="saa">A SID_AND_ATTRIBUTES structure.</param>
        public Sid(SidAndAttributes saa)
            : this(saa.Sid, saa.Attributes)
        { }

        /// <summary>
        /// Copies the specified SID.
        /// </summary>
        /// <param name="sid">A pointer to an existing SID.</param>
        /// <param name="attributes">The attributes associated with the SID.</param>
        public Sid(IntPtr sid, SidAttributes attributes)
            : this(sid, true, attributes)
        { }

        private Sid(IntPtr sid, bool hasAttributes, SidAttributes attributes)
        {
            NtStatus status;

            _memory = new MemoryAlloc(Win32.RtlLengthSid(sid));

            if ((status = Win32.RtlCopySid(_memory.Size, _memory, sid)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            _hasAttributes = hasAttributes;
            _attributes = attributes;
        }

        protected override void DisposeObject(bool disposing)
        {
            _memory.Dispose(disposing);
        }

        public SidAttributes Attributes
        {
            get { return _attributes; }
        }

        public string DomainName
        {
            get
            {
                if (_domain == null)
                    this.GetNameAndUse(out _domain, out _name, out _nameUse);
                return _domain;
            }
        }

        public byte[] IdentifierAuthority
        {
            get
            {
                unsafe
                {
                    return Utils.Create((*Win32.RtlIdentifierAuthoritySid(this)).Value, 6);
                }
            }
        }

        public bool HasAttributes
        {
            get { return _hasAttributes; }
        }

        public IntPtr Memory
        {
            get { return _memory; }
        }

        public SidNameUse NameUse
        {
            get
            {
                if (_nameUse == 0)
                    this.GetNameAndUse(out _domain, out _name, out _nameUse);
                return _nameUse;
            }
        }

        public string UserName
        {
            get
            {
                if (_name == null)
                    this.GetNameAndUse(out _domain, out _name, out _nameUse);
                return _name;
            }
        }

        public int[] SubAuthorities
        {
            get
            {
                unsafe
                {
                    byte count = *Win32.RtlSubAuthorityCountSid(this);
                    int[] subAuthorities = new int[count];

                    for (int i = 0; i < count; i++)
                        subAuthorities[i] = *Win32.RtlSubAuthoritySid(this, i);

                    return subAuthorities;
                }
            }
        }

        public int Size
        {
            get { return Win32.RtlLengthSid(this); }
        }

        public string StringSid
        {
            get
            {
                if (_stringSid == null)
                    _stringSid = this.GetString();
                return _stringSid;
            }
        }

        public bool DomainEquals(Sid obj)
        {
            bool equal;

            if (!Win32.EqualDomainSid(this, obj, out equal))
                Win32.ThrowLastError();

            return equal;
        }

        public bool Equals(Sid obj)
        {
            return Win32.RtlEqualSid(this, obj);
        }

        public string GetFullName(bool includeDomain)
        {
            try
            {
                if (string.IsNullOrEmpty(this.UserName))
                    return this.StringSid;
                if (includeDomain)
                    return this.DomainName + "\\" + this.UserName;
                else
                    return this.UserName;
            }
            catch
            {
                return this.StringSid;
            }
        }

        private void GetNameAndUse(out string domain, out string name, out SidNameUse nameUse)
        {
            StringBuilder nameSb = new StringBuilder(256);
            StringBuilder domainSb = new StringBuilder(256);
            int nameLen = 256;
            int domainLen = 256;

            if (!Win32.LookupAccountSid(null, this, nameSb, ref nameLen, domainSb, ref domainLen, out nameUse))
            {
                // if the name is longer than 256 characters, increase the capacity.
                nameSb.EnsureCapacity(nameLen);
                domainSb.EnsureCapacity(domainLen);

                if (!Win32.LookupAccountSid(null, this, nameSb, ref nameLen, domainSb, ref domainLen, out nameUse))
                    Win32.ThrowLastError();
            }

            domain = domainSb.ToString();
            name = nameSb.ToString();
        }

        public WellKnownSidIdentifierAuthority GetWellKnownIdentifierAuthority()
        {
            byte[] identifierAuthority = this.IdentifierAuthority;

            foreach (WellKnownSidIdentifierAuthority value in
                Enum.GetValues(typeof(WellKnownSidIdentifierAuthority)))
            {
                if (value == WellKnownSidIdentifierAuthority.None)
                    continue;

                if (Utils.Equals(identifierAuthority, GetWellKnownSidIdentifierAuthority(value, false)))
                    return value;
            }

            return WellKnownSidIdentifierAuthority.None;
        }

        private string GetString()
        {
            NtStatus status;
            UnicodeString str = new UnicodeString();

            if ((status = Win32.RtlConvertSidToUnicodeString(ref str, this, true)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            using (str)
                return str.Read();
        }

        public bool PrefixEquals(Sid obj)
        {
            return Win32.RtlEqualPrefixSid(this, obj);
        }

        public SidAndAttributes ToSidAndAttributes()
        {
            return new SidAndAttributes()
            {
                Attributes = _attributes,
                Sid = this
            };
        }

        public override string ToString()
        {
            return this.StringSid;
        }
    }
}
