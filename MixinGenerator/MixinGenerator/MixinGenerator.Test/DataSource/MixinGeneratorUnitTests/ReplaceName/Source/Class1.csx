class Class1
{
    private MyMixin<X> _abc;
    private MyMixin<X> _aaaBbbCcc;
}

class X
{
}

[Mixin("Value")]
struct MyMixin<T>
{
    private T _value;

    public T Value => _value ?? (_value = new T());

    public void DisposeValue()
    {
        if (_value != null)
        {
            (_value as IDisposable)?.Dispose();
            _value = null;
        }
    }

    public bool IsValueInitialized => _value != null;
}
