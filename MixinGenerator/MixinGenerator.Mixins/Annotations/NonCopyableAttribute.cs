using System;

namespace MixinGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class NonCopyableAttribute : Attribute
    {
    }
}
