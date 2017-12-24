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
        [Fact] public void SimpleProperty() => VerifyCSharpByConvention();
        [Fact] public void SimpleMethod() => VerifyCSharpByConvention();
        [Fact] public void SimpleEvent() => VerifyCSharpByConvention();
        [Fact] public void SimpleMembers() => VerifyCSharpByConvention();
        [Fact] public void GenericMethod() => VerifyCSharpByConvention();
        [Fact] public void MultipleMixins() => VerifyCSharpByConvention();
        [Fact] public void GenericTypes() => VerifyCSharpByConvention();
        [Fact] public void GenericConstraints() => VerifyCSharpByConvention();
        [Fact] public void RefMethod() => VerifyCSharpByConvention();
        [Fact] public void InterfaceImplementation() => VerifyCSharpByConvention();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MixinGeneratorCodeFixProvider();
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new MixinGeneratorAnalyzer();
    }
}
