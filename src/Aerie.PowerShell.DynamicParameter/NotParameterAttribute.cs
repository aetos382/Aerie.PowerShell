using System;

namespace Aerie.PowerShell
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NotParameterAttribute :
        Attribute
    {
    }
}
