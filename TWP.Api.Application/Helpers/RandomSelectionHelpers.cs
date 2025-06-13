using Common.Randomizer;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Application.Helpers
{
    public static class RandomSelectionHelpers
    {
        /// <summary>
        /// Get randomly one entry from the random table.
        /// </summary>
        /// <param name="rollTableDto"></param>
        /// <returns></returns>
        public static RollTableEntryDto GetRandomlyASingleEntry(this RollTableDto rollTableDto) 
        { 
            var rollResult = RollDie(rollTableDto.NumberOfDiceType, rollTableDto.DiceType);
            return rollTableDto.Entries.First(e => e.MinRoll <= rollResult && e.MaxRoll >= rollResult);
        }

        public static List<RollTableEntryDto> GetRandomlyManyEntries(this RollTableDto rollTableDto, int numberOfEntries)
        { 
            var results = new List<RollTableEntryDto>();
            for (int i = 0; i < numberOfEntries; i++)
            {
                results.Add(rollTableDto.GetRandomlyASingleEntry());
            }
            return results;
        }

        public static int RollDie(int numberOfDiceType, DiceTypeEnum diceType)
            => new Dice(numberOfDiceType, (int)diceType).Roll;
    }
}
