using System;

[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
class MixinAttribute : Attribute
{
    public MixinAttribute() { }
    public MixinAttribute(string raplaceToField) { }
}
