using System;

namespace Aerie.PowerShell
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NonParameterAttribute :
        Attribute
    {
    }
}
