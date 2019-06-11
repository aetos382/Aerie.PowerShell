using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterObjectProvider
    {
        [NotNull]
        object GetDynamicParameterObject(
            [NotNull] IDynamicParameterContext context);
    }
}
