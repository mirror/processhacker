using System.Collections;
using System.Collections.Generic;

namespace ProcessHacker.Common.Threading
{
    public class WaitableQueue<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly SemaphorePair _pair;

        public WaitableQueue()
            : this(int.MaxValue)
        { }

        public WaitableQueue(int maximumCount)
        {
            _pair = new SemaphorePair(maximumCount);
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public void Clear()
        {
            lock (_queue)
                _queue.Clear();
        }

        public bool Contains(T item)
        {
            lock (_queue)
                return _queue.Contains(item);
        }

        public T Dequeue()
        {
            // Wait for an item to dequeue.
            _pair.WaitRead();
            // Release a slot.
            _pair.ReleaseWrite();

            lock (_queue)
                return _queue.Dequeue();
        }

        public bool Dequeue(int timeout, out T item)
        {
            // Wait for an item to dequeue.
            bool waitResult = _pair.WaitRead(timeout);

            // Dequeue an item if we waited successfully, 
            // otherwise pass the default value back.
            if (waitResult)
            {
                lock (_queue)
                    item = _queue.Dequeue();

                // We just dequeued an item, so we can 
                // release a slot.
                _pair.ReleaseWrite();
            }
            else
            {
                item = default(T);
            }

            return waitResult;
        }

        public void Enqueue(T item)
        {
            // Make sure we have an available slot.
            _pair.WaitWrite();

            // Enqueue the item.
            lock (_queue)
                _queue.Enqueue(item);

            // Unwait one dequeuer.
            _pair.ReleaseRead();
        }

        public IEnumerator GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public T[] ToArray()
        {
            lock (_queue)
                return _queue.ToArray();
        }

        public void TrimExcess()
        {
            lock (_queue)
                _queue.TrimExcess();
        }
    }
}
