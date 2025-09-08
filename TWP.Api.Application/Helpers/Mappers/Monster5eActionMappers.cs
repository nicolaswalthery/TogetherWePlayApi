using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Helpers.Mappers
{
    public static class Monster5eActionMappers
    {
        /// <summary>
        /// Maps ActionDto to ActionDbEntity (for database operations)
        /// </summary>
        public static ActionDbEntity ToDbEntity(this ActionDto dto, Guid? monsterId = null)
        {
            if (dto == null) return null;

            return new ActionDbEntity
            {
                Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                MonsterId = monsterId ?? dto.MonsterId,
                Name = dto.Name,
                Type = ParseEnum<ActionTypeEnum>(dto.Type, ActionTypeEnum.Action),
                AttackType = ParseEnum<AttackTypeEnum>(dto.AttackType, AttackTypeEnum.None),
                Description = dto.Description,
                ShortRange = dto.ShortRange,
                LongRange = dto.LongRange,
                AttackBonus = dto.AttackBonus,
                DamageBonus = dto.DamageBonus,
                DamageDice = ParseNullableEnum<DiceTypeEnum>(dto.DamageDice),
                NumberDamageDice = dto.NumberDamageDice,
                DamageType = ParseNullableEnum<DamageTypeEnum>(dto.DamageType),
                LimitPerDay = dto.LimitPerDay,
                IsProhibitedForMinion = dto.IsProhibitedForMinion,
                actionTrigger = dto.ActionTrigger,
                advantageCondition = dto.AdvantageCondition,
                disadvantageCondition = dto.DisadvantageCondition
            };
        }

        /// <summary>
        /// Parses string to enum with default fallback
        /// </summary>
        private static TEnum ParseEnum<TEnum>(string value, TEnum defaultValue) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            // Try parsing as enum name
            if (Enum.TryParse<TEnum>(value, true, out var result))
                return result;

            // Try parsing as integer
            if (int.TryParse(value, out var intValue) && Enum.IsDefined(typeof(TEnum), intValue))
                return (TEnum)Enum.ToObject(typeof(TEnum), intValue);

            return defaultValue;
        }

        /// <summary>
        /// Parses string to nullable enum
        /// </summary>
        private static TEnum? ParseNullableEnum<TEnum>(string value) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Try parsing as enum name
            if (Enum.TryParse<TEnum>(value, true, out var result))
                return result;

            // Try parsing as integer
            if (int.TryParse(value, out var intValue) && Enum.IsDefined(typeof(TEnum), intValue))
                return (TEnum)Enum.ToObject(typeof(TEnum), intValue);

            return null;
        }
    }
}