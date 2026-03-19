using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Units;

[Route("api/units/{unitId}/abilities")]
[ApiController]
[Tags("Units")]
public class UnitAbilityController(UnitService unitService) : ControllerBase
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
        var abilityResult = await unitService.CreateUnitAbility(unitId, abilityData);
        return abilityResult.Match(
            a => Created(
                $"api/abilities/{a.AbilityId}",
                new AbilityResponseDto(
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
            ),
            this.ApiProblem
        );
    }

    [HttpGet]
    [EndpointSummary("Get all abilities for a unit")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities([FromRoute] int unitId)
    {
        var abilitiesResult = await unitService.GetUnitAbilities(unitId);
        return abilitiesResult.Match(
            abilities => Ok(
                abilities.Select(a => new AbilityResponseDto(
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
            ),
            this.ApiProblem
        );
    }
}
