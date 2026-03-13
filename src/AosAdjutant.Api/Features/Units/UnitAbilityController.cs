using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Units;

[Route("api/units/{unitId}/abilities")]
[ApiController]
[Tags("Units")]
public class UnitAbilityController(ApplicationDbContext context, AbilityService abilityService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create an ability for a unit")]
    [ProducesResponseType<AbilityResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbilityResponseDto>> CreateAbility(
        [FromRoute] int unitId,
        [FromBody] CreateAbilityDto abilityData
    )
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        var newAbilityResult = abilityService.CreateAbility(abilityData);

        if (!newAbilityResult.IsSuccess) return this.ApiProblem(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        unit.Abilities.Add(newAbility);
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
    [EndpointSummary("Get all abilities for a unit")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities([FromRoute] int unitId)
    {
        var unit = await context.Units
            .AsNoTracking()
            .Include(u => u.Abilities)
            .FirstOrDefaultAsync(u => u.UnitId == unitId);

        if (unit is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        return Ok(
            unit.Abilities.Select(a => new AbilityResponseDto(
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
