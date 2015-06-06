using System.IO;
using System.Runtime.CompilerServices;

namespace TestHelper
{
    public abstract class ContractCodeFixVerifier : CodeFixVerifier
    {
        public ContractCodeFixVerifier()
        {
            var t = GetType();
            var path = Path.Combine(t.Assembly.Location, @"../../../DataSource");

            DataSourcePath = Path.Combine(path, t.Name);
            HeaderSource = ReadIfExists(Path.Combine(path, "Common/Header.cs"));
            FooterSource = ReadIfExists(Path.Combine(path, "Common/Footer.cs"));
        }

        private string DataSourcePath { get; }
        private string HeaderSource { get; }
        private string FooterSource { get; }

        private static string ReadIfExists(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            return File.ReadAllText(path);
        }

        protected void VerifyDiagnostic(DiagnosticResult[] expectedResults, [CallerMemberName]string testName = null)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source.cs");
            var source = File.ReadAllText(sourcePath);
            source = HeaderSource + source + FooterSource;

            VerifyCSharpDiagnostic(source, expectedResults);
        }

        protected void VerifyDiagnostic([CallerMemberName]string testName = null)
            => VerifyDiagnostic(new DiagnosticResult[0], testName);

        protected void VerifyDiagnostic(DiagnosticResult expected, [CallerMemberName]string testName = null)
            => VerifyDiagnostic(new[] { expected }, testName);

        protected void VerifyCodeFix(DiagnosticResult[] expectedResults, [CallerMemberName]string testName = null)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source.cs");
            var source = File.ReadAllText(sourcePath);
            source = HeaderSource + source + FooterSource;

            var newSourcePath = Path.Combine(DataSourcePath, testName, "NewSource.cs");
            var newSource = File.ReadAllText(newSourcePath);
            newSource = HeaderSource + newSource + FooterSource;

            VerifyCSharpDiagnostic(source, expectedResults);
            VerifyCSharpFix(source, newSource);
        }

        protected void VerifyCodeFix([CallerMemberName]string testName = null)
            => VerifyCodeFix(new DiagnosticResult[0], testName);

        protected void VerifyCodeFix(DiagnosticResult expected, [CallerMemberName]string testName = null)
            => VerifyCodeFix(new[] { expected }, testName);
    }
}
