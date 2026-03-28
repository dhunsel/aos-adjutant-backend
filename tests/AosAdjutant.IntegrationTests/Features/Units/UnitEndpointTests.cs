using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Units;

public class UnitEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
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

    // --- GET /api/units/{id} ---

    [Fact]
    public async Task GetUnit_Returns200()
    {
        var created = await CreateUnitAsync();

        var response = await Client.GetAsync($"/api/units/{created.UnitId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UnitResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(created, body);
    }

    // --- PUT /api/units/{id} ---

    [Fact]
    public async Task UpdateUnit_Returns200()
    {
        var created = await CreateUnitAsync();
        var changeUnitDto = new ChangeUnitDto
        {
            Name = "UpdatedUnit",
            Health = 20,
            Move = "6",
            Save = 3,
            Control = 1,
            Version = created.Version
        };

        var response = await Client.PutAsJsonAsync($"/api/units/{created.UnitId}", changeUnitDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UnitResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(
            new
            {
                changeUnitDto.Name,
                changeUnitDto.Health,
                changeUnitDto.Move,
                changeUnitDto.Save,
                changeUnitDto.Control,
                changeUnitDto.WardSave,
                created.FactionId
            },
            body
        );
    }

    [Fact]
    public async Task UpdateUnit_Returns400_WhenNameIsEmpty()
    {
        var created = await CreateUnitAsync();

        var response = await Client.PutAsJsonAsync(
            $"/api/units/{created.UnitId}",
            new ChangeUnitDto
            {
                Name = "",
                Health = 20,
                Move = "6",
                Save = 3,
                Control = 1,
                Version = created.Version
            }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- DELETE /api/units/{id} ---

    [Fact]
    public async Task DeleteUnit_Returns204()
    {
        var created = await CreateUnitAsync();

        var response = await Client.DeleteAsync($"/api/units/{created.UnitId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
