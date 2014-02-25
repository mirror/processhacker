using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker.Common.Messaging
{
    public class MessageQueue
    {
        private Queue<Message> _queue = new Queue<Message>();
        private List<MessageQueueListener> _listeners = new List<MessageQueueListener>();

        public MessageQueue()
        {
            // Action message listener.
            this.AddListener(new MessageQueueListener<ActionMessage>((action) => action.Action()));
        }

        public void AddListener(MessageQueueListener listener)
        {
            lock (_listeners)
                _listeners.Add(listener);
        }

        public void Enqueue(Message message)
        {
            lock (_queue)
                _queue.Enqueue(message);
        }

        public void EnqueueAction(Action action)
        {
            this.Enqueue(new ActionMessage(action));
        }

        public void Listen()
        {
            lock (_queue)
            {
                // Start dequeuing.
                while (_queue.Count > 0)
                {
                    Message message = _queue.Dequeue();

                    // Look for receivers.
                    lock (_listeners)
                    {
                        foreach (MessageQueueListener listener in _listeners)
                        {
                            // If this listener is of the right type, execute the callback.
                            if (listener.Type.IsInstanceOfType(message))
                                listener.Callback(message);
                        }
                    }
                }
            }
        }

        public void RemoveListener(MessageQueueListener listener)
        {
            lock (_listeners)
                _listeners.Remove(listener);
        }
    }
}
