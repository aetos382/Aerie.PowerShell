using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

using Microsoft;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class RuntimeDefinedParametersBuilder :
        IDynamicParameterObjectBuilder
    {
        [NotNull]
        public static readonly RuntimeDefinedParametersBuilder Instance =
            new RuntimeDefinedParametersBuilder();

        public object Build(
            ICmdletContext context)
        {
            Requires.NotNull(context, nameof(context));

            var rdpd = new RuntimeDefinedParameterDictionary();

            foreach (var parameter in context.GetParameterDescriptors())
            {
                var attributes = new Collection<Attribute>();

                foreach (var attributeData in parameter.Attributes)
                {
                    attributes.Add((Attribute)attributeData.GetInstance());
                }

                var rdp = new RuntimeDefinedParameter(
                    parameter.ParameterName,
                    parameter.ParameterType,
                    attributes);

                rdpd.Add(parameter.ParameterName, rdp);
            }

            return rdpd;
        }
    }
}
