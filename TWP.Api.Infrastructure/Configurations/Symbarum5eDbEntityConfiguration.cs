using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class Symbarum5eDbEntityConfiguration : IEntityTypeConfiguration<Symbarum5eDbEntity>
    {
        public void Configure(EntityTypeBuilder<Symbarum5eDbEntity> builder)
        {
            // Table name
            builder.ToTable("symbarum5es");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.MonsterId)
                .HasColumnName("monster_id")
                .IsRequired();

            builder.Property(e => e.Shadow)
                .HasColumnName("shadow")
                .HasMaxLength(1500)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.MonsterId)
                .HasDatabaseName("IX_symbarum5e_monster_id")
                .IsUnique(); // One-to-One relationship

            // Relationship
            builder.HasOne(e => e.Monster)
                .WithOne(m => m.Symbarum5e)
                .HasForeignKey<Symbarum5eDbEntity>(e => e.MonsterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}