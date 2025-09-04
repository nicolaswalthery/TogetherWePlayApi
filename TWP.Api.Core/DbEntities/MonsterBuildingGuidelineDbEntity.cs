namespace TWP.Api.Core.DbEntities
{
    /// <summary>
    /// Database entity for D&D 5e 2024 Monster Building Guidelines
    /// Based on Alphastream.org Monster Building Guidelines table
    /// </summary>
    public class MonsterBuildingGuidelineDbEntity : DbEntity
    {
        /// <summary>
        /// Challenge Rating as string (e.g., "1/4", "1", "20")
        /// </summary>
        public string CR { get; set; } = string.Empty;

        /// <summary>
        /// Numeric value of the CR for calculations and sorting
        /// </summary>
        public float CRNumeric { get; set; }

        /// <summary>
        /// Proficiency bonus for the CR
        /// </summary>
        public int ProficiencyBonus { get; set; }

        /// <summary>
        /// Base Armor Class
        /// </summary>
        public int ArmorClass { get; set; }

        /// <summary>
        /// Minimum HP for the CR range
        /// </summary>
        public int MinHP { get; set; }

        /// <summary>
        /// Maximum HP for the CR range
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// Published average HP
        /// </summary>
        public int AverageHP { get; set; }

        /// <summary>
        /// Published HP range as string (e.g., "8-22")
        /// </summary>
        public string HPRange { get; set; } = string.Empty;

        /// <summary>
        /// Tales of the Flame HP format (e.g., "3 (2-4)")
        /// </summary>
        public string? ToFHP { get; set; }

        /// <summary>
        /// Attack bonus to hit
        /// </summary>
        public int AttackBonus { get; set; }

        /// <summary>
        /// Number of attacks in multiattack
        /// </summary>
        public int MultiAttackCount { get; set; }

        /// <summary>
        /// Average damage per round
        /// </summary>
        public int AverageDamagePerRound { get; set; }

        /// <summary>
        /// Total Damage Average (normal)
        /// </summary>
        public int TotalDamageAvg { get; set; }

        /// <summary>
        /// Total Damage Average (with legendary actions)
        /// </summary>
        public int TotalDamageLegendaryAvg { get; set; }

        /// <summary>
        /// Damage per round (primary column)
        /// </summary>
        public int DamagePerRound { get; set; }

        /// <summary>
        /// Damage per round (alternative column)
        /// </summary>
        public int DamagePerRoundAlt { get; set; }

        /// <summary>
        /// Save Difficulty Class
        /// </summary>
        public int SaveDC { get; set; }

        /// <summary>
        /// Initiative bonus
        /// </summary>
        public int InitiativeBonus { get; set; }

        /// <summary>
        /// Experience points awarded for this CR
        /// </summary>
        public int ExperiencePoints { get; set; }

        /// <summary>
        /// Example monsters at this CR (comma separated)
        /// </summary>
        public string? ExampleMonsters { get; set; }

        /// <summary>
        /// Whether this is an official guideline or custom
        /// </summary>
        public bool IsOfficial { get; set; } = true;

        /// <summary>
        /// Source of the guideline (e.g., "D&D 5e 2024", "DMG", etc.)
        /// </summary>
        public string Source { get; set; } = "D&D 5e 2024 - Alphastream.org";

        /// <summary>
        /// Notes or additional information about this CR level
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// When this guideline was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When this guideline was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Whether this guideline is currently active/visible
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}