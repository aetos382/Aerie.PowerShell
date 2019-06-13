using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class ReflectParameterDescriptor :
        ParameterDescriptor
    {
        [NotNull]
        private static readonly Dictionary<ParameterMemberInfo, ReflectParameterDescriptor> _descriptorCache =
            new Dictionary<ParameterMemberInfo, ReflectParameterDescriptor>();

        [NotNull]
        internal static ReflectParameterDescriptor GetDescriptor(
            [NotNull][ItemNotNull] ParameterMemberInfo member,
            [CanBeNull] IDynamicParameterAttributeProvider attributeProvider)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            attributeProvider = attributeProvider ?? DefaultDynamicParameterAttributeProvider.Instance;

            if (!_descriptorCache.TryGetValue(member, out var descriptor))
            {
                var initializationInfo = new ReflectParameterDescriptorInitializationInfo(member);
                _descriptorCache[member] = descriptor = new ReflectParameterDescriptor(initializationInfo, attributeProvider);
            }

            return descriptor;
        }

        private ReflectParameterDescriptor(
            [NotNull] ReflectParameterDescriptorInitializationInfo initializationInfo,
            IDynamicParameterAttributeProvider attributeProvider)
            : base(
                initializationInfo.ParameterName,
                initializationInfo.ParameterType)
        {
            var attributes = attributeProvider.GetAttributeData(initializationInfo.Member);

            foreach (var a in attributes)
            {
                this.Attributes.Add(a);
            }

            this.Member = initializationInfo.Member;
        }

        [NotNull]
        [ItemNotNull]
        internal ParameterMemberInfo Member { [Pure] get; }
        
        protected internal override object GetParameterValue(
            CmdletContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var value = this.Member.GetValue(context.Cmdlet);
            return value;
        }

        protected internal override void SetParameterValue(
            CmdletContext context,
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
