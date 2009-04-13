using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessHacker
{
    public class WeakReference<T>
        where T : class
    {
        private WeakReference _weakReference;

        public WeakReference(T obj)
            : this(obj, false)
        { }

        public WeakReference(T obj, bool trackResurrection)
        {
            _weakReference = new WeakReference(obj, trackResurrection);
        }

        public bool Alive
        {
            get { return _weakReference.IsAlive; }
        }

        public bool TrackResurrection
        {
            get { return _weakReference.TrackResurrection; }
        }

        public T Target
        {
            get { return _weakReference.Target as T; }
        }
    }
}
