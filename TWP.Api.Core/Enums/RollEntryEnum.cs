namespace TWP.Api.Core.Enums
{
    public enum RollEntryEnum
    {
        //Standart result
        Result = 1, 

        //The result ask to roll again on the same table
        RollAgain = 2, 

        //The result ask to roll on another table.
        Linked = 3, 

        //The result ask to roll a certain amount of time on the table.
        MultiRoll = 4
    }
}
