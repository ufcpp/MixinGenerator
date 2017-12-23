partial class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public int Value { get; set; }
    public string StringValue => Value.ToString();
}
