class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public void M3(out System.DateTime x) { x = default; }
    public void M4(in System.DateTime x, ref System.DateTime y, out System.DateTime z) { z = default; }
}
