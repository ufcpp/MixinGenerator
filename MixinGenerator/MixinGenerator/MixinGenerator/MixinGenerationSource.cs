using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MixinGenerator
{
    class MixinGenerationSource
    {
        public SemanticModel SemanticModel { get; }
        public VariableDeclaratorSyntax Field { get; }
        public TypeInfo MixinType { get; }
        public TypeDeclarationSyntax DeclaringType { get; }
        public CancellationToken CancellationToken { get; }

        public MixinGenerationSource(SemanticModel semanticModel, VariableDeclaratorSyntax field, TypeInfo mixinType, TypeDeclarationSyntax declaringType, CancellationToken cancellationToken)
        {
            SemanticModel = semanticModel;
            DeclaringType = declaringType;
            Field = field;
            MixinType = mixinType;
            CancellationToken = cancellationToken;
        }

        public static async Task<MixinGenerationSource> Create(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var field = (VariableDeclaratorSyntax)root.FindNode(diagnosticSpan);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var fieldDeclaration = ((VariableDeclarationSyntax)field.Parent).Type;
            var mixinType = semanticModel.GetTypeInfo(fieldDeclaration);

            var declaringType = (TypeDeclarationSyntax)field.Ancestors().First(x => x is TypeDeclarationSyntax);

            return new MixinGenerationSource(semanticModel, field, mixinType, declaringType, cancellationToken);
        }
    }
}
