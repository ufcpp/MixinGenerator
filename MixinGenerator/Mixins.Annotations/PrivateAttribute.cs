using System;

namespace Mixins.Annotations
{
    /// <summary>
    /// Mark as making the expansion result private.
    /// </summary>
    /// <remarks>
    /// "Private" members are never expanded. Instead, invoke original members in the mixin field directly.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
    public class PrivateAttribute : Attribute
    {
    }
}
