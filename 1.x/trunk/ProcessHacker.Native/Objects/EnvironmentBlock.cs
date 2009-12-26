using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;
using System.Runtime.InteropServices;

namespace ProcessHacker.Native.Objects
{
    public struct EnvironmentBlock
    {
        private static readonly EnvironmentBlock _zero = new EnvironmentBlock(IntPtr.Zero);

        public static EnvironmentBlock Zero
        {
            get { return _zero; }
        }

        public static EnvironmentBlock GetCurrent()
        {
            unsafe
            {
                return new EnvironmentBlock(ProcessHandle.GetCurrentProcessParameters()->Environment);
            }
        }

        public static string GetCurrentVariable(string name)
        {
            return GetCurrent().GetVariable(name);
        }

        public static EnvironmentBlock SetCurrent(EnvironmentBlock environmentBlock)
        {
            NtStatus status;
            IntPtr previousEnvironment;

            if ((status = Win32.RtlSetCurrentEnvironment(
                environmentBlock,
                out previousEnvironment
                )) >= NtStatus.Error)
                Win32.Throw(status);

            return new EnvironmentBlock(previousEnvironment);
        }

        public static void SetCurrentVariable(string name, string value)
        {
            NtStatus status;
            UnicodeString nameStr;
            UnicodeString valueStr;

            nameStr = new UnicodeString(name);

            try
            {
                valueStr = new UnicodeString(value);

                try
                {
                    if ((status = Win32.RtlSetEnvironmentVariable(
                        IntPtr.Zero,
                        ref nameStr,
                        ref valueStr
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
                finally
                {
                    valueStr.Dispose();
                }
            }
            finally
            {
                nameStr.Dispose();
            }
        }

        public static implicit operator IntPtr(EnvironmentBlock environmentBlock)
        {
            return environmentBlock.Memory;
        }

        private IntPtr _environment;

        public EnvironmentBlock(bool cloneCurrent)
        {
            NtStatus status;

            if ((status = Win32.RtlCreateEnvironment(
                cloneCurrent,
                out _environment
                )) >= NtStatus.Error)
                Win32.Throw(status);
        }

        public EnvironmentBlock(TokenHandle tokenHandle)
        {
            if (!Win32.CreateEnvironmentBlock(out _environment, tokenHandle, false))
                Win32.Throw();
        }

        public EnvironmentBlock(IntPtr environment)
        {
            _environment = environment;
        }

        public IntPtr Memory
        {
            get { return _environment; }
        }

        public void Destroy()
        {
            Win32.RtlDestroyEnvironment(this);
        }

        public unsafe int GetLength()
        {
            short* ptr = (short*)_environment;

            while (*ptr != 0)
                while (*ptr++ != 0)
                    ;

            ptr++;

            return (new IntPtr(ptr)).Decrement(_environment).ToInt32();
        }

        public string GetVariable(string name)
        {
            NtStatus status;
            UnicodeString nameStr;
            UnicodeString valueStr;

            nameStr = new UnicodeString(name);

            try
            {
                using (var data = new MemoryAlloc(100))
                {
                    valueStr = new UnicodeString();
                    valueStr.Buffer = data;
                    valueStr.MaximumLength = (ushort)data.Size;

                    status = Win32.RtlQueryEnvironmentVariable_U(
                        this,
                        ref nameStr,
                        ref valueStr
                        );

                    if (status == NtStatus.BufferTooSmall)
                    {
                        // Resize and try again (+2 for the null terminator).
                        data.ResizeNew(valueStr.Length + 2);
                        valueStr.Buffer = data;
                        valueStr.MaximumLength = (ushort)(valueStr.Length + 2);

                        status = Win32.RtlQueryEnvironmentVariable_U(
                            this,
                            ref nameStr,
                            ref valueStr
                            );
                    }

                    if (status >= NtStatus.Error)
                        Win32.Throw(status);

                    return valueStr.Read();
                }
            }
            finally
            {
                nameStr.Dispose();
            }
        }

        public void SetVariable(string name, string value)
        {
            NtStatus status;
            IntPtr environment = _environment;
            UnicodeString nameStr;
            UnicodeString valueStr;

            nameStr = new UnicodeString(name);

            try
            {
                valueStr = new UnicodeString(value);

                try
                {
                    if ((status = Win32.RtlSetEnvironmentVariable(
                        ref environment,
                        ref nameStr,
                        ref valueStr
                        )) >= NtStatus.Error)
                        Win32.Throw(status);
                }
                finally
                {
                    valueStr.Dispose();
                }
            }
            finally
            {
                nameStr.Dispose();
            }

            _environment = environment;
        }
    }
}
