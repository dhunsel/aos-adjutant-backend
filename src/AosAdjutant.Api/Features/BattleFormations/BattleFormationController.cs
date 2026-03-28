using AosAdjutant.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.BattleFormations;

[Route("api/battle-formations")]
[ApiController]
[Tags("Battle Formations")]
public sealed class BattleFormationController(BattleFormationService battleFormationService) : ControllerBase
{
    [HttpGet("{battleFormationId}")]
    [EndpointSummary("Get a battle formation by ID")]
    [ProducesResponseType<BattleFormationResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BattleFormationResponseDto>> GetBattleFormation([FromRoute] int battleFormationId)
    {
        var battleFormationResult = await battleFormationService.GetBattleFormation(battleFormationId);
        return battleFormationResult.Match(
            bf => Ok(new BattleFormationResponseDto(bf.BattleFormationId, bf.Name, bf.FactionId, bf.Version)),
            this.ApiProblem
        );
    }

    [HttpPut("{battleFormationId}")]
    [EndpointSummary("Update a battle formation")]
    [ProducesResponseType<BattleFormationResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BattleFormationResponseDto>> UpdateBattleFormation(
        [FromRoute] int battleFormationId,
        [FromBody] ChangeBattleFormationDto battleFormationData
    )
    {
        var battleFormationResult =
            await battleFormationService.UpdateBattleFormation(battleFormationId, battleFormationData);
        return battleFormationResult.Match(
            bf => Ok(new BattleFormationResponseDto(bf.BattleFormationId, bf.Name, bf.FactionId, bf.Version)),
            this.ApiProblem
        );
    }

    [HttpDelete("{battleFormationId}")]
    [EndpointSummary("Delete a battle formation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBattleFormation([FromRoute] int battleFormationId)
    {
        var deleteResult = await battleFormationService.DeleteBattleFormation(battleFormationId);
        return deleteResult.Match(NoContent, this.ApiProblem);
    }
}
