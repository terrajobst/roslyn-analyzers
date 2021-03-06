﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Test.Utilities.CSharpSecurityCodeFixVerifier<
    Microsoft.NetCore.Analyzers.Security.DoNotAddSchemaByURL,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = Test.Utilities.VisualBasicSecurityCodeFixVerifier<
    Microsoft.NetCore.Analyzers.Security.DoNotAddSchemaByURL,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Microsoft.NetCore.Analyzers.Security.UnitTests
{
    public class DoNotAddSchemaByURLTests
    {
        [Fact]
        public async Task TestAddWithStringStringParametersDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml.Schema;

class TestClass
{
    public void TestMethod()
    {
        XmlSchemaCollection xsc = new XmlSchemaCollection();
        xsc.Add(""urn: bookstore - schema"", ""books.xsd"");
    }
}",
            GetCSharpResultAt(10, 9));

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml.Schema

class TestClass
    public Sub TestMethod
        Dim xsc As New XmlSchemaCollection
        xsc.Add(""urn: bookstore - schema"", ""books.xsd"")
    End Sub
End Class",
            GetBasicResultAt(8, 9));
        }

        [Fact]
        public async Task TestAddWithNullStringParametersDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml.Schema;

class TestClass
{
    public void TestMethod()
    {
        XmlSchemaCollection xsc = new XmlSchemaCollection();
        xsc.Add(null, ""books.xsd"");
    }
}",
            GetCSharpResultAt(10, 9));

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml.Schema

class TestClass
    public Sub TestMethod
        Dim xsc As New XmlSchemaCollection
        xsc.Add(Nothing, ""books.xsd"")
    End Sub
End Class",
            GetBasicResultAt(8, 9));
        }

        [Fact]
        public async Task TestAddWithXmlSchemaCollectionParameterNoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml.Schema;

class TestClass
{
    public void TestMethod()
    {
        XmlSchemaCollection xsc = new XmlSchemaCollection();
        xsc.Add(xsc);
    }
}");
        }

        [Fact]
        public async Task TestAddWithXmlSchemaParameterNoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml.Schema;

class TestClass
{
    public void TestMethod()
    {
        XmlSchemaCollection xsc = new XmlSchemaCollection();
        xsc.Add(new XmlSchema());
    }
}");
        }

        [Fact]
        public async Task TestNormalAddMethodNoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml.Schema;

class TestClass
{
    public static void Add (string ns, string uri)
    {
    }

    public void TestMethod()
    {
        TestClass.Add(""urn: bookstore - schema"", ""books.xsd"");
    }
}");
        }

        private static DiagnosticResult GetCSharpResultAt(int line, int column)
            => VerifyCS.Diagnostic()
                .WithLocation(line, column);

        private static DiagnosticResult GetBasicResultAt(int line, int column)
            => VerifyVB.Diagnostic()
                .WithLocation(line, column);
    }
}
