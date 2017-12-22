using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using TestHelper;
using Xunit;
using MixinGenerator;

namespace MixinGenerator.Test
{
    public class MixinGeneratorUnitTests : ConventionCodeFixVerifier
    {
        [Fact]
        public void EmptySource() => VerifyCSharpByConvention();

        [Fact]
        public void LowercaseLetters() => VerifyCSharpByConvention();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}
