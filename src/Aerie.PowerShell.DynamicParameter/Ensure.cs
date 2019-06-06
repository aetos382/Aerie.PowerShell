using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal static class Ensure
    {
        [AssertionMethod]
        public static void ArgumentNotNull<T>(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            [NotNull] T value,
            [NotNull] [InvokerParameterName] string parameterName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [AssertionMethod]
        public static void ArgumentNotNull<T>(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            [NotNull][ItemNotNull] T[] values,
            [NotNull][InvokerParameterName] string parameterName,
            bool checkItem)
            where T : class
        {
            if (values is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (checkItem)
            {
                for (int i = 0; i < values.Length; ++i)
                {
                    if (values[i] is null)
                    {
                        throw new ArgumentNullException($"{nameof(values)}[{i}]");
                    }
                }
            }
        }

        [AssertionMethod]
        public static void ArgumentNotNull<T>(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            [NotNull][ItemNotNull] IList<T> values,
            [NotNull][InvokerParameterName] string parameterName,
            bool checkItem)
            where T : class
        {
            if (values is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (checkItem)
            {
                for (int i = 0; i < values.Count; ++i)
                {
                    if (values[i] is null)
                    {
                        throw new ArgumentNullException($"{nameof(values)}[{i}]");
                    }
                }
            }
        }

        [AssertionMethod]
        public static void ArgumentNotNull<T>(
            [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
            [NotNull][ItemNotNull] IReadOnlyList<T> values,
            [NotNull][InvokerParameterName] string parameterName,
            bool checkItem)
            where T : class
        {
            if (values is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (checkItem)
            {
                for (int i = 0; i < values.Count; ++i)
                {
                    if (values[i] is null)
                    {
                        throw new ArgumentNullException($"{nameof(values)}[{i}]");
                    }
                }
            }
        }
    }
}
