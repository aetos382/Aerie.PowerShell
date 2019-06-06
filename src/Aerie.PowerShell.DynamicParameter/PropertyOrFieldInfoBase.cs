using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public abstract class PropertyOrFieldInfoBase :
        MemberInfo
    {
        protected PropertyOrFieldInfoBase(
            [NotNull] MemberInfo member)
        {
            Ensure.ArgumentNotNull(member, nameof(member));

            if (member is PropertyOrFieldInfoBase pf)
            {
                this.PropertyOrFieldType = pf.PropertyOrFieldType;

                this.CanRead = pf.CanRead;
                this.CanWrite = pf.CanWrite;
                this.IsStatic = pf.IsStatic;

                this.BaseMemberInfo = pf.BaseMemberInfo;
            }
            else if (member is PropertyInfo p)
            {
                this.PropertyOrFieldType = p.PropertyType;

                this.CanRead = p.CanRead;
                this.CanWrite = p.CanWrite;
                this.IsStatic = p.GetAccessors(false).Any(a => a.IsStatic);

                this.BaseMemberInfo = member;
            }
            else if(member is FieldInfo f)
            {
                this.PropertyOrFieldType = f.FieldType;

                this.CanRead = true;
                this.CanWrite = true;
                this.IsStatic = f.IsStatic;

                this.BaseMemberInfo = member;
            }
            else
            {
                throw new ArgumentException();
            }

            this._getValueAccessor = new Lazy<GetValueAccessor>(this.CreateGetValueAccessor);
            this._setValueAccessor = new Lazy<SetValueAccessor>(this.CreateSetValueAccessor);
        }

        public override object[] GetCustomAttributes(
            bool inherit)
        {
            return this.BaseMemberInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(
            Type attributeType,
            bool inherit)
        {
            return this.BaseMemberInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(
            Type attributeType,
            bool inherit)
        {
            return this.BaseMemberInfo.IsDefined(attributeType, inherit);
        }

        public override Type DeclaringType
        {
            get
            {
                return this.BaseMemberInfo.DeclaringType;
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                return this.BaseMemberInfo.MemberType;
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

        public override int MetadataToken
        {
            get
            {
                return this.BaseMemberInfo.MetadataToken;
            }
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                return this.BaseMemberInfo.CustomAttributes;
            }
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return this.BaseMemberInfo.GetCustomAttributesData();
        }

        public override bool HasSameMetadataDefinitionAs(MemberInfo other)
        {
            return this.BaseMemberInfo.HasSameMetadataDefinitionAs(other);
        }

        public override Module Module
        {
            get
            {
                return this.BaseMemberInfo.Module;
            }
        }

        [NotNull]
        public MemberInfo BaseMemberInfo { [Pure] get; }

        public Type PropertyOrFieldType { [Pure] get; }

        public bool CanRead { get; }

        public bool CanWrite { get; }

        public bool IsStatic { get; }

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
    }
}
