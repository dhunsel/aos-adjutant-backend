using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Features.Factions;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.BattleFormations;

public sealed class BattleFormationService(ApplicationDbContext context, ILogger<BattleFormationService> logger)
{
    public async Task<Result<BattleFormation>> CreateBattleFormation(
        int factionId,
        CreateBattleFormationDto battleFormationData
    )
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<BattleFormation>.Failure(FactionErrors.NotFound);

        var isDuplicate = await context.BattleFormations.AnyAsync(bf =>
            bf.Name == battleFormationData.Name && bf.FactionId == factionId
        );
        if (isDuplicate)
            return Result<BattleFormation>.Failure(BattleFormationErrors.AlreadyExists);

        var newBattleFormation = new BattleFormation { Name = battleFormationData.Name, FactionId = factionId, };

        // Because of race conditions this might still fail on UK/FK error
        // Ignore for now (won't occur in practice) but revisit in the future
        context.BattleFormations.Add(newBattleFormation);
        await context.SaveChangesAsync();

        logger.Log_BattleFormationCreated(newBattleFormation.BattleFormationId, factionId);

        return Result<BattleFormation>.Success(newBattleFormation);
    }

    public async Task<Result<List<BattleFormation>>> GetFactionBattleFormations(int factionId)
    {
        var factionExists = await context.Factions.AnyAsync(f => f.FactionId == factionId);
        if (!factionExists)
            return Result<List<BattleFormation>>.Failure(FactionErrors.NotFound);

        var battleFormations = await context.BattleFormations.AsNoTracking()
            .Where(bf => bf.FactionId == factionId)
            .ToListAsync();
        return Result<List<BattleFormation>>.Success(battleFormations);
    }

    public async Task<Result<BattleFormation>> GetBattleFormation(int battleFormationId)
    {
        var battleFormation = await context.BattleFormations.AsNoTracking()
            .FirstOrDefaultAsync(bf => bf.BattleFormationId == battleFormationId);

        return battleFormation is null
            ? Result<BattleFormation>.Failure(BattleFormationErrors.NotFound)
            : Result<BattleFormation>.Success(battleFormation);
    }

    public async Task<Result<BattleFormation>> UpdateBattleFormation(
        int battleFormationId,
        ChangeBattleFormationDto battleFormationData
    )
    {
        var battleFormation = await context.BattleFormations.FindAsync(battleFormationId);

        if (battleFormation is null)
            return Result<BattleFormation>.Failure(BattleFormationErrors.NotFound);

        if (battleFormation.Version != battleFormationData.Version)
        {
            logger.Log_BattleFormationConcurrencyError(battleFormationId, battleFormationData.Version);
            return Result<BattleFormation>.Failure(BattleFormationErrors.Concurrency);
        }

        var isDuplicate = await context.BattleFormations.AnyAsync(bf =>
            bf.Name == battleFormationData.Name && bf.FactionId == battleFormation.FactionId &&
            bf.BattleFormationId != battleFormationId
        );
        if (isDuplicate)
            return Result<BattleFormation>.Failure(BattleFormationErrors.AlreadyExists);

        battleFormation.Name = battleFormationData.Name;
        await context.SaveChangesAsync();

        logger.Log_BattleFormationUpdated(battleFormationId, battleFormation.FactionId);

        return Result<BattleFormation>.Success(battleFormation);
    }

    public async Task<Result> DeleteBattleFormation(int battleFormationId)
    {
        var battleFormation = await context.BattleFormations.FindAsync(battleFormationId);

        if (battleFormation is null)
            return Result.Failure(BattleFormationErrors.NotFound);

        context.BattleFormations.Remove(battleFormation);
        await context.SaveChangesAsync();

        logger.Log_BattleFormationDeleted(battleFormationId);

        return Result.Success();
    }

    public async Task<Result<Ability>> CreateBattleFormationAbility(int battleFormationId, CreateAbilityDto abilityData)
    {
        var battleFormation = await context.BattleFormations.FindAsync(battleFormationId);

        if (battleFormation is null)
            return Result<Ability>.Failure(BattleFormationErrors.NotFound);

        var newAbilityResult = Ability.Create(
            new AbilityData
            {
                Name = abilityData.Name,
                Reaction = abilityData.Reaction,
                Declaration = abilityData.Declaration,
                Effect = abilityData.Effect,
                Phase = abilityData.Phase,
                Restriction = abilityData.Restriction,
                Turn = abilityData.Turn,
                IsGeneric = false,
            }
        );

        if (!newAbilityResult.IsSuccess) return Result<Ability>.Failure(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        battleFormation.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        logger.Log_ScopedAbilityCreated(newAbility.AbilityId, nameof(BattleFormation), battleFormationId);

        return Result<Ability>.Success(newAbility);
    }

    public async Task<Result<List<Ability>>> GetBattleFormationAbilities(int battleFormationId)
    {
        var battleFormation = await context.BattleFormations.AsNoTracking()
            .Include(bf => bf.Abilities)
            .FirstOrDefaultAsync(bf => bf.BattleFormationId == battleFormationId);

        return battleFormation is null
            ? Result<List<Ability>>.Failure(BattleFormationErrors.NotFound)
            : Result<List<Ability>>.Success(battleFormation.Abilities.ToList());
    }
}
