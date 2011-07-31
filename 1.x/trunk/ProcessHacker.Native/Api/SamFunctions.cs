/*
 * Process Hacker - 
 *   SAM functions
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
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Api
{
    public static partial class Win32
    {
        [DllImport("samlib.dll")]
        public static extern NtStatus SamAddMemberToAlias(
            [In] IntPtr AliasHandle,
            [In] IntPtr MemberId // Sid*
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamAddMemberToGroup(
            [In] IntPtr GroupHandle,
            [In] int MemberId,
            [In] int Attributes
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamAddMultipleMembersToAlias(
            [In] IntPtr AliasHandle,
            [In] IntPtr[] MemberIds, // Sid**
            [In] int MemberCount
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamChangePasswordUser(
            [In] IntPtr UserHandle,
            [In] ref UnicodeString OldPassword,
            [In] ref UnicodeString NewPassword
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamChangePasswordUser2(
            [In] ref UnicodeString ServerName,
            [In] ref UnicodeString UserName,
            [In] ref UnicodeString OldPassword,
            [In] ref UnicodeString NewPassword
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamCloseHandle(
            [In] IntPtr SamHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamConnect(
            [In] ref UnicodeString ServerName,
            [Out] out IntPtr ServerHandle,
            [In] SamServerAccess DesiredAccess,
            [In] ref ObjectAttributes ObjectAttributes
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamCreateAliasInDomain(
            [In] IntPtr DomainHandle,
            [In] ref UnicodeString AccountName,
            [In] SamAliasAccess DesiredAccess,
            [Out] out IntPtr AliasHandle,
            [Out] out int RelativeId
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamCreateGroupInDomain(
            [In] IntPtr DomainHandle,
            [In] ref UnicodeString AccountName,
            [In] SamGroupAccess DesiredAccess,
            [Out] out IntPtr GroupHandle,
            [Out] out int RelativeId
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamCreateUserInDomain(
            [In] IntPtr DomainHandle,
            [In] ref UnicodeString AccountName,
            [In] SamUserAccess DesiredAccess,
            [Out] out IntPtr UserHandle,
            [Out] out int RelativeId
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamCreateUser2InDomain(
            [In] IntPtr DomainHandle,
            [In] ref UnicodeString AccountName,
            [In] UserAccountFlags AccountType,
            [In] SamUserAccess DesiredAccess,
            [Out] out IntPtr UserHandle,
            [Out] out SamUserAccess GrantedAccess,
            [Out] out int RelativeId
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamDeleteAlias(
            [In] IntPtr AliasHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamDeleteGroup(
            [In] IntPtr GroupHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamDeleteUser(
            [In] IntPtr UserHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamEnumerateAliasesInDomain(
            [In] IntPtr DomainHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // SamRidEnumeration**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamEnumerateDomainsInSamServer(
            [In] IntPtr ServerHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // SamSidEnumeration**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamEnumerateGroupsInDomain(
            [In] IntPtr DomainHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // SamRidEnumeration**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamEnumerateUsersInDomain(
            [In] IntPtr DomainHandle,
            ref int EnumerationContext,
            [In] UserAccountFlags UserAccountControl,
            [Out] out IntPtr Buffer, // SamRidEnumeration**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamFreeMemory(
            [In] IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamGetAliasMembership(
            [In] IntPtr DomainHandle,
            [In] int PassedCount,
            [In] IntPtr[] Sids, // Sid**
            [Out] out int MembershipCount,
            [Out] out IntPtr Aliases // int**
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamGetGroupsForUser(
            [In] IntPtr UserHandle,
            [Out] out IntPtr Groups, // GroupMembership**
            [Out] out int MembershipCount
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamGetMembersInAlias(
            [In] IntPtr AliasHandle,
            [Out] out IntPtr MemberIds, // Sid***
            [Out] out int MemberCount
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamGetMembersInGroup(
            [In] IntPtr GroupHandle,
            [Out] out IntPtr MemberIds, // int**
            [Out] out IntPtr Attributes, // int**
            [Out] out int MemberCount
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamLookupDomainInSamServer(
            [In] IntPtr ServerHandle,
            [In] ref UnicodeString Name,
            [Out] out IntPtr DomainId // Sid**
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamLookupIdsInDomain(
            [In] IntPtr DomainHandle,
            [In] int Count,
            [In] int[] RelativeIds,
            [Out] out IntPtr Names, // UnicodeString**
            [Out] out IntPtr Use // SidNameUse**
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamLookupNamesInDomain(
            [In] IntPtr DomainHandle,
            [In] int Count,
            [In] UnicodeString[] Names,
            [Out] out IntPtr RelativeIds, // int**
            [Out] out IntPtr Use // SidNameUse**
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamOpenAlias(
            [In] IntPtr DomainHandle,
            [In] SamAliasAccess DesiredAccess,
            [In] int AliasId,
            [Out] out IntPtr AliasHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamOpenDomain(
            [In] IntPtr ServerHandle,
            [In] SamDomainAccess DesiredAccess,
            [In] IntPtr DomainId, // Sid*
            [Out] out IntPtr DomainHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamOpenGroup(
            [In] IntPtr DomainHandle,
            [In] SamGroupAccess DesiredAccess,
            [In] int GroupId,
            [Out] out IntPtr GroupHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamOpenUser(
            [In] IntPtr DomainHandle,
            [In] SamUserAccess DesiredAccess,
            [In] int UserId,
            [Out] out IntPtr UserHandle
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamQueryInformationAlias(
            [In] IntPtr AliasHandle,
            [In] AliasInformationClass AliasInformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamQueryInformationDomain(
            [In] IntPtr DomainHandle,
            [In] DomainInformationClass DomainInformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamQueryInformationGroup(
            [In] IntPtr GroupHandle,
            [In] GroupInformationClass GroupInformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamQueryInformationUser(
            [In] IntPtr UserHandle,
            [In] UserInformationClass UserInformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamQuerySecurityObject(
            [In] IntPtr ObjectHandle,
            [In] SecurityInformation SecurityInformation,
            [Out] out IntPtr SecurityDescriptor // SecurityDescriptor**
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamRemoveMemberFromAlias(
            [In] IntPtr AliasHandle,
            [In] IntPtr MemberId // Sid*
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamRemoveMemberFromForeignDomain(
            [In] IntPtr DomainHandle,
            [In] IntPtr MemberId // Sid*
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamRemoveMemberFromGroup(
            [In] IntPtr GroupHandle,
            [In] int MemberId
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamRemoveMultipleMembersFromAlias(
            [In] IntPtr AliasHandle,
            [In] IntPtr[] MemberIds, // Sid**
            [In] int MemberCount
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamRidToSid(
            [In] IntPtr DomainHandle,
            [In] int RelativeId,
            [Out] out IntPtr Sid // Sid**
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamSetInformationAlias(
            [In] IntPtr AliasHandle,
            [In] AliasInformationClass AliasInformationClass,
            [In] IntPtr AliasInformation
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamSetInformationDomain(
            [In] IntPtr DomainHandle,
            [In] DomainInformationClass DomainInformationClass,
            [In] IntPtr DomainInformation
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamSetInformationGroup(
            [In] IntPtr GroupHandle,
            [In] GroupInformationClass GroupInformationClass,
            [In] IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamSetInformationUser(
            [In] IntPtr UserHandle,
            [In] UserInformationClass UserInformationClass,
            [In] IntPtr Buffer
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamSetMemberAttributesOfGroup(
            [In] IntPtr GroupHandle,
            [In] int MemberId,
            [In] int Attributes
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamSetSecurityObject(
            [In] IntPtr ObjectHandle,
            [In] SecurityInformation SecurityInformation,
            [In] IntPtr SecurityDescriptor // SecurityDescriptor*
            );

        [DllImport("samlib.dll")]
        public static extern NtStatus SamShutdownSamServer(
            [In] IntPtr ServerHandle
            );
    }
}
