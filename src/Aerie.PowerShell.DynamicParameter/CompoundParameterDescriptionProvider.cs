﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Aerie.Commons.Contracts;

namespace Aerie.PowerShell.DynamicParameter
{
    public class CompoundParameterDescriptionProvider :
        IParameterDescriptionProvider
    {
        public IEnumerable<ParameterDescriptor> GetParameterDescriptors(
            Type cmdletType)
        {
            Ensures.ArgumentNotNull(cmdletType, nameof(cmdletType));

            var members = cmdletType.GetMember(
                "*",
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Instance | BindingFlags.Public);

            foreach (var member in members)
            {
                var attributes = member
                    .GetCustomAttributes<CompoundParameterAttribute>(true)
                    .ToArray();

                if (attributes.Length == 0)
                {
                    continue;
                }

                // TODO: 実装する
                yield break;
            }
        }
    }
}
