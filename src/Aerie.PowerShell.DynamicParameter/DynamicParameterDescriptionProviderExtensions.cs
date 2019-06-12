using System;
using System.Collections.Generic;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class DynamicParameterDescriptionProviderExtensions
    {
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<ParameterDescriptor> GetParameterDescriptors<TCmdlet>(
            [NotNull] this IParameterDescriptionProvider provider,
            [NotNull] TCmdlet cmdlet)
            where TCmdlet : Cmdlet
        {
            Ensure.ArgumentNotNull(provider, nameof(provider));
            Ensure.ArgumentNotNull(cmdlet, nameof(cmdlet));

            var context = CmdletContext.GetContext(cmdlet);
            var descriptors = provider.GetParameterDescriptors(context);

            return descriptors;
        }
    }
}
