using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.Auth;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.Api.Features.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi;
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

    // Path prefix applied via UsePathBase and reflected in the OpenAPI servers entry.
    // Both must stay in sync, so reference this single source of truth.
    const string BasePath = "/api";

    builder
        .Services.AddAuthentication(opts =>
        {
            opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(opts =>
        {
            opts.Cookie.Name = "__Host-aosadj-id";
            opts.Cookie.HttpOnly = true;
            opts.Cookie.SameSite = SameSiteMode.Strict;
            opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            opts.Cookie.Path = "/";
            opts.ExpireTimeSpan = TimeSpan.FromHours(8);
            opts.SlidingExpiration = true;
        })
        .AddOpenIdConnect(opts =>
        {
            opts.Authority = builder.Configuration["Authentication:Authority"];

            opts.ClientId = builder.Configuration["Authentication:ClientId"];
            opts.ClientSecret = builder.Configuration["Authentication:ClientSecret"];
            opts.ResponseType = OpenIdConnectResponseType.Code;
            opts.UsePkce = true;
            opts.SaveTokens = false;

            opts.Events.OnTokenValidated = AuthEvents.OnTokenValidated;
            opts.Events.OnRemoteFailure = AuthEvents.OnRemoteFailure;

            opts.GetClaimsFromUserInfoEndpoint = false;
            opts.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.MapInboundClaims = false;

            opts.Scope.Clear();
            opts.Scope.Add("openid");
            opts.Scope.Add("profile");
            opts.Scope.Add("email");
            opts.Scope.Add("groups");

            opts.RequireHttpsMetadata = builder.Configuration.GetValue(
                "Authentication:RequireHttpsMetadata",
                defaultValue: true
            );

            opts.TokenValidationParameters.NameClaimType = "preferred_username";
            opts.TokenValidationParameters.RoleClaimType = "groups";
        });

    builder
        .Services.AddAuthorizationBuilder()
        .AddFallbackPolicy("RequireAdmin", p => p.RequireRole("admins"));

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
                new JsonStringEnumConverter(
                    namingPolicy: JsonNamingPolicy.CamelCase,
                    allowIntegerValues: false
                )
            );
        });

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        options.SerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                namingPolicy: JsonNamingPolicy.CamelCase,
                allowIntegerValues: false
            )
        );
    });

    builder.Services.AddScoped<FactionService, FactionService>();
    builder.Services.AddScoped<BattleFormationService, BattleFormationService>();
    builder.Services.AddScoped<UnitService, UnitService>();
    builder.Services.AddScoped<AttackProfileService, AttackProfileService>();
    builder.Services.AddScoped<AbilityService, AbilityService>();
    builder.Services.AddScoped<UserService, UserService>();

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();

    builder.Services.AddOpenApi(opts =>
    {
        opts.AddOperationTransformer<CamelCaseQueryParametersTransformer>();

        // ASP.NET Core's OpenAPI generation does not emit a servers entry, so the document
        // describes endpoints at the root and ignores the UsePathBase prefix. Declare the
        // base path as a server so Scalar, generated clients and the checked-in spec target
        // the correct URLs.
        opts.AddDocumentTransformer(
            (document, _, _) =>
            {
                document.Servers = [new OpenApiServer { Url = BasePath }];
                return Task.CompletedTask;
            }
        );
    });

    builder.Services.AddDbContext<ApplicationDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration["AosAdjutant:DbContextConnectionString"])
    );

    var app = builder.Build();

    app.UsePathBase(BasePath);

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

    app.UseAuthentication();
    app.UseAuthorization();

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
