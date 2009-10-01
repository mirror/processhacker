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
        public static EnvironmentBlock GetCurrent()
        {
            IntPtr peb;
            IntPtr processParameters;
            IntPtr environment;

            peb = ProcessHandle.Current.GetBasicInformation().PebBaseAddress;
            processParameters = Marshal.ReadIntPtr(peb.Increment(Peb.ProcessParametersOffset));
            environment = Marshal.ReadIntPtr(processParameters.Increment(RtlUserProcessParameters.EnvironmentOffset));

            return new EnvironmentBlock(environment);
        }

        public string GetCurrentVariable(string name)
        {
            return GetCurrent().GetVariable(name);
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
                        Win32.ThrowLastError(status);
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
                Win32.ThrowLastError(status);
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
                        data.Resize(valueStr.Length + 2);
                        valueStr.Buffer = data;
                        valueStr.MaximumLength = (ushort)(valueStr.Length + 2);

                        status = Win32.RtlQueryEnvironmentVariable_U(
                            this,
                            ref nameStr,
                            ref valueStr
                            );
                    }

                    if (status >= NtStatus.Error)
                        Win32.ThrowLastError(status);
                }
            }
            finally
            {
                nameStr.Dispose();
            }

            return valueStr.Read();
        }

        public EnvironmentBlock SetCurrent()
        {
            NtStatus status;
            IntPtr previousEnvironment;

            if ((status = Win32.RtlSetCurrentEnvironment(
                this,
                out previousEnvironment
                )) >= NtStatus.Error)
                Win32.ThrowLastError(status);

            return new EnvironmentBlock(previousEnvironment);
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
                        Win32.ThrowLastError(status);
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
