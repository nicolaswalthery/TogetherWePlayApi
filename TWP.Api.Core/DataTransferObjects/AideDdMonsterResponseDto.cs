namespace TWP.Api.Core.DataTransferObjects
{
    public class AideDdMonsterResponseDto
    {
        public string Slug { get; set; } = "";
        public string Url { get; set; } = "";                 // href du <a>
        public string Name { get; set; } = "";
        public string? ChallengeRating { get; set; }          // 0.25, 14, etc. (null si "None")
        public string Gear { get; set; } = "";
        public string Senses { get; set; } = "";
        public string Languages { get; set; } = "";
        public string Skills { get; set; } = "";
        public string Resistances { get; set; } = "";
        public string Immunities { get; set; } = "";
        public string Initiative { get; set; } = "";
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

        // Characteristics (Strength, Dexterity, etc.)
        public string Strength { get; set; } = "";            // Example: "30"
        public string Dexterity { get; set; } = "";
        public string Constitution { get; set; } = "";
        public string Intelligence { get; set; } = "";
        public string Wisdom { get; set; } = "";
        public string Charisma { get; set; } = "";

        public string StrengthMod { get; set; } = "";            
        public string DexterityMod { get; set; } = "";
        public string ConstitutionMod { get; set; } = "";
        public string IntelligenceMod { get; set; } = "";
        public string WisdomMod { get; set; } = "";
        public string CharismaMod { get; set; } = "";

        // Saving Throws (StrengthSave, DexteritySave, etc.)
        public string StrengthSave { get; set; } = "";         // Example: "+10"
        public string DexteritySave { get; set; } = "";
        public string ConstitutionSave { get; set; } = "";
        public string IntelligenceSave { get; set; } = "";
        public string WisdomSave { get; set; } = "";
        public string CharismaSave { get; set; } = "";

        // Actions et Réactions ajoutées
        public List<string> Actions { get; set; }     // Extrait les actions du monstre
        public List<string> ReactionActions { get; set; }
        public List<string> BonusActions { get; set; }      // Extrait les Bonus actions du monstre
        public List<string> LegendaryActions { get; set; } // Extrait les Legendary actions du monstre

        // New property for Traits
        public List<string> Traits { get; set; } = new();     // List of traits for the monster
    }
}
