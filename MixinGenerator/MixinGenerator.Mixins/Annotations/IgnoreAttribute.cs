using System;

namespace MixinGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
