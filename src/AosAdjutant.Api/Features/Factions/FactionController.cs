using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.BattleFormations;
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
            $"api/factions/{factionId}/battle-formations/{newBattleFormation.BattleFormationId}",
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

    [HttpGet("/throw")]
    public ActionResult Throw() => throw new Exception("Exception");
}
