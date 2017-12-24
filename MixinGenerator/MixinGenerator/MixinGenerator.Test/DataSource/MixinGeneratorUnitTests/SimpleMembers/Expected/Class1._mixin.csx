
partial class Class1
{
    public int Value { get => _mixin.Value; set => _mixin.Value = value; }
    public event System.Action A { add => _mixin.A += value; remove => _mixin.A -= value; }
    public int M(int x) => _mixin.M(x);
    public int N(int x) => _mixin.N(x);
}
