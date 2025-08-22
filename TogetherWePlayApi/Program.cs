using Common.Extensions;
using Common.Security;
using Microsoft.OpenApi.Models;
using TogetherWePlayApi.Controllers;
using TWP.Api.Application.BusinessLayers;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Infrastructure.CsvRepositories;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Interops;
using TWP.Api.Infrastructure.JsonRepositories;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Api Key Settings
builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection(ApiKeyOptions.SectionName));

//Singleton pour le middleware d'API Key
builder.Services.AddSingleton<ApiKeyOptions>(sp =>
{ 
    var options = new ApiKeyOptions();
    builder.Configuration.GetSection(ApiKeyOptions.SectionName).Bind(options);

    var envApiKey = Environment.GetEnvironmentVariable("API_KEY");
    if (envApiKey.IsNotNullOrEmptyOrWhiteSpace())
    {
        options.ApiKeys.Clear();
        options.ApiKeys.Add(new ApiKeyConfiguration
        {
            Key = envApiKey!,
            Name = "Production",
            IsActive = true
        });
    }

    return options;

});

// 3. Cache pour le rate limiting (Api Key)
builder.Services.AddMemoryCache();

// 4. CORS pour Make/n8n
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddTransient<JsonRepositoryBase>();
builder.Services.AddTransient<IMonsterActivitiesJsonRepository, MonsterActivitiesJsonRepository>();
builder.Services.AddTransient<ISomethingHappenJsonRepository, SomethingHappenJsonRepository>();
builder.Services.AddTransient<IUltraModern5eJsonRepository, UltraModern5eJsonRepository>();
builder.Services.AddTransient<IPathfinder2eMonsterCoreJsonRepository, Pathfinder2eMonsterCoreJsonRepository>();
builder.Services.AddTransient<IPathfinder2eConditionsJsonRepository, Pathfinder2eConditionsJsonRepository>();
builder.Services.AddTransient<CsvRepositoryBase>();
builder.Services.AddTransient<IDnd2024AllMonsterStatsCsvRepository, Dnd2024AllMonsterStatsCsvRepository>();
builder.Services.AddTransient<IDnd5eEncounterDataJsonRepository, Dnd5eEncounterDataJsonRepository>();

// Add LLM Services
builder.Services.AddTransient<IOpenAiInterops, OpenAiInterops>();

builder.Services.AddTransient<IDndEncounterBusinessLayer, Dnd5eEncounterBusinessLayer>();
builder.Services.AddTransient<IUltraModern5eBusinessLayer, UltraModern5eBusinessLayer>();
builder.Services.AddTransient<IPathfinder2eBusinessLayer, Pathfinder2eBusinessLayer>();
builder.Services.AddTransient<IDnd5eMonsterBusinessLayer, Dnd5eMonsterBusinessLayer>();

builder.Services.AddTransient<IDndController, DndController>();
builder.Services.AddTransient<IUltraModern5eController, UltraModern5eController>();
builder.Services.AddTransient<IPathfinder2eController, Pathfinder2eController>();
builder.Services.AddTransient<IDnd5eMonsterController, Dnd5eMonsterController>();

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = false;
                });

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TogetherWePlay API",
        Version = "v1",
        Description = "API sécurisée pour D&D et autres jeux de rôle"
    });

    // Ajouter le support de l'API Key dans Swagger
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
});

var app = builder.Build();

// ===== PIPELINE =====

// CORS (avant tout le reste)
app.UseCors("ApiPolicy");

// HTTPS Redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Swagger UI (seulement en dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TogetherWePlay API v1");
    });
}

// Health Check PUBLIC (pour Railway)
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}))
.AllowAnonymous()
.WithName("HealthCheck")
.WithTags("Monitoring");

app.UseAuthorization();

app.MapControllers();

// Log de démarrage
app.Logger.LogInformation("?? TogetherWePlay API Started");
app.Logger.LogInformation($"?? Environment: {app.Environment.EnvironmentName}");
app.Logger.LogInformation($"?? API Key Security: Enabled");
app.Logger.LogInformation($"?? Ready for D&D adventures!");

app.Run();
