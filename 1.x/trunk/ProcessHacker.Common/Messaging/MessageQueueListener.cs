using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Messaging
{
    public delegate void MessageReceivedDelegate(Message message);

    public class MessageQueueListener
    {
        private MessageReceivedDelegate _callback;
        private Type _type;

        public MessageQueueListener(MessageReceivedDelegate callback, Type type)
        {
            _callback = callback;
            _type = type;
        }

        public MessageReceivedDelegate Callback
        {
            get { return _callback; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }

    public class MessageQueueListener<T> : MessageQueueListener
        where T : Message
    {
        public delegate void MessageReceivedDelegate(T message);

        public MessageQueueListener(MessageReceivedDelegate callback)
            : base((message) => callback((T)message), typeof(T))
        { }
    }
}
