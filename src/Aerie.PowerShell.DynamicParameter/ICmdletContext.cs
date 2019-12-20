using System;
using System.Collections.Generic;
using System.Text;

namespace Aerie.PowerShell.DynamicParameter
{
    public interface ICmdletContext
    {
        IEnumerable<DynamicParameterDescriptor> GetParameterDescriptors();
    }
}
