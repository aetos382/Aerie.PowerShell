using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class ParameterValueEventArgs :
        EventArgs
    {
        public ParameterValueEventArgs(
            [CanBeNull] object value)
        {
            this.Value = value;
        }

        [CanBeNull]
        public object Value { [Pure] get; }
    }
}
