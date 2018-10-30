using System;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aerie.PowerShell.DynamicParameter.UnitTests
{
    [TestClass]
    public class ExpressionToCustomAttributeDataTest
    {
        [TestMethod]
        public void NoParameterTest()
        {
            Expression<Func<Foo>> foo = () => new Foo();

            var attributeData = AttributeUtility.ExpressionToCustomAttributeData(foo.Body);

            Assert.AreEqual(typeof(Foo), attributeData.AttributeType);
            Assert.AreEqual(0, attributeData.ConstructorArguments.Count);
            Assert.AreEqual(0, attributeData.NamedArguments.Count);
        }

        [TestMethod]
        public void HasParametersTest()
        {
            Expression<Func<Foo>> foo = () => new Foo(1, 2)
            {
                i = 10,
                array = new [] { 3, 4 },
                T = typeof(object),
                S = default
            };

            var attributeData = AttributeUtility.ExpressionToCustomAttributeData(foo.Body);
            var constructorArguments = attributeData.ConstructorArguments;

            Assert.AreEqual(typeof(Foo), attributeData.AttributeType);

            Assert.AreEqual(1, constructorArguments[0].Value);
            Assert.AreEqual(2, constructorArguments[1].Value);

            for (int i = 0; i < 3; ++i)
            {
                var namedArgument = attributeData.NamedArguments[i];
                var value = namedArgument.TypedValue.Value;

                switch (namedArgument.MemberName)
                {
                    case nameof(Foo.i):
                        Assert.AreEqual(10, value);
                        break;

                    case nameof(Foo.array):
                        CollectionAssert.AreEqual(new[] { 3, 4 }, (int[])value);
                        break;

                    case nameof(Foo.T):
                        Assert.AreEqual(typeof(object), value);
                        break;

                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        [TestMethod]
        public void ManualExpressionTest()
        {
            var type = typeof(Foo);

            MemberInfo GetFooMember(
                string name)
            {
                const MemberTypes memberTypes = MemberTypes.Property | MemberTypes.Field;
                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

                var member = type.GetMember(name, memberTypes, bindingFlags)[0];
                return member;
            }

            var expression = Expression.MemberInit(
                Expression.New(
                    type.GetConstructor(new[]
                    {
                        typeof(int),
                        typeof(int),
                        typeof(int[])
                    }),
                    Expression.Constant(1),
                    Expression.Default(typeof(int)),
                    Expression.NewArrayInit(
                        typeof(int),
                        Expression.Constant(2),
                        Expression.Constant(3))),
                Expression.Bind(
                    GetFooMember(nameof(Foo.i)),
                    Expression.Constant(10)),
                Expression.Bind(
                    GetFooMember(nameof(Foo.array)),
                    Expression.NewArrayInit(
                        typeof(int),
                        Expression.Constant(3),
                        Expression.Constant(4))),
                Expression.Bind(
                    GetFooMember(nameof(Foo.T)),
                    Expression.Constant(typeof(object))),
                Expression.Bind(
                    GetFooMember(nameof(Foo.S)),
                    Expression.Default(typeof(string))),
                Expression.Bind(
                    GetFooMember(nameof(Foo.d)),
                    Expression.Default(typeof(int))));

            var attributeData = AttributeUtility.ExpressionToCustomAttributeData(expression);
            var constructorArguments = attributeData.ConstructorArguments;

            Assert.AreEqual(typeof(Foo), attributeData.AttributeType);

            Assert.AreEqual(1, constructorArguments[0].Value);
            Assert.AreEqual(0, constructorArguments[1].Value);
            CollectionAssert.AreEqual(new[] { 2, 3 }, (int[])constructorArguments[2].Value);

            for (int i = 0; i < 5; ++i)
            {
                var namedArgument = attributeData.NamedArguments[i];
                var value = namedArgument.TypedValue.Value;

                switch (namedArgument.MemberName)
                {
                    case nameof(Foo.i):
                        Assert.AreEqual(10, value);
                        break;

                    case nameof(Foo.array):
                        CollectionAssert.AreEqual(new[] { 3, 4 }, (int[])value);
                        break;

                    case nameof(Foo.T):
                        Assert.AreEqual(typeof(object), value);
                        break;

                    case nameof(Foo.S):
                        Assert.IsNull(value);
                        break;

                    case nameof(Foo.d):
                        Assert.AreEqual(0, value);
                        break;

                    default:
                        Assert.Fail();
                        break;
                }
            }
        }

        public class Foo
        {
            public Foo()
            {
            }

            public Foo(int x, int y, params int[] z)
            {
            }

            public int i { get; set; }

            public int[] array;

            public Type T { get; set; }

            public string S;

            public int d;
        }
    }
}
