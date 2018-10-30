using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterObjectBuilder
    {
        [NotNull]
        object GetDynamicParameterObject(
            [NotNull] IDynamicParameterContext context);
    }
}
