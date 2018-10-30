using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public static class AttributeUtility
    {
        [NotNull]
        private static readonly Dictionary<Type, object> _valueTypeDefaults = new Dictionary<Type, object>();

        [NotNull]
        public static CustomAttributeData ExpressionToCustomAttributeData<T>(
            [NotNull] Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var attributeData = ExpressionToCustomAttributeData(expression.Body);
            return attributeData;
        }

        [NotNull]
        public static CustomAttributeData ExpressionToCustomAttributeData(
            [NotNull] Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            NewExpression newExpression;

            var namedArguments = new List<CustomAttributeNamedArgument>();

            if (expression is MemberInitExpression m)
            {
                newExpression = m.NewExpression;

                foreach (var binding in m.Bindings)
                {
                    if (!(binding is MemberAssignment assignment))
                    {
                        throw new NotSupportedException();
                    }

                    var value = ExpressionToTypedArgument(assignment.Expression);

                    var namedArgument = new CustomAttributeNamedArgument(
                        binding.Member,
                        value);

                    namedArguments.Add(namedArgument);
                }
            }
            else if (expression is NewExpression n)
            {
                newExpression = n;
            }
            else
            {
                throw new ArgumentException("lambda body must be a NewExpression or MemberInitExpression.");
            }

            var constructorArguments = new List<CustomAttributeTypedArgument>();

            foreach (var argumentExpression in newExpression.Arguments)
            {
                var constructorArgument = ExpressionToTypedArgument(argumentExpression);
                constructorArguments.Add(constructorArgument);
            }

            var data = new ModifiableCustomAttributeData(
                newExpression.Constructor,
                constructorArguments,
                namedArguments);

            return data;
        }

        private static CustomAttributeTypedArgument ExpressionToTypedArgument(
            [NotNull] Expression expression)
        {
            CustomAttributeTypedArgument value;

            if (expression is ConstantExpression constant)
            {
                value = new CustomAttributeTypedArgument(constant.Type, constant.Value);
            }
            else if (expression is NewArrayExpression newArray)
            {
                int count = newArray.Expressions.Count;

                var arrayValue = Array.CreateInstance(newArray.Type.GetElementType(), count);

                for (int i = 0; i < count; ++i)
                {
                    var element = newArray.Expressions[i];

                    if (!(element is ConstantExpression constantElement))
                    {
                        throw new Exception();
                    }

                    arrayValue.SetValue(constantElement.Value, i);
                }

                value = new CustomAttributeTypedArgument(newArray.Type, arrayValue);
            }
            else if (expression is DefaultExpression d)
            {
                var type = d.Type;

                object defaultValue = null;

                if (type.IsValueType)
                {
                    if (!_valueTypeDefaults.TryGetValue(type, out defaultValue))
                    {
                        _valueTypeDefaults[type] = defaultValue = Activator.CreateInstance(type);
                    }
                }

                value = new CustomAttributeTypedArgument(type, defaultValue);
            }
            else
            {
                throw new NotSupportedException();
            }

            return value;
        }
    }
}
