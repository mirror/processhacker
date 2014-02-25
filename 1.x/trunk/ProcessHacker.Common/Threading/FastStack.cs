using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace ProcessHacker.Common.Threading
{
    public class FastStack<T> : IEnumerable<T>
    {
        private class FastStackNode<U>
        {
            public U Value;
            public FastStackNode<U> Next;
        }

        private int _count = 0;
        private FastStackNode<T> _bottom = null;

        public int Count
        {
            get { return _count; }
        }

        public T Peek()
        {
            FastStackNode<T> bottom;

            bottom = _bottom;

            if (bottom == null)
                throw new InvalidOperationException("The stack is empty.");

            return bottom.Value;
        }

        public T Pop()
        {
            FastStackNode<T> bottom;

            // Atomically replace the bottom of the stack.
            while (true)
            {
                bottom = _bottom;

                // If the bottom of the stack is null, the 
                // stack is empty.
                if (bottom == null)
                    throw new InvalidOperationException("The stack is empty.");

                // Try to replace the pointer.
                if (Interlocked.CompareExchange<FastStackNode<T>>(
                    ref _bottom,
                    bottom.Next,
                    bottom
                    ) == bottom)
                {
                    // Success.
                    return bottom.Value;
                }
            }
        }

        public void Push(T value)
        {
            FastStackNode<T> bottom;
            FastStackNode<T> entry;

            entry = new FastStackNode<T>();
            entry.Value = value;

            // Atomically replace the bottom of the stack.
            while (true)
            {
                bottom = _bottom;
                entry.Next = bottom;

                // Try to replace the pointer.
                if (Interlocked.CompareExchange<FastStackNode<T>>(
                    ref _bottom,
                    entry,
                    bottom
                    ) == bottom)
                {
                    // Success.
                    break;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            FastStackNode<T> entry;

            entry = _bottom;

            // Start the enumeration.
            while (entry != null)
            {
                yield return entry.Value;
                entry = entry.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
