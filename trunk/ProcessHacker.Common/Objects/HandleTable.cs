using System.Collections.Generic;

namespace ProcessHacker.Common.Objects
{
    /// <summary>
    /// Provides methods for managing handles to objects.
    /// </summary>
    public class HandleTable : BaseObject
    {
        private IdGenerator _handleGenerator = new IdGenerator(4, 4);
        private Dictionary<int, BaseObject> _handles =
            new Dictionary<int, BaseObject>();

        protected override void DisposeObject(bool disposing)
        {
            lock (_handles)
            {
                foreach (var obj in _handles.Values)
                    obj.Dereference(disposing);
            }
        }

        public int Allocate(BaseObject obj)
        {
            int handle = _handleGenerator.Pop();

            obj.Reference();
            obj.Dispose(); // GC should not own the object.

            lock (_handles)
                _handles.Add(handle, obj);

            return handle;
        }

        public bool Free(int handle)
        {
            BaseObject obj;

            lock (_handles)
            {
                if (!_handles.ContainsKey(handle))
                    return false;

                obj = _handles[handle];
                _handles.Remove(handle);
            }

            _handleGenerator.Push(handle);
            obj.Dereference();

            return true;
        }

        public BaseObject GetHandleObject(int handle)
        {
            lock (_handles)
            {
                if (_handles.ContainsKey(handle))
                    return _handles[handle];
                else
                    return null;
            }
        }

        public T GetHandleObject<T>(int handle)
            where T : BaseObject
        {
            return this.GetHandleObject(handle) as T;
        }

        public BaseObject ReferenceByHandle(int handle)
        {
            lock (_handles)
            {
                if (_handles.ContainsKey(handle))
                {
                    BaseObject obj = _handles[handle];

                    obj.Reference();

                    return obj;
                }
                else
                {
                    return null;
                }
            }
        }

        public T ReferenceByHandle<T>(int handle)
            where T : BaseObject
        {
            return this.ReferenceByHandle(handle) as T;
        }
    }
}
