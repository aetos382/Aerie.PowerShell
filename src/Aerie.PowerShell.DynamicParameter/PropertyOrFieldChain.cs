using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class PropertyOrFieldChain :
        PropertyOrFieldInfoBase,
        IReadOnlyList<PropertyOrFieldInfo>,
        IEquatable<PropertyOrFieldChain>
    {
        public PropertyOrFieldChain(
            params PropertyOrFieldInfo[] members)
            : this((IReadOnlyList<PropertyOrFieldInfo>)members)
        {
        }

        public PropertyOrFieldChain(
            [NotNull][ItemNotNull] IReadOnlyList<PropertyOrFieldInfo> members)
        {
            Ensure.ArgumentNotNull(members, nameof(members));

            if (members.Count == 0)
            {
                throw new ArgumentException("The collection is empty", nameof(members));
            }

            var firstMember = members[0];
            if (firstMember is null)
            {
                throw new ArgumentNullException($"{nameof(members)}[0]");
            }

            var names = new List<string>(members.Count);

            var targetParameter = Expression.Parameter(typeof(object));

            Expression pathExpression = Expression.Convert(targetParameter, firstMember.DeclaringType);

            PropertyOrFieldInfo lastMember = null;

            for (int i = 0; i < members.Count; ++i)
            {
                var member = members[i];

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

            this._getValueAccessor = getValueExpression.Compile();

            var valueParameter = Expression.Parameter(typeof(object));

            var setValueExpression =
                Expression.Lambda<SetValueAccessor>(
                    Expression.Assign(
                        pathExpression,
                        Expression.Convert(
                            valueParameter,
                            lastMember.PropertyOrFieldType)),
                    targetParameter,
                    valueParameter);

            this._setValueAccessor = setValueExpression.Compile();

            base.InitializeMemberInfo(lastMember);
        }
        
        public PropertyOrFieldChain(
            [NotNull] MemberExpression expression)
            : this(ExpressionToMemberInfoList(expression))
        {
        }

        public PropertyOrFieldChain(
            [NotNull] Type declaringType,
            [NotNull] string expression)
            : this(ExpressionToMemberInfoList(declaringType, expression))
        {
        }

        private static IReadOnlyList<PropertyOrFieldInfo> ExpressionToMemberInfoList(
            [NotNull] MemberExpression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var members = new List<PropertyOrFieldInfo>();

            while (true)
            {
                members.Add(new PropertyOrFieldInfo(expression.Member));

                if (!(expression.Expression is MemberExpression m))
                {
                    break;
                }

                expression = m;
            }

            members.Reverse();

            return members;
        }

        private static IReadOnlyList<PropertyOrFieldInfo> ExpressionToMemberInfoList(
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
            var members = new List<PropertyOrFieldInfo>();

            foreach (var name in memberNames)
            {
                var member = declaringType.GetMember(name);

                members.Add(new PropertyOrFieldInfo(member[0]));
            }

            return members;
        }

        [NotNull]
        [ItemNotNull]
        private readonly List<PropertyOrFieldInfo> _members = new List<PropertyOrFieldInfo>();

        IEnumerator<PropertyOrFieldInfo> IEnumerable<PropertyOrFieldInfo>.GetEnumerator()
        {
            return this._members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<PropertyOrFieldInfo>)this).GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this._members.Count;
            }
        }

        public PropertyOrFieldInfo this[int index]
        {
            get
            {
                return this._members[index];
            }
        }
        
        private static void EnsureChained(
            [NotNull] PropertyOrFieldInfo first,
            [NotNull] PropertyOrFieldInfo second)
        {
            if (!second.DeclaringType.IsAssignableFrom(first.PropertyOrFieldType))
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
            PropertyOrFieldChain other)
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
            if (!(obj is PropertyOrFieldChain c))
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

        [NotNull]
        private readonly GetValueAccessor _getValueAccessor;

        [NotNull]
        private readonly SetValueAccessor _setValueAccessor;

        protected override GetValueAccessor CreateGetValueAccessor()
        {
            return this._getValueAccessor;
        }

        protected override SetValueAccessor CreateSetValueAccessor()
        {
            return this._setValueAccessor;
        }
    }
}
