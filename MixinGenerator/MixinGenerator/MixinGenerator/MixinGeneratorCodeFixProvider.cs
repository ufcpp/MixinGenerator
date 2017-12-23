using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var field = (VariableDeclaratorSyntax)root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => GenerateMixin(context.Document, field, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> GenerateMixin(Document document, VariableDeclaratorSyntax field, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var declaringType = field.Ancestors().OfType<TypeDeclarationSyntax>().First();

            return await GenerateMixinCode(document, declaringType, field, cancellationToken);
        }

        private async Task<Solution> GenerateMixinCode(Document document, TypeDeclarationSyntax typeDecl, VariableDeclaratorSyntax field, CancellationToken cancellationToken)
        {
            document = await AddPartialModifier(document, typeDecl, cancellationToken);
            document = await AddNewDocument(document, typeDecl, field, cancellationToken);
            return document.Project.Solution;
        }

        private static async Task<Document> AddPartialModifier(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            var newTypeDecl = typeDecl.AddPartialModifier();

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;
            var newRoolt = root.ReplaceNode(typeDecl, newTypeDecl)
                .WithAdditionalAnnotations(Formatter.Annotation);

            document = document.WithSyntaxRoot(newRoolt);
            return document;
        }

        private static async Task<Document> AddNewDocument(Document document, TypeDeclarationSyntax typeDecl, VariableDeclaratorSyntax field, CancellationToken cancellationToken)
        {
            //var fieldDeclaration = ((VariableDeclarationSyntax)field.Parent).Type;
            //var fieldType = semanticModel.GetTypeInfo(fieldDeclaration);

            var newRoot = await GeneratePartialDeclaration(document, typeDecl, field, cancellationToken);

            var name = typeDecl.Identifier.Text;
            var generatedName = name + "." + field.Identifier.ValueText + ".cs";

            var project = document.Project;

            var existed = project.Documents.FirstOrDefault(d => d.Name == generatedName);
            if (existed != null) return existed.WithSyntaxRoot(newRoot);
            else return project.AddDocument(generatedName, newRoot, document.Folders);
        }

        private static async Task<CompilationUnitSyntax> GeneratePartialDeclaration(Document document, TypeDeclarationSyntax typeDecl, VariableDeclaratorSyntax field, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var ti = semanticModel.GetTypeInfo(typeDecl);

            //var generatedNodes = GetGeneratedNodes(def, field).ToArray();

            var newClassDecl = typeDecl.GetPartialTypeDelaration()
                //.AddMembers(generatedNodes)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var ns = typeDecl.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.WithoutTrivia().GetText().ToString();

            MemberDeclarationSyntax topDecl;
            if (ns != null)
            {
                topDecl = NamespaceDeclaration(IdentifierName(ns))
                    .AddMembers(newClassDecl)
                    .WithAdditionalAnnotations(Formatter.Annotation);
            }
            else
            {
                topDecl = newClassDecl;
            }

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;

            return CompilationUnit().AddUsings(root.Usings.ToArray())
                .AddMembers(topDecl)
                .WithTrailingTrivia(CarriageReturnLineFeed)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
