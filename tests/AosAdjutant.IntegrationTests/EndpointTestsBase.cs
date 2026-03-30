using System.Text.Json;
using System.Text.Json.Serialization;
using AosAdjutant.Api.Database;
using AosAdjutant.IntegrationTests.Fixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AosAdjutant.IntegrationTests;

[Collection(nameof(ApiCollection))]
public class EndpointTestsBase(ApiFactory factory) : IAsyncLifetime
{
    protected readonly HttpClient Client = factory.CreateClient();

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() }
    };

    public async Task InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var tableNames = context.Model.GetEntityTypes()
            .Where(e => e.GetTableName() != "weapon_effect") // Weapon effect contains seed data
            .Select(e => e.GetTableName())
            .Distinct();

#pragma warning disable EF1002
        await context.Database.ExecuteSqlRawAsync(
            $"TRUNCATE TABLE {string.Join(", ", tableNames.Select(t => $"\"{t}\""))} CASCADE"
        );
#pragma warning restore EF1002
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
