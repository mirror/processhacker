using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security.AccessControl;

namespace ProcessHacker.Native.Security
{
    public class ObjectSecurityInformation : ISecurityInformation
    {
        private List<MemoryAlloc> _pool = new List<MemoryAlloc>();
        private MemoryAlloc _accessRights;

        public ObjectSecurityInformation()
        {
            _accessRights = this.AllocateStructArray<SiAccess>(
                new SiAccess[]
                {
                    new SiAccess { Flags = SiAccessFlags.Specific, Name = this.AllocateStringFromPool("test"), Mask = 0x1 }
                });
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
        {
            MemoryAlloc alloc = new MemoryAlloc(Marshal.SizeOf(typeof(T)));

            alloc.WriteStruct<T>(0, value);

            return alloc;
        }     

        private MemoryAlloc AllocateStructFromPool<T>(T value)
        {
            MemoryAlloc m = this.AllocateStruct<T>(value);
            _pool.Add(m);
            return m;
        }

        private MemoryAlloc AllocateStructArray<T>(T[] value)
        {
            MemoryAlloc alloc = new MemoryAlloc(Marshal.SizeOf(typeof(T)) * value.Length);

            for (int i = 0; i < value.Length; i++)
                alloc.WriteStruct<T>(i * Marshal.SizeOf(typeof(T)), value[i]);

            return alloc;
        }  

        private MemoryAlloc AllocateStructArrayFromPool<T>(T[] value)
        {
            MemoryAlloc m = this.AllocateStructArray<T>(value);
            _pool.Add(m);
            return m;
        }

        private void ClearPool()
        {
            _pool.ForEach((alloc) => alloc.Dispose());
            _pool.Clear();
        }

        #region ISecurityInformation Members

        public int GetObjectInformation(out SiObjectInfo ObjectInfo)
        {
            SiObjectInfo soi = new SiObjectInfo();

            soi.Flags =
                SiObjectInfoFlags.All |
                SiObjectInfoFlags.MayWrite;
            soi.Instance = Win32.GetModuleHandle(null);
            soi.ObjectName = this.AllocateStringFromPool("test");
            ObjectInfo = soi;

            return 0;
        }

        public int GetSecurity(SecurityInformation RequestedInformation, out IntPtr SecurityDescriptor, bool Default)
        {
            try
            {
                var sd = ProcessHandle.Current.GetSecurity();

                sd.Reference(); // prevent the memory from being freed
                SecurityDescriptor = sd;
            }
            catch (WindowsException ex)
            {
                SecurityDescriptor = IntPtr.Zero;

                return Win32.GetHR(ex.ErrorCode);
            }

            return 0;
        }

        public int SetSecurity(SecurityInformation SecurityInformation, IntPtr SecurityDescriptor)
        {
            try
            {
                ProcessHandle.Current.SetSecurity(
                    SecurityInformation, 
                    new AbsoluteSecurityDescriptor(SecurityDescriptor)
                    );
            }
            catch (WindowsException ex)
            {
                return Win32.GetHR(ex.ErrorCode);
            }

            return 0;
        }

        public int GetAccessRights(ref Guid ObjectType, SiObjectInfoFlags Flags, out IntPtr Access, out int Accesses, out int DefaultAccess)
        {
            Access = _accessRights;
            Accesses = 1;
            DefaultAccess = 0;

            return 0;
        }

        public int MapGeneric(ref Guid ObjectType, ref byte AceFlags, ref int Mask)
        {
            Mask = 0;
            return 0;
        }

        public int GetInheritTypes(out IntPtr InheritTypes, out int InheritTypesCount)
        {
            InheritTypes = IntPtr.Zero;
            InheritTypesCount = 0;

            return 1;
        }

        public int PropertySheetPageCallback(IntPtr hWnd, SiCallbackMessage Msg, SiPageType Page)
        {
            return 0;
        }

        #endregion
    }
}
