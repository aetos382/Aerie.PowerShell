using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class DynamicParameterDescriptionProviderCollection :
        ICollection<IDynamicParameterDescriptionProvider>,
        IReadOnlyCollection<IDynamicParameterDescriptionProvider>
    {
        [NotNull]
        private readonly HashSet<IDynamicParameterDescriptionProvider> _innerSet =
            new HashSet<IDynamicParameterDescriptionProvider>();

        public DynamicParameterDescriptionProviderCollection()
        {
        }

        public DynamicParameterDescriptionProviderCollection(
            [NotNull][ItemNotNull] IReadOnlyCollection<IDynamicParameterDescriptionProvider> items)
        {
            Ensure.ArgumentNotNull(items, nameof(items));

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        IEnumerator<IDynamicParameterDescriptionProvider> IEnumerable<IDynamicParameterDescriptionProvider>.GetEnumerator()
        {
            return this._innerSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IDynamicParameterDescriptionProvider>)this).GetEnumerator();
        }

        public void Add(
            [NotNull] IDynamicParameterDescriptionProvider item)
        {
            Ensure.ArgumentNotNull(item, nameof(item));

            this._innerSet.Add(item);
        }

        public void Clear()
        {
            this._innerSet.Clear();
        }

        public bool Contains(
            [NotNull] IDynamicParameterDescriptionProvider item)
        {
            Ensure.ArgumentNotNull(item, nameof(item));

            return this._innerSet.Contains(item);
        }

        public void CopyTo(
            [NotNull] IDynamicParameterDescriptionProvider[] array,
            int arrayIndex)
        {
            Ensure.ArgumentNotNull(array, nameof(array));

            this._innerSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(
            [NotNull] IDynamicParameterDescriptionProvider item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return this._innerSet.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }
}
