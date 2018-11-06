using System;
using System.Collections.Immutable;
using System.Linq;
using System.Management.Automation;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                    ShouldCallDoDisposeRule.Descriptor);
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
            var asyncCmdletSymbol = context.Compilation.GetTypeByMetadataName(typeof(IAsyncCmdlet).FullName);

            var doDisposeSymbol = (IMethodSymbol)context.Compilation
                                         .GetTypeByMetadataName(typeof(AsyncCmdletExtensions).FullName)
                                         .GetMembers(nameof(AsyncCmdletExtensions.DoDispose))[0];

            context.RegisterSyntaxNodeAction(
                syntaxContext => AnalyzeClass(
                    syntaxContext,
                    cmdletSymbol,
                    asyncCmdletSymbol,
                    doDisposeSymbol),
                SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(
            SyntaxNodeAnalysisContext context,
            INamedTypeSymbol cmdletType,
            INamedTypeSymbol asyncCmdletType,
            IMethodSymbol doDisposeMethodSymbol)
        {
            if (!(context.ContainingSymbol is INamedTypeSymbol namedTypeSymbol))
            {
                return;
            }

            if (!IsAsyncCmdlet(namedTypeSymbol, cmdletType, asyncCmdletType))
            {
                return;
            }

            var disposableType = context.Compilation.GetSpecialType(SpecialType.System_IDisposable);
            var disposeMethod = disposableType.GetMembers(nameof(IDisposable.Dispose))[0];
            var disposeMethodImplementation = (IMethodSymbol)namedTypeSymbol.FindImplementationForInterfaceMember(disposeMethod);

            bool foundDoDispose = DoDisposeCalled(context, disposeMethodImplementation, doDisposeMethodSymbol);

            if (!foundDoDispose)
            {
                var diagnostic = Diagnostic.Create(
                    ShouldCallDoDisposeRule.Descriptor,
                    namedTypeSymbol.Locations[0],
                    namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool DoDisposeCalled(
            SyntaxNodeAnalysisContext context,
            IMethodSymbol symbol,
            IMethodSymbol definitionSymbol,
            bool innerScope = false)
        {
            var location = symbol.Locations[0];

            if (!location.IsInSource)
            {
                return false;
            }

            var node = context.Node.FindNode(location.SourceSpan);
            var invocations = node.DescendantNodes().Where(n => n.IsKind(SyntaxKind.InvocationExpression));

            foreach (var invocation in invocations)
            {
                var symbol2 = (IMethodSymbol)context.SemanticModel.GetSymbolInfo(invocation).Symbol;

                if (IsDoDisposeCall(symbol2, definitionSymbol))
                {
                    return true;
                }

                if (!innerScope &&
                    symbol2.Name == "Dispose" &&
                    symbol2.Parameters.Length == 1 &&
                    symbol2.Parameters[0].Type.SpecialType == SpecialType.System_Boolean)
                {
                    if (DoDisposeCalled(context, symbol2, definitionSymbol, true))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsDoDisposeCall(
            IMethodSymbol symbol,
            IMethodSymbol definitionSymbol)
        {
            if (symbol.MethodKind == MethodKind.ReducedExtension)
            {
                symbol = symbol.GetConstructedReducedFrom();

                if (symbol.IsGenericMethod)
                {
                    symbol = symbol.OriginalDefinition;

                    if (symbol.Equals(definitionSymbol))
                    {
                        return true;
                    }
                }
            }

            return false;
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
