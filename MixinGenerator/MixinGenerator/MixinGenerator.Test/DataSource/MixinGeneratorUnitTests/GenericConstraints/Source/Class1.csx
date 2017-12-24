class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public void M<T, U, V>(T t, U u, V v)
        where T : struct, System.IDisposable
        where U : class, new()
        => t.Dispose();
}
