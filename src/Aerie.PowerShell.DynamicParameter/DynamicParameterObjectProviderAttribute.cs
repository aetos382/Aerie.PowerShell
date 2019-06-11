using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DynamicParameterObjectProviderAttribute :
        Attribute
    {
        public DynamicParameterObjectProviderAttribute(
            [NotNull] Type providerType)
        {
            if (providerType is null)
            {
                throw new ArgumentNullException(nameof(providerType));
            }

            if (!ValidateProviderType(providerType))
            {
                throw new ArgumentException();
            }

            this.ProviderType = providerType;
        }

        [NotNull]
        public Type ProviderType { get; }

        private static bool ValidateProviderType(
            [NotNull] Type providerType)
        {
            if (!typeof(IDynamicParameterObjectProvider).IsAssignableFrom(providerType))
            {
                return false;
            }

            if (!providerType.IsDefaultConstructible())
            {
                return false;
            }

            return true;
        }
    }
}
