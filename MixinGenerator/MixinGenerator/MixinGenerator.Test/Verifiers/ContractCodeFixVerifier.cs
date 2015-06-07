using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

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

        protected void VerifyDiagnostic([CallerMemberName]string testName = null)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source.cs");
            var source = File.ReadAllText(sourcePath);
            source = HeaderSource + source + FooterSource;

            var resultsPath = Path.Combine(DataSourcePath, testName, "Results.csv");
            var expectedResults = ReadResults(resultsPath).ToArray();

            VerifyCSharpDiagnostic(source, expectedResults);
        }

        protected void VerifyCodeFix([CallerMemberName]string testName = null)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source.cs");
            var source = File.ReadAllText(sourcePath);
            source = HeaderSource + source + FooterSource;

            var newSourcePath = Path.Combine(DataSourcePath, testName, "NewSource.cs");
            var newSource = File.ReadAllText(newSourcePath);
            newSource = HeaderSource + newSource + FooterSource;

            var resultsPath = Path.Combine(DataSourcePath, testName, "Results.csv");
            var expectedResults = ReadResults(resultsPath).ToArray();

            VerifyCSharpDiagnostic(source, expectedResults);
            VerifyCSharpFix(source, newSource);
        }

        private IEnumerable<DiagnosticResult> ReadResults(string path)
        {
            if (!File.Exists(path)) yield break;

            foreach (var line in File.ReadLines(path))
            {
                var tokens = line.Split(',');
                tokens = tokens.Select(x => x.Trim(' ')).ToArray();

                var id = tokens[0];
                DiagnosticSeverity severity;
                Enum.TryParse(tokens[1], out severity);
                var lineNo = int.Parse(tokens[2]);
                var column = int.Parse(tokens[3]);
                var args = tokens.Skip(4).ToArray();

                yield return GetResult(id, severity, lineNo, column, args);
            }
        }

        protected DiagnosticResult GetResult(string diagnosticId, DiagnosticSeverity serivity, int line, int column, params string[] messageArgs)
        {
            var analyser = GetCSharpDiagnosticAnalyzer();
            var diag = analyser.SupportedDiagnostics.First(x => x.Id == diagnosticId);
            return new DiagnosticResult
            {
                Id = diagnosticId,
                Message = string.Format(diag.MessageFormat.ToString(), (object[])messageArgs),
                Severity = serivity,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) },
            };
        }
    }
}
