using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using AosAdjutant.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Factions;

public class FactionService(ApplicationDbContext context)
{
    public async Task<Result<Faction>> CreateFaction(CreateFactionDto factionData)
    {
        // First check to catch duplicates. Race conditions could still occur, the call to saveChanges below will
        // throw an exception in that case. Ignore for now (won't occur in practice) but revisit in the future
        var isDuplicate = await context.Factions.AnyAsync(f => f.Name == factionData.Name);
        if (isDuplicate)
            return Result<Faction>.Failure(new AppError(ErrorCode.UniqueKeyError, "Faction already exists."));

        var newFaction = new Faction { Name = factionData.Name };

        context.Factions.Add(newFaction);
        await context.SaveChangesAsync();

        return Result<Faction>.Success(newFaction);
    }

    public async Task<List<Faction>> GetFactions()
    {
        return await context.Factions.AsNoTracking().ToListAsync();
    }

    public async Task<Result<Faction>> GetFaction(int factionId)
    {
        var faction = await context.Factions.AsNoTracking().FirstOrDefaultAsync(f => f.FactionId == factionId);

        return faction is null
            ? Result<Faction>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."))
            : Result<Faction>.Success(faction);
    }

    public async Task<Result<Faction>> ChangeFaction(int factionId, ChangeFactionDto factionData)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return Result<Faction>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        if (faction.Version != factionData.Version)
            return Result<Faction>.Failure(
                new AppError(ErrorCode.ConcurrencyError, "Faction was already modified in the background.")
            );

        var isDuplicate = await context.Factions.AnyAsync(f => f.Name == factionData.Name && f.FactionId != factionId);
        if (isDuplicate)
            return Result<Faction>.Failure(new AppError(ErrorCode.UniqueKeyError, "Faction already exists."));

        faction.Name = factionData.Name;
        await context.SaveChangesAsync();

        return Result<Faction>.Success(faction);
    }

    public async Task<Result> DeleteFaction(int factionId)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return Result.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        context.Factions.Remove(faction);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Ability>> CreateFactionAbility(int factionId, CreateAbilityDto abilityData)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return Result<Ability>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."));

        var newAbilityResult = Ability.Create(
            abilityData.Name,
            abilityData.Reaction,
            abilityData.Declaration,
            abilityData.Effect,
            abilityData.Phase,
            abilityData.Restriction,
            abilityData.Turn,
            false
        );

        if (!newAbilityResult.IsSuccess) return Result<Ability>.Failure(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        faction.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        return Result<Ability>.Success(newAbility);
    }

    public async Task<Result<List<Ability>>> GetFactionAbilities(int factionId)
    {
        var faction = await context.Factions
            .AsNoTracking()
            .Include(f => f.Abilities)
            .FirstOrDefaultAsync(f => f.FactionId == factionId);

        return faction is null
            ? Result<List<Ability>>.Failure(new AppError(ErrorCode.NotFound, "Faction not found."))
            : Result<List<Ability>>.Success(faction.Abilities.ToList());
    }
}
