using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IDynamicParameterAttributeProvider
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<CustomAttributeData> GetAttributeData(
            [NotNull] MemberInfo member);
    }
}
