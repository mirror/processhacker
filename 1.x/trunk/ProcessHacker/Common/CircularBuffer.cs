using System;
using System.Collections.Generic;
using System.Collections;

namespace ProcessHacker.Common
{
    /// <summary>
    /// A Less than perfect CircularList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularList<T> : IList<T>
    {
        private List<T> data = new List<T>();
        public int Max;

        // TODO: rewrite entire class at some stage.
        public CircularList(int max)
        {
            this.Max = max;
        }

        public int Count { get { return data.Count; } }

        public T this[int index]
        {
            get { return this.data[index]; }
            set { this.data[index] = value; }
        }

        public void Add(T item)
        {
            //Less than perfect.
            if (this.data.Count >= this.Max)
            {
                this.data.RemoveRange(0, this.data.Count - this.Max);

                for (int i = 0; i <= this.data.Count - 2; i++)
                {
                    this.data[i] = this.data[i + 1];
                }
                this.data[this.data.Count - 1] = item;
            }
            else
            {
                this.data.Add(item);
            }          
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(T item)
        {
            return data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            return data.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
