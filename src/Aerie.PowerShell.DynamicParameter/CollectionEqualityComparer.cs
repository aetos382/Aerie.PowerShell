using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class CollectionEqualityComparer<T> :
        IEqualityComparer<IEnumerable<T>>,
        IEqualityComparer
    {
        [NotNull]
        private readonly IEqualityComparer<T> _elementComparer;

        public CollectionEqualityComparer()
            : this(null)
        {
        }

        public CollectionEqualityComparer(
            [CanBeNull] IEqualityComparer<T> elementComparer)
        {
            this._elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(
            IEnumerable<T> x,
            IEnumerable<T> y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            var equals = x.SequenceEqual(y, this._elementComparer);
            return equals;
        }

        public int GetHashCode(
            IEnumerable<T> obj)
        {
            if (obj is null)
            {
                return 0;
            }

            int hashCode = 0;

            var comparer = this._elementComparer;

            foreach (var element in obj)
            {
                if (object.ReferenceEquals(element, null))
                {
                    continue;
                }

                hashCode ^= comparer.GetHashCode(element);
            }

            return hashCode;
        }

        [NotNull]
        public static CollectionEqualityComparer<T> Default = new CollectionEqualityComparer<T>();

        bool IEqualityComparer.Equals(
            object x,
            object y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (!(x is IEnumerable<T> left) || !(y is IEnumerable<T> right))
            {
                throw new ArgumentException();
            }

            bool equals = this.Equals(left, right);
            return equals;
        }

        int IEqualityComparer.GetHashCode(
            object obj)
        {
            if (obj is null)
            {
                return 0;
            }

            if (!(obj is IEnumerable<T> e))
            {
                throw new ArgumentException();
            }

            int hashCode = this.GetHashCode(e);
            return hashCode;
        }
    }
}
