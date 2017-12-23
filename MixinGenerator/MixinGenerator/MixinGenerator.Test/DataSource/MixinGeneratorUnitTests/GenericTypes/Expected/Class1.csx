partial class Class1<T>
{
    private MyMixin<T> _mixin;
    private T _t;
}

[Mixin]
struct MyMixin<T>
{
    public T TValue { get; set; }
    public int IValue { get; set; }
    public void M<U>(T t, U u) { }
}
