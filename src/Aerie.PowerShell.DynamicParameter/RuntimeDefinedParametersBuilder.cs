﻿using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public class RuntimeDefinedParametersBuilder :
        IDynamicParameterObjectBuilder
    {
        [NotNull]
        public static readonly RuntimeDefinedParametersBuilder Instance = new RuntimeDefinedParametersBuilder();

        public object GetDynamicParameterObject(
            IDynamicParameterContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var rdpd = new RuntimeDefinedParameterDictionary();

            foreach (var parameter in context.GetParameters())
            {
                var descriptor = parameter.ParameterDescriptor;

                var attributes = new Collection<Attribute>();

                foreach (var attributeData in descriptor.Attributes)
                {
                    attributes.Add((Attribute)attributeData.GetInstance());
                }

                var rdp = new RuntimeDefinedParameter(
                    descriptor.ParameterName,
                    descriptor.ParameterType,
                    attributes);

                if (parameter.IsSet)
                {
                    rdp.Value = parameter.Value;
                }

                rdpd.Add(descriptor.ParameterName, rdp);
            }

            return rdpd;
        }
    }
}
