using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Factions;

public class FactionAbilityEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<FactionResponseDto> CreateFactionAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto("TestFaction"));
        return (await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;
    }

    private static CreateAbilityDto ValidAbilityDto() => new(
        "TestAbility",
        null,
        "TestDeclaration",
        "TestEffect",
        TurnPhase.Hero,
        null,
        PlayerTurn.YourTurn
    );

    // --- POST /api/factions/{factionId}/abilities ---

    [Fact]
    public async Task CreateAbility_Returns201()
    {
        var faction = await CreateFactionAsync();
        var createAbilityDto = ValidAbilityDto();

        var response = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/abilities",
            createAbilityDto,
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AbilityResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body.AbilityId > 0);
        Assert.Equivalent(
            new
            {
                createAbilityDto.Name,
                createAbilityDto.Reaction,
                createAbilityDto.Declaration,
                createAbilityDto.Effect,
                createAbilityDto.Phase,
                createAbilityDto.Restriction,
                createAbilityDto.Turn
            },
            body
        );
    }

    // --- GET /api/factions/{factionId}/abilities ---

    [Fact]
    public async Task GetAbilities_Returns200()
    {
        var faction = await CreateFactionAsync();
        await Client.PostAsJsonAsync($"/api/factions/{faction.FactionId}/abilities", ValidAbilityDto(), JsonOptions);

        var response = await Client.GetAsync($"/api/factions/{faction.FactionId}/abilities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<AbilityResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}
