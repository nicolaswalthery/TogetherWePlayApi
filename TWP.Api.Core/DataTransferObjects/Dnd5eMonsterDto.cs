using Common.Extensions;

namespace TWP.Api.Core.DataTransferObjects
{
    public class Dnd5eMonsterDto
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Alignment { get; set; }
        public string Habitat { get; set; }
        public string MainHabitat { get; set; }
        public string OtherHabitat { get; set; }
        public string Treasure { get; set; }

        public string AC { get; set; }
        public string HP { get; set; }
        public string Initiative { get; set; }

        public string Walk { get; set; }
        public string Burrow { get; set; }
        public string Climb { get; set; }
        public string Fly { get; set; }
        public string Hover { get; set; }
        public string Swim { get; set; }

        public string STR { get; set; }
        public string DEX { get; set; }
        public string CON { get; set; }
        public string INT { get; set; }
        public string WIS { get; set; }
        public string CHA { get; set; }

        public string STR_Save { get; set; }
        public string DEX_Save { get; set; }
        public string CON_Save { get; set; }
        public string INT_Save { get; set; }
        public string WIS_Save { get; set; }
        public string CHA_Save { get; set; }

        public string Proficient { get; set; }
        public string Expertise { get; set; }

        public string Vulnerabilities { get; set; }
        public string Slashing { get; set; }
        public string ImmunitiesConditions { get; set; }
        public string ImmunitiesDamage { get; set; }

        public string Blindsight { get; set; }
        public string Darkvision { get; set; }
        public string Truesight { get; set; }
        public string Tremorsense { get; set; }
        public string PassivePerception { get; set; }

        public string Languages { get; set; }
        public string CR { get; set; }
        public string XP { get; set; }
        public string PB { get; set; }

        public string Traits { get; set; }
        public string NumberOfLegendaryResistance { get; set; }
        public string NumberOfAtk { get; set; }

        public string Atk1Type { get; set; }
        public string Atk1Mod { get; set; }
        public string Atk1Range { get; set; }
        public string Atk1RangeShort { get; set; }
        public string Atk1Damage { get; set; }
        public string Atk1DamageType { get; set; }

        public string Atk2Type { get; set; }
        public string Atk2Mod { get; set; }
        public string Atk2Range { get; set; }
        public string Atk2RangeShort { get; set; }
        public string Atk2Damage { get; set; }
        public string Atk2DamageType { get; set; }

        public string Atk3Type { get; set; }
        public string Atk3Mod { get; set; }
        public string Atk3Range { get; set; }
        public string Atk3RangeShort { get; set; }
        public string Atk3Damage { get; set; }
        public string Atk3DamageType { get; set; }

        public string Atk4Type { get; set; }
        public string Atk4Mod { get; set; }
        public string Atk4Range { get; set; }
        public string Atk4RangeShort { get; set; }
        public string Atk4Damage { get; set; }
        public string Atk4DamageType { get; set; }

        public string SaveDC { get; set; }
        public string SavingThrow { get; set; }

        public string ActionNotes { get; set; }
        public string Ability { get; set; }

        public string SpellSaveDC { get; set; }
        public string SpellSavingThrows { get; set; }
        public string SpellAttack { get; set; }

        public string AtWill { get; set; }
        public string ThreePerDay { get; set; }
        public string TwoPerDay { get; set; }
        public string OnePerDay { get; set; }

        public string BonusAction { get; set; }
        public string Reaction { get; set; }
        public string Amount { get; set; }

        public string LegendaryActionSaveDC { get; set; }
        public string LegendaryActionSavingThrow { get; set; }
        public string LegendaryActions { get; set; }

        public string Lair { get; set; }
        public string LegendaryResistance { get; set; }
        public string LairSaveDC { get; set; }
        public string LairSavingThrows { get; set; }

        public string Other { get; set; }

        /// <summary>
        /// XP data without any commas.
        /// </summary>
        public int SanitizedXp 
            => XP.Replace(",", "").Trim().ToInt();
    }


}
