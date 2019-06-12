using System;
using System.Collections.Generic;

namespace Aerie.Commons.Collections
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
        
        protected override ICollection<T> BaseCollection { get; }
    }
}
