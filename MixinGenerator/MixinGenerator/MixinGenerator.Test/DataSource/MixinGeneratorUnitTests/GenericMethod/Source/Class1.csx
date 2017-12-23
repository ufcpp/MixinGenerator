class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public string M<T>(T x) => x.ToString();
}
