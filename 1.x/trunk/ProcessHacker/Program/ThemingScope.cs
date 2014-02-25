using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessHacker
{
    public static class ThemingScope
    {
        [DllImport("kernel32.dll")]
        private static extern bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);

        public static void Activate()
        {
            IntPtr zero = IntPtr.Zero;
            Assembly windowsForms = Assembly.GetAssembly(typeof(Control));

            // HACK
            IntPtr hActCtx = (IntPtr)windowsForms.GetType("System.Windows.Forms.UnsafeNativeMethods", true).
                GetNestedType("ThemingScope", BindingFlags.NonPublic | BindingFlags.Static).
                GetField("hActCtx", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            if (OSFeature.Feature.IsPresent(OSFeature.Themes))
                ActivateActCtx(hActCtx, out zero);
        }
    }
}
