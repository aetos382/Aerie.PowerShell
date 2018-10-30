using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo(AssemblyInfo.ProxyAssemblyName)]
[assembly: InternalsVisibleTo("Aerie.PowerShell.DynamicParameter.UnitTests")]

internal static class AssemblyInfo
{
    public const string ProxyAssemblyName = "Aerie.PowerShell.DynamicParameter.ParameterProxies";
}
