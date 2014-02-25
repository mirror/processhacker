using System;
using System.Runtime.InteropServices;
using ProcessHacker.Common.Objects;
using ProcessHacker.Native.Api;

namespace ProcessHacker.Native.Lpc
{
    public class PortMessage : BaseObject
    {
        private static readonly int _portMessageSize = Marshal.SizeOf(typeof(PortMessageStruct));

        public static MemoryAlloc AllocateBuffer()
        {
            return new MemoryAlloc(Win32.PortMessageMaxLength);
        }

        private PortMessageStruct _message;
        private MemoryRegion _data;
        private MemoryRegion _referencedData;

        public PortMessage(byte[] data)
            : this(null, data)
        { }

        public PortMessage(PortMessage existingMessage, byte[] data)
        {
            using (var alloc = new MemoryAlloc(data.Length))
            {
                alloc.WriteBytes(0, data);
                this.InitializeMessage(existingMessage, alloc, (short)alloc.Size);
            }
        }

        public PortMessage(MemoryRegion data, short dataLength)
            : this(null, data, dataLength)
        { }

        public PortMessage(PortMessage existingMessage, MemoryRegion data, short dataLength)
        {
            this.InitializeMessage(existingMessage, data, dataLength);
        }

        internal PortMessage(MemoryRegion headerAndData)
        {
            _message = headerAndData.ReadStruct<PortMessageStruct>();
            _data = new MemoryRegion(headerAndData, _portMessageSize, _message.DataLength);

            _referencedData = headerAndData;
            _referencedData.Reference();
        }

        protected override void DisposeObject(bool disposing)
        {
            _referencedData.Dereference(disposing);
        }

        public ClientId ClientId
        {
            get { return _message.ClientId; }
            set { _message.ClientId = value; }
        }

        public MemoryRegion Data
        {
            get { return _data; }
        }

        public int DataLength
        {
            get { return _message.DataLength; }
        }

        internal PortMessageStruct Header
        {
            get { return _message; }
        }

        public int MessageId
        {
            get { return _message.MessageId; }
        }

        public PortMessageType Type
        {
            get { return _message.Type; }
        }

        private void InitializeMessage(PortMessage existingMessage, MemoryRegion data, short dataLength)
        {
            if (dataLength > Win32.PortMessageMaxDataLength)
                throw new ArgumentOutOfRangeException("Data length is too large.");
            if (dataLength < 0)
                throw new ArgumentOutOfRangeException("Data length cannot be negative.");

            _message = new PortMessageStruct();

            _message.DataLength = dataLength;
            _message.TotalLength = (short)(_portMessageSize + dataLength);
            _message.DataInfoOffset = 0;

            if (existingMessage != null)
            {
                _message.ClientId = existingMessage.ClientId;
                _message.MessageId = existingMessage.MessageId;
            }

            _data = data;

            _referencedData = data;
            _referencedData.Reference();
        }

        internal void SetHeader(MemoryRegion data)
        {
            _message = data.ReadStruct<PortMessageStruct>();
        }

        public MemoryAlloc ToMemory()
        {
            MemoryAlloc data = new MemoryAlloc(_portMessageSize + _message.DataLength);

            data.WriteStruct<PortMessageStruct>(_message);
            data.WriteMemory(_portMessageSize, _data, _message.DataLength);

            return data;
        }
    }
}
