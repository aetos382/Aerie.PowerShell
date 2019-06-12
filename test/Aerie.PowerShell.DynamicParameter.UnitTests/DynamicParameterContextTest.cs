using System;
using System.Management.Automation;

using NUnit.Framework;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    public class DynamicParameterContextTest
    {
        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CmdletContext(null, new TestCmdlet()));

            Assert.Throws<ArgumentNullException>(
                () => new CmdletContext(typeof(TestCmdlet), null));

            Assert.Throws<ArgumentException>(
                () => new CmdletContext(typeof(TestCmdlet), new TestCmdlet2()));

            var context = new CmdletContext(typeof(TestCmdlet), new TestCmdlet());
            Assert.IsNotNull(context);
        }

        [Test]
        public void GetContext()
        {
            var cmdletInstance = new TestCmdlet();

            var context = CmdletContext.GetContext(cmdletInstance);
            Assert.IsNotNull(context);

            var context2 = CmdletContext.GetContext(cmdletInstance);
            Assert.AreSame(context, context2);

            var otherCmdletInstance = new TestCmdlet();

            var context3 = CmdletContext.GetContext(otherCmdletInstance);
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
