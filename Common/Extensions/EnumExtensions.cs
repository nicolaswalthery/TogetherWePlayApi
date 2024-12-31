using Common.Randomizer;
using Common.ResultPattern;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Common.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsIncludedIn<T>(this string value)
            where T : Enum
            => Enum.IsDefined(typeof(T), value);

        public static bool IsExcludedFrom<T>(this string value)
            where T : Enum
            => !value.IsIncludedIn<T>();

        public static IEnumerable<T> GetEveryOthersValues<T>(this T enumValue)
            where T : Enum
        {
            var listContainingAllValues = GetAllElementsOf<T>();
            listContainingAllValues.Remove(enumValue);
            return listContainingAllValues;
        }

        public static IList<T> GetAllElementsOf<T>()
            where T : Enum
            => new List<T>(Enum.GetValues(typeof(T)) as T[]);

        public static int CountElementInEnum<T>()
            where T : Enum
            => Enum.GetValues(typeof(T)).Length;

        public static T GetRandomElementOfEnum<T>()
            where T : Enum
        {
            var result = new Random().Next(1, CountElementInEnum<T>() + 1);
            return (T)(object)result;
        }

        public static T[] GetManyRandomElementOfEnum<T>(int howMany)
            where T : Enum
        {
            var result = new T[howMany];
            var allValues = GetAllValuesOfEnum<T>().ToList();
            if(allValues.Count <= 0)
                throw new ArgumentException("Empty enum !");

            for (int i = 0; i < howMany; i++)
            {
                var rndNumber = new Random().Next(1, allValues.Count + 1);
                result[i] = allValues[rndNumber];
                allValues.RemoveAt(rndNumber);
            }
            return (T[])(object)result;
        }

        public static int GetMaxValue<T>(this T enummeration)
            where T : Enum
            => Enum.GetValues(typeof(T)).Cast<int>().Max();

        public static T[] OrderedByValue<T>(this T enumeration)
            where T : Enum
        {
            T[] enumValues = GetAllValuesOfEnum<T>();
            if (enumValues.Length <= 0)
                throw new ArgumentException("Empty enum !");
            Array.Sort(enumValues, (a, b) => (a).CompareTo(b));
            return enumValues;
        }

        public static T[] GetAllValuesOfEnum<T>()
            where T : Enum 
            => (T[])Enum.GetValues(typeof(T));

        public static T GetPercentilResultFromEnum<T>(T enumeration)
            where T : Enum
        {
            var resultPercentDie = new Dice(1, 100).Roll;
            T[] orderedEnumArray = enumeration.OrderedByValue();
            return new RandomSelector<T>().PercentilSelector(orderedEnumArray!, resultPercentDie);
        }
    }
}
