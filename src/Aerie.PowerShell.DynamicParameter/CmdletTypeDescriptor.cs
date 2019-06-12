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
        private static readonly DynamicParameterDescriptionProviderCollection _defaultProviders =
            new DynamicParameterDescriptionProviderCollection
            {
                new ReflectDynamicParameterDescriptionProvider(),
                new CompoundParameterDescriptionProvider()
            };

        [NotNull]
        [ItemNotNull]
        public static DynamicParameterDescriptionProviderCollection DefaultProviders
        {
            [Pure]
            get
            {
                return _defaultProviders;
            }
        }

        [NotNull]
        [ItemNotNull]
        private readonly DynamicParameterDescriptionProviderCollection _providers =
            new DynamicParameterDescriptionProviderCollection(_defaultProviders);

        [NotNull]
        [ItemNotNull]
        public DynamicParameterDescriptionProviderCollection DynamicParameterDescriptionProviders
        {
            [Pure]
            get
            {
                return this._providers;
            }
        }
    }
}
