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
            var uniqueEntries = rollTableDto.Entries.ToList();
            var results = new List<RollTableEntryDto>();
            var random = new Random();

            // Shuffle the entries for randomness
            uniqueEntries = uniqueEntries.OrderBy(x => random.Next()).ToList();

            var toTake = Math.Min(numberOfEntries, uniqueEntries.Count);

            // Take as many unique as possible
            results.AddRange(uniqueEntries.Take(toTake));

            // If more are needed, allow duplicates
            for (int i = toTake; i < numberOfEntries; i++)
                results.Add(rollTableDto.GetRandomlyASingleEntry());

            return results;
        }

        public static int RollDie(int numberOfDiceType, DiceTypeEnum diceType)
            => new Dice(numberOfDiceType, (int)diceType).Roll;
    }
}
