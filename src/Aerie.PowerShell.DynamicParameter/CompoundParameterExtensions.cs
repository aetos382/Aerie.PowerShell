using System;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class CompoundParameterExtensions
    {
        [NotNull]
        public static DynamicParameter EnableDynamicParameter<T>(
            [NotNull] this T parameter,
            [NotNull] string parameterExpression)
            where T : ICompoundParameter
        {
            if ((ICompoundParameter)parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var context = CompoundParameterContext.GetContext(parameter);
            var members = new ParameterMemberInfo(typeof(T), parameterExpression);

            var instance = context.EnableParameter(members);
            return instance;
        }

        [NotNull]
        public static DynamicParameter EnableDynamicParameter<T, TParameter>(
            [NotNull] this T parameter,
            [NotNull] Expression<Func<T, TParameter>> parameterExpression)
            where T : ICompoundParameter
        {
            if ((ICompoundParameter)parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var context = CompoundParameterContext.GetContext(parameter);
            var members = new ParameterMemberInfo((MemberExpression)parameterExpression.Body);

            var instance = context.EnableParameter(members);
            return instance;
        }

/*
        public static void DisableDynamicParameter<T>(
            [NotNull] this T parameter,
            [NotNull] string parameterExpression)
            where T : ICompoundParameter
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            throw new NotImplementedException();
        }

        public static void DisableDynamicParameter<T, TParameter>(
            [NotNull] this T parameter,
            [NotNull] Expression<Func<T, TParameter>> paramaeterExpression)
            where T : ICompoundParameter
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (paramaeterExpression is null)
            {
                throw new ArgumentNullException(nameof(paramaeterExpression));
            }

            throw new NotImplementedException();
        }
*/
    }
}
