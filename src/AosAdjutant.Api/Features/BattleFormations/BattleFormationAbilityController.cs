using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.Abilities;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.BattleFormations;

[Route("api/battle-formations/{battleFormationId}/abilities")]
[ApiController]
[Tags("Battle Formations")]
public sealed class BattleFormationAbilityController(BattleFormationService battleFormationService) : ControllerBase
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
        var abilityResult = await battleFormationService.CreateBattleFormationAbility(battleFormationId, abilityData);
        return abilityResult.Match(
            a => CreatedAtAction(
                nameof(AbilityController.GetAbility),
                "Ability",
                new { abilityId = a.AbilityId },
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
    [EndpointSummary("Get all abilities for a battle formation")]
    [ProducesResponseType<List<AbilityResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AbilityResponseDto>>> GetAbilities([FromRoute] int battleFormationId)
    {
        var abilitiesResult = await battleFormationService.GetBattleFormationAbilities(battleFormationId);
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
