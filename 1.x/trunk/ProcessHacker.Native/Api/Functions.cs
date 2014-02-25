/*
 * Process Hacker - 
 *   windows API functions
 *                       
 * Copyright (C) 2009 Flavio Erlich
 * Copyright (C) 2009 Uday Shanbhag
 * Copyright (C) 2009 Dean
 * Copyright (C) 2008-2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

/* This file contains function declarations for the Win32 API.
 * 
 * All functions which do not belong in any other category 
 * are placed in this file.
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;

namespace ProcessHacker.Native.Api
{
    public static partial class Win32
    {
        #region Controls

        [DllImport("comctl32.dll")]
        public static extern int LoadIconMetric(
            [In] IntPtr InstanceHandle,
            [In] IntPtr Name,
            [In] int Metric,
            [Out] out IntPtr IconHandle
            );

        [DllImport("user32.dll")]
        public static extern IntPtr LoadImage(
            [In] IntPtr InstanceHandle,
            [In] IntPtr Name,
            [In] LoadImageType Type,
            [In] int DesiredWidth,
            [In] int DesiredHeight,
            [In] int Flags
            );

        #endregion

        #region Credentials

        [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredPackAuthenticationBuffer(
            [In] CredPackFlags Flags,
            [In] string UserName,
            [In] string Password,
            [In] IntPtr PackedCredentials,
            ref int PackedCredentialsSize
            );

        [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CredUnPackAuthenticationBuffer(
            [In] CredPackFlags Flags,
            [In] IntPtr AuthBuffer,
            [In] int AuthBufferSize,
            [In] IntPtr UserName,
            ref int UserNameSize,
            [In] IntPtr DomainName,
            ref int DomainNameSize,
            [In] IntPtr Password,
            ref int PasswordSize
            );

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern Win32Error CredUIPromptForCredentials(
            [In] [Optional] ref CredUiInfo UiInfo,
            [In] string TargetName,
            [In] IntPtr Reserved,
            [In] [Optional] Win32Error AuthError,
            [In] IntPtr UserName,
            [In] int UserNameMaxChars,
            [In] IntPtr Password,
            [In] int PasswordMaxChars,
            [MarshalAs(UnmanagedType.Bool)]
            ref bool Save,
            [In] CredUiFlags Flags
            );

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern Win32Error CredUIPromptForWindowsCredentials(
            [In] [Optional] ref CredUiInfo UiInfo,
            [In] [Optional] Win32Error AuthError,
            ref int AuthPackage,
            [In] [Optional] IntPtr InAuthBuffer,
            [In] int InAuthBufferSize,
            [Out] out IntPtr OutAuthBuffer,
            [Out] out int OutAuthBufferSize,
            [MarshalAs(UnmanagedType.Bool)]
            ref bool Save,
            [In] CredUiWinFlags Flags
            );

        [DllImport("secur32.dll")]
        public static extern int EnumerateSecurityPackages(
            [Out] out int Packages,
            [Out] out IntPtr PackageInfo
            );

        [DllImport("secur32.dll")]
        public static extern int FreeContextBuffer(
            [In] IntPtr ContextBuffer
            );

        #endregion

        #region Cryptography

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int CertNameToStr(
            [In] int CertEncodingType,
            [In] IntPtr Name, // CertNameBlob*
            [In] int Type,
            [In] IntPtr Buffer,
            [In] int Size
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATCatalogInfoFromContext(
            [In] IntPtr CatInfoHandle,
            [Out] out CatalogInfo CatInfo,
            [In] int Flags
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern IntPtr CryptCATAdminEnumCatalogFromHash(
            [In] IntPtr CatAdminHandle,
            [In] byte[] Hash,
            [In] int HashSize,
            [In] int Flags,
            [In] IntPtr PrevCatInfoHandle
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminAcquireContext(
            [Out] out IntPtr CatAdminHandle,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid Subsystem,
            [In] int Flags
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminCalcHashFromFileHandle(
            [In] IntPtr FileHandle,
            ref int HashSize,
            [In] byte[] Hash,
            [In] int Flags
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminReleaseContext(
            [In] IntPtr CatAdminHandle,
            [In] int Flags
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern bool CryptCATAdminReleaseCatalogContext(
            [In] IntPtr CatAdminHandle, 
            [In] IntPtr CatInfoHandle,
            [In] int Flags
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern uint WinVerifyTrust(
             [In] IntPtr hWnd,
             [In] [MarshalAs(UnmanagedType.LPStruct)] Guid ActionId,
             [In] ref WintrustData WintrustData
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern IntPtr WTHelperGetProvSignerFromChain(
            [In] IntPtr ProvData, // CryptProviderData*
            [In] int SignerIndex,
            [In] bool CounterSigner,
            [In] int CounterSignerIndex
            );

        [DllImport("wintrust.dll", SetLastError = true)]
        public static extern IntPtr WTHelperProvDataFromStateData(
            [In] IntPtr StateData
            );

        #endregion

        #region Debugging

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcess(
            [In] int Pid
            );

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcessStop(
            [In] int Pid
            );

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugSetProcessKillOnExit(
            [In] bool KillOnExit
            );

        #endregion

        #region Error Handling

        /// <summary>
        /// Removes an Application from Windows Error Reporting on Windows XP
        /// </summary>
        /// <param name="ExeName">The process.exe or the path\process.exe to be excluded</param>
        /// <returns>True if successfully excluded</returns>
        [DllImport("faultrep.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool AddERExcludedApplication(
            [In] string ExeName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int FormatMessage(
            [In] int Flags,
            [In] [Optional] IntPtr Source,
            [In] int MessageId,
            [In] int LanguageId,
            [Out] StringBuilder Buffer,
            [In] int Size,
            [In] [Optional] IntPtr Arguments
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int FormatMessage(
            [In] int Flags,
            [In] [Optional] IntPtr Source,
            [In] int MessageId,
            [In] int LanguageId,
            [In] IntPtr Buffer,
            [In] int Size,
            [In] [Optional] IntPtr Arguments
            );

        /// <summary>
        /// Removes an Application from Windows Error Reporting on Windows Vista
        /// </summary>
        /// <param name="ExeName">The process.exe or the path\process.exe to be excluded</param>
        /// <param name="AllUsers">true to exclude process from all users. Note: Administrator access is Required if set true</param>
        /// <returns>A HResult indicating the result</returns>
        [DllImport("wer.dll", CharSet = CharSet.Unicode)]
        public static extern HResult WerAddExcludedApplication(
            [In] string ExeName,
            [In] bool AllUsers
            );

        #endregion

        #region Files

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int SearchPath(
            [In] [Optional] string Path,
            [In] string FileName,
            [In] [Optional] string Extension,
            [In] int BufferLength,
            [In] IntPtr Buffer,
            [Out] out IntPtr FilePart
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileInformationByHandleEx(
            [In] IntPtr FileHandle,
            [In] int FileInformationClass,
            [In] IntPtr FileInformation,
            [In] int FileInformationLength
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileSizeEx(
            [In] IntPtr FileHandle,
            [Out] out long FileSize
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int QueryDosDevice(
            [In] [Optional] string DeviceName,
            [In] IntPtr TargetPath,
            [In] int MaxLength
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            [In] string FileName, 
            [In] FileAccess DesiredAccess,
            [In] FileShareMode ShareMode,
            [In] [Optional] int SecurityAttributes,
            [In] FileCreationDispositionWin32 CreationDisposition,
            [In] int FlagsAndAttributes,
            [In] [Optional] IntPtr TemplateFile
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            [In] IntPtr FileHandle,
            [Out] byte[] Buffer,
            [In] int Bytes, 
            [Out] [Optional] out int ReadBytes,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
            [In] IntPtr FileHandle, 
            [In] byte[] Buffer,
            [In] int Bytes, 
            [Out] [Optional] out int WrittenBytes,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool WriteFile(
            [In] IntPtr FileHandle,
            [In] void* Buffer,
            [In] int Bytes,
            [Out] [Optional] out int WrittenBytes,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            [In] IntPtr FileHandle,
            [In] int IoControlCode,
            [In] [Optional] byte[] InBuffer,
            [In] int InBufferLength, 
            [Out] [Optional] byte[] OutBuffer,
            [In] int OutBufferLength,
            [Out] [Optional]out int BytesReturned,
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool DeviceIoControl(
            [In] IntPtr FileHandle,
            [In] int IoControlCode,
            [In] [Optional] byte* InBuffer,
            [In] int InBufferLength, 
            [Out] [Optional] byte* OutBuffer,
            [In] int OutBufferLength,
            [Out] [Optional] out int BytesReturned,
            [Optional] IntPtr Overlapped
            );

        #endregion

        #region GDI

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(
            [In] IntPtr Object
            );

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(
            [In] GdiStockObject Object
            );

        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(
            [In] IntPtr hDC,
            [In] int LeftRect,   
            [In] int TopRect,
            [In] int RightRect,
            [In] int BottomRect
            );

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(
            [In] IntPtr hDC,
            [In] IntPtr hGdiObject
            );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(
            [In] GdiPenStyle PenStyle,
            [In] int Width,
            [In] IntPtr Color
            );

        [DllImport("gdi32.dll")]
        public static extern bool RestoreDC(
            [In] IntPtr hDC,
            [In] int SavedDC
            );

        [DllImport("gdi32.dll")]
        public static extern int SaveDC(
            [In] IntPtr hDC
            );

        [DllImport("gdi32.dll")]
        public static extern GdiBlendMode SetROP2(
            [In] IntPtr hDC,
            [In] GdiBlendMode DrawMode
            );

        #endregion

        #region Images

        [DllImport("imagehlp.dll", SetLastError = true)]
        public static extern IntPtr CheckSumMappedFile(
            [In] IntPtr BaseAddress,
            [In] int FileLength,
            [Out] out int HeaderSum,
            [Out] out int CheckSum
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern IntPtr ImageNtHeader(
            [In] IntPtr ImageBase
            );

        [DllImport("imagehlp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern bool MapAndLoad(
            [In] string ImageName,
            [In] [Optional] string DllPath,
            [Out] out LoadedImage LoadedImage,
            [MarshalAs(UnmanagedType.Bool)]
            [In] bool DotDll,
            [In] bool ReadOnly
            );

        [DllImport("imagehlp.dll", SetLastError = true)]
        public static extern bool UnMapAndLoad(
            [In] ref LoadedImage LoadedImage
            );

        #endregion

        #region Jobs

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateJobObject(
            [In] IntPtr JobHandle,
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AssignProcessToJobObject(
            [In] IntPtr JobHandle,
            [In] IntPtr ProcessHandle
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateJobObject(
            [In] [Optional] IntPtr SecurityAttributes,
            [In] [Optional] string Name
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenJobObject(
            [In] JobObjectAccess DesiredAccess, 
            [In] bool Inherit, 
            [In] string Name
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryInformationJobObject(
            [In] [Optional] IntPtr JobHandle,
            [In] JobObjectInformationClass JobInformationClass,
            [Out] IntPtr JobInformation, 
            [In] int JobInformationLength, 
            [Out] [Optional] out int ReturnLength
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool QueryInformationJobObject(
            [In] [Optional] IntPtr JobHandle, 
            [In] JobObjectInformationClass JobInformationClass,
            [Out] out JobObjectBasicUiRestrictions JobInformation,
            [In] int JobInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        #endregion

        #region Kernel

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumDeviceDrivers(
            [Out] IntPtr[] ImageBases,
            [In] int Size,
            [Out] out int Needed
            );

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetDeviceDriverBaseName(
            [In] IntPtr ImageBase, 
            [Out] StringBuilder FileName, 
            [In] int Size
            );

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetDeviceDriverFileName(
            [In] IntPtr ImageBase, 
            [Out] StringBuilder FileName, 
            [In] int Size
            );

        #endregion

        #region Libraries

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(
            [In] string FileName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibraryEx(
            [In] string FileName, 
            IntPtr File,
            [In] int Flags
            );

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(
            [In] IntPtr Handle
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(
            [In] [Optional] string ModuleName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr Module, 
            [In] string ProcName
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr Module, 
            [In] ushort ProcOrdinal
            );

        #endregion

        #region Mailslots

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateMailslot(
            [In] string Name,
            [In] int MaxMessageSize,
            [In] int ReadTimeout,
            [In] IntPtr SecurityAttributes
            );

        #endregion

        #region Memory

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocateUserPhysicalPages(
            [In] IntPtr ProcessHandle,
            ref IntPtr NumberOfPages,
            IntPtr[] UserPfnArray
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeUserPhysicalPages(
            [In] IntPtr ProcessHandle,
            ref IntPtr NumberOfPages,
            IntPtr[] UserPfnArray
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool MapUserPhysicalPages(
            [In] IntPtr Address,
            IntPtr NumberOfPages,
            IntPtr[] UserPfnArray
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalAlloc(
            [In] AllocFlags Flags,
            [In] int Bytes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalReAlloc(
            [In] IntPtr Memory,
            [In] AllocFlags Flags,
            [In] int Bytes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(
            [In] IntPtr Memory
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessHeaps(
            [In] int NumberOfHeaps, 
            [Out] IntPtr[] Heaps
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int HeapCompact(
            [In] IntPtr Heap, 
            [In] bool NoSerialize
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapCreate(
            [In] HeapFlags Flags,
            [In] IntPtr InitialSize,
            [In] IntPtr MaximumSize
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool HeapDestroy(
            [In] IntPtr Heap
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool HeapFree(
            [In] IntPtr Heap,
            [In] HeapFlags Flags, 
            [In] IntPtr Memory
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapAlloc(
            [In] IntPtr Heap,
            [In] HeapFlags Flags,
            [In] IntPtr Bytes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr HeapReAlloc(
            [In] IntPtr Heap,
            [In] HeapFlags Flags,
            [In] IntPtr Memory,
            [In] IntPtr Bytes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHeap();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(
            [In] IntPtr Process, 
            [In] [Optional] IntPtr Address,
            [Out] [MarshalAs(UnmanagedType.Struct)] out MemoryBasicInformation Buffer,
            [In] int Size
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualProtectEx(
            [In] IntPtr Process,
            [In] IntPtr Address,
            [In] int Size,
            [In] MemoryProtection NewProtect, 
            [Out] out MemoryProtection OldProtect
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(
            [In] IntPtr Process,
            [In] [Optional] IntPtr Address,
            [In] int Size,
            [In] MemoryState Type,
            [In] MemoryProtection Protect
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(
            [In] IntPtr Process,
            [In] IntPtr Address,
            [In] int Size,
            [In] MemoryState FreeType
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadProcessMemory(
            [In] IntPtr Process,
            [In] IntPtr BaseAddress,
            [Out] byte[] Buffer,
            [In] int Size, 
            [Out] out int BytesRead
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool ReadProcessMemory(
            [In] IntPtr Process, 
            [In] IntPtr BaseAddress,
            [Out] void* Buffer, 
            [In] int Size, 
            [Out] out int BytesRead
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(
            [In] IntPtr Process,
            [In] IntPtr BaseAddress,
            [In] byte[] Buffer,
            [In] int Size, 
            [Out] out int BytesWritten
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool WriteProcessMemory(
            [In] IntPtr Process,
            [In] IntPtr BaseAddress,
            [In] void* Buffer,
            [In] int Size, 
            [Out] out int BytesWritten
            );

        #endregion

        #region Misc.

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessShutdownParameters(
            [In] int Level,
            [In] int Flags
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(
            [In] ExitWindowsFlags flags,
            [In] int reason
            );

        [DllImport("powrprof.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetSuspendState(
            [In] bool hibernate,
            [In] bool forceCritical,
            [In] bool disableWakeEvent
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockWorkStation();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceFrequency(
            [Out] out long PerformanceFrequency
            );

        [DllImport("kernel32.dll")]
        public static extern int GetTickCount();

        #endregion

        #region Named Pipes

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConnectNamedPipe(
            [In] IntPtr NamedPipe, 
            [Optional] IntPtr Overlapped
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateNamedPipe(
            [In] string Name,
            [In] PipeAccessMode OpenMode,
            [In] PipeMode PipeMode,
            [In] int MaxInstances,
            [In] int OutBufferSize,
            [In] int InBufferSize,
            [In] int DefaultTimeOut,
            [In] [Optional] IntPtr SecurityAttributes
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisconnectNamedPipe(
            [In] IntPtr NamedPipe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNamedPipeClientProcessId(
            [In] IntPtr NamedPipeHandle, 
            [Out] out int ServerProcessId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNamedPipeHandleState(
            [In] IntPtr NamedPipeHandle, 
            [Out] [Optional] out PipeState State,
            [Out] [Optional] out int CurInstances,
            [Out] [Optional] out int MaxCollectionCount,
            [Out] [Optional] out int CollectDataTimeout,
            [Out] [Optional] out int UserName,
            [In] int MaxUserNameSize
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WaitNamedPipe(
            [In] string Name,
            [In] int Timeout
            );

        #endregion

        #region Network

        /// <summary>
        /// Allows an application to check if a connection to the Internet can be established.
        /// </summary>
        /// <param name="Url">A string that specifies the URL to use for checking the connection.</param>
        /// <param name="Flags">Forces a connection, must be 1.</param>
        /// <param name="Reserved">This parameter is reserved and must be 0.</param>
        /// <returns></returns>
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool InternetCheckConnection(
            [In] string Url,
            [In] int Flags,
            [In] int Reserved
            );

        /// <summary>
        /// The NdfCancelIncident function is used to cancel unneeded functions which have been previously called on an existing incident.
        /// </summary>
        /// <remarks>Before using this API, an application must call an incident creation function such as NdfCreateWebIncident.
        /// NdfCloseIncident should be used to close an incident once it has been resolved, as NdfCancelIncident does not actually close the incident itself.
        /// </remarks>
        /// <param name="NdfHandle">A handle to the Network Diagnostics Framework incident. 
        /// This handle should match the handle of an existing incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll")]
        public static extern HResult NdfCancelIncident(
            [In] IntPtr NdfHandle
            );

        /// <summary>
        /// The NdfCloseIncident function is used to close an Network Diagnostics Framework (NDF) incident following its resolution.
        /// </summary>
        /// <param name="NdfHandle">The handle to the NDF incident that is being closed.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll")]
        public static extern HResult NdfCloseIncident(
            [In] IntPtr NdfHandle
            );

        /// <summary>
        /// The NdfCreateConnectivityIncident function diagnoses generic internet connectivity problems.
        /// </summary>
        /// <param name="NdfHandle">The handle to the Network Diagnostics Framework incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll")]
        public static extern HResult NdfCreateConnectivityIncident(
            [In, Out] ref IntPtr NdfHandle
            );

        /// <summary>
        /// The NdfCreateInboundIncident function creates a session to diagnose inbound connectivity for a specific application or service.
        /// </summary>
        /// <param name="applicationID">The fully qualified path to the application receiving the inbound traffic.</param>
        /// <param name="serviceID">The Windows service receiving the inbound traffic.</param>
        /// <param name="userID">The SID for the application receiving the traffic. If NULL, the caller's SID is automatically used.</param>
        /// <param name="localTarget">A SOCKADDR_STORAGE structure which limits the diagnosis to traffic to a specific IP address. If NULL, all traffic will be included in the diagnosis</param>
        /// <param name="protocol">The protocol which should be diagnosed. For example, IPPROTO_TCP would be used to indicate the TCP/IP protocol.</param>
        /// <param name="dwFlags">The Inbound flags for specifying the type of options to preform during diagnostics.</param>
        /// <param name="NdfHandle">Pointer to a handle to the Network Diagnostics Framework incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll", CharSet = CharSet.Unicode)]
        public static extern HResult NdfCreateInboundIncident(
            [In, Optional] string applicationID,
            [In, Optional] string serviceID,
            [In, Optional] int userID, //Incorrect
            [In, Optional] int localTarget, //Incorrect
                           int protocol, //Incorrect
                           int dwFlags, //IsUnum: NDF_INBOUND_FLAG
            [In, Out] ref IntPtr NdfHandle
            );

        /// <summary>
        /// The NdfCreateDNSIncident function diagnoses name resolution issues in resolving a specific host name.
        /// </summary>
        /// <param name="hostname">The host name with which there is a name resolution issue.</param>
        /// <param name="querytype">The numeric representation of the type of record that was queried when the issue occurred.</param>
        /// <param name="NdfHandle">A handle to the Network Diagnostics Framework incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll", CharSet = CharSet.Unicode)]
        public static extern HResult NdfCreateDNSIncident(
            [In] string hostname,
                 ushort querytype, //enum value: see the windns.h header file
            [In, Out] ref IntPtr NdfHandle
            );

        /// <summary>
        /// The NdfCreateSharingIncident function diagnoses network problems in accessing a specific network share.
        /// </summary>
        /// <param name="shareName">The full UNC string (for example, "\\server\folder\file.ext")for the shared asset with which there is a connectivity issue.</param>
        /// <param name="NdfHandle">A handle to the Network Diagnostics Framework incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll", CharSet = CharSet.Unicode)]
        public static extern HResult NdfCreateSharingIncident(
             [In] string shareName,
             [In, Out] ref IntPtr NdfHandle
             );

        /// <summary>
        /// The NdfCreateWebIncident function diagnoses web connectivity problems concerning a specific URL.
        /// </summary>
        /// <param name="url">The URL with which there is a connectivity issue.</param>
        /// <param name="NdfHandle">A handle to the Network Diagnostics Framework incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll", CharSet = CharSet.Unicode)]
        public static extern HResult NdfCreateWebIncident(
             [In] string url,
             [In, Out] ref IntPtr NdfHandle
             );

        /// <summary>
        /// The NdfCreateWebIncidentEx function diagnoses web connectivity problems concerning a specific URL. This function allows for more control over the underlying diagnosis than the NdfCreateWebIncident function.
        /// </summary>
        /// <param name="url">The URL with which there is a connectivity issue.</param>
        /// <param name="useWinHTTP">If TRUE, diagnosis is performed using the WinHTTP APIs. Otherwise, the WinInet APIs are used. </param>
        /// <param name="moduleName">The module name to use when checking against application-specific filtering rules (for example, "C:\Program Files\Internet Explorer\iexplorer.exe"). If NULL, the value is autodetected during the diagnosis.</param>
        /// <param name="NdfHandle">A handle to the Network Diagnostics Framework incident.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll", CharSet = CharSet.Unicode)]
        public static extern HResult NdfCreateWebIncidentEx(
             [In] string url,
             [MarshalAs(UnmanagedType.Bool)]
             [In] bool useWinHTTP,
             [In] string moduleName,
             [In, Out] ref IntPtr NdfHandle
             );

        /// <summary>
        /// The NdfExecuteDiagnosis function is used to diagnose the root cause of the incident that has occurred.
        /// </summary>
        /// <param name="NdfHandle">A handle to the Network Diagnostics Framework incident.</param>
        /// <param name="hwnd">The handle to the window that is intended to display the diagnostic information. If specified, the NDF UI is modal to the window. If NULL, the UI is non-modal.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll")]
        public static extern HResult NdfExecuteDiagnosis(
             [In] IntPtr NdfHandle,
             [In] IntPtr hwnd
                 );

        /// <summary>
        /// The NdfGetTraceFile function is used to retrieve the path containing an Event Trace Log (ETL) file that contains Event Tracing for Windows (ETW) events from a diagnostic session.
        /// </summary>
        /// <param name="NdfHandle">A handle to a Network Diagnostics Framework incident. This handle should match the handle of an existing incident.</param>
        /// <param name="TraceFileLocation">Pointer to a string that contains the location of the trace file.</param>
        /// <returns>A HResult value indicating the result</returns>
        [DllImport("ndfapi.dll", CharSet = CharSet.Unicode)]
        public static extern HResult NdfGetTraceFile(
             [In] IntPtr NdfHandle,
             [Out] string TraceFileLocation
                 );

        #endregion

        #region Private Namespaces

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AddSIDToBoundaryDescriptor(
            ref IntPtr BoundaryDescriptor,
            [In] IntPtr RequiredSid
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ClosePrivateNamespace(
            [In] IntPtr PrivateNamespaceHandle,
            [In] PrivateNamespaceFlags Flags
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateBoundaryDescriptor(
            [In] string Name,
            [In] int Flags
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreatePrivateNamespace(
            [In] IntPtr PrivateNamespaceAttributes,
            [In] IntPtr BoundaryDescriptor,
            [In] string AliasPrefix
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void DeleteBoundaryDescriptor(
            [In] IntPtr BoundaryDescriptor
            );

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenPrivateNamespace(
            [In] IntPtr BoundaryDescriptor,
            [In] string AliasPrefix
            );

        #endregion

        #region Processes

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void ExitProcess(
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryProcessCycleTime(
            [In] IntPtr ProcessHandle, 
            [Out] out ulong CycleTime
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetPriorityClass(
            [In] IntPtr ProcessHandle, 
            [In] ProcessPriorityClassWin32 Priority
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ProcessPriorityClassWin32 GetPriorityClass(
            [In] IntPtr ProcessHandle
            );

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyWorkingSet(
            [In] IntPtr ProcessHandle
            );

        [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetMappedFileName(
            [In] IntPtr ProcessHandle,
            [In] IntPtr Address,
            [Out] StringBuilder Buffer,
            [In] int Size
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcessWithTokenW(
            [In] IntPtr TokenHandle,
            [In] LogonFlags Flags,
            [In] [Optional] string ApplicationName,
            [Optional] string CommandLine,
            [In] ProcessCreationFlags CreationFlags,
            [In] [Optional] int Environment,
            [In] [Optional] string CurrentDirectory,
            [In] ref StartupInfo StartupInfo,
            [Out] out ProcessInformation ProcessInfo
            );

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcessAsUser(
            [In] [Optional] IntPtr TokenHandle,
            [In] [Optional] string ApplicationName,
            [Optional] string CommandLine,
            [In] [Optional] IntPtr ProcessAttributes,
            [In] [Optional] IntPtr ThreadAttributes,
            [In] bool InheritHandles,
            [In] ProcessCreationFlags CreationFlags,
            [In] [Optional] IntPtr Environment,
            [In] [Optional] string CurrentDirectory,
            [In] ref StartupInfo StartupInfo,
            [Out] out ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcess(
            [In] [Optional] string ApplicationName,
            [Optional] string CommandLine,
            [In] [Optional] IntPtr ProcessAttributes,
            [In] [Optional] IntPtr ThreadAttributes,
            [In] bool InheritHandles,
            [In] ProcessCreationFlags CreationFlags,
            [In] [Optional] IntPtr Environment,
            [In] [Optional] string CurrentDirectory,
            [In] ref StartupInfo StartupInfo,
            [Out] out ProcessInformation ProcessInformation
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(
            [In] IntPtr ProcessHandle, 
            [Out] out int ExitCode
            );

        // Vista and higher
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryFullProcessImageName(
            [In] IntPtr ProcessHandle, 
            [In] [MarshalAs(UnmanagedType.Bool)] bool UseNativeName,
            [Out] StringBuilder ExeName, 
            ref int Size
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsProcessInJob(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr JobHandle,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool Result
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessAffinityMask(
            [In] IntPtr ProcessHandle,
            [In] IntPtr ProcessAffinityMask
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessAffinityMask(
            [In] IntPtr ProcessHandle, 
            [Out] out IntPtr ProcessAffinityMask,
            [Out] out IntPtr SystemAffinityMask
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckRemoteDebuggerPresent(
            [In] IntPtr ProcessHandle, 
            [MarshalAs(UnmanagedType.Bool)] ref bool DebuggerPresent
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(
            [In] IntPtr ProcessHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessDEPPolicy(
            [In] IntPtr ProcessHandle, 
            [Out] out DepFlags Flags,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool Permanent
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(
            [In] IntPtr ProcessHandle, 
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            [In] ProcessAccess DesiredAccess, 
            [In] bool InheritHandle, 
            [In] int ProcessId
            );

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModules(
            [In] IntPtr ProcessHandle, 
            [Out] IntPtr[] ModuleHandles, 
            [In] int Size, 
            [Out] out int RequiredSize
            );
        
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModulesEx(
            [In] IntPtr ProcessHandle,
            [Out] IntPtr[] ModuleHandles,
            [In] int Size,
            [Out] out int RequiredSize,
            [In] ModuleFilterFlags dwFilterFlag
            );

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleBaseName(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] IntPtr ModuleHandle, 
            [Out] StringBuilder BaseName, 
            [In] int Size
            );

        [DllImport("psapi.dll", CharSet = CharSet.Unicode)]
        public static extern int GetModuleFileNameEx(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] IntPtr ModuleHandle,
            [Out] StringBuilder FileName, 
            [In] int Size
            );

        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetModuleInformation(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] IntPtr ModuleHandle,
            [Out] ModuleInfo ModInfo, 
            [In] int Size
            );

        #endregion

        #region Resources/Handles

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateMutex(
            [In] [Optional] IntPtr attributes,
            [In] bool initialOwner, 
            [In] [Optional] string name
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetHandleInformation(
            [In] IntPtr handle,
            [In] Win32HandleFlags mask,
            [In] Win32HandleFlags flags
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetHandleInformation(
            [In] IntPtr handle,
            [Out] out Win32HandleFlags flags
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(
            [In] IntPtr Handle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern WaitResult WaitForSingleObject(
            [In] IntPtr Object, 
            [In] uint Timeout
            );

        #endregion

        #region Security

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateWellKnownSid(
            [In] WellKnownSidType WellKnownSidType,
            [In] [Optional] IntPtr DomainSid,
            [In] IntPtr Sid,
            ref int SidSize
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EqualDomainSid(
            [In] IntPtr Sid1,
            [In] IntPtr Sid2,
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool Equal
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertStringSidToSid(
            [In] string StringSid,
            [Out] out IntPtr Sid
            );

        [DllImport("aclui.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EditSecurity(
            [In] IntPtr hWnd,
            [MarshalAs(UnmanagedType.Interface)]
            [In] ISecurityInformation SecurityInformation
            );

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSecurityDescriptorDacl(
            [In] IntPtr SecurityDescriptor,
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool DaclPresent,
            [Out] out IntPtr Dacl,
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool DaclDefaulted
            );

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSecurityDescriptorGroup(
            [In] IntPtr SecurityDescriptor,
            [Out] out IntPtr Group,
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool GroupDefaulted
            );

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSecurityDescriptorOwner(
            [In] IntPtr SecurityDescriptor,
            [Out] out IntPtr Owner,
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool OwnerDefaulted
            );

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSecurityDescriptorSacl(
            [In] IntPtr SecurityDescriptor,
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool SaclPresent,
            [Out] out IntPtr Sacl,
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool SaclDefaulted
            );

        [DllImport("advapi32.dll")]
        public static extern Win32Error GetSecurityInfo(
            [In] IntPtr Handle,
            [In] SeObjectType ObjectType,
            [In] SecurityInformation SecurityInformation,
            [Out] [Optional] out IntPtr OwnerSid,
            [Out] [Optional] out IntPtr GroupSid,
            [Out] [Optional] out IntPtr Dacl,
            [Out] [Optional] out IntPtr Sacl,
            [Out] [Optional] out IntPtr SecurityDescriptor
            );

        [DllImport("advapi32.dll")]
        public static extern Win32Error SetSecurityInfo(
            [In] IntPtr Handle,
            [In] SeObjectType ObjectType,
            [In] SecurityInformation SecurityInformation,
            [In] [Optional] IntPtr OwnerSid,
            [In] [Optional] IntPtr GroupSid,
            [In] [Optional] IntPtr Dacl,
            [In] [Optional] IntPtr Sacl
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImpersonateLoggedOnUser(
            [In] IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RevertToSelf();

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LogonUser(
            [In] string Username,
            [In] [Optional] string Domain,
            [In] string Password,
            [In] LogonType LogonType,
            [In] LogonProvider LogonProvider,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            [In] IntPtr ProcessHandle, 
            [In] TokenAccess DesiredAccess,
            [Out] out IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenThreadToken(
            [In] IntPtr ThreadHandle, 
            [In] TokenAccess DesiredAccess,
            [In] bool OpenAsSelf, 
            [Out] out IntPtr TokenHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateTokenEx(
            [In] IntPtr ExistingToken, 
            [In] TokenAccess DesiredAccess,
            [In] [Optional] IntPtr TokenAttributes, 
            [In] SecurityImpersonationLevel ImpersonationLevel, 
            [In] TokenType TokenType,
            [Out] out IntPtr NewToken
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [In] ref int TokenInformation,
            [In] int TokenInformationLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Out] [Optional] IntPtr TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Out] out int TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Out] out IntPtr TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Optional] out TokenSource TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TokenInformationClass TokenInformationClass,
            [Optional] out TokenStatistics TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupAccountName(
            [In] [Optional] string SystemName,
            [In] string AccountName,
            [In] [Optional] IntPtr Sid,
            ref int SidSize,
            [Out] [Optional] StringBuilder ReferencedDomainName,
            ref int ReferencedDomainNameSize,
            [Out] out SidNameUse Use
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupAccountSid(
            [In] [Optional] string SystemName,
            [In] IntPtr Sid,
            [Out] [Optional] StringBuilder Name, 
            ref int NameSize,
            [Out] [Optional] StringBuilder ReferencedDomainName, 
            ref int ReferencedDomainNameSize,
            [Out] out SidNameUse Use
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeDisplayName(
            [In] [Optional] string SystemName, 
            [In] string Name,
            [Out] [Optional] StringBuilder DisplayName, 
            ref int DisplayNameSize,
            [Out] out int LanguageId
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeName(
            [In] [Optional] string SystemName, 
            [In] ref Luid Luid,
            [Out] [Optional] StringBuilder Name, 
            ref int RequiredSize
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(
            [In] [Optional] string SystemName, 
            [In] string PrivilegeName, 
            [Out] out Luid Luid
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenGroups(
            [In] IntPtr TokenHandle,
            [In] [MarshalAs(UnmanagedType.Bool)] bool ResetToDefault,
            [In] [Optional] ref TokenGroups NewState,
            [In] int BufferLength,
            [Out] [Optional] IntPtr PreviousState,
            [Out] [Optional] IntPtr ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
            [In] IntPtr TokenHandle,
            [In] [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
            [In] [Optional] ref TokenPrivileges NewState,
            [In] int BufferLength,
            [Out] [Optional] IntPtr PreviousState, 
            [Out] [Optional] IntPtr ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InitializeSecurityDescriptor(
            IntPtr SecurityDescriptor, 
            [In] int Revision
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetSecurityDescriptorDacl(
            IntPtr SecurityDescriptor,
            [In] [MarshalAs(UnmanagedType.Bool)] bool DaclPresent,
            [In] [Optional] IntPtr Dacl,
            [In] [MarshalAs(UnmanagedType.Bool)] bool DaclDefaulted
            );

        #endregion      

        #region Services

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(
            [In] IntPtr ServiceHandle
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(
            [In] IntPtr Service, 
            [In] int NumServiceArgs, 
            [In] [Optional] string[] Args
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfig(
            [In] IntPtr Service,
            [In] ServiceType ServiceType,
            [In] ServiceStartType StartType,
            [In] ServiceErrorControl ErrorControl, 
            [In] [Optional] string BinaryPath,
            [In] [Optional] string LoadOrderGroup,
            [Out] [Optional] IntPtr TagId, 
            [In] [Optional] string Dependencies,
            [In] [Optional] string StartName,
            [In] [Optional] string Password,
            [In] [Optional] string DisplayName
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ControlService(
            [In] IntPtr Service,
            [In] ServiceControl Control, 
            [Out] out ServiceStatus ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateService(
            [In] IntPtr SCManager,
            [In] string ServiceName,
            [In] [Optional] string DisplayName,
            [In] ServiceAccess DesiredAccess,
            [In] ServiceType ServiceType,
            [In] ServiceStartType StartType,
            [In] ServiceErrorControl ErrorControl,
            [In] [Optional] string BinaryPathName,
            [In] [Optional] string LoadOrderGroup,
            [Out] [Optional] IntPtr TagId,
            [In] [Optional] IntPtr Dependencies,
            [In] [Optional] string ServiceStartName,
            [In] [Optional] string Password
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteService(
            [In] IntPtr Service
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceStatus(
            [In] IntPtr Service, 
            [Out] out ServiceStatus ServiceStatus
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceStatusEx(
            [In] IntPtr Service, 
            [In] int InfoLevel,
            [Out] [Optional] out ServiceStatusProcess ServiceStatus,
            [In] int BufferSize, 
            [Out] out int BytesNeeded
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceConfig(
            [In] IntPtr Service,
            [Out] [Optional] IntPtr ServiceConfig,
            [In] int BufferSize, 
            [Out] out int BytesNeeded
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceConfig2(
            [In] IntPtr Service,
            [In] ServiceInfoLevel InfoLevel,
            [Out] [Optional] IntPtr Buffer, 
            [In] int BufferSize, 
            [Out] out int ReturnLength
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenService(
            [In] IntPtr SCManager,
            [In] string ServiceName, 
            [In] ServiceAccess DesiredAccess
            );

        /// <summary>
        /// Enumerates services in the specified service control manager database. 
        /// The name and status of each service are provided, along with additional 
        /// data based on the specified information level.
        /// </summary>
        /// <param name="SCManager">A handle to the service control manager database.</param>
        /// <param name="InfoLevel">Set this to 0.</param>
        /// <param name="ServiceType">The type of services to be enumerated.</param>
        /// <param name="ServiceState">The state of the services to be enumerated.</param>
        /// <param name="Services">A pointer to the buffer that receives the status information.</param>
        /// <param name="BufSize">The size of the buffer pointed to by the Services parameter, in bytes.</param>
        /// <param name="BytesNeeded">A pointer to a variable that receives the number of bytes needed to 
        /// return the remaining service entries, if the buffer is too small.</param>
        /// <param name="ServicesReturned">A pointer to a variable that receives the number of service 
        /// entries returned.</param>
        /// <param name="ResumeHandle">A pointer to a variable that, on input, specifies the 
        /// starting point of enumeration. You must set this value to zero the first time the 
        /// EnumServicesStatusEx function is called.</param>
        /// <param name="GroupName">Must be 0 for this definition.</param>
        /// <returns>A non-zero value for success, zero for failure.</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumServicesStatusEx(
            [In] IntPtr SCManager,
            [In] IntPtr InfoLevel,
            [In] ServiceQueryType ServiceType,
            [In] ServiceQueryState ServiceState,
            [Out] [Optional] IntPtr Services,
            [In] int BufSize, 
            [Out] out int BytesNeeded,
            [Out] out int ServicesReturned,
            ref int ResumeHandle,
            [In] [Optional] string GroupName
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            [In] [Optional] string MachineName, 
            [In] [Optional] string DatabaseName,
            [In] ScManagerAccess DesiredAccess
            );

        #endregion

        #region Shell

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int ShellAbout(
            [In] [Optional] IntPtr hWnd,
            [In] string App,
            [In] [Optional] string OtherStuff,
            [In] [Optional] IntPtr IconHandle
            );

        [DllImport("shell32.dll", EntryPoint = "#61", CharSet = CharSet.Unicode)]
        public static extern int RunFileDlg(
            [In] IntPtr hWnd,
            [In] IntPtr Icon,
            [In] string Path,
            [In] string Title,
            [In] string Prompt,
            [In] RunFileDialogFlags Flags
            );

        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShellExecuteEx(
            [MarshalAs(UnmanagedType.Struct)] ref ShellExecuteInfo s
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(
            [In] int HookId, 
            [In] IntPtr HookFunction,
            [In] IntPtr Module,
            [In] int ThreadId
            );

        [DllImport("shell32.dll")]
        public extern static int ExtractIconEx(
            [In] string libName,
            [In] int iconIndex,
            [Out] IntPtr[] largeIcon,
            [Out] IntPtr[] smallIcon,
            [In] int nIcons
            );

        [DllImport("shell32.dll")]
        public static extern int SHGetFileInfo(
            [In] string pszPath,
            [In] uint dwFileAttributes,
            [Out] out ShFileInfo psfi,
            [In] uint cbSizeFileInfo,
            [In] uint uFlags);

        [DllImport("shell32.dll", EntryPoint = "#660")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIconInit([In] bool RestoreCache);

        #endregion

        #region Statistics

        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo(
            [Out] out PerformanceInformation PerformanceInformation,
            [In] int Size
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessTimes(
            [In] IntPtr ProcessHandle,
            [Out] out LargeInteger CreationTime,
            [Out] out LargeInteger ExitTime,
            [Out] out LargeInteger KernelTime,
            [Out] out LargeInteger UserTime
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetProcessIoCounters(
            [In] IntPtr ProcessHandle, 
            [Out] out IoCounters IoCounters
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSystemTimes(
            [Out] out LargeInteger IdleTime,
            [Out] out LargeInteger KernelTime,
            [Out] out LargeInteger UserTime
            );

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetThreadTimes(
            [In] IntPtr ThreadHandle,
            [Out] out LargeInteger CreationTime,
            [Out] out LargeInteger ExitTime,
            [Out] out LargeInteger KernelTime,
            [Out] out LargeInteger UserTime
            );

        #endregion

        #region Symbols/Stack Walking

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MiniDumpWriteDump(
            [In] IntPtr ProcessHandle,
            [In] int ProcessId,
            [In] IntPtr FileHandle,
            [In] MinidumpType DumpType,
            [In] IntPtr ExceptionParam,
            [In] IntPtr UserStreamParam,
            [In] IntPtr CallbackParam
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StackWalk64(
            [In] MachineType MachineType,
            [In] IntPtr ProcessHandle,
            [In] IntPtr ThreadHandle,
            ref StackFrame64 StackFrame,
            [In] IntPtr ContextRecord,
            [In] [Optional] ReadProcessMemoryProc64 ReadMemoryRoutine,
            [In] [Optional] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [In] [Optional] GetModuleBaseProc64 GetModuleBaseRoutine,
            [In] [Optional] IntPtr TranslateAddress
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StackWalk64(
            [In] MachineType MachineType,
            [In] IntPtr ProcessHandle,
            [In] IntPtr ThreadHandle,
            ref StackFrame64 StackFrame,
            ref Context ContextRecord,
            [In] [Optional] ReadProcessMemoryProc64 ReadMemoryRoutine,
            [In] [Optional] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [In] [Optional] GetModuleBaseProc64 GetModuleBaseRoutine,
            [In] [Optional] IntPtr TranslateAddress
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StackWalk64(
            [In] MachineType MachineType,
            [In] IntPtr ProcessHandle,
            [In] IntPtr ThreadHandle,
            ref StackFrame64 StackFrame,
            ref ContextAmd64 ContextRecord,
            [In] [Optional] ReadProcessMemoryProc64 ReadMemoryRoutine,
            [In] [Optional] FunctionTableAccessProc64 FunctionTableAccessRoutine,
            [In] [Optional] GetModuleBaseProc64 GetModuleBaseRoutine,
            [In] [Optional] IntPtr TranslateAddress
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymCleanup(
            [In] IntPtr ProcessHandle
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymEnumSymbols(
            [In] IntPtr ProcessHandle,
            [In] ulong BaseOfDll,
            [In] [Optional] string Mask,
            [In] SymEnumSymbolsProc EnumSymbolsCallback,
            [In] [Optional] IntPtr UserContext
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymFromAddr(
            [In] IntPtr ProcessHandle,
            [In] ulong Address,
            [Out] out ulong Displacement,
            [In] IntPtr Symbol
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymFromIndex(
            [In] IntPtr ProcessHandle,
            [In] ulong BaseOfDll,
            [In] int Index,
            IntPtr Symbol
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymFromName(
            [In] IntPtr ProcessHandle,
            [In] string Name,
            [In] IntPtr Symbol
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern IntPtr SymFunctionTableAccess64(
            [In] IntPtr ProcessHandle,
            [In] ulong AddrBase
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymGetLineFromAddr64(
            [In] IntPtr ProcessHandle,
            [In] ulong Address,
            [Out] out int Displacement,
            [Out] out ImagehlpLine64 Line
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern ulong SymGetModuleBase64(
            [In] IntPtr ProcessHandle,
            [In] ulong Address
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern SymbolOptions SymGetOptions();

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymGetSearchPath(
            [In] IntPtr ProcessHandle,
            [Out] StringBuilder SearchPath,
            [In] int SearchPathLength
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymInitialize(
            [In] IntPtr ProcessHandle,
            [In] [Optional] string UserSearchPath,
            [In] bool InvadeProcess
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern long SymLoadModule64(
            [In] IntPtr ProcessHandle,
            [In] [Optional] IntPtr FileHandle,
            [In] [Optional] string ImageName,
            [In] [Optional] string ModuleName,
            [In] ulong BaseOfDll,
            [In] int SizeOfDll
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        public static extern SymbolOptions SymSetOptions(
            [In] SymbolOptions SymOptions
            );

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymSetSearchPath(
            [In] IntPtr ProcessHandle, 
            [In] [Optional] string SearchPath
            );

        [DllImport("dbghelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymUnloadModule64(
            [In] IntPtr ProcessHandle,
            [In] ulong BaseOfDll
            );

        [DllImport("symsrv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymbolServerSetOptions(
            [In] SymbolServerOption Options,
            [In] ulong Data
            );

        #endregion

        #region TCP

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int SetTcpEntry(
            [In] ref MibTcpRow TcpRow
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetExtendedTcpTable(
            [Out] IntPtr Table,
            ref int Size,
            [In] bool Order,
            [In] AiFamily IpVersion,
            [In] TcpTableClass TableClass,
            [In] int Reserved
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetTcpStatistics(
            [Out] out MibTcpStats pStats
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetTcpTable(
            [Out] byte[] tcpTable, 
            ref int pdwSize, 
            [In] bool bOrder
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetTcp6Table(
            [Out] byte[] tcpTable,
            ref int pdwSize,
            [In] bool bOrder);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int AllocateAndGetTcpExTableFromStack(
            [Out] out IntPtr pTable, 
            [In] bool bOrder,
            [In] IntPtr heap,
            [In] int flags,
            [In] int family
            );

        #endregion

        #region Terminal Server

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ProcessIdToSessionId(
            [In] int ProcessId,
            [Out] out int SessionId
            );

        [DllImport("wtsapi32.dll")]
        public static extern void WTSCloseServer(
            [In] IntPtr ServerHandle
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSConnectSession(
            [In] int LogonId,
            [In] int TargetLogonId,
            [In] string Password,
            [In] bool Wait
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSDisconnectSession(
            [In] IntPtr ServerHandle,
            [In] int SessionId,
            [In] bool Wait
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSEnumerateProcesses(
            [In] IntPtr ServerHandle,
            [In] int Reserved,
            [In] int Version,
            [Out] out IntPtr ProcessInfo,
            [Out] out int Count
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSEnumerateSessions(
            [In] IntPtr ServerHandle,
            [In] int Reserved,
            [In] int Version,
            [Out] out IntPtr SessionInfo,
            [Out] out int Count
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSFreeMemory(
            [In] IntPtr Memory
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSLogoffSession(
            [In] IntPtr ServerHandle,
            [In] int SessionId,
            [In] bool Wait
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr WTSOpenServer(
            [In] string ServerName
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSQuerySessionInformation(
            [In] IntPtr ServerHandle,
            [In] int SessionId,
            [In] WtsInformationClass InfoClass,
            [Out] out IntPtr Buffer,
            [Out] out int BytesReturned
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSRegisterSessionNotification(
            [In] IntPtr hWnd,
            [In] WtsNotificationFlags Flags
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSRegisterSessionNotificationEx(
            [In] IntPtr ServerHandle,
            [In] IntPtr hWnd,
            [In] WtsNotificationFlags Flags
            );

        [DllImport("wtsapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSSendMessage(
            [In] IntPtr ServerHandle,
            [In] int SessionId,
            [In] string Title,
            [In] int TitleLength,
            [In] string Message,
            [In] int MessageLength,
            [In] int Style,
            [In] int Timeout,
            [Out] out DialogResult Response,
            [In] bool Wait
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSShutdownSystem(
            [In] IntPtr ServerHandle,
            [In] WtsShutdownFlags ShutdownFlag
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSTerminateProcess(
            [In] IntPtr ServerHandle,
            [In] int ProcessId,
            [In] int ExitCode
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSUnRegisterSessionNotification(
            [In] IntPtr hWnd
            );

        [DllImport("wtsapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WTSUnRegisterSessionNotificationEx(
            [In] IntPtr ServerHandle,
            [In] IntPtr hWnd
            );

        #endregion

        #region Threads

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryThreadCycleTime(
            [In] IntPtr ThreadHandle, 
            [Out] out ulong CycleTime
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueueUserAPC(
            [In] IntPtr APC,
            [In] IntPtr ThreadHandle,
            [In] IntPtr Data
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueueUserAPC(
            [MarshalAs(UnmanagedType.FunctionPtr)]
            [In] ApcRoutine APC,
            [In] IntPtr ThreadHandle,
            [In] IntPtr Data
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeThread(
            [In] IntPtr ThreadHandle,
            [Out] out int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetThreadPriority(
            [In] IntPtr ThreadHandle, 
            [In] int Priority
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadPriority([In] IntPtr ThreadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateThread(
            [In] [Optional] IntPtr ThreadAttributes,
            [In] int StackSize,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] ThreadStart StartAddress,
            [In] IntPtr Parameter,
            [In] int CreationFlags,
            [Out] out int ThreadId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessIdOfThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetThreadId(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(
            [In] ThreadAccess DesiredAccess, 
            [In] bool InheritHandle, 
            [In] int ThreadId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateThread(
            IntPtr ThreadHandle,
            [In] int ExitCode
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SuspendThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(
            [In] IntPtr ThreadHandle
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetThreadContext(
            [In] IntPtr ThreadHandle, 
            [In] ref Context Context
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetThreadContext(
            [In] IntPtr ThreadHandle, 
            ref Context Context
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(
            [In] IntPtr ProcessHandle,
            [In] IntPtr ThreadAttributes,
            [In] IntPtr StackSize,
            [In] IntPtr StartAddress,
            [In] IntPtr Parameter,
            [In] ProcessCreationFlags CreationFlags,
            [Out] out int ThreadId
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();

        #endregion

        #region Toolhelp

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(
            [In] SnapshotFlags dwFlags,
            [In] int th32ProcessID
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32First(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ProcessEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32Next(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out ProcessEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Thread32First(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ThreadEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Thread32Next(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out ThreadEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Module32First(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref ModuleEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Module32Next(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out ModuleEntry32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Heap32ListFirst(
            [In] IntPtr hSnapshot,
            [MarshalAs(UnmanagedType.Struct)] ref HeapList32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Heap32ListNext(
            [In] IntPtr hSnapshot,
            [Out] [MarshalAs(UnmanagedType.Struct)] out HeapList32 lppe
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Heap32First(
            [MarshalAs(UnmanagedType.Struct)] ref HeapEntry32 lppe,
            [In] int ProcessID,
            [In] IntPtr HeapID
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Heap32Next(
            [Out] [MarshalAs(UnmanagedType.Struct)] out HeapEntry32 lppe
            );

        #endregion

        #region UDP

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int GetExtendedUdpTable(
            [Out] IntPtr Table, 
            ref int Size,
            [In] bool Order,
            [In] AiFamily IpVersion,
            [In] UdpTableClass TableClass,
            [In] int Reserved
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpStatistics(
            [Out] out MibUdpStats pStats
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetUdpTable(
            [Out] byte[] udpTable, 
            ref int pdwSize, 
            [In] bool bOrder
            );

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public extern static int AllocateAndGetUdpExTableFromStack(
            [Out] out IntPtr pTable, 
            [In] bool bOrder,
            [In] IntPtr heap,
            [In] int flags,
            [In] int family
            );

        #endregion

        #region User

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwitchDesktop(
            [In] IntPtr DesktopHandle
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetUserObjectSecurity(
            [In] IntPtr Handle,
            [In] ref SiRequested SiRequested,
            [In] IntPtr Sid
            );

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenDesktop(
            [In] string Desktop,
            [In] int Flags,
            [In] bool Inherit,
            [In] DesktopAccess DesiredAccess
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseDesktop(
            [In] IntPtr Handle
            );

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenWindowStation(
            [In] string WinSta,
            [In] bool Inherit,
            [In] WindowStationAccess DesiredAccess
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseWindowStation(
            [In] IntPtr Handle
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(
            [In] int ThreadId
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetThreadDesktop(
            [In] IntPtr DesktopHandle
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessWindowStation(
            [In] IntPtr WindowStationHandle
            );

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateEnvironmentBlock(
            [Out] out IntPtr Environment,
            [In] IntPtr TokenHandle,
            [In] bool Inherit
            );

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LoadUserProfile(
            [In] IntPtr TokenHandle,
            ref ProfileInformation ProfileInfo
            );

        [DllImport("userenv.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnloadUserProfile(
            [In] IntPtr TokenHandle,
            [In] IntPtr ProfileHandle
            );

        #endregion

        #region Windows

        [DllImport("user32.dll")]
        public static extern bool EndTask(
            [In] IntPtr hWnd,
            [In] bool ShutDown,
            [In] bool Force
            );

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(
            [In] IntPtr hWnd,
            [In] int X,
            [In] int Y,
            [In] int Width,
            [In] int Height,
            [In] bool Repaint
            );

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(
            [In] int Index
            );

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(
            [In] IntPtr hWnd,
            [In] IntPtr Rect,
            [In] bool Erase
            );

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(
            [In] IntPtr hWnd,
            [In] ref Rect Rect,
            [In] bool Erase
            );

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(
            [In] IntPtr hWnd,
            [In] IntPtr UpdateRect,
            [In] IntPtr UpdateRgn,
            [In] RedrawWindowFlags Flags
            );

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(
            [In] IntPtr hWnd,
            [In] IntPtr hDC
            );

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(
            [In] Point location
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(
            [Out] out Point location
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeWindowMessageFilter(
            [In] WindowMessage message, 
            [In] UipiFilterFlag flag
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(
            [In] IntPtr hWnd, 
            [In] WindowMessage msg,
            [In] int w,
            [In] int l
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            [In] IntPtr hWnd,
            [In] WindowMessage msg,
            [In] int w,
            [In] int l,
            [In] SmtoFlags flags,
            [In] int timeout,
            [Out] out int result
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(
            [In] IntPtr hWnd,
            [In] WindowMessage msg,
            [In] int w,
            [In] int l
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllowSetForegroundWindow(
            [In] int processId
            );

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern HResult SetWindowTheme(
            [In] IntPtr hWnd,
            [In] string appName,
            [In] string idList
            );

        [DllImport("user32.dll")]
        public static extern int GetGuiResources(
            [In] IntPtr ProcessHandle,
            [In] int UserObjects
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(
            [In] IntPtr Handle
            );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] EnumWindowsProc Callback,
            [In] int param
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(
            [In] int ThreadId,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] EnumThreadWndProc callback,
            [In] int Param
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(
            [In] IntPtr hWnd,
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] EnumChildProc callback,
            [In] int param
            );

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(
            [In] IntPtr hWnd,
            [Out] out int processId
            );

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(
            [Out] out Message msg, 
            [In] IntPtr hWnd,
            [In] uint messageFilterMin,
            [In] uint messageFilterMax,
            [In] PeekMessageFlags flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(
            [In] ref Message msg
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern IntPtr DispatchMessage(
            [In] ref Message msg
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(
            [In] IntPtr hWnd, 
            [In] WindowMessage msg,
            [In] IntPtr wParam,
            [In] IntPtr lParam
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(
            [In] int exitCode
            );

#if _WIN64
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
#else
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
#endif
		private static extern IntPtr SetWindowLongPtr(
            [In] IntPtr hWnd,
            [In] GetWindowLongOffset Index, 
            [In] [MarshalAs(UnmanagedType.FunctionPtr)] WndProcDelegate WndProc
            );

#if _WIN64
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
#else
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
#endif
        public static extern IntPtr SetWindowLongPtr(
            [In] IntPtr hWnd,
            [In] GetWindowLongOffset Index,
            [In] IntPtr NewLong
            );

#if _WIN64
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
#else
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
#endif
        public static extern IntPtr GetWindowLongPtr(
            [In] IntPtr hWnd,
            [In] GetWindowLongOffset Index
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(
            [In] IntPtr hWnd,
            [Out] out Rect rect
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(
            [In] IntPtr hWnd,
            [Out] out Rect rect
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            [In] IntPtr hWnd,
            [In] IntPtr hWndAfter,
            [In] int x,
            [In] int y,
            [In] int w,
            [In] int h,
            [In] uint flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ScreenToClient(
            [In] IntPtr hWnd, 
            ref Point point
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetFocus(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(
            [In] IntPtr hWnd, 
            [Out] out MonitorInformation info
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MonitorFromWindow(
            [In] IntPtr hWnd, 
            [In] uint flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(
            [In] uint Key
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetCapture(
            [In] IntPtr handle
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(
            [In] IntPtr hWnd, 
            [In] ShowWindowType flags
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenu(
            [In] IntPtr hWnd, 
            [In] IntPtr menuHandle
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseWindow(
            [In] IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustWindowRect(
            ref Rect rect, 
            [In] WindowStyles style,
            [In] [MarshalAs(UnmanagedType.Bool)]bool menu
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterClass(
            [In] ref WindowClass wndClass
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterClass(
            [In] [MarshalAs(UnmanagedType.LPTStr)] string className,
            [In] IntPtr instanceHandle
            );

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindow(
            [In] int ExStyle,
            [In] [MarshalAs(UnmanagedType.LPTStr)] string ClassName,
            [In] [MarshalAs(UnmanagedType.LPTStr)] string WindowName,
            [In] WindowStyles Style,
            [In] int X,
            [In] int Y,
            [In] int Width,
            [In] int Height,
            [In] IntPtr Parent,
            [In] IntPtr MenuHandle,
            [In] IntPtr InstanceHandle,
            [In] IntPtr Zero
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetCaretBlinkTime();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int InternalGetWindowText(
            [In] IntPtr hWnd, 
            [In] IntPtr String, 
            [In] int MaxCount
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool IsHungAppWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool IsWindow(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool IsWindowVisible(
            [In] IntPtr hWnd
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(
            [In] IntPtr hWnd,
            ref WindowPlacement WindowPlacement
            );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(
            [In] string ClassName,
            [In] string WindowName
            );

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        #endregion

        #region Window Stations

        [DllImport("winsta.dll", SetLastError = true)]
        public static extern bool WinStationRevertFromServicesSession();

        [DllImport("winsta.dll", SetLastError = true)]
        public static extern bool WinStationSwitchToServicesSession();

        [DllImport("winsta.dll", SetLastError = true)]
        public static extern bool WinStationTerminateProcess(
            [In] IntPtr ServerHandle,
            [In] int ProcessId,
            [In] int ExitCode
            );

        #endregion
    }
}
