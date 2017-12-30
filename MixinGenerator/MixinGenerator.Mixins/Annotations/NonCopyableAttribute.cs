using System;

namespace MixinGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class NonCopyableAttribute : Attribute
    {
    }
}
