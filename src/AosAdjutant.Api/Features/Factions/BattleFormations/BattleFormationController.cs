using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Factions.BattleFormations;

[Route("api/factions/{factionId}/battle-formations")]
[ApiController]
public class BattleFormationController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
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
            $"api/factions/{factionId}/battle-formations/{newBattleFormation.BattleFormationId}",
            new BattleFormationResponseDto(
                newBattleFormation.BattleFormationId,
                newBattleFormation.Name,
                newBattleFormation.FactionId,
                newBattleFormation.Version
            )
        );
    }

    [HttpGet]
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

    [HttpGet("{battleFormationId}")]
    public async Task<ActionResult<BattleFormationResponseDto>> GetBattleFormation(
        [FromRoute] int factionId,
        [FromRoute] int battleFormationId
    )
    {
        var battleFormation = await context.BattleFormations
            .AsNoTracking()
            .FirstOrDefaultAsync(bf => bf.BattleFormationId == battleFormationId && bf.FactionId == factionId);

        return battleFormation is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Battle formation not found."))
            : Ok(
                new BattleFormationResponseDto(
                    battleFormation.BattleFormationId,
                    battleFormation.Name,
                    battleFormation.FactionId,
                    battleFormation.Version
                )
            );
    }

    [HttpPut("{battleFormationId}")]
    public async Task<ActionResult<BattleFormationResponseDto>> UpdateBattleFormation(
        [FromRoute] int factionId,
        [FromRoute] int battleFormationId,
        [FromBody] ChangeBattleFormationDto battleFormationData
    )
    {
        var battleFormation = await context.BattleFormations.FirstOrDefaultAsync(bf =>
            bf.BattleFormationId == battleFormationId && bf.FactionId == factionId
        );

        if (battleFormation is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Battle formation not found."));

        if (battleFormation.Version != battleFormationData.Version)
            return this.ApiProblem(
                new AppError(ErrorCode.ConcurrencyError, "Battle formation was already modified in the background.")
            );

        var isDuplicate = await context.BattleFormations.AnyAsync(bf =>
            bf.Name == battleFormationData.Name && bf.FactionId == factionId &&
            bf.BattleFormationId != battleFormationId
        );
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Battle formation already exists."));

        battleFormation.Name = battleFormationData.Name;
        await context.SaveChangesAsync();

        return Ok(
            new BattleFormationResponseDto(
                battleFormation.BattleFormationId,
                battleFormation.Name,
                battleFormation.FactionId,
                battleFormation.Version
            )
        );
    }

    [HttpDelete("{battleFormationId}")]
    public async Task<ActionResult> DeleteBattleFormation([FromRoute] int factionId, [FromRoute] int battleFormationId)
    {
        var battleFormation = await context.BattleFormations.FirstOrDefaultAsync(bf =>
            bf.BattleFormationId == battleFormationId && bf.FactionId == factionId
        );

        if (battleFormation is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Battle formation not found."));

        context.BattleFormations.Remove(battleFormation);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
