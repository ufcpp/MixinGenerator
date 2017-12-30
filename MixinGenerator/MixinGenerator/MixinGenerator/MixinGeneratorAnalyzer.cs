using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MixinGenerator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MixinGeneratorAnalyzer : DiagnosticAnalyzer
    {
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor("Mixin", "code generation for mixin", "generate {0} mixin code", "CodeGeneration", DiagnosticSeverity.Info, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var f = (IFieldSymbol)context.Symbol;

            if (f.Type.GetMixinAttribute() != null)
            {
                var diagnostic = Diagnostic.Create(Rule, f.Locations[0], f.Type.Name);
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }
    }
}
