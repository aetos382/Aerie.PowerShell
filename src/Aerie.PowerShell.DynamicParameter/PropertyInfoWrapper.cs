using System;
using System.Reflection;

using JetBrains.Annotations;

using NJsonSchema.Infrastructure;

namespace Aerie.PowerShell
{
    internal sealed class PropertyInfoWrapper :
        MemberInfoWrapper
    {
        [NotNull]
        public PropertyInfo PropertyInfo { get; }

        public PropertyInfoWrapper(
            [NotNull] PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }

        public override object GetValue(
            object target)
        {
            return this.PropertyInfo.GetValue(target);
        }

        public override void SetValue(
            object target,
            object value)
        {
            this.PropertyInfo.SetValue(target, value);
        }

        public override Type Type
        {
            get
            {
                return this.PropertyInfo.PropertyType;
            }
        }

        public static implicit operator PropertyInfo(
            [CanBeNull] PropertyInfoWrapper wrapper)
        {
            return wrapper?.PropertyInfo;
        }
    }
}
