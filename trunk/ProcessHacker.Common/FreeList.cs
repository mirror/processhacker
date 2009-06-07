using System.Collections.Generic;

namespace ProcessHacker.Common
{
    /// <summary>
    /// Manages a list of free objects that can be re-used.
    /// </summary>
    public class FreeList<T>
        where T : IResettable, new()
    {
        private LinkedList<T> _list = new LinkedList<T>();
        private int _maximumCount = 0;

        public int MaximumCount
        {
            get { return _maximumCount; }
            set { _maximumCount = value; }
        }

        public T Allocate()
        {
            lock (_list)
            {
                if (_list.Count == 0)
                    return this.AllocateNew();
                T obj = _list.First.Value;
                _list.RemoveFirst();
                return obj;
            }
        }

        private T AllocateNew()
        {
            T obj = new T();
            obj.ResetObject();
            return obj;
        }

        public void Clear()
        {
            lock (_list)
                _list.Clear();
        }

        public void Free(T obj)
        {
            lock (_list)
            {
                if (_list.Count < _maximumCount || _maximumCount == 0)
                    _list.AddFirst(obj);
            }
        }
    }
}
