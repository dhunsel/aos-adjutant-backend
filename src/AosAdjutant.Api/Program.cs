using System.Text.Json.Serialization;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false))
    );

builder.Services.AddScoped<FactionService, FactionService>();
builder.Services.AddScoped<BattleFormationService, BattleFormationService>();
builder.Services.AddScoped<UnitService, UnitService>();
builder.Services.AddScoped<AttackProfileService, AttackProfileService>();
builder.Services.AddScoped<AbilityService, AbilityService>();

builder.Services.AddProblemDetails();

builder.Services.AddHttpLogging(opts => { });

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration["AosAdjutant:DbContextConnectionString"])
);

var app = builder.Build();

app.UseHttpLogging();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

await app.RunAsync();
