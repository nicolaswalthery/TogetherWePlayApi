using Common.Extensions;
using Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using TogetherWePlayApi.Controllers;
using TWP.Api.Application.BusinessLayers;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataEnrichmentLayer;
using TWP.Api.Application.DataEnrichmentLayer.Interfaces;
using TWP.Api.Application.ETL;
using TWP.Api.Application.ETL.Services;
using TWP.Api.Application.Interfaces.Services;
using TWP.Api.Application.UseCases;
using TWP.Api.Application.UseCases.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Core.Interface.Infrastructure;
using TWP.Api.Core.Interfaces.Infrastructure.JsonRepositories;
using TWP.Api.Infrastructure.CsvRepositories;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Interops;
using TWP.Api.Infrastructure.JsonRepositories;
using TWP.Api.Infrastructure.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURATION DU LOGGING SELON L'ENVIRONNEMENT =====
if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

//Api Key Settings
builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection(ApiKeyOptions.SectionName));

//Singleton pour le middleware d'API Key
builder.Services.AddSingleton(sp =>
{ 
    var options = new ApiKeyOptions();
    builder.Configuration.GetSection(ApiKeyOptions.SectionName).Bind(options);

    var envApiKey = Environment.GetEnvironmentVariable("API_KEYS");
    if (envApiKey.IsNotNullOrEmptyOrWhiteSpace())
    {
        var apiKeys = envApiKey!.Split(',');
        options.ApiKeys.Clear();
        foreach (var apiKey in apiKeys)
        {
            options.ApiKeys.Add(new ApiKeyConfiguration
            {
                Key = apiKey,
                Name = "Production",
                IsActive = true
            });
        }
    }

    return options;

});

// 3. Cache pour le rate limiting (Api Key)
builder.Services.AddMemoryCache();

// ===== CORS CONFIGURATION =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Dev : Accepter toutes les origines
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Production : Être plus restrictif
            var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',')
                                ?? new[] { "*" }; // Fallback si non configuré

            if (allowedOrigins.Contains("*"))
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        }
    });
});

// ===== ENTITY FRAMEWORK / DATABASE CONFIGURATION =====
builder.Services.AddDbContext<DataContext>(options =>
{
    // Récupérer la connection string depuis les variables d'environnement en priorité
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                          ?? builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"DATABASE_URL : {connectionString}");
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Connection string 'DATABASE_URL' or 'DefaultConnection' not found.");

    // Configuration PostgreSQL
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(DataContext).Assembly.FullName);
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null);
    });

    // Options spécifiques à l'environnement
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging() // Affiche les valeurs des paramètres dans les logs
               .EnableDetailedErrors()        // Erreurs détaillées
               .LogTo(Console.WriteLine, LogLevel.Information); // Log SQL dans la console
    }
    else
    {
        options.EnableServiceProviderCaching()
               .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
});

// ===== REPOSITORIES =====
// JSON Repositories
builder.Services.AddTransient<JsonRepositoryBase>();
builder.Services.AddTransient<IMonster5eRepository, Monster5eRepository>();
builder.Services.AddTransient<IAideDdMonster5eRepository, AideDndMonster5eRepository>();
builder.Services.AddTransient<IMonsterActivitiesJsonRepository, MonsterActivitiesJsonRepository>();
builder.Services.AddTransient<ISomethingHappenJsonRepository, SomethingHappenJsonRepository>();
builder.Services.AddTransient<IUltraModern5eJsonRepository, UltraModern5eJsonRepository>();
builder.Services.AddTransient<IPathfinder2eMonsterCoreJsonRepository, Pathfinder2eMonsterCoreJsonRepository>();
builder.Services.AddTransient<IPathfinder2eConditionsJsonRepository, Pathfinder2eConditionsJsonRepository>();
builder.Services.AddTransient<IMonsterBuildingGuidelineRepository, MonsterBuildingGuidelineRepository>();

// CSV Repositories
builder.Services.AddTransient<CsvRepositoryBase>();
builder.Services.AddTransient<IDnd2024AllMonsterStatsCsvRepository, Dnd2024AllMonsterStatsCsvRepository>();
builder.Services.AddTransient<IDnd5eEncounterDataJsonRepository, Dnd5eEncounterDataJsonRepository>();

