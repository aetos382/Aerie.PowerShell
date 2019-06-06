using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class ReflectParameterDescriptor :
        DynamicParameterDescriptor
    {
        [NotNull]
        private static readonly Dictionary<IReadOnlyList<MemberInfo>, ReflectParameterDescriptor> _descriptorCache =
            new Dictionary<IReadOnlyList<MemberInfo>, ReflectParameterDescriptor>(CollectionEqualityComparer<MemberInfo>.Default);

        [NotNull]
        internal static ReflectParameterDescriptor GetParameterDescriptor(
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members)
        {
            if (!_descriptorCache.TryGetValue(members, out var descriptor))
            {
                var initializationInfo = new ReflectParameterDescriptorInitializationInfo(members);
                _descriptorCache[members] = descriptor = new ReflectParameterDescriptor(initializationInfo);
            }

            return descriptor;
        }

        private ReflectParameterDescriptor(
            [NotNull] ReflectParameterDescriptorInitializationInfo initializationInfo)
            : base(
                initializationInfo.ParameterName,
                initializationInfo.ParameterType)
        {
            var members = initializationInfo.Members.ToArray();

            var attributesData = members.Last().GetCustomAttributesData();

            foreach (var a in attributesData)
            {
                if (a.AttributeType == typeof(DynamicParameterAttribute))
                {
                    this.Attributes.Add(new ParameterAttributeData(a));
                }
                else if (!Attribute.IsDefined(a.AttributeType, typeof(DynamicParameterInternalAttribute)))
                {
                    this.Attributes.Add(a);
                }
            }

            this.Members = members;

            CreateAccessors(members, out var getAccessor, out var setAccessor);

            this._getValueAccessor = getAccessor;
            this._setValueAccessor = setAccessor;
        }

        [NotNull]
        [ItemNotNull]
        internal IReadOnlyList<MemberInfo> Members { [Pure] get; }
        
        protected internal override object GetParameterValue(
            IDynamicParameterContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var value = this._getValueAccessor(context.Cmdlet);
            return value;
        }

        protected internal override void SetParameterValue(
            IDynamicParameterContext context,
            object value)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this._setValueAccessor(context.Cmdlet, value);
        }

        public override bool Equals(DynamicParameterDescriptor other)
        {
            if (!base.Equals(other))
            {
                return false;
            }

            Debug.Assert(!(other is null));

            if (!((ReflectParameterDescriptor)other).Members.SequenceEqual(this.Members))
            {
                return false;
            }

            return true;
        }

        protected override void GetHashCode(HashCode hashCode)
        {
            foreach (var member in this.Members)
            {
                hashCode.Add(member);
            }
        }

        [NotNull]
        private readonly Func<Cmdlet, object> _getValueAccessor;
        
        [CanBeNull]
        private readonly Action<Cmdlet, object> _setValueAccessor;

        private static void CreateAccessors(
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members,
            [NotNull] out Func<Cmdlet, object> getAccessor,
            [NotNull] out Action<Cmdlet, object> setAccessor)
        {
            if (members is null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            var cmdletParameter = Expression.Parameter(typeof(Cmdlet));
            var cmdletType = members[0].DeclaringType;

            Expression expression = Expression.Convert(cmdletParameter, cmdletType);

            foreach (var member in members)
            {
                expression = Expression.PropertyOrField(expression, member.Name);
            }

            var getterLambda = Expression.Lambda<Func<Cmdlet, object>>(
                Expression.Convert(
                    expression,
                    typeof(object)),
                cmdletParameter);

            getAccessor = getterLambda.Compile();

            var lastMember = members.Last();

            var valueType = Utilities.GetMemberType(lastMember);

            var valueParameter = Expression.Parameter(typeof(object));

            var setterLambda = Expression.Lambda<Action<Cmdlet, object>>(
                Expression.Assign(
                    expression,
                    Expression.Convert(
                        valueParameter,
                        valueType)),
                cmdletParameter, valueParameter);

            setAccessor = setterLambda.Compile();
        }

        private class ReflectParameterDescriptorInitializationInfo
        {
            public ReflectParameterDescriptorInitializationInfo(
                [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members)
            {
                var leafMember = members.Last();
                var memberType = Utilities.GetMemberType(leafMember);

                var nameAttribute = leafMember.GetCustomAttribute<DynamicParameterNameAttribute>();
                this.ParameterName = nameAttribute?.ParameterName ?? leafMember.Name;

                this.Members = members.ToArray();
                this.ParameterType = memberType;
            }

            [NotNull]
            public string ParameterName { [Pure] get; }

            [NotNull]
            public Type ParameterType { [Pure] get; }

            [NotNull]
            [ItemNotNull]
            public IReadOnlyList<MemberInfo> Members { [Pure] get; }
        }
    }
}
