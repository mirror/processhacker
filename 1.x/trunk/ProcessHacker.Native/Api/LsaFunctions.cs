/*
 * Process Hacker - 
 *   LSA functions
 *
 * Copyright (C) 2009 wj32
 * Copyright (C) 2011 dmex
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

using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Api
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    public static partial class Win32
    {
        /* Note: Be very careful about where these functions are 
         * imported from. Some come from advapi32.dll, others are 
         * from secur32.dll.
         * 
         * An important side-effect is that ALL buffers allocated 
         * by secur32 functions MUST be freed with 
         * LsaFreeReturnBuffer, while advapi32-allocated buffers 
         * must be freed with LsaFreeMemory.
         */

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaAddAccountRights(
            [In] IntPtr PolicyHandle,
            [In] IntPtr AccountSid, // Sid*
            [In] UnicodeString[] UserRights,
            [In] int CountOfRights
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaAddPrivilegesToAccount(
            [In] IntPtr AccountHandle,
            [In] IntPtr Privileges // PrivilegeSet*
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaClearAuditLog(
            [In] IntPtr PolicyHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaClose(
            [In] IntPtr ObjectHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaChangePassword(
            [In] UnicodeString ServerName,
            [In] UnicodeString DomainName,
            [In] UnicodeString AccountName,
            [In] UnicodeString OldPassword,
            [In] UnicodeString NewPassword
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaConnectUntrusted(
            [Out] out IntPtr LsaHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaCreateAccount(
            [In] IntPtr PolicyHandle,
            [In] IntPtr AccountSid, // Sid*
            [In] LsaAccountAccess DesiredAccess,
            [Out] out IntPtr AccountHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaCreateSecret(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString SecretName,
            [In] LsaSecretAccess DesiredAccess,
            [Out] out IntPtr SecretHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaCreateTrustedDomain(
            [In] IntPtr PolicyHandle,
            [In] LsaTrustInformation TrustedDomainInformation,
            [In] LsaTrustedAccess DesiredAccess,
            [Out] out IntPtr TrustedDomainHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaDelete(
            [In] IntPtr ObjectHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaDeleteTrustedDomain(
            [In] IntPtr PolicyHandle,
            [In] IntPtr TrustedDomainSid // Sid*
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaDeregisterLogonProcess(
            [In] IntPtr LsaHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumerateAccounts(
            [In] IntPtr PolicyHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // Sid***
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumerateAccountsWithUserRight(
            [In] IntPtr PolicyHandle,
            [In, Optional] UnicodeString UserRight,
            [Out] out IntPtr Buffer, // Sid***
            [Out] out int CountReturned
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumerateAccountRights(
            [In] IntPtr PolicyHandle,
            [In] IntPtr AccountSid, // Sid*
            [Out] IntPtr UserRights, // UnicodeString**
            [Out] out int CountOfRights
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaEnumerateLogonSessions(
            [Out] out int LogonSessionCount,
            [Out] out IntPtr LogonSessionList // Luid**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumeratePrivileges(
            [In] IntPtr PolicyHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // PolicyPrivilegeDefinition**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumeratePrivilegesOfAccount(
            [In] IntPtr AccountHandle,
            [Out] out IntPtr Privileges // PrivilegeSet**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumerateTrustedDomains(
            [In] IntPtr PolicyHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // LsaTrustInformation**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaEnumerateTrustedDomainsEx(
            [In] IntPtr PolicyHandle,
            ref int EnumerationContext,
            [Out] out IntPtr Buffer, // TrustedDomainInformationEx**
            [In] int PreferredMaximumLength,
            [Out] out int CountReturned
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaFreeMemory(
            [In] IntPtr Buffer
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaFreeReturnBuffer(
            [In] IntPtr Buffer
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaGetLogonSessionData(
            [In] Luid LogonId,
            [Out] out IntPtr LogonSessionData // SecurityLogonSessionData**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaGetQuotasForAccount(
            [In] IntPtr AccountHandle,
            [Out] out QuotaLimits QuotaLimits
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaGetRemoteUserName(
            [In] [Optional] ref UnicodeString SystemName,
            [Out] out IntPtr UserName, // UnicodeString**
            [Out] [Optional] out IntPtr DomainName // UnicodeString**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaGetSystemAccessAccount(
            [In] IntPtr AccountHandle,
            [Out] out SecuritySystemAccess SystemAccess
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaGetUserName(
            [Out] out IntPtr UserName, // UnicodeString**
            [Out] [Optional] out IntPtr DomainName // UnicodeString**
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaLogonUser(
            [In] IntPtr LsaHandle,
            [In] AnsiString OriginName,
            [In] SecurityLogonType LogonType,
            [In] int AuthenticationPackage,
            [In] IntPtr AuthenticationInformation,
            [In] int AuthenticationInformationLength,
            [In] [Optional] IntPtr LocalGroups, // TokenGroups*
            [In] TokenSource SourceContext,
            [Out] out IntPtr ProfileBuffer,
            [Out] out int ProfileBufferLength,
            [Out] out Luid LogonId,
            [Out] out IntPtr Token,
            [Out] out QuotaLimits Quotas,
            [Out] out NtStatus SubStatus
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaLookupAuthenticationPackage(
            [In] IntPtr LsaHandle,
            [In] AnsiString PackageName,
            [Out] out int AuthenticationPackage
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaLookupNames(
            [In] IntPtr PolicyHandle,
            [In] int Count,
            [In] UnicodeString[] Names,
            [Out] out IntPtr ReferencedDomains, // LsaReferencedDomainList**
            [Out] out IntPtr Sids // LsaTranslatedSid**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaLookupNames2(
            [In] IntPtr PolicyHandle,
            [In] int Flags,
            [In] int Count,
            [In] UnicodeString[] Names,
            [Out] out IntPtr ReferencedDomains, // LsaReferencedDomainList**
            [Out] out IntPtr Sids // LsaTranslatedSid2**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaLookupPrivilegeDisplayName(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString Name,
            [Out] out IntPtr DisplayName, // UnicodeString**
            [Out] out short LanguageReturned
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaLookupPrivilegeName(
            [In] IntPtr PolicyHandle,
            [In, Out] ref Luid Value,
            [Out] out IntPtr Name // UnicodeString**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaLookupPrivilegeValue(
            [In] IntPtr PolicyHandle,
            [In, Out] ref UnicodeString Name,
            [Out] out Luid Value
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaLookupSids(
            [In] IntPtr PolicyHandle,
            [In] int Count,
            [In] IntPtr[] Sids, // Sid**
            [Out] out IntPtr ReferencedDomains, // LsaReferencedDomainList**
            [Out] out IntPtr Names // LsaTranslatedName**
            );

        [DllImport("advapi32.dll")]
        public static extern int LsaNtStatusToWinError(
            [In] NtStatus Status
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaOpenAccount(
            [In] IntPtr PolicyHandle,
            [In] IntPtr AccountSid, // Sid*
            [In] LsaAccountAccess DesiredAccess,
            [Out] out IntPtr AccountHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaOpenPolicy(
            [In] [Optional] ref UnicodeString SystemName,
            [In] ref ObjectAttributes ObjectAttributes,
            [In] LsaPolicyAccess DesiredAccess,
            [Out] out IntPtr PolicyHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaOpenPolicySce(
            [In] [Optional] ref UnicodeString SystemName,
            [In] ObjectAttributes ObjectAttributes,
            [In] LsaPolicyAccess DesiredAccess,
            [Out] out IntPtr PolicyHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaOpenSecret(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString SecretName,
            [In] LsaSecretAccess DesiredAccess,
            [Out] out IntPtr SecretHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaOpenTrustedDomain(
            [In] IntPtr PolicyHandle,
            [In] IntPtr TrustedDomainSid, // Sid*
            [In] LsaTrustedAccess DesiredAccess,
            [Out] out IntPtr TrustedDomainHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaOpenTrustedDomainByName(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString TrustedDomainName,
            [In] LsaTrustedAccess DesiredAccess,
            [Out] out IntPtr TrustedDomainHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQueryDomainInformationPolicy(
            [In] IntPtr PolicyHandle,
            [In] PolicyDomainInformationClass InformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQueryInformationPolicy(
            [In] IntPtr PolicyHandle,
            [In] PolicyInformationClass InformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQueryInfoTrustedDomain(
            [In] IntPtr TrustedDomainHandle,
            [In] TrustedInformationClass InformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQuerySecret(
            [In] IntPtr SecretHandle,
            [Out] [Optional] out IntPtr CurrentValue, // UnicodeString**
            [Out] [Optional] out long CurrentValueSetTime,
            [Out] [Optional] out IntPtr OldValue, // UnicodeString**
            [Out] [Optional] out long OldValueSetTime
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQuerySecurityObject(
            [In] IntPtr ObjectHandle,
            [In] SecurityInformation SecurityInformation,
            [Out] out IntPtr SecurityDescriptor // SecurityDescriptor**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQueryTrustedDomainInfo(
            [In] IntPtr PolicyHandle,
            [In] IntPtr TrustedDomainSid, // Sid*
            [In] TrustedInformationClass InformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaQueryTrustedDomainInfoByName(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString TrustedDomainName,
            [In] TrustedInformationClass InformationClass,
            [Out] out IntPtr Buffer
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaRegisterLogonProcess(
            [In] AnsiString LogonProcessName,
            [Out] out IntPtr LsaHandle,
            [Out] out LsaOperationalMode SecurityMode
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaRegisterPolicyChangeNotification(
            [In] PolicyNotificationInformationClass InformationClass,
            [In] IntPtr NotificationEventHandle
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaRemoveAccountRights(
            [In] IntPtr PolicyHandle,
            [In] IntPtr AccountSid, // Sid*
            [In] bool AllRights,
            [In] UnicodeString[] UserRights,
            [In] int CountOfRights
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaRemovePrivilegesFromAccount(
            [In] IntPtr AccountHandle,
            [In] bool AllPrivileges,
            [In] [Optional] IntPtr Privileges // PrivilegeSet*
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaRetrievePrivateData(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString KeyName,
            [Out] out IntPtr PrivateData // UnicodeString**
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetDomainInformationPolicy(
            [In] IntPtr PolicyHandle,
            [In] PolicyDomainInformationClass InformationClass,
            [In] [Optional] IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetInformationPolicy(
            [In] IntPtr PolicyHandle,
            [In] PolicyInformationClass InformationClass,
            [In] IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetInformationTrustedDomain(
            [In] IntPtr TrustedDomainHandle,
            [In] TrustedInformationClass InformationClass,
            [In] IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetQuotasForAccount(
            [In] IntPtr AccountHandle,
            [In] QuotaLimits QuotaLimits
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetSecret(
            [In] IntPtr SecretHandle,
            [In] [Optional] ref UnicodeString CurrentValue,
            [In] [Optional] ref UnicodeString OldValue
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetSecurityObject(
            [In] IntPtr ObjectHandle,
            [In] SecurityInformation SecurityInformation,
            [In] IntPtr SecurityDescriptor // SecurityDescriptor*
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetSystemAccessAccount(
            [In] IntPtr AccountHandle,
            [In] SecuritySystemAccess SystemAccess
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetTrustedDomainInformation(
            [In] IntPtr PolicyHandle,
            [In] IntPtr TrustedDomainSid, // Sid*
            [In] TrustedInformationClass InformationClass,
            [In] IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaSetTrustedDomainInfoByName(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString TrustedDomainName,
            [In] TrustedInformationClass InformationClass,
            [In] IntPtr Buffer
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaStorePrivateData(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString KeyName,
            [In, Optional] UnicodeString PrivateData
            );

        [DllImport("advapi32.dll")]
        public static extern NtStatus LsaStorePrivateData(
            [In] IntPtr PolicyHandle,
            [In] UnicodeString KeyName,
            [In, Optional] IntPtr PrivateData
            );

        [DllImport("secur32.dll")]
        public static extern NtStatus LsaUnregisterPolicyChangeNotification(
            [In] PolicyNotificationInformationClass InformationClass,
            [In] IntPtr NotificationEventHandle
            );
    }
}
