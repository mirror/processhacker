using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Api
{
    [ComImport, Guid("965fc360-16ff-11d0-91cb-00aa00bbb723"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISecurityInformation
    {
        int GetObjectInformation(
            [Out] out SiObjectInfo ObjectInfo
            );
        int GetSecurity(
            [In] SecurityInformation RequestedInformation,
            [Out] out IntPtr SecurityDescriptor,
            [In] bool Default
            );
        int SetSecurity(
            [In] SecurityInformation SecurityInformation,
            [In] IntPtr SecurityDescriptor
            );
        int GetAccessRights(
            [In] ref Guid ObjectType,
            [In] SiObjectInfoFlags Flags,
            [Out] out IntPtr Access,
            [Out] out int Accesses,
            [Out] out int DefaultAccess
            );
        int MapGeneric(
            [In] ref Guid ObjectType,
            [In] ref byte AceFlags,
            [In] ref int Mask
            );
        int GetInheritTypes(
            [Out] out IntPtr InheritTypes,
            [Out] out int InheritTypesCount
            );
        int PropertySheetPageCallback(
            [In] IntPtr hWnd,
            [In] SiCallbackMessage Msg,
            [In] SiPageType Page
            );
    }
}
