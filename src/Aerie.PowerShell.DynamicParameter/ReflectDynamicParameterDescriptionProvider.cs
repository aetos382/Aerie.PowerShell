using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aerie.PowerShell.DynamicParameter
{
    public class ReflectDynamicParameterDescriptionProvider :
        IParameterDescriptionProvider
    {
        public IEnumerable<ParameterDescriptor> GetParameterDescriptors(
            IDynamicParameterContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var members = context.CmdletType.GetMember(
                "*",
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Instance | BindingFlags.Public);

            foreach (var member in members)
            {
                var attributes = member
                    .GetCustomAttributes<DynamicParameterAttribute>(true)
                    .ToArray();

                if (attributes.Length == 0)
                {
                    continue;
                }

                var descriptor =
                    ReflectParameterDescriptor.GetDescriptor(
                        new ParameterMemberInfo(member), null);

                yield return descriptor;
            }
        }
    }
}
