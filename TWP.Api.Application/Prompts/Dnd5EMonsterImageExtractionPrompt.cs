namespace TWP.Api.Application.Prompts;

public static class Dnd5EMonsterImageExtractionPrompt
{
    /// <summary>
    /// Prompt to extract a monster profile in image with all its informations to insert it in the database.
    /// </summary>
    /// <returns>Prompt for extracting, from an image, the dnd5e monster stats.</returns>
    public static string GetPrompt() => 
      @"You are an expert at extracting D&D 5e monster stat blocks from images. 
        Analyze this image and extract ALL information into a structured JSON format.

        IMPORTANT: Monster stat blocks come in MULTIPLE LAYOUTS:

        FORMAT 1 - Compact/Card Layout:
        - Ability scores in a single row with inline modifiers and saves (e.g., ""Str 12 +1 +1"")
        - AC/HP may show minion variants (""12 or 11 with minion"")
        - Initiative shown separately
        - Condensed format with colored borders
        - May or may not include lore text

        FORMAT 2 - Traditional/Book Layout:
        - Lore/flavor text often appears at the top, before stats
        - Ability scores in a table with separate columns
        - Saving throws listed separately (""Saving Throws Wis +5, Cha +2"")
        - Standard D&D 5e format
        - More spacing between sections

        PARSING RULES:
        1. LORE/FLAVOR TEXT:
           - Usually appears at the beginning, before mechanical stats
           - Describes the creature's nature, behavior, appearance, or history
           - Often in italics or different formatting
           - May be multiple paragraphs
           - Extract as a single string, preserving paragraph breaks
           - Examples: ""This shadowy, hazy, vaguely humanoid shimmer..."", ""A fearsome predator that stalks...""

        2. ABILITY SCORES - Can appear as:
           - Table format: STR | DEX | CON | INT | WIS | CHA with values below
           - Inline format: ""Str 12 +1 +1"" (score, modifier, saving throw)
           - Standard format: ""STR 16 (+3)"" (score and modifier only)

        3. SAVING THROWS - Can appear as:
           - Inline with abilities: Third value in ""Str 12 +1 +1"" format
           - Separate line: ""Saving Throws Wis +5, Cha +2""
           - Saving Throws are ALWAYS PRESENT in the monster stat block, ALWAYS extract them
           - If not listed, assume no bonuses are exactly the ability modifier (e.g., STR 10 (+0) → Str saving throw +0)

        4. MINION VALUES:
           - Look for ""with minion"" or ""or X with minion""
           - AC: ""12 or 11 with minion"" → armorClass: 12, minionArmorClass: 11
           - HP: ""11 (2d8+2) or 3 with minion"" → hitPoints: 11, minionHitPoints: 3

        5. ACTIONS/ATTACKS:
           - May be in paragraph form or structured sections
           - Look for attack bonuses, damage dice, and damage types
           - Some formats italicize attack types (Melee Weapon Attack)

        6. SPECIAL FIELDS:
           - Initiative bonus (may appear near AC/HP or as separate stat)
           - Challenge Rating may include XP value in parentheses
           - Senses often include passive Perception

