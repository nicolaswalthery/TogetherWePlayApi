using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.DataTransferObjects
{
    public class RollTableEntryDto
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

    }
}
