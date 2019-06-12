using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class MutableCustomAttributeData :
        CustomAttributeData
    {
        public MutableCustomAttributeData(
            [NotNull] ConstructorInfo constructor)
            : this(
                constructor,
                Array.Empty<CustomAttributeTypedArgument>(),
                Array.Empty<CustomAttributeNamedArgument>())
        {
        }

        public MutableCustomAttributeData(
            [NotNull] ConstructorInfo constructor,
            [NotNull] IReadOnlyList<CustomAttributeTypedArgument> constructorArguments)
            : this(
                constructor,
                constructorArguments,
                Array.Empty<CustomAttributeNamedArgument>())
        {
        }

        public MutableCustomAttributeData(
            [NotNull] ConstructorInfo constructor,
            [NotNull] IReadOnlyList<CustomAttributeTypedArgument> constructorArguments,
            [NotNull] IReadOnlyCollection<CustomAttributeNamedArgument> namedArguments)
        {
            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            if (namedArguments is null)
            {
                throw new ArgumentNullException(nameof(namedArguments));
            }

            this.Constructor = constructor;
            this.ConstructorArguments = constructorArguments.ToList();
            this.NamedArguments = namedArguments.ToList();
        }

        public override ConstructorInfo Constructor { [Pure] get; }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments { [Pure] get; }

        public override IList<CustomAttributeNamedArgument> NamedArguments { [Pure] get; }
    }
}
