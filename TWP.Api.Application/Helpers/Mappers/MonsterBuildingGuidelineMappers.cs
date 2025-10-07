using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.Helpers.Mappers
{
    /// <summary>
    /// Mapper for Monster Building Guidelines between DbEntity and DTOs
    /// </summary>
    public static class MonsterBuildingGuidelineMappers
    {
        /// <summary>
        /// Convert DbEntity to DTO
        /// </summary>
        public static MonsterBuildingGuidelineDto ToDto(this MonsterBuildingGuidelineDbEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new MonsterBuildingGuidelineDto
            {
                CR = entity.CR,
                CRNumeric = entity.CRNumeric,
                ProfBonus = entity.ProficiencyBonus,
                AC = entity.ArmorClass,
                MinHP = entity.MinHP,
                MaxHP = entity.MaxHP,
                PubAvgHP = entity.AverageHP,
                PubHPRange = entity.HPRange,
                ToFHP = entity.ToFHP ?? string.Empty,
                AttackBonus = entity.AttackBonus,
                MultiAttacks = entity.MultiAttackCount,
                AvgDmgPerRound = entity.AverageDamagePerRound,
                TDAvg = entity.TotalDamageAvg,
                TDLegAvg = entity.TotalDamageLegendaryAvg,
                DmgPerRound = entity.DamagePerRound,
                DmgPerRoundAlt = entity.DamagePerRoundAlt,
                SaveDC = entity.SaveDC,
                Initiative = entity.InitiativeBonus,
                XP = entity.ExperiencePoints,
                ExampleMonster = entity.ExampleMonsters ?? string.Empty
            };
        }

        /// <summary>
        /// Convert DTO to DbEntity
        /// </summary>
        public static MonsterBuildingGuidelineDbEntity ToEntity(this MonsterBuildingGuidelineDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new MonsterBuildingGuidelineDbEntity
            {
                Id = Guid.NewGuid(),
                CR = dto.CR,
                CRNumeric = dto.CRNumeric,
                ProficiencyBonus = dto.ProfBonus,
                ArmorClass = dto.AC,
                MinHP = dto.MinHP,
                MaxHP = dto.MaxHP,
                AverageHP = dto.PubAvgHP,
                HPRange = dto.PubHPRange,
                ToFHP = string.IsNullOrWhiteSpace(dto.ToFHP) ? null : dto.ToFHP,
                AttackBonus = dto.AttackBonus,
                MultiAttackCount = dto.MultiAttacks,
                AverageDamagePerRound = dto.AvgDmgPerRound,
                TotalDamageAvg = dto.TDAvg,
                TotalDamageLegendaryAvg = dto.TDLegAvg,
                DamagePerRound = dto.DmgPerRound,
                DamagePerRoundAlt = dto.DmgPerRoundAlt,
                SaveDC = dto.SaveDC,
                InitiativeBonus = dto.Initiative,
                ExperiencePoints = dto.XP,
                ExampleMonsters = string.IsNullOrWhiteSpace(dto.ExampleMonster) ? null : dto.ExampleMonster,
                IsOfficial = true,
                Source = "D&D 5e 2024 - Alphastream.org",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        /// <summary>
        /// Update an existing entity with DTO values
        /// </summary>
        public static void UpdateFromDto(this MonsterBuildingGuidelineDbEntity entity, MonsterBuildingGuidelineDto dto)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            entity.CR = dto.CR;
            entity.CRNumeric = dto.CRNumeric;
            entity.ProficiencyBonus = dto.ProfBonus;
            entity.ArmorClass = dto.AC;
            entity.MinHP = dto.MinHP;
            entity.MaxHP = dto.MaxHP;
            entity.AverageHP = dto.PubAvgHP;
            entity.HPRange = dto.PubHPRange;
            entity.ToFHP = string.IsNullOrWhiteSpace(dto.ToFHP) ? null : dto.ToFHP;
            entity.AttackBonus = dto.AttackBonus;
            entity.MultiAttackCount = dto.MultiAttacks;
            entity.AverageDamagePerRound = dto.AvgDmgPerRound;
            entity.TotalDamageAvg = dto.TDAvg;
            entity.TotalDamageLegendaryAvg = dto.TDLegAvg;
            entity.DamagePerRound = dto.DmgPerRound;
            entity.DamagePerRoundAlt = dto.DmgPerRoundAlt;
            entity.SaveDC = dto.SaveDC;
            entity.InitiativeBonus = dto.Initiative;
            entity.ExperiencePoints = dto.XP;
            entity.ExampleMonsters = string.IsNullOrWhiteSpace(dto.ExampleMonster) ? null : dto.ExampleMonster;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Convert a list of entities to DTOs
        /// </summary>
        public static List<MonsterBuildingGuidelineDto> ToDto(this IEnumerable<MonsterBuildingGuidelineDbEntity> entities)
        {
            return entities?.Select(e => e.ToDto()).ToList() ?? new List<MonsterBuildingGuidelineDto>();
        }

        /// <summary>
        /// Convert a list of DTOs to entities
        /// </summary>
        public static List<MonsterBuildingGuidelineDbEntity> ToEntity(this IEnumerable<MonsterBuildingGuidelineDto> dtos)
        {
            return dtos?.Select(d => d.ToEntity()).ToList() ?? new List<MonsterBuildingGuidelineDbEntity>();
        }
    }
}