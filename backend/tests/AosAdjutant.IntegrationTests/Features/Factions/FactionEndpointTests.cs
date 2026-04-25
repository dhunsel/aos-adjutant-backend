using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Factions;

public class FactionEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<FactionResponseDto> CreateFactionAsync(
        string name = "TestFaction",
        GrandAlliance grandAlliance = GrandAlliance.Order
    )
    {
        var response = await Client.PostAsJsonAsync(
            "/api/factions",
            new CreateFactionDto { Name = name, GrandAlliance = grandAlliance },
            JsonOptions
        );
        return (await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;
    }

    // --- POST /api/factions ---

    [Fact]
    public async Task CreateFaction_Returns201()
    {
        var createFactionDto = new CreateFactionDto
        {
            Name = "TestFaction",
            GrandAlliance = GrandAlliance.Order,
        };

        var response = await Client.PostAsJsonAsync("/api/factions", createFactionDto, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body.FactionId > 0);
        Assert.Equivalent(createFactionDto, body);
    }

    [Fact]
    public async Task CreateFaction_Returns400_WhenNameIsEmpty()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/factions",
            new CreateFactionDto { Name = "", GrandAlliance = GrandAlliance.Order },
            JsonOptions
        );

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
        Assert.Equivalent(created, body);
    }

    // --- PUT /api/factions/{id} ---

    [Fact]
    public async Task UpdateFaction_Returns200()
    {
        var created = await CreateFactionAsync();
        var changeFactionDto = new ChangeFactionDto
        {
            Name = "TestFactionUpdated",
            GrandAlliance = GrandAlliance.Order,
            Version = created.Version,
        };

        var response = await Client.PutAsJsonAsync(
            $"/api/factions/{created.FactionId}",
            changeFactionDto,
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(new { changeFactionDto.Name, changeFactionDto.GrandAlliance }, body);
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
