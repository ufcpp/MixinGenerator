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
            var newRoot = await GenerateMixinDeclaration(document, gen);

            var name = Path.GetFileNameWithoutExtension(document.Name);
            var generatedName = name + "." + gen.Field.Identifier.ValueText + ".cs";

            var project = document.Project;

            var existed = project.Documents.FirstOrDefault(d => d.Name == generatedName);
            if (existed != null) return existed.WithSyntaxRoot(newRoot);
            else return project.AddDocument(generatedName, newRoot, document.Folders);
        }

        private static async Task<CompilationUnitSyntax> GenerateMixinDeclaration(Document document, MixinGenerationSource gen)
        {
            var newTypeDecl = GetPartialTypeDeclaration(gen);
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

        private static TypeDeclarationSyntax GetPartialTypeDeclaration(MixinGenerationSource gen)
        {
            var typeDecl = gen.DeclaringType;
            var source = gen.GetBuilder();

            source.Append("partial ", typeDecl.Keyword.ValueText, " ");
            AppendGenericName(typeDecl, source);
            AppendInterfaces(gen, source);
            source.Append(@"
{
}
");
            return (TypeDeclarationSyntax)ParseCompilationUnit(source.ToString()).Members[0];
        }

        private static void AppendInterfaces(MixinGenerationSource gen, StringBuilder source)
        {
            var interfaces = gen.MixinType.Type.Interfaces;
            if (interfaces.Length > 0)
            {
                source.Append(" : ");

                bool first = true;
                foreach (var i in interfaces)
                {
                    if (first) first = false;
                    else source.Append(", ");
                    source.Append(gen.GetTypeName(i));
                }
            }
        }

        private static void AppendGenericName(TypeDeclarationSyntax typeDecl, StringBuilder source)
        {
            source.Append(typeDecl.Identifier.Text);

            if (typeDecl.TypeParameterList == null) return;

            source.Append("<");

            var first = true;
            foreach (var p in typeDecl.TypeParameterList.Parameters)
            {
                if (first) first = false;
                else source.Append(", ");
                source.Append(p.Identifier.Text);
            }

            source.Append(">");
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
            source.Append("public ", Refness(s.RefKind, RefPlace.ReturnType), gen.GetTypeName(s.Type), " ", s.Name);

            if (s.SetMethod != null)
            {
                source.Append(" { get => ", gen.FieldName, ".", s.Name)
                    .Append("; set => ", gen.FieldName, ".", s.Name, " = value; }");
            }
            else
            {
                source.Append(" => ", Refness(s.RefKind, RefPlace.ReturnStatement), gen.FieldName, ".", s.Name, ";");
            }

            return ParseCompilationUnit(source.ToString()).Members[0]
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(CarriageReturnLineFeed);
        }

        private static MemberDeclarationSyntax GenerateNodes(IEventSymbol s, MixinGenerationSource gen)
        {
            var source = gen.GetBuilder();
            source.Append("public event ", gen.GetTypeName(s.Type), " ", s.Name)
                .Append(" { add => ", gen.FieldName, ".", s.Name, " += value;")
                .Append(" remove => ", gen.FieldName, ".", s.Name, " -= value; }");

            return ParseCompilationUnit(source.ToString()).Members[0]
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(CarriageReturnLineFeed);
        }

        private static MemberDeclarationSyntax GenerateNodes(IMethodSymbol s, MixinGenerationSource gen)
        {
            var source = gen.GetBuilder();
            source.Append("public ", Refness(s.RefKind, RefPlace.ReturnType), gen.GetTypeName(s.ReturnType), " ");
            GenerateGenericMethodName(s, source);
            source.Append("(");
            GenerateParameters(gen, s.Parameters, source);
            source.Append(")");
            GenerateGenericConstraints(gen, s, source);
            source.Append(" => ", Refness(s.RefKind, RefPlace.ReturnStatement), gen.FieldName, ".");
            GenerateGenericMethodName(s, source);
            source.Append("(");
            GenerateArguments(s.Parameters, source);
            source.Append(");");

            return ParseCompilationUnit(source.ToString()).Members[0]
                .WithLeadingTrivia(gen.Field.GetLeadingTrivia())
                .WithTrailingTrivia(CarriageReturnLineFeed);
        }

        private static void GenerateGenericMethodName(IMethodSymbol m, StringBuilder source)
        {
            source.Append(m.Name);

            if (m.IsGenericMethod)
            {
                source.Append("<");
                bool first = true;
                foreach (var a in m.TypeArguments)
                {
                    if (first) first = false;
                    else source.Append(", ");
                    source.Append(a.Name);
                }
                source.Append(">");
            }
        }

        private static void GenerateGenericConstraints(MixinGenerationSource gen, IMethodSymbol m, StringBuilder source)
        {
            if (!m.IsGenericMethod) return;

            foreach (var p in m.TypeParameters)
            {
                if (!p.HasReferenceTypeConstraint && !p.HasValueTypeConstraint && !p.HasConstructorConstraint && p.ConstraintTypes.Length == 0) continue;

                source.Append(" where ", p.Name, " :");

                bool first = true;

                if (p.HasReferenceTypeConstraint)
                {
                    source.Append(" class");
                    first = false;
                }

                if (p.HasValueTypeConstraint)
                {
                    source.Append(" struct");
                    first = false;
                }

                foreach (var c in p.ConstraintTypes)
                {
                    if (first) first = false;
                    else source.Append(", ");
                    source.Append(gen.GetTypeName(c));
                }

                if (p.HasConstructorConstraint)
                {
                    if (first) first = false;
                    else source.Append(", ");
                    source.Append(" new()");
                }
            }
        }

        private static void GenerateParameters(MixinGenerationSource gen, ImmutableArray<IParameterSymbol> parameters, StringBuilder source)
        {
            bool first = true;
            foreach (var p in parameters)
            {
                if (first) first = false;
                else source.Append(", ");
                source.Append(Refness(p.RefKind, RefPlace.ParameterType), gen.GetTypeName(p.Type), " ", p.Name);
            }
        }

        private static void GenerateArguments(ImmutableArray<IParameterSymbol> parameters, StringBuilder source)
        {
            bool first = true;
            foreach (var p in parameters)
            {
                if (first) first = false;
                else source.Append(", ");
                source.Append(Refness(p.RefKind, RefPlace.ParameterType), p.Name);
            }
        }

        enum RefPlace
        {
            ParameterType,
            ReturnType,
            ReturnStatement,
        }

        private static string Refness(RefKind kind, RefPlace place)
        {
            switch (kind)
            {
                case RefKind.Ref: return "ref ";
                case RefKind.Out: return "out ";
                case RefKind.In:
                    switch (place)
                    {
                        case RefPlace.ParameterType: return "in ";
                        case RefPlace.ReturnType: return "ref readonly ";
                        case RefPlace.ReturnStatement: return "ref ";
                        default: return "";
                    }
                default: return "";
            }
        }
    }
}
