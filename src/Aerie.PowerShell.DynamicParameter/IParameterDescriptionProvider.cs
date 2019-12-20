using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface IParameterDescriptionProvider
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<DynamicParameterDescriptor> GetParameterDescriptors(
            [NotNull] Type cmdletType);
    }
}
