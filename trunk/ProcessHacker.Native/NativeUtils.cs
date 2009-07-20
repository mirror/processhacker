using System;
using ProcessHacker.Native.Objects;

namespace ProcessHacker.Native
{
    /// <summary>
    /// Provides various utility methods.
    /// </summary>
    public static class NativeUtils
    {
        /// <summary>
        /// Calls a function.
        /// </summary>
        /// <param name="address">The address of the function.</param>
        /// <param name="param1">The first parameter to pass.</param>
        /// <param name="param2">The second parameter to pass.</param>
        /// <param name="param3">The third parameter to pass.</param>
        public static void Call(IntPtr address, IntPtr param1, IntPtr param2, IntPtr param3)
        {
            // Queue a user-mode APC to the current thread.
            ThreadHandle.Current.QueueApc(address, param1, param2, param3);
            // Flush the APC queue.
            ThreadHandle.TestAlert();
        }

        public static string FormatNativeKeyName(string nativeKeyName)
        {
            const string hklmString = "\\registry\\machine";
            const string hkcrString = "\\registry\\machine\\software\\classes";
            string hkcuString = "\\registry\\user\\" +
                System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString().ToLower();
            string hkcucrString = "\\registry\\user\\" +
                System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString().ToLower() + "_classes";
            const string hkuString = "\\registry\\user";

            if (nativeKeyName.ToLower().StartsWith(hkcrString))
                return "HKCR" + nativeKeyName.Substring(hkcrString.Length);
            else if (nativeKeyName.ToLower().StartsWith(hklmString))
                return "HKLM" + nativeKeyName.Substring(hklmString.Length);
            else if (nativeKeyName.ToLower().StartsWith(hkcucrString))
                return "HKCU\\Software\\Classes" + nativeKeyName.Substring(hkcucrString.Length);
            else if (nativeKeyName.ToLower().StartsWith(hkcuString))
                return "HKCU" + nativeKeyName.Substring(hkcuString.Length);
            else if (nativeKeyName.ToLower().StartsWith(hkuString))
                return "HKU" + nativeKeyName.Substring(hkuString.Length);
            else
                return nativeKeyName;
        }
    }
}
