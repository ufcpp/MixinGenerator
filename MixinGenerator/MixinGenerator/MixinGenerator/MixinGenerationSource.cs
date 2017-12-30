using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MixinGenerator
{
    class MixinGenerationSource
    {
        public SemanticModel SemanticModel { get; }
        public VariableDeclaratorSyntax Field { get; }
        public TypeInfo MixinType { get; }
        public TypeDeclarationSyntax DeclaringType { get; }
        public CancellationToken CancellationToken { get; }

        public AttributeData MixinAttribute { get; }
        public string ReplaceToField { get; }
        public string PascalCaceFieldName { get; }

        public MixinGenerationSource(SemanticModel semanticModel, VariableDeclaratorSyntax field, TypeInfo mixinType, TypeDeclarationSyntax declaringType, CancellationToken cancellationToken)
        {
            SemanticModel = semanticModel;
            DeclaringType = declaringType;
            Field = field;
            MixinType = mixinType;
            CancellationToken = cancellationToken;

            var a = mixinType.Type.GetMixinAttribute();
            if(a != null)
            {
                MixinAttribute = a;

                //todo: needs to check parameter name?
                var args = a.ConstructorArguments;
                if (args.Length > 0)
                {
                    ReplaceToField = args[0].Value as string;
                    PascalCaceFieldName = field.Identifier.ValueText.ToPascalCase();
                }
            }
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

        public string FieldName => Field.Identifier.ValueText;

        private StringBuilder _stringBuilder;

        public StringBuilder GetBuilder()
        {
            if (_stringBuilder == null) _stringBuilder = new StringBuilder(1024);
            else _stringBuilder.Clear();
            return _stringBuilder;
        }

        public string GetTypeName(ITypeSymbol t) => t.ToMinimalDisplayString(SemanticModel, 0);

        public string ReplaceSymbolName(string symbolName)
        {
            if (ReplaceToField == null) return symbolName;
            return symbolName.Replace(ReplaceToField, PascalCaceFieldName);
        }
    }
}
