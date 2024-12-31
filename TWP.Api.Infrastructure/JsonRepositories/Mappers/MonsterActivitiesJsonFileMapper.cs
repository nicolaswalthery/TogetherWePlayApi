using Common.Extensions;
using System.Text.RegularExpressions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class MonsterActivitiesJsonFileMapper
    {
        public static RollTableDto ToDto(this string json)
        {
            var deserializedObject = json.ToObject<Root>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d6,
                Genre = GenreEnum.Fantasy,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Monster Activity",
                NumberOfDiceType = 2,
                Subgenres = { SubgenreEnum.DarkFantasy },
                Source = SourceEnum.Shadowdark,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var activity in deserializedObject.ActivityTable) // Matches the updated property
            {
                var (minRoll, maxRoll) = ExtractNumbers(activity.Roll);
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = activity.Activity,
                    Type = Core.Enums.RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static (int firstNumber, int secondNumber) ExtractNumbers(string roll)
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

    internal class MonsterActivity
    {
        public string Roll { get; set; }
        public string Activity { get; set; }
    }

    internal class Root
    {
        public List<MonsterActivity> ActivityTable { get; set; } // Matches the JSON key
    }
}
