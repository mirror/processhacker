using System;
using System.Threading;
using ProcessHacker.Common;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Objects;
using ProcessHacker.Native.Security;
using ProcessHacker.Native.Threading;

namespace ProcessHacker.Native.SsLogging
{
    public delegate void ArgumentBlockReceivedDelegate(SsData argBlock);
    public delegate void EventBlockReceivedDelegate(SsEvent eventBlock);
    public delegate void RawArgumentBlockReceivedDelegate(MemoryRegion argBlock);
    public delegate void RawEventBlockReceivedDelegate(MemoryRegion eventBlock);

    public sealed class SsLogger
    {
        private const int _highBlockSize = 0x200;

        public static SsData ReadArgumentBlock(MemoryRegion data)
        {
            KphSsArgumentBlock argBlock = data.ReadStruct<KphSsArgumentBlock>(0, KphSsArgumentBlock.SizeOf, 0);

            MemoryRegion dataRegion;
            SsData ssArg = null;

            dataRegion = new MemoryRegion(data, KphSsArgumentBlock.DataOffset);

            // Process the argument block based on its type.
            switch (argBlock.Type)
            {
                case KphSsArgumentType.Int8:
                    {
                        SsSimple simpleArg = new SsSimple
                        {
                            Argument = argBlock.Data.Int8, 
                            Type = typeof(Byte)
                        };

                        ssArg = simpleArg;
                    }
                    break;
                case KphSsArgumentType.Int16:
                    {
                        SsSimple simpleArg = new SsSimple
                        {
                            Argument = argBlock.Data.Int16, 
                            Type = typeof(Int16)
                        };

                        ssArg = simpleArg;
                    }
                    break;
                case KphSsArgumentType.Int32:
                    {
                        SsSimple simpleArg = new SsSimple
                        {
                            Argument = argBlock.Data.Int32, 
                            Type = typeof(Int32)
                        };

                        ssArg = simpleArg;
                    }
                    break;
                case KphSsArgumentType.Int64:
                    {
                        SsSimple simpleArg = new SsSimple
                        {
                            Argument = argBlock.Data.Int64, 
                            Type = typeof(Int64)
                        };

                        ssArg = simpleArg;
                    }
                    break;
                case KphSsArgumentType.Handle:
                    {
                        ssArg = new SsHandle(dataRegion);
                    }
                    break;
                case KphSsArgumentType.UnicodeString:
                    {
                        ssArg = new SsUnicodeString(dataRegion);
                    }
                    break;
                case KphSsArgumentType.ObjectAttributes:
                    {
                        ssArg = new SsObjectAttributes(dataRegion);
                    }
                    break;
                case KphSsArgumentType.ClientId:
                    {
                        ssArg = new SsClientId(dataRegion);
                    }
                    break;
                case KphSsArgumentType.Bytes:
                    {
                        ssArg = new SsBytes(dataRegion);
                    }
                    break;
            }

            ssArg.Index = argBlock.Index;

            return ssArg;
        }

        public static SsEvent ReadEventBlock(MemoryRegion data)
        {
            KphSsEventBlock eventBlock = data.ReadStruct<KphSsEventBlock>(0, KphSsEventBlock.SizeOf, 0);

            // Reconstruct the argument and stack trace arrays.

            int[] arguments = new int[eventBlock.NumberOfArguments];
            IntPtr[] stackTrace = new IntPtr[eventBlock.TraceCount];

            for (int i = 0; i < arguments.Length; i++)
                arguments[i] = data.ReadInt32(eventBlock.ArgumentsOffset, i);
            for (int i = 0; i < stackTrace.Length; i++)
                stackTrace[i] = data.ReadIntPtr(eventBlock.TraceOffset, i);

            // Create an event object.
            SsEvent ssEvent = new SsEvent
            {
                // Basic information
                Time = DateTime.FromFileTime(eventBlock.Time), 
                ThreadId = eventBlock.ClientId.ThreadId, 
                ProcessId = eventBlock.ClientId.ProcessId, 
                Arguments = arguments, 
                StackTrace = stackTrace, ArgumentsCopyFailed = 
                eventBlock.Flags.HasFlag(KphSsEventFlags.CopyArgumentsFailed), 
                ArgumentsProbeFailed = eventBlock.Flags.HasFlag(KphSsEventFlags.ProbeArgumentsFailed), 
                CallNumber = eventBlock.Number
            };

            // Flags
            if ((eventBlock.Flags & KphSsEventFlags.UserMode) == KphSsEventFlags.UserMode)
                ssEvent.Mode = KProcessorMode.UserMode;
            else
                ssEvent.Mode = KProcessorMode.KernelMode;

            return ssEvent;
        }

