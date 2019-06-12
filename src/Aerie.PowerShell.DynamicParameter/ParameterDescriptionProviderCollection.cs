using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class ParameterDescriptionProviderCollection :
        NonNullHashSet<IParameterDescriptionProvider>
    {
        public ParameterDescriptionProviderCollection()
        {
        }

        public ParameterDescriptionProviderCollection(
            [NotNull][ItemNotNull] IReadOnlyCollection<IParameterDescriptionProvider> items)
        {
            Ensure.ArgumentNotNull(items, nameof(items));

            foreach (var item in items)
            {
                this.Add(item);
            }
        }
    }
}
