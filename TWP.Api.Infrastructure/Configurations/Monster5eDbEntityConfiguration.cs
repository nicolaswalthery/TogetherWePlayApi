using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class Monster5eDbEntityConfiguration : IEntityTypeConfiguration<Monster5eDbEntity>
    {
        public void Configure(EntityTypeBuilder<Monster5eDbEntity> builder)
        {
            // Table name
            builder.ToTable("monsters");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Alignment)
                .HasColumnName("alignment")
                .HasConversion<string>() // Converts enum to string in database
                .HasMaxLength(50);

            builder.Property(e => e.ChallengeRating)
                .HasColumnName("challenge_rating")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(e => e.Xp)
                .HasColumnName("xp")
                .IsRequired();

            builder.Property(e => e.InitiativeBonus)
                .HasColumnName("initiative_bonus")
                .IsRequired();

            builder.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion<string>() // Converts enum to string in database
                .HasMaxLength(50);

            // Defense Properties
            builder.Property(e => e.ArmorClass)
                .HasColumnName("armor_class")
                .IsRequired();

            builder.Property(e => e.MinionArmorClass)
                .HasColumnName("minion_armor_class");

            builder.Property(e => e.HitPoints)
                .HasColumnName("hit_points")
                .IsRequired();

            builder.Property(e => e.HitDice)
                .HasColumnName("hit_dice");

            builder.Property(e => e.CreatureSize)
                .HasColumnName("creature_size")
                .HasConversion<string>() // Converts enum to string in database
                .HasMaxLength(20)
                .IsRequired();

            // Movement Properties
            builder.Property(e => e.Speed)
                .HasColumnName("speed")
                .HasMaxLength(100);

            builder.Property(e => e.Climb)
                .HasColumnName("climb")
                .HasMaxLength(50);

            builder.Property(e => e.Swim)
                .HasColumnName("swim")
                .HasMaxLength(50);

            builder.Property(e => e.Fly)
                .HasColumnName("fly")
                .HasMaxLength(50);

            // Ability Scores
            builder.Property(e => e.Strength)
                .HasColumnName("strength");

            builder.Property(e => e.Dexterity)
                .HasColumnName("dexterity");

            builder.Property(e => e.Constitution)
                .HasColumnName("constitution");

            builder.Property(e => e.Intelligence)
                .HasColumnName("intelligence");

            builder.Property(e => e.Wisdom)
                .HasColumnName("wisdom");

            builder.Property(e => e.Charisma)
                .HasColumnName("charisma");

            // Skills and Immunities
            builder.Property(e => e.Skills)
                .HasColumnName("skills")
                .HasColumnType("jsonb"); // PostgreSQL JSON type

            builder.Property(e => e.DamageImmunities)
                .HasColumnName("damage_immunities")
                .HasMaxLength(500);

            builder.Property(e => e.DamageResistances)
                .HasColumnName("damage_resistances")
                .HasMaxLength(500);

            builder.Property(e => e.Senses)
                .HasColumnName("senses")
                .HasMaxLength(500);

            builder.Property(e => e.Languages)
                .HasColumnName("languages")
                .HasMaxLength(500);

            // Saving Throws
            builder.Property(e => e.ConSavingThrow)
                .HasColumnName("con_saving_throw");

            builder.Property(e => e.DexSavingThrow)
                .HasColumnName("dex_saving_throw");

            builder.Property(e => e.StrSavingThrow)
                .HasColumnName("str_saving_throw");

            builder.Property(e => e.WisSavingThrow)
                .HasColumnName("wis_saving_throw");

            builder.Property(e => e.ChaSavingThrow)
                .HasColumnName("cha_saving_throw");

            builder.Property(e => e.IntSavingThrow)
                .HasColumnName("int_saving_throw");

            builder.Property(e => e.ProficiencyBonus)
                .HasColumnName("proficiency_bonus");

            // Equipment and Habitat
            builder.Property(e => e.Equipments)
                .HasColumnName("equipments")
                .HasColumnType("jsonb"); // PostgreSQL JSON type

            builder.Property(e => e.Habitats)
                .HasColumnName("habitats")
                .HasMaxLength(500);

            builder.Property(e => e.CreatureType)
                .HasColumnName("creature_type")
                .HasMaxLength(100);

            builder.Property(e => e.CreatureSubType)
                .HasColumnName("creature_sub_type")
                .HasMaxLength(100);

            builder.Property(e => e.MonsterGroup)
                .HasColumnName("monster_group")
                .HasMaxLength(100);

            // Lore Properties
            builder.Property(e => e.Manner)
                .HasColumnName("manner")
                .HasMaxLength(1000);

            builder.Property(e => e.Lore)
                .HasColumnName("lore")
                .HasColumnType("jsonb"); // PostgreSQL JSON type

            builder.Property(e => e.PageSource)
                .HasColumnName("page_source")
                .IsRequired();

            builder.Property(e => e.Source)
                .HasColumnName("source")
                .HasMaxLength(255)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.Name)
                .HasDatabaseName("IX_monster_name");

            builder.HasIndex(e => e.ChallengeRating)
                .HasDatabaseName("IX_monster_challenge_rating");

            builder.HasIndex(e => e.CreatureType)
                .HasDatabaseName("IX_monster_creature_type");

            builder.HasIndex(e => e.MonsterGroup)
                .HasDatabaseName("IX_monster_group");

            // Relationships
            builder.HasMany(e => e.Actions)
                .WithOne(a => a.Monster)
                .HasForeignKey(a => a.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Traits)
                .WithOne(mt => mt.Monster)
                .HasForeignKey(mt => mt.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Symbarum5e)
                .WithOne(s => s.Monster)
                .HasForeignKey<Symbarum5eDbEntity>(s => s.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}