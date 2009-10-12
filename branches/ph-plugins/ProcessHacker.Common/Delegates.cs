namespace System
{
    public delegate void Action();
    //public delegate void Action<T>(T a1);
    public delegate void Action<T, U>(T a1, U a2);
    public delegate void Action<T, U, V>(T a1, U a2, V a3);
    public delegate void Action<T, U, V, W>(T a1, U a2, V a3, W a4);
    public delegate void Action<T, U, V, W, X>(T a1, U a2, V a3, W a4, X a5);
    public delegate void Action<T, U, V, W, X, Y>(T a1, U a2, V a3, W a4, X a5, Y a6);
    public delegate void Action<T, U, V, W, X, Y, Z>(T a1, U a2, V a3, W a4, X a5, Y a6, Z a7);

    public delegate T Func<T>();
    public delegate U Func<T, U>(T a1);
    public delegate V Func<T, U, V>(T a1, U a2);
    public delegate W Func<T, U, V, W>(T a1, U a2, V a3);
    public delegate X Func<T, U, V, W, X>(T a1, U a2, V a3, W a4);
    public delegate Y Func<T, U, V, W, X, Y>(T a1, U a2, V a3, W a4, X a5);
    public delegate Z Func<T, U, V, W, X, Y, Z>(T a1, U a2, V a3, W a4, X a5, Y a6);
}
