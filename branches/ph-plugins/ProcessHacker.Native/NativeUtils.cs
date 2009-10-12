using System;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using System.Runtime.InteropServices;

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

        public unsafe static void CopyProcessParameters(
            ProcessHandle processHandle,
            IntPtr peb,
            ProcessCreationFlags creationFlags,
            string imagePathName,
            string dllPath,
            string currentDirectory,
            string commandLine,
            EnvironmentBlock environment,
            string windowTitle,
            string desktopInfo,
            string shellInfo,
            string runtimeInfo,
            ref StartupInfo startupInfo
            )
        {
            UnicodeString imagePathNameStr;
            UnicodeString dllPathStr;
            UnicodeString currentDirectoryStr;
            UnicodeString commandLineStr;
            UnicodeString windowTitleStr;
            UnicodeString desktopInfoStr;
            UnicodeString shellInfoStr;
            UnicodeString runtimeInfoStr;

            // Create the unicode strings.

            imagePathNameStr = new UnicodeString(imagePathName);
            dllPathStr = new UnicodeString(dllPath);
            currentDirectoryStr = new UnicodeString(currentDirectory);
            commandLineStr = new UnicodeString(commandLine);
            windowTitleStr = new UnicodeString(windowTitle);
            desktopInfoStr = new UnicodeString(desktopInfo);
            shellInfoStr = new UnicodeString(shellInfo);
            runtimeInfoStr = new UnicodeString(runtimeInfo);

            try
            {
                NtStatus status;
                IntPtr processParameters;

                // Create the process parameter block.

                status = Win32.RtlCreateProcessParameters(
                    out processParameters,
                    ref imagePathNameStr,
                    ref dllPathStr,
                    ref currentDirectoryStr,
                    ref commandLineStr,
                    environment,
                    ref windowTitleStr,
                    ref desktopInfoStr,
                    ref shellInfoStr,
                    ref runtimeInfoStr
                    );

                if (status >= NtStatus.Error)
                    Win32.ThrowLastError(status);

                try
                {
                    // Allocate a new memory region in the remote process for 
                    // the environment block and copy it over.

                    int environmentLength;
                    IntPtr newEnvironment;

                    environmentLength = environment.GetLength();
                    newEnvironment = processHandle.AllocateMemory(
                        environmentLength,
                        MemoryProtection.ReadWrite
                        );

                    processHandle.WriteMemory(
                        newEnvironment,
                        environment,
                        environmentLength
                        );

                    // Copy over the startup info data.
                    RtlUserProcessParameters* paramsStruct = (RtlUserProcessParameters*)processParameters;

                    paramsStruct->Environment = newEnvironment;
                    paramsStruct->StartingX = startupInfo.X;
                    paramsStruct->StartingY = startupInfo.Y;
                    paramsStruct->CountX = startupInfo.XSize;
                    paramsStruct->CountY = startupInfo.YSize;
                    paramsStruct->CountCharsX = startupInfo.XCountChars;
                    paramsStruct->CountCharsY = startupInfo.YCountChars;
                    paramsStruct->FillAttribute = startupInfo.FillAttribute;
                    paramsStruct->WindowFlags = startupInfo.Flags;
                    paramsStruct->ShowWindowFlags = startupInfo.ShowWindow;

                    if ((startupInfo.Flags & StartupFlags.UseStdHandles) == StartupFlags.UseStdHandles)
                    {
                        paramsStruct->StandardInput = startupInfo.StdInputHandle;
                        paramsStruct->StandardOutput = startupInfo.StdOutputHandle;
                        paramsStruct->StandardError = startupInfo.StdErrorHandle;
                    }

                    // TODO: Add console support.

                    // Allocate a new memory region in the remote process for 
                    // the process parameters.

                    IntPtr newProcessParameters;
                    IntPtr regionSize = paramsStruct->Length.ToIntPtr();

                    newProcessParameters = processHandle.AllocateMemory(
                        IntPtr.Zero,
                        ref regionSize,
                        MemoryFlags.Commit,
                        MemoryProtection.ReadWrite
                        );

                    paramsStruct->MaximumLength = regionSize.ToInt32();

                    processHandle.WriteMemory(newProcessParameters, processParameters, paramsStruct->Length);

                    // Modify the process parameters pointer in the PEB.
                    processHandle.WriteMemory(
                        peb.Increment(Peb.ProcessParametersOffset),
                        &newProcessParameters,
                        IntPtr.Size
                        );
                }
                finally
                {
                    Win32.RtlDestroyProcessParameters(processParameters);
                }
            }
            finally
            {
                imagePathNameStr.Dispose();
                dllPathStr.Dispose();
                currentDirectoryStr.Dispose();
                commandLineStr.Dispose();
                windowTitleStr.Dispose();
                desktopInfoStr.Dispose();
                shellInfoStr.Dispose();
                runtimeInfoStr.Dispose();
            }
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

        public static string GetMessage(IntPtr dllHandle, int messageTableId, int messageLanguageId, int messageId)
        {
            NtStatus status;
            IntPtr messageEntry;
            string message;

            status = Win32.RtlFindMessage(
                dllHandle,
                messageTableId,
                messageLanguageId,
                messageId,
                out messageEntry
                );

            if (status.IsError())
                return null;

            var region = new MemoryRegion(messageEntry);
            var entry = region.ReadStruct<MessageResourceEntry>();

            // Read the message, depending on format.
            if ((entry.Flags & MessageResourceFlags.Unicode) == MessageResourceFlags.Unicode)
            {
                message = region.ReadUnicodeString(MessageResourceEntry.TextOffset);
            }
            else
            {
                message = region.ReadAnsiString(MessageResourceEntry.TextOffset);
            }

            return message;
        }
    }
}
