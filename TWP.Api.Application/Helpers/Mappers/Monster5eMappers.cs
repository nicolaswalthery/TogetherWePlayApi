using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.Helpers.Mappers
{
    public static class Monster5eMappers
    {
        // Mapper pour Monster5eDbEntity vers Monster5eDto
        public static Monster5eDto ToDto(this Monster5eDbEntity entity)
        {
            if (entity == null) return null;

            return new Monster5eDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Alignment = entity.Alignment?.ToString(), // Enum en string
                ChallengeRating = entity.ChallengeRating,
                Xp = entity.Xp,
                InitiativeBonus = entity.InitiativeBonus,
                Role = entity.Role?.ToString(), // Enum en string
                CreatureSize = entity.CreatureSize.ToString(),

                ArmorClass = entity.ArmorClass,
                MinionArmorClass = entity.MinionArmorClass,
                HitPoints = entity.HitPoints,
                HitDice = entity.HitDice,

                Speed = entity.Speed,
                Climb = entity.Climb,
                Swim = entity.Swim,
                Fly = entity.Fly,

                Strength = entity.Strength,
                Dexterity = entity.Dexterity,
                Constitution = entity.Constitution,
                Intelligence = entity.Intelligence,
                Wisdom = entity.Wisdom,
                Charisma = entity.Charisma,

                Skills = entity.GetSkills(), // JSON parsing
                DamageImmunities = entity.DamageImmunities,
                Senses = entity.Senses,
                Languages = entity.Languages,

                ConSavingThrow = entity.ConSavingThrow,
                DexSavingThrow = entity.DexSavingThrow,
                StrSavingThrow = entity.StrSavingThrow,
                WisSavingThrow = entity.WisSavingThrow,
                ChaSavingThrow = entity.ChaSavingThrow,
                IntSavingThrow = entity.IntSavingThrow,
                ProficiencyBonus = entity.ProficiencyBonus,

                Equipments = entity.GetEquipments(), // JSON parsing
                Habitats = entity.Habitats,
                CreatureType = entity.CreatureType,
                CreatureSubType = entity.CreatureSubType,
                MonsterGroup = entity.MonsterGroup,

                Manner = entity.Manner,
                Lore = entity.Lore,
                PageSource = entity.PageSource,
                Source = entity.Source,

                Actions = entity.Actions.Select(a => a.ToDto()).ToList(),
                Traits = entity.Traits.Select(t => t.ToDto()).ToList()
            };
        }

        // Mapper pour ActionDbEntity vers ActionDto
        public static ActionDto ToDto(this ActionDbEntity entity)
        {
            if (entity == null) return null;

            return new ActionDto
            {
                Name = entity.Name,
                Description = entity.Description,
                ShortRange = entity.ShortRange,
                LongRange = entity.LongRange,
                AttackBonus = entity.AttackBonus,
                DamageBonus = entity.DamageBonus,
                DamageDice = entity.DamageDice?.ToString(),
                NumberDamageDice = entity.NumberDamageDice,
                DamageType = entity.DamageType?.ToString(),
                LimitPerDay = entity.LimitPerDay,
                IsProhibitedForMinion = entity.IsProhibitedForMinion,
                ActionTrigger = entity.actionTrigger,
                AdvantageCondition = entity.advantageCondition,
                DisadvantageCondition = entity.disadvantageCondition
            };
        }

        // Mapper pour TraitDbEntity vers TraitDto
        public static TraitDto ToDto(this TraitDbEntity entity)
        {
            if (entity == null) return null;

            return new TraitDto
            {
                Title = entity.Title,
                Description = entity.Description,
                AttackBonus = entity.AttackBonus,
                DamageBonus = entity.DamageBonus,
                DamageDice = entity.DamageDice?.ToString(),
                NumberDamageDice = entity.NumberDamageDice,
                DamageType = entity.DamageType?.ToString(),
                TraitTrigger = entity.traitTrigger,
                AdvantageCondition = entity.advantageCondition,
                DisadvantageCondition = entity.disadvantageCondition,
                IsOptional = entity.IsOptional
            };
        }
    }

}
