/*
 * Process Hacker - 
 *   SAM user handle
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
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    /// <summary>
    /// Represents a handle to a SAM user.
    /// </summary>
    public sealed class SamUserHandle : SamHandle<SamUserAccess>
    {
        public static SamUserHandle Create(SamUserAccess access, SamDomainHandle domainHandle, string name, out int userId)
        {
            NtStatus status;
            UnicodeString nameStr;
            IntPtr handle;

            nameStr = new UnicodeString(name);

            try
            {
                if ((status = Win32.SamCreateUserInDomain(
                    domainHandle,
                    ref nameStr,
                    access,
                    out handle,
                    out userId
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                nameStr.Dispose();
            }

            return new SamUserHandle(handle, true);
        }

        public static SamUserHandle Open(string name, SamUserAccess access)
        {
            return Open(Sid.FromName(name), access);
        }

        public static SamUserHandle Open(Sid sid, SamUserAccess access)
        {
            using (var dhandle = new SamDomainHandle(sid.DomainName, SamDomainAccess.Lookup))
            {
                return new SamUserHandle(dhandle, dhandle.LookupName(sid.Name), access);
            }
        }

        private SamUserHandle(IntPtr handle, bool owned)
            : base(handle, owned)
        { }

        public SamUserHandle(SamDomainHandle domainHandle, string name, SamUserAccess access)
            : this(domainHandle, domainHandle.LookupName(name), access)
        { }

        /// <summary>
        /// Opens a SAM user.
        /// </summary>
        /// <param name="domainHandle">A handle to a SAM domain.</param>
        /// <param name="userId">The relative ID of the user to open.</param>
        /// <param name="access">The desired access to the user.</param>
        public SamUserHandle(SamDomainHandle domainHandle, int userId, SamUserAccess access)
        {
            NtStatus status;
            IntPtr handle;

            if ((status = Win32.SamOpenUser(
                domainHandle,
                access,
                userId,
                out handle
                )) >= NtStatus.Error)
                Win32.Throw(status);

            this.Handle = handle;
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            NtStatus status;
            UnicodeString oldPasswordStr;
            UnicodeString newPasswordStr;

            oldPasswordStr = new UnicodeString(oldPassword);
            newPasswordStr = new UnicodeString(newPassword);

            try
            {
                if ((status = Win32.SamChangePasswordUser(
                    this,
                    ref oldPasswordStr,
                    ref newPasswordStr
                    )) >= NtStatus.Error)
                    Win32.Throw(status);
            }
            finally
            {
                oldPasswordStr.Dispose();
                newPasswordStr.Dispose();
            }
        }

        public void Delete()
        {
            NtStatus status;

            if ((status = Win32.SamDeleteUser(this)) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public string GetAdminComment()
        {
            return this.GetInformation().AdminComment;
        }

        public UserAccountFlags GetFlags()
        {
            return this.GetInformation().UserFlags;
        }

        public string GetFullName()
        {
            using (var data = this.GetInformation(UserInformationClass.UserFullNameInformation))
                return data.ReadStruct<UserFullNameInformation>().FullName.Read();
        }

        public int[] GetGroups()
        {
            NtStatus status;
            IntPtr groups;
            int count;

            if ((status = Win32.SamGetGroupsForUser(
                this,
                out groups,
                out count
                )) >= NtStatus.Error)
                Win32.Throw(status);

            using (var groupsAlloc = new SamMemoryAlloc(groups))
            {
                return groupsAlloc.ReadInt32Array(0, count);
            }
        }

        public SamUserInformation GetInformation()
        {
            using (var data = this.GetInformation(UserInformationClass.UserAllInformation))
            {
                UserAllInformation info = data.ReadStruct<UserAllInformation>();

                return new SamUserInformation(
                    SamDomainHandle.ToDateTime(info.LastLogon),
                    SamDomainHandle.ToDateTime(info.LastLogoff),
                    SamDomainHandle.ToDateTime(info.PasswordLastSet),
                    SamDomainHandle.ToDateTime(info.AccountExpires),
                    SamDomainHandle.ToDateTime(info.PasswordCanChange),
                    SamDomainHandle.ToDateTime(info.PasswordMustChange),
                    info.UserName.Read(),
                    info.FullName.Read(),
                    info.AdminComment.Read(),
                    info.UserComment.Read(),
                    info.UserId,
                    info.PrimaryGroupId,
                    info.UserAccountControl,
                    info.PasswordExpired
                    );
            }
        }

        private SamMemoryAlloc GetInformation(UserInformationClass infoClass)
        {
            NtStatus status;
            IntPtr buffer;

            if ((status = Win32.SamQueryInformationUser(
                this,
                infoClass,
                out buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new SamMemoryAlloc(buffer);
        }

        public string GetName()
        {
            using (var data = this.GetInformation(UserInformationClass.UserAccountNameInformation))
                return data.ReadStruct<UserAccountNameInformation>().UserName.Read();
        }

        public string GetPasswordHint()
        {
            using (var data = this.GetInformation(UserInformationClass.UserExtendedInformation))
                return data.ReadStruct<UserExtendedInformation>().PasswordHint.Read();
        }

        public void SetAdminComment(string comment)
        {
            unsafe
            {
                UserAllInformation info = new UserAllInformation();

                info.WhichFields = UserWhichFields.AdminComment;
                info.AdminComment = new UnicodeString(comment);

                try
                {
                    this.SetInformation(UserInformationClass.UserAllInformation, new IntPtr(&info));
                }
                finally
                {
                    info.AdminComment.Dispose();
                }
            }
        }

        public void SetFlags(UserAccountFlags flags)
        {
            unsafe
            {
                UserAllInformation info = new UserAllInformation();

                info.WhichFields = UserWhichFields.UserAccountControl;
                info.UserAccountControl = flags;

                this.SetInformation(UserInformationClass.UserAllInformation, new IntPtr(&info));
            }
        }

        public void SetFullName(string fullName)
        {
            unsafe
            {
                UserFullNameInformation info = new UserFullNameInformation();

                info.FullName = new UnicodeString(fullName);

                try
                {
                    this.SetInformation(UserInformationClass.UserFullNameInformation, new IntPtr(&info));
                }
                finally
                {
                    info.FullName.Dispose();
                }
            }
        }

        private void SetInformation(UserInformationClass infoClass, IntPtr buffer)
        {
            NtStatus status;

            if ((status = Win32.SamSetInformationUser(
                this,
                infoClass,
                buffer
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public void SetPassword(string password, bool expired)
        {
            unsafe
            {
                UserSetPasswordInformation info = new UserSetPasswordInformation();

                info.Password = new UnicodeString(password);
                info.PasswordExpired = expired;

                try
                {
                    this.SetInformation(UserInformationClass.UserSetPasswordInformation, new IntPtr(&info));
                }
                finally
                {
                    info.Password.Dispose();
                }
            }
        }

        public void SetPasswordHint(string passwordHint)
        {
            unsafe
            {
                UserExtendedInformation info = new UserExtendedInformation();

                info.ExtendedWhichFields = UserExtendedWhichFields.PasswordHint;
                info.PasswordHint = new UnicodeString(passwordHint);

                try
                {
                    this.SetInformation(UserInformationClass.UserExtendedInformation, new IntPtr(&info));
                }
                finally
                {
                    info.PasswordHint.Dispose();
                }
            }
        }
    }

    public class SamUserInformation
    {
        private DateTime _lastLogon;
        private DateTime _lastLogoff;
        private DateTime _passwordLastSet;
        private DateTime _accountExpires;
        private DateTime _passwordCanChange;
        private DateTime _passwordMustChange;
        private string _userName;
        private string _fullName;
        private string _adminComment;
        private string _userComment;
        private int _userId;
        private int _primaryGroupId;
        private UserAccountFlags _userFlags;
        private bool _passwordExpired;

        public SamUserInformation(
            DateTime lastLogon,
            DateTime lastLogoff,
            DateTime passwordLastSet,
            DateTime accountExpires,
            DateTime passwordCanChange,
            DateTime passwordMustChange,
            string userName,
            string fullName,
            string adminComment,
            string userComment,
            int userId,
            int primaryGroupId,
            UserAccountFlags userFlags,
            bool passwordExpired
            )
        {
            _lastLogon = lastLogon;
            _lastLogoff = lastLogoff;
            _passwordLastSet = passwordLastSet;
            _accountExpires = accountExpires;
            _passwordCanChange = passwordCanChange;
            _passwordMustChange = passwordMustChange;
            _userName = userName;
            _fullName = fullName;
            _adminComment = adminComment;
            _userComment = userComment;
            _userId = userId;
            _primaryGroupId = primaryGroupId;
            _userFlags = userFlags;
            _passwordExpired = passwordExpired;
        }

        public DateTime LastLogon { get { return _lastLogon; } }
        public DateTime LastLogoff { get { return _lastLogoff; } }
        public DateTime PasswordLastSet { get { return _passwordLastSet; } }
        public DateTime AccountExpires { get { return _accountExpires; } }
        public DateTime PasswordCanChange { get { return _passwordCanChange; } }
        public DateTime PasswordMustChange { get { return _passwordMustChange; } }
        public string UserName { get { return _userName; } }
        public string FullName { get { return _fullName; } }
        public string AdminComment { get { return _adminComment; } }
        public string UserComment { get { return _userComment; } }
        public int UserId { get { return _userId; } }
        public int PrimaryGroupId { get { return _primaryGroupId; } }
        public UserAccountFlags UserFlags { get { return _userFlags; } }
        public bool PasswordExpired { get { return _passwordExpired; } }
    }
}
