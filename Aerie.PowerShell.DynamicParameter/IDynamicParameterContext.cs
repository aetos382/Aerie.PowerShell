using System;
using System.Collections.Generic;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterContext
    {
        [NotNull]
        Cmdlet Cmdlet { [Pure] get; }

        [NotNull]
        [ItemNotNull]
        [Pure]
        IReadOnlyCollection<DynamicParameterInstance> GetParameters();
    }
}
