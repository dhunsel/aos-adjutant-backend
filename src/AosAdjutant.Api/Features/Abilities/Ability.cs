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

    public static Result<Ability> Create(
        string name,
        string? reaction,
        string? declaration,
        string effect,
        TurnPhase phase,
        ActivationRestriction? restriction,
        PlayerTurn? turn,
        bool isGeneric
    )
    {
        var validationResult = ValidateAbility(name, reaction, declaration, effect, phase, restriction, turn);

        if (!validationResult.IsSuccess) return Result<Ability>.Failure(validationResult.GetError);

        var ability = new Ability
        {
            Name = name,
            Reaction = reaction,
            Declaration = declaration,
            Effect = effect,
            Phase = phase,
            Restriction = restriction,
            Turn = turn,
            IsGeneric = isGeneric
        };

        return Result<Ability>.Success(ability);
    }

    public Result ChangeAbility(
        string name,
        string? reaction,
        string? declaration,
        string effect,
        TurnPhase phase,
        ActivationRestriction? restriction,
        PlayerTurn? turn
    )
    {
        var validationResult = ValidateAbility(name, reaction, declaration, effect, phase, restriction, turn);

        if (!validationResult.IsSuccess) return Result.Failure(validationResult.GetError);

        Name = name;
        Reaction = reaction;
        Declaration = declaration;
        Effect = effect;
        Phase = phase;
        Restriction = restriction;
        Turn = turn;

        return Result.Success();
    }

    private static Result ValidateAbility(
        string name,
        string? reaction,
        string? declaration,
        string effect,
        TurnPhase phase,
        ActivationRestriction? restriction,
        PlayerTurn? turn
    )
    {
        if (phase == TurnPhase.Passive && !(reaction is null && declaration is null &&
                                            restriction is null && turn is null))
            return Result.Failure(
                new AppError(
                    ErrorCode.ValidationError,
                    "A passive ability cannot have a reaction/declaration/restriction/turn."
                )
            );

        if (phase != TurnPhase.Passive && declaration is null)
            return Result.Failure(
                new AppError(ErrorCode.ValidationError, "A non-passive ability must have a declaration.")
            );

        return Result.Success();
    }
}
