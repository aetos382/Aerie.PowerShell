using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class ParameterAttributeData :
        CustomAttributeData
    {
        public ParameterAttributeData(
            [NotNull] CustomAttributeData attributeData)
        {
            var type = typeof(ParameterAttribute);

            var namedArguments = new List<CustomAttributeNamedArgument>(attributeData.NamedArguments.Count);

            foreach (var na in attributeData.NamedArguments)
            {
                var member = type.GetProperty(na.MemberName);
                var namedArgument = new CustomAttributeNamedArgument(member, na.TypedValue);

                namedArguments.Add(namedArgument);
            }

            this.ConstructorArguments = attributeData.ConstructorArguments.ToArray();
            this.NamedArguments = namedArguments.ToArray();
        }

        [NotNull]
        private static readonly ConstructorInfo _defaultConstructor;

        static ParameterAttributeData()
        {
            var type = typeof(ParameterAttribute);

            _defaultConstructor = type.GetConstructor(Type.EmptyTypes);
        }

        public override ConstructorInfo Constructor
        {
            [Pure]
            get
            {
                return _defaultConstructor;
            }
        }

        public override IList<CustomAttributeNamedArgument> NamedArguments { [Pure] get; }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments { [Pure] get; }
    }
}
