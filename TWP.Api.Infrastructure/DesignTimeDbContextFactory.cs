using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TWP.Api.Infrastructure
{
    public class DataContextDesignTimeFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            // Connection string hardcodée - MODIFIER SELON VOS BESOINS
            var connectionString = "Host=localhost;Port=5432;Database=[TWP][LOCAL];Username=postgres;Password=72hoignee;Include Error Detail=false";

            optionsBuilder.UseNpgsql(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}