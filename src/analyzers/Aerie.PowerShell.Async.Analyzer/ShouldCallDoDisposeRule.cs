using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace Aerie.PowerShell.Async.Analyzer
{
    public static class ShouldCallDoDisposeRule
    {
        public const string DiagnosticId = "ShouldCallDoDispose";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ShouldCallDoDisposeTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ShouldCallDoDisposeMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ShouldCallDoDisposeDescription), Resources.ResourceManager, typeof(Resources));

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            AnalyzerCategory.Correctness,
            DiagnosticSeverity.Warning,
            true,
            Description);
    }
}
