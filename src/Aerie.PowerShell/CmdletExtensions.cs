﻿using System;
using System.Linq.Expressions;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public static class CmdletExtensions
    {
        [Pure]
        public static bool HasParameter(
            [NotNull] this PSCmdlet cmdlet,
            [NotNull] string name)
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return cmdlet.MyInvocation.BoundParameters.ContainsKey(name);
        }

        [Pure]
        public static bool HasParameter<TCmdlet, TProperty>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] Expression<Func<TCmdlet, TProperty>> expression)
            where TCmdlet : PSCmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string name = GetMemberName(cmdlet, expression);
            return HasParameter(cmdlet, name);
        }

        [Pure]
        private static string GetMemberName<TObject, TProperty>(
            [NotNull] TObject obj,
            [NotNull] Expression<Func<TObject, TProperty>> expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            string name = ((MemberExpression)expression.Body).Member.Name;
            return name;
        }
    }
}
