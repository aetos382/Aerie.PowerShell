using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class DynamicParameterDescriptionProviderCollection :
        ICollection<IParameterDescriptionProvider>,
        IReadOnlyCollection<IParameterDescriptionProvider>
    {
        [NotNull]
        private readonly HashSet<IParameterDescriptionProvider> _innerSet =
            new HashSet<IParameterDescriptionProvider>();

        public DynamicParameterDescriptionProviderCollection()
        {
        }

        public DynamicParameterDescriptionProviderCollection(
            [NotNull][ItemNotNull] IReadOnlyCollection<IParameterDescriptionProvider> items)
        {
            Ensure.ArgumentNotNull(items, nameof(items));

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        IEnumerator<IParameterDescriptionProvider> IEnumerable<IParameterDescriptionProvider>.GetEnumerator()
        {
            return this._innerSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IParameterDescriptionProvider>)this).GetEnumerator();
        }

        public void Add(
            [NotNull] IParameterDescriptionProvider item)
        {
            Ensure.ArgumentNotNull(item, nameof(item));

            this._innerSet.Add(item);
        }

        public void Clear()
        {
            this._innerSet.Clear();
        }

        public bool Contains(
            [NotNull] IParameterDescriptionProvider item)
        {
            Ensure.ArgumentNotNull(item, nameof(item));

            return this._innerSet.Contains(item);
        }

        public void CopyTo(
            [NotNull] IParameterDescriptionProvider[] array,
            int arrayIndex)
        {
            Ensure.ArgumentNotNull(array, nameof(array));

            this._innerSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(
            [NotNull] IParameterDescriptionProvider item)
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
