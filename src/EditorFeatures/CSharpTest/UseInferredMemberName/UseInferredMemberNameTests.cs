﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.UseInferredMemberName;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Diagnostics;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.InferredMemberName
{
    [Trait(Traits.Feature, Traits.Features.CodeActionsUseInferredMemberName)]
    public class UseInferredMemberNameTests : AbstractCSharpDiagnosticProviderBasedUserDiagnosticTest
    {
        internal override (DiagnosticAnalyzer, CodeFixProvider) CreateDiagnosticProviderAndFixer(Workspace workspace)
            => (new CSharpUseInferredMemberNameDiagnosticAnalyzer(), new CSharpUseInferredMemberNameCodeFixProvider());

        private static readonly CSharpParseOptions s_parseOptions =
            CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

        [Fact]
        public async Task TestInferredTupleName()
        {
            await TestAsync(
@"
class C
{
    void M()
    {
        int a = 1;
        var t = ([||]a: a, 2);
    }
}",
@"
class C
{
    void M()
    {
        int a = 1;
        var t = ( a, 2);
    }
}", parseOptions: s_parseOptions);
        }

        [Fact]
        public async Task TestInferredTupleNameAfterCommaWithCSharp6()
        {
            await TestActionCountAsync(
@"
class C
{
    void M()
    {
        int a = 2;
        var t = (1, [||]a: a);
    }
}", count: 0, parameters: new TestParameters(CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp6)));
        }

        [Fact]
        public async Task TestInferredTupleNameAfterCommaWithCSharp7()
        {
            await TestActionCountAsync(
@"
class C
{
    void M()
    {
        int a = 2;
        var t = (1, [||]a: a);
    }
}", count: 0, parameters: new TestParameters(CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7)));
        }

        [Fact]
        public async Task TestFixAllInferredTupleNameWithTrivia()
        {
            await TestAsync(
@"
class C
{
    void M()
    {
        int a = 1;
        int b = 2;
        var t = ( /*before*/ {|FixAllInDocument:a:|} /*middle*/ a /*after*/, /*before*/ b: /*middle*/ b /*after*/);
    }
}",
@"
class C
{
    void M()
    {
        int a = 1;
        int b = 2;
        var t = ( /*before*/  /*middle*/ a /*after*/, /*before*/  /*middle*/ b /*after*/);
    }
}", parseOptions: s_parseOptions, ignoreTrivia: false);
        }

        [Fact]
        public async Task TestInferredAnonymousTypeMemberName()
        {
            await TestAsync(
@"
class C
{
    void M()
    {
        int a = 1;
        var t = new { [||]a=a, 2 };
    }
}",
@"
class C
{
    void M()
    {
        int a = 1;
        var t = new { a, 2 };
    }
}", parseOptions: s_parseOptions);
        }

        [Fact]
        public async Task TestFixAllInferredAnonymousTypeMemberNameWithTrivia()
        {
            await TestAsync(
@"
class C
{
    void M()
    {
        int a = 1;
        int b = 2;
        var t = new { /*before*/ {|FixAllInDocument:a =|} /*middle*/ a /*after*/, /*before*/ b = /*middle*/ b /*after*/ };
    }
}",
@"
class C
{
    void M()
    {
        int a = 1;
        int b = 2;
        var t = new { /*before*/  /*middle*/ a /*after*/, /*before*/  /*middle*/ b /*after*/ };
    }
}", parseOptions: s_parseOptions, ignoreTrivia: false);
        }
    }
}
