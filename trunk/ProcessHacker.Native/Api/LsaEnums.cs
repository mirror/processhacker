/*
 * Process Hacker - 
 *   LSA enumerations
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
    [Flags]
    public enum LsaOperationalMode
    {
        PasswordProtected = 0x1,
        IndividualAccounts = 0x2,
        MandatoryAccess = 0x4,
        LogFull = 0x8
    }

    public enum PolicyDomainInformationClass
    {
        PolicyDomainEfsInformation = 2,
        PolicyDomainKerberosTicketInformation
    }

    public enum PolicyInformationClass
    {
        PolicyAuditLogInformation = 1,
        PolicyAuditEventsInformation,
        PolicyPrimaryDomainInformation,
        PolicyPdAccountInformation,
        PolicyAccountDomainInformation,
        PolicyLsaServerRoleInformation,
        PolicyReplicaSourceInformation,
        PolicyDefaultQuotaInformation,
        PolicyModificationInformation,
        PolicyAuditFullSetInformation,
        PolicyAuditFullQueryInformation,
        PolicyDnsDomainInformation,
        PolicyDnsDomainInformationInt
    }

    public enum PolicyNotificationInformationClass
    {
        PolicyNotifyAuditEventsInformation = 1,
        PolicyNotifyAccountDomainInformation,
        PolicyNotifyServerRoleInformation,
        PolicyNotifyDnsDomainInformation,
        PolicyNotifyDomainEfsInformation,
        PolicyNotifyDomainKerberosTicketInformation,
        PolicyNotifyMachineAccountPasswordInformation
    }

    public enum SecurityLogonType
    {
        Interactive = 2,
        Network,
        Batch,
        Service,
        Proxy,
        Unlock,
        NetworkCleartext,
        NewCredentials,
        RemoteInteractive,
        CachedInteractive,
        CachedRemoteInteractive,
        CachedUnlock
    }

    [Flags]
    public enum SecuritySystemAccess : int
    {
        Interactive = 0x1,
        Network = 0x2,
        Batch = 0x4,
        Service = 0x10,
        Proxy = 0x20,
        DenyInteractive = 0x40,
        DenyNetwork = 0x80,
        DenyBatch = 0x100,
        DenyService = 0x200,
        RemoteInteractive = 0x400,
        DenyRemoteInteractive = 0x800
    }

    public enum TrustedInformationClass
    {
        TrustedDomainNameInformation = 1,
        TrustedControllersInformation,
        TrustedPosixOffsetInformation,
        TrustedPasswordInformation,
        TrustedDomainInformationBasic,
        TrustedDomainInformationEx,
        TrustedDomainAuthInformation,
        TrustedDomainFullInformation,
        TrustedDomainAuthInformationInternal,
        TrustedDomainFullInformationInternal,
        TrustedDomainInformationEx2Internal,
        TrustedDomainFullInformation2Internal
    }
}
