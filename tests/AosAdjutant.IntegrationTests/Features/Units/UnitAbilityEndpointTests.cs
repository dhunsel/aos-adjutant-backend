using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Units;

public class UnitAbilityEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<UnitResponseDto> CreateUnitAsync()
    {
        var factionResponse = await Client.PostAsJsonAsync(
            "/api/factions",
            new CreateFactionDto { Name = "TestFaction" }
        );
        var faction = (await factionResponse.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;

        var response = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/units",
            new CreateUnitDto
            {
                Name = "TestUnit",
                Health = 10,
                Move = "5",
                Save = 4,
                Control = 2
            }
        );
        return (await response.Content.ReadFromJsonAsync<UnitResponseDto>(JsonOptions))!;
    }

    private static CreateAbilityDto ValidAbilityDto() => new()
    {
        Name = "TestAbility",
        Declaration = "TestDeclaration",
        Effect = "TestEffect",
        Phase = TurnPhase.Hero,
        Turn = PlayerTurn.YourTurn
    };

    // --- POST /api/units/{id}/abilities ---

    [Fact]
    public async Task CreateAbility_Returns201()
    {
        var unit = await CreateUnitAsync();
        var createAbilityDto = ValidAbilityDto();

        var response = await Client.PostAsJsonAsync(
            $"/api/units/{unit.UnitId}/abilities",
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

    // --- GET /api/units/{id}/abilities ---

    [Fact]
    public async Task GetAbilities_Returns200()
    {
        var unit = await CreateUnitAsync();
        await Client.PostAsJsonAsync($"/api/units/{unit.UnitId}/abilities", ValidAbilityDto(), JsonOptions);

        var response = await Client.GetAsync($"/api/units/{unit.UnitId}/abilities");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<AbilityResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}
