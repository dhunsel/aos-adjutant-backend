using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions/{factionId}/abilities")]
[ApiController]
[Tags("Factions")]
public class FactionAbilityController(ApplicationDbContext context, AbilityService abilityService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create an ability for a faction")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbilityResponseDto>> CreateAbility(
        [FromRoute] int factionId,
        [FromBody] CreateAbilityDto abilityData
    )
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        var newAbilityResult = abilityService.CreateAbility(abilityData);

        if (!newAbilityResult.IsSuccess) return this.ApiProblem(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        faction.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        return Created(
            $"api/abilities/{newAbility.AbilityId}",
            new AbilityResponseDto(
                newAbility.AbilityId,
                newAbility.Name,
                newAbility.Reaction,
                newAbility.Declaration,
                newAbility.Effect,
                newAbility.Phase,
                newAbility.Restriction,
                newAbility.Turn,
                newAbility.Version
            )
        );
    }

    [HttpGet]
    [EndpointSummary("Get all abilities for a faction")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities([FromRoute] int factionId)
    {
        var faction = await context.Factions
            .AsNoTracking()
            .Include(f => f.Abilities)
            .FirstOrDefaultAsync(f => f.FactionId == factionId);

        if (faction is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        return Ok(
            faction.Abilities.Select(a => new AbilityResponseDto(
                    a.AbilityId,
                    a.Name,
                    a.Reaction,
                    a.Declaration,
                    a.Effect,
                    a.Phase,
                    a.Restriction,
                    a.Turn,
                    a.Version
                )
            )
        );
    }
}
