using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal static class ListExtensions
    {
        [CanBeNull]
        public static T Last<T>(
            [NotNull] this IReadOnlyList<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!TryGetLast(source, out var element))
            {
                throw new InvalidOperationException();
            }

            return element;
        }

        public static bool TryGetLast<T>(
            [NotNull] this IReadOnlyList<T> source,
            [CanBeNull] out T element)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            element = default;

            if (source.Count == 0)
            {
                return false;
            }

            element = source[source.Count - 1];
            return true;
        }
    }
}
