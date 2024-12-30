using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.DbEntities
{
    public class RollTableEntryJsonStaticDataDTO
    {
        /// <summary>
        /// Name of the roll table.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of physical dice used to randomly select an entry in the table.
        /// </summary>
        public DiceTypeEnum DiceType { get; set; }

        /// <summary>
        /// Number of physical dice used to randomly select an entry in the table.
        /// </summary>
        public int NumberOfDiceType { get; set; }

        /// <summary>
        /// Maximum number of rerolls allowed for this table.
        /// </summary>
        public int MaxRerolls { get; set; }

        /// <summary>
        /// Template sentence with placeholders to be completed by the roll result
        /// </summary>
        public string SentenceTemplate { get; set; }
        
        /// <summary>
        /// Setting from which the table is coming from.
        /// </summary>
        public SettingEnum Setting { get; set; }

        /// <summary>
        /// Tells us in what subgenres this roll table is most appropriated.
        /// </summary>
        public List<SubgenreEnum> Subgenres { get; set; }

        /// <summary>
        /// Tells us in what genre this roll table is most appropriated.
        /// </summary>
        public GenreEnum Genre { get; set; }

        /// <summary>
        /// Flag that tells us if the table is protected or not by a copywrite law restriction.
        /// </summary>
        public bool IsTableCopywriteFree { get; set; }

        
        //Navigation Property
        public List<RollTableEntryJsonStaticDataDTO> RollTableEntries { get; set; }
    }
}
