using System;
using System.Collections.Generic;

using Aerie.Commons.Collections;
using Aerie.Commons.Contracts;

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
            Ensures.ArgumentNotNull(items, nameof(items));

            foreach (var item in items)
            {
                this.Add(item);
            }
        }
    }
}
