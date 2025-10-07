using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class MonsterAideddDbEntityConfiguration : IEntityTypeConfiguration<MonsterAideddDbEntity>
    {
        public void Configure(EntityTypeBuilder<MonsterAideddDbEntity> builder)
        {
            // Table name
            builder.ToTable("aidedd_monsters");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.CR)
                .HasColumnName("cr")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.Type)
                .HasColumnName("type")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Size)
                .HasColumnName("size")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.AC)
                .HasColumnName("ac")
                .IsRequired();

            builder.Property(e => e.HP)
                .HasColumnName("hp")
                .IsRequired();

            builder.Property(e => e.Speed)
                .HasColumnName("speed")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Alignment)
                .HasColumnName("alignment")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Legendary)
                .HasColumnName("legendary")
                .HasMaxLength(50);

            builder.Property(e => e.Habitat)
                .HasColumnName("habitat")
                .HasMaxLength(500);

            builder.Property(e => e.Source)
                .HasColumnName("source")
                .HasMaxLength(255)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => e.Name)
                .HasDatabaseName("IX_monster_name");
        }
    }
}
