using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Aerie.PowerShell.Async.Analyzer
{
    internal class DisposeMethod
    {
        public static bool TryParse(
            SyntaxNodeAnalysisContext context,
            out DisposeMethod method)
        {
            method = default;

            if (!(context.ContainingSymbol is IMethodSymbol methodSymbol) ||
                !(context.Node is MethodDeclarationSyntax methodDeclaration) ||
                methodSymbol.Name != "Dispose" ||
                !methodSymbol.ReturnsVoid)
            {
                return false;
            }

            return false;
        }

        private DisposeMethod()
        {
        }
    }
}
