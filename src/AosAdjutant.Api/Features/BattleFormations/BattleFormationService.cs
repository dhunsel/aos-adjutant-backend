using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.BattleFormations;

public class BattleFormationService(ApplicationDbContext context)
{
    public async Task<Result<BattleFormation>> CreateBattleFormation(
        int factionId,
        CreateBattleFormationDto battleFormationData
    )
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<BattleFormation>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        var isDuplicate = await context.BattleFormations.AnyAsync(bf =>
            bf.Name == battleFormationData.Name && bf.FactionId == factionId
        );
        if (isDuplicate)
            return Result<BattleFormation>.Failure(
                new AppError(ErrorCode.UniqueKeyError, "Battle formation already exists.")
            );

        var newBattleFormation = new BattleFormation { Name = battleFormationData.Name, FactionId = factionId, };

        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.BattleFormations.Add(newBattleFormation);
        await context.SaveChangesAsync();

        return Result<BattleFormation>.Success(newBattleFormation);
    }

    public async Task<Result<List<BattleFormation>>> GetFactionBattleFormations(int factionId)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<List<BattleFormation>>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        var battleFormations = await context.BattleFormations
            .AsNoTracking()
            .Where(bf => bf.FactionId == factionId)
            .ToListAsync();
        return Result<List<BattleFormation>>.Success(battleFormations);
    }

    public async Task<Result<BattleFormation>> GetBattleFormation(int battleFormationId)
    {
        var battleFormation = await context.BattleFormations
            .AsNoTracking()
            .FirstOrDefaultAsync(bf => bf.BattleFormationId == battleFormationId);

        return battleFormation is null
            ? Result<BattleFormation>.Failure(new AppError(ErrorCode.NotFound, "Battle formation not found."))
            : Result<BattleFormation>.Success(battleFormation);
    }

    public async Task<Result<BattleFormation>> UpdateBattleFormation(
        int battleFormationId,
        ChangeBattleFormationDto battleFormationData
    )
    {
        var battleFormation = await context.BattleFormations.FindAsync(battleFormationId);

        if (battleFormation is null)
            return Result<BattleFormation>.Failure(new AppError(ErrorCode.NotFound, "Battle formation not found."));

        if (battleFormation.Version != battleFormationData.Version)
            return Result<BattleFormation>.Failure(
                new AppError(ErrorCode.ConcurrencyError, "Battle formation was already modified in the background.")
            );

        var isDuplicate = await context.BattleFormations.AnyAsync(bf =>
            bf.Name == battleFormationData.Name && bf.FactionId == battleFormation.FactionId &&
            bf.BattleFormationId != battleFormationId
        );
        if (isDuplicate)
            return Result<BattleFormation>.Failure(
                new AppError(ErrorCode.UniqueKeyError, "Battle formation already exists.")
            );

        battleFormation.Name = battleFormationData.Name;
        await context.SaveChangesAsync();

        return Result<BattleFormation>.Success(battleFormation);
    }

    public async Task<Result> DeleteBattleFormation(int battleFormationId)
    {
        var battleFormation = await context.BattleFormations.FindAsync(battleFormationId);

        if (battleFormation is null)
            return Result.Failure(new AppError(ErrorCode.NotFound, "Battle formation not found."));

        context.BattleFormations.Remove(battleFormation);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}
