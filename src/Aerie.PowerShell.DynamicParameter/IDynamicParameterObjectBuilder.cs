using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface IDynamicParameterObjectBuilder
    {
        [NotNull]
        object Build(
            [NotNull] ICmdletContext context);
    }
}
