namespace TWP.Api.Core.DataTransferObjects
{
    public class AideDdMonsterResponseDto
    {
        public string Slug { get; set; } = "";
        public string Url { get; set; } = "";                 // href du <a>
        public string Name { get; set; } = "";
        public string? ChallengeRating { get; set; }          // 0.25, 14, etc. (null si "None")
        public string ChallengeRatingText { get; set; } = ""; // "1/4", "14", "None", etc.
        public string Type { get; set; } = "";
        public string Size { get; set; } = "";                // Gargantuan, Large, etc.
        public string? ArmorClass { get; set; }
        public string? HitPoints { get; set; }
        public string Speed { get; set; } = "";               // "20 ft., Fly 50 ft." (ou similaire)
        public string SpeedKeywords { get; set; } = "";       // "fly", "fly, swim", etc.
        public string Alignment { get; set; } = "";
        public bool Legendary { get; set; }
        public string Habitat { get; set; } = "";
        public string Source { get; set; } = "";
        public bool HasImage { get; set; }
        public List<string> Translations { get; set; } = new();

        // Actions et Réactions ajoutées
        public List<string> Actions { get; set; }     // Extrait les actions du monstre
        public List<string> ReactionActions { get; set; }
        public List<string> BonusActions { get; set; }      // Extrait les Bonus actions du monstre
        public List<string> LegendaryActions { get; set; } // Extrait les Legendary actions du monstre

        // New property for Traits
        public List<string> Traits { get; set; } = new();     // List of traits for the monster
    }
}
