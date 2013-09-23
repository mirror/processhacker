/*
 * Process Hacker - 
 *   driver handle
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

namespace ProcessHacker.Native.Objects
{
    public sealed class DriverHandle : NativeHandle
    {
        public DriverHandle(string name)
            : this(name, 0, null)
        { }

        public DriverHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);

            try
            {
                this.Handle = IntPtr.Zero;// KProcessHacker.Instance.KphOpenDriver(oa).ToIntPtr();
            }
            finally
            {
                oa.Dispose();
            }
        }

        public string DriverName
        {
            get { return this.GetInformationUnicodeString(DriverInformationClass.DriverNameInformation); }
        }

        public string ServiceKeyName
        {
            get { return this.GetInformationUnicodeString(DriverInformationClass.DriverServiceKeyNameInformation); }
        }

        public unsafe DriverBasicInformation BasicInformation
        {
            get
            {
                DriverBasicInformation basicInfo = new DriverBasicInformation();

                //KProcessHacker.Instance.KphQueryInformationDriver(
                //    this,
                //    DriverInformationClass.DriverBasicInformation,
                //    new IntPtr(&basicInfo),
                //    DriverBasicInformation.SizeOf,
                //    out retLength
                //    );

                return basicInfo;
            }
        }

        private string GetInformationUnicodeString(DriverInformationClass infoClass)
        {
            using (MemoryAlloc data = new MemoryAlloc(0x1000))
            {
                //try
                //{
                //    KProcessHacker.Instance.KphQueryInformationDriver(
                //        this,
                //        infoClass,
                //        data,
                //        data.Size,
                //        out retLength
                //        );
                //}
                //catch (WindowsException)
                //{
                //    data.ResizeNew(retLength);

                //    KProcessHacker.Instance.KphQueryInformationDriver(
                //        this,
                //        infoClass,
                //        data,
                //        data.Size,
                //        out retLength
                //        );
                //}

                return data.ReadStruct<UnicodeString>().Text;
            }
        }
    }
}
