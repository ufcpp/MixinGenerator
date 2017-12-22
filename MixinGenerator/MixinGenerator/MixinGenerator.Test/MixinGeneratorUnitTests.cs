﻿using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace MixinGenerator.Test
{
    public class MixinGeneratorUnitTests : ConventionCodeFixVerifier
    {
        [Fact] public void EmptySource() => VerifyCSharpByConvention();
        [Fact] public void Annotaion() => VerifyCSharpByConvention();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}
