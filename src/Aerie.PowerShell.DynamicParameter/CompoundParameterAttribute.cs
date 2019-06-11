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
            Ensure.ArgumentNotNull(member, nameof(member));

            throw new NotImplementedException();
        }
    }
}
