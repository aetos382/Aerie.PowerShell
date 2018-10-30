using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface ICompoundParameter
    {
        void ConfigureDynamicParameters(
            [NotNull] ICompoundParameterContext context);
    }
}
