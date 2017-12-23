class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public int Value { get; set; }
    public event System.Action A;
    public int M(int x) => x * Value;
}
