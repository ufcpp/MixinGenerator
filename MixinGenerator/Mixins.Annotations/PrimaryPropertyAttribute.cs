using System;

namespace Mixins.Annotations
{
    /// <summary>
    /// Mark a property in a mixin struct as primary.
    /// </summary>
    /// <remarks>
    /// A "primary property" means that the expansion result of the property is named after the field of the mixin.
    ///
    /// Assume:
    /// <code><![CDATA[
    /// [Mixin]
    /// struct
    /// X<T>
    /// {
    ///     [PrimaryProperty]
    ///     T Primary => default(T);
    ///
    ///     T NonPrimary => default(T);
    /// }
    /// ]]></code>
    ///
    /// original source:
    /// <code><![CDATA[
    /// class Sample
    /// {
    ///     X<string> _item;
    /// }
    /// ]]></code>
    ///
    /// expansion result:
    /// <code><![CDATA[
    /// class Sample
    /// {
    ///     X<string> _item;
    /// 
    ///     public string Item => _item.Primary; // named after the field "_item"
    ///
    ///     public string NonPrimary => _item.NonPrimary; // named after its property name "NonPrimary"
    /// }
    /// ]]></code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrimaryPropertyAttribute : Attribute
    {
    }
}
