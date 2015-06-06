using System;

namespace Mixins.Annotations
{
    /// <summary>
    /// Mark as making the expansion result protected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
    public class ProtectedAttribute : Attribute
    {
    }
}
