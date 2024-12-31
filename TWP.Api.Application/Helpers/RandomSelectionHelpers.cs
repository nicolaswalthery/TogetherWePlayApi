using Common.Randomizer;
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
        public static RollTableEntryDto GetRandomlyOneEntry(this RollTableDto rollTableDto)
        {
            var rollResult = new Dice(rollTableDto.NumberOfDiceType, (int)rollTableDto.DiceType).Roll;
            return rollTableDto.Entries.First(e => e.MinRoll <= rollResult && e.MaxRoll >= rollResult);
        }
    }
}
