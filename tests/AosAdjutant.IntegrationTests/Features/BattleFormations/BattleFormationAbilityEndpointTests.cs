using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.BattleFormations;

public class BattleFormationAbilityEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<BattleFormationResponseDto> CreateBattleFormationAsync()
    {
        var factionResponse = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto("TestFaction"));
        var faction = (await factionResponse.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;

        var response = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/battle-formations",
            new CreateBattleFormationDto("TestBattleFormation")
        );
        return (await response.Content.ReadFromJsonAsync<BattleFormationResponseDto>(JsonOptions))!;
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

    // --- POST /api/battle-formations/{id}/abilities ---

    [Fact]
    public async Task CreateAbility_Returns201()
    {
        var battleFormation = await CreateBattleFormationAsync();
        var createAbilityDto = ValidAbilityDto();

        var response = await Client.PostAsJsonAsync(
            $"/api/battle-formations/{battleFormation.BattleFormationId}/abilities",
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

    // --- GET /api/battle-formations/{id}/abilities ---

    [Fact]
    public async Task GetAbilities_Returns200()
    {
        var battleFormation = await CreateBattleFormationAsync();
        await Client.PostAsJsonAsync(
            $"/api/battle-formations/{battleFormation.BattleFormationId}/abilities",
            ValidAbilityDto(),
            JsonOptions
        );

        var response = await Client.GetAsync($"/api/battle-formations/{battleFormation.BattleFormationId}/abilities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<AbilityResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}
