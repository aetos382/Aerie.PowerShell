using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    [TestClass]
    public class ListExtensionsTest
    {
        [TestMethod]
        public void LastTest()
        {
            var source = new[] { 1, 3, 5, 7, 9 };

            var last = ListExtensions.Last(source);

            Assert.AreEqual(9, last);
        }

        [TestMethod]
        public void LastItemNotFoundTest()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                ListExtensions.Last(Array.Empty<GenericParameterHelper>()));
        }
    }
}
