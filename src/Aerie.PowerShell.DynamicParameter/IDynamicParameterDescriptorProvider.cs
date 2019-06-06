using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterDescriptorProvider
    {
        [NotNull]
        DynamicParameterDescriptor GetDynamicParameterDescriptor(
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members);
    }
}
