using System.Text.Json;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.Helpers.Mappers
{
    public static class Monster5eMappers
    {
        public static Monster5eDto ToDto(this Monster5eDbEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new Monster5eDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Alignment = entity.Alignment?.ToString(),
                ChallengeRating = entity.ChallengeRating,
                Cr = entity.Cr,
                CrInLair = entity.CrInLair,
                Xp = entity.Xp,
                InitiativeBonus = entity.InitiativeBonus,
                Role = entity.Role?.ToString(),
                CreatureSize = entity.CreatureSize.ToString(),

                // Defense
                ArmorClass = entity.ArmorClass,
                MinionArmorClass = entity.MinionArmorClass,
                HitPoints = entity.HitPoints,
                HitDice = entity.HitDice,

                // Movement
                Speed = entity.Speed,
                Climb = entity.Climb,
                Swim = entity.Swim,
                Fly = entity.Fly,

                // Abilities
                Strength = entity.Strength,
                Dexterity = entity.Dexterity,
                Constitution = entity.Constitution,
                Intelligence = entity.Intelligence,
                Wisdom = entity.Wisdom,
                Charisma = entity.Charisma,

                // Skills & Immunities
                Skills = ParseJsonToDictionary(entity.Skills),
                DamageImmunities = entity.DamageImmunities,
                DamageResistances = entity.DamageResistances,
                Senses = entity.Senses,
                Languages = entity.Languages,

                // Saving Throws
                ConSavingThrow = entity.ConSavingThrow,
                DexSavingThrow = entity.DexSavingThrow,
                StrSavingThrow = entity.StrSavingThrow,
                WisSavingThrow = entity.WisSavingThrow,
                ChaSavingThrow = entity.ChaSavingThrow,
                IntSavingThrow = entity.IntSavingThrow,
                ProficiencyBonus = entity.ProficiencyBonus,

                // Equipment & Habitat
                Equipments = ParseJsonToList(entity.Equipments),
                Habitats = entity.Habitats,
                CreatureType = entity.CreatureType,
                CreatureSubType = entity.CreatureSubType,
                MonsterGroup = entity.MonsterGroup,

                // Lore
                Manner = entity.Manner,
                Lore = entity.Lore,
                PageSource = entity.PageSource,
                Source = entity.Source,

                // Related entities
                Actions = entity.Actions?.Select(a => a.ToDto()).ToList() ?? new List<ActionDto>(),
                Traits = entity.Traits?.Select(t => t.ToDto()).ToList() ?? new List<TraitDto>(),
                Symbarum5e = entity.Symbarum5e?.ToDto()
            };
        }

        public static ActionDto ToDto(this ActionDbEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ActionDto
            {
                Id = entity.Id,
                MonsterId = entity.MonsterId,
                Name = entity.Name,
                Type = entity.Type.ToString(),
                AttackType = entity.AttackType.ToString(),
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

        public static TraitDto ToDto(this TraitDbEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new TraitDto
            {
                Id = entity.Id,
                MonsterId = entity.MonsterId,
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

        public static Symbarum5eDto ToDto(this Symbarum5eDbEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new Symbarum5eDto
            {
                Id = entity.Id,
                MonsterId = entity.MonsterId,
                Shadow = entity.Shadow
            };
        }

        // Helper method for batch conversion
        public static List<Monster5eDto> ToDto(this IEnumerable<Monster5eDbEntity> entities)
        {
            return entities?.Select(e => e.ToDto()).ToList() ?? new List<Monster5eDto>();
        }

        // Private helper methods for JSON parsing
        private static Dictionary<string, object>? ParseJsonToDictionary(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            }
            catch
            {
                return null;
            }
        }

        private static List<string>? ParseJsonToList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
