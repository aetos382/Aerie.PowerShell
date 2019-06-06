﻿using System;

using NUnit.Framework;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    public class ListExtensionsTest
    {
        [Test]
        public void LastTest()
        {
            var source = new[] { 1, 3, 5, 7, 9 };

            var last = ListExtensions.Last(source);

            Assert.AreEqual(9, last);
        }

        [Test]
        public void LastItemNotFoundTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
                ListExtensions.Last(Array.Empty<object>()));
        }
    }
}
