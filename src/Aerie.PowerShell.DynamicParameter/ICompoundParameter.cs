using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface ICompoundParameter
    {
        void ConfigureDynamicParameters(
            [NotNull] ICompoundParameterContext context);
    }
}
