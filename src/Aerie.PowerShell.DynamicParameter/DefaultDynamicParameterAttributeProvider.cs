using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class DefaultDynamicParameterAttributeProvider :
        IDynamicParameterAttributeProvider
    {
        [NotNull]
        private static readonly Dictionary<MemberInfo, List<CustomAttributeData>> _cache =
            new Dictionary<MemberInfo, List<CustomAttributeData>>();

        public IEnumerable<CustomAttributeData> GetAttributeData(
            MemberInfo member)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (!_cache.TryGetValue(member, out var attributes))
            {
                _cache[member] = attributes = new List<CustomAttributeData>();

                foreach (var attribute in member.CustomAttributes)
                {
                    var attributeType = attribute.AttributeType;

                    if (attributeType == typeof(DynamicParameterAttribute))
                    {
                        attributes.Add(new ParameterAttributeData(attribute));
                    }
                    else if (!Attribute.IsDefined(attributeType, typeof(DynamicParameterInternalAttribute)))
                    {
                        attributes.Add(attribute);
                    }
                }
            }

            return attributes;
        }

        [NotNull]
        public static readonly DefaultDynamicParameterAttributeProvider Instance =
            new DefaultDynamicParameterAttributeProvider();
    }
}
