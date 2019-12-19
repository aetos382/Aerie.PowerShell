using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using JetBrains.Annotations;

using Microsoft;

namespace Aerie.PowerShell.DynamicParameter
{
    public class DynamicParameterDescriptorCollection :
        Collection<ParameterDescriptor>,
        IEquatable<DynamicParameterDescriptorCollection>
    {
        public DynamicParameterDescriptorCollection(
            [NotNull][ItemNotNull] IEnumerable<ParameterDescriptor> descriptors)
        {
            Requires.NotNull(descriptors, nameof(descriptors));

            foreach (var descriptor in descriptors)
            {
                this.Add(descriptor);
            }
        }

        public bool Equals(
            DynamicParameterDescriptorCollection other)
        {
            if (other is null)
            {
                return false;
            }

            if (!other.SequenceEqual(this))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is DynamicParameterDescriptorCollection collection))
            {
                return false;
            }

            if (!this.Equals(collection))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            foreach (var descriptor in this)
            {
                hashCode.Add(descriptor);
            }

            return hashCode.ToHashCode();
        }
    }
}
