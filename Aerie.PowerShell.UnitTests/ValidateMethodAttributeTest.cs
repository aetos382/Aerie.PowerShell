using System;
using System.Management.Automation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aerie.PowerShell.UnitTests
{
    [TestClass]
    public class ValidateMethodAttributeTest
    {
        [TestMethod]
        public void ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ValidateMethodAttribute(
                    null, "FooBar");
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new ValidateMethodAttribute(
                    typeof(ValidateMethodAttributeTest), null);
            });
        }

        [TestMethod]
        public void MethodNotFoundTest()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                // not found
                new ValidateMethodAttribute(
                    typeof(ValidateMethodAttributeTest), "NoSuchMethod");
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // non-static
                new ValidateMethodAttribute(
                    typeof(ValidateMethodAttributeTest), nameof(this.InvalidMethod1));
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // return type is not void
                new ValidateMethodAttribute(
                    typeof(ValidateMethodAttributeTest), nameof(this.InvalidMethod2));
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // not single parameter
                new ValidateMethodAttribute(
                    typeof(ValidateMethodAttributeTest), nameof(this.InvalidMethod3));
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // ambiguous
                new ValidateMethodAttribute(
                    typeof(ValidateMethodAttributeTest), nameof(this.InvalidMethod4));
            });
        }

        [TestMethod]
        public void InvalidArgumentType()
        {
            var validator = new ValidateMethodAttribute(
                typeof(ValidateMethodAttributeTest), nameof(this.Validate));

            Assert.ThrowsException<InvalidCastException>(() =>
            {
                // 1 can't cast to TestClass
                validator.InternalValidateElement(1);
            });
        }

        [TestMethod]
        public void OK()
        {
            var validator = new ValidateMethodAttribute(
                typeof(ValidateMethodAttributeTest), nameof(this.Validate));

            var value = new TestClass { Value = 1 };

            validator.InternalValidateElement(value);

            Assert.IsTrue(value.Called);
        }
 
        [TestMethod]
        public void OK2()
        {
            var validator = new ValidateMethodAttribute(
                typeof(ValidateMethodAttributeTest), nameof(this.Validate2));

            var value = new TestClass { Value = 1 };

            validator.InternalValidateElement(value);

            Assert.IsTrue(value.Called);
        }
   
        [TestMethod]
        public void OK3()
        {
            var validator = new ValidateMethodAttribute(
                typeof(ValidateMethodAttributeTest), nameof(this.Validate));

            validator.InternalValidateElement(null);
        }
        
        [TestMethod]
        public void OK4()
        {
            var validator = new ValidateMethodAttribute(
                typeof(ValidateMethodAttributeTest), nameof(this.Validate));

            var value = new TestClass { Value = 1 };

            validator.InternalValidateElement(PSObject.AsPSObject(value));

            Assert.IsTrue(value.Called);
        }

        [TestMethod]
        public void NG()
        {
            var validator = new ValidateMethodAttribute(
                typeof(ValidateMethodAttributeTest), nameof(this.Validate));

            var value = new TestClass { Value = -1 };

            Assert.ThrowsException<ValidationMetadataException>(() =>
            {
                validator.InternalValidateElement(value);
            });

            Assert.IsTrue(value.Called);
        }

        private static void Validate(
            TestClass test)
        {
            if (test is null)
            {
                return;
            }

            test.Called = true;

            if (test.Value < 0)
            {
                throw new ValidationMetadataException($"Value ({test.Value}) is negative.");
            }
        }
        
        private static void Validate2(
            object test)
        {
            Validate((TestClass)test);
        }

        private class TestClass
        {
            public int Value { get; set; }

            public bool Called { get; set; }
        }

        // non-static
        private bool InvalidMethod1(
            TestClass test)
        {
            return false;
        }

        // return type is not void
        private static int InvalidMethod2(
            TestClass test)
        {
            return 0;
        }

        // not single parameter
        private static void InvalidMethod3(
            TestClass test,
            TestClass test2)
        {
        }

        // ambiguous
        private static void InvalidMethod4(
            TestClass test)
        {
        }

        // ambiguous
        private static void InvalidMethod4(
            object test)
        {
        }
    }
}
