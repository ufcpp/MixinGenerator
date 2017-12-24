using Microsoft.CodeAnalysis;

namespace MixinGenerator
{
    static class AttributeExtensions
    {
        /// <summary>
        /// has `This` attribute.
        /// </summary>
        public static bool IsThisReceiver(this IParameterSymbol s)
        {
            foreach (var a in s.GetAttributes())
            {
                if (a.AttributeClass.Name == "ThisAttribute") return true;
            }

            return false;
        }

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
