using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    public class CollectionEqualityComparerTest
    {
        [Test]
        public void GetHashCodeTest()
        {
            var items = new[] { 1, 2, 3 };

            var collection1 = new List<int>(items);
            var collection2 = new List<int>(items);

            Assert.AreNotSame(collection1, collection2);

            var comparer = CollectionEqualityComparer<int>.Default;

            int hashCode1 = comparer.GetHashCode(collection1);
            int hashCode2 = comparer.GetHashCode(collection2);

            Assert.AreEqual(hashCode1, hashCode2);
        }

        [Test]
        public void GetHashCodeAllowsNullTest()
        {
            int hashCode = CollectionEqualityComparer<object>.Default.GetHashCode(null);
            Assert.AreEqual(0, hashCode);
        }

        [Test]
        public void EqualsTest()
        {
            var items = new[] { 1, 2, 3 };

            var collection1 = new List<int>(items);
            var collection2 = new List<int>(items);

            Assert.AreNotSame(collection1, collection2);

            var comparer = CollectionEqualityComparer<int>.Default;

            bool equals = comparer.Equals(collection1, collection2);

            Assert.IsTrue(equals);
        }

        [Test]
        public void NotEqualsTest()
        {
            var collection1 = new[] { 1, 2, 3 };
            var collection2 = new[] { 1, 2, 4 };

            var comparer = CollectionEqualityComparer<int>.Default;

            bool equals = comparer.Equals(collection1, collection2);

            Assert.IsFalse(equals);
        }

        [Test]
        public void EqualsAllowsNullTest()
        {
            var comparer = CollectionEqualityComparer<int>.Default;

            bool equals = comparer.Equals(null, null);

            Assert.IsTrue(equals);
        }
    }
}
