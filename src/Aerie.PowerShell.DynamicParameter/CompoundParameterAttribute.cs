using System;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    [NonParameter]
    public sealed class CompoundParameterAttribute :
        Attribute,
        IDynamicParameterDescriptionProvider
    {
    }
}
