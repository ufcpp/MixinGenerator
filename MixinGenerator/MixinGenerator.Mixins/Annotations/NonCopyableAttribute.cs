using System;

namespace MixinGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Struct)]
    internal class NonCopyableAttribute : Attribute
    {
    }
}
