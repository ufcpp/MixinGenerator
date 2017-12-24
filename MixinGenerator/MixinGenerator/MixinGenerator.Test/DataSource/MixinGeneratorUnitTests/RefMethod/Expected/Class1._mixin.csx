partial class Class1
{
    public ref System.DateTime M1(ref System.DateTime x) => ref _mixin.M1(ref x);
    public ref readonly System.DateTime M2(in System.DateTime x) => ref _mixin.M2(in x);
    public void M3(out System.DateTime x) => _mixin.M3(out x);
    public void M4(in System.DateTime x, ref System.DateTime y, out System.DateTime z) => _mixin.M4(in x, ref y, out z);
    public ref int X => ref _mixin.X;
    public ref readonly int Y => ref _mixin.Y;
}
