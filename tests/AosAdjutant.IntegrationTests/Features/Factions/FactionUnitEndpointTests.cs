using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Factions;

public class FactionUnitEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<FactionResponseDto> CreateFactionAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto { Name = "TestFaction" });
        return (await response.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;
    }

    private static CreateUnitDto ValidUnitDto() => new()
    {
        Name = "TestUnit",
        Health = 10,
        Move = "5",
        Save = 4,
        Control = 2
    };

    // --- POST /api/factions/{factionId}/units ---

    [Fact]
    public async Task CreateUnit_Returns201()
    {
        var faction = await CreateFactionAsync();
        var createUnitDto = ValidUnitDto();

        var response = await Client.PostAsJsonAsync($"/api/factions/{faction.FactionId}/units", createUnitDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UnitResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body.UnitId > 0);
        Assert.Equivalent(
            new
            {
                createUnitDto.Name,
                createUnitDto.Health,
                createUnitDto.Move,
                createUnitDto.Save,
                createUnitDto.Control,
                createUnitDto.WardSave,
                faction.FactionId
            },
            body
        );
    }

    [Fact]
    public async Task CreateUnit_Returns400_WhenNameIsEmpty()
    {
        var faction = await CreateFactionAsync();

        var response = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/units",
            ValidUnitDto() with { Name = "" }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- GET /api/factions/{factionId}/units ---

    [Fact]
    public async Task GetUnits_Returns200()
    {
        var faction = await CreateFactionAsync();
        await Client.PostAsJsonAsync($"/api/factions/{faction.FactionId}/units", ValidUnitDto());

        var response = await Client.GetAsync($"/api/factions/{faction.FactionId}/units");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<UnitResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}
