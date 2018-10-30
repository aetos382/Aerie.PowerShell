using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    [TestClass]
    public class DynamicParameterContextTest
    {
        [TestMethod]
        public void Construct()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new DynamicParameterContext(null, new TestCmdlet()));

            Assert.ThrowsException<ArgumentNullException>(
                () => new DynamicParameterContext(typeof(TestCmdlet), null));

            Assert.ThrowsException<ArgumentException>(
                () => new DynamicParameterContext(typeof(TestCmdlet), new TestCmdlet2()));

            var context = new DynamicParameterContext(typeof(TestCmdlet), new TestCmdlet());
            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void GetContext()
        {
            var cmdletInstance = new TestCmdlet();

            var context = DynamicParameterContext.GetContext(cmdletInstance);
            Assert.IsNotNull(context);

            var context2 = DynamicParameterContext.GetContext(cmdletInstance);
            Assert.AreSame(context, context2);

            var otherCmdletInstance = new TestCmdlet();

            var context3 = DynamicParameterContext.GetContext(otherCmdletInstance);
            Assert.AreNotSame(context, context3);
        }

        private class TestCmdlet : Cmdlet
        {
        }

        private class TestCmdlet2 : Cmdlet
        {
        }
    }
}
