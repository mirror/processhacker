using System;
using System.Collections.Generic;
using System.Text;                  
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    public class NativeObject : IDisposable
    {
        public static void WaitAll(NativeObject[] objects)
        {
            NativeHandle.WaitAll(ObjectsToISync(objects));
        }

        public static void WaitAll(NativeObject[] objects, int timeout)
        {
            NativeHandle.WaitAll(ObjectsToISync(objects), timeout * Win32.TimeMsTo100Ns);
        }

        public static void WaitAll(NativeObject[] objects, DateTime timeout)
        {
            NativeHandle.WaitAll(ObjectsToISync(objects), false, timeout.ToFileTime(), false);
        }

        public static void WaitAny(NativeObject[] objects)
        {
            NativeHandle.WaitAny(ObjectsToISync(objects));
        }

        public static void WaitAny(NativeObject[] objects, int timeout)
        {
            NativeHandle.WaitAny(ObjectsToISync(objects), timeout * Win32.TimeMsTo100Ns);
        }

        public static void WaitAny(NativeObject[] objects, DateTime timeout)
        {
            NativeHandle.WaitAny(ObjectsToISync(objects), false, timeout.ToFileTime(), false);
        }

        private static ISynchronizable[] ObjectsToISync(NativeObject[] objects)
        {
            ISynchronizable[] newArray = new ISynchronizable[objects.Length];

            for (int i = 0; i < newArray.Length; i++)
                newArray[i] = objects[i].Handle;

            return newArray;
        }

        private NativeHandle _handle;

        /// <summary>
        /// Closes the reference to the object.
        /// </summary>
        public void Dispose()
        {
            _handle.Dispose();
        }

        /// <summary>
        /// Gets the underlying handle for the object.
        /// </summary>
        public NativeHandle Handle
        {
            get { return _handle; }
            protected set { _handle = value; }
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        public WaitStatus Wait()
        {
            return (WaitStatus)_handle.Wait();
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="timeout">A timeout value, in milliseconds.</param>
        public WaitStatus Wait(int timeout)
        {
            return (WaitStatus)_handle.Wait(timeout * Win32.TimeMsTo100Ns, true);
        }

        /// <summary>
        /// Waits for the object to be signaled.
        /// </summary>
        /// <param name="timeout">A time to wait until.</param>
        public WaitStatus Wait(DateTime timeout)
        {
            return (WaitStatus)_handle.Wait(timeout.ToFileTime(), false);
        }
    }

    public class NativeObject<THandle> : NativeObject
        where THandle : NativeHandle
    {
        protected new THandle Handle
        {
            get { return base.Handle as THandle; }
            set { base.Handle = value; }
        }
    }

    public enum WaitStatus : uint
    {
        Wait0 = 0x00000000,
        Wait1 = 0x00000001,
        Wait2 = 0x00000002,
        Wait3 = 0x00000003,
        Wait4 = 0x00000004,
        Wait5 = 0x00000005,
        Wait6 = 0x00000006,
        Wait7 = 0x00000007,
        Wait63 = 0x0000003f,
        Abandoned = 0x00000080,
        AbandonedWait0 = 0x00000080,
        AbandonedWait1 = 0x00000081,
        AbandonedWait2 = 0x00000082,
        AbandonedWait3 = 0x00000083,
        AbandonedWait4 = 0x00000084,
        AbandonedWait5 = 0x00000085,
        AbandonedWait6 = 0x00000086,
        AbandonedWait7 = 0x00000087,
        AbandonedWait63 = 0x000000bf,
        UserApc = 0x000000c0,
        KernelApc = 0x00000100,
        Alerted = 0x00000101,
        Timeout = 0x00000102
    }
}
