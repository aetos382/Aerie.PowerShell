using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal static class Utilities
    {
        [NotNull]
        [ItemNotNull]
        [Pure]
        public static MemberInfo[] ParseExpression(
            [NotNull] Type parentType,
            [NotNull] string expressionString)
        {
            if (parentType == null)
            {
                throw new ArgumentNullException(nameof(parentType));
            }

            if (expressionString == null)
            {
                throw new ArgumentNullException(nameof(expressionString));
            }

            var expressionSegments = expressionString.Split('.');
            var members = new List<MemberInfo>(expressionSegments.Length);

            var type = parentType;

            foreach (string segment in expressionSegments)
            {
                var member = GetParameterMember(type, segment);
                members.Add(member);

                type = GetMemberType(member);
            }

            return members.ToArray();
        }
        
        [NotNull]
        [ItemNotNull]
        [Pure]
        public static MemberInfo[] ParseExpression(
            [NotNull] MemberExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var members = new List<MemberInfo>();

            for (Expression e = expression; e is MemberExpression m; e = m.Expression)
            {
                members.Add(m.Member);
            }

            members.Reverse();

            return members.ToArray();
        }

        [NotNull]
        [Pure]
        public static MemberInfo GetParameterMember(
            [NotNull] Type type,
            [NotNull] string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var members = type.GetMember(
                name,
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Public | BindingFlags.Instance);

            if (members.Length == 0)
            {
                // no member
                throw new ArgumentException();
            }

            if (members.Length > 1)
            {
                // ambiguous member
                throw new ArgumentException();
            }

            var member = members[0];

            return member;
        }

        [NotNull]
        [ItemNotNull]
        [Pure]
        public static MemberInfo[] GetParameterMembers(
            [NotNull] Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var members = type.GetMember(
                "*",
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Public | BindingFlags.Instance);

            return members;
        }

        [NotNull]
        [Pure]
        public static Type GetMemberType(
            [NotNull] MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (member is PropertyInfo p)
            {
                return p.PropertyType;
            }

            if (member is FieldInfo f)
            {
                return f.FieldType;
            }

            throw new ArgumentException();
        }

        [CanBeNull]
        public static object GetMemberValue(
            [NotNull] object target,
            [NotNull] MemberInfo member)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (member is PropertyInfo p)
            {
                return p.GetValue(target);
            }

            if (member is FieldInfo f)
            {
                return f.GetValue(target);
            }

            throw new ArgumentException();
        }
    }
}
