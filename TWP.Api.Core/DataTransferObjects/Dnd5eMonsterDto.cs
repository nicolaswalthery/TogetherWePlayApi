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

        public int AC { get; set; }
        public int HP { get; set; }
        public int Initiative { get; set; }

        public int Walk { get; set; }
        public int Burrow { get; set; }
        public int Climb { get; set; }
        public int Fly { get; set; }
        public bool Hover { get; set; }
        public int Swim { get; set; }

        public int STR { get; set; }
        public int DEX { get; set; }
        public int CON { get; set; }
        public int INT { get; set; }
        public int WIS { get; set; }
        public int CHA { get; set; }

        public int STR_Save { get; set; }
        public int DEX_Save { get; set; }
        public int CON_Save { get; set; }
        public int INT_Save { get; set; }
        public int WIS_Save { get; set; }
        public int CHA_Save { get; set; }

        public string Proficient { get; set; }
        public string Expertise { get; set; }

        public string Vulnerabilities { get; set; }
        public string Slashing { get; set; }
        public string ImmunitiesConditions { get; set; }
        public string ImmunitiesDamage { get; set; }

        public int Blindsight { get; set; }
        public int Darkvision { get; set; }
        public int Truesight { get; set; }
        public int Tremorsense { get; set; }
        public int PassivePerception { get; set; }

        public string Languages { get; set; }
        public string CR { get; set; }
        public int XP { get; set; }
        public int PB { get; set; }

        public string Traits { get; set; }
        public int NumberOfLegendaryResistance { get; set; }
        public int NumberOfAtk { get; set; }

        public string Atk1Type { get; set; }
        public int Atk1Mod { get; set; }
        public string Atk1Range { get; set; }
        public int Atk1RangeShort { get; set; }
        public string Atk1Damage { get; set; }
        public string Atk1DamageType { get; set; }

        public string Atk2Type { get; set; }
        public int Atk2Mod { get; set; }
        public string Atk2Range { get; set; }
        public int Atk2RangeShort { get; set; }
        public string Atk2Damage { get; set; }
        public string Atk2DamageType { get; set; }

        public string Atk3Type { get; set; }
        public int Atk3Mod { get; set; }
        public string Atk3Range { get; set; }
        public int Atk3RangeShort { get; set; }
        public string Atk3Damage { get; set; }
        public string Atk3DamageType { get; set; }

        public string Atk4Type { get; set; }
        public int Atk4Mod { get; set; }
        public string Atk4Range { get; set; }
        public int Atk4RangeShort { get; set; }
        public string Atk4Damage { get; set; }
        public string Atk4DamageType { get; set; }

        public int SaveDC { get; set; }
        public string SavingThrow { get; set; }

        public string ActionNotes { get; set; }
        public string Ability { get; set; }

        public int SpellSaveDC { get; set; }
        public string SpellSavingThrows { get; set; }
        public string SpellAttack { get; set; }

        public string AtWill { get; set; }
        public string ThreePerDay { get; set; }
        public string TwoPerDay { get; set; }
        public string OnePerDay { get; set; }

        public string BonusAction { get; set; }
        public string Reaction { get; set; }
        public string Amount { get; set; }

        public int LegendaryActionSaveDC { get; set; }
        public string LegendaryActionSavingThrow { get; set; }
        public string LegendaryActions { get; set; }

        public string Lair { get; set; }
        public int LegendaryResistance { get; set; }
        public int LairSaveDC { get; set; }
        public string LairSavingThrows { get; set; }

        public string Other { get; set; }
    }


}
