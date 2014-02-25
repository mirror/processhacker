using System;
using System.Runtime.InteropServices;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.SsLogging
{
    public enum KphSsArgumentType : byte
    {
        Normal = 0,
        Int8,
        Int16,
        Int32,
        Int64,
        Handle,
        String,
        WString,
        AnsiString,
        UnicodeString,
        ObjectAttributes,
        ClientId,
        Context,
        InitialTeb,
        Guid,
        Bytes
    }

    public enum KphSsBlockType : ushort
    {
        Reset,
        Event,
        Argument
    }

    [Flags]
    public enum KphSsEventFlags : ushort
    {
        ProbeArgumentsFailed = 0x1,
        CopyArgumentsFailed = 0x2,
        KernelMode = 0x4,
        UserMode = 0x8
    }

    public enum KphSsFilterType : int
    {
        Include,
        Exclude
    }

    [Flags]
    public enum KphSsModeFlags : int
    {
        UserMode = 0x1,
        KernelMode = 0x2
    }

    public enum KphSsRuleSetAction : int
    {
        Log
    }

    public class KphSsClientEntryHandle : KphHandle
    {
        internal KphSsClientEntryHandle(IntPtr handle)
            : base(handle)
        { }
    }

    public class KphSsRuleSetEntryHandle : KphHandle
    {
        internal KphSsRuleSetEntryHandle(IntPtr handle)
            : base(handle)
        { }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsArgumentBlock
    {
        public static readonly int DataOffset = Marshal.OffsetOf(typeof(KphSsArgumentBlock), "Data").ToInt32();

        [StructLayout(LayoutKind.Explicit)]
        public struct KphSsArgumentUnion
        {
            [FieldOffset(0)]
            public int Normal;
            [FieldOffset(0)]
            public byte Int8;
            [FieldOffset(0)]
            public short Int16;
            [FieldOffset(0)]
            public int Int32;
            [FieldOffset(0)]
            public long Int64;
        }

        public KphSsBlockHeader Header;
        public byte Index;
        public KphSsArgumentType Type;
        public KphSsArgumentUnion Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsBlockHeader
    {
        public ushort Size;
        public KphSsBlockType Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsBytes
    {
        public static readonly int BufferOffset = Marshal.OffsetOf(typeof(KphSsBytes), "Buffer").ToInt32();

        public ushort Length;
        public byte Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsClientInformation
    {
        public IntPtr ProcessId;
        public IntPtr BufferBase;
        public int BufferSize;
        public int NumberOfBlocksWritten;
        public int NumberOfBlocksDropped;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsEventBlock
    {
        public KphSsBlockHeader Header;
        public KphSsEventFlags Flags;
        public long Time;
        public ClientId ClientId;

        public int Number;
        public ushort NumberOfArguments;
        public ushort ArgumentsOffset;

        public ushort TraceCount;
        public ushort TraceOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsHandle
    {
        public ClientId ClientId;
        public ushort TypeNameOffset;
        public ushort NameOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsObjectAttributes
    {
        public ObjectAttributes ObjectAttributes;
        public ushort RootDirectoryOffset;
        public ushort ObjectNameOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsUnicodeString
    {
        public static readonly int BufferOffset = Marshal.OffsetOf(typeof(KphSsUnicodeString), "Buffer").ToInt32();

        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Pointer;
        public byte Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KphSsWString
    {
        public static readonly int BufferOffset = Marshal.OffsetOf(typeof(KphSsWString), "Buffer").ToInt32();

        public ushort Length;
        public byte Buffer;
    }
}
