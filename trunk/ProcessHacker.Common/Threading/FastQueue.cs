using System;
using System.Collections;
using System.Collections.Generic;

namespace ProcessHacker.Common.Threading
{
    public class FastQueue<T> : IEnumerable<T>
    {
        private class FastQueueNode<U>
        {
            public U Value;
            public FastQueueNode<U> Next;
        }

        private int _count = 0;
        // The head node. The next pointer of the head node always points 
        // to the least recently added node - the node to dequeue first.
        private FastQueueNode<T> _head;
        // The tail node. This is always the most recently added node.
        private FastQueueNode<T> _tail;
        // Note: all next pointers point to less recently added nodes (i.e. 
        // the next node to dequeue).

        public FastQueue()
        {
            _head = new FastQueueNode<T>();
            _tail = _head;
            _tail.Next = null;
        }

        public int Count
        {
            get { return _count; }
        }

        public T Dequeue()
        {
            throw new NotImplementedException();
        }

        public void Enqueue(T value)
        {
            throw new NotImplementedException();

            FastQueueNode<T> tail;
            FastQueueNode<T> tailNext;
            FastQueueNode<T> node;

            // Create a new queue node.
            node = new FastQueueNode<T>();
            node.Value = value;
            node.Next = null;

            // Add the node to the tail of the list, atomically. 
            // We have to set the next pointer of the current tail node 
            // and then replace the tail pointer with our new node.
            while (true)
            {
                tailNext = _tail.Next;

                while (true)
                {
                    tail = _tail;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
