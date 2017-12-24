using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinGenerator
{
    static class SyntaxExtensions
    {
        private static readonly SyntaxToken PartialToken = Token(SyntaxKind.PartialKeyword);

        public static TypeDeclarationSyntax AddPartialModifier(this TypeDeclarationSyntax typeDecl)
        {
            if (typeDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))) return typeDecl;

            switch (typeDecl)
            {
                case ClassDeclarationSyntax s: return s.AddModifiers(new[] { PartialToken });
                case StructDeclarationSyntax s: return s.AddModifiers(new[] { PartialToken });
                default: throw new NotSupportedException();
            }
        }

        public static TypeDeclarationSyntax GetPartialTypeDeclaration(this TypeDeclarationSyntax typeDecl, StringBuilder source)
        {
            source.Append("partial ", typeDecl.Keyword.ValueText, " ", typeDecl.GetGenericName());
            source.Append(@"
{
}
");
            return (TypeDeclarationSyntax)ParseCompilationUnit(source.ToString()).Members[0];
        }

        private static string GetGenericName(this TypeDeclarationSyntax typeDecl)
        {
            var name = typeDecl.Identifier.Text;

            if (typeDecl.TypeParameterList == null)
            {
                return name;
            }

            var sb = new StringBuilder();

            sb.Append(name, "<");

            var first = true;
            foreach (var p in typeDecl.TypeParameterList.Parameters)
            {
                if (first) first = false;
                else sb.Append(", ");
                sb.Append(p.Identifier.Text);
            }

            sb.Append(">");

            return sb.ToString();
        }

        public static TypeDeclarationSyntax AddMembers(this TypeDeclarationSyntax typeDecl, MemberDeclarationSyntax[] items)
        {
            switch (typeDecl)
            {
                case ClassDeclarationSyntax s: return s.AddMembers(items);
                case StructDeclarationSyntax s: return s.AddMembers(items);
                default: throw new NotSupportedException();
            }
        }

        public static TypeDeclarationSyntax AddBaseListTypes(this TypeDeclarationSyntax typeDecl, params BaseTypeSyntax[] items)
        {
            switch (typeDecl)
            {
                case ClassDeclarationSyntax s: return s.AddBaseListTypes(items);
                case StructDeclarationSyntax s: return s.AddBaseListTypes(items);
                default: throw new NotSupportedException();
            }
        }
    }
}
