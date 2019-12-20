using System;
using System.Diagnostics;

using Microsoft;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class DelegateParameterDescriptor :
        DynamicParameterDescriptor
    {
        public DelegateParameterDescriptor(
            [NotNull] string parameterName,
            [NotNull] Type parameterType)
            : base(parameterName, parameterType)
        {
        }

        [NotNull]
        private Func<ICmdletContext, object> _getValueAccessor = _ => default;

        [NotNull]
        private Action<ICmdletContext, object> _setValueAccessor = (context, value) => { };

        [NotNull]
        public Func<ICmdletContext, object> GetValueAccessor
        {
            [Pure]
            get
            {
                return this._getValueAccessor;
            }

            set
            {
                Requires.NotNull(value, nameof(value));

                this._getValueAccessor = value;
            }
        }

        [NotNull]
        public Action<ICmdletContext, object> SetValueAccessor
        {
            [Pure]
            get
            {
                return this._setValueAccessor;
            }

            set
            {
                Requires.NotNull(value, nameof(value));

                this._setValueAccessor = value;
            }
        }

        protected internal override object GetParameterValue(
            ICmdletContext context)
        {
            var value = this.GetValueAccessor(context);
            return value;
        }

        protected internal override void SetParameterValue(
            ICmdletContext context,
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
            DynamicParameterDescriptor other)
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
