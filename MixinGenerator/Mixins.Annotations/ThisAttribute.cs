using System;

namespace Mixins.Annotations
{
    /// <summary>
    /// Mark a parameter as automatically expanding to "this".
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ThisAttribute : Attribute
    {
    }
}
