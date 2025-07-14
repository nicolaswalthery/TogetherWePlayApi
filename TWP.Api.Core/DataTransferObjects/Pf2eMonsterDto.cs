namespace TWP.Api.Core.DataTransferObjects
{
    public class Pf2eMonsterDto
    {
        public string _id { get; set; }
        public string img { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<ItemDto> items { get; set; }
        public EagleSystemDto system { get; set; }
    }

    public class ItemDto
    {
        public string _id { get; set; }
        public string img { get; set; }
        public string name { get; set; }
        public int sort { get; set; }
        public string type { get; set; }
        public ItemSystemDto system { get; set; }
    }

    public class ItemSystemDto
    {
        public AttackDto attack { get; set; }
        public AttackEffectsDto attackEffects { get; set; }
        public BonusDto bonus { get; set; }
        public Dictionary<string, DamageRollDto> damageRolls { get; set; }
        public DescriptionDto description { get; set; }
        public PublicationDto publication { get; set; }
        public List<object> rules { get; set; }
        public string slug { get; set; }
        public TraitsDto traits { get; set; }
        public WeaponTypeDto weaponType { get; set; }
        public ActionTypeDto actionType { get; set; }
        public ActionsDto actions { get; set; }
        public string category { get; set; }
    }

    public class AttackDto
    {
        public string value { get; set; }
    }

    public class AttackEffectsDto
    {
        public string custom { get; set; }
        public List<string> value { get; set; }
    }

    public class BonusDto
    {
        public int value { get; set; }
    }

    public class DamageRollDto
    {
        public string damage { get; set; }
        public string damageType { get; set; }
    }

    public class DescriptionDto
    {
        public string value { get; set; }
    }

    public class PublicationDto
    {
        public string license { get; set; }
        public bool remaster { get; set; }
        public string title { get; set; }
    }

    public class TraitsDto
    {
        public string rarity { get; set; }
        public List<string> value { get; set; }
    }

    public class WeaponTypeDto
    {
        public string value { get; set; }
    }

    public class ActionTypeDto
    {
        public string value { get; set; }
    }

    public class ActionsDto
    {
        public int? value { get; set; }
    }

    public class EagleSystemDto
    {
        public AbilityScoresDto abilities { get; set; }
        public AttributesDto attributes { get; set; }
        public DetailsDto details { get; set; }
        public InitiativeDto initiative { get; set; }
        public PerceptionDto perception { get; set; }
        public SavesDto saves { get; set; }
        public Dictionary<string, SkillDto> skills { get; set; }
        public TraitsDto traits { get; set; }
    }

    public class AbilityScoresDto
    {
        public ModifierDto str { get; set; }
        public ModifierDto dex { get; set; }
        public ModifierDto con { get; set; }
        public ModifierDto string_ { get; set; }
        public ModifierDto wis { get; set; }
        public ModifierDto cha { get; set; }
    }

    public class ModifierDto
    {
        public int mod { get; set; }
    }

    public class AttributesDto
    {
        public AcDto ac { get; set; }
        public HpDto hp { get; set; }
        public SpeedDto speed { get; set; }
        public AllSavesDto allSaves { get; set; }
    }

    public class AcDto
    {
        public int value { get; set; }
        public string details { get; set; }
    }

    public class HpDto
    {
        public int value { get; set; }
        public int max { get; set; }
        public int temp { get; set; }
    }

    public class SpeedDto
    {
        public int? value { get; set; }
        public List<OtherSpeedDto> otherSpeeds { get; set; }
    }

    public class OtherSpeedDto
    {
        public string type { get; set; }
        public int? value { get; set; }
    }

    public class AllSavesDto
    {
        public string value { get; set; }
    }

    public class PerceptionDto
    {
        public string details { get; set; }
        public int mod { get; set; }
        public List<SenseDto> senses { get; set; }
    }

    public class SenseDto
    {
        public string type { get; set; }
    }

    public class InitiativeDto
    {
        public string statistic { get; set; }
    }

    public class SavesDto
    {
        public SaveDto fortitude { get; set; }
        public SaveDto reflex { get; set; }
        public SaveDto will { get; set; }
    }

    public class SaveDto
    {
        public string saveDetail { get; set; }
        public int value { get; set; }
    }

    public class SkillDto
    {
        public int @base { get; set; }
    }

    public class DetailsDto
    {
        public string blurb { get; set; }
        public string publicNotes { get; set; }
        public string privateNotes { get; set; }
        public LevelDto level { get; set; } 
        public LanguagesDto languages { get; set; }
        public PublicationDto publication { get; set; }
    }

    public class LevelDto
    {
        public int value { get; set; }
    }

    public class LanguagesDto
    {
        public string details { get; set; }
        public List<string> value { get; set; }
    }
}

