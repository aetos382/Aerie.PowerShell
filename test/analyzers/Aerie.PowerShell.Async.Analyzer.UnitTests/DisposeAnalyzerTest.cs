using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TestHelper;

namespace Aerie.PowerShell.Async.Analyzer.UnitTests
{
    [TestClass]
    public class DisposeAnalyzerTest :
        DiagnosticVerifier
    {
        [TestMethod]
        public void NotReportedForEmptySource()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void NotReportedForCorrectDisposeMethod()
        {
            var test = @"
using System;
using System.Management.Automation;
using System.Threading.Tasks;

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

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void NotReportedForCorrectDisposeBoolMethod()
        {
            var test = @"
using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Aerie.PowerShell.Async.Analyzer.UnitTests
{
    public class AsyncCmdlet : Cmdlet, IAsyncCmdlet
    {
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            this.DoDispose();
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void ReportedForIncorrectDisposeMethod()
        {
            var test = @"using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Aerie.PowerShell.Async.Analyzer.UnitTests
{
    public class AsyncCmdlet : Cmdlet, IAsyncCmdlet
    {
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "ShouldCallDoDispose",
                Message = "Should call AsyncCmdletExtensions.DoDispose() from AsyncCmdlet.Dispose() or AsyncCmdlet.Dispose(bool).",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 7, 18)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void ReportedForIncorrectDisposeBoolMethod()
        {
            var test = @"using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Aerie.PowerShell.Async.Analyzer.UnitTests
{
    public class AsyncCmdlet : Cmdlet, IAsyncCmdlet
    {
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "ShouldCallDoDispose",
                Message = "Should call AsyncCmdletExtensions.DoDispose() from AsyncCmdlet.Dispose() or AsyncCmdlet.Dispose(bool).",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 7, 18)
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
