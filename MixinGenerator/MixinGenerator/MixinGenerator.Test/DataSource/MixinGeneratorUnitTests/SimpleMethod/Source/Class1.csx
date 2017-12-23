class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public int A { get; set; }
    public void M(int x) => A * x;
}
