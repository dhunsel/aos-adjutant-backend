using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Units;

[Route("api/units")]
[ApiController]
[Tags("Units")]
public class UnitController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("{unitId}")]
    [EndpointSummary("Get a unit by ID")]
    [ProducesResponseType<UnitResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnitResponse>> GetUnit([FromRoute] int unitId)
    {
        var unit = await context.Units.AsNoTracking().FirstOrDefaultAsync(u => u.UnitId == unitId);

        return unit is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."))
            : Ok(
                new UnitResponse(
                    unit.UnitId,
                    unit.Name,
                    unit.Health,
                    unit.Move,
                    unit.Save,
                    unit.Control,
                    unit.WardSave,
                    unit.FactionId,
                    unit.Version
                )
            );
    }

    [HttpPut("{unitId}")]
    [EndpointSummary("Update a unit")]
    [ProducesResponseType<UnitResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnitResponse>> UpdateUnit([FromRoute] int unitId, [FromBody] ChangeUnitDto unitData)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        if (unit.Version != unitData.Version)
            return this.ApiProblem(
                new AppError(ErrorCode.ConcurrencyError, "Unit was already modified in the background.")
            );

        var isDuplicate = await context.Units.AnyAsync(u =>
            u.Name == unitData.Name && u.FactionId == unit.FactionId && u.UnitId != unitId
        );
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Unit already exists."));

        unit.Name = unitData.Name;
        unit.Health = unitData.Health;
        unit.Move = unitData.Move;
        unit.Save = unitData.Save;
        unit.Control = unitData.Control;
        unit.WardSave = unitData.WardSave;

        await context.SaveChangesAsync();

        return Ok(
            new UnitResponse(
                unit.UnitId,
                unit.Name,
                unit.Health,
                unit.Move,
                unit.Save,
                unit.Control,
                unit.WardSave,
                unit.FactionId,
                unit.Version
            )
        );
    }

    [HttpDelete("{unitId}")]
    [EndpointSummary("Delete a unit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUnit([FromRoute] int unitId)
    {
        var unit = await context.Units.FindAsync(unitId);

        if (unit is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Unit not found."));

        context.Units.Remove(unit);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
