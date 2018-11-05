using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Aerie.PowerShell.Async.Analyzer.Test
{
    [TestClass]
    public class UnitTest : DiagnosticVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Management.Automation;

    using Aerie.PowerShell;

    namespace Aerie.PowerShell.Async.Analyzer.UnitTests
    {
        public class AsyncCmdlet : Cmdlet, IAsyncCmdlet
        {
            public virtual void Dispose()
            {
                this.DoDispose();
                GC.SuppressFinalize(this);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "AeriePowerShellAsyncAnalyzer",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DisposeDiagnosticAnalyzer();
        }
    }
}
