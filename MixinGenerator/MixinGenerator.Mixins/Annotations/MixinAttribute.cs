using System;

namespace MixinGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class MixinAttribute : Attribute
    {
        public MixinAttribute() { }
        public MixinAttribute(string raplaceToField) { }
    }
}
