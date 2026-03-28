using AosAdjutant.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions")]
[ApiController]
[Tags("Factions")]
public sealed class FactionController(FactionService factionService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a faction")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FactionResponseDto>> CreateFaction([FromBody] CreateFactionDto factionData)
    {
        var factionResult = await factionService.CreateFaction(factionData);
        return factionResult.Match(
            f => CreatedAtAction(
                nameof(GetFaction),
                new { factionId = f.FactionId },
                new FactionResponseDto(f.FactionId, f.Name, f.Version)
            ),
            this.ApiProblem
        );
    }

    [HttpGet]
    [EndpointSummary("Get all factions")]
    [ProducesResponseType<List<FactionResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FactionResponseDto>>> GetFactions()
    {
        var factions = await factionService.GetFactions();
        return Ok(factions.Select(f => new FactionResponseDto(f.FactionId, f.Name, f.Version)));
    }

    [HttpGet("{factionId}")]
    [EndpointSummary("Get a faction by ID")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FactionResponseDto>> GetFaction([FromRoute] int factionId)
    {
        var factionResult = await factionService.GetFaction(factionId);
        return factionResult.Match(f => Ok(new FactionResponseDto(f.FactionId, f.Name, f.Version)), this.ApiProblem);
    }

    [HttpPut("{factionId}")]
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
        return factionResult.Match(f => Ok(new FactionResponseDto(f.FactionId, f.Name, f.Version)), this.ApiProblem);
    }

    [HttpDelete("{factionId}")]
    [EndpointSummary("Delete a faction")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteFaction([FromRoute] int factionId)
    {
        var deleteResult = await factionService.DeleteFaction(factionId);
        return deleteResult.Match(NoContent, this.ApiProblem);
    }
}
