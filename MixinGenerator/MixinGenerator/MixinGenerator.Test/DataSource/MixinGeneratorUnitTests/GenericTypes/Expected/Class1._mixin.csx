partial class Class1<T>
{
    public T TValue { get => _mixin.TValue; set => _mixin.TValue = value; }
    public int IValue { get => _mixin.IValue; set => _mixin.IValue = value; }
    public void M<U>(T t, U u) => _mixin.M<U>(t, u);
}
