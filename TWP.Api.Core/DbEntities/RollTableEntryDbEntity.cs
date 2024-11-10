using TWP.Api.Core.Enums;

namespace TWP.Api.Core.DbEntities
{
    public class RollTableEntryDbEntity : Entity
    {
        /// <summary>
        /// Minimum roll value to select this entry
        /// </summary>
        public int MinRoll { get; set; }

        /// <summary>
        /// Maximum roll value to select this entry
        /// </summary>
        public int MaxRoll { get; set; }

        /// <summary>
        /// Type of entry (e.g., "result", "rollAgain", "linked", "multiRoll")
        /// </summary>
        public RollEntryEnum Type { get; set; }

        /// <summary>
        /// Main result or sentence for the entry, to be inserted into the SentenceTemplate
        /// </summary>
        public string ResultText { get; set; }


        //Navigation Property

        /// <summary>
        /// Foreign key to Parent RollTable to specify the table this entry belongs to
        /// </summary>
        public int RollTableId { get; set; }
        public RollTableDbEntity RollTable { get; set; } // Navigation property to RollTable

        /// <summary>
        /// Optional foreign key to another RollTable if this entry links to another table
        /// </summary>
        public int? LinkedTableId { get; set; }
        public RollTableDbEntity LinkedTable { get; set; }
    }
}
