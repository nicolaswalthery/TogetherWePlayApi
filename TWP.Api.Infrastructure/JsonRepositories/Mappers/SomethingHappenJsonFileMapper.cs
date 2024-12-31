using Common.Extensions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class SomethingHappenJsonFileMapper
    {
        public static RollTableDto ToSomethingHappensRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<SomethingHappensRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d100,
                Genre = GenreEnum.Fantasy,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Something Happen",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.DarkFantasy },
                Source = SourceEnum.Shadowdark,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var somethingHappen in deserializedObject.SomethingHappens)
            {
                var (minRoll, maxRoll) = ExtractNumbers(somethingHappen.d100);
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.Details,
                    Type = Core.Enums.RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        private static (int firstNumber, int secondNumber) ExtractNumbers(string roll)
        {
            if (Regex.IsMatch(roll, @"^\d+$")) // Single number
            {
                int number = int.Parse(roll);
                return (number, number);
            }
            else if (Regex.IsMatch(roll, @"(\d+)-(\d+)")) // Number range
            {
                var match = Regex.Match(roll, @"(\d+)-(\d+)");
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

    internal class SomethingHappensEntry
    {
        public string d100 { get; set; }
        public string Details { get; set; }
    }

    internal class SomethingHappensRoot
    {
        public List<SomethingHappensEntry> SomethingHappens { get; set; }
    }
}
