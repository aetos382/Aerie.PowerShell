using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Aerie.PowerShell.Async.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisposeDiagnosticAnalyzer :
        DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(
            ShouldCallDoDisposeDescriptor.Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            if (!IsAsyncCmdlet(context))
            {
                return;
            }
        }

        private static bool IsAsyncCmdlet(
            SyntaxNodeAnalysisContext context)
        {
            if (!(context.ContainingSymbol is IMethodSymbol methodSymbol))
            {
                return false;
            }

            var asyncCmdletSymbol = context.Compilation.GetTypeByMetadataName(typeof(IAsyncCmdlet).FullName);
            var cmdletSymbol = context.Compilation.GetTypeByMetadataName(typeof(Cmdlet).FullName);

            if (!methodSymbol.ContainingType.AllInterfaces.Contains(asyncCmdletSymbol))
            {
                return false;
            }

            var cmdletType = methodSymbol.ContainingType.BaseType;

            for (; cmdletType != cmdletSymbol; cmdletType = cmdletType.BaseType) ;

            if (cmdletType != cmdletSymbol)
            {
                return false;
            }

            return true;
        }
    }
}
