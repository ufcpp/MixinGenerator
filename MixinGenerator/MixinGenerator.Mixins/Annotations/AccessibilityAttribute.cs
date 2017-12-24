using System;

namespace MixinGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    public class AccessibilityAttribute : Attribute
    {
        public Accessibility Accessibility { get; set; }
        public AccessibilityAttribute() { }
        public AccessibilityAttribute(Accessibility accessibility) => Accessibility = accessibility;
    }
}
