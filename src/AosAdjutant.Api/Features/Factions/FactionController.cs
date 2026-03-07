using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Factions;

[Route("api/factions")]
[ApiController]
public class FactionController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
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
    public async Task<ActionResult<List<FactionResponseDto>>> GetFactions()
    {
        var factions = await context.Factions
            .AsNoTracking()
            .Select(f => new FactionResponseDto(f.FactionId, f.Name, f.Version))
            .ToListAsync();
        return Ok(factions);
    }

    [HttpGet("{factionId}")]
    public async Task<ActionResult<FactionResponseDto>> GetFaction([FromRoute] int factionId)
    {
        var faction = await context.Factions.AsNoTracking().FirstOrDefaultAsync(f => f.FactionId == factionId);

        return faction is null
            ? this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."))
            : Ok(new FactionResponseDto(faction.FactionId, faction.Name, faction.Version));
    }

    [HttpPut("{factionId}")]
    public async Task<ActionResult<FactionResponseDto>> UpdateFaction(
        [FromRoute] int factionId,
        [FromBody] ChangeFactionDto factionData
    )
    {
        var faction = await context.Factions.FindAsync(factionId);

        // First check to catch duplicates/version conflicts. Race conditions could still occur, the call to saveChanges below will
        // throw an exception in that case. Ignore for now (won't occur in practice) but revisit in the future
        if (faction is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        if (faction.Version != factionData.Version)
            return this.ApiProblem(
                new AppError(ErrorCode.ConcurrencyError, "Faction was already modified in the background.")
            );

        var isDuplicate = await context.Factions.AnyAsync(f => f.Name == factionData.Name);
        if (isDuplicate)
            return this.ApiProblem(new AppError(ErrorCode.UniqueKeyError, "Faction already exists."));

        // context.Entry(faction).Property(f => f.Version).OriginalValue = factionData.Version; Check above guarantees that versions are equal
        faction.Name = factionData.Name;
        await context.SaveChangesAsync();

        return Ok(new FactionResponseDto(faction.FactionId, faction.Name, faction.Version));
    }

    [HttpDelete("{factionId}")]
    public async Task<ActionResult> DeleteFaction([FromRoute] int factionId)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return this.ApiProblem(new AppError(ErrorCode.NotFound, "Faction not found."));

        context.Factions.Remove(faction);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("/throw")]
    public ActionResult Throw() => throw new Exception("Exception");
}
