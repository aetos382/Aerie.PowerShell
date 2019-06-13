using System;
using System.Collections.Generic;
using System.Management.Automation;

using Aerie.Commons.Collections;
using Aerie.Commons.Contracts;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class CmdletTypeDescriptor
    {
        protected CmdletTypeDescriptor(
            [NotNull] Type cmdletType)
        {
            Ensures.ArgumentNotNull(cmdletType, nameof(cmdletType));

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

        [NotNull]
        [ItemNotNull]
        public ICollection<ParameterDescriptor> ParameterDescriptors { get; } =
            new NonNullHashSet<ParameterDescriptor>();

        private bool _initialized = false;

        public void CreateParameterDescriptors()
        {
            if (this._initialized)
            {
                return;
            }

            foreach (var provider in this._providers)
            {
                var descriptors = provider.GetParameterDescriptors(this.CmdletType);

                foreach (var descriptor in descriptors)
                {
                    this.ParameterDescriptors.Add(descriptor);
                }
            }

            this._initialized = true;
        }

        [NotNull]
        [ItemNotNull]
        private static readonly KeyedByTypeCollection<CmdletTypeDescriptor> _descriptors =
            new KeyedByTypeCollection<CmdletTypeDescriptor>();

        [NotNull]
        public static CmdletTypeDescriptor GetDescriptor(
            [NotNull] Type cmdletType)
        {
            Ensures.ArgumentNotNull(cmdletType, nameof(cmdletType));

            if (!cmdletType.IsSubclassOf(typeof(Cmdlet)))
            {
                throw new ArgumentException();
            }

            if (!_descriptors.TryGetValue(cmdletType, out var descriptor))
            {
                descriptor = new CmdletTypeDescriptor(cmdletType);
                _descriptors.Add(descriptor);
            }

            return descriptor;
        }
    }
}
