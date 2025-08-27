using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class ActionDbEntityConfiguration : IEntityTypeConfiguration<ActionDbEntity>
    {
        public void Configure(EntityTypeBuilder<ActionDbEntity> builder)
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

            builder.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>() // Converts ActionTypeEnum to string in database
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            builder.Property(e => e.AttackBonus)
                .HasColumnName("attack_bonus");

            builder.Property(e => e.DamageDice)
                .HasColumnName("damage_dice")
                .HasConversion<string>() // Converts DiceTypeEnum to string in database
                .HasMaxLength(20);

            builder.Property(e => e.NumberDamageDice)
                .HasColumnName("number_damage_dice");

            builder.Property(e => e.DamageType)
                .HasColumnName("damage_type")
                .HasConversion<string>() // Converts DamageTypeEnum to string in database
                .HasMaxLength(50);

            builder.Property(e => e.LimitPerDay)
                .HasColumnName("limit_per_day");

            // Indexes
            builder.HasIndex(e => e.MonsterId)
                .HasDatabaseName("IX_action_monster_id");

            builder.HasIndex(e => e.Name)
                .HasDatabaseName("IX_action_name");

            builder.HasIndex(e => e.Type)
                .HasDatabaseName("IX_action_type");

            // Relationship
            builder.HasOne(e => e.Monster)
                .WithMany(m => m.Actions)
                .HasForeignKey(e => e.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}