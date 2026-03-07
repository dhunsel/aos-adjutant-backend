using System.ComponentModel.DataAnnotations;

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
    [Display(Name = "Passive")] Passive
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

public class Ability
{
    public int AbilityId { get; set; }
    public required string Name { get; set; }
    public string? Reaction { get; set; }
    public string? Declaration { get; set; }
    public required string Effect { get; set; }
    public TurnPhase Phase { get; set; }
    public ActivationRestriction? Restriction { get; set; }
    public PlayerTurn? Turn { get; set; }
    public uint Version { get; set; }
}
