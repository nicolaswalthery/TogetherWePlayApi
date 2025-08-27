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

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(2000);

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255);

            // Indexes
            builder.HasIndex(e => e.Title)
                .HasDatabaseName("IX_trait_title");

            // Relationships
            builder.HasMany(e => e.MonsterTraits)
                .WithOne(mt => mt.Trait)
                .HasForeignKey(mt => mt.TraitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}