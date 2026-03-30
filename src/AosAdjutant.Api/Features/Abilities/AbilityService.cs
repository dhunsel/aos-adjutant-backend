using AosAdjutant.Api.Common;
using AosAdjutant.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Abilities;

public sealed class AbilityService(ApplicationDbContext context, ILogger<AbilityService> logger)
{
    public async Task<Result<Ability>> CreateGenericAbility(CreateAbilityDto abilityData)
    {
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
                IsGeneric = true,
            }
        );

        if (!newAbilityResult.IsSuccess) return Result<Ability>.Failure(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        context.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        logger.Log_GenericAbilityCreated(newAbility.AbilityId);

        return Result<Ability>.Success(newAbility);
    }

    public async Task<Result<Ability>> GetAbility(int abilityId)
    {
        var ability = await context.Abilities.AsNoTracking().FirstOrDefaultAsync(a => a.AbilityId == abilityId);

        return ability is null ? Result<Ability>.Failure(AbilityErrors.NotFound) : Result<Ability>.Success(ability);
    }

    public async Task<Result<Ability>> UpdateAbility(int abilityId, ChangeAbilityDto abilityData)
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return Result<Ability>.Failure(AbilityErrors.NotFound);

        if (ability.Version != abilityData.Version)
        {
            logger.Log_AbilityConcurrencyError(abilityId, abilityData.Version);
            return Result<Ability>.Failure(AbilityErrors.Concurrency);
        }

        var changeResult = ability.ChangeAbility(
            new AbilityData
            {
                Name = abilityData.Name,
                Reaction = abilityData.Reaction,
                Declaration = abilityData.Declaration,
                Effect = abilityData.Effect,
                Phase = abilityData.Phase,
                Restriction = abilityData.Restriction,
                Turn = abilityData.Turn,
                IsGeneric = ability.IsGeneric,
            }
        );

        if (!changeResult.IsSuccess) return Result<Ability>.Failure(changeResult.GetError);

        await context.SaveChangesAsync();

        logger.Log_AbilityUpdated(abilityId);

        return Result<Ability>.Success(ability);
    }

    public async Task<Result> DeleteAbility(int abilityId)
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return Result.Failure(AbilityErrors.NotFound);

        context.Abilities.Remove(ability);
        await context.SaveChangesAsync();

        logger.Log_AbilityDeleted(abilityId);

        return Result.Success();
    }
}
