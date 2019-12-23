using System;
using System.Linq.Expressions;

using Microsoft;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class CmdletContextBuilderExtensions
    {
        public static ICmdletContextBuilder FromType(
            this ICmdletContextBuilder builder,
            Type cmdletType)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNull(cmdletType, nameof(cmdletType));

            // TODO: Implement
            return builder;
        }

        public static ICmdletContextBuilder AddMember(
            this ICmdletContextBuilder builder,
            Type cmdletType,
            string stringExpression)
        {
            throw new NotImplementedException();
        }

        public static ICmdletContextBuilder AddMember<TCmdlet, TMember>(
            this ICmdletContextBuilder builder,
            Expression<Func<TCmdlet, TMember>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
