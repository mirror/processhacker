/*
 * Process Hacker - 
 *   SAM enumerations
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
using System.Text;

namespace ProcessHacker.Native.Api
{
    public enum AliasInformationClass : int
    {
        AliasGeneralInformation = 1,
        AliasNameInformation,
        AliasAdminCommentInformation
    }

    public enum DomainInformationClass : int
    {
        DomainPasswordInformation = 1,
        DomainGeneralInformation,
        DomainLogoffInformation,
        DomainOemInformation,
        DomainNameInformation,
        DomainReplicationInformation,
        DomainServerRoleInformation,
        DomainModifiedInformation,
        DomainStateInformation,
        DomainUasInformation,
        DomainGeneralInformation2,
        DomainLockoutInformation,
        DomainModifiedInformation2
    }

    public enum DomainServerEnableState : int
    {
        Enabled = 1,
        Disabled
    }

    public enum DomainServerRole : int
    {
        Backup = 2,
        Primary
    }

    public enum GroupInformationClass : int
    {
        GroupGeneralInformation = 1,
        GroupNameInformation,
        GroupAttributeInformation,
        GroupAdminCommentInformation
    }

    public enum SamAccountType : int
    {
        User = 1,
        Group,
        Alias
    }

    public enum UserAccountFlags : uint
    {
        Disabled = 0x1,
        HomeDirectoryRequired = 0x2,
        PasswordNotRequired = 0x4,
        TempDuplicateAccount = 0x8,
        NormalAccount = 0x10,
        MnsLogonAccount = 0x20,
        InterdomainTrustAccount = 0x40,
        WorkstationTrustAccount = 0x80,
        ServerTrustAccount = 0x100,
        DontExpirePassword = 0x200,
        AccountAutoLocked = 0x400,

        MachineAccountMask = InterdomainTrustAccount | WorkstationTrustAccount |
            ServerTrustAccount,
        AccountTypeMask = TempDuplicateAccount | NormalAccount | MachineAccountMask
    }

    public enum UserInformationClass : int
    {
        UserGeneralInformation = 1,
        UserPreferencesInformation,
        UserLogonInformation,
        UserLogonHoursInformation,
        UserAccountInformation,
        UserNameInformation,
        UserAccountNameInformation,
        UserFullNameInformation,
        UserPrimaryGroupInformation,
        UserHomeInformation,
        UserScriptInformation,
        UserProfileInformation,
        UserAdminCommentInformation,
        UserWorkStationsInformation,
        UserSetPasswordInformation,
        UserControlInformation,
        UserExpiresInformation,
        UserInternal1Information,
        UserInternal2Information,
        UserParametersInformation,
        UserAllInformation,
        UserInternal3Information,
        UserInternal4Information,
        UserInternal5Information
    }

    public enum UserWhichFields : uint
    {
        Username = 0x1,
        Fullname = 0x2,
        UserId = 0x4,
        PrimaryGroupId = 0x8,
        AdminComment = 0x10,
        UserComment = 0x20,
        HomeDirectory = 0x40,
        HomeDirectoryDrive = 0x80,
        ScriptPath = 0x100,
        ProfilePath = 0x200,
        Workstations = 0x400,
        LastLogon = 0x800,
        LastLogoff = 0x1000,
        LogonHours = 0x2000,
        BadPasswordCount = 0x4000,
        LogonCount = 0x8000,
        PasswordCanChange = 0x10000,
        PasswordMustChange = 0x20000,
        PasswordLastSet = 0x40000,
        AccountExpires = 0x80000,
        UserAccountControl = 0x100000,
        Parameters = 0x200000,
        CountryCode = 0x400000,
        CodePage = 0x800000,
        NtPasswordPresent = 0x1000000,
        LmPasswordPresent = 0x2000000,
        PrivateData = 0x4000000,
        PasswordExpired = 0x8000000,
        SecurityDescriptor = 0x10000000,
        OwfPassword = 0x20000000,

        UndefinedMask = 0xc0000000,

        ReadGeneralMask = Username | Fullname | UserId | PrimaryGroupId |
            AdminComment | UserComment,
        ReadLogonMask = HomeDirectory | HomeDirectoryDrive | ScriptPath |
            ProfilePath | Workstations | LastLogon | LastLogoff |
            LogonHours | BadPasswordCount | LogonCount | PasswordCanChange |
            PasswordMustChange,
        ReadAccountMask = PasswordLastSet | AccountExpires |
            UserAccountControl | Parameters,
        ReadPreferencesMask = CountryCode | CodePage,
        ReadTrustedMask = NtPasswordPresent | LmPasswordPresent |
            PasswordExpired | SecurityDescriptor | PrivateData,
        ReadCantMask = UndefinedMask,

        WriteAccountMask = Username | Fullname | PrimaryGroupId |
            HomeDirectory | HomeDirectoryDrive | ScriptPath |
            ProfilePath | AdminComment | Workstations | LogonHours |
            AccountExpires | UserAccountControl | Parameters,
        WritePreferencesMask = UserComment | CountryCode | CodePage,
        WriteForcePasswordChangeMask = NtPasswordPresent |
            LmPasswordPresent | PasswordExpired,
        WriteTrustedMask = LastLogon | LastLogoff | BadPasswordCount |
            LogonCount | PasswordLastSet | SecurityDescriptor |
            PrivateData,
        WriteCantMask = UserId | PasswordCanChange | PasswordMustChange |
            UndefinedMask
    }
}