        public static string ReadWString(MemoryRegion data)
        {
            KphSsWString wString = data.ReadStruct<KphSsWString>(0, KphSsWString.SizeOf, 0);

            return data.ReadUnicodeString(KphSsWString.BufferOffset, wString.Length / 2);
        }

        public event ArgumentBlockReceivedDelegate ArgumentBlockReceived;
        public event EventBlockReceivedDelegate EventBlockReceived;
        public event RawArgumentBlockReceivedDelegate RawArgumentBlockReceived;
        public event RawEventBlockReceivedDelegate RawEventBlockReceived;

        private bool _started;
        private readonly object _startLock = new object();

        private bool _terminating;
        private Thread _bufferWorkerThread;
        private ThreadHandle _bufferWorkerThreadHandle;
        private readonly Event _bufferWorkerThreadReadyEvent = new Event(true, false);

        private readonly VirtualMemoryAlloc _buffer;
        private readonly SemaphoreHandle _readSemaphore;
        private readonly SemaphoreHandle _writeSemaphore;
        private int _cursor;

        public SsLogger(int bufferedBlockCount, bool includeAll)
        {
            // Allocate a buffer.
            _buffer = new VirtualMemoryAlloc(_highBlockSize * bufferedBlockCount);

            // Create the read and write semaphores.

            // Read semaphore: no blocks initially, so 0 initial count.
            _readSemaphore = SemaphoreHandle.Create(SemaphoreAccess.All, 0, bufferedBlockCount);
            // Write semaphore: all buffer blocks available so max. initial count.
            _writeSemaphore = SemaphoreHandle.Create(SemaphoreAccess.All, bufferedBlockCount, bufferedBlockCount);

            // Create the client entry.
            //_clientEntryHandle = KProcessHacker.Instance.SsCreateClientEntry(
            //    ProcessHandle.Current,
            //    _readSemaphore,
            //    _writeSemaphore,
            //    _buffer,
            //    _buffer.Size
            //    );

            // Create the ruleset entry.
            //_ruleSetEntryHandle = KProcessHacker.Instance.SsCreateRuleSetEntry(
            //    _clientEntryHandle,
            //    includeAll ? KphSsFilterType.Include : KphSsFilterType.Exclude,
            //    KphSsRuleSetAction.Log
            //    );
        }

        //public IntPtr AddNumberRule(FilterType filterType, int number)
        //{
        //    return KProcessHacker.Instance.SsAddNumberRule(
        //        _ruleSetEntryHandle,
        //        filterType.ToKphSs(),
        //        number
        //        );
        //}

        //public IntPtr AddPreviousModeRule(FilterType filterType, KProcessorMode previousMode)
        //{
        //    return KProcessHacker.Instance.SsAddPreviousModeRule(
        //        _ruleSetEntryHandle,
        //        filterType.ToKphSs(),
        //        previousMode
        //        );
        //}

        //public IntPtr AddProcessIdRule(FilterType filterType, int pid)
        //{
        //    return KProcessHacker.Instance.SsAddProcessIdRule(
        //        _ruleSetEntryHandle,
        //        filterType.ToKphSs(),
        //        pid.ToIntPtr()
        //        );
        //}

        //public IntPtr AddThreadIdRule(FilterType filterType, int tid)
        //{
        //    return KProcessHacker.Instance.SsAddProcessIdRule(
        //        _ruleSetEntryHandle,
        //        filterType.ToKphSs(),
        //        tid.ToIntPtr()
        //        );
        //}