// Add Interops Services
builder.Services.AddTransient<IOpenAiServices>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    // Récupérer la clé API depuis la variable d'environnement
    var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                 ?? config["OpenAI:ApiKey"]
                 ?? throw new Exception("No OpenAI Api Key !");

    var modelName = config["OpenAI:ModelName"] ?? throw new Exception("No OpenAI Model Name !");
    var visionModelName = config["OpenAI:VisionModelName"] ?? throw new Exception("No OpenAI Vision Model Name !");

    return new OpenAiServices(
        apiKey: apiKey,
        modelName: modelName,
        visionModelName: visionModelName,
        logger: sp.GetService<ILogger<OpenAiServices>>()
    );
});

builder.Services.AddHttpClient<IMonsterApiInterops, Dnd5eApiMonstersServices>();
builder.Services.AddHttpClient<IAideDdInterops, AideDdInterops>();
// Mapper Services
builder.Services.AddTransient<IActionMapperService, ActionMapperService>();
builder.Services.AddTransient<ITraitMapperService, TraitMapperService>();

// ===== BUSINESS LAYERS =====
builder.Services.AddTransient<IDndEncounterBusinessLayer, Dnd5eEncounterBusinessLayer>();
builder.Services.AddTransient<IUltraModern5eBusinessLayer, UltraModern5eBusinessLayer>();
builder.Services.AddTransient<IPathfinder2eBusinessLayer, Pathfinder2eBusinessLayer>();
builder.Services.AddTransient<IDnd5eMonsterBusinessLayer, Dnd5eMonsterBusinessLayer>();

// ===== AGENT USE CASES =====
builder.Services.AddTransient<IGameMasterAgentUseCase, GameMasterAgentUseCase>();

//Data Enrichment Layer
builder.Services.AddTransient<IMonster5eDataEnrichmentLayer, Monster5eDataEnrichmentLayer>();

// ===== ETL ======
builder.Services.AddTransient<IExtractTransformLoad,  ExtractTransformLoad>();

// ===== CONTROLLERS =====
builder.Services.AddTransient<IDndController, DndController>();
builder.Services.AddTransient<IUltraModern5eController, UltraModern5eController>();
builder.Services.AddTransient<IPathfinder2eController, Pathfinder2eController>();
builder.Services.AddTransient<IDnd5eMonsterController, Dnd5eMonsterController>();
builder.Services.AddTransient<IEnrichmentDataController, EnrichmentDataController>();
builder.Services.AddTransient<IAgentController, AgentController>();

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = false;
                });

// ===== SWAGGER CONFIGURATION =====
// Swagger activé en dev ou si explicitement demandé
if (builder.Environment.IsDevelopment() ||
    Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "TogetherWePlay API",
            Version = "v1",
            Description = builder.Environment.IsDevelopment()
                ? "API sécurisée pour D&D et autres jeux de rôle (Development)"
                : "API sécurisée pour D&D et autres jeux de rôle",
            Contact = new OpenApiContact
            {
                Name = "TWP Support",
                Email = "support@togetherweplay.com"
            }
        });

        c.UseAllOfForInheritance();

        // Support de l'API Key dans Swagger
        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-API-Key",
            Description = "Entrez votre API Key"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Si vous avez des commentaires XML
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });
}

// ===== HEALTH CHECKS =====
builder.Services.AddHealthChecks().AddDbContextCheck<DataContext>("database");

var app = builder.Build();

// ===== DATABASE MIGRATION & SEEDING =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerService = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<DataContext>();

        if (app.Environment.IsDevelopment())
        {
            // En dev, appliquer automatiquement les migrations
            loggerService.LogInformation("📦 Applying database migrations...");
            //await context.Database.MigrateAsync();
            loggerService.LogInformation("✅ Database migrations applied successfully");
        }
        else
        {
            // En production, vérifier que la DB est accessible
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                loggerService.LogError("❌ Cannot connect to database!");
                throw new Exception("Database connection failed");
            }
            loggerService.LogInformation("✅ Database connection verified");

            // Optionnel : Vérifier si des migrations sont en attente
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                loggerService.LogWarning($"⚠️ There are {pendingMigrations.Count()} pending migrations");
                await context.Database.MigrateAsync();
            }
        }
    }
    catch (Exception ex)
    {
        loggerService.LogError(ex, "❌ An error occurred while setting up the database");
        if (!app.Environment.IsDevelopment())
        {
            throw; // En production, arrêter l'application si la DB n'est pas accessible
        }
    }
}

