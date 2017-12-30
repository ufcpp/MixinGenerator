using MixinGenerator.Annotations;
using MixinGenerator.Test.TestTypes;

class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
    public int A { get; }

    [Ignore]
    public int B { get; }
}
