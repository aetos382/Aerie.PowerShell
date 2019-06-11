using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public abstract class DynamicParameterDescriptor :
        IEquatable<DynamicParameterDescriptor>
    {
        protected DynamicParameterDescriptor(
            [NotNull] string parameterName,
            [NotNull] Type parameterType)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (parameterType is null)
            {
                throw new ArgumentNullException(nameof(parameterType));
            }

            this.ParameterName = parameterName;
            this.ParameterType = parameterType;

            this.Attributes = new List<CustomAttributeData>();

            this.Id = GetId();
        }

        [NotNull]
        public string ParameterName { [Pure] get; }

        [NotNull]
        public Type ParameterType { [Pure] get; }

        [NotNull]
        [ItemNotNull]
        public IList<CustomAttributeData> Attributes { [Pure] get; }

        [CanBeNull]
        protected internal abstract object GetParameterValue(
            [NotNull] IDynamicParameterContext context);

        protected internal abstract void SetParameterValue(
            [NotNull] IDynamicParameterContext context,
            [CanBeNull] object value);

        internal int Id { [Pure] get; }

        private static int _currentId = 0;

        private static int GetId()
        {
            return Interlocked.Increment(ref _currentId);
        }

        public virtual bool Equals(
            DynamicParameterDescriptor other)
        {
            if (other is null)
            {
                return false;
            }

            if (object.ReferenceEquals(other, this))
            {
                return true;
            }

            if (other.GetType() != this.GetType())
            {
                return false;
            }

            // TODO: ParameterName は大文字と小文字を区別する？
            if (!string.Equals(other.ParameterName, this.ParameterName))
            {
                return false;
            }

            if (other.ParameterType != this.ParameterType)
            {
                return false;
            }

            if (!other.Attributes.SequenceEqual(this.Attributes))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(
            object obj)
        {
            if (!(obj is DynamicParameterDescriptor descriptor))
            {
                return false;
            }

            return this.Equals(descriptor);
        }

        protected virtual void GetHashCode(HashCode hashCode)
        {
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            // TODO: 大文字と小文字
            hashCode.Add(this.ParameterName);
            hashCode.Add(this.ParameterType);

            foreach (var attribute in this.Attributes)
            {
                hashCode.Add(attribute);
            }

            this.GetHashCode(hashCode);

            return hashCode.ToHashCode();
        }

        [NotNull]
        public static DynamicParameterDescriptor GetDynamicParameterDescriptor(
            [NotNull][ItemNotNull] ParameterMemberInfo member,
            [NotNull] IDynamicParameterDescriptionProvider provider)
        {
            Ensure.ArgumentNotNull(member, nameof(member));
            Ensure.ArgumentNotNull(provider, nameof(provider));

            throw new NotImplementedException();
        }
    }
}
