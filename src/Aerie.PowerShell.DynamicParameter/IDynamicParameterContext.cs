using System;
using System.Collections.Generic;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface IDynamicParameterContext
    {
        [NotNull]
        Cmdlet Cmdlet { [Pure] get; }

        [NotNull]
        Type CmdletType { [Pure] get; }

        [NotNull]
        [ItemNotNull]
        [Pure]
        IReadOnlyCollection<DynamicParameter> GetParameters();
    }
}
