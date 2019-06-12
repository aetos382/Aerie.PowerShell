using System;
using System.Collections.Generic;

namespace Aerie.PowerShell.DynamicParameter
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field)]
    [NotParameter]
    public sealed class CompoundParameterAttribute :
        Attribute
    {
    }
}
