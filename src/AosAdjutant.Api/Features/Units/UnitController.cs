using AosAdjutant.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace AosAdjutant.Api.Features.Units;

[Route("api/units")]
[ApiController]
[Tags("Units")]
public sealed class UnitController(UnitService unitService) : ControllerBase
{
    [HttpGet("{unitId}")]
    [EndpointSummary("Get a unit by ID")]
    [ProducesResponseType<UnitResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnitResponseDto>> GetUnit([FromRoute] int unitId)
    {
        var unitResult = await unitService.GetUnit(unitId);
        return unitResult.Match(
            u => Ok(
                new UnitResponseDto(
                    u.UnitId,
                    u.Name,
                    u.Health,
                    u.Move,
                    u.Save,
                    u.Control,
                    u.WardSave,
                    u.FactionId,
                    u.Version
                )
            ),
            this.ApiProblem
        );
    }

    [HttpPut("{unitId}")]
    [EndpointSummary("Update a unit")]
    [ProducesResponseType<UnitResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnitResponseDto>> UpdateUnit(
        [FromRoute] int unitId,
        [FromBody] ChangeUnitDto unitData
    )
    {
        var unitResult = await unitService.UpdateUnit(unitId, unitData);
        return unitResult.Match(
            u => Ok(
                new UnitResponseDto(
                    u.UnitId,
                    u.Name,
                    u.Health,
                    u.Move,
                    u.Save,
                    u.Control,
                    u.WardSave,
                    u.FactionId,
                    u.Version
                )
            ),
            this.ApiProblem
        );
    }

    [HttpDelete("{unitId}")]
    [EndpointSummary("Delete a unit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUnit([FromRoute] int unitId)
    {
        var unitResult = await unitService.DeleteUnit(unitId);
        return unitResult.Match(NoContent, this.ApiProblem);
    }
}
