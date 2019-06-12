using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface IDynamicParameterDescriptionProvider
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<ParameterDescriptor> GetParameterDescriptors(
            [NotNull] IDynamicParameterContext context);
    }
}
