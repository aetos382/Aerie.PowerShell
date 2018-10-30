using System;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    [DynamicParameterInternal]
    public sealed class CompoundParameterAttribute :
        Attribute
    {
    }
}
