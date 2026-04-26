using System.Net;
using System.Net.Http.Json;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.IntegrationTests.Fixture;

namespace AosAdjutant.IntegrationTests.Features.Abilities;

public class AbilityEndpointTests(ApiFactory factory) : EndpointTestsBase(factory)
{
    private async Task<AbilityResponseDto> CreateAbilityAsync()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/abilities",
            new CreateAbilityDto
            {
                Name = "TestAbility",
                Declaration = "TestDeclaration",
                Effect = "TestEffect",
                Phase = Phase.Hero,
                Turn = Turn.YourTurn,
            },
            JsonOptions
        );
        return (await response.Content.ReadFromJsonAsync<AbilityResponseDto>(JsonOptions))!;
    }

    // --- POST /api/abilities ---

    [Fact]
    public async Task CreateAbility_Returns201()
    {
        var createAbilityDto = new CreateAbilityDto
        {
            Name = "TestAbility",
            Declaration = "TestDeclaration",
            Effect = "TestEffect",
            Phase = Phase.Hero,
            Turn = Turn.YourTurn,
        };

        var response = await Client.PostAsJsonAsync(
            "/api/abilities",
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
                createAbilityDto.Turn,
            },
            body
        );
    }

    // --- GET /api/abilities/{id} ---

    [Fact]
    public async Task GetAbility_Returns200()
    {
        var created = await CreateAbilityAsync();

        var response = await Client.GetAsync($"/api/abilities/{created.AbilityId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AbilityResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(created, body);
    }

    // --- PUT /api/abilities/{id} ---

    [Fact]
    public async Task UpdateAbility_Returns200()
    {
        var created = await CreateAbilityAsync();
        var changeAbilityDto = new ChangeAbilityDto
        {
            Name = "UpdatedAbility",
            Declaration = "UpdatedDeclaration",
            Effect = "UpdatedEffect",
            Phase = Phase.Combat,
            Turn = Turn.EnemyTurn,
            Version = created.Version,
        };

        var response = await Client.PutAsJsonAsync(
            $"/api/abilities/{created.AbilityId}",
            changeAbilityDto,
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AbilityResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equivalent(
            new
            {
                changeAbilityDto.Name,
                changeAbilityDto.Reaction,
                changeAbilityDto.Declaration,
                changeAbilityDto.Effect,
                changeAbilityDto.Phase,
                changeAbilityDto.Restriction,
                changeAbilityDto.Turn,
            },
            body
        );
    }

    [Fact]
    public async Task UpdateAbility_Returns400_WhenNameIsEmpty()
    {
        var created = await CreateAbilityAsync();

        var response = await Client.PutAsJsonAsync(
            $"/api/abilities/{created.AbilityId}",
            new ChangeAbilityDto
            {
                Name = "",
                Declaration = "UpdatedDeclaration",
                Effect = "UpdatedEffect",
                Phase = Phase.Combat,
                Turn = Turn.EnemyTurn,
                Version = created.Version,
            },
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- DELETE /api/abilities/{id} ---

    [Fact]
    public async Task DeleteAbility_Returns204()
    {
        var created = await CreateAbilityAsync();

        var response = await Client.DeleteAsync($"/api/abilities/{created.AbilityId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
