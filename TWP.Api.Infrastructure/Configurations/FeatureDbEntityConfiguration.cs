using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class FeatureDbEntityConfiguration : IEntityTypeConfiguration<FeatureDbEntity>
    {
        public void Configure(EntityTypeBuilder<FeatureDbEntity> builder)
        {
            // Table name
            builder.ToTable("feature");

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
                .HasMaxLength(2000);

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255);

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
                .WithMany(m => m.Features)
                .HasForeignKey(e => e.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}