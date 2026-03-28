#pragma warning disable MA0048
using System.ComponentModel.DataAnnotations;
using AosAdjutant.Api.Common;

namespace AosAdjutant.Api.Features.Abilities;

public enum TurnPhase
{
    [Display(Name = "Deployment Phase")] Deployment,
    [Display(Name = "Start Phase")] Start,
    [Display(Name = "Hero Phase")] Hero,
    [Display(Name = "Movement Phase")] Movement,
    [Display(Name = "Shooting Phase")] Shooting,
    [Display(Name = "Charge Phase")] Charge,
    [Display(Name = "Combat Phase")] Combat,
    [Display(Name = "End Phase")] End,
    [Display(Name = "Passive")] Passive,
}

public enum ActivationRestriction
{
    [Display(Name = "Once per turn (army)")]
    OnceTurnArmy,

    [Display(Name = "Once per battle round (army)")]
    OnceRoundArmy,

    [Display(Name = "Once per battle (army)")]
    OnceBattleArmy,

    [Display(Name = "Once per battle round")]
    OnceRound,

    [Display(Name = "Once per battle")] OnceBattle
}

public enum PlayerTurn
{
    [Display(Name = "Your")] YourTurn,
    [Display(Name = "Enemy")] EnemyTurn,
    [Display(Name = "Any")] AnyTurn
}

public sealed record AbilityData
{
    public required string Name { get; init; }
    public string? Reaction { get; init; }
    public string? Declaration { get; init; }
    public required string Effect { get; init; }
    public required TurnPhase Phase { get; init; }
    public ActivationRestriction? Restriction { get; init; }
    public PlayerTurn? Turn { get; init; }
    public required bool IsGeneric { get; init; }
}

public sealed class Ability
{
    public int AbilityId { get; set; }
    public required string Name { get; set; }
    public string? Reaction { get; set; }
    public string? Declaration { get; set; }
    public required string Effect { get; set; }
    public TurnPhase Phase { get; set; }
    public ActivationRestriction? Restriction { get; set; }
    public PlayerTurn? Turn { get; set; }
    public bool IsGeneric { get; set; }
    public uint Version { get; set; }

    public static Result<Ability> Create(AbilityData data)
    {
        var validationResult = ValidateAbility(data);

        if (!validationResult.IsSuccess) return Result<Ability>.Failure(validationResult.GetError);

        var ability = new Ability
        {
            Name = data.Name,
            Reaction = data.Reaction,
            Declaration = data.Declaration,
            Effect = data.Effect,
            Phase = data.Phase,
            Restriction = data.Restriction,
            Turn = data.Turn,
            IsGeneric = data.IsGeneric,
        };

        return Result<Ability>.Success(ability);
    }

    public Result ChangeAbility(AbilityData data)
    {
        var validationResult = ValidateAbility(data);

        if (!validationResult.IsSuccess) return Result.Failure(validationResult.GetError);

        Name = data.Name;
        Reaction = data.Reaction;
        Declaration = data.Declaration;
        Effect = data.Effect;
        Phase = data.Phase;
        Restriction = data.Restriction;
        Turn = data.Turn;

        return Result.Success();
    }

    private static Result ValidateAbility(AbilityData data)
    {
        if (data.Phase == TurnPhase.Passive && !(data.Reaction is null && data.Declaration is null &&
                                                 data.Restriction is null && data.Turn is null))
            return Result.Failure(
                new AppError(
                    ErrorCode.ValidationError,
                    "A passive ability cannot have a reaction/declaration/restriction/turn."
                )
            );

        if (data.Phase != TurnPhase.Passive && data.Declaration is null)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A non-passive ability must have a declaration.")
            );

        return Result.Success();
    }
}

public static class AbilityErrors
{
    public static readonly AppError NotFound = new(ErrorCode.NotFound, "Ability not found.");

    public static readonly AppError Concurrency = new(
        ErrorCode.ConcurrencyError,
        "Ability was already modified in the background."
    );
}
