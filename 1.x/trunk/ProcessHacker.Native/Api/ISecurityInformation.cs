/*
 * Process Hacker - 
 *   ISecurityInformation definition
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
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Api
{
    [ComImport, Guid("965fc360-16ff-11d0-91cb-00aa00bbb723"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISecurityInformation
    {
        [PreserveSig]
        HResult GetObjectInformation(
            [Out] out SiObjectInfo ObjectInfo
            );

        [PreserveSig]
        HResult GetSecurity(
            [In] SecurityInformation RequestedInformation,
            [Out] out IntPtr SecurityDescriptor,
            [In] bool Default
            );

        [PreserveSig]
        HResult SetSecurity(
            [In] SecurityInformation SecurityInformation,
            [In] IntPtr SecurityDescriptor
            );

        [PreserveSig]
        HResult GetAccessRights(
            [In] ref Guid ObjectType,
            [In] SiObjectInfoFlags Flags,
            [Out] out IntPtr Access,
            [Out] out int Accesses,
            [Out] out int DefaultAccess
            );

        [PreserveSig]
        HResult MapGeneric(
            [In] ref Guid ObjectType,
            [In] ref AceFlags AceFlags,
            [In] ref int Mask
            );

        [PreserveSig]
        HResult GetInheritTypes(
            [Out] out IntPtr InheritTypes,
            [Out] out int InheritTypesCount
            );

        [PreserveSig]
        HResult PropertySheetPageCallback(
            [In] IntPtr hWnd,
            [In] SiCallbackMessage Msg,
            [In] SiPageType Page
            );
    }
}