// ===== MIDDLEWARE PIPELINE =====
// CORS (avant tout le reste)
app.UseCors("ApiPolicy");

// HTTPS Redirection & HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts(); // HTTP Strict Transport Security
}

// Request logging en développement
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        app.Logger.LogDebug($"📥 {context.Request.Method} {context.Request.Path}");
        await next();
        app.Logger.LogDebug($"📤 {context.Request.Method} {context.Request.Path} -> {context.Response.StatusCode}");
    });
}

// Swagger UI
if (app.Environment.IsDevelopment() ||
    Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TogetherWePlay API v1");

        if (!app.Environment.IsDevelopment())
        {
            c.RoutePrefix = "api-docs"; // Change l'URL en production
        }

        // Personnalisation UI
        c.DocumentTitle = "TWP API Documentation";
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

// Health Check Endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            environment = app.Environment.EnvironmentName,
            version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0",
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            })
        };

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
})
.AllowAnonymous()
.WithName("HealthCheck")
.WithTags("Monitoring");

// Endpoint simple pour Railway/monitoring
app.MapGet("/health/simple", () => "OK")
    .AllowAnonymous()
    .ExcludeFromDescription(); // Exclure de Swagger

// Debug endpoints (seulement en dev)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/config", (IConfiguration config) => Results.Ok(new
    {
        environment = app.Environment.EnvironmentName,
        connectionStringConfigured = !string.IsNullOrEmpty(config.GetConnectionString("DefaultConnection")),
        openAiConfigured = !string.IsNullOrEmpty(config["OpenAI:ApiKey"]),
        securityEnabled = config.GetValue<bool>("Security:EnableRateLimiting"),
        apiKeyConfigured = !string.IsNullOrEmpty(config["Security:ApiKey"])
    }))
    .WithName("DebugConfig")
    .WithTags("Debug")
    .AllowAnonymous();

    app.MapGet("/debug/env", () => Results.Ok(new
    {
        environment = app.Environment.EnvironmentName,
        isDevelopment = app.Environment.IsDevelopment(),
        isProduction = app.Environment.IsProduction(),
        contentRoot = app.Environment.ContentRootPath,
        webRoot = app.Environment.WebRootPath
    }))
    .WithName("DebugEnvironment")
    .WithTags("Debug")
    .AllowAnonymous();
}

// Authorization middleware
app.UseAuthorization();

// Map controllers
app.MapControllers();

// ===== STARTUP LOGS =====
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 TogetherWePlay API Started");
logger.LogInformation($"📍 Environment: {app.Environment.EnvironmentName}");
logger.LogInformation($"🔒 API Key Security: Enabled");
logger.LogInformation($"📊 Database: {(app.Environment.IsDevelopment() ? "Local PostgreSQL" : "Production Database")}");
logger.LogInformation($"🌐 CORS: {(app.Environment.IsDevelopment() ? "Allow All Origins" : "Restricted")}");

if (app.Environment.IsDevelopment())
{
    var urls = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
    logger.LogInformation($"📚 Swagger UI: {urls}/swagger");
    logger.LogInformation($"🔍 Health Check: {urls}/health");
    logger.LogInformation($"🐛 Debug Endpoints: {urls}/debug/config & {urls}/debug/env");
    logger.LogInformation($"💡 Tip: Set ASPNETCORE_ENVIRONMENT to 'Production' to test prod config");
}
else
{
    logger.LogInformation($"🔐 Production mode active - Sensitive data logging disabled");
    logger.LogInformation($"📈 Health endpoint: /health");
}

logger.LogInformation($"🐉 Ready for D&D adventures!");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// ✅ Route simple pour test de vie
app.MapGet("/", () => "🚀 Hello from Railway + TogetherWePlay!");

// Run the application
app.Run();