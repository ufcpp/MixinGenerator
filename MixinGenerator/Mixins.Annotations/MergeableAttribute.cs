using System;

namespace Mixins.Annotations
{
    /// <summary>
    /// Mark a method mergeable.
    /// </summary>
    /// <remarks>
    /// If a name of a mergeable method is duplicated, the expansion result is merged into a single method.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MergeableAttribute : Attribute
    {
    }
}
