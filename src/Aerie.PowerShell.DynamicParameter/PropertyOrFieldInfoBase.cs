using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public abstract class PropertyOrFieldInfoBase :
        MemberInfo,
        IEquatable<PropertyOrFieldInfoBase>
    {
        protected PropertyOrFieldInfoBase()
        {
            this._getValueAccessor = new Lazy<GetValueAccessor>(this.CreateGetValueAccessor);
            this._setValueAccessor = new Lazy<SetValueAccessor>(this.CreateSetValueAccessor);
        }

        protected PropertyOrFieldInfoBase(
            [NotNull] MemberInfo member)
        {
            this.InitializeMemberInfo(member);
        }

        public override object[] GetCustomAttributes(
            bool inherit)
        {
            this.EnsureInitialized();

            return this.BaseMemberInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(
            Type attributeType,
            bool inherit)
        {
            this.EnsureInitialized();

            return this.BaseMemberInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(
            Type attributeType,
            bool inherit)
        {
            this.EnsureInitialized();

            return this.BaseMemberInfo.IsDefined(attributeType, inherit);
        }

        public override Type DeclaringType
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.DeclaringType;
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.MemberType;
            }
        }

        public override string Name
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.Name;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.ReflectedType;
            }
        }

        public override int MetadataToken
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.MetadataToken;
            }
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.CustomAttributes;
            }
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            this.EnsureInitialized();

            return this.BaseMemberInfo.GetCustomAttributesData();
        }

        public override bool HasSameMetadataDefinitionAs(MemberInfo other)
        {
            this.EnsureInitialized();

            return this.BaseMemberInfo.HasSameMetadataDefinitionAs(other);
        }

        public override Module Module
        {
            get
            {
                this.EnsureInitialized();

                return this.BaseMemberInfo.Module;
            }
        }

        protected void InitializeMemberInfo(
            [NotNull] MemberInfo baseMemberInfo)
        {
            Ensure.ArgumentNotNull(baseMemberInfo, nameof(baseMemberInfo));

            this.EnsureNotInitialized();

            if (baseMemberInfo is PropertyOrFieldInfoBase pf)
            {
                this.PropertyOrFieldType = pf.PropertyOrFieldType;

                this.CanRead = pf.CanRead;
                this.CanWrite = pf.CanWrite;
                this.IsStatic = pf.IsStatic;

                this.BaseMemberInfo = pf.BaseMemberInfo;
            }
            else if (baseMemberInfo is PropertyInfo p)
            {
                this.PropertyOrFieldType = p.PropertyType;

                this.CanRead = p.CanRead;
                this.CanWrite = p.CanWrite;
                this.IsStatic = p.GetAccessors(false).Any(a => a.IsStatic);

                this.BaseMemberInfo = baseMemberInfo;
            }
            else if(baseMemberInfo is FieldInfo f)
            {
                this.PropertyOrFieldType = f.FieldType;

                this.CanRead = true;
                this.CanWrite = true;
                this.IsStatic = f.IsStatic;

                this.BaseMemberInfo = baseMemberInfo;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void EnsureInitialized()
        {
            if (this.BaseMemberInfo is null)
            {
                throw new InvalidOperationException($"{nameof(this.BaseMemberInfo)} is not initialized.");
            }
        }

        private void EnsureNotInitialized()
        {
            if (!(this.BaseMemberInfo is null))
            {
                throw new InvalidOperationException($"{nameof(this.BaseMemberInfo)} is already initialized.");
            }
        }

        [CanBeNull]
        public MemberInfo BaseMemberInfo { get; private set; }

        public Type PropertyOrFieldType { [Pure] get; private set; }

        public bool CanRead { get; private set; }

        public bool CanWrite { get; private set; }

        public bool IsStatic { get; private set; }

        protected delegate object GetValueAccessor(object target);

        protected delegate void SetValueAccessor(object target, object value);

        [NotNull]
        protected abstract GetValueAccessor CreateGetValueAccessor();

        [NotNull]
        protected abstract SetValueAccessor CreateSetValueAccessor();

        [NotNull]
        private readonly Lazy<GetValueAccessor> _getValueAccessor;

        [NotNull]
        private readonly Lazy<SetValueAccessor> _setValueAccessor;

        public virtual object GetValue(
            object target)
        {
            return this._getValueAccessor.Value(target);
        }

        public virtual void SetValue(
            object target,
            object value)
        {
            this._setValueAccessor.Value(target, value);
        }

        public override string ToString()
        {
            var baseMember = this.BaseMemberInfo;

            return $"{baseMember.DeclaringType.Name}.{baseMember.Name}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PropertyOrFieldInfoBase p))
            {
                return false;
            }

            return this.Equals(p);
        }

        public override int GetHashCode()
        {
            if (this.BaseMemberInfo is null)
            {
                return 0;
            }

            return this.BaseMemberInfo.GetHashCode();
        }

        public bool Equals(
            PropertyOrFieldInfoBase other)
        {
            if (other is null)
            {
                return false;
            }
            
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return object.Equals(this.BaseMemberInfo, other.BaseMemberInfo);
        }
    }
}
