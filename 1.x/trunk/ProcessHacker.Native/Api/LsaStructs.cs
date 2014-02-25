/*
 * Process Hacker - 
 *   LSA structures
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
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessHacker.Native.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LsaReferencedDomainList
    {
        public int Entries;
        public IntPtr Domains; // LsaTrustInformation*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LsaTranslatedName
    {
        public SidNameUse Use;
        public UnicodeString Name;
        public int DomainIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LsaTranslatedSid
    {
        public SidNameUse Use;
        public int RelativeId;
        public int DomainIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LsaTranslatedSid2
    {
        public SidNameUse Use;
        public IntPtr Sid; // Sid*
        public int DomainIndex;
        public int Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LsaTrustInformation
    {
        public UnicodeString Name;
        public IntPtr Sid; // Sid*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Msv1_0_InteractiveLogon
    {
        public Msv1_0_LogonSubmitType MessageType;
        public UnicodeString LogonDomainName;
        public UnicodeString UserName;
        public UnicodeString Password;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Msv1_0_InteractiveProfile
    {
        public Msv1_0_ProfileBufferType MessageType;
        public ushort LogonCount;
        public ushort BadPasswordCount;
        public long LogonTime;
        public long LogoffTime;
        public long KickOffTime;
        public long PasswordLastSet;
        public long PasswordCanChange;
        public long PasswordMustChange;
        public UnicodeString LogonScript;
        public UnicodeString HomeDirectory;
        public UnicodeString FullName;
        public UnicodeString ProfilePath;
        public UnicodeString HomeDirectoryDrive;
        public UnicodeString LogonServer;
        public int UserFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PolicyPrivilegeDefinition
    {
        public UnicodeString Name;
        public Luid LocalValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct QuotaLimits
    {
        public IntPtr PagedPoolLimit;
        public IntPtr NonPagedPoolLimit;
        public IntPtr MinimumWorkingSetSize;
        public IntPtr MaximumWorkingSetSize;
        public IntPtr PagefileLimit;
        public long TimeLimit;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityLogonSessionData
    {
        public int Size;
        public Luid LogonId;
        public UnicodeString UserName;
        public UnicodeString LogonDomain;
        public UnicodeString AuthenticationPackage;
        public LogonType LogonType;
        public int Session;
        public IntPtr Sid; // Sid*
        public long LogonTime;
        public UnicodeString LogonServer;
        public UnicodeString DnsDomainName;
        public UnicodeString Upn;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TrustedDomainInformationEx
    {
        public UnicodeString Name;
        public UnicodeString FlatName;
        public IntPtr Sid; // Sid*
        public int TrustDirection;
        public int TrustType;
        public int TrustAttributes;
    }
}
