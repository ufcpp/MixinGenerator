partial class Class1
{
    public void M<T, U, V>(T t, U u, V v) where T : struct, System.IDisposable where U : class, new() => _mixin.M<T, U, V>(t, u, v);
}
