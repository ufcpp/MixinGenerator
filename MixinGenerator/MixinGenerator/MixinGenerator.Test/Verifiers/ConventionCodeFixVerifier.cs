using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace TestHelper
{
    public class Result
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "sevirity")]
        public DiagnosticSeverity Sevirity { get; set; }

        [JsonProperty(PropertyName = "line")]
        public int Line { get; set; }

        [JsonProperty(PropertyName = "column")]
        public int Column { get; set; }

        [JsonProperty(PropertyName = "message-args")]
        public string[] MessageArgs { get; set; }
    }

    public abstract class ConventionCodeFixVerifier : CodeFixVerifier
    {
        public ConventionCodeFixVerifier()
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

        protected void VerifyCSharpByConvention([CallerMemberName]string testName = null)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source.cs");
            var source = File.ReadAllText(sourcePath);
            source = HeaderSource + source + FooterSource;

            var resultsPath = Path.Combine(DataSourcePath, testName, "Results.json");
            var expectedResults = ReadResults(resultsPath).ToArray();

            var newSourcePath = Path.Combine(DataSourcePath, testName, "NewSource.cs");
            if (File.Exists(newSourcePath))
            {
                var newSource = File.ReadAllText(newSourcePath);
                newSource = HeaderSource + newSource + FooterSource;

                VerifyCSharpDiagnostic(source, expectedResults);
                VerifyCSharpFix(source, newSource);
            }
        }

        private IEnumerable<DiagnosticResult> ReadResults(string path)
        {
            if (!File.Exists(path)) yield break;

            var results = JsonConvert.DeserializeObject<Result[]>(File.ReadAllText(path));

            var analyzer = GetCSharpDiagnosticAnalyzer().SupportedDiagnostics.ToDictionary(x => x.Id);

            foreach (var r in results)
            {
                var diag = analyzer[r.Id];
                yield return new DiagnosticResult
                {
                    Id = r.Id,
                    Message = r.MessageArgs == null ? diag.MessageFormat.ToString() : string.Format(diag.MessageFormat.ToString(), (object[])r.MessageArgs),
                    Severity = r.Sevirity,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", r.Line, r.Column) },
                };
            }
        }
    }
}
