﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.IO;
using System.Collections.Generic;
using System;
using Microsoft.CodeAnalysis.CSharp;

namespace MixinGenerator
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MixinGeneratorCodeFixProvider)), Shared]
    public class MixinGeneratorCodeFixProvider : CodeFixProvider
    {
        private const string title = "Generate mixin code";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MixinGeneratorAnalyzer.Rule.Id);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var diagnostic = context.Diagnostics[0];

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => GenerateMixin(document, diagnostic, c),
                    equivalenceKey: title),
                diagnostic);

            return Task.CompletedTask;
        }

        private async Task<Solution> GenerateMixin(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var gen = await MixinGenerationSource.Create(document, diagnostic, cancellationToken);
            return await GenerateMixin(document, gen);
        }

        private async Task<Solution> GenerateMixin(Document document, MixinGenerationSource gen)
        {
            document = await AddPartialModifier(document, gen);
            document = await AddNewDocument(document, gen);
            return document.Project.Solution;
        }

        private static async Task<Document> AddPartialModifier(Document document, MixinGenerationSource gen)
        {
            var newTypeDecl = gen.DeclaringType.AddPartialModifier();

            var root = await document.GetSyntaxRootAsync(gen.CancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;
            var newRoolt = root.ReplaceNode(gen.DeclaringType, newTypeDecl)
                .WithAdditionalAnnotations(Formatter.Annotation);

            document = document.WithSyntaxRoot(newRoolt);
            return document;
        }

        private static async Task<Document> AddNewDocument(Document document, MixinGenerationSource gen)
        {
            var newRoot = await GeneratePartialDeclaration(document, gen);

            var name = Path.GetFileNameWithoutExtension(document.Name);
            var generatedName = name + "." + gen.Field.Identifier.ValueText + ".cs";

            var project = document.Project;

            var existed = project.Documents.FirstOrDefault(d => d.Name == generatedName);
            if (existed != null) return existed.WithSyntaxRoot(newRoot);
            else return project.AddDocument(generatedName, newRoot, document.Folders);
        }

        private static async Task<CompilationUnitSyntax> GeneratePartialDeclaration(Document document, MixinGenerationSource gen)
        {
            var newTypeDecl = gen.DeclaringType.GetPartialTypeDelaration();
            var generatedNodes = GenerateNodes(newTypeDecl, gen).ToArray();

            newTypeDecl = newTypeDecl
                .AddMembers(generatedNodes)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var ns = gen.DeclaringType.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.WithoutTrivia().GetText().ToString();

            MemberDeclarationSyntax topDecl;
            if (ns != null)
            {
                topDecl = NamespaceDeclaration(IdentifierName(ns))
                    .AddMembers(newTypeDecl)
                    .WithAdditionalAnnotations(Formatter.Annotation);
            }
            else
            {
                topDecl = newTypeDecl;
            }

            var root = await document.GetSyntaxRootAsync(gen.CancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;

            return CompilationUnit().AddUsings(root.Usings.ToArray())
                .AddMembers(topDecl)
                .WithTrailingTrivia(CarriageReturnLineFeed)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static IEnumerable<MemberDeclarationSyntax> GenerateNodes(TypeDeclarationSyntax typeDecl, MixinGenerationSource gen)
        {
            foreach (var m in gen.MixinType.Type.GetMembers())
            {
                switch (m)
                {
                    case IPropertySymbol s:
                        yield return GenerateNodes(s, gen);
                        break;
                    case IMethodSymbol s when s.MethodKind == MethodKind.Ordinary:
                        yield return GenerateNodes(s, gen);
                        break;
                    case IEventSymbol s:
                        yield return GenerateNodes(s, gen);
                        break;
                }
            }
            yield break;
        }

        private static MemberDeclarationSyntax GenerateNodes(IPropertySymbol s, MixinGenerationSource gen)
        {
            var typeName = s.Type.ToMinimalDisplayString(gen.SemanticModel, 0);

            var access = gen.Field.Identifier.ValueText + "." + s.Name;
            var source = s.SetMethod != null
                ? $"public {typeName} {s.Name} {{ get => {access}; set => {access} = value; }}"
                : $"public {typeName} {s.Name} => {access};";

            var p = (PropertyDeclarationSyntax)ParseCompilationUnit(source).Members[0];
            return p
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(gen.Field.GetTrailingTrivia())
                ;
        }

        private static MemberDeclarationSyntax GenerateNodes(IMethodSymbol s, MixinGenerationSource gen)
        {
            throw new NotImplementedException();
        }

        private static MemberDeclarationSyntax GenerateNodes(IEventSymbol s, MixinGenerationSource gen)
        {
            throw new NotImplementedException();
        }
    }
}
