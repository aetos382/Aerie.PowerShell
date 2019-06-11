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
            [NotNull] MemberInfo firstMember,
            [NotNull][ItemNotNull] params MemberInfo[] restMembers)
            : this(BeginWith(firstMember, restMembers))
        {
        }

        public ParameterMemberInfo(
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members)
        {
            Ensure.ArgumentNotNull(members, nameof(members));

            if (members.Count == 0)
            {
                throw new ArgumentException("The collection is empty", nameof(members));
            }

            var targetParameter = Expression.Parameter(typeof(object));

            Expression pathExpression =
                Expression.Convert(targetParameter, members[0].DeclaringType);
            
            var names = new List<string>(members.Count);

            Type tailMemberType = null;

            for (int i = 0; i < members.Count; ++i)
            {
                var member = members[i];

                switch (member)
                {
                    case PropertyInfo p:
                        pathExpression = Expression.Property(pathExpression, p);
                        tailMemberType = p.PropertyType;
                        break;

                    case FieldInfo f:
                        pathExpression = Expression.Field(pathExpression, f);
                        tailMemberType = f.FieldType;
                        break;

                    default:
                        throw new ArgumentException();
                }

                this._members.Add(member);
                names.Add(member.Name);
            }

            Debug.Assert(!(tailMemberType is null));

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
                            tailMemberType)),
                    targetParameter,
                    valueParameter);

            this._setValue = setValueExpression.Compile();

            this._head = members[0];
            this._tail = members.Last();
 
            this.Path = string.Join(".", names);
            this.Type = tailMemberType;
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
        private static IReadOnlyList<MemberInfo> ExpressionToMemberInfoList(
            [NotNull] MemberExpression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var members = new List<MemberInfo>();

            while (true)
            {
                members.Add(expression.Member);

                if (!(expression.Expression is MemberExpression m))
                {
                    break;
                }

                expression = m;
            }

            members.Reverse();

            return members.ToArray();
        }

        [NotNull]
        [ItemNotNull]
        private static IReadOnlyList<MemberInfo> ExpressionToMemberInfoList(
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
            var members = new List<MemberInfo>();

            foreach (var name in memberNames)
            {
                var member = declaringType.GetMember(
                    name,
                    MemberTypes.Property | MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.Instance);

                members.Add(member[0]);
            }

            return members.ToArray();
        }

        private delegate object GetValueAccessor(object target);

        [NotNull]
        private readonly GetValueAccessor _getValue;

        public object GetValue(
            object target)
        {
            return this._getValue(target);
        }

        private delegate void SetValueAccessor(object target, object value);

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
        private readonly List<MemberInfo> _members = new List<MemberInfo>();

        [NotNull]
        private readonly MemberInfo _head;

        [NotNull]
        private readonly MemberInfo _tail;

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

        [NotNull]
        public string Path { get; }

        [NotNull]
        public Type Type { get; }

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
                hashCode.Add(member);
            }

            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return $"{this.DeclaringType.Name}.{this.Path}";
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return this._tail.CustomAttributes;
            }
        }
        
        public override Type DeclaringType
        {
            get
            {
                return this._head.DeclaringType;
            }
        }

        public override object[] GetCustomAttributes(
            Type attributeType,
            bool inherit)
        {
            return this._tail.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(
            bool inherit)
        {
            return this._tail.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return this._tail.GetCustomAttributesData();
        }

        public override bool HasSameMetadataDefinitionAs(
            MemberInfo other)
        {
            return this._tail.HasSameMetadataDefinitionAs(other);
        }

        public override bool IsDefined(
            Type attributeType,
            bool inherit)
        {
            return this._tail.IsDefined(attributeType, inherit);
        }

        public override MemberTypes MemberType
        {
            get
            {
                return this._tail.MemberType;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return this._tail.MetadataToken;
            }
        }
        
        public override Module Module
        {
            get
            {
                return this._tail.Module;
            }
        }
        
        public override string Name
        {
            get
            {
                return this._tail.Name;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                return this._head.ReflectedType;
            }
        }

        private static IReadOnlyList<T> BeginWith<T>(
            [CanBeNull] T first,
            [NotNull] T[] rest)
        {
            var list = new List<T>(rest.Length + 1);

            list.Add(first);
            list.AddRange(rest);

            return list;
        }
    }
}
