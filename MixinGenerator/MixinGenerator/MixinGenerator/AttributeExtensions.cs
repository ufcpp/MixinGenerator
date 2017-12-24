using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MixinGenerator
{
    static class AttributeExtensions
    {
        public static string MemberAccessibility(this ISymbol s)
        {
            foreach (var a in s.GetAttributes())
            {
                var value = GetAccessibility(a);

                switch (GetAccessibility(a))
                {
                    case 1: return "private ";
                    case 2: return "private protected ";
                    case 3: return "protected ";
                    case 4: return "internal ";
                    case 5: return "protected internal ";
                    default: return "public ";
                }
            }

            return "public ";
        }

        private static int? GetAccessibility(AttributeData a)
        {
            if (a.AttributeClass.Name == "AccessibilityAttribute")
            {
                foreach (var arg in a.ConstructorArguments)
                {
                    if (arg.Type.Name == "Accessibility")
                        return arg.Value as int?;
                }
                foreach (var arg in a.NamedArguments)
                {
                    if (arg.Value.Type.Name == "Accessibility")
                        return arg.Value.Value as int?;
                }
            }

            return null;
        }
    }
}
