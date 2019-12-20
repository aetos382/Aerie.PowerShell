using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

using Microsoft;

namespace Aerie.PowerShell.DynamicParameter.Internal
{
    internal class DefaultCmdletContextBuilder :
        ICmdletContextBuilder
    {
        public DefaultCmdletContextBuilder(
            Cmdlet cmdlet)
        {
            Requires.NotNull(cmdlet, nameof(cmdlet));
        }

        public ICmdletContext Build()
        {
            throw new NotImplementedException();
        }
    }
}
