using System;
using System.Collections.Generic;
using System.Text;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class CmdletTypeDescriptor
    {
        public CmdletTypeDescriptor(
            [NotNull] Type cmdletType)
        {
            Ensure.ArgumentNotNull(cmdletType, nameof(cmdletType));

            this.CmdletType = cmdletType;
        }

        [NotNull]
        public Type CmdletType { get; }

        [NotNull]
        [ItemNotNull]
        private static readonly ParameterDescriptionProviderCollection _defaultProviders =
            new ParameterDescriptionProviderCollection
            {
                new ReflectDynamicParameterDescriptionProvider(),
                new CompoundParameterDescriptionProvider()
            };

        [NotNull]
        [ItemNotNull]
        public static ParameterDescriptionProviderCollection DefaultProviders
        {
            [Pure]
            get
            {
                return _defaultProviders;
            }
        }

        [NotNull]
        [ItemNotNull]
        private readonly ParameterDescriptionProviderCollection _providers =
            new ParameterDescriptionProviderCollection(_defaultProviders);

        [NotNull]
        [ItemNotNull]
        public ParameterDescriptionProviderCollection DynamicParameterDescriptionProviders
        {
            [Pure]
            get
            {
                return this._providers;
            }
        }
    }
}
