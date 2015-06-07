using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace MixinGenerator.Test
{
    [TestClass]
    public class MixinsTest : ContractCodeFixVerifier
    {
        [TestMethod]
        public void CompositeDisposableMixin() => VerifyDiagnostic();
        [TestMethod]
        public void LazyMixin() => VerifyDiagnostic();
        [TestMethod]
        public void NotifyPropertyChangedMixin() => VerifyDiagnostic();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}