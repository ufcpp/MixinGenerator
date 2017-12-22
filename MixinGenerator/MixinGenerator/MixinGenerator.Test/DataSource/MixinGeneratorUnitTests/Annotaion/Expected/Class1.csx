using System;

class Class1
{
    private MyMixin _mixin;
}

[Mixin]
struct MyMixin
{
}

[AttributeUsage(AttributeTargets.Struct)]
class MixinAttribute : Attribute { }
