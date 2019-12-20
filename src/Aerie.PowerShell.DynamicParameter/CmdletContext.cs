using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

using Aerie.PowerShell.DynamicParameter.Internal;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class CmdletContext
    {
        public static ICmdletContextBuilder CreateDefaultBuilder(
            Cmdlet cmdlet)
        {
            return new DefaultCmdletContextBuilder(cmdlet);
        }
    }
}
