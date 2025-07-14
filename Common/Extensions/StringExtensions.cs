namespace Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Capitalize the first letter of a string. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <remarks>Return String.Empty if <paramref name="str"/> is null or empty or white space</remarks>
        public static string ToCapitalize(this string str)
            => str.IsNotNullOrEmptyOrWhiteSpace() ? char.ToUpper(str[0]) + str.Substring(1) : String.Empty;
        public static bool IsNotNullOrWhiteSpace(this string? str)
            => !String.IsNullOrWhiteSpace(str);
        
        public static bool IsNotNullOrEmpty(this string? str)
            => !String.IsNullOrEmpty(str);

        public static bool IsNullOrEmpty(this string? str)
            => String.IsNullOrEmpty(str);

        public static bool IsNullOrEmptyOrWhiteSpace(this string? str)
            => String.IsNullOrWhiteSpace(str) || String.IsNullOrEmpty(str);

        public static bool IsNotNullOrEmptyOrWhiteSpace(this string? str)
            => !str.IsNullOrEmptyOrWhiteSpace();

        public static bool AllNullOrEmpty(this IEnumerable<string> stringList)
        {
            foreach (var str in stringList)
            {
                if (!String.IsNullOrEmpty(str))
                    return false;
            }
            return true;
        }

        public static bool AllNullOrEmpty(string str1, string str2)
            => String.IsNullOrEmpty(str1) && String.IsNullOrEmpty(str2);

        public static bool AtLeastOneNullOrEmpty(this string[] stringArray)
        {
            var found = false;
            for (int i = 0; i < stringArray.Count() && found; i++)
            {
                if (String.IsNullOrEmpty(stringArray[i]))
                    found = true;
            }
            return found;
        }

        public static string ReplaceByEmpty(this string str, string valueToEmpty) 
            => str.Replace(valueToEmpty, "");

        public static string ReplaceByEmpty(this string str, string[] valuesToEmpty)
        {
            foreach (var value in valuesToEmpty)
            {
                str = str.ReplaceByEmpty(value);
            }
            return str;
        }

        public static string Truncate(this string str, int maxLenght)
        {
            if (str.Length < maxLenght)
                maxLenght = str.Length;
            return str.Substring(0, maxLenght);
        }

        public static string DisplayLineByLine(this string[] strings)
            => string.Join("\r\n", strings);

        public static int CountOccurrences(this string text, string searchString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchString))
                return 0;

            int count = 0;
            int currentIndex = 0;

            while ((currentIndex = text.IndexOf(searchString, currentIndex, StringComparison.Ordinal)) != -1)
            {
                count++;
                currentIndex += searchString.Length;
            }

            return count;
        }
    }
}
