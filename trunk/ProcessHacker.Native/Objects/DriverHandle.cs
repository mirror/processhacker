using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    public class DriverHandle : NativeHandle
    {
        public DriverHandle(string name)
            : this(name, 0, null)
        { }

        public DriverHandle(string name, ObjectFlags objectFlags, DirectoryHandle rootDirectory)
        {
            ObjectAttributes oa = new ObjectAttributes(name, objectFlags, rootDirectory);

            try
            {
                this.Handle = KProcessHacker.Instance.KphOpenDriver(oa).ToIntPtr();
            }
            finally
            {
                oa.Dispose();
            }
        }

        public DriverBasicInformation GetBasicInformation()
        {
            unsafe
            {
                DriverBasicInformation basicInfo;
                int retLength;

                KProcessHacker.Instance.KphQueryInformationDriver(
                    this,
                    DriverInformationClass.DriverBasicInformation,
                    new IntPtr(&basicInfo),
                    Marshal.SizeOf(typeof(DriverBasicInformation)),
                    out retLength
                    );

                return basicInfo;
            }
        }

        public string GetDriverName()
        {
            return this.GetInformationUnicodeString(DriverInformationClass.DriverNameInformation);
        }

        private string GetInformationUnicodeString(DriverInformationClass infoClass)
        {
            using (MemoryAlloc data = new MemoryAlloc(0x1000))
            {
                int retLength = 0;

                try
                {
                    KProcessHacker.Instance.KphQueryInformationDriver(
                        this,
                        infoClass,
                        data,
                        data.Size,
                        out retLength
                        );
                }
                catch (WindowsException)
                {
                    data.Resize(retLength);

                    KProcessHacker.Instance.KphQueryInformationDriver(
                        this,
                        infoClass,
                        data,
                        data.Size,
                        out retLength
                        );
                }

                return data.ReadStruct<UnicodeString>().Read();
            }
        }

        public string GetServiceKeyName()
        {
            return this.GetInformationUnicodeString(DriverInformationClass.DriverServiceKeyNameInformation);
        }
    }
}