        private void BufferWorkerThreadStart()
        {
            // Open a handle to the current thread so other functions 
            // can alert us.
            _bufferWorkerThreadHandle = ThreadHandle.OpenCurrent(ThreadAccess.All);

            // We're ready.
            _bufferWorkerThreadReadyEvent.Set();

            while (!_terminating)
            {
                NtStatus status;
                KphSsBlockHeader blockHeader;

                // Wait for a block to read (enable alerting so we can 
                // be interrupted if someone wants us to stop).
                status = _readSemaphore.Wait(true);

                // Did we get alerted?
                if (status == NtStatus.Alerted)
                    return;

                // Check if we have an implicit cursor reset.
                if (_buffer.Size - _cursor < KphSsBlockHeader.SizeOf)
                    _cursor = 0;

                // Read the block header.
                blockHeader = _buffer.ReadStruct<KphSsBlockHeader>(_cursor, KphSsBlockHeader.SizeOf, 0);

                // Check if we have an explicit cursor reset.
                if (blockHeader.Type == KphSsBlockType.Reset)
                {
                    _cursor = 0;
                    blockHeader = _buffer.ReadStruct<KphSsBlockHeader>(_cursor, KphSsBlockHeader.SizeOf, 0);
                }

                // Process the block.
                switch (blockHeader.Type)
                {
                    case KphSsBlockType.Event:
                        if (this.EventBlockReceived != null)
                            this.EventBlockReceived(ReadEventBlock(new MemoryRegion(this._buffer, this._cursor)));
                        if (this.RawEventBlockReceived != null)
                            this.RawEventBlockReceived(new MemoryRegion(this._buffer, this._cursor));
                        break;
                    case KphSsBlockType.Argument:
                        if (this.ArgumentBlockReceived != null)
                            this.ArgumentBlockReceived(ReadArgumentBlock(new MemoryRegion(this._buffer, this._cursor)));
                        if (this.RawArgumentBlockReceived != null)
                            this.RawArgumentBlockReceived(new MemoryRegion(this._buffer, this._cursor));
                        break;
                }

                // Advance the cursor.
                _cursor += blockHeader.Size;
                // Signal that a buffer block is available for writing.
                _writeSemaphore.Release();
            }
        }

        public void GetStatistics(out int blocksWritten, out int blocksDropped)
        {
            //KphSsClientInformation info;
            //KProcessHacker.Instance.SsQueryClientEntry(
            //    _clientEntryHandle,
            //    out info,
            //    KphSsClientInformation.SizeOf,
            //    out retLength
            //    );

            //blocksWritten = info.NumberOfBlocksWritten;
            //blocksDropped = info.NumberOfBlocksDropped;
            
            blocksWritten = 0;
            blocksDropped = 0;
        }

        public void RemoveRule(IntPtr handle)
        {
            //KProcessHacker.Instance.SsRemoveRule(_ruleSetEntryHandle, handle);
        }

        public void Start()
        {
            lock (_startLock)
            {
                if (!_started)
                {
                    //KProcessHacker.Instance.SsRef();
                    //KProcessHacker.Instance.SsEnableClientEntry(_clientEntryHandle, true);
                    _started = true;

                    _terminating = false;

                    // Create the buffer worker thread.
                    _bufferWorkerThread = new Thread(this.BufferWorkerThreadStart, Utils.SixteenthStackSize)
                    {
                        IsBackground = true
                    };
                    _bufferWorkerThread.Start();
                    // Wait for the thread to initialize.
                    _bufferWorkerThreadReadyEvent.Wait();
                }
            }
        }

        public void Stop()
        {
            lock (_startLock)
            {
                if (_started)
                {
                    //KProcessHacker.Instance.SsEnableClientEntry(_clientEntryHandle, false);
                    //KProcessHacker.Instance.SsUnref();
                    _started = false;

                    // Tell the worker thread to stop.
                    _terminating = true;
                    // Alert it just in case it is waiting.
                    _bufferWorkerThreadHandle.Alert();
                    // Wait for the worker thread to terminate.
                    _bufferWorkerThreadHandle.Wait();
                    // Close the thread handle.
                    _bufferWorkerThreadHandle.Dispose();
                }
            }
        }
    }
}
