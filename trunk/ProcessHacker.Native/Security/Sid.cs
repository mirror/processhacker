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
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Security
{
    public class Sid : DisposableObject, IEquatable<Sid>
    {
        public static Sid FromPointer(IntPtr sid)
        {
            return new Sid(sid, false);
        }

        public static implicit operator IntPtr(Sid sid)
        {
            return sid.Memory;
        }

        private MemoryAlloc _sid;
        private bool _hasAttributes;
        private SidAttributes _attributes;

        private string _stringSid;
        private string _domain;
        private string _name;
        private SidNameUse _nameUse = 0;

        private Sid(IntPtr sid, bool owned)
            : base(owned)
        {
            _sid = new MemoryAlloc(sid, false);
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
        /// <param name="sid">A pointer to an existing SID.</param>
        /// <param name="attributes">The attributes associated with the SID.</param>
        public Sid(IntPtr sid, SidAttributes attributes)
            : this(sid, true, attributes)
        { }

        private Sid(IntPtr sid, bool hasAttributes, SidAttributes attributes)
        {
            NtStatus status;

            _sid = new MemoryAlloc(Win32.RtlLengthSid(sid));

            if ((status = Win32.RtlCopySid(_sid.Size, _sid, sid)) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            _hasAttributes = hasAttributes;
            _attributes = attributes;
        }

        protected override void DisposeObject(bool disposing)
        {
            _sid.Dispose(disposing);
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

        public SidIdentifierAuthority IdentifierAuthority
        {
            get
            {
                unsafe
                {
                    return *Win32.RtlIdentifierAuthoritySid(this);
                }
            }
        }

        public bool HasAttributes
        {
            get { return _hasAttributes; }
        }

        public IntPtr Memory
        {
            get { return _sid; }
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

        public override string ToString()
        {
            return this.StringSid;
        }
    }
}
