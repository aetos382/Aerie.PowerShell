using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class ReflectParameterDescriptor :
        DynamicParameterDescriptor
    {
        [NotNull]
        private static readonly Dictionary<ParameterMemberInfo, ReflectParameterDescriptor> _descriptorCache =
            new Dictionary<ParameterMemberInfo, ReflectParameterDescriptor>();

        [NotNull]
        internal static ReflectParameterDescriptor GetDescriptor(
            [NotNull][ItemNotNull] ParameterMemberInfo members)
        {
            if (members is null)
            {
                throw new ArgumentNullException(nameof(members));
            }

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
            var attributesData = initializationInfo.Member.GetCustomAttributesData();

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

            this.Member = initializationInfo.Member;
        }

        [NotNull]
        [ItemNotNull]
        internal ParameterMemberInfo Member { [Pure] get; }
        
        protected internal override object GetParameterValue(
            IDynamicParameterContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var value = this.Member.GetValue(context.Cmdlet);
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

            this.Member.SetValue(context.Cmdlet, value);
        }

        /*
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
        */

        private class ReflectParameterDescriptorInitializationInfo
        {
            public ReflectParameterDescriptorInitializationInfo(
                [NotNull][ItemNotNull] ParameterMemberInfo member)
            {
                var nameAttribute = member.GetCustomAttribute<DynamicParameterNameAttribute>();
                this.ParameterName = nameAttribute?.ParameterName ?? member.Name;

                this.Member = member;
                this.ParameterType = member.Type;
            }

            [NotNull]
            public string ParameterName { [Pure] get; }

            [NotNull]
            public Type ParameterType { [Pure] get; }

            [NotNull]
            [ItemNotNull]
            public ParameterMemberInfo Member { [Pure] get; }
        }
    }
}
