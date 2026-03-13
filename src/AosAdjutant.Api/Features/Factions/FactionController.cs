using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.BattleFormations;
using AosAdjutant.Api.Features.Units;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions")]
[ApiController]
[Tags("Factions")]
public class FactionController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a faction")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FactionResponseDto>> CreateFaction([FromBody] CreateFactionDto factionData)
    {
        // First check to catch duplicates. Race conditions could still occur, the call to saveChanges below will
        // throw an exception in that case. Ignore for now (won't occur in practice) but revisit in the future
        var isDuplicate = await context.Factions.AnyAsync(f => f.Name == factionData.Name);
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Faction already exists."));

        var newFaction = new Faction { Name = factionData.Name };

        context.Factions.Add(newFaction);
        await context.SaveChangesAsync();

        return Created(
            $"api/factions/{newFaction.FactionId}",
            new FactionResponseDto(newFaction.FactionId, newFaction.Name, newFaction.Version)
        );
    }

    [HttpGet]
    [EndpointSummary("Get all factions")]
    [ProducesResponseType<List<FactionResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FactionResponseDto>>> GetFactions()
    {
        var factions = await context.Factions
            .AsNoTracking()
            .Select(f => new FactionResponseDto(f.FactionId, f.Name, f.Version))
            .ToListAsync();
        return Ok(factions);
    }

    [HttpGet("{factionId}")]
    [EndpointSummary("Get a faction by ID")]
    [ProducesResponseType<FactionResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FactionResponseDto>> GetFaction([FromRoute] int factionId)
    {
        var faction = await context.Factions.AsNoTracking().FirstOrDefaultAsync(f => f.FactionId == factionId);

        return faction is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."))
            : Ok(new FactionResponseDto(faction.FactionId, faction.Name, faction.Version));
    }

    [HttpDelete("{factionId}")]
    [EndpointSummary("Delete a faction")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteFaction([FromRoute] int factionId)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        context.Factions.Remove(faction);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{factionId}/battle-formations")]
    [EndpointSummary("Create a battle formation under a faction")]
    [ProducesResponseType<BattleFormationResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BattleFormationResponseDto>> CreateBattleFormation(
        [FromRoute] int factionId,
        [FromBody] CreateBattleFormationDto battleFormationData
    )
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        var isDuplicate = await context.BattleFormations.AnyAsync(bf =>
            bf.Name == battleFormationData.Name && bf.FactionId == factionId
        );
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Battle formation already exists."));

        var newBattleFormation = new BattleFormation { Name = battleFormationData.Name, FactionId = factionId, };

        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.BattleFormations.Add(newBattleFormation);
        await context.SaveChangesAsync();

        return Created(
            $"api/battle-formations/{newBattleFormation.BattleFormationId}",
            new BattleFormationResponseDto(
                newBattleFormation.BattleFormationId,
                newBattleFormation.Name,
                newBattleFormation.FactionId,
                newBattleFormation.Version
            )
        );
    }

    [HttpGet("{factionId}/battle-formations")]
    [EndpointSummary("Get all battle formations for a faction")]
    [ProducesResponseType<List<BattleFormationResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<BattleFormationResponseDto>>> GetBattleFormations([FromRoute] int factionId)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        var battleFormations = await context.BattleFormations
            .AsNoTracking()
            .Where(bf => bf.FactionId == factionId)
            .Select(bf => new BattleFormationResponseDto(bf.BattleFormationId, bf.Name, bf.FactionId, bf.Version))
            .ToListAsync();
        return Ok(battleFormations);
    }

    [HttpPost("{factionId}/units")]
    [EndpointSummary("Create a unit under a faction")]
    [ProducesResponseType<UnitResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnitResponse>> CreateUnit(
        [FromRoute] int factionId,
        [FromBody] CreateUnitDto unitData
    )
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        var isDuplicate = await context.Units.AnyAsync(u => u.Name == unitData.Name && u.FactionId == factionId);
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Unit already exists."));

        var newUnit = new Unit
        {
            Name = unitData.Name,
            Health = unitData.Health,
            Move = unitData.Move,
            Save = unitData.Save,
            Control = unitData.Control,
            WardSave = unitData.WardSave,
            FactionId = factionId,
        };

        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.Units.Add(newUnit);
        await context.SaveChangesAsync();

        return Created(
            $"api/units/{newUnit.UnitId}",
            new UnitResponse(
                newUnit.UnitId,
                newUnit.Name,
                newUnit.Health,
                newUnit.Move,
                newUnit.Save,
                newUnit.Control,
                newUnit.WardSave,
                newUnit.FactionId,
                newUnit.Version
            )
        );
    }

    [HttpGet("{factionId}/units")]
    [EndpointSummary("Get all units for a faction")]
    [ProducesResponseType<List<UnitResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UnitResponse>>> GetUnits([FromRoute] int factionId)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        var units = await context.Units
            .AsNoTracking()
            .Where(u => u.FactionId == factionId)
            .Select(u => new UnitResponse(
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
            )
            .ToListAsync();
        return Ok(units);
    }

    [HttpGet("/throw")]
    public ActionResult Throw() => throw new Exception("Exception");
}
