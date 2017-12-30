namespace MixinGenerator
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string s)
        {
            //todo: I'd like to use Span<char> to avoid allocation.

            var trimmed = s.TrimStart('_');
            if (trimmed.Length == 0)
            {
                return s;
            }
            else
            {
                if (char.IsLower(trimmed[0]))
                    trimmed = char.ToUpper(trimmed[0]) + trimmed.Substring(1);
                return trimmed;
            }
        }
    }
}
