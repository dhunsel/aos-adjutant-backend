using AosAdjutant.Api.Database;
using AosAdjutant.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace AosAdjutant.Api.Features.Abilities;

public class AbilityService(ApplicationDbContext context)
{
    public async Task<Result<AbilityResponseDto>> CreateGenericAbility(CreateAbilityDto abilityData)
    {
        var newAbility = new Ability
        {
            Name = abilityData.Name,
            Reaction = abilityData.Reaction,
            Declaration = abilityData.Declaration,
            Effect = abilityData.Effect,
            Phase = abilityData.Phase,
            Restriction = abilityData.Restriction,
            Turn = abilityData.Turn,
            IsGeneric = true
        };

        var validationResult = ValidateAbility(newAbility);
        if (!validationResult.IsSuccess) return Result<AbilityResponseDto>.Failure(validationResult.GetError);

        context.Abilities.Add(newAbility);
        await context.SaveChangesAsync();

        return Result<AbilityResponseDto>.Success(
            new AbilityResponseDto(
                newAbility.AbilityId,
                newAbility.Name,
                newAbility.Reaction,
                newAbility.Declaration,
                newAbility.Effect,
                newAbility.Phase,
                newAbility.Restriction,
                newAbility.Turn,
                newAbility.Version
            )
        );
    }

    public async Task<Result<AbilityResponseDto>> GetAbility(int abilityId)
    {
        var ability = await context.Abilities.AsNoTracking().FirstOrDefaultAsync(a => a.AbilityId == abilityId);

        return ability is null
            ? Result<AbilityResponseDto>.Failure(new AppError(ErrorCode.NotFound, "Ability not found."))
            : Result<AbilityResponseDto>.Success(
                new AbilityResponseDto(
                    ability.AbilityId,
                    ability.Name,
                    ability.Reaction,
                    ability.Declaration,
                    ability.Effect,
                    ability.Phase,
                    ability.Restriction,
                    ability.Turn,
                    ability.Version
                )
            );
    }

    public async Task<Result<AbilityResponseDto>> UpdateAbility(int abilityId, ChangeAbilityDto abilityData)
    {
        var ability = await context.Abilities.FindAsync(abilityId);

        if (ability is null)
            return Result<AbilityResponseDto>.Failure(new AppError(ErrorCode.NotFound, "Ability not found."));

        if (ability.Version != abilityData.Version)
            return Result<AbilityResponseDto>.Failure(
                new AppError(ErrorCode.ConcurrencyError, "Ability was already modified in the background.")
            );

        ability.Name = abilityData.Name;
        ability.Reaction = abilityData.Reaction;
        ability.Declaration = abilityData.Declaration;
        ability.Effect = abilityData.Effect;
        ability.Phase = abilityData.Phase;
        ability.Restriction = abilityData.Restriction;
        ability.Turn = abilityData.Turn;

        var validationResult = ValidateAbility(ability);
        if (!validationResult.IsSuccess) return Result<AbilityResponseDto>.Failure(validationResult.GetError);

        await context.SaveChangesAsync();

        return Result<AbilityResponseDto>.Success(
            new AbilityResponseDto(
                ability.AbilityId,
                ability.Name,
                ability.Reaction,
                ability.Declaration,
                ability.Effect,
                ability.Phase,
                ability.Restriction,
                ability.Turn,
                ability.Version
            )
        );
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

    public Result<Ability> CreateAbility(CreateAbilityDto abilityData)
    {
        var newAbility = new Ability
        {
            Name = abilityData.Name,
            Reaction = abilityData.Reaction,
            Declaration = abilityData.Declaration,
            Effect = abilityData.Effect,
            Phase = abilityData.Phase,
            Restriction = abilityData.Restriction,
            Turn = abilityData.Turn,
            IsGeneric = false
        };

        var validationResult = ValidateAbility(newAbility);

        return validationResult.IsSuccess
            ? Result<Ability>.Success(newAbility)
            : Result<Ability>.Failure(validationResult.GetError);
    }

    private Result ValidateAbility(Ability ability)
    {
        if (ability.Phase == TurnPhase.Passive && !(ability.Reaction is null && ability.Declaration is null &&
                                                    ability.Restriction is null && ability.Turn is null))
            return Result.Failure(
                new AppError(
                    ErrorCode.ValidationError,
                    "A passive ability cannot have a reaction/declaration/restriction/turn."
                )
            );

        if (ability.Phase != TurnPhase.Passive && ability.Declaration is null)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A non-passive ability must have a declaration.")
            );

        return Result.Success();
    }
}
