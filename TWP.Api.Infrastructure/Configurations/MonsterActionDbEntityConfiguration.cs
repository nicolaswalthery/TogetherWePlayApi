using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class MonsterActionDbEntityConfiguration : IEntityTypeConfiguration<MonsterActionDbEntity>
    {
        public void Configure(EntityTypeBuilder<MonsterActionDbEntity> builder)
        {
            // Table name
            builder.ToTable("actions");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.MonsterId)
                .HasColumnName("monster_id")
                .IsRequired();

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            builder.Property(e => e.AttackBonus)
                .HasColumnName("attack_bonus");

            builder.Property(e => e.Damage)
                .HasColumnName("damage")
                .HasMaxLength(100);

            builder.Property(e => e.DamageType)
                .HasColumnName("damageType")
                .HasMaxLength(50);

            builder.Property(e => e.LimitPerDay)
                .HasColumnName("limitPerDay");

            builder.Property(e => e.IsMovement)
                .HasColumnName("is_movement")
                .HasDefaultValue(false);

            builder.Property(e => e.IsAction)
                .HasColumnName("is_action")
                .HasDefaultValue(false);

            builder.Property(e => e.IsBonus)
                .HasColumnName("is_bonus")
                .HasDefaultValue(false);

            builder.Property(e => e.IsReaction)
                .HasColumnName("is_reaction")
                .HasDefaultValue(false);

            builder.Property(e => e.IsLegendary)
                .HasColumnName("is_legendary")
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(e => e.MonsterId)
                .HasDatabaseName("IX_monsters_action_monster_id");

            builder.HasIndex(e => e.Name)
                .HasDatabaseName("IX_monsters_action_name");

            builder.HasIndex(e => e.IsLegendary)
                .HasDatabaseName("IX_monsters_action_is_legendary");

            // Relationship
            builder.HasOne(e => e.Monster)
                .WithMany(m => m.Actions)
                .HasForeignKey(e => e.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}