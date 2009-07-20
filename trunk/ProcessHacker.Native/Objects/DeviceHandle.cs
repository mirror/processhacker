using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    public class DeviceHandle : NativeHandle
    {
        public DeviceHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);

            try
            {
                this.Handle = KProcessHacker.Instance.KphOpenDevice(oa).ToIntPtr();
            }
            finally
            {
                oa.Dispose();
            }
        }
    }
}
