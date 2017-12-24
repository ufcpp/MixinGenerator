class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public void Log([This] object @this, string message) => System.Console.WriteLine($"{@this} {message}");
    public void Log([This] object a, [This] object b) { }
}
