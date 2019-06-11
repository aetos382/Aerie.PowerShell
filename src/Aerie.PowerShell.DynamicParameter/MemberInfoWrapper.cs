using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal abstract class MemberInfoWrapper :
        MemberInfo
    {
        protected MemberInfoWrapper(
            [NotNull] MemberInfo memberInfo)
        {
            if (memberInfo is null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            this.BaseMemberInfo = memberInfo;
        }

        [NotNull]
        public MemberInfo BaseMemberInfo { get; }

        public abstract Type Type { get; }

        public abstract object GetValue(
            object target);

        public abstract void SetValue(
            object target,
            object value);

        public static MemberInfoWrapper Create(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo p)
            {
                return new PropertyInfoWrapper(p);
            }

            if (memberInfo is FieldInfo f)
            {
                return new FieldInfoWrapper(f);
            }

            throw new ArgumentException();
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return this.BaseMemberInfo.CustomAttributes;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                return this.BaseMemberInfo.DeclaringType;
            }
        }

        public bool Equals(MemberInfoWrapper other)
        {
            if (other is null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.BaseMemberInfo.Equals(other.BaseMemberInfo))
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is MemberInfoWrapper m)
            {
                return this.Equals(m);
            }

            return false;
        }

        public override object[] GetCustomAttributes(
            Type attributeType,
            bool inherit)
        {
            return this.BaseMemberInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(
            bool inherit)
        {
            return this.BaseMemberInfo.GetCustomAttributes(inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return this.BaseMemberInfo.GetCustomAttributesData();
        }

        public override int GetHashCode()
        {
            return this.BaseMemberInfo.GetHashCode();
        }

        public override bool HasSameMetadataDefinitionAs(
            MemberInfo other)
        {
            return this.BaseMemberInfo.HasSameMetadataDefinitionAs(other);
        }

        public override bool IsDefined(
            Type attributeType,
            bool inherit)
        {
            return this.BaseMemberInfo.IsDefined(attributeType, inherit);
        }

        public override MemberTypes MemberType
        {
            get
            {
                return this.BaseMemberInfo.MemberType;
            }
        }

        public override int MetadataToken
        {
            get
            {
                return this.BaseMemberInfo.MetadataToken;
            }
        }

        public override Module Module
        {
            get
            {
                return this.BaseMemberInfo.Module;
            }
        }

        public override string Name
        {
            get
            {
                return this.BaseMemberInfo.Name;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                return this.BaseMemberInfo.ReflectedType;
            }
        }

        public override string ToString()
        {
            return this.BaseMemberInfo.ToString();
        }
    }
}
