using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterDescriptionProvider
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<DynamicParameterDescriptor> GetParameterDescriptors(
            [NotNull][ItemNotNull] ParameterMemberInfo member);
    }
}
