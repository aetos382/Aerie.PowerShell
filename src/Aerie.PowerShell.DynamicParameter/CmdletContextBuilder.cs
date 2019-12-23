using System;

namespace Aerie.PowerShell.DynamicParameter
{
    public class CmdletContextBuilder :
        ICmdletContextBuilder
    {
        public ICmdletContextBuilder AddDynamicParameter(
            DynamicParameterDescriptor parameterDescriptor)
        {
            throw new NotImplementedException();
        }

        public ICmdletContext Build()
        {
            throw new NotImplementedException();
        }
    }
}
