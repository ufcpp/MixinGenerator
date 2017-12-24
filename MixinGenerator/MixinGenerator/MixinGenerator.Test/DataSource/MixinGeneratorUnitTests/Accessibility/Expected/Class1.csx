partial class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    [Accessibility(Accessibility.Private)]
    public int Private { get; }

    [Accessibility(accessibility: Accessibility.ProtectedAndInternal)]
    public int ProtectedAndInternal { get; }

    [Accessibility(Accessibility = Accessibility.Protected)]
    public int Protected { get; }

    [Accessibility(Accessibility.Internal)]
    public int Internal { get; }

    [Accessibility(Accessibility.ProtectedOrInternal)]
    public int ProtectedOrInternal { get; }

    [Accessibility(Accessibility.Public)]
    public int Public { get; }

    [Accessibility]
    public int Default { get; }
}
