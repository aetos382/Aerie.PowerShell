using System;
using System.Diagnostics;

using Aerie.Commons.Contracts;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class DelegateParameterDescriptor :
        ParameterDescriptor
    {
        public DelegateParameterDescriptor(
            [NotNull] string parameterName,
            [NotNull] Type parameterType)
            : base(parameterName, parameterType)
        {
        }

        [NotNull]
        private Func<IDynamicParameterContext, object> _getValueAccessor = _ => default;

        [NotNull]
        private Action<IDynamicParameterContext, object> _setValueAccessor = (context, value) => { };

        [NotNull]
        public Func<IDynamicParameterContext, object> GetValueAccessor
        {
            [Pure]
            get
            {
                return this._getValueAccessor;
            }

            set
            {
                Ensures.ArgumentNotNull(value, nameof(value));

                this._getValueAccessor = value;
            }
        }

        [NotNull]
        public Action<IDynamicParameterContext, object> SetValueAccessor
        {
            [Pure]
            get
            {
                return this._setValueAccessor;
            }

            set
            {
                Ensures.ArgumentNotNull(value, nameof(value));

                this._setValueAccessor = value;
            }
        }

        protected internal override object GetParameterValue(
            IDynamicParameterContext context)
        {
            var value = this.GetValueAccessor(context);
            return value;
        }

        protected internal override void SetParameterValue(
            IDynamicParameterContext context,
            object value)
        {
            this.SetValueAccessor(context, value);
        }

        protected override void GetHashCode(HashCode hashCode)
        {
            hashCode.Add(this.GetValueAccessor);
            hashCode.Add(this.SetValueAccessor);
        }

        public override bool Equals(
            ParameterDescriptor other)
        {
            if (!(base.Equals(other)))
            {
                return false;
            }

            Debug.Assert(!(other is null));

            var descriptor = (DelegateParameterDescriptor)other;

            if (!object.Equals(descriptor.GetValueAccessor, this.GetValueAccessor))
            {
                return false;
            }

            if (!object.Equals(descriptor.SetValueAccessor, this.SetValueAccessor))
            {
                return false;
            }

            return true;
        }
    }
}
