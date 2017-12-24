using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace MixinGenerator.Test
{
    public class PracticalExamples : ConventionCodeFixVerifier
    {
        [Fact] public void NotifyPropertyChanged() => VerifyCSharpByConvention();
        [Fact] public void DisposableList() => VerifyCSharpByConvention();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}
