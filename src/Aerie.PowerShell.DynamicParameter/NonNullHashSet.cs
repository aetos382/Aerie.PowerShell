using System;
using System.Collections.Generic;

namespace Aerie.PowerShell.DynamicParameter
{
    public class NonNullHashSet<T> :
        NonNullCollectionBase<T>
        where T : class
    {
        public NonNullHashSet()
        {
            this.BaseCollection = new HashSet<T>();
        }

        public NonNullHashSet(
            IReadOnlyCollection<T> collection)
        {
            this.BaseCollection = new HashSet<T>(collection);
        }

        public NonNullHashSet(
            IReadOnlyCollection<T> collection,
            IEqualityComparer<T> comparer)
        {
            this.BaseCollection = new HashSet<T>(collection, comparer);
        }

        public NonNullHashSet(
            int capacity)
        {
            this.BaseCollection = new HashSet<T>(capacity);
        }

        protected override ICollection<T> BaseCollection { get; }
    }
}
