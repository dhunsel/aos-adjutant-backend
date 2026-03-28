using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.AttackProfiles;
using AosAdjutant.Api.Features.Factions;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Units;

public class UnitAttackProfileEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
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

    private static CreateAttackProfileDto ValidAttackProfileDto() => new()
    {
        Name = "TestAttackProfile",
        IsRanged = false,
        Attacks = "2",
        ToHit = 3,
        ToWound = 3,
        Damage = "1",
        WeaponEffects = []
    };

    // --- POST /api/units/{id}/attack-profiles ---

    [Fact]
    public async Task CreateAttackProfile_Returns201()
    {
        var unit = await CreateUnitAsync();
        var createAttackProfileDto = ValidAttackProfileDto();

        var response = await Client.PostAsJsonAsync(
            $"/api/units/{unit.UnitId}/attack-profiles",
            createAttackProfileDto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AttackProfileResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.True(body.AttackProfileId > 0);
        Assert.Equivalent(
            new
            {
                createAttackProfileDto.Name,
                createAttackProfileDto.IsRanged,
                createAttackProfileDto.Range,
                createAttackProfileDto.Attacks,
                createAttackProfileDto.ToHit,
                createAttackProfileDto.ToWound,
                createAttackProfileDto.Rend,
                createAttackProfileDto.Damage,
                unit.UnitId
            },
            body
        );
    }

    [Fact]
    public async Task CreateAttackProfile_Returns201_WithWeaponEffects()
    {
        var unit = await CreateUnitAsync();
        var createAttackProfileDto = ValidAttackProfileDto() with { WeaponEffects = ["crit_mortal", "companion"] };

        var response = await Client.PostAsJsonAsync(
            $"/api/units/{unit.UnitId}/attack-profiles",
            createAttackProfileDto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AttackProfileResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(2, body.WeaponEffects.Count);
    }

    [Fact]
    public async Task CreateAttackProfile_Returns400_WhenNameIsEmpty()
    {
        var unit = await CreateUnitAsync();

        var response = await Client.PostAsJsonAsync(
            $"/api/units/{unit.UnitId}/attack-profiles",
            ValidAttackProfileDto() with { Name = "" }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- GET /api/units/{id}/attack-profiles ---

    [Fact]
    public async Task GetAttackProfiles_Returns200()
    {
        var unit = await CreateUnitAsync();
        await Client.PostAsJsonAsync($"/api/units/{unit.UnitId}/attack-profiles", ValidAttackProfileDto());

        var response = await Client.GetAsync($"/api/units/{unit.UnitId}/attack-profiles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<AttackProfileResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Single(body);
    }
}