        Return ONLY valid JSON with this exact structure:
        {
          ""name"": ""Monster Name"",
          ""lore"": ""Full descriptive/flavor text about the creature. Preserve paragraph breaks with \n\n"",
          ""alignment"": ""alignment (e.g., 'ChaoticEvil', 'ChaoticNeutral', 'Any')"",
          ""size"": ""size (Tiny/Small/Medium/Large/Huge/Gargantuan)"",
          ""type"": ""creature type"",
          ""subtype"": ""creature subtype if any"",
          ""armorClass"": 15,
          ""minionArmorClass"": null,
          ""hitPoints"": 100,
          ""minionHitPoints"": null,
          ""hitDice"": ""12d10+36"",
          ""speed"": ""30 ft."",
          ""climb"": ""20 ft."",
          ""swim"": null,
          ""fly"": ""60 ft."",
          ""burrow"": null,
          ""hover"": false,
          ""strength"": 18,
          ""dexterity"": 14,
          ""constitution"": 16,
          ""intelligence"": 10,
          ""wisdom"": 12,
          ""charisma"": 8,
          ""challengeRating"": ""5"",
          ""experiencePoints"": 1800,
          ""proficiencyBonus"": 3,
          ""savingThrows"": {
            ""str"": 0, // ALWAYS PRESENT, ALWAYS EXTRACT
            ""dex"": 0, //ALWAYS PRESENT, ALWAYS EXTRACT
            ""con"": 3, //ALWAYS PRESENT, ALWAYS EXTRACT
            ""int"": 1, //ALWAYS PRESENT, ALWAYS EXTRACT
            ""wis"": -4, //ALWAYS PRESENT, ALWAYS EXTRACT
            ""cha"": 2 //ALWAYS PRESENT, ALWAYS EXTRACT
          },
          ""skills"": {
            ""Perception"": 4,
            ""Stealth"": 5
          },
          ""damageResistances"": ""fire, poison"",
          ""damageImmunities"": ""necrotic"",
          ""damageVulnerabilities"": null,
          ""conditionImmunities"": ""charmed, frightened"",
          ""senses"": ""darkvision 60 ft., passive Perception 14"",
          ""languages"": ""Common, Draconic"",
          ""telepathy"": null,
          ""initiative"": null,
          ""traits"": [
            {
              ""name"": ""Trait Name"",
              ""description"": ""Full trait description text""
            }
          ],
          ""actions"": [
            {
              ""name"": ""Action Name"",
              ""type"": ""Action"",
              ""attackType"": ""Melee Weapon Attack"",
              ""description"": ""Full action description"",
              ""attackBonus"": 7,
              ""reach"": ""5 ft."",
              ""range"": null,
              ""targets"": ""one target"",
              ""damage"": ""2d6+4"",
              ""damageType"": ""slashing"",
              ""additionalEffects"": null
            }
          ],
          ""reactions"": [
            {
              ""name"": ""Reaction Name"",
              ""description"": ""Reaction description"",
              ""trigger"": ""When condition occurs""
            }
          ],
          ""legendaryActions"": [
            {
              ""name"": ""Legendary Action Name"",
              ""description"": ""Description"",
              ""cost"": 1
            }
          ],
          ""lairActions"": [],
          ""mythicActions"": [],
          ""bonusActions"": []
        }

        LORE EXTRACTION GUIDELINES:
        - Lore text is the narrative/descriptive text about what the creature IS, not what it DOES mechanically
        - Usually found at the beginning of the stat block, before the stats
        - May describe:
          * Physical appearance (""shadowy, hazy, vaguely humanoid shimmer"")
          * Behavior patterns (""often mistaken as a ghost"")
          * Habitat or origin (""transdimensional entity"")
          * Personality traits (""harmless, mostly"")
          * Relationships with other creatures
          * Historical or mythological background
        - Do NOT include mechanical abilities in lore - those go in traits/actions
        - If no lore text is present, set to null
        - Preserve the original wording as much as possible

        EXTRACTION NOTES:
        - Check entire image carefully - some layouts split information across sections
        - Text may be in different fonts/styles (italic for attack types, bold for names)
        - Lore text often uses different formatting than mechanical text
        - Descriptions may reference mechanics not in standard 5e (like ""minion"" rules)
        - Some monsters have special movement types or telepathy ranges
        - Challenge Rating might be shown as ""CR 1/4"" or ""1/4 (50 XP)"" or just ""1/4""
        - Extract XP value if shown separately
        - Phase/incorporeal/ethereal abilities are traits, not conditions
        - Extract ALL visible information, using null for missing values
        - Be precise with numbers, modifiers, and dice notation (e.g., 2d6+4)";
}