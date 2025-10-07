using TWP.Api.Core.Enums;

namespace TWP.Api.Core.DataTransferObjects
{
    /// <summary>
    /// Represents the monster building guidelines for D&D 5e 2024
    /// Based on Alphastream.org Monster Building Guidelines table
    /// </summary>
    public class MonsterBuildingGuidelineDto
    {
        /// <summary>
        /// Challenge Rating as string (e.g., "1/4", "1", "20")
        /// </summary>
        public string CR { get; set; } = string.Empty;

        /// <summary>
        /// Proficiency bonus for the CR
        /// </summary>
        public int ProfBonus { get; set; }

        /// <summary>
        /// Base Armor Class
        /// </summary>
        public int AC { get; set; }

        /// <summary>
        /// Published HP range (e.g., "8-22")
        /// </summary>
        public string PubHPRange { get; set; } = string.Empty;

        /// <summary>
        /// Published average HP
        /// </summary>
        public int PubAvgHP { get; set; }

        /// <summary>
        /// Tales of the Flame HP range format (e.g., "3 (2-4)")
        /// </summary>
        public string ToFHP { get; set; } = string.Empty;

        /// <summary>
        /// Attack bonus to hit
        /// </summary>
        public int AttackBonus { get; set; }

        /// <summary>
        /// Number of attacks in multiattack
        /// </summary>
        public int MultiAttacks { get; set; }

        /// <summary>
        /// Average damage per round
        /// </summary>
        public int AvgDmgPerRound { get; set; }

        /// <summary>
        /// Total Damage Average
        /// </summary>
        public int TDAvg { get; set; }

        /// <summary>
        /// Total Damage Legendary Average
        /// </summary>
        public int TDLegAvg { get; set; }

        /// <summary>
        /// Damage per round (primary)
        /// </summary>
        public int DmgPerRound { get; set; }

        /// <summary>
        /// Damage per round (alternative)
        /// </summary>
        public int DmgPerRoundAlt { get; set; }

        /// <summary>
        /// Save Difficulty Class
        /// </summary>
        public int SaveDC { get; set; }

        /// <summary>
        /// Initiative bonus
        /// </summary>
        public int Initiative { get; set; }

        /// <summary>
        /// Example monsters at this CR
        /// </summary>
        public string ExampleMonster { get; set; } = string.Empty;

        /// <summary>
        /// Experience points awarded
        /// </summary>
        public int XP { get; set; }

        /// <summary>
        /// Numeric value of the CR for calculations
        /// </summary>
        public float CRNumeric { get; set; }

        /// <summary>
        /// Minimum HP for the CR range
        /// </summary>
        public int MinHP { get; set; }

        /// <summary>
        /// Maximum HP for the CR range
        /// </summary>
        public int MaxHP { get; set; }
    }
}