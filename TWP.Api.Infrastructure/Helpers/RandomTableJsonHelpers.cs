using System.Text.RegularExpressions;

namespace TWP.Api.Infrastructure.Helpers
{
    public static class RandomTableJsonHelpers
    {
        /// <summary>
        /// Extract the numbers from a string. Used to extract the min and max roll from a string.
        /// </summary>
        /// <param name="str">The string that contains one or more numbers</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static (int firstNumber, int secondNumber) ExtractMinAndMaxRollNumbers(this string str)
        {
            if (Regex.IsMatch(str, @"^\d+$")) // Single number
            {
                int number = int.Parse(str);
                return (number, number);
            }
            else if (Regex.IsMatch(str, @"(\d+)-(\d+)")) // Number range
            {
                var match = Regex.Match(str, @"(\d+)-(\d+)");
                int firstNumber = int.Parse(match.Groups[1].Value);
                int secondNumber = int.Parse(match.Groups[2].Value);
                return (firstNumber, secondNumber);
            }
            else
            {
                throw new ArgumentException("The input string does not contain a valid number or range.");
            }
        }
    }
}
