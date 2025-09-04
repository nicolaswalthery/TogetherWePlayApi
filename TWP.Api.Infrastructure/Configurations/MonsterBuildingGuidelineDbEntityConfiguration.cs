using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for MonsterBuildingGuidelineDbEntity
    /// </summary>
    public class MonsterBuildingGuidelineDbEntityConfiguration : IEntityTypeConfiguration<MonsterBuildingGuidelineDbEntity>
    {
        public void Configure(EntityTypeBuilder<MonsterBuildingGuidelineDbEntity> builder)
        {
            // Table name
            builder.ToTable("monster_building_guidelines");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.CR)
                .HasColumnName("cr")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(e => e.CRNumeric)
                .HasColumnName("cr_numeric")
                .HasPrecision(5, 3) // Max 99.999
                .IsRequired();

            builder.Property(e => e.ProficiencyBonus)
                .HasColumnName("proficiency_bonus")
                .IsRequired();

            builder.Property(e => e.ArmorClass)
                .HasColumnName("armor_class")
                .IsRequired();

            builder.Property(e => e.MinHP)
                .HasColumnName("min_hp")
                .IsRequired();

            builder.Property(e => e.MaxHP)
                .HasColumnName("max_hp")
                .IsRequired();

            builder.Property(e => e.AverageHP)
                .HasColumnName("average_hp")
                .IsRequired();

            builder.Property(e => e.HPRange)
                .HasColumnName("hp_range")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.ToFHP)
                .HasColumnName("tof_hp")
                .HasMaxLength(50);

            builder.Property(e => e.AttackBonus)
                .HasColumnName("attack_bonus")
                .IsRequired();

            builder.Property(e => e.MultiAttackCount)
                .HasColumnName("multi_attack_count")
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.AverageDamagePerRound)
                .HasColumnName("average_damage_per_round")
                .IsRequired();

            builder.Property(e => e.TotalDamageAvg)
                .HasColumnName("total_damage_avg")
                .IsRequired();

            builder.Property(e => e.TotalDamageLegendaryAvg)
                .HasColumnName("total_damage_legendary_avg")
                .IsRequired();

            builder.Property(e => e.DamagePerRound)
                .HasColumnName("damage_per_round")
                .IsRequired();

            builder.Property(e => e.DamagePerRoundAlt)
                .HasColumnName("damage_per_round_alt")
                .IsRequired();

            builder.Property(e => e.SaveDC)
                .HasColumnName("save_dc")
                .IsRequired();

            builder.Property(e => e.InitiativeBonus)
                .HasColumnName("initiative_bonus")
                .IsRequired();

            builder.Property(e => e.ExperiencePoints)
                .HasColumnName("experience_points")
                .IsRequired();

            builder.Property(e => e.ExampleMonsters)
                .HasColumnName("example_monsters")
                .HasMaxLength(500);

            builder.Property(e => e.IsOfficial)
                .HasColumnName("is_official")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.Source)
                .HasColumnName("source")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasColumnType("text");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            builder.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(e => e.CR)
                .HasDatabaseName("IX_guideline_cr")
                .IsUnique(); // CR should be unique

            builder.HasIndex(e => e.CRNumeric)
                .HasDatabaseName("IX_guideline_cr_numeric");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_guideline_is_active");

            builder.HasIndex(e => new { e.IsActive, e.CRNumeric })
                .HasDatabaseName("IX_guideline_active_cr_numeric");
        }
    }
}