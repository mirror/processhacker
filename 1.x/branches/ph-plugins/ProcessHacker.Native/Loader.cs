using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native
{
    public static class Loader
    {
        public static IntPtr GetProcedure(string dllName, string procedureName)
        {
            return GetProcedure(GetDllHandle(dllName), procedureName);
        }

        public static IntPtr GetProcedure(IntPtr dllHandle, string procedureName)
        {
            return Win32.GetProcAddress(dllHandle, procedureName);
        }

        public static IntPtr GetProcedure(IntPtr dllHandle, int procedureNumber)
        {
            return Win32.GetProcAddress(dllHandle, (ushort)procedureNumber);
        }

        public static IntPtr GetDllHandle(string dllName)
        {
            return Win32.GetModuleHandle(dllName);
        }

        public static IntPtr LoadDll(string dllName)
        {
            return Win32.LoadLibrary(dllName);
        }

        public static bool UnloadDll(IntPtr dllHandle)
        {
            return Win32.FreeLibrary(dllHandle);
        }
    }
}
