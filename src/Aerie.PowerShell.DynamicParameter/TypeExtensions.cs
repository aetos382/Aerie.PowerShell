using System;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public static class TypeExtensions
    {
        public static bool IsDefaultConstructible(
            [NotNull] this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsGenericType)
            {
                if (!type.IsConstructedGenericType)
                {
                    return false;
                }
            }

            if (type.IsInterface)
            {
                return false;
            }

            if (type.IsAbstract)
            {
                return false;
            }

            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor is null)
            {
                return false;
            }

            return true;
        }
    }
}
