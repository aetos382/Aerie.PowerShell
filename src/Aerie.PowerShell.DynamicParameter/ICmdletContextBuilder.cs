using System;
using System.Collections.Generic;
using System.Text;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface ICmdletContextBuilder
    {
        ICmdletContextBuilder AddDynamicParameter(
            DynamicParameterDescriptor parameterDescriptor);

        ICmdletContext Build();
    }
}
