﻿using System;
using System.Linq;
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

        public static TypeDeclarationSyntax GetPartialTypeDelaration(this TypeDeclarationSyntax typeDecl)
            => CSharpSyntaxTree.ParseText($@"
partial {typeDecl.Keyword.ValueText} {typeDecl.Identifier.Text}
{{
}}
").GetRoot().ChildNodes().OfType<TypeDeclarationSyntax>().First();
    }
}
