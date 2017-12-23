using System.Collections.Immutable;
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
using System.Text;

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
            var source = gen.GetBuilder();
            source.Append("public ", gen.GetTypeName(s.Type), " ", s.Name);

            if (s.SetMethod != null)
            {
                source.Append(" { get => ", gen.FieldName, ".", s.Name)
                    .Append("; set => ", gen.FieldName, ".", s.Name, " = value; }");
            }
            else
            {
                source.Append(" => ", gen.FieldName, ".", s.Name, ";");
            }

            return ParseCompilationUnit(source.ToString()).Members[0]
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(gen.Field.GetTrailingTrivia());
        }

        private static MemberDeclarationSyntax GenerateNodes(IMethodSymbol s, MixinGenerationSource gen)
        {
            var source = gen.GetBuilder();
            source.Append("public ", gen.GetTypeName(s.ReturnType), " ", s.Name, "(");
            GenerateParameters(gen, s.Parameters, source);
            source.Append(") => ", gen.FieldName, ".", s.Name, "(");
            GenerateArguments(gen, s.Parameters, source);
            source.Append(");");

            return ParseCompilationUnit(source.ToString()).Members[0]
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(gen.Field.GetTrailingTrivia());
        }

        private static void GenerateParameters(MixinGenerationSource gen, ImmutableArray<IParameterSymbol> parameters, StringBuilder source)
        {
            bool first = true;
            foreach (var p in parameters)
            {
                source.Append(gen.GetTypeName(p.Type), " ", p.Name);
                if (first) first = false;
                else source.Append(", ");
            }
        }

        private static void GenerateArguments(MixinGenerationSource gen, ImmutableArray<IParameterSymbol> parameters, StringBuilder source)
        {
            bool first = true;
            foreach (var p in parameters)
            {
                source.Append(p.Name);
                if (first) first = false;
                else source.Append(", ");
            }
        }

        private static MemberDeclarationSyntax GenerateNodes(IEventSymbol s, MixinGenerationSource gen)
        {
            var source = gen.GetBuilder();
            source.Append("public event ", gen.GetTypeName(s.Type), " ", s.Name)
                .Append(" { add => ", gen.FieldName, ".", s.Name, " += value;")
                .Append(" remove => ", gen.FieldName, ".", s.Name, " -= value; }");

            return ParseCompilationUnit(source.ToString()).Members[0]
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(gen.Field.GetTrailingTrivia());
        }
    }
}
