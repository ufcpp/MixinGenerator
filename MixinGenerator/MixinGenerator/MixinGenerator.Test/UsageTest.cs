using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace MixinGenerator.Test
{
    [TestClass]
    public class UsageTest : ContractCodeFixVerifier
    {
        [TestMethod]
        public void Accessibility() => VerifyDiagnostic();
        [TestMethod]
        public void DoNotMarkAsBothPrivateAndProtected() => VerifyDiagnostic();
        [TestMethod]
        public void DoNotUseAssignment() => VerifyDiagnostic();
        [TestMethod]
        public void DoNotUseProperty() => VerifyDiagnostic();
        [TestMethod]
        public void DoNotUseReadOnlyField() => VerifyDiagnostic();
        [TestMethod]
        public void MergeableMethodMustBeVoid() => VerifyDiagnostic();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}