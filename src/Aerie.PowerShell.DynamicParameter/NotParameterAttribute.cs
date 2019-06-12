using System;

namespace Aerie.PowerShell.DynamicParameter
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NotParameterAttribute :
        Attribute
    {
    }
}
