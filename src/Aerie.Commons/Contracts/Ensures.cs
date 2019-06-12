using System;

using JetBrains.Annotations;

namespace Aerie.Commons.Contracts
{
    public static class Ensures
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
