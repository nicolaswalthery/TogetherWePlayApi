using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Configurations
{
    public class Monster5eDbEntityConfiguration : IEntityTypeConfiguration<Monster5eDbEntity>
    {
        public void Configure(EntityTypeBuilder<Monster5eDbEntity> builder)
        {
            
        }
    }
}
