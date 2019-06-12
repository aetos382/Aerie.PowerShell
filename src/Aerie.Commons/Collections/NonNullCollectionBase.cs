using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using Aerie.Commons.Contracts;

namespace Aerie.Commons.Collections
{
    public abstract class NonNullCollectionBase<T> :
        ICollection<T>,
        IReadOnlyCollection<T>
        where T : class
    {
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.BaseCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        public void Add(
            [NotNull] T item)
        {
            Ensures.ArgumentNotNull(item, nameof(item));

            this.BaseCollection.Add(item);
        }

        public void Clear()
        {
            this.BaseCollection.Clear();
        }

        public bool Contains(
            [NotNull] T item)
        {
            Ensures.ArgumentNotNull(item, nameof(item));

            return this.BaseCollection.Contains(item);
        }

        public void CopyTo(
            T[] array,
            int arrayIndex)
        {
            Ensures.ArgumentNotNull(array, nameof(array));

            this.BaseCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(
            [NotNull] T item)
        {
            Ensures.ArgumentNotNull(item, nameof(item));

            return this.BaseCollection.Remove(item);
        }

        public int Count
        {
            get
            {
                return this.BaseCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.BaseCollection.IsReadOnly;
            }
        }

        [NotNull]
        [ItemNotNull]
        protected abstract ICollection<T> BaseCollection { get; }
    }
}
