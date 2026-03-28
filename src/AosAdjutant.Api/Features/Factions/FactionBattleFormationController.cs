using AosAdjutant.Api.Common;
using AosAdjutant.Api.Features.BattleFormations;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions/{factionId}/battle-formations")]
[ApiController]
[Tags("Battle Formations")]
public sealed class FactionBattleFormationController(BattleFormationService battleFormationService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a battle formation under a faction")]
    [ProducesResponseType<BattleFormationResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BattleFormationResponseDto>> CreateBattleFormation(
        [FromRoute] int factionId,
        [FromBody] CreateBattleFormationDto battleFormationData
    )
    {
        var battleFormationResult = await battleFormationService.CreateBattleFormation(factionId, battleFormationData);
        return battleFormationResult.Match(
            bf => CreatedAtAction(
                nameof(BattleFormationController.GetBattleFormation),
                "BattleFormation",
                new { battleFormationId = bf.BattleFormationId },
                new BattleFormationResponseDto(bf.BattleFormationId, bf.Name, bf.FactionId, bf.Version)
            ),
            this.ApiProblem
        );
    }

    [HttpGet]
    [EndpointSummary("Get all battle formations for a faction")]
    [ProducesResponseType<List<BattleFormationResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<BattleFormationResponseDto>>> GetBattleFormations([FromRoute] int factionId)
    {
        var battleFormationsResult = await battleFormationService.GetFactionBattleFormations(factionId);
        return battleFormationsResult.Match(
            battleFormations => Ok(
                battleFormations.Select(bf => new BattleFormationResponseDto(
                        bf.BattleFormationId,
                        bf.Name,
                        bf.FactionId,
                        bf.Version
                    )
                )
            ),
            this.ApiProblem
        );
    }
}
