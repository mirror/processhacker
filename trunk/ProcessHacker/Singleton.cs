namespace ProcessHacker
{
    /// <summary>
    /// Implements the singleton pattern.
    /// </summary>
    public class Singleton<T>
        where T : class, new()
    {
        private object _instanceLock = new object();
        private T _instance = null;

        /// <summary>
        /// The single instance of the type.
        /// </summary>
        public T Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new T();

                    return _instance;
                }
            }
        }
    }
}
