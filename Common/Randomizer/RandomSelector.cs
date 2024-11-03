namespace Common.Randomizer
{
    public static class RandomSelector
    {
        public static T SelectOneRandomly<T>(T[] array)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(0, array.Count());
            return array[index];
        }
    }

    public class RandomSelector<T>
    {
        private Random _random;
        public RandomSelector()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Pick the one enum value whose index is equal or lower and the lowest index then the percentil
        /// </summary>
        /// <param name="array"></param>
        /// <param name="percentil"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>Just emulate a d100 roll to pick a value from a chart.</remarks>
        public T PercentilSelector(T[] array, int percentil)
        {
            if(!typeof(T).IsEnum)
                throw new ArgumentException($"T is must be an enum !");
            if (array.Length <= 0)
                throw new ArgumentException("Empty array !");
            int result;
            foreach (var item in array)
            {
                if(percentil <= (int)(object)item)
                    return item;
            }
            throw new ArgumentException("No integer parsed !");
        }

        public T SelectOneRandomly(T[] array)
        {
            var index = _random.Next(0, array.Count());
            return array[index];
        }

        public T[] SelectManyRandomly(T[] array, int howMany)
        {
            if (howMany > array.Count())
                throw new Exception($"Impossible to get all different numbers, the {howMany} is greater than the number of indexes of the array.");

            var indexes = GetDifferentNumbers(array, howMany);
            T[] selectionArray = new T[howMany];
            for (int i = 0; i < howMany; i++)
            {
                selectionArray[i] = array[indexes[i]];
            }
            return selectionArray;
        }

        private int[] GetDifferentNumbers(T[] array, int howMany)
        {
            if (howMany > array.Count())
                throw new Exception($"Impossible to get all different numbers, the {howMany} is greater than the number of indexes of the array.");

            var numbers = new int[howMany];
            numbers[0] = GetRandomArrayIndex(array);
            var securityLock = 10000;
            for (int i = 1; i < howMany; i++)
            {
                var getOut = 0;
                int otherNumber;
                {
                    getOut++;
                    otherNumber = GetRandomArrayIndex(array);
                } while (numbers.Contains(otherNumber) && getOut < securityLock) ;
                numbers[i] = otherNumber;
            }
            return numbers;
        }

        private int GetRandomArrayIndex(T[] array) => _random.Next(0, array.Count() - 1);
    }
}
