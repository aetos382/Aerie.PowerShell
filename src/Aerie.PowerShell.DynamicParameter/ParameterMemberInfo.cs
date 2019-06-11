using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class ParameterMemberInfo :
        MemberInfo,
        IReadOnlyList<MemberInfo>,
        IEquatable<ParameterMemberInfo>
    {
        public ParameterMemberInfo(
            params MemberInfo[] members)
            : this(MemberInfoToMemberInfoWrapper(members))
        {
        }

        public ParameterMemberInfo(
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members)
            : this(MemberInfoToMemberInfoWrapper(members))
        {
        }

        private ParameterMemberInfo(
            [NotNull][ItemNotNull] MemberInfoWrapper[] members)
        {
            Ensure.ArgumentNotNull(members, nameof(members));

            if (members.Length == 0)
            {
                throw new ArgumentException("The collection is empty", nameof(members));
            }

            var names = new List<string>(members.Length);

            var targetParameter = Expression.Parameter(typeof(object));

            Expression pathExpression = Expression.Convert(targetParameter, members[0].DeclaringType);

            MemberInfoWrapper lastMember = null;

            for (int i = 0; i < members.Length; ++i)
            {
                var member = MemberInfoWrapper.Create(members[i]);

                if (i > 0)
                {
                    if (member is null)
                    {
                        throw new ArgumentNullException($"{nameof(members)}[{i}]");
                    }

                    Debug.Assert(!(lastMember is null));

                    EnsureChained(lastMember, member);
                }

                this._members.Add(member);
                names.Add(member.Name);

                pathExpression = Expression.PropertyOrField(pathExpression, member.Name);

                lastMember = member;
            }

            this.Path = string.Join(".", names);

            var getValueExpression =
                Expression.Lambda<GetValueAccessor>(
                    Expression.Convert(
                        pathExpression,
                        typeof(object)),
                    targetParameter);

            this._getValue = getValueExpression.Compile();

            var valueParameter = Expression.Parameter(typeof(object));

            var setValueExpression =
                Expression.Lambda<SetValueAccessor>(
                    Expression.Assign(
                        pathExpression,
                        Expression.Convert(
                            valueParameter,
                            lastMember.Type)),
                    targetParameter,
                    valueParameter);

            this._setValue = setValueExpression.Compile();
        }

        public ParameterMemberInfo(
            [NotNull] MemberExpression expression)
            : this(ExpressionToMemberInfoList(expression))
        {
        }

        public ParameterMemberInfo(
            [NotNull] Type declaringType,
            [NotNull] string expression)
            : this(ExpressionToMemberInfoList(declaringType, expression))
        {
        }

        [NotNull]
        [ItemNotNull]
        private static MemberInfoWrapper[] MemberInfoToMemberInfoWrapper(
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members)
        {
            return members.Select(MemberInfoWrapper.Create).ToArray();
        }

        private static MemberInfoWrapper[] ExpressionToMemberInfoList(
            [NotNull] MemberExpression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var members = new List<MemberInfoWrapper>();

            while (true)
            {
                var member = MemberInfoWrapper.Create(expression.Member);
                members.Add(member);

                if (!(expression.Expression is MemberExpression m))
                {
                    break;
                }

                expression = m;
            }

            members.Reverse();

            return members.ToArray();
        }

        private static MemberInfoWrapper[] ExpressionToMemberInfoList(
            [NotNull] Type declaringType,
            [NotNull] string expression)
        {
            if (declaringType is null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var memberNames = expression.Split('.');
            var members = new List<MemberInfoWrapper>();

            foreach (var name in memberNames)
            {
                var member = MemberInfoWrapper.Create(declaringType.GetMember(name).Single());
                members.Add(member);
            }

            return members.ToArray();
        }

        [NotNull]
        private readonly GetValueAccessor _getValue;

        public object GetValue(
            object target)
        {
            return this._getValue(target);
        }

        [NotNull]
        private readonly SetValueAccessor _setValue;

        public void SetValue(
            object target,
            object value)
        {
            this._setValue(target, value);
        }

        [NotNull]
        [ItemNotNull]
        private readonly List<MemberInfoWrapper> _members = new List<MemberInfoWrapper>();

        IEnumerator<MemberInfo> IEnumerable<MemberInfo>.GetEnumerator()
        {
            return this._members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<MemberInfo>)this).GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this._members.Count;
            }
        }

        public MemberInfo this[int index]
        {
            get
            {
                return this._members[index];
            }
        }
        
        private static void EnsureChained(
            [NotNull] MemberInfoWrapper first,
            [NotNull] MemberInfoWrapper second)
        {
            if (!second.DeclaringType.IsAssignableFrom(first.Type))
            {
                throw new ArgumentException();
            }
        }

        public override Type DeclaringType
        {
            get
            {
                return this._members[0].DeclaringType;
            }
        }

        public string Path { get; }

        public override Type ReflectedType
        {
            get
            {
                return this._members[0].ReflectedType;
            }
        }
        
        public bool Equals(
            ParameterMemberInfo other)
        {
            if (other is null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this._members.SequenceEqual(other._members);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ParameterMemberInfo c))
            {
                return false;
            }

            return this.Equals(c);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            foreach (var member in this._members)
            {
                hashCode.Add(member.BaseMemberInfo);
            }

            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return $"{this.DeclaringType.Name}.{this.Path}";
        }

        private delegate object GetValueAccessor(object target);

        private delegate void SetValueAccessor(object target, object value);
    }
}
