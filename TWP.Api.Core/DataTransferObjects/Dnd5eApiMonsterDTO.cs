using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TWP.Api.Core.DataTransferObjects
{
    public class Dnd5eApiMonsterDTO
    {
        /// <summary>
        /// A shorthand identifier for the monster (e.g. "aboleth").
        /// </summary>
        [JsonPropertyName("index")]
        public string? Index { get; set; }

        /// <summary>
        /// The display name of the monster.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The overall size category of the monster【613648048947610†L110-L114】.
        /// Possible values include Tiny, Small, Medium, Large, Huge and Gargantuan.
        /// </summary>
        [JsonPropertyName("size")]
        public string? Size { get; set; }

        /// <summary>
        /// The creature type (e.g. beast, dragon, undead, aberration, etc.).
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// A more specific sub‑type classification for the creature when present【613648048947610†L116-L123】.
        /// </summary>
        [JsonPropertyName("subtype")]
        public string? Subtype { get; set; }

        /// <summary>
        /// The alignment of the creature (e.g. "lawful evil").
        /// </summary>
        [JsonPropertyName("alignment")]
        public string? Alignment { get; set; }

        /// <summary>
        /// A collection of armour class entries detailing how the monster’s AC is derived【613648048947610†L128-L134】.
        /// </summary>
        [JsonPropertyName("armor_class")]
        public List<ArmorClass>? ArmorClass { get; set; }

        /// <summary>
        /// The creature’s average hit point total【613648048947610†L132-L146】.
        /// </summary>
        [JsonPropertyName("hit_points")]
        public int? HitPoints { get; set; }

        /// <summary>
        /// The hit dice formula used to generate hit points (e.g. "18d10").
        /// </summary>
        [JsonPropertyName("hit_dice")]
        public string? HitDice { get; set; }

        /// <summary>
        /// The full hit point roll formula including modifiers (e.g. "18d10+36").
        /// </summary>
        [JsonPropertyName("hit_points_roll")]
        public string? HitPointsRoll { get; set; }

        /// <summary>
        /// Object representing the monster’s various movement speeds【613648048947610†L867-L895】.
        /// </summary>
        [JsonPropertyName("speed")]
        public Speed? Speed { get; set; }

        /// <summary>
        /// Ability scores. See the schema for more information【613648048947610†L82-L104】.
        /// </summary>
        [JsonPropertyName("strength")]
        public int Strength { get; set; }

        [JsonPropertyName("dexterity")]
        public int Dexterity { get; set; }

        [JsonPropertyName("constitution")]
        public int Constitution { get; set; }

        [JsonPropertyName("intelligence")]
        public int Intelligence { get; set; }

        [JsonPropertyName("wisdom")]
        public int Wisdom { get; set; }

        [JsonPropertyName("charisma")]
        public int Charisma { get; set; }

        /// <summary>
        /// Skill or save proficiencies together with their associated modifiers【260544307443888†L4-L14】.
        /// </summary>
        [JsonPropertyName("proficiencies")]
        public List<Proficiency>? Proficiencies { get; set; }

        /// <summary>
        /// A list of damage types to which the monster is vulnerable.
        /// </summary>
        [JsonPropertyName("damage_vulnerabilities")]
        public List<string>? DamageVulnerabilities { get; set; }

        /// <summary>
        /// A list of damage types to which the monster is resistant.
        /// </summary>
        [JsonPropertyName("damage_resistances")]
        public List<string>? DamageResistances { get; set; }

        /// <summary>
        /// A list of damage types to which the monster is immune.
        /// </summary>
        [JsonPropertyName("damage_immunities")]
        public List<string>? DamageImmunities { get; set; }

        /// <summary>
        /// Conditions that the monster is immune to. Each entry references another resource.
        /// </summary>
        [JsonPropertyName("condition_immunities")]
        public List<ApiReference>? ConditionImmunities { get; set; }

        /// <summary>
        /// Sensory capabilities such as darkvision and passive perception【260544307443888†L15-L16】.
        /// </summary>
        [JsonPropertyName("senses")]
        public Senses? Senses { get; set; }

        /// <summary>
        /// Languages the monster can speak and/or understand【260544307443888†L16-L17】.
        /// </summary>
        [JsonPropertyName("languages")]
        public string? Languages { get; set; }

        /// <summary>
        /// The challenge rating which determines difficulty and experience value.
        /// May contain fractional values such as 0.25 or 13.0.
        /// </summary>
        [JsonProperty("challenge_rating")]
        public decimal ChallengeRating { get; set; }

        /// <summary>
        /// The proficiency bonus used for attack rolls and saving throws【260544307443888†L16-L17】.
        /// </summary>
        [JsonPropertyName("proficiency_bonus")]
        public int? ProficiencyBonus { get; set; }

        /// <summary>
        /// Experience points awarded for defeating the creature【613648048947610†L897-L900】.
        /// </summary>
        [JsonPropertyName("xp")]
        public int? Xp { get; set; }

        /// <summary>
        /// A list of special abilities the monster possesses. These are passive or triggered features【260544307443888†L18-L27】.
        /// </summary>
        [JsonPropertyName("special_abilities")]
        public List<SpecialAbility>? SpecialAbilities { get; set; }

        /// <summary>
        /// A list of standard actions the monster can take during combat【260544307443888†L28-L58】.
        /// </summary>
        [JsonPropertyName("actions")]
        public List<MonsterAction>? Actions { get; set; }

        /// <summary>
        /// A list of legendary actions available to the monster【260544307443888†L60-L66】.
        /// </summary>
        [JsonPropertyName("legendary_actions")]
        public List<MonsterAction>? LegendaryActions { get; set; }

        /// <summary>
        /// Optional reactions the monster may take. Each reaction typically includes a name, description and optional damage array.
        /// </summary>
        [JsonPropertyName("reactions")]
        public List<Reaction>? Reactions { get; set; }

        /// <summary>
        /// URL of an image representing the monster【613648048947610†L106-L109】.
        /// </summary>
        [JsonPropertyName("image")]
        public string? Image { get; set; }

        /// <summary>
        /// Relative URL of the monster resource itself【260544307443888†L60-L67】.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Timestamp representing when the monster entry was last updated.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Some monsters may include alternative form references. Each is an API reference.
        /// </summary>
        [JsonPropertyName("forms")]
        public List<ApiReference>? Forms { get; set; }

        /// <summary>
        /// A longer description of the monster when provided. The API may return multiple paragraphs.
        /// </summary>
        [JsonPropertyName("desc")]
        public List<string>? Description { get; set; }
    }

    /// <summary>
    /// Defines an armour class entry for a monster. A monster may have multiple AC sources
    /// such as natural armour or a base armor class with a shield.
    /// </summary>
    public class ArmorClass
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("value")]
        public int? Value { get; set; }

        /// <summary>
        /// Some monsters include a textual description of their armour class (e.g. "chain shirt and shield").
        /// </summary>
        [JsonPropertyName("armor_desc")]
        public string? ArmorDesc { get; set; }

        /// <summary>
        /// An optional bonus to the armour class value.
        /// </summary>
        [JsonPropertyName("bonus")]
        public int? Bonus { get; set; }
    }

    /// <summary>
    /// Represents the different speeds a creature can move.
    /// Missing properties are simply absent from the JSON when the creature lacks that speed【613648048947610†L867-L895】.
    /// </summary>
    public class Speed
    {
        [JsonPropertyName("walk")]
        public string? Walk { get; set; }

        [JsonPropertyName("burrow")]
        public string? Burrow { get; set; }

        [JsonPropertyName("climb")]
        public string? Climb { get; set; }

        [JsonPropertyName("fly")]
        public string? Fly { get; set; }

        [JsonPropertyName("swim")]
        public string? Swim { get; set; }
    }

    /// <summary>
    /// Represents a proficiency with an associated modifier.
    /// </summary>
    public class Proficiency
    {
        /// <summary>
        /// The numeric bonus or proficiency value【260544307443888†L4-L14】.
        /// </summary>
        [JsonPropertyName("value")]
        public int? Value { get; set; }

        /// <summary>
        /// A reference to the proficiency (either a skill or saving throw).
        /// </summary>
        [JsonPropertyName("proficiency")]
        public ApiReference? ProficiencyInfo { get; set; }
    }

    /// <summary>
    /// Generic reference to another API resource. This type appears in many nested objects.
    /// </summary>
    public class ApiReference
    {
        [JsonPropertyName("index")]
        public string? Index { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    /// <summary>
    /// Senses available to a monster, such as darkvision or blindsight【260544307443888†L15-L17】.
    /// </summary>
    public class Senses
    {
        [JsonPropertyName("darkvision")]
        public string? Darkvision { get; set; }

        [JsonPropertyName("blindsight")]
        public string? Blindsight { get; set; }

        [JsonPropertyName("tremorsense")]
        public string? Tremorsense { get; set; }

        [JsonPropertyName("truesight")]
        public string? Truesight { get; set; }

        [JsonPropertyName("passive_perception")]
        public int? PassivePerception { get; set; }
    }

    /// <summary>
    /// A special ability grants a passive or triggered trait to a monster【260544307443888†L18-L27】.
    /// </summary>
    public class SpecialAbility
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("desc")]
        public string? Description { get; set; }

        /// <summary>
        /// Optional damage details associated with the ability.
        /// </summary>
        [JsonPropertyName("damage")]
        public List<Damage>? Damage { get; set; }

        /// <summary>
        /// Difficulty class details if the ability forces saving throws.
        /// </summary>
        [JsonPropertyName("dc")]
        public Dc? Dc { get; set; }

        /// <summary>
        /// Spellcasting information for abilities that allow the monster to cast spells.
        /// </summary>
        [JsonPropertyName("spellcasting")]
        public Spellcasting? Spellcasting { get; set; }

        /// <summary>
        /// A usage pattern describing how many times per rest/day the ability can be used.
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    /// <summary>
    /// Represents an action a monster can perform during combat【260544307443888†L28-L58】.
    /// Legendary actions are also represented by this type.
    /// </summary>
    public class MonsterAction
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("desc")]
        public string? Description { get; set; }

        /// <summary>
        /// When the action is a multiattack, describes the type of multiattack (e.g. "actions").
        /// </summary>
        [JsonPropertyName("multiattack_type")]
        public string? MultiattackType { get; set; }

        /// <summary>
        /// A list of component actions used by multiattack entries.
        /// </summary>
        [JsonPropertyName("actions")]
        public List<MultiattackAction>? Actions { get; set; }

        /// <summary>
        /// The attack roll bonus for attack actions.
        /// </summary>
        [JsonPropertyName("attack_bonus")]
        public int? AttackBonus { get; set; }

        /// <summary>
        /// Damage dealt by the action, if applicable【260544307443888†L40-L49】.
        /// </summary>
        [JsonPropertyName("damage")]
        public List<Damage>? Damage { get; set; }

        /// <summary>
        /// Difficulty class for saving throw actions【260544307443888†L40-L43】.
        /// </summary>
        [JsonPropertyName("dc")]
        public Dc? Dc { get; set; }

        /// <summary>
        /// Optional set of options from which a player must choose when the monster acts.
        /// </summary>
        [JsonPropertyName("options")]
        public Choice? Options { get; set; }

        /// <summary>
        /// A usage limitation describing how often the action can be used.
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    /// <summary>
    /// Represents the simplified attack entry used within a multiattack action.
    /// </summary>
    public class MultiattackAction
    {
        [JsonPropertyName("action_name")]
        public string? ActionName { get; set; }

        [JsonPropertyName("count")]
        public string? Count { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("damage")]
        public List<Damage>? Damage { get; set; }

        [JsonPropertyName("dc")]
        public Dc? Dc { get; set; }
    }

    /// <summary>
    /// A reaction the monster can take outside of its turn. Reactions mirror the structure of actions.
    /// </summary>
    public class Reaction
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("desc")]
        public string? Description { get; set; }

        [JsonPropertyName("damage")]
        public List<Damage>? Damage { get; set; }

        [JsonPropertyName("dc")]
        public Dc? Dc { get; set; }

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    /// <summary>
    /// Represents a damage entry used in actions and special abilities【260544307443888†L40-L44】.
    /// </summary>
    public class Damage
    {
        /// <summary>
        /// Damage dice string such as "2d6+5"【260544307443888†L40-L44】.
        /// </summary>
        [JsonPropertyName("damage_dice")]
        public string? DamageDice { get; set; }

        /// <summary>
        /// A reference to the damage type (bludgeoning, piercing, acid, etc.)【260544307443888†L40-L44】.
        /// </summary>
        [JsonPropertyName("damage_type")]
        public ApiReference? DamageType { get; set; }

        /// <summary>
        /// Flat damage bonus that may be added to the damage dice.
        /// </summary>
        [JsonPropertyName("damage_bonus")]
        public int? DamageBonus { get; set; }
    }

    /// <summary>
    /// Details regarding a difficulty class (DC) for saving throws【260544307443888†L23-L24】.
    /// </summary>
    public class Dc
    {
        [JsonPropertyName("dc_type")]
        public ApiReference? DcType { get; set; }

        [JsonPropertyName("dc_value")]
        public int? DcValue { get; set; }

        [JsonPropertyName("success_type")]
        public string? SuccessType { get; set; }
    }

    /// <summary>
    /// A generic choice structure used to represent options that must be selected from.
    /// The API uses this for spell and action options.
    /// </summary>
    public class Choice
    {
        [JsonPropertyName("desc")]
        public string? Desc { get; set; }

        [JsonPropertyName("choose")]
        public int? Choose { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// The objects from which to choose. The structure of this property varies widely (it may be an array
        /// of API references, spells, actions, etc.), so it's represented as a JsonElement for flexibility.
        /// </summary>
        [JsonPropertyName("from")]
        public JsonElement From { get; set; }
    }

    /// <summary>
    /// Represents how often a special ability, action or spell can be used【613648048947610†L860-L870】.
    /// </summary>
    public class Usage
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("times")]
        public int? Times { get; set; }

        [JsonPropertyName("rest_types")]
        public List<string>? RestTypes { get; set; }
    }

    /// <summary>
    /// Represents the spellcasting block for monsters that can cast spells【613648048947610†L825-L852】.
    /// </summary>
    public class Spellcasting
    {
        [JsonPropertyName("level")]
        public int? Level { get; set; }

        /// <summary>
        /// The ability score used for spellcasting (usually INT, WIS or CHA).
        /// </summary>
        [JsonPropertyName("ability")]
        public ApiReference? Ability { get; set; }

        [JsonPropertyName("dc")]
        public int? Dc { get; set; }

        [JsonPropertyName("modifier")]
        public int? Modifier { get; set; }

        [JsonPropertyName("components_required")]
        public List<string>? ComponentsRequired { get; set; }

        /// <summary>
        /// School of magic for the spells (e.g. "evocation").
        /// </summary>
        [JsonPropertyName("school")]
        public string? School { get; set; }

        /// <summary>
        /// Spell slot counts by level. The property names correspond to spell levels ("1", "2", etc.).
        /// </summary>
        [JsonPropertyName("slots")]
        public Dictionary<string, int>? Slots { get; set; }

        /// <summary>
        /// A list of spell objects known to the monster. Each entry includes the spell name and level.
        /// </summary>
        [JsonPropertyName("spells")]
        public List<SpellInfo>? Spells { get; set; }

        /// <summary>
        /// Optional usage limitations for spellcasting (e.g. at will, per day).
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    /// <summary>
    /// Information about a specific spell a monster can cast.
    /// </summary>
    public class SpellInfo
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("level")]
        public int? Level { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Spellcasting usage details for per-day or recharge spells.
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }


}


