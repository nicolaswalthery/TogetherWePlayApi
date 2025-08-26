using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TWP.Api.Core.DbEntities;
using TWP.Api.Infrastructure.Configurations;

public class DataContext : DbContext
{
    private readonly string _defaultSchema = "dbo";
    private readonly ILogger<DataContext>? _logger;
    private readonly IWebHostEnvironment? _environment;

    public DataContext(
        DbContextOptions<DataContext> options,
        ILogger<DataContext>? logger = null,
        IWebHostEnvironment? environment = null)
        : base(options)
    {
        _logger = logger;
        _environment = environment;
    }
    // DbSets for all entities
    public DbSet<Monster5eDbEntity> Monsters { get; set; }
    public DbSet<MonsterActionDbEntity> MonsterActions { get; set; }
    public DbSet<FeatureDbEntity> Features { get; set; }
    public DbSet<TraitDbEntity> Traits { get; set; }
    public DbSet<MonsterTraitDbEntity> MonsterTraits { get; set; }
    public DbSet<Symbarum5eDbEntity> Symbarum5e { get; set; }

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

        modelBuilder.HasDefaultSchema(_defaultSchema);
        // Apply all configurations
        modelBuilder.ApplyConfiguration(new Monster5eDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MonsterActionDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TraitDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MonsterTraitDbEntityConfiguration());
        modelBuilder.ApplyConfiguration(new Symbarum5eDbEntityConfiguration());
    }
}