using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Abilities;

public class AbilityService(ApplicationDbContext context)
{
    public async Task<Result<Ability>> CreateGenericAbility(CreateAbilityDto abilityData)
    {
        var newAbilityResult = Ability.Create(
            abilityData.Name,
            abilityData.Reaction,
            abilityData.Declaration,
            abilityData.Effect,
            abilityData.Phase,
            abilityData.Restriction,
            abilityData.Turn,
            true
        );

        if (!newAbilityResult.IsSuccess) return Result<Ability>.Failure(newAbilityResult.GetError);

        var newAbility = newAbilityResult.GetValue;
        context.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        return Result<Ability>.Success(newAbility);
    }

    public async Task<Result<Ability>> GetAbility(int abilityId)
    {
        var ability = await context.Abilities.AsNoTracking().FirstOrDefaultAsync(a => a.AbilityId == abilityId);

        return ability is null
            ? Result<Ability>.Failure(new AppError(ErrorCode.NotFound, "Ability not found."))
            : Result<Ability>.Success(ability);
    }

    public async Task<Result<Ability>> UpdateAbility(int abilityId, ChangeAbilityDto abilityData)
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return Result<Ability>.Failure(new AppError(ErrorCode.NotFound, "Ability not found."));

        if (ability.Version != abilityData.Version)
            return Result<Ability>.Failure(
                new AppError(ErrorCode.ConcurrencyError, "Ability was already modified in the background.")
            );

        var changeResult = ability.ChangeAbility(
            abilityData.Name,
            abilityData.Reaction,
            abilityData.Declaration,
            abilityData.Effect,
            abilityData.Phase,
            abilityData.Restriction,
            abilityData.Turn
        );

        if (!changeResult.IsSuccess) return Result<Ability>.Failure(changeResult.GetError);

        await context.SaveChangesAsync();

        return Result<Ability>.Success(ability);
    }

    public async Task<Result> DeleteAbility(int abilityId)
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return Result.Failure(new AppError(ErrorCode.NotFound, "Ability not found."));

        context.Abilities.Remove(ability);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}
