class Class1
{
    private A _a;
    private B _b;
}

[Mixin]
struct A
{
    public int M(int x) => x * Value;
}

[Mixin]
struct B
{
    public void N<T>(T x) { }
}
