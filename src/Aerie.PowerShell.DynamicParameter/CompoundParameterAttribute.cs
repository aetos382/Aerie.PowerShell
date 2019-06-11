using System;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    [NotParameter]
    public sealed class CompoundParameterAttribute :
        Attribute,
        IDynamicParameterDescriptionProvider
    {
    }
}
