using Microsoft.EntityFrameworkCore;
using TWP.Api.Core.DbEntities;
using TWP.Api.Infrastructure.Configurations;

public class DataContext : DbContext
{
    private readonly string _defaultSchema = "dbo";

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Monster5eDbEntity> Monster5eDbEntities { get; set; }


    // Configuration optionnelle du modèle
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(_defaultSchema);
        modelBuilder.ApplyConfiguration(new Monster5eDbEntityConfiguration());
    }
}