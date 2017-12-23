using System.Text;

namespace MixinGenerator
{
    static class StringBuilderExtensions
    {
        public static StringBuilder Append(this StringBuilder sb, string s1, string s2)
        {
            sb.Append(s1);
            sb.Append(s2);
            return sb;
        }

        public static StringBuilder Append(this StringBuilder sb, string s1, string s2, string s3)
        {
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            return sb;
        }

        public static StringBuilder Append(this StringBuilder sb, string s1, string s2, string s3, string s4)
        {
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            return sb;
        }

        public static StringBuilder Append(this StringBuilder sb, string s1, string s2, string s3, string s4, string s5)
        {
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            sb.Append(s5);
            return sb;
        }

        public static StringBuilder Append(this StringBuilder sb, string s1, string s2, string s3, string s4, string s5, string s6)
        {
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            sb.Append(s5);
            sb.Append(s6);
            return sb;
        }

        public static StringBuilder Append(this StringBuilder sb, string s1, string s2, string s3, string s4, string s5, string s6, string s7)
        {
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            sb.Append(s5);
            sb.Append(s6);
            sb.Append(s7);
            return sb;
        }

        public static StringBuilder Append(this StringBuilder sb, string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8)
        {
            sb.Append(s1);
            sb.Append(s2);
            sb.Append(s3);
            sb.Append(s4);
            sb.Append(s5);
            sb.Append(s6);
            sb.Append(s7);
            sb.Append(s8);
            return sb;
        }

        // I really want `params Span<T>`, no-allocation variable-length parameter!
    }
}
