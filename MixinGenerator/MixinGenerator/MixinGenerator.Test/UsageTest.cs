using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace MixinGenerator.Test
{
    [TestClass]
    public class UsageTest : ConventionCodeFixVerifier
    {
        [TestMethod]
        public void Accessibility() => VerifyCSharpByConvention();
        [TestMethod]
        public void DoNotMarkAsBothPrivateAndProtected() => VerifyCSharpByConvention();
        [TestMethod]
        public void DoNotUseAssignment() => VerifyCSharpByConvention();
        [TestMethod]
        public void DoNotUseProperty() => VerifyCSharpByConvention();
        [TestMethod]
        public void DoNotUseReadOnlyField() => VerifyCSharpByConvention();
        [TestMethod]
        public void MergeableMethodMustBeVoid() => VerifyCSharpByConvention();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}