namespace System
{
    using Runtime.InteropServices;
    using ProcessHacker.Native;  
    using ProcessHacker.Native.Api;
    using ProcessHacker.Native.Objects;

    /// <summary>
    /// Utility class to wrap an unmanaged DLL and be responsible for freeing it.
    /// </summary>
    /// <remarks>This is a managed wrapper over the native LoadLibrary, GetProcAddress, and FreeLibrary calls.</remarks>
    public sealed class NativeLibrary : NativeHandle
    {
        /// <summary>
        /// LoadLibraryEx constructor to load a dll and be responsible for freeing it.
        /// </summary>
        /// <param name="dllName">full path name of dll to load</param>
        /// <param name="flags"></param>
        /// <remarks>Throws exceptions on failure. Most common failure would be file-not-found, or that the file is not a loadable image.</remarks>
        public NativeLibrary(string dllName, LoadLibraryFlags flags)
        {
            UnicodeString str = new UnicodeString(dllName);

            IntPtr ptr;

            NtStatus result = Win32.LdrLoadDll(null, (int)flags, ref str, out ptr);

            if (result.IsError())
            {
                this.MarkAsInvalid();
            }

            str.Dispose();
        }

        /// <summary>
        /// Dynamically lookup a function in the dll via kernel32!GetProcAddress.
        /// </summary>
        /// <param name="functionName">raw name of the function in the export table.</param>
        /// <returns>null if function is not found. Else a delegate to the unmanaged function.</returns>
        /// <remarks>GetProcAddress results are valid as long as the dll is not yet unloaded. This
        /// is very very dangerous to use since you need to ensure that the dll is not unloaded
        /// until after you're done with any objects implemented by the dll. For example, if you
        /// get a delegate that then gets an IUnknown implemented by this dll,
        /// you can not dispose this library until that IUnknown is collected. Else, you may free
        /// the library and then the CLR may call release on that IUnknown and it will crash.</remarks>
        public TDelegate GetUnmanagedFunction<TDelegate>(string functionName) where TDelegate : class
        {
            using (AnsiString str = new AnsiString(functionName))
            {
                IntPtr functionPtr;

                NtStatus result = Win32.LdrGetProcedureAddress(this, str.Buffer, 0, out functionPtr);

                // Failure is a common case, especially for adaptive code.
                if (result.IsError())
                {
                    //result.ReturnException().Log();

                    return null;
                }

                Delegate function = Marshal.GetDelegateForFunctionPointer(functionPtr, typeof(TDelegate));

                // Ideally, we'd just make the constraint on TDelegate be
                // System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
                // prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
                object o = function;

                return (TDelegate)o;
            }
        }

        #region Disposable Members

        /// <summary>
        /// Call FreeLibrary on the unmanaged dll. All function pointers handed out from this class become invalid after this.
        /// </summary>
        /// <remarks>This is very dangerous because it suddenly invalidate
        /// everything retrieved from this dll. This includes any functions
        /// handed out via GetProcAddress, and potentially any objects returned
        /// from those functions (which may have an implementation in the dll).
        /// </remarks>
        protected override void Close()
        {
            Win32.LdrUnloadDll(this).ThrowIf();
        }

        #endregion

        public IntPtr GetProcedure(string procedureName)
        {
            AnsiString str = new AnsiString(procedureName);
            {
                IntPtr functionPtr = IntPtr.Zero;

                NtStatus result = Win32.LdrGetProcedureAddress(this, functionPtr, 0, out functionPtr);

                return functionPtr;
            }
        }

        public IntPtr GetProcedure(int procedureNumber)
        {
            IntPtr functionPtr;

            NtStatus result = Win32.LdrGetProcedureAddress(this, IntPtr.Zero, procedureNumber, out functionPtr);

            return functionPtr;
        }

        #region Static Methods

        public static IntPtr GetProcedure(string dllName, string procedureName)
        {
            return GetProcedure(GetModuleHandle(dllName), procedureName);
        }

        public static IntPtr GetProcedure(IntPtr dllHandle, string procedureName)
        {
            IntPtr handle;

            using (AnsiString str = new AnsiString(procedureName))
            {
                Win32.LdrGetProcedureAddress(dllHandle, str.Buffer, 0, out handle).ThrowIf();
            }

            return handle;
        }

        public static IntPtr GetModuleHandle(string dllName)
        {
            IntPtr handle;

            UnicodeString str = new UnicodeString(dllName);
            {
                Win32.LdrGetDllHandle(null, 0, ref str, out handle).ThrowIf();
            }

            return handle;
        }

        #endregion
    }

    [Flags] //TODO: move to enum file
    public enum LoadLibraryFlags
    {
        None = 0,

        /// <summary>
        /// If this value is used, and the executable module is a DLL, the system does not call DllMain for process and thread initialization and termination. Also, the system does not load additional executable modules that are referenced by the specified module. Do not use this value; it is provided only for backwards compatibility. If you are planning to access only data or resources in the DLL, use LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE or LOAD_LIBRARY_AS_IMAGE_RESOURCE or both. Otherwise, load the library as a DLL or executable module using the LoadLibrary function.
        /// </summary>
        DontResolveDllReferences = 0x00000001,

        /// <summary>
        /// If this value is used, the system maps the file into the calling process's virtual address space as if it were a data file. Nothing is done to execute or prepare to execute the mapped file. Therefore, you cannot call functions like GetModuleFileName, GetModuleHandle or GetProcAddress with this DLL. Using this value causes writes to read-only memory to raise an access violation. Use this flag when you want to load a DLL only to extract messages or resources from it. This value can be used with LOAD_LIBRARY_AS_IMAGE_RESOURCE. 
        /// </summary>
        LOAD_LIBRARY_AS_DATAFILE = 0x00000002,

        /// <summary>
        /// If this value is used and lpFileName specifies an absolute path, the system uses the alternate file search strategy discussed in the Remarks section to find associated executable modules that the specified module causes to be loaded. If this value is used and lpFileName specifies a relative path, the behavior is undefined. If this value is not used, or if lpFileName does not specify a path, the system uses the standard search strategy discussed in the Remarks section to find associated executable modules that the specified module causes to be loaded.
        /// </summary>
        LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,

        /// <summary>
        /// If this value is used, the system does not check AppLocker rules or apply Software Restriction Policies for the DLL. This action applies only to the DLL being loaded and not to its dependents. This value is recommended for use in setup programs that must run extracted DLLs during installation.
        /// </summary>
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,

        /// <summary>
        /// If this value is used, the system maps the file into the process's virtual address space as an image file. However, the loader does not load the static imports or perform the other usual initialization steps. Use this flag when you want to load a DLL only to extract messages or resources from it. Unless the application depends on the image layout, this value should be used with either LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE or LOAD_LIBRARY_AS_DATAFILE. For more information, see the Remarks section.
        /// </summary>
        LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
        
        /// <summary>
        /// Similar to LOAD_LIBRARY_AS_DATAFILE, except that the DLL file on the disk is opened for exclusive write access. Therefore, other processes cannot open the DLL file for write access while it is in use. However, the DLL can still be opened by other processes. This value can be used with LOAD_LIBRARY_AS_IMAGE_RESOURCE. For more information, see Remarks.
        /// </summary>
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,


        LOAD_LIBRARY_REQUIRE_SIGNED_TARGET = 0x00000080
    }
}
