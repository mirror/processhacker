using System;

namespace ProcessHacker.Common
{
    public class WeakReference<T>
        where T : class
    {
        public static implicit operator T(WeakReference<T> reference)
        {
            return reference.Target;
        }

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
