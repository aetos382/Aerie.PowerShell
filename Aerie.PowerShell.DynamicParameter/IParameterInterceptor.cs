using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public interface IParameterInterceptor
    {
        void GetValue(
            [NotNull] IDynamicParameterContext context,
            [NotNull] DynamicParameterDescriptor descriptor,
            [CanBeNull] ref object value);

        void SetValue(
            [NotNull] IDynamicParameterContext context, 
            [NotNull] DynamicParameterDescriptor descriptor,
            [CanBeNull] ref object value);
    }
}
