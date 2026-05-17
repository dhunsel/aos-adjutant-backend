using AosAdjutant.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Factions;

[Route("factions")]
[ApiController]
[Tags("Factions")]
public sealed class FactionController(FactionService factionService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "RequireAdmin")]
    [EndpointSummary("Create a faction")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FactionResponseDto>> CreateFaction(
        [FromBody] CreateFactionDto factionData
    )
    {
        var factionResult = await factionService.CreateFaction(factionData);
        return factionResult.Match(
            f =>
                CreatedAtAction(
                    nameof(GetFaction),
                    new { factionId = f.FactionId },
                    new FactionResponseDto(f.FactionId, f.Name, f.GrandAlliance, f.Version)
                ),
            this.ApiProblem
        );
    }

    [HttpGet]
    [Authorize]
    [EndpointSummary("Get all factions")]
    [ProducesResponseType<PaginatedResponse<FactionResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<FactionResponseDto>>> GetFactions(
        [FromQuery] FactionQuery factionQuery
    )
    {
        var factions = await factionService.GetFactions(factionQuery);
        return Ok(
            factions.Map(f => new FactionResponseDto(
                f.FactionId,
                f.Name,
                f.GrandAlliance,
                f.Version
            ))
        );
    }

    [HttpGet("{factionId:int}")]
    [Authorize]
    [EndpointSummary("Get a faction by ID")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FactionResponseDto>> GetFaction([FromRoute] int factionId)
    {
        var factionResult = await factionService.GetFaction(factionId);
        return factionResult.Match(
            f => Ok(new FactionResponseDto(f.FactionId, f.Name, f.GrandAlliance, f.Version)),
            this.ApiProblem
        );
    }

    [HttpPut("{factionId:int}")]
    [Authorize(Policy = "RequireAdmin")]
    [EndpointSummary("Update a faction")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FactionResponseDto>> UpdateFaction(
        [FromRoute] int factionId,
        [FromBody] ChangeFactionDto factionData
    )
    {
        var factionResult = await factionService.ChangeFaction(factionId, factionData);
        return factionResult.Match(
            f => Ok(new FactionResponseDto(f.FactionId, f.Name, f.GrandAlliance, f.Version)),
            this.ApiProblem
        );
    }

    [HttpDelete("{factionId:int}")]
    [Authorize(Policy = "RequireAdmin")]
    [EndpointSummary("Delete a faction")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteFaction([FromRoute] int factionId)
    {
        var deleteResult = await factionService.DeleteFaction(factionId);
        return deleteResult.Match(NoContent, this.ApiProblem);
    }
}
