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
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

        opts.CallbackPath = "/api/signin-oidc";
        opts.SignedOutCallbackPath = "/api/signout-callback-oidc";

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

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService(
            serviceName: "AosAdjutantApi",
            serviceVersion: "v0.1.0",
            serviceInstanceId: Environment.MachineName
        )
    )
    .WithTracing(tracing =>
        tracing
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint = new Uri(builder.Configuration["OTLP:Endpoint"]!);
            })
    )
    .WithMetrics(metrics =>
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .SetExemplarFilter(ExemplarFilterType.TraceBased)
            .AddOtlpExporter(
                (exporterOptions, metricReaderOptions) =>
                {
                    exporterOptions.Endpoint = new Uri(builder.Configuration["OTLP:Endpoint"]!);
                    metricReaderOptions
                        .PeriodicExportingMetricReaderOptions
                        .ExportIntervalMilliseconds = builder.Configuration.GetValue<int>(
                        "OTLP:Metrics:ExportIntervalMilliseconds"
                    );
                }
            )
    )
    .WithLogging(logging =>
        logging.AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(builder.Configuration["OTLP:Endpoint"]!);
        })
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
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>(
    (sp, opt) =>
        opt.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>())
            .UseNpgsql(builder.Configuration["AosAdjutant:DbContextConnectionString"])
);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapStaticAssets().AllowAnonymous();
app.MapControllers();
app.MapFallbackToFile("index.html").AllowAnonymous();

await app.RunAsync();
