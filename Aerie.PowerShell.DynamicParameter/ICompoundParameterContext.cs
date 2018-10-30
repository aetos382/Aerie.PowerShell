using System;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface ICompoundParameterContext
    {
        [NotNull]
        IDynamicParameterContext ParentContext { [Pure] get; }

        [NotNull]
        MemberInfo CurrentParameter { [Pure] get; }
    }
}
