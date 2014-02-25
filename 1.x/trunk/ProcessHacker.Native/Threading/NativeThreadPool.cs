using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native.Threading
{
    public delegate void RegisterWaitCallback(object argument, bool timeout);

    public static class NativeThreadPool
    {
        public static void QueueWorkItem(Action<object> work, object argument)
        {
            Win32.RtlQueueWorkItem((context) => work(argument), IntPtr.Zero, WtFlags.ExecuteDefault).ThrowIf(); 
        }

        public static IntPtr RegisterWait(IntPtr handle, RegisterWaitCallback callback, object argument, int timeoutMilliseconds)
        {
            IntPtr waitHandle;

            Win32.RtlRegisterWait(
                out waitHandle,
                handle,
                (context, timeout) => callback(argument, timeout),
                IntPtr.Zero,
                timeoutMilliseconds,
                WtFlags.ExecuteDefault
                ).ThrowIf();

            return waitHandle;
        }

        public static void UnregisterWait(IntPtr waitHandle)
        {
            Win32.RtlDeregisterWait(waitHandle).ThrowIf();
        }
    }
}
