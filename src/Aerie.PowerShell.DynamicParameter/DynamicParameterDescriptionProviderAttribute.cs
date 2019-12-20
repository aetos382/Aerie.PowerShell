using System;

using Microsoft;

namespace Aerie.PowerShell.DynamicParameter
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Property |
        AttributeTargets.Field)]
    public sealed class DynamicParameterDescriptionProviderAttribute :
        Attribute
    {
        public DynamicParameterDescriptionProviderAttribute(
            Type providerType)
        {
            Requires.NotNull(providerType, nameof(providerType));

            this.ProviderType = providerType;
        }

        public Type ProviderType { get; }
    }
}
