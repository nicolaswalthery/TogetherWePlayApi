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
        public static RollTableEntryDto GetRandomlyOneEntry(this RollTableDto rollTableDto)
        {
            var rollResult = RollDie(rollTableDto.NumberOfDiceType, rollTableDto.DiceType);
            return rollTableDto.Entries.First(e => e.MinRoll <= rollResult && e.MaxRoll >= rollResult);
        }

        public static int RollDie(int numberOfDiceType, DiceTypeEnum diceType)
            => new Dice(numberOfDiceType, (int)diceType).Roll;
    }
}
