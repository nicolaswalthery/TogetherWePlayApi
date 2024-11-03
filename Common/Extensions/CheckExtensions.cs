namespace Common.Extensions
{
    public static class CheckExtensions
    {
        /// <summary>
        /// Determines whether a string is convertible to Guid or not.
        /// </summary>
        /// <param name="str">The string.</param>
        public static bool IsGuidConvertible(this string str)
            => Guid.TryParse(str, out Guid _);

        /// <summary>
        /// Determines if a string is not convertible to Guid.
        /// </summary>
        /// <param name="str">The string.</param>
        public static bool IsNotGuidConvertible(this string str)
            => !Guid.TryParse(str, out Guid _);

        /// <summary>
        /// Determines if this collection has at least one element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        public static bool HasElement<T>(this IEnumerable<T> collection)
            => collection.Any();

        /// <summary>
        /// Determines if the collection has NO elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        public static bool HasNoElement<T>(this IEnumerable<T> collection)
            => !collection.Any();
    }
}