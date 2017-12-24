partial class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public ref System.DateTime M1(ref System.DateTime x) => ref x;
    public ref readonly System.DateTime M2(in System.DateTime x) => ref x;
    public void M3(out System.DateTime x) { x = default; }
    public void M4(in System.DateTime x, ref System.DateTime y, out System.DateTime z) { z = default; }

    private int _x;
    public ref int X => ref _x;
    public ref readonly int Y => ref _x;
}
