using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Randomizer
{
    /// <summary>
    /// Provides helper methods for shuffling collections.
    /// </summary>
    public static class ShuffleHelper
    {
        /// <summary>
        /// Shuffles the elements of the list in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">Type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        private static void Shuffle<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            var random = new Random(DateTime.Now.Millisecond);
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary>
        /// Returns a new list with the elements of the source shuffled.
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection.</typeparam>
        /// <param name="source">The source collection to shuffle.</param>
        /// <returns>A new shuffled list.</returns>
        public static List<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Shuffle(source);
            return source.ToList();
        }
    }
} 