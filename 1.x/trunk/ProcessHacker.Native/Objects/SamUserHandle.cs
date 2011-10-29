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
            IntPtr handle;

            UnicodeString nameStr = new UnicodeString(name);

            try
            {
                Win32.SamCreateUserInDomain(
                    domainHandle,
                    ref nameStr,
                    access,
                    out handle,
                    out userId
                    ).ThrowIf();
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
            IntPtr handle;

            Win32.SamOpenUser(
                domainHandle,
                access,
                userId,
                out handle
                ).ThrowIf();

            this.Handle = handle;
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            UnicodeString oldPasswordStr = new UnicodeString(oldPassword);
            UnicodeString newPasswordStr = new UnicodeString(newPassword);

            try
            {
                Win32.SamChangePasswordUser(
                    this,
                    ref oldPasswordStr,
                    ref newPasswordStr
                    ).ThrowIf();
            }
            finally
            {
                oldPasswordStr.Dispose();
                newPasswordStr.Dispose();
            }
        }

        public void Delete()
        {
            Win32.SamDeleteUser(this).ThrowIf();
        }

        public string AdminComment
        {
            get { return this.Information.AdminComment; }
        }

        public UserAccountFlags Flags
        {
            get { return this.Information.UserFlags; }
        }

        public string FullName
        {
            get
            {
                using (SamMemoryAlloc data = this.GetInformation(UserInformationClass.UserFullNameInformation))
                {
                    return data.ReadStruct<UserFullNameInformation>().FullName.Text;
                }
            }
        }

        public int[] Groups
        {
            get
            {
                IntPtr groups;
                int count;

                Win32.SamGetGroupsForUser(
                    this,
                    out groups,
                    out count
                    ).ThrowIf();

                using (SamMemoryAlloc groupsAlloc = new SamMemoryAlloc(groups))
                {
                    return groupsAlloc.ReadInt32Array(0, count);
                }
            }
        }

        public SamUserInformation Information
        {
            get
            {
                using (SamMemoryAlloc data = this.GetInformation(UserInformationClass.UserAllInformation))
                {
                    UserAllInformation info = data.ReadStruct<UserAllInformation>();

                    return new SamUserInformation(
                        SamDomainHandle.ToDateTime(info.LastLogon),
                        SamDomainHandle.ToDateTime(info.LastLogoff),
                        SamDomainHandle.ToDateTime(info.PasswordLastSet),
                        SamDomainHandle.ToDateTime(info.AccountExpires),
                        SamDomainHandle.ToDateTime(info.PasswordCanChange),
                        SamDomainHandle.ToDateTime(info.PasswordMustChange),
                        info.UserName.Text,
                        info.FullName.Text,
                        info.AdminComment.Text,
                        info.UserComment.Text,
                        info.UserId,
                        info.PrimaryGroupId,
                        info.UserAccountControl,
                        info.PasswordExpired
                        );
                }
            }
        }

        private SamMemoryAlloc GetInformation(UserInformationClass infoClass)
        {
            IntPtr buffer;

            Win32.SamQueryInformationUser(
                this,
                infoClass,
                out buffer
                ).ThrowIf();

            return new SamMemoryAlloc(buffer);
        }

        public string Name
        {
            get
            {
                using (SamMemoryAlloc data = this.GetInformation(UserInformationClass.UserAccountNameInformation))
                {
                    return data.ReadStruct<UserAccountNameInformation>().UserName.Text;
                }
            }
        }

        public string PasswordHint
        {
            get
            {
                using (SamMemoryAlloc data = this.GetInformation(UserInformationClass.UserExtendedInformation))
                {
                    return data.ReadStruct<UserExtendedInformation>().PasswordHint.Text;
                }
            }
        }

        public unsafe void SetAdminComment(string comment)
        {
            UserAllInformation info = new UserAllInformation
            {
                WhichFields = UserWhichFields.AdminComment, 
                AdminComment = new UnicodeString(comment)
            };

            try
            {
                this.SetInformation(UserInformationClass.UserAllInformation, new IntPtr(&info));
            }
            finally
            {
                info.AdminComment.Dispose();
            }
        }

        public unsafe void SetFlags(UserAccountFlags flags)
        {
            UserAllInformation info = new UserAllInformation
            {
                WhichFields = UserWhichFields.UserAccountControl, 
                UserAccountControl = flags
            };

            this.SetInformation(UserInformationClass.UserAllInformation, new IntPtr(&info));
        }

        public unsafe void SetFullName(string fullName)
        {
            UserFullNameInformation info = new UserFullNameInformation
            {
                FullName = new UnicodeString(fullName)
            };

            try
            {
                this.SetInformation(UserInformationClass.UserFullNameInformation, new IntPtr(&info));
            }
            finally
            {
                info.FullName.Dispose();
            }
        }

        private void SetInformation(UserInformationClass infoClass, IntPtr buffer)
        {
            Win32.SamSetInformationUser(
                this,
                infoClass,
                buffer
                ).ThrowIf();
        }

        public unsafe void SetPassword(string password, bool expired)
        {
            UserSetPasswordInformation info = new UserSetPasswordInformation
            {
                Password = new UnicodeString(password), 
                PasswordExpired = expired
            };

            try
            {
                this.SetInformation(UserInformationClass.UserSetPasswordInformation, new IntPtr(&info));
            }
            finally
            {
                info.Password.Dispose();
            }
        }

        public unsafe void SetPasswordHint(string passwordHint)
        {
            UserExtendedInformation info = new UserExtendedInformation
            {
                ExtendedWhichFields = UserExtendedWhichFields.PasswordHint, 
                PasswordHint = new UnicodeString(passwordHint)
            };

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

    public class SamUserInformation
    {
        private readonly DateTime _lastLogon;
        private readonly DateTime _lastLogoff;
        private readonly DateTime _passwordLastSet;
        private readonly DateTime _accountExpires;
        private readonly DateTime _passwordCanChange;
        private readonly DateTime _passwordMustChange;
        private readonly string _userName;
        private readonly string _fullName;
        private readonly string _adminComment;
        private readonly string _userComment;
        private readonly int _userId;
        private readonly int _primaryGroupId;
        private readonly UserAccountFlags _userFlags;
        private readonly bool _passwordExpired;

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
