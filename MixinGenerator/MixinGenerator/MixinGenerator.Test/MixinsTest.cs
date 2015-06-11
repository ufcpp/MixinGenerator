using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace MixinGenerator.Test
{
    [TestClass]
    public class MixinsTest : ConventionCodeFixVerifier
    {
        [TestMethod]
        public void CompositeDisposableMixin() => VerifyCSharpByConvention();
        [TestMethod]
        public void LazyMixin() => VerifyCSharpByConvention();
        [TestMethod]
        public void NotifyPropertyChangedMixin() => VerifyCSharpByConvention();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}