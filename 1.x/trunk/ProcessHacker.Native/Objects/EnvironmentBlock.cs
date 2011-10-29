using System;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Objects
{
    public struct EnvironmentBlock
    {
        private static readonly EnvironmentBlock _zero = new EnvironmentBlock(IntPtr.Zero);

        public static EnvironmentBlock Zero
        {
            get { return _zero; }
        }

        public unsafe static EnvironmentBlock GetCurrent()
        {
            return new EnvironmentBlock(ProcessHandle.GetCurrentProcessParameters()->Environment);
        }

        public static string GetCurrentVariable(string name)
        {
            return GetCurrent().GetVariable(name);
        }

        public static EnvironmentBlock SetCurrent(EnvironmentBlock environmentBlock)
        {
            IntPtr previousEnvironment;

            Win32.RtlSetCurrentEnvironment(
                environmentBlock,
                out previousEnvironment
                ).ThrowIf();

            return new EnvironmentBlock(previousEnvironment);
        }

        public static void SetCurrentVariable(string name, string value)
        {
            UnicodeString nameStr = new UnicodeString(name);
            UnicodeString valueStr = new UnicodeString(value);

            try
            {
                Win32.RtlSetEnvironmentVariable(
                    IntPtr.Zero,
                    ref nameStr,
                    ref valueStr
                    ).ThrowIf();
            }
            finally
            {
                valueStr.Dispose();
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
            Win32.RtlCreateEnvironment(
                cloneCurrent,
                out _environment
                ).ThrowIf();
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

        public unsafe int Length
        {
            get
            {
                short* ptr = (short*)_environment;

                while (*ptr != 0)
                {
                    while (*ptr++ != 0)
                    {

                    }
                }

                ptr++;

                return (new IntPtr(ptr)).Decrement(_environment).ToInt32();
            }
        }

        public string GetVariable(string name)
        {
            UnicodeString nameStr = new UnicodeString(name);

            try
            {
                using (MemoryAlloc data = new MemoryAlloc(100))
                {
                    UnicodeString valueStr = new UnicodeString
                    {
                        Buffer = data, 
                        MaximumLength = (ushort)data.Size
                    };

                    NtStatus status = Win32.RtlQueryEnvironmentVariable_U(
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

                    status.ThrowIf();

                    return valueStr.Text;
                }
            }
            finally
            {
                nameStr.Dispose();
            }
        }

        public void SetVariable(string name, string value)
        {
            IntPtr environment = _environment;

            UnicodeString nameStr = new UnicodeString(name);
            UnicodeString valueStr = new UnicodeString(value);

            try
            {
                Win32.RtlSetEnvironmentVariable(
                    ref environment,
                    ref nameStr,
                    ref valueStr
                    ).ThrowIf();
            }
            finally
            {
                valueStr.Dispose();
                nameStr.Dispose();
            }

            _environment = environment;
        }
    }
}
