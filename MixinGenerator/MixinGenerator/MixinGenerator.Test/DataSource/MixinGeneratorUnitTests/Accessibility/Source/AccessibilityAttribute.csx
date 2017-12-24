using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
public class AccessibilityAttribute : Attribute
{
    public Accessibility Accessibility { get; set; }
    public AccessibilityAttribute() { }
    public AccessibilityAttribute(Accessibility accessibility) => Accessibility = accessibility;
}

public enum Accessibility
{
    Private = 1,
    ProtectedAndInternal = 2,
    Protected = 3,
    Internal = 4,
    ProtectedOrInternal = 5,
    Public = 6
}
