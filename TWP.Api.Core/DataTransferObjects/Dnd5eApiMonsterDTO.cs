namespace TWP.Api.Core.DataTransferObjects
{
    public class Dnd5eApiMonsterDTO
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Alignment { get; set; }
        public List<ArmorClass> ArmorClass { get; set; }
        public int HitPoints { get; set; }
        public string HitDice { get; set; }
        public string HitPointsRoll { get; set; }
        public Speed Speed { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public List<Proficiency> Proficiencies { get; set; }
        public List<string> DamageVulnerabilities { get; set; }
        public List<string> DamageResistances { get; set; }
        public List<string> DamageImmunities { get; set; }
        public List<string> ConditionImmunities { get; set; }
        public Senses Senses { get; set; }
        public string Languages { get; set; }
        public string ChallengeRating { get; set; }
        public int ProficiencyBonus { get; set; }
        public int Xp { get; set; }
        public List<SpecialAbility> SpecialAbilities { get; set; }
        public List<ActionDTO> Actions { get; set; }
        public List<LegendaryAction> LegendaryActions { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Forms { get; set; }
        public List<string> Reactions { get; set; }
    }

    public class ArmorClass
    {
        public string Type { get; set; }
        public int Value { get; set; }
    }

    public class Speed
    {
        public string Walk { get; set; }
        public string Swim { get; set; }
    }

    public class Proficiency
    {
        public int Value { get; set; }
        public ProficiencyDetail ProficiencyDetail { get; set; }
    }

    public class ProficiencyDetail
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Senses
    {
        public string Darkvision { get; set; }
        public int PassivePerception { get; set; }
    }

    public class SpecialAbility
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<string> Damage { get; set; }
        public DC DC { get; set; }
    }

    public class DC
    {
        public DCType DCType { get; set; }
        public int DCValue { get; set; }
        public string SuccessType { get; set; }
    }

    public class DCType
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class ActionDTO
    {
        public List<string> Damage { get; set; }
        public string Name { get; set; }
        public string MultiattackType { get; set; }
        public string Desc { get; set; }
        public List<ActionDetail> Actions { get; set; }
        public int? AttackBonus { get; set; }
        public DC DC { get; set; }
    }

    public class ActionDetail
    {
        public string ActionName { get; set; }
        public int Count { get; set; }
        public string Type { get; set; }
    }

    public class LegendaryAction
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<string> Damage { get; set; }
    }


}


