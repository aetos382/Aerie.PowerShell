using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class DynamicParameterInstance
    {
        internal DynamicParameterInstance(
            [NotNull] DynamicParameterContext context,
            [NotNull] DynamicParameterDescriptor descriptor)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            this.Context = context;
            this.ParameterDescriptor = descriptor;
        }

        [NotNull]
        public IDynamicParameterContext Context { get; }

        [NotNull]
        public DynamicParameterDescriptor ParameterDescriptor { [Pure] get; }

        public bool IsSet { [Pure] get; private set; }

        public event EventHandler<ParameterValueEventArgs> ValueSet; 

        [CanBeNull]
        private object _value;

        [CanBeNull]
        public object Value
        {
            [Pure]
            get
            {
                var value = this._value;

                if (!this.IsSet)
                {
                    value = this.ParameterDescriptor.GetParameterValue(this.Context);
                }

                return value;
            }

            set
            {
                this.ParameterDescriptor.SetParameterValue(this.Context, value);

                this._value = value;
                this.IsSet = true;

                if (this.ValueSet != null)
                {
                    this.ValueSet(this, new ParameterValueEventArgs(value));
                }
            }
        }
    }
}
