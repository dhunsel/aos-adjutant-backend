using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Factions;

public class FactionEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<FactionResponseDto> CreateFactionAsync(string name = "TestFaction")
    {
        var response = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto { Name = name });
        return (await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;
    }

    // --- POST /api/factions ---

    [Fact]
    public async Task CreateFaction_Returns201()
    {
        var response = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto { Name = "TestFaction" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("TestFaction", body.Name);
        Assert.True(body.FactionId > 0);
    }

    [Fact]
    public async Task CreateFaction_Returns400_WhenNameIsEmpty()
    {
        var response = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto { Name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- GET /api/factions ---

    [Fact]
    public async Task GetFactions_Returns200()
    {
        await CreateFactionAsync("TestFaction1");
        await CreateFactionAsync("TestFaction2");

        var response = await Client.GetAsync("/api/factions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<FactionResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(2, body.Count);
    }

    // --- GET /api/factions/{id} ---

    [Fact]
    public async Task GetFaction_Returns200()
    {
        var created = await CreateFactionAsync();

        var response = await Client.GetAsync($"/api/factions/{created.FactionId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("TestFaction", body.Name);
    }

    // --- PUT /api/factions/{id} ---

    [Fact]
    public async Task UpdateFaction_Returns200()
    {
        var created = await CreateFactionAsync();

        var response = await Client.PutAsJsonAsync(
            $"/api/factions/{created.FactionId}",
            new ChangeFactionDto { Name = "TestFactionUpdated", Version = created.Version }
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("TestFactionUpdated", body.Name);
    }

    // --- DELETE /api/factions/{id} ---

    [Fact]
    public async Task DeleteFaction_Returns204()
    {
        var created = await CreateFactionAsync();

        var response = await Client.DeleteAsync($"/api/factions/{created.FactionId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
