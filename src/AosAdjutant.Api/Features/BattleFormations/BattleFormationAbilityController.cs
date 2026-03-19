using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.BattleFormations;

[Route("api/battle-formations/{battleFormationId}/abilities")]
[ApiController]
[Tags("Battle Formations")]
public class BattleFormationAbilityController(ApplicationDbContext context, AbilityService abilityService)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create an ability for a battle formation")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbilityResponseDto>> CreateAbility(
        [FromRoute] int battleFormationId,
        [FromBody] CreateAbilityDto abilityData
    )
    {
        var battleFormation = await context.BattleFormations.FindAsync(battleFormationId);

        if (battleFormation is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Battle formation not found."));

        var newAbilityResult = Ability.Create(
            abilityData.Name,
            abilityData.Reaction,
            abilityData.Declaration,
            abilityData.Effect,
            abilityData.Phase,
            abilityData.Restriction,
            abilityData.Turn,
            false
        );

        if (!newAbilityResult.IsSuccess) return this.ApiProblem(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        battleFormation.Abilities.Add(newAbility);
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
    [EndpointSummary("Get all abilities for a battle formation")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities([FromRoute] int battleFormationId)
    {
        var battleFormation = await context.BattleFormations
            .AsNoTracking()
            .Include(bf => bf.Abilities)
            .FirstOrDefaultAsync(bf => bf.BattleFormationId == battleFormationId);

        if (battleFormation is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Battle formation not found."));

        return Ok(
            battleFormation.Abilities.Select(a => new AbilityResponseDto(
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
