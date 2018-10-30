using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class DefaultInterceptor:
        IParameterInterceptor
    {
        private DefaultInterceptor()
        {
        }

        [NotNull]
        public static DefaultInterceptor Instance = new DefaultInterceptor();

        public void GetValue(
            IDynamicParameterContext context, 
            DynamicParameterDescriptor descriptor,
            ref object value)
        {
        }

        public void SetValue(
            IDynamicParameterContext context, 
            DynamicParameterDescriptor descriptor,
            ref object value)
        {
        }
    }
}
