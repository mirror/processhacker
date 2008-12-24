/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        public class TokenWithLinkedToken : Win32.IWithToken
        {
            private Win32.TokenHandle _token;

            public TokenWithLinkedToken(Win32.TokenHandle token)
            {
                _token = token;
            }

            public Win32.TokenHandle GetToken()
            {
                int linkedToken;
                int retLen;

                if (!Win32.GetTokenInformation(_token, Win32.TOKEN_INFORMATION_CLASS.TokenLinkedToken,
                    out linkedToken, 4, out retLen))
                    throw new Exception(Win32.GetLastErrorMessage());

                return new Win32.TokenHandle(linkedToken, true);
            }

            public Win32.TokenHandle GetToken(Win32.TOKEN_RIGHTS access)
            {
                throw new NotSupportedException();
            }
        }
    }
}
