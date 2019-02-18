using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
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
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

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
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

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
    }
}
