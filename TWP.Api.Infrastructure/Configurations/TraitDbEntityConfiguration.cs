using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class TraitDbEntityConfiguration : IEntityTypeConfiguration<TraitDbEntity>
    {
        public void Configure(EntityTypeBuilder<TraitDbEntity> builder)
        {
            // Table name
            builder.ToTable("traits");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.MonsterId)
                .HasColumnName("monster_id")
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.DamageBonus)
                .HasColumnName("damage_bonus");

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

            builder.Property(e => e.IsOptional)
                .HasColumnName("is_optional")
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(e => e.MonsterId)
                .HasDatabaseName("IX_feature_monster_id");

            builder.HasIndex(e => e.Title)
                .HasDatabaseName("IX_feature_title");

            // Relationship
            builder.HasOne(e => e.Monster)
                .WithMany(m => m.Traits)
                .HasForeignKey(e => e.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}