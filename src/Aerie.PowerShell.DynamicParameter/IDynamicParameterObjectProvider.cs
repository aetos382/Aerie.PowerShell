using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface IDynamicParameterObjectProvider
    {
        [NotNull]
        object GetDynamicParameterObject(
            [NotNull] IDynamicParameterContext context);
    }
}
