using System;
using System.Collections.Generic;
using System.Text;
using ProcessHacker.Native.Api;
using ProcessHacker.Native.Security;
using System.Runtime.InteropServices;
using ProcessHacker.Common.Objects;

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
        private MemoryAlloc _data;

        public PortMessage(MemoryAlloc data, short dataLength, ClientId clientId, int messageId)
        {
            if (dataLength > Win32.PortMessageMaxDataLength)
                throw new ArgumentOutOfRangeException("Data length is too large.");
            if (dataLength < 0)
                throw new ArgumentOutOfRangeException("Data length cannot be negative.");

            _message = new PortMessageStruct();
            _message.DataLength = dataLength;
            _message.TotalLength = (short)(_portMessageSize + dataLength);
            _message.ClientId = clientId;
            _message.MessageId = messageId;
            _data = data;
            _data.Reference();
        }

        internal PortMessage(MemoryAlloc headerAndData)
        {
            _message = headerAndData.ReadStruct<PortMessageStruct>();
            _data = new MemoryAlloc(headerAndData.Memory.Increment(_portMessageSize), _message.DataLength, false);
            _data.Reference();
        }

        protected override void DisposeObject(bool disposing)
        {
            _data.Dereference(disposing);
        }

        public ClientId ClientId
        {
            get { return _message.ClientId; }
            set { _message.ClientId = value; }
        }

        public MemoryAlloc Data
        {
            get { return _data; }
        }

        public int DataLength
        {
            get { return _message.DataLength; }
        }

        public int MessageId
        {
            get { return _message.MessageId; }
        }

        public PortMessageType Type
        {
            get { return _message.Type; }
        }

        public MemoryAlloc ToMemory()
        {
            MemoryAlloc data = new MemoryAlloc(_portMessageSize + _message.DataLength);

            data.WriteStruct<PortMessageStruct>(_message);
            data.WriteMemory(_portMessageSize, _data, 0, _message.DataLength);

            return data;
        }
    }
}
