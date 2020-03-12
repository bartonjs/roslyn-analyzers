// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Test.Utilities.CSharpCodeFixVerifier<
    Microsoft.NetCore.Analyzers.Runtime.DoNotRaiseReservedExceptionTypesAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = Test.Utilities.VisualBasicCodeFixVerifier<
    Microsoft.NetCore.Analyzers.Runtime.DoNotRaiseReservedExceptionTypesAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Microsoft.NetCore.Analyzers.Runtime.UnitTests
{
    public class DoNotUseReferenceEqualsWithValueTypesTests
    {
        [Fact]
        public async Task CreateSystemNotImplementedException()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod()
        {
            throw new NotImplementedException();
        }
    }
}");

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System

Namespace TestNamespace
	Class TestClass
		Private Shared Sub TestMethod()
            Throw New NotImplementedException()
		End Sub
	End Class
End Namespace");
        }

        [Fact]
        public async Task CreateSystemException()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;

namespace TestNamespace
{
    class TestClass
    {
        private static bool TestMethod()
        {
            return object.ReferenceEquals(4, string.Concat(4, string.Empty));
        }
    }
}",
                GetTooGenericCSharpResultAt(10, 19, "System.Exception"));

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System

Namespace TestNamespace
	Class TestClass
		Private Shared Sub TestMethod()
            Throw New Exception()
		End Sub
	End Class
End Namespace",
                GetTooGenericBasicResultAt(7, 19, "System.Exception"));
        }

        private DiagnosticResult GetTooGenericCSharpResultAt(int line, int column, string callee)
            => VerifyCS.Diagnostic(DoNotRaiseReservedExceptionTypesAnalyzer.TooGenericRule)
                .WithLocation(line, column)
                .WithArguments(callee);

        private DiagnosticResult GetTooGenericBasicResultAt(int line, int column, string callee)
            => VerifyVB.Diagnostic(DoNotRaiseReservedExceptionTypesAnalyzer.TooGenericRule)
                .WithLocation(line, column)
                .WithArguments(callee);
    }
}