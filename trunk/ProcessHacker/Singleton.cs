namespace ProcessHacker
{
    /// <summary>
    /// Implements the singleton pattern.
    /// </summary>
    public class Singleton<T>
        where T : class, new()
    {
        private static object _instanceLock = new object();
        private static T _instance = null;

        /// <summary>
        /// The single instance of the type.
        /// </summary>
        public static T Instance
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
