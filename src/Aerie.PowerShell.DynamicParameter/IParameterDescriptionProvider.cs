using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface IParameterDescriptionProvider
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<ParameterDescriptor> GetParameterDescriptors(
            [NotNull] Type cmdletType);
    }
}
