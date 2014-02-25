/*
 * Process Hacker - 
 *   SAM structures
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

namespace ProcessHacker.Native.Api
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AliasAdmCommentInformation
    {
        public UnicodeString AdminComment;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AliasGeneralInformation
    {
        public UnicodeString Name;
        public int MemberCount;
        public UnicodeString AdminComment;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AliasNameInformation
    {
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DomainGeneralInformation
    {
        public long ForceLogoff;
        public UnicodeString OemInformation;
        public UnicodeString DomainName;
        public UnicodeString ReplicaSourceNodeName;
        public long DomainModifiedCount;
        public DomainServerEnableState DomainServerState;
        public DomainServerRole DomainServerRole;
        [MarshalAs(UnmanagedType.I1)]
        public bool UasCompatibilityRequired;
        public int UserCount;
        public int GroupCount;
        public int AliasCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DomainNameInformation
    {
        public UnicodeString DomainName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DomainPasswordInformation
    {
        public ushort MinPasswordLength;
        public ushort PasswordHistoryLength;
        public DomainPasswordProperties PasswordProperties;
        public long MaxPasswordAge;
        public long MinPasswordAge;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GroupAdmInformation
    {
        public UnicodeString AdminComment;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GroupGeneralInformation
    {
        public UnicodeString Name;
        public int Attributes;
        public int MemberCount;
        public UnicodeString AdminComment;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GroupMembership
    {
        public int RelativeId;
        public int Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GroupNameInformation
    {
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LogonHours
    {
        public ushort UnitsPerWeek;
        public IntPtr LogonHoursBitmap; // byte* (RTL bitmap, buffer)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SamByteArray32K
    {
        public int Size;
        public IntPtr Data; // byte*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SamRidEnumeration
    {
        public int RelativeId;
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SamSidEnumeration
    {
        public IntPtr Sid; // Sid*
        public UnicodeString Name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SrSecurityDescriptor
    {
        public int Length;
        public IntPtr SecurityDescriptor; // SecurityDescriptor**
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserAccountNameInformation
    {
        public UnicodeString UserName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserAdminCommentInformation
    {
        public UnicodeString AdminComment;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct UserAllInformation
    {
        public long LastLogon;
        public long LastLogoff;
        public long PasswordLastSet;
        public long AccountExpires;
        public long PasswordCanChange;
        public long PasswordMustChange;
        public UnicodeString UserName;
        public UnicodeString FullName;
        public UnicodeString HomeDirectory;
        public UnicodeString HomeDirectoryDrive;
        public UnicodeString ScriptPath;
        public UnicodeString ProfilePath;
        public UnicodeString AdminComment;
        public UnicodeString WorkStations;
        public UnicodeString UserComment;
        public UnicodeString Parameters;
        public UnicodeString LmPassword;
        public UnicodeString NtPassword;
        public UnicodeString PrivateData;
        public SrSecurityDescriptor SecurityDescriptor;
        public int UserId;
        public int PrimaryGroupId;
        public UserAccountFlags UserAccountControl;
        public UserWhichFields WhichFields;
        public LogonHours LogonHours;
        public ushort BadPasswordCount;
        public ushort LogonCount;
        public ushort CountryCode;
        public ushort CodePage;
        [MarshalAs(UnmanagedType.I1)]
        public bool LmPasswordPresent;
        [MarshalAs(UnmanagedType.I1)]
        public bool NtPasswordPresent;
        [MarshalAs(UnmanagedType.I1)]
        public bool PasswordExpired;
        [MarshalAs(UnmanagedType.I1)]
        public bool PrivateDataSensitive;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserExtendedInformation
    {
        public UserExtendedWhichFields ExtendedWhichFields;
        public SamByteArray32K UserTile;
        public UnicodeString PasswordHint;
        [MarshalAs(UnmanagedType.I1)]
        public bool DontShowInLogonUI;
        public SamByteArray32K ShellAdminObjectProperties;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserFullNameInformation
    {
        public UnicodeString FullName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserGeneralInformation
    {
        public UnicodeString UserName;
        public UnicodeString FullName;
        public int PrimaryGroupId;
        public UnicodeString AdminComment;
        public UnicodeString UserComment;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserLogonUiInformation
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool PasswordIsBlank;
        [MarshalAs(UnmanagedType.I1)]
        public bool AccountIsDisabled;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserNameInformation
    {
        public UnicodeString UserName;
        public UnicodeString FullName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserPrimaryGroupInformation
    {
        public int PrimaryGroupId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserSetPasswordInformation
    {
        public UnicodeString Password;
        [MarshalAs(UnmanagedType.I1)]
        public bool PasswordExpired;
    }
}
