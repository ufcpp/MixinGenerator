partial class Class1
{
    private int Private => _mixin.Private;
    private protected int ProtectedAndInternal => _mixin.ProtectedAndInternal;
    protected int Protected => _mixin.Protected;
    internal int Internal => _mixin.Internal;
    protected internal int ProtectedOrInternal => _mixin.ProtectedOrInternal;
    public int Public => _mixin.Public;
    public int Default => _mixin.Default;
}
