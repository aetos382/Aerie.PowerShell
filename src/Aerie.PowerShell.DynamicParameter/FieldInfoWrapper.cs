using System;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal sealed class FieldInfoWrapper :
        MemberInfoWrapper
    {
        [NotNull]
        public FieldInfo FieldInfo { [Pure] get; }

        public FieldInfoWrapper(
            [NotNull] FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            this.FieldInfo = fieldInfo;
        }

        public override object GetValue(
            object target)
        {
            return this.FieldInfo.GetValue(target);
        }

        public override void SetValue(
            object target,
            object value)
        {
            this.FieldInfo.SetValue(target, value);
        }

        public override Type Type
        {
            get
            {
                return this.FieldInfo.FieldType;
            }
        }

        public static implicit operator FieldInfo(
            [CanBeNull] FieldInfoWrapper wrapper)
        {
            return wrapper?.FieldInfo;
        }
    }
}
