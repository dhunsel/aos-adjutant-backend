using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using AosAdjutant.Api.Features.Abilities;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Factions;

public sealed class FactionService(ApplicationDbContext context)
{
    public async Task<Result<Faction>> CreateFaction(CreateFactionDto factionData)
    {
        // First check to catch duplicates. Race conditions could still occur, the call to saveChanges below will
        // throw an exception in that case. Ignore for now (won't occur in practice) but revisit in the future
        var isDuplicate = await context.Factions.AnyAsync(f => f.Name == factionData.Name);
        if (isDuplicate)
            return Result<Faction>.Failure(FactionErrors.AlreadyExists);

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

        return faction is null ? Result<Faction>.Failure(FactionErrors.NotFound) : Result<Faction>.Success(faction);
    }

    public async Task<Result<Faction>> ChangeFaction(int factionId, ChangeFactionDto factionData)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return Result<Faction>.Failure(FactionErrors.NotFound);

        if (faction.Version != factionData.Version)
            return Result<Faction>.Failure(FactionErrors.Concurrency);

        var isDuplicate = await context.Factions.AnyAsync(f => f.Name == factionData.Name && f.FactionId != factionId);
        if (isDuplicate)
            return Result<Faction>.Failure(FactionErrors.AlreadyExists);

        faction.Name = factionData.Name;
        await context.SaveChangesAsync();

        return Result<Faction>.Success(faction);
    }

    public async Task<Result> DeleteFaction(int factionId)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return Result.Failure(FactionErrors.NotFound);

        context.Factions.Remove(faction);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Ability>> CreateFactionAbility(int factionId, CreateAbilityDto abilityData)
    {
        var faction = await context.Factions.FindAsync(factionId);

        if (faction is null)
            return Result<Ability>.Failure(FactionErrors.NotFound);

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
                IsGeneric = false
            }
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
            ? Result<List<Ability>>.Failure(FactionErrors.NotFound)
            : Result<List<Ability>>.Success(faction.Abilities.ToList());
    }
}
