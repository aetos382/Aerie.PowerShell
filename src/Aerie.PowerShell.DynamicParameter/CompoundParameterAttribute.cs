using System;
using System.Collections.Generic;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    [NotParameter]
    public sealed class CompoundParameterAttribute :
        Attribute,
        IDynamicParameterDescriptionProvider
    {
        public IEnumerable<DynamicParameterDescriptor> GetParameterDescriptors(
            ParameterMemberInfo member)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            throw new NotImplementedException();
        }
    }
}
