using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    internal static class CustomAttributeDataExtensions
    {
        [NotNull]
        private static readonly ConditionalWeakTable<CustomAttributeData, object> _cache =
            new ConditionalWeakTable<CustomAttributeData, object>();

        [NotNull]
        public static object GetInstance(
            [NotNull] this CustomAttributeData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var instance = _cache.GetValue(data, x => CreateInstanceNoCache(x));
            return instance;
        }

        [NotNull]
        private static object CreateInstanceNoCache(
            [NotNull] CustomAttributeData data)
        {
            var constructorArguments = new List<object>(data.ConstructorArguments.Count);

            foreach (var arg in data.ConstructorArguments)
            {
                if (arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> arrayArg)
                {
                    var array = Array.CreateInstance(arg.ArgumentType.GetElementType(), arrayArg.Count);

                    for (int i = 0; i < arrayArg.Count; ++i)
                    {
                        array.SetValue(arrayArg[i].Value, i);
                    }

                    constructorArguments.Add(array);
                }
                else
                {
                    constructorArguments.Add(arg.Value);
                }
            }

            var instance = data.Constructor.Invoke(constructorArguments.ToArray());

            foreach (var namedArgument in data.NamedArguments)
            {
                if (namedArgument.IsField)
                {
                    ((FieldInfo)namedArgument.MemberInfo).SetValue(instance, namedArgument.TypedValue.Value);
                }
                else
                {
                    ((PropertyInfo)namedArgument.MemberInfo).SetValue(instance, namedArgument.TypedValue.Value);
                }
            }

            return (Attribute)instance;
        }
    }
}
