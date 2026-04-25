using System.Globalization;
using System.Text.Json.Serialization;
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;

// Bootstrap logger to log errors during startup
// Replaced at a later step with a fully-featured logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

Log.Information("Starting the application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(
        (services, lc) =>
            lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );

    builder
        .Services.AddOpenTelemetry()
        .ConfigureResource(resource =>
            resource.AddService(serviceName: "AosAdjutantApi", serviceVersion: "0.0.1")
        )
        .WithTracing(tracing =>
            tracing
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint = new Uri(builder.Configuration["OTLP:Endpoint"]!);
                })
        )
        .WithMetrics(metrics =>
            metrics
                .AddAspNetCoreInstrumentation()
                .SetExemplarFilter(ExemplarFilterType.TraceBased)
                .AddOtlpExporter(
                    (exporterOptions, metricReaderOptions) =>
                    {
                        exporterOptions.Endpoint = new Uri(
                            builder.Configuration["Metrics:Endpoint"]!
                        );
                        exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                        metricReaderOptions
                            .PeriodicExportingMetricReaderOptions
                            .ExportIntervalMilliseconds = builder.Configuration.GetValue<int>(
                            "Metrics:ExportIntervalMilliseconds"
                        );
                    }
                )
        );

    builder
        .Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
            opts.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(allowIntegerValues: false)
            );
        });

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        options.SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false)
        );
    });

    builder.Services.AddScoped<FactionService, FactionService>();
    builder.Services.AddScoped<BattleFormationService, BattleFormationService>();
    builder.Services.AddScoped<UnitService, UnitService>();
    builder.Services.AddScoped<AttackProfileService, AttackProfileService>();
    builder.Services.AddScoped<AbilityService, AbilityService>();

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddOpenApi();

    builder.Services.AddCors(options =>
        options.AddPolicy(
            "Frontend",
            policy =>
                policy
                    .WithOrigins(
                        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                            ?? []
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
        )
    );

    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration["AosAdjutant:DbContextConnectionString"])
    );

    var app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.UseExceptionHandler();

    app.UseCors("Frontend");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.MapControllers();

    await app.RunAsync();

    Log.Information("Application shutdown successful");
    return 0;
}
#pragma warning disable CA1031 // Ignore here because we catch exception just to log it before shutdown
catch (Exception ex)
#pragma warning restore CA1031
{
    Log.Fatal(ex, "Unhandled exception during startup");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}
