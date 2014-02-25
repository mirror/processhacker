using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Objects
{
    public sealed class TypeObjectHandle : NativeHandle<TypeObjectAccess>
    {
        public TypeObjectHandle(string name)
            : this(name, 0, null)
        { }

        public TypeObjectHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);

            try
            {
                this.Handle = KProcessHacker.Instance.KphOpenType(oa).ToIntPtr();
            }
            finally
            {
                oa.Dispose();
            }
        }
    }
}
