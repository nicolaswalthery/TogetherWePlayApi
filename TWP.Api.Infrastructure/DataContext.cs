using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TWP.Api.Core.DbEntities;
using TWP.Api.Infrastructure.Configurations;

public class DataContext : DbContext
{
    private readonly string _defaultPostgresSchema = "public";
    private readonly ILogger<DataContext>? _logger;
    private readonly IWebHostEnvironment? _environment;

    public DataContext(
        DbContextOptions<DataContext> options,
        ILogger<DataContext>? logger = null,
        IWebHostEnvironment? environment = null)
        : base(options)
    {
    }
    
    public DbSet<Monster5eDbEntity> Monsters { get; set; }
    public DbSet<ActionDbEntity> MonsterActions { get; set; }
    public DbSet<TraitDbEntity> Traits { get; set; }
    public DbSet<Symbarum5eDbEntity> Symbarum5es { get; set; }

    public DbSet<MonsterAideddDbEntity> AideddMonsters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Additional configuration for development
        if (_environment?.IsDevelopment() == true)
        {
            optionsBuilder.LogTo(
                message => _logger?.LogDebug(message),
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information);
        }
    }

    // Configuration optionnelle du modèle
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(_defaultPostgresSchema);
        
        modelBuilder.ApplyConfiguration(new Monster5eDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ActionDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TraitDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TraitDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new Symbarum5eDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new Monster5eDbEntityConfiguration());
    }
}