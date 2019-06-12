using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field)]
    [NotParameter]
    public sealed class DynamicParameterNameAttribute :
        Attribute
    {
        public DynamicParameterNameAttribute(
            [NotNull] string parameterName)
        {
            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            this.ParameterName = parameterName;
        }

        [NotNull]
        public string ParameterName { [Pure] get; }
    }
}
