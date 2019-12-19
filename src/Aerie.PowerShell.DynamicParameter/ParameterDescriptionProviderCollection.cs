using System;
using System.Collections.Generic;

using Microsoft;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class ParameterDescriptionProviderCollection :
        HashSet<IParameterDescriptionProvider>
    {
        public ParameterDescriptionProviderCollection()
        {
        }

        public ParameterDescriptionProviderCollection(
            [NotNull][ItemNotNull] IReadOnlyCollection<IParameterDescriptionProvider> items)
        {
            Requires.NotNull(items, nameof(items));

            foreach (var item in items)
            {
                this.Add(item);
            }
        }
    }
}
