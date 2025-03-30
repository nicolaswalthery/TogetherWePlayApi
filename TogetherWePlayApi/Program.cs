using TogetherWePlayApi.Controllers;
using TWP.Api.Application.BusinessLayers;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<JsonRepositoryBase>();
builder.Services.AddTransient<IMonsterActivitiesJsonRepository, MonsterActivitiesJsonRepository>();
builder.Services.AddTransient<ISomethingHappenJsonRepository, SomethingHappenJsonRepository>();
builder.Services.AddTransient<IUltraModern5eJsonRepository, UltraModern5eJsonRepository>();
builder.Services.AddTransient<IPathfinder2eBusinessLayer, Pathfinder2eBusinessLayer>();

builder.Services.AddTransient<IDndEncounterBusinessLayer, Dnd5eEncounterBusinessLayer>();
builder.Services.AddTransient<IUltraModern5eBusinessLayer, UltraModern5eBusinessLayer>();
builder.Services.AddTransient<IPathfinder2eBusinessLayer, Pathfinder2eBusinessLayer>();

builder.Services.AddTransient<IDndController, DndController>();
builder.Services.AddTransient<IUltraModern5eController, UltraModern5eController>();

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = false;
                });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
