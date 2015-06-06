using System;

namespace Mixins.Annotations
{
    /// <summary>
    /// Mark a struct as mixin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]

    public class MixinAttribute : Attribute
    {
    }
}
