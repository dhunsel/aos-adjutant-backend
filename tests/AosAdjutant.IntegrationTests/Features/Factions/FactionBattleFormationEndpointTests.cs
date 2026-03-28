using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Factions;

public class FactionBattleFormationEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<FactionResponseDto> CreateFactionAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto { Name = "TestFaction" });
        return (await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;
    }

    // --- POST /api/factions/{factionId}/battle-formations ---

    [Fact]
    public async Task CreateBattleFormation_Returns201()
    {
        var faction = await CreateFactionAsync();

        var response = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/battle-formations",
            new CreateBattleFormationDto { Name = "TestBattleFormation" }
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<BattleFormationResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body.BattleFormationId > 0);
        Assert.Equivalent(new { Name = "TestBattleFormation", FactionId = faction.FactionId }, body);
    }

    [Fact]
    public async Task CreateBattleFormation_Returns400_WhenNameIsEmpty()
    {
        var faction = await CreateFactionAsync();

        var response = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/battle-formations",
            new CreateBattleFormationDto { Name = "" }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- GET /api/factions/{factionId}/battle-formations ---

    [Fact]
    public async Task GetBattleFormations_Returns200()
    {
        var faction = await CreateFactionAsync();
        await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/battle-formations",
            new CreateBattleFormationDto { Name = "TestBattleFormation" }
        );

        var response = await Client.GetAsync($"/api/factions/{faction.FactionId}/battle-formations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<BattleFormationResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}
