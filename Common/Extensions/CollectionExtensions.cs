namespace Common.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Groups the elements of a collection by two.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        ///<remarks>If you want a specific order, order your collection before. This method ONLY group by two elements</remarks>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static List<Tuple<T,T>> GroupByTwo<T>(this IList<T> list)
        {
            if (list.HasNoElement())
                throw new ArgumentNullException($"{nameof(GroupByTwo)} method - Empty array");

            var duos = new List<Tuple<T, T>>();
            do
            {
                var item1 = list.GetThenRemove();
                var item2 = list.GetThenRemove();
                var tuple = new Tuple<T, T>(item1, item2);
                duos.Add(tuple);
            }while (list.HasElement());

            return duos;
        }

        /// <summary>
        /// Gets the first element or default and than remove it from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        public static T GetThenRemove<T>(this IList<T> list)
        {
            var retVal = list.FirstOrDefault();
            if(retVal is not null)
                list.Remove(retVal);
            return retVal;
        }
    }
}
