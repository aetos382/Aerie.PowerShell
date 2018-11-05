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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    ShouldCallDoDisposeDescriptor.Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStart);
        }

        private static void CompilationStart(
            CompilationStartAnalysisContext context)
        {
            var cmdletSymbol = context.Compilation.GetTypeByMetadataName(typeof(Cmdlet).FullName);
            var disposableSymbol = context.Compilation.GetTypeByMetadataName(typeof(IDisposable).FullName);
            var asyncCmdletSymbol = context.Compilation.GetTypeByMetadataName(typeof(IAsyncCmdlet).FullName);

            context.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeMethod(syntaxContext, cmdletSymbol, disposableSymbol, asyncCmdletSymbol),
                SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethod(
            SyntaxNodeAnalysisContext context,
            INamedTypeSymbol cmdletType,
            INamedTypeSymbol disposableType,
            INamedTypeSymbol asyncCmdletType)
        {
            if (!(context.ContainingSymbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            if (!IsAsyncCmdlet(methodSymbol.ContainingType, cmdletType, asyncCmdletType))
            {
                return;
            }
        }

        private static bool IsAsyncCmdlet(
            INamedTypeSymbol concreteType,
            INamedTypeSymbol cmdletType,
            INamedTypeSymbol asyncCmdletType)
        {
            if (!concreteType.AllInterfaces.Contains(asyncCmdletType))
            {
                return false;
            }

            var baseType = concreteType.BaseType;

            for (; baseType != cmdletType; baseType = baseType.BaseType) ;

            if (baseType != cmdletType)
            {
                return false;
            }

            return true;
        }
    }
}
