using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.AttackProfiles;

public class AttackProfileEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<AttackProfileResponseDto> CreateAttackProfileAsync()
    {
        var factionResponse = await Client.PostAsJsonAsync("/api/factions", new CreateFactionDto("TestFaction"));
        var faction = (await factionResponse.Content.ReadFromJsonAsync<FactionResponseDto>(JsonOptions))!;

        var unitResponse = await Client.PostAsJsonAsync(
            $"/api/factions/{faction.FactionId}/units",
            new CreateUnitDto("TestUnit", 10, "5", 4, 2, null)
        );
        var unit = (await unitResponse.Content.ReadFromJsonAsync<UnitResponseDto>(JsonOptions))!;

        var response = await Client.PostAsJsonAsync(
            $"/api/units/{unit.UnitId}/attack-profiles",
            new CreateAttackProfileDto("TestProfile", false, null, "2", 3, 3, null, "1", [])
        );
        return (await response.Content.ReadFromJsonAsync<AttackProfileResponseDto>(JsonOptions))!;
    }

    // --- GET /api/attack-profiles/{id} ---

    [Fact]
    public async Task GetAttackProfile_Returns200()
    {
        var created = await CreateAttackProfileAsync();

        var response = await Client.GetAsync($"/api/attack-profiles/{created.AttackProfileId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AttackProfileResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(created, body);
    }

    // --- PUT /api/attack-profiles/{id} ---

    [Fact]
    public async Task UpdateAttackProfile_Returns200()
    {
        var created = await CreateAttackProfileAsync();
        var changeDto = new ChangeAttackProfileDto(
            "UpdatedProfile",
            false,
            null,
            "3",
            4,
            4,
            null,
            "2",
            created.Version,
            []
        );

        var response = await Client.PutAsJsonAsync($"/api/attack-profiles/{created.AttackProfileId}", changeDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AttackProfileResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(
            new
            {
                changeDto.Name,
                changeDto.IsRanged,
                changeDto.Range,
                changeDto.Attacks,
                changeDto.ToHit,
                changeDto.ToWound,
                changeDto.Rend,
                changeDto.Damage
            },
            body
        );
    }

    [Fact]
    public async Task UpdateAttackProfile_Returns400_WhenNameIsEmpty()
    {
        var created = await CreateAttackProfileAsync();

        var response = await Client.PutAsJsonAsync(
            $"/api/attack-profiles/{created.AttackProfileId}",
            new ChangeAttackProfileDto("", false, null, "3", 4, 4, null, "2", created.Version, [])
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- DELETE /api/attack-profiles/{id} ---

    [Fact]
    public async Task DeleteAttackProfile_Returns204()
    {
        var created = await CreateAttackProfileAsync();

        var response = await Client.DeleteAsync($"/api/attack-profiles/{created.AttackProfileId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
