using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterDescriptorProvider
    {
        [NotNull]
        DynamicParameterDescriptor GetDynamicParameterDescriptor(
            [NotNull][ItemNotNull] PropertyOrFieldChain chain);
    }
}
