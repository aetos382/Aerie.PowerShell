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
    }
}
