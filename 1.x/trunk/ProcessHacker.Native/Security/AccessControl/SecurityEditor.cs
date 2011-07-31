/*
 * Process Hacker - 
 *   ISecurityInformation implementation
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
using System.Windows.Forms;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Security.AccessControl
{
    public class SecurityEditor : IDisposable, ISecurityInformation
    {
        private class SecurableObjectWrapper : ISecurable
        {
            private Func<StandardRights, NativeHandle> _openMethod;

            public SecurableObjectWrapper(Func<StandardRights, NativeHandle> openMethod)
            {
                _openMethod = openMethod;
            }

            public SecurityDescriptor GetSecurity(SecurityInformation securityInformation)
            {
                using (var handle = _openMethod(
                    StandardRights.ReadControl |
                    ((securityInformation & SecurityInformation.Sacl) != 0 ? StandardRights.AccessSystemSecurity : 0)
                    ))
                    return handle.GetSecurity(securityInformation);
            }

            public void SetSecurity(SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
            {
                using (var handle = _openMethod(
                    ((securityInformation & SecurityInformation.Dacl) != 0 ? StandardRights.WriteDac : 0) |
                    ((securityInformation & SecurityInformation.Owner) != 0 ? StandardRights.WriteOwner : 0) |
                    ((securityInformation & SecurityInformation.Sacl) != 0 ? StandardRights.AccessSystemSecurity : 0)
                    ))
                {
                    handle.SetSecurity(securityInformation, securityDescriptor);
                }
            }
        }

        private class SeSecurableObjectWrapper : ISecurable
        {
            private SeObjectType _objectType;
            private Func<StandardRights, NativeHandle> _openMethod;

            public SeSecurableObjectWrapper(SeObjectType objectType, Func<StandardRights, NativeHandle> openMethod)
            {
                _objectType = objectType;
                _openMethod = openMethod;
            }

            public SecurityDescriptor GetSecurity(SecurityInformation securityInformation)
            {
                using (var handle = _openMethod(
                    StandardRights.ReadControl |
                    ((securityInformation & SecurityInformation.Sacl) != 0 ? StandardRights.AccessSystemSecurity : 0)
                    ))
                    return SecurityDescriptor.GetSecurity(handle, _objectType, securityInformation); 
            }

            public void SetSecurity(SecurityInformation securityInformation, SecurityDescriptor securityDescriptor)
            {
                using (var handle = _openMethod(
                    ((securityInformation & SecurityInformation.Dacl) != 0 ? StandardRights.WriteDac : 0) |
                    ((securityInformation & SecurityInformation.Owner) != 0 ? StandardRights.WriteOwner : 0) |
                    ((securityInformation & SecurityInformation.Sacl) != 0 ? StandardRights.AccessSystemSecurity : 0)
                    ))
                {
                    SecurityDescriptor.SetSecurity(handle, _objectType, securityInformation, securityDescriptor);
                }
            }
        }

        public static void EditSecurity(IWin32Window owner, ISecurable securable, string name, IEnumerable<AccessEntry> accessEntries)
        {
            using (var osi = new SecurityEditor(securable, name, accessEntries))
                Win32.EditSecurity(owner != null ? owner.Handle : IntPtr.Zero, osi);
        }

        public static ISecurable GetSecurableWrapper(IntPtr handle)
        {
            return GetSecurableWrapper((access) => new NativeHandle<StandardRights>(handle, access));
        }

        public static ISecurable GetSecurableWrapper(SeObjectType objectType, IntPtr handle)
        {
            return GetSecurableWrapper(objectType, (access) => new NativeHandle<StandardRights>(handle, access));
        }

        public static ISecurable GetSecurableWrapper(Func<StandardRights, NativeHandle> openMethod)
        {
            return new SecurableObjectWrapper(openMethod);
        }

        public static ISecurable GetSecurableWrapper(SeObjectType objectType, Func<StandardRights, NativeHandle> openMethod)
        {
            return new SeSecurableObjectWrapper(objectType, openMethod);
        }

        private bool _disposed = false;
        private ISecurable _securable;
        private List<MemoryAlloc> _pool = new List<MemoryAlloc>();
        private string _name;
        private MemoryAlloc _accessRights;
        private int _accessRightCount;

        internal SecurityEditor(ISecurable securable, string name, IEnumerable<AccessEntry> accessEntries)
        {
            List<SiAccess> accesses;

            _securable = securable;
            _name = name;

            accesses = new List<SiAccess>();

            foreach (var entry in accessEntries)
            {
                if (entry.Mask != 0)
                {
                    accesses.Add(new SiAccess()
                    {
                        Guid = IntPtr.Zero,
                        Mask = entry.Mask,
                        Flags = (entry.General ? SiAccessFlags.General : 0) | (entry.Specific ? SiAccessFlags.Specific : 0),
                        Name = this.AllocateStringFromPool(entry.Name)
                    });
                }
            }

            _accessRights = this.AllocateStructArray<SiAccess>(accesses.ToArray());
            _accessRightCount = accesses.Count;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _pool.ForEach((alloc) => alloc.Dispose());
                _pool.Clear();
                _disposed = true;
            }
        }

        private MemoryAlloc AllocateArray(IntPtr[] value)
        {
            MemoryAlloc alloc = new MemoryAlloc(IntPtr.Size * value.Length);

            for (int i = 0; i < value.Length; i++)
                alloc.WriteIntPtr(i * IntPtr.Size, value[i]);

            return alloc;
        }

        private MemoryAlloc AllocateArrayFromPool(IntPtr[] value)
        {
            MemoryAlloc m = this.AllocateArray(value);
            _pool.Add(m);
            return m;
        }

        private MemoryAlloc AllocateString(string value)
        {
            MemoryAlloc alloc = new MemoryAlloc((value.Length + 1) * 2);

            alloc.WriteUnicodeString(0, value);
            alloc.WriteInt16(value.Length * 2, 0);

            return alloc;
        }

        private MemoryAlloc AllocateStringFromPool(string value)
        {
            MemoryAlloc m = this.AllocateString(value);
            _pool.Add(m);
            return m;
        }

        private MemoryAlloc AllocateStruct<T>(T value)
            where T : struct
        {
            MemoryAlloc alloc = new MemoryAlloc(Marshal.SizeOf(typeof(T)));

            alloc.WriteStruct<T>(0, value);

            return alloc;
        }

        private MemoryAlloc AllocateStructFromPool<T>(T value)
            where T : struct
        {
            MemoryAlloc m = this.AllocateStruct<T>(value);
            _pool.Add(m);
            return m;
        }

        private MemoryAlloc AllocateStructArray<T>(T[] value)
            where T : struct
        {
            MemoryAlloc alloc = new MemoryAlloc(Marshal.SizeOf(typeof(T)) * value.Length);

            for (int i = 0; i < value.Length; i++)
                alloc.WriteStruct<T>(i, value[i]);

            return alloc;
        }

        private MemoryAlloc AllocateStructArrayFromPool<T>(T[] value)
            where T : struct
        {
            MemoryAlloc m = this.AllocateStructArray<T>(value);
            _pool.Add(m);
            return m;
        }

        #region ISecurityInformation Members

        public HResult GetObjectInformation(out SiObjectInfo ObjectInfo)
        {
            SiObjectInfo soi = new SiObjectInfo();

            soi.Flags =
                SiObjectInfoFlags.EditAudits |
                SiObjectInfoFlags.EditOwner |
                SiObjectInfoFlags.EditPerms |
                SiObjectInfoFlags.Advanced |
                SiObjectInfoFlags.NoAclProtect |
                SiObjectInfoFlags.NoTreeApply;
            soi.Instance = IntPtr.Zero;
            soi.ObjectName = this.AllocateStringFromPool(_name);
            ObjectInfo = soi;

            return HResult.OK;
        }

        public HResult GetSecurity(SecurityInformation RequestedInformation, out IntPtr SecurityDescriptor, bool Default)
        {
            try
            {
                using (var sd = _securable.GetSecurity(RequestedInformation))
                {
                    // Since the ACL editor will free the security descriptor using 
                    // LocalFree, we need to use a local memory allocation and copy 
                    // the security descriptor into it.
                    using (var localAlloc = new LocalMemoryAlloc(sd.Length))
                    {
                        localAlloc.WriteMemory(0, sd.Memory, sd.Length);
                        localAlloc.Reference(); // reference for ACL editor
                        SecurityDescriptor = localAlloc;
                    }
                }
            }
            catch (WindowsException ex)
            {
                SecurityDescriptor = IntPtr.Zero;

                return ex.ErrorCode.GetHResult();
            }

            return HResult.OK;
        }

        public HResult SetSecurity(SecurityInformation SecurityInformation, IntPtr SecurityDescriptor)
        {
            try
            {
                _securable.SetSecurity(
                    SecurityInformation, 
                    new SecurityDescriptor(new MemoryRegion(SecurityDescriptor))
                    );
            }
            catch (WindowsException ex)
            {
                return ex.ErrorCode.GetHResult();
            }

            return HResult.OK;
        }

        public HResult GetAccessRights(ref Guid ObjectType, SiObjectInfoFlags Flags, out IntPtr Access, out int Accesses, out int DefaultAccess)
        {
            Access = _accessRights;
            Accesses = _accessRightCount;
            DefaultAccess = 0;

            return HResult.OK;
        }

        public HResult MapGeneric(ref Guid ObjectType, ref AceFlags AceFlags, ref int Mask)
        {
            return HResult.OK;
        }

        public HResult GetInheritTypes(out IntPtr InheritTypes, out int InheritTypesCount)
        {
            InheritTypes = IntPtr.Zero;
            InheritTypesCount = 0;

            return HResult.Fail;
        }

        public HResult PropertySheetPageCallback(IntPtr hWnd, SiCallbackMessage Msg, SiPageType Page)
        {
            return HResult.OK;
        }

        #endregion
    }

    public struct AccessEntry
    {
        private bool _general;
        private int _mask;
        private string _name;
        private bool _specific;

        public AccessEntry(string name, object mask, bool general, bool specific)
        {
            _name = name;
            _mask = Convert.ToInt32(mask);
            _general = general;
            _specific = specific;
        }

        public bool General
        {
            get { return _general; }
        }

        public int Mask
        {
            get { return _mask; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Specific
        {
            get { return _specific; }
        }
    }
}
