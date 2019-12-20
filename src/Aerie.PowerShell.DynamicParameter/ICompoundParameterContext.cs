using System;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface ICompoundParameterContext
    {
        [NotNull]
        ICmdletContext ParentContext { [Pure] get; }

        [NotNull]
        MemberInfo CurrentParameter { [Pure] get; }
    }
}
