using System;
using System.Management.Automation;

using Aerie.PowerShell.DynamicParameter.Internal;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class CmdletContext
    {
        public static ICmdletContextBuilder CreateDefaultBuilder(
            Cmdlet cmdlet)
        {
            ICmdletContextBuilder builder = new CmdletContextBuilder();
            builder = builder.FromCmdlet(cmdlet);

            return builder;
        }
    }
}
