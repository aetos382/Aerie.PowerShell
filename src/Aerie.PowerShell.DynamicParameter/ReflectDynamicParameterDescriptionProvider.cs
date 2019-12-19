using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft;

namespace Aerie.PowerShell.DynamicParameter
{
    public class ReflectDynamicParameterDescriptionProvider :
        IParameterDescriptionProvider
    {
        public IEnumerable<ParameterDescriptor> GetParameterDescriptors(
            Type cmdletType)
        {
            Requires.NotNull(cmdletType, nameof(cmdletType));

            var members = cmdletType.GetMember(
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
