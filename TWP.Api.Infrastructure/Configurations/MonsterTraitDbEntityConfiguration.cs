using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class MonsterTraitDbEntityConfiguration : IEntityTypeConfiguration<MonsterTraitDbEntity>
    {
        public void Configure(EntityTypeBuilder<MonsterTraitDbEntity> builder)
        {
            // Table name
            builder.ToTable("monsters_traits");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.MonsterId)
                .HasColumnName("monster_id")
                .IsRequired();

            builder.Property(e => e.TraitId)
                .HasColumnName("trait_id")
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.MonsterId)
                .HasDatabaseName("IX_monster_trait_monster_id");

            builder.HasIndex(e => e.TraitId)
                .HasDatabaseName("IX_monster_trait_trait_id");

            // Composite index for unique constraint
            builder.HasIndex(e => new { e.MonsterId, e.TraitId })
                .HasDatabaseName("IX_monster_trait_monster_trait")
                .IsUnique();

            // Relationships
            builder.HasOne(e => e.Monster)
                .WithMany(m => m.MonsterTraits)
                .HasForeignKey(e => e.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Trait)
                .WithMany(t => t.MonsterTraits)
                .HasForeignKey(e => e.TraitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}