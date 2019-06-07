using System;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class PropertyOrFieldInfo :
        PropertyOrFieldInfoBase
    {
        public PropertyOrFieldInfo(
            [NotNull] MemberInfo member)
            : base(member)
        {
        }

        public static implicit operator PropertyOrFieldInfo(
            PropertyInfo property)
        {
            Ensure.ArgumentNotNull(property, nameof(property));

            return new PropertyOrFieldInfo(property);
        }

        public static implicit operator PropertyOrFieldInfo(
            FieldInfo field)
        {
            Ensure.ArgumentNotNull(field, nameof(field));

            return new PropertyOrFieldInfo(field);
        }

        protected override GetValueAccessor CreateGetValueAccessor()
        {
            var parameter = Expression.Parameter(typeof(object));

            var lambda = Expression.Lambda<GetValueAccessor>(
                Expression.Convert(
                    Expression.PropertyOrField(
                        Expression.Convert(
                            parameter,
                            this.BaseMemberInfo.DeclaringType),
                        this.BaseMemberInfo.Name),
                    typeof(object)),
                parameter);

            return lambda.Compile();
        }

        protected override SetValueAccessor CreateSetValueAccessor()
        {
            var targetParameter = Expression.Parameter(typeof(object));
            var valueParameter = Expression.Parameter(typeof(object));

            var lambda = Expression.Lambda<SetValueAccessor>(
                Expression.Assign(
                    Expression.PropertyOrField(
                        Expression.Convert(
                            targetParameter,
                            this.BaseMemberInfo.DeclaringType),
                        this.BaseMemberInfo.Name),
                    Expression.Convert(
                        valueParameter,
                        this.PropertyOrFieldType)),
                targetParameter,
                valueParameter);

            return lambda.Compile();
        }
    }
}
